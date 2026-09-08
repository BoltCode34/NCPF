using NCPF.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// The bake diagnostic record: one build = layers of path checks. Storage is
    /// geometric (WorldConfig poses, Config cells) — poses and 3D cells only.
    /// </summary>
    [Serializable]
    public struct BuildLog
    {
        public List<LayerLog> Layers;
    }

    [Serializable]
    public struct LayerLog
    {
        [Header("Main info")]
        public WorldConfig Origin;
        public Config OriginCell;
        [Header("Additional info")]
        [InlineField("X: {EndCell.X} Y: {EndCell.Y} Ang: {EndCell.Angle}")]
        public List<PathLog> PathLogs;

        public LayerLog(WorldConfig origin, Config originCell)
        {
            Origin = origin;
            OriginCell = originCell;
            PathLogs = new List<PathLog>();
        }
    }

    [Serializable]
    public struct PathLog
    {
        [Header("Main info")]
        public WorldConfig End;
        public Config EndCell;
        public float Radius;
        public float Length;
        [Header("Results")]
        public bool IncludedInResult;
        [Header("Additional info")]
        public LimiterPathLog LimiterLog;

        public PathLog(WorldConfig end, Config endCell, float radius, float length, LimiterPathLog limiterLog, bool includedInResult = false)
        {
            End = end;
            EndCell = endCell;
            Radius = radius;
            Length = length;
            IncludedInResult = includedInResult;
            LimiterLog = limiterLog;
        }
    }

    [Serializable]
    public struct LimiterPathLog
    {
        [Header("Passed the limits")]
        public bool Result;
        public Defects OtherInfo;
        [Header("Check")]
        public float PosMinDist;
        public float DirectLength;
        public float Length;

        public LimiterPathLog(bool result, float posMinDist, float directLength, Defects otherInfo, float length)
        {
            Result = result;
            PosMinDist = posMinDist;
            DirectLength = directLength;
            OtherInfo = otherInfo;
            Length = length;
        }

        [Serializable]
        public struct Defects
        {
            [Header("Divide check")]
            public bool CanBeDivided;
            public bool SecondCheck;
            [Space(10)]
            public bool NearState;
            public bool ContainThisState;
            public WorldConfig Nearest;
            public Config NearestCell;
            [InlineField("X: {Cell.X} Y: {Cell.Y} Angle: {Cell.Angle}")]
            public List<NearestPoint> NearestPoints;
            [Header("Other")]
            public bool LengthTooLong;
        }

        [Serializable]
        public struct NearestPoint
        {
            public Config Cell;
            public WorldConfig EvaluatedPos;
            public bool Compare;
            public bool Pos;
            public bool Angle;
            public float PosDist;
            public float AngleDist;

            public NearestPoint(Config cell, WorldConfig evaluatedPos, bool compare, bool pos, bool angle, float posDist, float angleDist)
            {
                Cell = cell;
                EvaluatedPos = evaluatedPos;
                Compare = compare;
                Pos = pos;
                Angle = angle;
                PosDist = posDist;
                AngleDist = angleDist;
            }
        }
    }
}
