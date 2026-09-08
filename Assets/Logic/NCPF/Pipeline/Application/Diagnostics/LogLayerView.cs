using NCPF.Domain;
// DISABLED: this viewer targets the OLD 4D bake log (velocity in cells).
// The bake/log is geometric now (Config/WorldConfig). Re-enable by defining
// NCPF_LEGACY_DIAGNOSTICS after reworking it for the geometric log.
#if NCPF_LEGACY_DIAGNOSTICS
using UnityEngine;
using BuildLog = Features.NCPF.Application.ControlSetBuildLogger.BuildLog;
using PathLog = Features.NCPF.Application.ControlSetBuildLogger.PathLog;
using System.Collections.Generic;
using System;
using Features.NCPF.Domain;
namespace Features.NCPF.Application
{
    [CreateAssetMenu(fileName = "LogLayerView", menuName = "Logs/LogLayerView")]
    public class LogLayerView : ScriptableObject
    {
        [SerializeField] private ControlSetBuildLogger _logger;
        [Header("Log")]
        [SerializeField] private int _log = 0;
        [SerializeField] private bool _getLastLog = false;
        [Header("Layer")]
        [SerializeField] private int _layer = 0;
        [SerializeField] private bool _getByCoord = false;
        [SerializeField] private int _angle;
        [SerializeField] private int _velocity;
        [Header("Extrude parameters")]
        [SerializeField] private int _maxPathsCount = 500;
        [SerializeField] private Vector2 _radiusRange;
        [SerializeField] private Config4D _endState;

        private ControlSetBuildLogger.LayerLog Log;
        [Header("Result")]
        public LayerLog DetalizedLog;
        [InlineField("X: {EndCell.Pose.X} Y: {EndCell.Pose.Y} Angle: {EndCell.Pose.Angle} Vel: {EndCell.Velocity}")]
        public PathLog Left;
        [InlineField("X: {EndCell.Pose.X} Y: {EndCell.Pose.Y} Angle: {EndCell.Pose.Angle} Vel: {EndCell.Velocity}")]
        public PathLog Right;


        [InlineField("Radius: {Radius} Count: {Logs.Count}")]
        [Serializable]
        public struct LayerLog
        {
            public DynamicState State;
            public Config4D Cell;
            public int IncludedPathCount;
            public int ExcludedPathCount;
            [InlineField("Radius: {Radius} Count: {Logs.Count}")]
            public List<RadiusLog> Radius;

            public LayerLog(DynamicState state, Config4D cell) : this()
            {
                State = state;
                Cell = cell;
                Radius = new List<RadiusLog>();
            }
        }
        [Serializable]
        public struct PointLog
        {
            public int X;
            public int Y;
            [InlineField("X: {EndCell.X} Y: {EndCell.Y} Ang: {EndCell.Angle} Vel: {EndCell.Velocity}")]
            public List<PathLog> IncludedPaths;
            [InlineField("X: {EndCell.X} Y: {EndCell.Y} Ang: {EndCell.Angle} Vel: {EndCell.Velocity}")]
            public List<PathLog> ExcludedPaths;

            public PointLog( int x,int y)
            {
                Y = y;
                X = x;
                ExcludedPaths = new List<PathLog>();
                IncludedPaths = new List<PathLog>();
            }
        }
        [Serializable]
        public struct RadiusLog
        {
            public float Radius;
            [InlineField("X: {X} Y: {Y}")]
            public List<PointLog> Logs;

            public RadiusLog(float radius) : this()
            {
                Radius = radius;
                Logs = new List<PointLog>();
            }


        }
        [ContextMenu("FindSimetricalPaths")]
        public void FindSimetricalPaths()
        {
            Config4D leftCell = _endState;
            leftCell.Pose.X = -Mathf.Abs(_endState.X);
            leftCell.Pose.Angle = 16- _endState.Angle;
            Config4D rightCell = _endState;
            leftCell.Pose.X = Mathf.Abs(_endState.X);
            if (_logger != null)
            {
                if (_logger.Logs == null || _logger.Logs.Count == 0)
                {
                    Debug.LogError("There is no logs yet");
                    return;
                }
                int log = _log;
                if (_getLastLog)
                {
                    log = _logger.Logs.Count - 1;
                }
                if (log < _logger.Logs.Count)
                {
                    BuildLog buildLog = _logger.Logs[log];
                    int layerInd = _layer;
                    if (_getByCoord)
                    {
                        int coordInd = buildLog.Layers.FindIndex(layer =>
                        layer.OriginCell.Angle == _angle &&
                        layer.OriginCell.Velocity == _velocity);
                        if (coordInd == -1)
                        {
                            Debug.LogWarning("There is no layer with the same coords");
                        }
                        else
                        {
                            layerInd = coordInd;
                        }
                    }
                    ControlSetBuildLogger.LayerLog layer = _logger.Logs[log].Layers[layerInd];
                    Right = layer.PathLogs.Find(log=>log.EndCell == rightCell);
                    Left = layer.PathLogs.Find(log => log.EndCell == leftCell);
                }
            }
            else
            {
                Debug.LogError("logger is null");
            }
        }
        public void AddPoint(List<PointLog> pointsLogs, PathLog path)
        {
            int index = pointsLogs.FindIndex(r => r.X == path.EndCell.X&& r.Y == path.EndCell.Y);
            if(index == -1)
            {
                index = pointsLogs.Count;
                pointsLogs.Add(new PointLog(path.EndCell.X, path.EndCell.Y));
            }
            if (path.IncludedInResult)
            {
                pointsLogs[index].IncludedPaths.Add(path);
                DetalizedLog.IncludedPathCount++;
            }
            else
            {

                pointsLogs[index].ExcludedPaths.Add(path);
                DetalizedLog.ExcludedPathCount++;
            }
        }
        public int FindRadius(LayerLog log, float radius)
        {
            List<RadiusLog> radiusLogs = log.Radius;
            int index = radiusLogs.FindIndex(r => r.Radius == radius);
            if (index == -1)
            {
                index = radiusLogs.Count;
                radiusLogs.Add(new RadiusLog(radius));
            }
            return index;
        }
        //public void LoadDetalized()
        //{
        //    LayerLog log = new LayerLog(Log.Origin, Log.OriginCell);
        //    for (int i = 0; i < Log.PathLogs.Count; i++)
        //    {
        //        int RadIndex = FindRadius(log, Log.PathLogs[i].Radius);
        //        AddPoint(log.Radius[RadIndex].Logs, Log.PathLogs[i]);
        //    }
        //    DetalizedLog = log;
        //}
        public void LoadDetalized()
        {
            LayerLog log = new LayerLog(Log.Origin, Log.OriginCell);
            for (int i = 0; i < Log.PathLogs.Count; i++)
            {
                if(Log.PathLogs[i].Radius > _radiusRange.x&& Log.PathLogs[i].Radius < _radiusRange.y)
                {
                    int RadIndex = FindRadius(log, Log.PathLogs[i].Radius);
                    AddPoint(log.Radius[RadIndex].Logs, Log.PathLogs[i]);
                }

            }
            DetalizedLog = log;
        }
        [ContextMenu("LoadLayer")]
        public void LoadLayer()
        {
            if( _logger != null)
            {
                if(_logger.Logs == null || _logger.Logs.Count == 0)
                {
                    Debug.LogError("There is no logs yet");
                    return;
                }
                int log = _log;
                if (_getLastLog)
                {
                    log = _logger.Logs.Count - 1;
                }
                if(log < _logger.Logs.Count )
                {
                    BuildLog buildLog = _logger.Logs[log];
                    int layerInd = _layer;
                    if (_getByCoord)
                    {
                        int coordInd = buildLog.Layers.FindIndex(layer =>
                        layer.OriginCell.Angle == _angle &&
                        layer.OriginCell.Velocity == _velocity);
                        if(coordInd == -1)
                        {
                            Debug.LogWarning("There is no layer with the same coords");
                        }
                        else
                        {
                            layerInd = coordInd;
                        }
                    }
                    ControlSetBuildLogger.LayerLog layer = _logger.Logs[log].Layers[layerInd];

                    if(_logger.Logs[log].Layers.Count > layerInd)
                    Log = new ControlSetBuildLogger.LayerLog(layer.Origin, layer.OriginCell);
                    for (int i = 0; i < layer.PathLogs.Count && i < _maxPathsCount; i++)
                    {
                        Log.PathLogs.Add(layer.PathLogs[i]);
                    }
                }
            }
            else
            {
                Debug.LogError("logger is null");
            }
            LoadDetalized();
        }
    }
}

#endif
