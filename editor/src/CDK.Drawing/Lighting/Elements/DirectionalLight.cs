using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct DirectionalLight : IEquatable<DirectionalLight>
    {
        public object Key;
        public Vector3 Direction;
        public Color3 Color;
        public bool CastShadow;
        public bool CastShadow2D;
        public Shadow Shadow;

        public DirectionalLight(object key, in Vector3 direction, in Color3 color, bool castShadow, bool castShadow2D, in Shadow shadow)
        {
            Key = key;
            Direction = direction;
            Color = color;
            CastShadow = castShadow;
            CastShadow2D = castShadow2D;
            Shadow = shadow;
        }

        public static bool operator ==(in DirectionalLight left, in DirectionalLight right) => left.Equals(right);
        public static bool operator !=(in DirectionalLight left, in DirectionalLight right) => !left.Equals(right);

        public override string ToString() => $"Direction:{Direction} Color:{Color} CastShadow:{CastShadow} CastShadow2D:{CastShadow2D} Shadow:{Shadow}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Key.GetHashCode());
            hash.Combine(Direction.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(CastShadow.GetHashCode());
            hash.Combine(CastShadow2D.GetHashCode());
            hash.Combine(Shadow.GetHashCode());
            return hash;
        }

        public bool Equals(DirectionalLight other)
        {
            return Key.Equals(other.Key) && 
                Direction == other.Direction && 
                Color == other.Color &&
                CastShadow == other.CastShadow &&
                CastShadow2D == other.CastShadow2D &&
                Shadow == other.Shadow;
        }

        public override bool Equals(object obj) => obj is DirectionalLight other && Equals(other);
    }
}
