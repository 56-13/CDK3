using System;
using System.Numerics;

namespace CDK.Drawing
{
    public struct SpotLight : IEquatable<SpotLight>
    {
        public object Key;
        public Vector3 Position;
        public Vector3 Direction;
        public float Angle;
        public float Dispersion;
        public Color3 Color;
        public LightAttenuation Attenuation;
        public bool CastShadow;
        public Shadow Shadow;

        public SpotLight(object key, in Vector3 position, in Vector3 direction, float angle, float dispersion, in Color3 color, LightAttenuation attenuation, bool castShadow, in Shadow shadow)
        {
            Key = key;
            Position = position;
            Direction = direction;
            Angle = angle;
            Dispersion = dispersion;
            Color = color;
            Attenuation = attenuation;
            CastShadow = castShadow;
            Shadow = shadow;
        }

        public float Range => (float)Math.Sqrt(Color.Brightness) * Attenuation.Range;
        public static bool operator ==(in SpotLight left, in SpotLight right) => left.Equals(right);
        public static bool operator !=(in SpotLight left, in SpotLight right) => !left.Equals(right);

        public override string ToString() => $"Position:{Position} Direction:{Direction} Angle:{Angle * MathUtil.ToDegrees:F2} Dispersion:{Dispersion * MathUtil.ToDegrees:F2} Color:{Color} Attenuation:{Attenuation.Range} CastShadow:{CastShadow} Shadow:{Shadow}";
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Key.GetHashCode());
            hash.Combine(Position.GetHashCode());
            hash.Combine(Direction.GetHashCode());
            hash.Combine(Angle.GetHashCode());
            hash.Combine(Dispersion.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(Attenuation.GetHashCode());
            hash.Combine(CastShadow.GetHashCode());
            hash.Combine(Shadow.GetHashCode());
            return hash;
        }

        public bool Equals(SpotLight other)
        {
            return Key.Equals(other.Key) && 
                Position == other.Position && 
                Direction == other.Direction && 
                Angle == other.Angle &&
                Dispersion == other.Dispersion &&
                Color == other.Color &&
                Attenuation == other.Attenuation &&
                CastShadow == other.CastShadow &&
                Shadow == other.Shadow;
        }

        public override bool Equals(object obj) => obj is SpotLight other && Equals(other);
    }
}
