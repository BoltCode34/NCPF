using System.Collections.Generic;

namespace NCPF.Domain
{
    public sealed class MinPriorityQueue
    {
        private readonly List<Entry> _heap = new List<Entry>();
        private readonly Dictionary<int, int> _indices = new Dictionary<int, int>();

        public int Count => _heap.Count;

        public void EnqueueOrDecrease(int node, float priority)
        {
            if (_indices.TryGetValue(node, out var index))
            {
                if (priority >= _heap[index].Priority)
                {
                    return;
                }

                _heap[index] = new Entry(node, priority);
                HeapifyUp(index);
                return;
            }

            _heap.Add(new Entry(node, priority));
            var newIndex = _heap.Count - 1;
            _indices[node] = newIndex;
            HeapifyUp(newIndex);
        }

        public (int Node, float Priority) Dequeue()
        {
            var top = _heap[0];
            _indices.Remove(top.Node);
            var lastIndex = _heap.Count - 1;
            if (lastIndex > 0)
            {
                _heap[0] = _heap[lastIndex];
                _indices[_heap[0].Node] = 0;
            }
            _heap.RemoveAt(lastIndex);

            if (_heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return (top.Node, top.Priority);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                var parent = (index - 1) / 2;
                if (_heap[parent].Priority <= _heap[index].Priority)
                {
                    return;
                }

                Swap(parent, index);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                var left = index * 2 + 1;
                var right = left + 1;
                var smallest = index;

                if (left < _heap.Count && _heap[left].Priority < _heap[smallest].Priority)
                {
                    smallest = left;
                }

                if (right < _heap.Count && _heap[right].Priority < _heap[smallest].Priority)
                {
                    smallest = right;
                }

                if (smallest == index)
                {
                    return;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;

            _indices[_heap[i].Node] = i;
            _indices[_heap[j].Node] = j;
        }

        private readonly struct Entry
        {
            public readonly int Node;
            public readonly float Priority;

            public Entry(int node, float priority)
            {
                Node = node;
                Priority = priority;
            }
        }
    }
}
