using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Limiter log decorator for logging path limit checks. Samples are compared
    /// to lattice nodes by POSE only (position + angle tube from grid resolution).
    /// </summary>
    public class LimiterLog : PathLimiterBase
    {
        private readonly IControlSetBuildLogger _logger;
        private LimiterPathLog _log = new();
        private bool _canBeDividedChecked;

        public LimiterLog(IControlSetBuildLogger logger,
            float breakDistance = 1.4f,
            float positionTube = 0.25f,
            float angleTube = 10f) : base(breakDistance, positionTube, angleTube)
        {
            _logger = logger;
            _log = new();
            _log.OtherInfo.NearestPoints = new();
        }

        public override bool CanBeDivided(
            IGridGeometry3D space,
            HashSet<Config> states,
            IPath path)
        {
            bool res = base.CanBeDivided(space, states, path);
            if (!_canBeDividedChecked)
            {
                _log.OtherInfo.CanBeDivided = res;
                _canBeDividedChecked = true;
            }
            else
            {
                _log.OtherInfo.SecondCheck = res;
            }
            return res;
        }

        public override bool TryAllowPath(IPath path, IGridGeometry3D gridSpace, HashSet<Config> states)
        {
            bool res = base.TryAllowPath(path, gridSpace, states);

            float posTube = PositionTube;
            float angleTube = AngleTube;
            float step = path.Length / 100f;
            float min = float.MaxValue;
            Config start = gridSpace.WorldToCell3D(path.Evaluate(0));
            Config end = gridSpace.WorldToCell3D(path.Evaluate(path.Length));

            for (int i = 0; i < 100; i++)
            {
                WorldConfig point = path.Evaluate(i * step);
                Config cell = gridSpace.WorldToCell3D(point);
                WorldConfig nearest = gridSpace.Cell3DToWorld(cell);

                float posDist = Vector2.Distance(nearest.Position, point.Position);
                float angleDist = Mathf.Abs(Mathf.DeltaAngle(point.Angle, nearest.Angle));
                bool posOk = posDist < posTube;
                bool angleOk = angleDist < angleTube;
                bool compare = posOk && angleOk;

                var pointLog = new LimiterPathLog.NearestPoint(
                    cell,
                    point,
                    compare,
                    posOk,
                    angleOk,
                    posDist,
                    angleDist);

                int index = _log.OtherInfo.NearestPoints.FindIndex(t => t.Cell == cell);
                if (index == -1)
                {
                    _log.OtherInfo.NearestPoints.Add(pointLog);
                }
                else if (_log.OtherInfo.NearestPoints[index].PosDist > pointLog.PosDist)
                {
                    _log.OtherInfo.NearestPoints[index] = pointLog;
                }

                if (compare)
                {
                    _log.OtherInfo.NearState = true;

                    if (cell.Cell != start.Cell && cell.Cell != end.Cell && states.Contains(cell))
                    {
                        _log.OtherInfo.ContainThisState = true;
                        if (min > posDist)
                        {
                            min = posDist;
                        }
                        _log.OtherInfo.Nearest = nearest;
                        _log.OtherInfo.NearestCell = cell;
                    }
                }
            }

            _log.Result = res;
            _log.DirectLength = Vector2.Distance(path.Evaluate(path.Length).Position, path.Evaluate(0).Position);
            _log.PosMinDist = min;
            _log.Length = path.Length;
            _logger.LogCheckingPath(_log, end);
            _log = new();
            _log.OtherInfo.NearestPoints = new();
            _canBeDividedChecked = false;
            return res;
        }

        public override bool CheckLength(IPath path)
        {
            bool res = base.CheckLength(path);
            _log.OtherInfo.LengthTooLong = !res;
            _log.Length = path.Length;
            return res;
        }
    }
}
