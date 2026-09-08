using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The ring-achievement builder: the center arrives as the end cell (same contract as the point
    /// one), the radii are the feature's authored values. Cheap: the DIM default covers both surfaces.
    /// </summary>
    public class RingAchivementBuilder : IAchivementBuilder
    {
        private readonly IGridMap3D _map3D;
        private readonly float _minDistance;
        private readonly float _maxDistance;

        public RingAchivementBuilder(IGridMap3D map3D, float minDistance, float maxDistance)
        {
            _map3D = map3D;
            _minDistance = Mathf.Min(minDistance, maxDistance);
            _maxDistance = Mathf.Max(minDistance, maxDistance);
        }

        public IAchivementHandler Create(Config start, Config end)
        {
            int endId = _map3D.Cell3DToId(end);
            return new RingAchivement3D(_map3D, endId, _minDistance, _maxDistance);
        }
    }

    /// <summary>
    /// Any cell of the [min..max] METER annulus around the center counts. Heading is dropped on
    /// purpose — same as the point achievement: the ring is a zone, facing is a pass's call. Inside
    /// the hole (closer than min) is NOT achieved. Meters, not cells: the comparison runs in world
    /// coordinates.
    /// </summary>
    public class RingAchivement3D : IAchivementHandler
    {
        private readonly IGridMap3D _grid;
        private readonly Vector2 _center;
        private readonly float _min, _max;

        public RingAchivement3D(IGridMap3D grid, int target, float minDistance, float maxDistance)
        {
            _grid = grid;
            _center = grid.Cell3DToWorld(grid.IdToCell3D(target)).Position;
            _min = minDistance;
            _max = maxDistance;
        }

        public bool Achieved(int point)
        {
            Vector2 pos = _grid.Cell3DToWorld(_grid.IdToCell3D(point)).Position;
            float d = Vector2.Distance(pos, _center);
            return d >= _min && d <= _max;
        }
    }
}
