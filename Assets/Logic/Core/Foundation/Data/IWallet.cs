using System;

namespace Core.Foundation.Data
{
    public interface IWallet
    {
        int Value { get; }
        public void Add(int points);
        public void Reset();
        public event Action<int> ValueChanged;
    }
}
