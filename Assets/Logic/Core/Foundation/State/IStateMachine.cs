namespace Core.Foundation.State
{
    public interface IStateMachine
    {
        public IState CurrentState { get; }
        public void SetState(IState state);
    }
    public class StateMachineBase : IStateMachine
    {
        public IState CurrentState {get; protected set;}

        public void SetState(IState state)
        {
            CurrentState?.Disable();
            CurrentState = state;
            CurrentState?.Enable();
        }
    }
}