using ClothoidX;
using NCPF.Domain;
using NCPF.Shared.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DomainConfig = NCPF.Domain.Config;

namespace NCPF.Config
{
    /// <summary>
    /// Stores the geometric control set by cubic curvature coefficients: each path
    /// is (A,B,C,D,L) plus its End node. A clothoid is the degenerate cubic (A=B=0)
    /// and serialises through the same store. Only a quarter of the lattice is
    /// stored, the other three quadrants are rebuilt natively on read; read
    /// returns pure shapes, dynamics is the Pipeline assembly's business.
    /// </summary>
    [CreateAssetMenu(fileName = "FuncControlSetContainer", menuName = "NCPF/Containers/FuncControlSetContainer")]
    public class FuncControlSetContainer : ControlSetAsset
    {
        [SerializeField] private List<Layer> _layers = new List<Layer>();
        [Serializable]
        public class LayerContainer
        {
            public List<Layer> Layers;

            public LayerContainer(List<Layer> layers)
            {
                Layers = layers;
            }
        }
        public int LayersCount => _layers.Count;
        public int AngleCount => _layers.Max(layer => layer.Angle) + 1;
        [ContextMenu("SaveToJson")]
        public void SaveToJson()
        {
            LayerContainer container = new LayerContainer(_layers);
            string json = JsonUtility.ToJson(container);
            File.WriteAllText(UnityEngine.Application.dataPath + "/FuncControlSetConfig.json", json);
        }

        [ContextMenu("ReadFromJson")]
        public void ReadFromJson()
        {
            string json = File.ReadAllText(UnityEngine.Application.dataPath + "/FuncControlSetConfig.json");
            LayerContainer container = JsonUtility.FromJson<LayerContainer>(json);
            _layers = container.Layers;
        }

        public override void Write(ControlSet<IPath> controlSet, IGridGeometry3D space)
        {
            _layers.Clear();

            int[] keys = controlSet.GetLayers();

            foreach (int layerAngle in keys)
            {
                IPath[] paths = controlSet.GetLayerSet(layerAngle);

                PathConfig[] configs = new PathConfig[paths.Length];

                for (int i = 0; i < paths.Length; i++)
                {
                    IPath orig = paths[i];
                    if (orig is not IPolynomialPath polynomial)
                    {
                        Debug.LogError($"FuncControlSetContainer stores IPolynomialPath (cubic/clothoid) only; got {orig?.GetType().Name}");
                        return;
                    }
                    configs[i] = new PathConfig(
                        polynomial.A, polynomial.B, polynomial.C, polynomial.D,
                        orig.Length,
                        space.WorldToCell3D(orig.Evaluate(orig.Length)),
                        orig is IRearFirstPath);
                }

                _layers.Add(new Layer(layerAngle, configs));
            }
        }

        public override ControlSet<IPath> Read(IGridGeometry3D space)
        {
            EnsureLoaded();
            ControlSet<IPath> controlSet = new ControlSet<IPath>();

            int fullAngle = QuarterSymmetry.FullAngleCount(space);
            int quarterAngle = fullAngle / 4;

            for (int i = 0; i < _layers.Count; i++)
            {
                Layer layer = _layers[i];
                for (int q = 0; q < 4; q++)
                {
                    int angle = q * quarterAngle + layer.Angle;
                    controlSet.LoadLayer(
                        angle,
                        BuildRotatedLayer(layer, q, fullAngle, space));
                }
            }

            return controlSet;
        }

        public override IPath[] ReadLayer(int angle, IGridGeometry3D space)
        {
            EnsureLoaded();
            IPath[] layer = ReadLayerDirty(angle, space);

            if (layer == null)
            {
                ReadFromJson();
                layer = ReadLayerDirty(angle, space);
            }
            return layer;
        }

        private IPath[] ReadLayerDirty(int angle, IGridGeometry3D space)
        {
            int fullAngle = QuarterSymmetry.FullAngleCount(space);
            int quarterAngle = Mathf.Max(1, fullAngle / 4);
            int quadrant = angle / quarterAngle;
            int localAngle = angle % quarterAngle;

            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Angle == localAngle)
                {
                    return BuildRotatedLayer(_layers[i], quadrant, fullAngle, space);
                }
            }
            return null;
        }

        private void EnsureLoaded()
        {
            if (_layers == null || _layers.Count == 0)
                ReadFromJson();
        }

        /// <summary>
        /// The layer stores forward and rear-first entries interleaved — the
        /// rear-first ones are baked natively (BackwardLayerBuilder) and are not
        /// synthesized on read: the mirror drove their ends off the cell centres
        /// on every layer but the 45° ones, tearing the path at junctions.
        /// </summary>
        private IPath[] BuildRotatedLayer(Layer layer, int quadrant, int fullAngle, IGridGeometry3D space)
        {
            int startAngle = quadrant * (fullAngle / 4) + layer.Angle;
            WorldConfig start = space.Cell3DToWorld(new DomainConfig(0, 0, startAngle));

            IPath[] paths = new IPath[layer.Paths.Length];
            for (int j = 0; j < layer.Paths.Length; j++)
            {
                paths[j] = BuildFunctionPath(layer.Paths[j], start, quadrant, fullAngle, space);
            }
            return paths;
        }

        /// <summary>
        /// A rear-first entry stores TANGENTS (heading+180) — the solver and the
        /// integration run on those; the RearFirstPath wrapper takes the 180°
        /// back off into the nose-heading channel. A clothoid (A=B=0) is rebuilt
        /// as the exact Euler-spiral segment the G1 solver produced, so sampling
        /// is bit-identical to the bake; a genuine cubic spiral has no closed
        /// form and is integrated finely (step ∝ length) instead.
        /// </summary>
        private IPath BuildFunctionPath(PathConfig config, WorldConfig start, int quadrant, int fullAngle, IGridGeometry3D space)
        {
            DomainConfig endCell = QuarterSymmetry.RotateCell(config.End, quadrant, fullAngle);
            WorldConfig end = space.Cell3DToWorld(endCell);

            if (config.Backward)
            {
                start = new WorldConfig(start.Position, Mathf.Repeat(start.Angle + 180f, 360f));
                end = new WorldConfig(end.Position, Mathf.Repeat(end.Angle + 180f, 360f));
            }

            IPolynomialPath path;
            if (Mathf.Abs(config.A) < 1e-9f && Mathf.Abs(config.B) < 1e-9f)
            {
                ClothoidSegment segment = new ClothoidSegment(
                    new System.Numerics.Vector3(start.X, 0f, start.Y),
                    (start.Angle + 90.0) * Mathf.Deg2Rad,
                    config.D,
                    config.C,
                    Mathf.Max(1e-4f, config.L),
                    SolutionType.BERTOLAZZIFREGO);
                path = new ClothoidPath(segment, start, end);
            }
            else
            {
                float minStep = Mathf.Max(0.005f, config.L / 64f);
                path = new PolynomialBasedPath(
                    new CubicPolyniomFunction(config.A, config.B, config.C, config.D, config.L),
                    start,
                    end,
                    minStep);
            }

            return config.Backward ? new RearFirstPath(path) : path;
        }

        public int GetLayerPathCount(int angle)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Angle == angle)
                {
                    return _layers[i].Paths.Length;
                }
            }
            return 0;
        }

        public bool HasLayer(int angle)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                if (_layers[i].Angle == angle)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetTotalPathCount()
        {
            int count = 0;

            for (int i = 0; i < _layers.Count; i++)
            {
                count += _layers[i].Paths.Length;
            }

            return count;
        }

        /// <summary>
        /// One baked primitive: cubic coefficients (A..D), length L and the End
        /// node. Backward marks a rear-first entry baked with heading+180
        /// tangents; legacy stores without the field read as forward.
        /// </summary>
        [Serializable]
        public struct PathConfig
        {
            public float A;
            public float B;
            public float C;
            public float D;
            public float L;
            public DomainConfig End;
            public bool Backward;
            public PathConfig(float a, float b, float c, float d, float l, DomainConfig end, bool backward = false)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                L = l;
                End = end;
                Backward = backward;
            }
        }

        /// <summary>
        /// One stored heading layer of the baked quarter.
        /// </summary>
        [Serializable]
        public struct Layer
        {
            public int Angle;
            public PathConfig[] Paths;
            public DomainConfig Start => new DomainConfig(0, 0, Angle);
            public Layer(int angle, PathConfig[] paths)
            {
                Angle = angle;
                Paths = paths;
            }
        }
    }
}
