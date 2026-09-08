namespace Core.Foundation.Events
{
    public interface ICommand
    {
        public void Execute();
    }
    public interface ICommand<D1>
    {
        public void Execute(D1 d);
    }
    public interface ICommand<R1, D1>
    {
        public R1 Execute(D1 d);
    }
    public interface ICommand<R1, D1, D2>
    {
        public R1 Execute(D1 d1, D2 d2);
    }
}
