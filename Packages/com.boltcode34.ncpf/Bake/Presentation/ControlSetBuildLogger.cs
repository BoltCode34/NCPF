using NCPF.Bake.Application;
using NCPF.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace NCPF.Bake.Presentation
{
    /// <summary>
    /// Bake diagnostics log storage: a rolling buffer of builds kept in a
    /// ScriptableObject, writable to a readable text file next to the project.
    /// Storage is geometric (WorldConfig poses, Config cells) — no dynamics.
    /// </summary>
    [CreateAssetMenu(fileName = "ControlSetBuildLogger", menuName = "Logs/Control set")]
    public class ControlSetBuildLogger : ScriptableObject, IControlSetBuildLogger
    {
        public int Buffer = 3;
        public List<BuildLog> Logs = new List<BuildLog>();

        public void StartLogGeneration()
        {
            if (Logs.Count >= Buffer)
            {
                Logs.RemoveAt(0);
                StartLogGeneration();
                return;
            }
            BuildLog log = new BuildLog();
            log.Layers = new();
            Logs.Add(log);
        }

        public void StartLogLayer(WorldConfig origin, Config originCell)
        {
            Logs[^1].Layers.Add(new LayerLog(origin, originCell));
        }

        public void LogCheckingPath(LimiterPathLog log, Config pos)
        {
            List<PathLog> pathLogs = Logs[^1].Layers[^1].PathLogs;
            int pathIndex = pathLogs.FindIndex(t => t.EndCell == pos);
            if (pathIndex == -1)
            {
                pathIndex = pathLogs.Count;
                LogPath(pos, new WorldConfig());
            }
            PathLog pathLog = pathLogs[pathIndex];
            pathLog.LimiterLog = log;
            pathLogs[pathIndex] = pathLog;
        }

        public void MarkLayerAsIncluded(Config end)
        {
            List<PathLog> pathLogs = Logs[^1].Layers[^1].PathLogs;
            int pathIndex = pathLogs.FindIndex(t => t.EndCell == end);
            if (pathIndex == -1)
            {
                pathIndex = pathLogs.Count;
                LogPath(end, new WorldConfig());
            }
            PathLog pathLog = pathLogs[pathIndex];
            pathLog.IncludedInResult = true;
            pathLogs[pathIndex] = pathLog;
        }

        [ContextMenu("Save Log To File")]
        public void SaveLog()
        {
            StringBuilder sb = new();
            sb.AppendLine("=== ControlSet Build Log ===");
            sb.AppendLine($"builds={Logs.Count}");

            for (int b = 0; b < Logs.Count; b++)
            {
                BuildLog build = Logs[b];
                int layerCount = build.Layers != null ? build.Layers.Count : 0;
                sb.AppendLine();
                sb.AppendLine($"Build #{b}  layers={layerCount}");
                if (build.Layers == null) continue;

                foreach (LayerLog layer in build.Layers)
                {
                    List<PathLog> logs = layer.PathLogs;
                    int total = logs != null ? logs.Count : 0;
                    int included = 0;
                    if (logs != null)
                        foreach (PathLog p in logs)
                            if (p.IncludedInResult) included++;

                    sb.AppendLine($"  Layer origin={layer.OriginCell}  originWorld=({layer.Origin.X:0.###},{layer.Origin.Y:0.###} a={layer.Origin.Angle:0.#})  " +
                                  $"checked={total}  included={included}");
                    if (logs == null) continue;

                    foreach (PathLog p in logs)
                    {
                        LimiterPathLog lim = p.LimiterLog;
                        LimiterPathLog.Defects d = lim.OtherInfo;
                        sb.AppendLine(
                            $"    [{(p.IncludedInResult ? "KEEP" : "drop")}] end={p.EndCell} " +
                            $"world=({p.End.X:0.###},{p.End.Y:0.###} a={p.End.Angle:0.#})  r={p.Radius:0.###} L={p.Length:0.###}  " +
                            $"pass={lim.Result} | divisible={d.CanBeDivided} tooLong={d.LengthTooLong}  " +
                            $"minPos={lim.PosMinDist:0.###} direct={lim.DirectLength:0.###}");
                    }
                }
            }

            string file = Path.Combine(UnityEngine.Application.dataPath, "ControlSetBuildLog.txt");
            File.WriteAllText(file, sb.ToString());
            Debug.Log($"[ControlSetBuildLogger] wrote {Logs.Count} build(s) -> {file}");
        }

        public void LogPath(Config endCell, WorldConfig end, float radius = -1, float length = -1, bool includedInResult = false)
        {
            List<PathLog> pathLogs = Logs[^1].Layers[^1].PathLogs;
            int pathIndex = pathLogs.FindIndex(t => t.EndCell == endCell);
            if (pathIndex == -1)
            {
                pathLogs.Add(new PathLog(end, endCell, radius, length, new LimiterPathLog()));
            }
            else
            {
                PathLog pathLog = pathLogs[pathIndex];
                pathLog.End = end;
                pathLog.EndCell = endCell;
                if (radius != -1) pathLog.Radius = radius;
                if (length != -1) pathLog.Length = length;
                if (includedInResult) pathLog.IncludedInResult = includedInResult;
                pathLogs[pathIndex] = pathLog;
            }
        }
    }
}
