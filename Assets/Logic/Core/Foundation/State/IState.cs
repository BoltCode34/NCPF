using Core.Foundation.LifeTime;

namespace Core.Foundation.State
{
    public interface IState : IActivatable
    {
        IStateMachine StateMachine { get; }

    }
}
