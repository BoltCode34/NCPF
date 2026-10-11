using NCPF.Domain;
using System.Collections.Generic;

namespace NCPF.Pipeline.Application
{
    /// <summary>A* over a <see cref="IGraph{T}"/> with an <see cref="IGoal"/>. Generic in the edge
    /// type so the same engine serves the geometric and rope graphs.</summary>
    public class GoalPathFinderBase<T> : IGoalPathFinder<T>
        where T : ITransition
    {
        public virtual T[] FindPath(
            IGraph<T> graph,
            int from,
            IGoal goal, int maxIterations)
        {
            if (graph == null)
            {
                return System.Array.Empty<T>();
            }

            if (!graph.PointFree(from))
            {
                return System.Array.Empty<T>();
            }

            var open = new MinPriorityQueue();

            var gScore = new Dictionary<int, float>();
            var parents = new Dictionary<int, (int node, T transition)>();
            var closed = new HashSet<int>();

            gScore[from] = 0f;

            open.EnqueueOrDecrease(from, goal.Heuristic(from));
            int iteration = 0;
            while (open.Count > 0 && maxIterations > iteration++)
            {
                var (current, _) = open.Dequeue();

                if (closed.Contains(current))
                    continue;

                if (goal.Achieved(current))
                {
                    return RestorePath(parents, from, current);
                }
                closed.Add(current);

                var neighbors = graph.GetNeightbors(current);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    var transition = neighbors[i];
                    int next = transition.Neightbor;

                    if (closed.Contains(next))
                        continue;

                    if (!graph.PointFree(next))
                        continue;

                    float tentativeG =
                        gScore[current] + transition.Weight;

                    if (gScore.TryGetValue(next, out float oldG)
                        && tentativeG >= oldG)
                        continue;

                    gScore[next] = tentativeG;
                    parents[next] = (current, transition);

                    float f =
                        tentativeG +
                        goal.Heuristic(next);

                    open.EnqueueOrDecrease(next, f);
                }
            }
            return System.Array.Empty<T>();
        }

        private static T[] RestorePath(
            Dictionary<int, (int node, T transition)> parents,
            int start,
            int end)
        {
            List<T> path = new();
            int current = end;

            while (current != start)
            {
                if (!parents.TryGetValue(current, out var data))
                    return System.Array.Empty<T>();

                path.Add(data.transition);
                current = data.node;
            }

            path.Reverse();
            return path.ToArray();
        }
    }
}
