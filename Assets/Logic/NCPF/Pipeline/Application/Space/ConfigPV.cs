using System;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// A cell of the (part, velocity) lattice — the PV twin of <see cref="ConfigAV"/>.
    /// "Where" is the window part index whose ENTRY the node sits on (0 = the part under
    /// the cursor); "how fast" is a SIGNED speed bucket: the sign is the GEAR
    /// (+ nose-first, − rear-first), so a gear change between parts can only pass
    /// through zero.
    /// </summary>
    [Serializable]
    public struct ConfigPV : IEquatable<ConfigPV>
    {
        public int Part;
        public int Speed;

        public ConfigPV(int part, int speed)
        {
            Part = part;
            Speed = speed;
        }

        public static bool operator ==(ConfigPV left, ConfigPV right)
            => left.Part == right.Part && left.Speed == right.Speed;

        public static bool operator !=(ConfigPV left, ConfigPV right) => !(left == right);

        public bool Equals(ConfigPV other) => this == other;
        public override bool Equals(object obj) => obj is ConfigPV other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Part, Speed);
        public override string ToString() => $"(p={Part}, v={Speed})";
    }
}
