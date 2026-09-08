using Core.Foundation.Events;

namespace Core.Foundation.LifeTime
{
    public interface IUpdatable : ICommand
    {
        void ICommand.Execute() => UpdateTick();
        public void UpdateTick();
    }
}