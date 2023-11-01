using System;

namespace CDK.Assets.Terrain
{
    internal struct TerrainSurfaceInstance : IEquatable<TerrainSurfaceInstance>
    {
        public double Intermediate;
        public double Current;

        public TerrainSurfaceInstance(double value)
            : this()
        {
            Intermediate = value;
            Current = value;
        }

        public TerrainSurfaceInstance(double intermediate, double current)
            : this()
        {
            Intermediate = intermediate;
            Current = current;
        }

        public byte TextureValue => (byte)Math.Round(Current * 255);

        public static bool operator ==(in TerrainSurfaceInstance x, in TerrainSurfaceInstance y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(in TerrainSurfaceInstance x, in TerrainSurfaceInstance y)
        {
            return !x.Equals(y);
        }

        public bool IsEmpty => Intermediate == 0 && Current == 0;

        public bool Equals(TerrainSurfaceInstance other)
        {
            return Intermediate == other.Intermediate && Current == other.Current;
        }

        public override bool Equals(object obj)
        {
            return obj is TerrainSurfaceInstance i && Equals(i);
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Intermediate.GetHashCode());
            hash.Combine(Current.GetHashCode());
            return hash;
        }

        public static TerrainSurfaceInstance Empty = new TerrainSurfaceInstance();
    }
}
