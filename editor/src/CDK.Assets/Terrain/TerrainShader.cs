using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    internal class TerrainShader : IDisposable
    {
        private ProgramBranch _programs;
        private ProgramBranch _shadowPrograms;
        private ProgramBranch _waterPrograms;

        public TerrainShader()
        {
            var commonShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain.glsl"));
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, ShaderCode.Base, commonShaderCode, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, ShaderCode.FSBase, commonShaderCode, fragmentShaderCode);

            var shadowCommonShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow.glsl"));
            var geometryShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow_gs.glsl"));
            fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "shadow_fs.glsl"));

            _shadowPrograms = new ProgramBranch();
            _shadowPrograms.Attach(ShaderType.VertexShader, ShaderCode.Base, shadowCommonShaderCode, commonShaderCode, vertexShaderCode);
            _shadowPrograms.Attach(ShaderType.GeometryShader, shadowCommonShaderCode, geometryShaderCode);
            _shadowPrograms.Attach(ShaderType.FragmentShader, ShaderCode.Base, shadowCommonShaderCode, fragmentShaderCode);

            commonShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain_water.glsl"));
            vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain_water_vs.glsl"));
            fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "terrain_water_fs.glsl"));

            _waterPrograms = new ProgramBranch();
            _waterPrograms.Attach(ShaderType.VertexShader, ShaderCode.Base, commonShaderCode, vertexShaderCode);
            _waterPrograms.Attach(ShaderType.FragmentShader, ShaderCode.FSBase, commonShaderCode, fragmentShaderCode);
        }

        public void Dispose()
        {
            _programs.Dispose();
            _shadowPrograms.Dispose();
            _waterPrograms.Dispose();
        }

        public const int TextureBindingSurfaceAmbientOcclusionMap = RenderState.TextureBindingExtension;
        public const int TextureBindingSurfaceIntensityMap = RenderState.TextureBindingExtension + 1;

        public const int TextureBindingWaterDestMap = RenderState.TextureBindingExtension;
        public const int TextureBindingWaterDepthMap = RenderState.TextureBindingExtension + 1;
        public const int TextureBindingWaterFoamMap = RenderState.TextureBindingExtension + 2;

        public const int UniformBlockBindingData = RenderState.UniformBlockBindingExtension;
        public const int UniformBlockBindingSubData = RenderState.UniformBlockBindingExtension + 1;

        [StructLayout(LayoutKind.Sequential)]
        public struct SurfaceUniformData : IEquatable<SurfaceUniformData>
        {
            public Matrix4x4 World;
            public Vector2 PositionScale;
            public Vector2 TerrainScale;
            public Vector2 SurfaceOffset;
            public float SurfaceScale;
            public float SurfaceRotation;
            public Color4 BaseColor;
            public float AmbientOcclusionIntensity;

            public static bool operator ==(in SurfaceUniformData a, in SurfaceUniformData b) => a.Equals(b);
            public static bool operator !=(in SurfaceUniformData a, in SurfaceUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(World.GetHashCode());
                hash.Combine(PositionScale.GetHashCode());
                hash.Combine(TerrainScale.GetHashCode());
                hash.Combine(SurfaceOffset.GetHashCode());
                hash.Combine(SurfaceScale.GetHashCode());
                hash.Combine(SurfaceRotation.GetHashCode());
                hash.Combine(BaseColor.GetHashCode());
                hash.Combine(AmbientOcclusionIntensity.GetHashCode());
                return hash;
            }

            public bool Equals(SurfaceUniformData other)
            {
                return World == other.World &&
                    PositionScale == other.PositionScale &&
                    TerrainScale == other.TerrainScale &&
                    SurfaceOffset == other.SurfaceOffset &&
                    SurfaceScale == other.SurfaceScale &&
                    SurfaceRotation == other.SurfaceRotation &&
                    BaseColor == other.BaseColor &&
                    AmbientOcclusionIntensity == other.AmbientOcclusionIntensity;
            }

            public override bool Equals(object obj)
            {
                return obj is SurfaceUniformData other && Equals(other);
            }
        }

        public void DrawSurface(GraphicsApi api, in RenderState state, in Matrix4x4 world, in Color4 color, TerrainAsset asset, TerrainSurface surface, VertexArray vertices, Texture ambientOcclusionMap, Texture intensityMap)
        {
            state.ApplyBranch(_programs, false, false, false);

            _programs.AddBranch("UsingSurface", surface != null, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingSurfaceRotation", surface != null && surface.Rotation != 0, ProgramBranchMask.Vertex);
            _programs.AddBranch("UsingTriPlaner", surface != null && surface.TriPlaner, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api);

            if (state.UsingLight)
            {
                api.BindTextureBase(TextureTarget.Texture2D, TextureBindingSurfaceAmbientOcclusionMap, ambientOcclusionMap.Object);
            }

            var data = new SurfaceUniformData()
            {
                World = world,
                TerrainScale = new Vector2(1.0f / (asset.Width * asset.VertexCell), 1.0f / (asset.Height * asset.VertexCell)),
                PositionScale = new Vector2((float)asset.Grid / asset.VertexCell, asset.Grid),
                BaseColor = color * state.Material.Color,
                AmbientOcclusionIntensity = asset.AmbientOcclusionIntensity,
            };

            if (surface != null)
            {
                var surfaceScale = 0f;

                if (surface.Material.HasTexture)
                {
                    var width = surface.Material.Origin.Width;

                    if (width != 0) surfaceScale = surface.Scale / width;
                }
                data.SurfaceOffset = asset.SurfaceOffset;
                data.SurfaceScale = surfaceScale;
                data.SurfaceRotation = surface.Rotation * MathUtil.ToRadians;

                api.BindTextureBase(TextureTarget.Texture2D, TextureBindingSurfaceIntensityMap, intensityMap.Object);
            }
            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingData, dataBuffer.Object);

            vertices.DrawElements(api, PrimitiveMode.Triangles);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShadowUniformData : IEquatable<ShadowUniformData>
        {
            public Matrix4x4 World;
            public Vector2 PositionScale;

            public static bool operator ==(in ShadowUniformData a, in ShadowUniformData b) => a.Equals(b);
            public static bool operator !=(in ShadowUniformData a, in ShadowUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(World.GetHashCode());
                hash.Combine(PositionScale.GetHashCode());
                return hash;
            }

            public bool Equals(ShadowUniformData other)
            {
                return World == other.World &&
                    PositionScale == other.PositionScale;
            }

            public override bool Equals(object obj)
            {
                return obj is ShadowUniformData other && Equals(other);
            }
        }

        public void DrawShadow(GraphicsApi api, in RenderState state, in Matrix4x4 world, TerrainAsset asset, VertexArray vertices)
        {
            var param = (ShadowRendererParam)state.RendererParam;

            _shadowPrograms.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            if (param.Mode == ShadowMode.Point)
            {
                _programs.AddLink(ProgramBranchMask.Geometry);
                _programs.AddBranch("UsingPrimitive", 3, 4, ProgramBranchMask.Geometry);
            }

            _shadowPrograms.AddBranch("UsingShadowMode", (int)param.Mode, 4, ProgramBranchMask.Vertex | ProgramBranchMask.Geometry | ProgramBranchMask.Fragment);

            var program = _shadowPrograms.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api);

            api.BindBufferBase(BufferTarget.UniformBuffer, RenderState.UniformBlockBindingShadow, param.UniformBuffer.Object);

            var data = new ShadowUniformData()
            {
                World = world,
                PositionScale = new Vector2((float)asset.Grid / asset.VertexCell, asset.Grid)
            };
            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingData, dataBuffer.Object);

            vertices.DrawElements(api, PrimitiveMode.Triangles);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WaterUniformData : IEquatable<WaterUniformData>
        {
            public Matrix4x4 World;
            public float PositionScale;
            public float PerturbIntensity;
            public Vector2 TextureScale;
            public Vector2 TextureFlowForward;
            public Vector2 TextureFlowCross;
            public Vector2 FoamScale;
            public Vector2 FoamFlowForward;
            public Vector2 FoamFlowCross;
            public float FoamIntensity;
            public float FoamDepth;
            public Color4 BaseColor;
            public Color4 ShallowColor;
            public Vector2 Wave;
            public float DepthMax;

            public static bool operator ==(in WaterUniformData a, in WaterUniformData b) => a.Equals(b);
            public static bool operator !=(in WaterUniformData a, in WaterUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(World.GetHashCode());
                hash.Combine(PositionScale.GetHashCode());
                hash.Combine(PerturbIntensity.GetHashCode());
                hash.Combine(TextureScale.GetHashCode());
                hash.Combine(TextureFlowForward.GetHashCode());
                hash.Combine(TextureFlowCross.GetHashCode());
                hash.Combine(FoamScale.GetHashCode());
                hash.Combine(FoamFlowForward.GetHashCode());
                hash.Combine(FoamFlowCross.GetHashCode());
                hash.Combine(FoamIntensity.GetHashCode());
                hash.Combine(FoamDepth.GetHashCode());
                hash.Combine(BaseColor.GetHashCode());
                hash.Combine(ShallowColor.GetHashCode());
                hash.Combine(Wave.GetHashCode());
                hash.Combine(DepthMax.GetHashCode());
                return hash;
            }

            public bool Equals(WaterUniformData other)
            {
                return World == other.World &&
                    PositionScale == other.PositionScale &&
                    PerturbIntensity == other.PerturbIntensity &&
                    TextureScale == other.TextureScale &&
                    TextureFlowForward == other.TextureFlowForward &&
                    TextureFlowCross == other.TextureFlowCross &&
                    FoamScale == other.FoamScale &&
                    FoamFlowForward == other.FoamFlowForward &&
                    FoamFlowCross == other.FoamFlowCross &&
                    FoamIntensity == other.FoamIntensity &&
                    FoamDepth == other.FoamDepth &&
                    BaseColor == other.BaseColor &&
                    ShallowColor == other.ShallowColor &&
                    Wave == other.Wave &&
                    DepthMax == other.DepthMax;
            }

            public override bool Equals(object obj)
            {
                return obj is WaterUniformData other && Equals(other);
            }
        }

        public void DrawWater(GraphicsApi api, RenderState state, in Matrix4x4 world, in Color4 color, TerrainAsset asset, TerrainWater water, VertexArray vertices, float progress, float scale, Texture destMap, Texture depthMap)
        {
           state.Material.NormalMap = water.Material.Origin?.NormalMap.Texture;     //라이트 적용이 없어도 노멀맵을 사용해야 함

            var foamMap = water.FoamTexture?.Content.Texture;

            state.ApplyBranch(_waterPrograms, false, false, false);

            _waterPrograms.AddBranch("UsingWave", water.WaveDistance != 0 && water.WaveAltitude != 0, ProgramBranchMask.Vertex);
            _waterPrograms.AddBranch("UsingFoam", foamMap != null, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _waterPrograms.AddBranch("UsingCross", water.CrossSpeed != 0, ProgramBranchMask.Fragment);
            _waterPrograms.AddBranch("UsingDepth", water.DepthMax != 0, ProgramBranchMask.Fragment);
            _waterPrograms.AddBranch("UsingTransparency", water.Material.HasOpacity || (water.DepthMax != 0 && water.ShallowColor.A < 1), ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _waterPrograms.AddBranch("UsingFoamDepth", foamMap != null && water.FoamDepth != 0, ProgramBranchMask.Fragment);

            var program = _waterPrograms.EndBranch();

            program.Use(api);

            state.ApplyUniforms(api);

            var radian = water.Angle * MathUtil.ToRadians;
            var forward = new Vector2(water.ForwardSpeed * (float)Math.Sin(radian), water.ForwardSpeed * (float)Math.Cos(radian));
            var cross = new Vector2(water.CrossSpeed * -(float)Math.Cos(radian), water.CrossSpeed * (float)Math.Sin(radian));

            api.BindTextureBase(TextureTarget.Texture2D, TextureBindingWaterDestMap, destMap.Object);
            api.BindTextureBase(TextureTarget.Texture2D, TextureBindingWaterDepthMap, depthMap.Object);

            var data = new WaterUniformData
            {
                World = world,
                PositionScale = asset.Grid,
                BaseColor = color * state.Material.Color,
                ShallowColor = water.ShallowColor,
                Wave = new Vector2(water.WaveDistance * MathUtil.ToRadians, water.WaveAltitude),
                DepthMax = water.DepthMax * scale
            };
            if (state.UsingMap)
            {
                var width = water.Material.Origin.Width;
                var height = water.Material.Origin.Height;
                data.TextureScale = new Vector2(asset.Grid / (water.TextureScale * width), asset.Grid / (water.TextureScale * height));
                data.TextureFlowForward = new Vector2(forward.X / width, forward.Y / height);
                data.TextureFlowCross = new Vector2(cross.X / width, cross.Y / height);
                data.PerturbIntensity = water.PerturbIntensity;
            }
            if (foamMap != null)
            {
                api.BindTextureBase(TextureTarget.Texture2D, TextureBindingWaterFoamMap, foamMap.Object);

                var width = foamMap.Width;
                var height = foamMap.Height;
                data.FoamScale = new Vector2(asset.Grid / (water.FoamScale * width), asset.Grid / (water.FoamScale * height));
                data.FoamFlowForward = new Vector2(forward.X / width, forward.Y / height);
                data.FoamFlowCross = new Vector2(cross.X / width, cross.Y / height);
                data.FoamIntensity = water.FoamIntensity;
                data.FoamDepth = water.FoamDepth * scale;
            }
            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingData, dataBuffer.Object);

            dataBuffer = Buffers.FromData(progress, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingSubData, dataBuffer.Object);

            vertices.DrawElements(api, PrimitiveMode.Triangles);
        }
    }
}
