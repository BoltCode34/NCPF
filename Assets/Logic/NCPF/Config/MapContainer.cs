using Core.Foundation;
using NCPF.Domain;
using NCPF.Shared.Presentation;
using System;
using UnityEngine;

namespace NCPF.Config
{
    /// <summary>
    /// Stores a baked C-space map as serialisable occupancy layers plus the
    /// agent signature, and reassembles a Map3D on read.
    /// </summary>
    [CreateAssetMenu(fileName ="NCPFMap", menuName = "NCPF/Containers/MapContainer")]
    public class MapContainer : MapAsset
    {
        [SerializeField, HideInInspector] private Layer[] _map;
        [SerializeField, HideInInspector] public int SizeX;
        [SerializeField, HideInInspector] public int SizeY;
        [SerializeField, HideInInspector] public float CellSize;
        [SerializeField, HideInInspector] public AgentSignature Agent;

        public override void Write(Map3D map)
        {
            bool[,,] config = map.Map;
            _map = new Layer[map.SizeZ];
            SizeX = config.GetLength(0);
            SizeY = config.GetLength(1);
            CellSize = map.CellSize;
            Agent = AgentSignature.FromAgent(map.Rig);
            for (int z = 0; z < config.GetLength(2); z++)
            {
                bool[] layer = new bool[config.GetLength(0)* config.GetLength(1)];
                for (int x = 0; x < config.GetLength(0); x++)
                {
                    for (int y = 0; y < config.GetLength(1); y++)
                    {
                        layer[x * config.GetLength(1) + y] = config[x, y, z];
                    }
                }
                _map[z] = new Layer
                {
                    Angle = map.AngleStep * z,
                    Map = layer,
                    SizeX = this.SizeX,
                    SizeY = this.SizeY
                };
            }

        }
        public override Map3D Read()
        {
            if( _map == null || _map.Length == 0)
            {
                return new Map3D(new bool[0, 0, 0], null, 0, 1);
            }
            bool[,,] config = new bool[SizeX, SizeY, _map.Length];
            for (int z = 0; z < config.GetLength(2); z++)
            {
                for (int x = 0; x < config.GetLength(0); x++)
                {
                    for (int y = 0; y < config.GetLength(1); y++)
                    {
                        config[x, y, z] = _map[z].Map[x * config.GetLength(1) + y];
                    }
                }
            }
            return new Map3D(config, AgentSignature.ToAgent(Agent), 360f / _map.Length, CellSize);
        }
        [Serializable]
        public struct Layer
        {
            public float Angle;
            public bool[] Map;
            public int SizeX;
            public int SizeY;
        }
        /// <summary>
        /// The agent's discrete footprint. The anchor is part of the signature,
        /// so an asset baked for a different reference point reads as stale
        /// instead of silently valid.
        /// </summary>
        [Serializable]
        public struct AgentSignature
        {
            public bool[] Footprint;
            public int Side;
            public float CellSize;
            public int AnchorX;
            public int AnchorY;

            public AgentSignature(bool[] footprint, float cellSize, int side, int anchorX, int anchorY)
            {
                Footprint = footprint;
                CellSize = cellSize;
                Side = side;
                AnchorX = anchorX;
                AnchorY = anchorY;
            }

            public static AgentSignature FromAgent(AgentRig agent)
            {
                bool[,] config = agent.Footprint;
                bool[] footprint = new bool[agent.Size*agent.Size];
                for (int x = 0; x < config.GetLength(0); x++)
                {
                    for (int y = 0; y < config.GetLength(1); y++)
                    {
                        footprint[x * config.GetLength(1) + y] = config[x, y];
                    }
                }
                return new AgentSignature(
                    footprint,
                    agent.CellSize,
                    agent.Size,
                    agent.Anchor.x,
                    agent.Anchor.y);
            }
            public static AgentRig ToAgent(AgentSignature signature)
            {
                bool[,] config = new bool[signature.Side, signature.Side];
                bool[] footprint = signature.Footprint;
                for (int x = 0; x < config.GetLength(0); x++)
                {
                    for (int y = 0; y < config.GetLength(1); y++)
                    {
                        config[x, y] = footprint[x * config.GetLength(1) + y];
                    }
                }
                return new AgentRig(config, signature.CellSize, new Int2(signature.AnchorX, signature.AnchorY));
            }
        }
    }
}
