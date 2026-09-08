using System;

namespace Core.Foundation.Events
{
    public interface IAsyncCommand : ICommand<Action>
    {
        public void Execute(Action onComplete);
    }
}