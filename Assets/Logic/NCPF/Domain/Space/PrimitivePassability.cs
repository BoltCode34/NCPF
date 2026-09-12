namespace NCPF.Domain
{
    /// <summary>
    /// Per-node mask of which control-set primitives clear the obstacles when projected onto that
    /// node. Projecting a primitive onto a node is a pure translation, so the SHAPE is identical at
    /// every node of a heading layer — the only thing that actually varies per node is whether the
    /// projected sweep fits. That is what this stores, one bit per (node, primitive), which is why
    /// a whole map costs megabytes instead of the gigabytes materialised transitions would.
    /// A primitive is addressed by its INDEX inside its heading layer, so a mask is only meaningful
    /// against the control set it was baked from.
    /// </summary>
    public class PrimitivePassability
    {
        private const int WordBits = 64;

        private readonly ulong[] _bits;
        private readonly int _stride;
        private readonly int _nodeCount;
        private readonly int _capacity;

        public PrimitivePassability(int nodeCount, int primitiveCapacity)
        {
            _nodeCount = nodeCount < 0 ? 0 : nodeCount;
            _capacity = primitiveCapacity < 0 ? 0 : primitiveCapacity;
            _stride = (_capacity + WordBits - 1) / WordBits;
            _bits = new ulong[(long)_nodeCount * _stride <= 0 ? 0 : _nodeCount * _stride];
        }

        public int NodeCount => _nodeCount;

        /// <summary>How many primitives per node the mask can address — the widest layer it was baked for.</summary>
        public int PrimitiveCapacity => _capacity;

        /// <summary>
        /// Out-of-range reads answer "blocked" rather than throwing: a mask baked against a
        /// different control set is a real possibility (rebake the set, keep the map), and the
        /// search must degrade into finding nothing instead of into an exception on a pool thread.
        /// </summary>
        public bool Passable(int nodeId, int primitiveIndex)
        {
            if (nodeId < 0 || nodeId >= _nodeCount || primitiveIndex < 0 || primitiveIndex >= _capacity)
            {
                return false;
            }

            int word = nodeId * _stride + (primitiveIndex >> 6);
            ulong bit = 1UL << (primitiveIndex & (WordBits - 1));
            return (_bits[word] & bit) != 0UL;
        }

        /// <summary>The baker's write surface: one writer fills the mask, the search only reads it.</summary>
        public void Set(int nodeId, int primitiveIndex, bool passable)
        {
            if (nodeId < 0 || nodeId >= _nodeCount || primitiveIndex < 0 || primitiveIndex >= _capacity)
            {
                return;
            }

            int word = nodeId * _stride + (primitiveIndex >> 6);
            ulong bit = 1UL << (primitiveIndex & (WordBits - 1));
            if (passable)
            {
                _bits[word] |= bit;
            }
            else
            {
                _bits[word] &= ~bit;
            }
        }
    }
}
