using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public struct RenderState : IEquatable<RenderState>
    {
        public RenderTarget Target;
        public IRenderer Renderer;
        public Camera Camera;
        public Material Material;
        public Color3 FogColor;
        public float FogNear;
        public float FogFar;
        public float BloomThreshold;
        public float Brightness;
        public float Contrast;
        public float Saturation;
        public DepthMode DepthMode;
        public PolygonMode PolygonMode;
        public StencilMode StencilMode;
        public byte StencilDepth;
        public byte Layer;
        public byte StrokeWidth;
        public StrokeMode StrokeMode;
        public Color4 StrokeColor;
        public float LineWidth;
        public Rectangle Scissor;
        public LightSpaceState LightSpaceState;
        public object RendererParam;

        public void Validate() => Renderer.Validate(ref this);
        public bool Visible => Renderer.Visible(this);
        public bool UsingMap => Material.ColorMap != null || Material.NormalMap != null || Material.MaterialMap != null || Material.EmissiveMap != null;
        public bool UsingVertexNormal => UsingLight || Material.Shader == MaterialShader.Distortion;
        public bool UsingVertexTangent => Material.NormalMap != null;
        public bool UsingFog => FogFar > FogNear;
        public bool UsingStroke => (Material.BlendMode == BlendMode.Alpha || Material.BlendMode == BlendMode.None) && StrokeWidth != 0 && StrokeColor.A != 0;
        public bool UsingLight => LightSpaceState != null && Material.ReceiveLight;
        public bool UsingShadow => LightSpaceState != null && LightSpaceState.UsingShadow && Material.ReceiveShadow;
        public bool UsingShadow2D => LightSpaceState != null && LightSpaceState.UsingShadow && Material.ReceiveShadow2D;
        public LightMode LightMode => UsingLight ? LightSpaceState.Mode : 0;
        public bool UsingDirectionalLight => UsingLight && LightSpaceState.UsingDirectionalLight;           //TODO:FIX? 라이트를 안받고 그림자, 2d의 경우에는 더욱 그렇다.
        public bool UsingPointLight => UsingLight && LightSpaceState.UsingPointLight;
        public bool UsingSpotLight => UsingLight && LightSpaceState.UsingSpotLight;
        public static bool operator ==(in RenderState left, in RenderState right) => left.Equals(right);
        public static bool operator !=(in RenderState left, in RenderState right) => !left.Equals(right);
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Target.GetHashCode());
            hash.Combine(Renderer.GetHashCode());
            hash.Combine(Camera.GetHashCode());
            hash.Combine(Material.GetHashCode());
            hash.Combine(FogColor.GetHashCode());
            hash.Combine(FogNear.GetHashCode());
            hash.Combine(FogFar.GetHashCode());
            hash.Combine(BloomThreshold.GetHashCode());
            hash.Combine(Brightness.GetHashCode());
            hash.Combine(Contrast.GetHashCode());
            hash.Combine(Saturation.GetHashCode());
            hash.Combine(PolygonMode.GetHashCode());
            hash.Combine(StencilMode.GetHashCode());
            hash.Combine(StencilDepth.GetHashCode());
            hash.Combine(Layer.GetHashCode());
            hash.Combine(StrokeWidth.GetHashCode());
            hash.Combine(StrokeMode.GetHashCode());
            hash.Combine(StrokeColor.GetHashCode());
            hash.Combine(LineWidth.GetHashCode());
            hash.Combine(Scissor.GetHashCode());
            if (LightSpaceState != null) hash.Combine(LightSpaceState.GetHashCode());
            if (RendererParam != null) hash.Combine(RendererParam.GetHashCode());
            return hash;
        }

        public bool Equals(RenderState other)
        {
            return Target == other.Target &&
                Renderer == other.Renderer &&
                Camera == other.Camera &&
                Material == other.Material &&
                FogColor == other.FogColor &&
                FogNear == other.FogNear &&
                FogFar == other.FogFar &&
                BloomThreshold == other.BloomThreshold &&
                Brightness == other.Brightness &&
                Contrast == other.Contrast &&
                Saturation == other.Saturation &&
                PolygonMode == other.PolygonMode &&
                Layer == other.Layer &&
                StrokeWidth == other.StrokeWidth &&
                StrokeMode == other.StrokeMode &&
                StrokeColor == other.StrokeColor &&
                LineWidth == other.LineWidth &&
                Scissor == other.Scissor &&
                LightSpaceState == other.LightSpaceState &&
                RendererParam == other.RendererParam;
        }

        public override bool Equals(object obj) => obj is RenderState other && Equals(other);

        public void ApplyBranch(ProgramBranch programs, bool usingBone, bool usingInstance, bool usingVertexColor)
        {
            programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            programs.AddBranch("UsingLight", UsingLight, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingNormal", UsingVertexNormal, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingMap", UsingMap, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingColorMap", Material.ColorMap != null, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingNormalMap", Material.NormalMap != null, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            programs.AddBranch("UsingDisplacementMap", Material.DisplacementScale != 0, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingMaterialMap", (int)Material.MaterialMapComponents, 8, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingEmissiveMap", Material.EmissiveMap != null, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingBone", usingBone, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingInstance", usingInstance, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingVertexColor", usingVertexColor, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingPerspective", Camera.Fov != 0, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingAlphaTest", Material.AlphaTest, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingDepthBias", Material.DepthBias != 0, ProgramBranchMask.Vertex);
            programs.AddBranch("UsingEmission", Material.Emission != Color3.Black, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingUVOffset", Material.UVOffset != Vector2.Zero, ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            programs.AddBranch("UsingFog", UsingFog, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingBrightness", Brightness != 0, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingContrast", Contrast != 1, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingSaturation", Saturation != 1, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingRim", Material.Rim != 0, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingReflection", Material.Reflection, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingLightMode", (int)LightMode, 8, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingDirectionalLight", UsingDirectionalLight, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingPointLight", UsingPointLight, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingSpotLight", UsingSpotLight, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingShadow", UsingShadow, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingShadow2D", UsingShadow2D, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingBloom", Material.Bloom, ProgramBranchMask.Fragment);
            programs.AddBranch("UsingBloomSupported", Target.BloomSupported, ProgramBranchMask.Fragment);
        }

        public const int VertexAttribPosition = 0;
        public const int VertexAttribColor = 1;
        public const int VertexAttribTexCoord = 2;
        public const int VertexAttribNormal = 3;
        public const int VertexAttribTangent = 4;
        public const int VertexAttribBoneIndices = 5;
        public const int VertexAttribBoneWeights = 6;
        public const int VertexAttribInstanceModel0 = 7;
        public const int VertexAttribInstanceModel1 = 8;
        public const int VertexAttribInstanceModel2 = 9;
        public const int VertexAttribInstanceModel3 = 10;
        public const int VertexAttribInstanceColor = 11;
        public const int VertexAttribInstanceBoneOffset = 12;

        public const int TextureBindingColorMap = 0;
        public const int TextureBindingNormalMap = 1;
        public const int TextureBindingMaterialMap = 2;
        public const int TextureBindingEmissiveMap = 3;
        public const int TextureBindingEnvMap = 4;
        public const int TextureBindingBrdfMap = 5;
        public const int TextureBindingScreenMap = 6;
        public const int TextureBindingDirectionalShadowMap = 7;
        public const int TextureBindingDirectionalShadow2DMap = 10;
        public const int TextureBindingPointShadowMap = 13;
        public const int TextureBindingSpotShadowMap = 16;
        public const int TextureBindingExtension = 19;

        public const int ImageBindingPointLightClusterMap = 0;
        public const int ImageBindingSpotLightClusterMap = 1;
        public const int ImageBindingExtension = 2;

        public const int UniformBlockBindingEnv = 0;
        public const int UniformBlockBindingCamera = 1;
        public const int UniformBlockBindingModel = 2;
        public const int UniformBlockBindingBone = 3;
        public const int UniformBlockBindingLight = 4;
        public const int UniformBlockBindingDirectionalLight = 5;
        public const int UniformBlockBindingPointLight = 6;
        public const int UniformBlockBindingSpotLight = 7;
        public const int UniformBlockBindingSpotShadow = 8;
        public const int UniformBlockBindingShadow = 9;
        public const int UniformBlockBindingStroke = 10;
        public const int UniformBlockBindingExtension = 11;

        [StructLayout(LayoutKind.Sequential)]
        private struct EnvUniformData : IEquatable<EnvUniformData>
        {
            public Vector2 Resolution;
            public float FogNear;
            public float FogFar;
            public Color3 FogColor;

            public static bool operator ==(in EnvUniformData a, in EnvUniformData b) => a.Equals(b);
            public static bool operator !=(in EnvUniformData a, in EnvUniformData b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Resolution.GetHashCode());
                hash.Combine(FogNear.GetHashCode());
                hash.Combine(FogFar.GetHashCode());
                hash.Combine(FogColor.GetHashCode());
                return hash;
            }

            public bool Equals(EnvUniformData other)
            {
                return Resolution == other.Resolution &&
                    FogNear == other.FogNear &&
                    FogFar == other.FogFar &&
                    FogColor == other.FogColor;
            }

            public override bool Equals(object obj)
            {
                return obj is EnvUniformData other && Equals(other);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CameraUniformData : IEquatable<CameraUniformData>
        {
            public Matrix4x4 ViewProjection;
            public Vector3 Eye;
            public float Near;
            public float Far;

            public static bool operator ==(in CameraUniformData a, in CameraUniformData b) => a.Equals(b);
            public static bool operator !=(in CameraUniformData a, in CameraUniformData b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(ViewProjection.GetHashCode());
                return hash;
            }

            public bool Equals(CameraUniformData other)
            {
                return ViewProjection == other.ViewProjection;
            }

            public override bool Equals(object obj)
            {
                return obj is CameraUniformData other && Equals(other);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ModelUniformData : IEquatable<ModelUniformData>
        {
            public Vector4 MaterialFactor;
            public Color3 EmissiveFactor;
            public float DisplacementScale;
            public Vector2 UVOffset;
            public float DistortionScale;
            public float BloomThreshold;
            public float Brightness;
            public float Contrast;
            public float Saturation;
            public float AlphaTestBias;
            public float DepthBias;
            public float DepthBiasPerspective;

            public static bool operator ==(in ModelUniformData a, in ModelUniformData b) => a.Equals(b);
            public static bool operator !=(in ModelUniformData a, in ModelUniformData b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(MaterialFactor.GetHashCode());
                hash.Combine(EmissiveFactor.GetHashCode());
                hash.Combine(DisplacementScale.GetHashCode());
                hash.Combine(UVOffset.GetHashCode());
                hash.Combine(DistortionScale.GetHashCode());
                hash.Combine(BloomThreshold.GetHashCode());
                hash.Combine(Brightness.GetHashCode());
                hash.Combine(Contrast.GetHashCode());
                hash.Combine(Saturation.GetHashCode());
                hash.Combine(AlphaTestBias.GetHashCode());
                hash.Combine(DepthBias.GetHashCode());
                hash.Combine(DepthBiasPerspective.GetHashCode());
                return hash;
            }

            public bool Equals(ModelUniformData other)
            {
                return MaterialFactor == other.MaterialFactor &&
                    EmissiveFactor == other.EmissiveFactor &&
                    DisplacementScale == other.DisplacementScale &&
                    UVOffset == other.UVOffset &&
                    DistortionScale == other.DistortionScale &&
                    BloomThreshold == other.BloomThreshold &&
                    Brightness == other.Brightness &&
                    Contrast == other.Contrast &&
                    Saturation == other.Saturation &&
                    AlphaTestBias == other.AlphaTestBias &&
                    DepthBias == other.DepthBias &&
                    DepthBiasPerspective == other.DepthBiasPerspective;
            }

            public override bool Equals(object obj)
            {
                return obj is ModelUniformData other && Equals(other);
            }
        }

        public void ApplyUniforms(GraphicsApi api, Buffer boneBuffer = null)
        {
            var envData = new EnvUniformData
            {
                Resolution = new Vector2(api.CurrentTarget.Width, api.CurrentTarget.Height),
                FogNear = FogNear,
                FogFar = FogFar,
                FogColor = FogColor
            };
            var cameraData = new CameraUniformData
            {
                ViewProjection = Camera.ViewProjection,
                Eye = Camera.Position,
                Near = Camera.Near,
                Far = Camera.Far
            };
            var modelData = new ModelUniformData
            {
                MaterialFactor = new Vector4(Material.Metallic, Material.Roughness, Material.AmbientOcclusion, Material.Rim),
                EmissiveFactor = Material.Emission,
                UVOffset = Material.UVOffset,
                DisplacementScale = Material.DisplacementScale,
                DistortionScale = Material.DistortionScale,
                BloomThreshold = BloomThreshold,
                Brightness = Brightness,
                Contrast = Contrast,
                Saturation = Saturation,
                AlphaTestBias = Material.AlphaTestBias,
                DepthBias = Material.DepthBias
            };
            if (Material.DepthBias != 0 && Camera.Fov != 0) modelData.DepthBiasPerspective = Material.DepthBias * (1.0f + Camera.Near / Camera.Far);

            var envDataBuffer = Buffers.FromData(envData, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingEnv, envDataBuffer.Object);

            var cameraDataBuffer = Buffers.FromData(cameraData, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingCamera, cameraDataBuffer.Object);

            var modelDataBuffer = Buffers.FromData(modelData, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingModel, modelDataBuffer.Object);

            if (boneBuffer != null) api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingBone, boneBuffer.Object);

            if (Material.ColorMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingColorMap, Material.ColorMap.Object);
            if (Material.NormalMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingNormalMap, Material.NormalMap.Object);
            if (Material.MaterialMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingMaterialMap, Material.MaterialMap.Object);
            if (Material.EmissiveMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingEmissiveMap, Material.EmissiveMap.Object);

            if (UsingLight)
            {
                api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingLight, LightSpaceState.LightBuffer.Object);

                if (LightSpaceState.UsingDirectionalLight)
                {
                    api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingDirectionalLight, LightSpaceState.DirectionalLightBuffer.Object);

                    if (LightSpaceState.UsingShadow)
                    {
                        if (Material.ReceiveShadow)
                        {
                            for (var i = 0; i < Drawing.LightSpace.MaxDirectionalLightCount; i++)
                            {
                                var shadowMap = LightSpaceState.GetDirectionalShadowMap(i);
                                if (shadowMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingDirectionalShadowMap + i, shadowMap.Object);
                            }
                        }
                        if (Material.ReceiveShadow2D)
                        {
                            for (var i = 0; i < Drawing.LightSpace.MaxDirectionalLightCount; i++)
                            {
                                var shadowMap = LightSpaceState.GetDirectionalShadow2DMap(i);
                                if (shadowMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingDirectionalShadow2DMap + i, shadowMap.Object);
                            }
                        }
                    }
                }
                if (LightSpaceState.UsingPointLight)
                {
                    api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingPointLight, LightSpaceState.PointLightBuffer.Object);

                    api.BindImageTexture(ImageBindingPointLightClusterMap, LightSpaceState.PointLightClusterMap.Object, 0, false, 0, TextureAccess.ReadOnly, LightSpaceState.PointLightClusterMap.Format);

                    if (LightSpaceState.UsingShadow && Material.ReceiveShadow)
                    {
                        for (var i = 0; i < Drawing.LightSpace.MaxPointShadowCount; i++)
                        {
                            var shadowMap = LightSpaceState.GetPointShadowMap(i);
                            if (shadowMap != null) api.BindTextureBase(TextureTarget.TextureCubeMap, TextureBindingPointShadowMap + i, shadowMap.Object);
                        }
                    }
                }
                if (LightSpaceState.UsingSpotLight)
                {
                    api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingSpotLight, LightSpaceState.SpotLightBuffer.Object);

                    api.BindImageTexture(ImageBindingSpotLightClusterMap, LightSpaceState.SpotLightClusterMap.Object, 0, false, 0, TextureAccess.ReadOnly, LightSpaceState.SpotLightClusterMap.Format);

                    if (LightSpaceState.UsingShadow && Material.ReceiveShadow)
                    {
                        if (LightSpaceState.SpotShadowBuffer != null) api.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockBindingSpotShadow, LightSpaceState.SpotShadowBuffer.Object);

                        for (var i = 0; i < Drawing.LightSpace.MaxSpotShadowCount; i++)
                        {
                            var shadowMap = LightSpaceState.GetSpotShadowMap(i);
                            if (shadowMap != null) api.BindTextureBase(TextureTarget.Texture2D, TextureBindingSpotShadowMap + i, shadowMap.Object);
                        }
                    }
                }

                if (Material.Reflection && LightSpaceState.EnvMap != null) api.BindTextureBase(TextureTarget.TextureCubeMap, TextureBindingEnvMap, LightSpaceState.EnvMap.Object);
                api.BindTextureBase(TextureTarget.Texture2D, TextureBindingBrdfMap, (LightSpaceState.BRDFMap ?? Textures.BRDF).Object);
            }
        }
    }
}
