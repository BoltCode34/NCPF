using NCPF.Domain;
using NCPF.Shared;
using UnityEngine;

namespace NCPF.Bake.Presentation
{
    /// <summary>
    /// Bakes the C-space map from the 2D occupancy grid and writes it into the
    /// map asset through the port.
    /// </summary>
    public class NCPFMapBuilder : MonoBehaviour
    {
        public MapAsset _map;
        public AgentConfig _agent;
        public Unity2DGrid _grid;
        public int AngleStepCount;
        public bool _buildOnAwake;
        public void Awake()
        {
            if (_buildOnAwake)
            {
                Build();
            }
        }

        [ContextMenu("Build")]
        public void Build()
        {
            _grid.Bake();
            bool[,] field = new bool[_grid.Size.x, _grid.Size.y];
            for (int x = 0; x < _grid.Size.x; x++)
            {
                for (int y = 0; y < _grid.Size.y; y++)
                {
                    field[x, y] = !_grid.CellFree(x, y);
                }
            }
            MapBuilder builder = new MapBuilder(new Map(field, _grid.CellSize), _agent.GetAgent(_grid.CellSize));
            Map3D map = builder.BuildMap(AngleStepCount);
            _map.Write(map);
        }
    }
}
