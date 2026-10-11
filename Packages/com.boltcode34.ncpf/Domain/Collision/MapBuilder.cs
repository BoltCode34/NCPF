namespace NCPF.Domain
{
    /// <summary>
    /// Bakes a <see cref="Map3D"/> from a 2D obstacle grid: for every direction
    /// layer the agent footprint is convolved over the world, then the resulting
    /// rig snapshot is packed with the map.
    /// </summary>
    public class MapBuilder
    {
        private Map _worldMap;
        private Agent _agent;

        public MapBuilder(Map worldMap, Agent agent)
        {
            _worldMap = worldMap;
            _agent = agent;
        }

        public Map3D BuildMap(int angleStepCount)
        {
            bool[,] world = _worldMap.Obstacles;
            float angleStep = 360f / angleStepCount;
            bool[,,] maps = new bool[world.GetLength(0), world.GetLength(1), angleStepCount];
            for (int z = 0; z < angleStepCount; z++)
            {
                float angle = z * angleStep;
                _agent.Angle = angle;
                ConvolveAngle(maps, z);
            }
            _agent.X = 0;
            _agent.Y = 0;
            _agent.Angle = 0;
            AgentRig rig = new AgentRig(_agent.Footprint, _agent.CellSize, _agent.Anchor);
            Map3D map = new Map3D(maps, rig, angleStep, _worldMap.CellSize);
            return map;
        }

        private void ConvolveAngle(bool[,,] maps, int z)
        {
            bool[,] result = Convolve();
            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    maps[x, y, z] = result[x, y];
                }
            }
        }

        private bool[,] Convolve()
        {
            bool[,] world = _worldMap.Obstacles;
            bool[,] agent = _agent.Footprint;
            bool[,] result = ConvolutionUtility.Convolve(world, agent, _agent.Anchor);

            return result;
        }
    }
}
