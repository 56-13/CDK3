using System;

namespace CDK.Assets.Terrain
{
    internal class TerrainWaterInstance : IEquatable<TerrainWaterInstance>
    {
        public TerrainWater Water { private set; get; }
        public float Altitude { private set; get; }

        public TerrainWaterInstance(TerrainWater water, float altitude)
        {
            Water = water;
            Altitude = altitude;
        }

        public bool Equals(TerrainWaterInstance other) 
        {
            return Water == other.Water && Altitude == other.Altitude;
        }

        public override bool Equals(object obj)
        {
            return obj is TerrainWaterInstance other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Water.GetHashCode());
            hash.Combine(Altitude.GetHashCode());
            return hash;
        }
    }
}
