using Core.Foundation.Events;
using Core.Foundation.LifeTime;

namespace Core.Foundation.State
{
    public class UpdatebleState : UpdatebleObject, IState
    {
        private IState _state;
        public UpdatebleState(EventBus<IUpdatable> updateBus, EventBus<IFixedUpdatable> fixedUpdateBus, IState state) : base(updateBus, fixedUpdateBus)
        {
            _state = state;
        }

        public IStateMachine StateMachine { get; set; }

        public override void OnDisable()
        {
            _state?.Disable();
        }
        public override void OnEnable()
        {
            _state?.Enable();
        }
        public override void FixedTick()
        {
            if(_state is IFixedUpdatable)
            {
                IFixedUpdatable updateble = _state as IFixedUpdatable;
                updateble.FixedTick();
            }
        }
        public override void UpdateTick()
        {
            if (_state is IUpdatable)
            {
                IUpdatable updateble = _state as IUpdatable;
                updateble.UpdateTick();
            }
        }
    }
}