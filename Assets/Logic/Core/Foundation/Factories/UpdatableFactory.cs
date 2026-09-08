using Core.Foundation.Events;
using Core.Foundation.LifeTime;

namespace Core.Foundation.Factories
{
    public class UpdatableFactory<Interface,Concrete> : IFactory<Interface> where Concrete : class, IUpdatable, Interface
    {
        private readonly IFactory<Concrete> _concreteFactory;
        private readonly EventBus<IUpdatable> _eventBus;

        public UpdatableFactory(IFactory<Concrete> concreteFactory, EventBus<IUpdatable> eventBus)
        {
            this._concreteFactory = concreteFactory;
            this._eventBus = eventBus;
        }

        public Interface Create()
        {
            Concrete concrete = _concreteFactory.Create();
            _eventBus.AddListener(concrete);
            return concrete;
        }
    }
}