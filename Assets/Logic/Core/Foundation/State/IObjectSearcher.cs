namespace Core.Foundation.State
{
    public interface IObjectSearcher<T>
    {
        public T TryFind();
    }
}