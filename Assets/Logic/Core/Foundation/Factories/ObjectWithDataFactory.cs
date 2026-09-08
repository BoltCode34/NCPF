using System;

namespace Core.Foundation.Factories
{
    internal class ObjectWithDataFactory<Inf, Concrete, Data> : IFactory<Inf> where Concrete : Inf where Inf : class
    {
        private Data _data;
        public ObjectWithDataFactory(Data data)
        {
            _data = data;
        }

        public Inf Create()
        {

            return (Concrete)Activator.CreateInstance(typeof(Concrete), _data);
        }
    }
}