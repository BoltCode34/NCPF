using System;

namespace Core.Foundation.State
{
    public interface IFiniteState : IState
    {
        public Action OnComplete { get; set; }
    }
    public class FiniteStateBase : IState
    {
        public FiniteStateBase(IStateMachine stateMachine = null, Action onComplete = null)
        {
            OnComplete = onComplete;
            StateMachine = stateMachine;
        }

        public Action OnComplete { get; set; }

        public IStateMachine StateMachine { get; set; }

        public virtual void Disable()
        {
            OnComplete?.Invoke();
        }

        public virtual void Enable()
        {

        }
    }
}
