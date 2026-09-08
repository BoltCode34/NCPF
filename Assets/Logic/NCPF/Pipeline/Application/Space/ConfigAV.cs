using System;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// A cell of the (angle, velocity) lattice — the AV twin of <see cref="Config"/>.
    /// <see cref="Sample"/> is the progress index along the planning window: the path is already
    /// chosen, so "where" is a single number, and the only things left to decide are the two the
    /// agent actually controls.
    /// </summary>
    [Serializable]
    public struct ConfigAV : IEquatable<ConfigAV>
    {
        public int Sample;
        public int Angle;
        public int Velocity;

        public ConfigAV(int sample, int angle, int velocity)
        {
            Sample = sample;
            Angle = angle;
            Velocity = velocity;
        }

        public static bool operator ==(ConfigAV left, ConfigAV right)
            => left.Sample == right.Sample && left.Angle == right.Angle && left.Velocity == right.Velocity;

        public static bool operator !=(ConfigAV left, ConfigAV right) => !(left == right);

        public bool Equals(ConfigAV other) => this == other;
        public override bool Equals(object obj) => obj is ConfigAV other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Sample, Angle, Velocity);
        public override string ToString() => $"(s={Sample}, a={Angle}, v={Velocity})";
    }

    /// <summary>
    /// The continuous state of the AV lattice — the AV twin of <see cref="WorldConfig"/>:
    /// facing in DEGREES and speed in METRES PER SECOND, as the rest of the world speaks them.
    /// </summary>
    [Serializable]
    public struct WorldConfigAV
    {
        public int Sample;
        public float Angle;
        public float Velocity;

        public WorldConfigAV(int sample, float angle, float velocity)
        {
            Sample = sample;
            Angle = angle;
            Velocity = velocity;
        }

        public override string ToString() => $"(s={Sample}, {Angle:0.#}°, {Velocity:0.##} m/s)";
    }
}
