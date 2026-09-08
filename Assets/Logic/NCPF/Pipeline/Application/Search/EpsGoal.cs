namespace NCPF.Pipeline.Application
{
    /// <summary>Weighted-A* decorator: multiplies the decoratee's heuristic by <c>eps</c>, leaving
    /// achievement untouched. eps = 1 is optimal, &gt;1 is greedier and faster; the ε ladder runs one
    /// goal through several eps values, taking the first success.</summary>
    public class EpsGoal : IGoal
    {
        private IGoal _decoratee;
        private float _eps;

        public EpsGoal(IGoal decoratee, float eps)
        {
            _decoratee = decoratee;
            _eps = eps;
        }

        public bool Achieved(int point) => _decoratee.Achieved(point);

        public float Heuristic(int point)
        {
            return _decoratee.Heuristic(point) * _eps;
        }
    }
}
