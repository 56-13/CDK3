using System;
using System.Collections.Generic;
using System.Numerics;

namespace CDK.Drawing
{
    public enum MaterialShader
    {
        Light,
        NoLight,
        Distortion
    }

    [Flags]
    public enum MaterialMapComponent
    {
        Metallic = 1,
        Roughness = 2,
        Occlusion = 4
    }

    public struct Material : IEquatable<Material>
    {
        public MaterialShader Shader;
        public BlendMode BlendMode;
        public CullMode CullMode;
        public MaterialMapComponent MaterialMapComponents;
        public bool DepthTest;
        public bool AlphaTest;
        public float DepthBias;
        public float AlphaTestBias;
        public float DisplacementScale;
        public float DistortionScale;
        public Texture ColorMap;
        public Texture NormalMap;
        public Texture MaterialMap;
        public Texture EmissiveMap;
        public Color4 Color;
        public bool Bloom;
        public bool Reflection;
        public bool ReceiveShadow;
        public bool ReceiveShadow2D;
        public float Metallic;
        public float Roughness;
        public float AmbientOcclusion;
        public float Rim;
        public Color3 Emission;
        public Vector2 UVOffset;

        public bool UsingColor
        {
            get
            {
                switch (Shader)
                {
                    case MaterialShader.Light:
                    case MaterialShader.NoLight:
                        return true;
                }
                return false;
            }
        }

        public bool ReceiveLight
        {
            get
            {
                switch (Shader)
                {
                    case MaterialShader.Light:
                        return true;
                }
                return false;
            }
        }

        public bool Batch(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes)
        {
            var result = true;
            if (ColorMap != null) result &= ColorMap.Batch(reads, writes, BatchFlag.Read);
            if (NormalMap != null) result &= NormalMap.Batch(reads, writes, BatchFlag.Read);
            if (MaterialMap != null) result &= MaterialMap.Batch(reads, writes, BatchFlag.Read);
            if (EmissiveMap != null) result &= EmissiveMap.Batch(reads, writes, BatchFlag.Read);
            return result;
        }

        public static bool operator ==(in Material left, in Material right) => left.Equals(right);
        public static bool operator !=(in Material left, in Material right) => !left.Equals(right);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Shader.GetHashCode());
            hash.Combine(BlendMode.GetHashCode());
            hash.Combine(CullMode.GetHashCode());
            hash.Combine(MaterialMapComponents.GetHashCode());
            hash.Combine(DepthTest.GetHashCode());
            hash.Combine(AlphaTest.GetHashCode());
            hash.Combine(DepthBias.GetHashCode());
            hash.Combine(AlphaTestBias.GetHashCode());
            hash.Combine(DisplacementScale.GetHashCode());
            hash.Combine(DistortionScale.GetHashCode());
            if (ColorMap != null) hash.Combine(ColorMap.GetHashCode());
            if (NormalMap != null) hash.Combine(NormalMap.GetHashCode());
            if (MaterialMap != null) hash.Combine(MaterialMap.GetHashCode());
            if (EmissiveMap != null) hash.Combine(EmissiveMap.GetHashCode());
            hash.Combine(Color.GetHashCode());
            hash.Combine(Bloom.GetHashCode());
            hash.Combine(Reflection.GetHashCode());
            hash.Combine(ReceiveShadow.GetHashCode());
            hash.Combine(ReceiveShadow2D.GetHashCode());
            hash.Combine(Metallic.GetHashCode());
            hash.Combine(Roughness.GetHashCode());
            hash.Combine(AmbientOcclusion.GetHashCode());
            hash.Combine(Rim.GetHashCode());
            hash.Combine(Emission.GetHashCode());
            hash.Combine(UVOffset.GetHashCode());
            return hash;
        }

        public bool Equals(Material other)
        {
            return Shader == other.Shader &&
                BlendMode == other.BlendMode &&
                CullMode == other.CullMode &&
                MaterialMapComponents == other.MaterialMapComponents &&
                DepthTest == other.DepthTest &&
                AlphaTest == other.AlphaTest &&
                DepthBias == other.DepthBias &&
                AlphaTestBias == other.AlphaTestBias &&
                DisplacementScale == other.DisplacementScale &&
                DistortionScale == other.DistortionScale &&
                ColorMap == other.ColorMap &&
                NormalMap == other.NormalMap &&
                MaterialMap == other.MaterialMap &&
                EmissiveMap == other.EmissiveMap &&
                Color == other.Color &&
                Bloom == other.Bloom &&
                Reflection == other.Reflection &&
                ReceiveShadow == other.ReceiveShadow &&
                ReceiveShadow2D == other.ReceiveShadow2D &&
                Metallic == other.Metallic &&
                Roughness == other.Roughness &&
                AmbientOcclusion == other.AmbientOcclusion &&
                Rim == other.Rim &&
                Emission == other.Emission &&
                UVOffset == other.UVOffset;
        }

        public override bool Equals(object obj) => obj is Material other && Equals(other);


        public static readonly Material Default = new Material()
        {
            Shader = MaterialShader.Light,
            CullMode = CullMode.Back,
            DepthTest = true,
            Metallic = 0.5f,
            Roughness = 0.5f,
            AmbientOcclusion = 1f,
            Color = Color4.White,
            Bloom = true,
            Reflection = true,
            ReceiveShadow = true
        };
    }
}
