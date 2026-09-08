using Core.Foundation.Events;
using Core.Foundation.State;

namespace Core.Foundation.LifeTime
{
    public abstract class UpdatebleObject : IActivatable, IFixedUpdatable, IUpdatable
    {
        private EventBus<IUpdatable> _updateBus;
        private EventBus<IFixedUpdatable> _fixedUpdateBus;

        private bool _enabled;

        protected UpdatebleObject(EventBus<IUpdatable> updateBus, EventBus<IFixedUpdatable> fixedUpdateBus)
        {
            _updateBus = updateBus;
            _fixedUpdateBus = fixedUpdateBus;
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (value)
                    {
                        Enable();
                    }
                    else
                    {
                        Disable();
                    }
                } 
            }
        }
        public void Disable()
        {
            _enabled = false;
            _updateBus.RemoveListener(this);
            _fixedUpdateBus.RemoveListener(this);
            OnDisable();
        }

        public void Enable()
        {
            _enabled = true;
            _updateBus.AddListener(this);
            _fixedUpdateBus.AddListener(this);
            OnEnable();
        }
        public virtual void OnDisable()
        {
        }

        public virtual void OnEnable()
        {
        }
        public virtual void FixedTick()
        {
        }

        public virtual void UpdateTick()
        {
        }
    }
    public class ObjectUpdater : UpdatebleObject
    {
        private object _state;
        public ObjectUpdater(EventBus<IUpdatable> updateBus, EventBus<IFixedUpdatable> fixedUpdateBus, IState state) : base(updateBus, fixedUpdateBus)
        {
            _state = state;
        }

        public IStateMachine StateMachine { get; set; }

        public override void OnDisable()
        {
            if (_state is IActivatable)
            {
                IActivatable updateble = _state as IActivatable;
                updateble.Disable();
            }
        }
        public override void OnEnable()
        {
            if (_state is IActivatable)
            {
                IActivatable updateble = _state as IActivatable;
                updateble.Enable();
            }
        }
        public override void FixedTick()
        {
            if (_state is IFixedUpdatable)
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