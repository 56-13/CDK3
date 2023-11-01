using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct PointLight : IEquatable<PointLight>
    {
        public object Key;
        public Vector3 Position;
        public Color3 Color;
        public LightAttenuation Attenuation;
        public bool CastShadow;
        public Shadow Shadow;

        public PointLight(object key, in Vector3 position, in Color3 color, LightAttenuation attenuation, bool castShadow, in Shadow shadow)
        {
            Key = key;
            Position = position;
            Color = color;
            Attenuation = attenuation;
            CastShadow = castShadow;
            Shadow = shadow;
        }

        public float Range => (float)Math.Sqrt(Color.Brightness) * Attenuation.Range;
        public static bool operator ==(in PointLight left, in PointLight right) => left.Equals(right);
        public static bool operator !=(in PointLight left, in PointLight right) => !left.Equals(right);

        public override string ToString() => $"Position:{Position} Color:{Color} Attenuation:{Attenuation.Range} CastShadow:{CastShadow} Shadow:{Shadow}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Key.GetHashCode());
            hash.Combine(Position.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(Attenuation.GetHashCode());
            hash.Combine(CastShadow.GetHashCode());
            hash.Combine(Shadow.GetHashCode());
            return hash;
        }

        public bool Equals(PointLight other)
        {
            return Key.Equals(other.Key) && 
                Position == other.Position && 
                Color == other.Color &&
                Attenuation == other.Attenuation &&
                CastShadow == other.CastShadow &&
                Shadow == other.Shadow;
        }

        public override bool Equals(object obj) => obj is PointLight other && Equals(other);
    }
}
