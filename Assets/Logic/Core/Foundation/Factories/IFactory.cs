namespace Core.Foundation.Factories
{
    public interface IFactory<T>
    {
        T Create();
    }
    public interface IFactory<T, D>
    {

        T Create(D data);
    }
    public interface IFactory<T, D, D1>
    {

        T Create(D data, D1 data1);
    }
    public interface IFactory<T, D, D1, D2>
    {

        T Create(D data, D1 data1, D2 data2);
    }
}