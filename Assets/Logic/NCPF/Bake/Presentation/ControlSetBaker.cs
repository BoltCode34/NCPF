using NCPF.Bake.Application;
using NCPF.Domain;
using NCPF.Shared.Presentation;
using UnityEngine;

namespace NCPF.Bake.Presentation
{
    /// <summary>
    /// The bake root on the sandbox scene: reads the map through the port, runs
    /// the bake service and writes the control set back through the port.
    /// ContextMenu "Bake" bakes the stored quarter, "Bake Layer" a single
    /// heading layer.
    /// </summary>
    public class ControlSetBaker : MonoBehaviour
    {
        [SerializeField] private MapAsset _mapContainer;
        [SerializeField] private ControlSetAsset _controlSetContainer;
        [SerializeField] private Unity2DGrid _grid;
        [SerializeField] private bool _draw;
        [SerializeField] private int _angleLayer;
        [SerializeField] private float _maxRadius = 1;
        [SerializeField] private float _minTurnRadius = 0f;
        [SerializeField, Min(0f)] private float _positionTube = 0.25f;
        [SerializeField, Min(0f)] private float _angleTube = 10f;
        [SerializeField, Min(1f)] private float _lengthMultiplier = 1.4f;
        private ControlSetBakeService _bakeService;
        private ControlSet<IPath> _controlSet;

        /// <summary>External DI seam: swap the bake service (and its <see cref="IPathBuilder"/>) before the first Bake call.</summary>
        public void Construct(ControlSetBakeService bakeService)
        {
            _bakeService = bakeService;
        }

        [ContextMenu("Bake")]
        public void Bake()
        {
            if (_mapContainer == null || _grid == null || _controlSetContainer == null) return;
            if (_bakeService == null) _bakeService = new ControlSetBakeService(new ClothoidPathBuilder());
            _grid.Bake();
            SpacedMap3D map3D = new SpacedMap3D(_grid, _mapContainer.Read());
            _controlSet = _bakeService.Bake(map3D, _maxRadius, _lengthMultiplier, _positionTube, _angleTube, _minTurnRadius);
            _controlSetContainer.Write(_controlSet, map3D);
        }

        [ContextMenu("Bake Layer")]
        public void BakeLayer()
        {
            if (_mapContainer == null || _grid == null || _controlSetContainer == null) return;
            if (_bakeService == null) _bakeService = new ControlSetBakeService(new ClothoidPathBuilder());
            _grid.Bake();
            SpacedMap3D map3D = new SpacedMap3D(_grid, _mapContainer.Read());
            IPath[] layer = _bakeService.BakeLayer(map3D, _angleLayer, _maxRadius, _lengthMultiplier, _positionTube, _angleTube, _minTurnRadius);
            _controlSet = new ControlSet<IPath>();
            _controlSet.LoadLayer(_angleLayer, layer);
            _controlSetContainer.Write(_controlSet, map3D);
        }

        private void OnDrawGizmos()
        {
            if (!_draw) return;
            if (_grid == null || _mapContainer == null) return;
            SpacedMap3D map3D = new SpacedMap3D(_grid, _mapContainer.Read());
            WorldConfig center = (map3D.Cell3DToWorld(new Config(map3D.WorldToCell(Vector2.zero), 0)) - map3D.Cell3DToWorld(new Config(0, 0, 0)));
            IPath[] pathes;
            if (_controlSet != null)
            {
                pathes = _controlSet.GetLayerSet(_angleLayer);
            }
            else
            {
                pathes = _controlSetContainer.ReadLayer(_angleLayer, map3D);
            }
            if (pathes == null) return;

            for (int j = 0; j < pathes.Length; j++)
            {
                IPath path = pathes[j];
                Gizmos.color = Color.red;
                float step = path.Length / 100;
                Vector2 start = center.Position+ path.Evaluate(0).Position;
                for (int i = 1; i <= 100; i++)
                {
                    WorldConfig point = center+ path.Evaluate(step * i) ;
                    Gizmos.DrawLine(start, point.Position);
                    start = point.Position;
                }
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(center.Position+ path.Evaluate(path.Length).Position, _positionTube);
            }
        }
    }
}
