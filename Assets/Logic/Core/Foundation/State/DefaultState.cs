namespace Core.Foundation.State
{
    public class DefaultState : IState
    {
        public IStateMachine StateMachine {get;set;}
        public IState Default;

        public DefaultState(IStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public void Disable()
        {
        }

        public void Enable()
        {
            StateMachine.SetState(Default);
        }
    }
}