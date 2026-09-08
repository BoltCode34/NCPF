using NCPF.Domain;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NCPF.Pipeline.Application
{
    public interface IAsyncGoalPathFinder<T> : IGoalPathFinder<T> where T : ITransition
    {
        public IPathFindingTask<T> FindPathAsync(
            IGraph<T> graph,
            int from,
            IGoal goal, int maxIterations = 1000);
    }

    public interface IPathFindingTask<T> where T : ITransition
    {
        public TaskState State { get; }
        public T[] Result { get; }
        TaskAwaiter<T[]> GetAwaiter();
        public void Break();

        public enum TaskState
        {
            InProccess,
            Canceled,
            Completed
        }
    }
}
