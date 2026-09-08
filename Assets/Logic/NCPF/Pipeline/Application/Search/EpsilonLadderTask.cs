using NCPF.Domain;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>One ε-ladder run in the background. Runs the goal through the eps rungs (wrapping
    /// each in <see cref="EpsGoal"/>) and returns the FIRST success. Lives on the thread pool; the
    /// caller polls <see cref="State"/>/<see cref="Result"/> or awaits.</summary>
    public sealed class EpsilonLadderTask : IPathFindingTask<CurvateTransition>
    {
        private readonly Task<CurvateTransition[]> _task;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public EpsilonLadderTask(IGoalPathFinder<CurvateTransition> finder,
                                    IGraph<CurvateTransition> graph,
                                    IGoal goal,
                                    int startId,
                                    float[] ladder, int budget)
        {
            CancellationToken token = _cts.Token;
            float[] rungs = (ladder != null && ladder.Length > 0) ? ladder : new[] { 2f };
            int cap = Mathf.Max(1, budget);

            _task = Task.Run(() =>
            {
                foreach (float eps in rungs)
                {
                    IGoal epsGoal = new EpsGoal(goal, eps);
                    if (token.IsCancellationRequested) break;
                    CurvateTransition[] path = finder.FindPath(graph, startId, epsGoal, cap);
                    if (path != null && path.Length > 0)
                        return path;
                }
                return System.Array.Empty<CurvateTransition>();
            }, token);
        }

        public IPathFindingTask<CurvateTransition>.TaskState State =>
            _task.IsCanceled ? IPathFindingTask<CurvateTransition>.TaskState.Canceled :
            _task.IsCompleted ? IPathFindingTask<CurvateTransition>.TaskState.Completed :
            IPathFindingTask<CurvateTransition>.TaskState.InProccess;

        public CurvateTransition[] Result => _task.IsCompletedSuccessfully ? _task.Result : null;

        public TaskAwaiter<CurvateTransition[]> GetAwaiter() => _task.GetAwaiter();

        public void Break() => _cts.Cancel();
    }
}
