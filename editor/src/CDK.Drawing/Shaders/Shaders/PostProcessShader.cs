using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.IO;

namespace CDK.Drawing
{
    public class PostProcessShader : IDisposable
    {
        private ProgramBranch _programs;
        private Program _bloomProgram;

        public PostProcessShader()
        {
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "post_process_vs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "post_process_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, vertexShaderCode);
            _programs.Attach(ShaderType.FragmentShader, fragmentShaderCode);

            vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "bloom_vs.glsl"));
            fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "bloom_fs.glsl"));
            var vertexShader = new Shader(ShaderType.VertexShader, vertexShaderCode);
            var fragmentShader = new Shader(ShaderType.FragmentShader, fragmentShaderCode);

            _bloomProgram = new Program();
            _bloomProgram.Attach(vertexShader);
            _bloomProgram.Attach(fragmentShader);
            _bloomProgram.Link();

            vertexShader.Dispose();
            fragmentShader.Dispose();
        }

        public void Dispose()
        {
            _programs.Dispose();
            _bloomProgram.Dispose();
        }

        private void Bloom(GraphicsApi api, RenderTarget target, ref int pass)
        {
            var width = Math.Min(target.Width, target.Height);
            while ((width >> pass) <= 1) pass--;
            if (pass <= 0)
            {
                pass = 0;
                return;
            }

            var targetDesc = new RenderTargetDescription()
            {
                Width = target.Width,
                Height = target.Height,
                Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment1,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment1),
                        Texture = true,
                        TextureWrapS = TextureWrapMode.ClampToEdge,
                        TextureWrapT = TextureWrapMode.ClampToEdge,
                        TextureWrapR = TextureWrapMode.ClampToEdge,
                        TextureMinFilter = TextureMinFilter.Linear,
                        TextureMagFilter = TextureMagFilter.Linear
                    }
                }
            };

            var targets = new RenderTarget[pass + 1];
            targets[0] = target;

            for (var i = 1; i <= pass; i++)
            {
                targetDesc.Width /= 2;
                targetDesc.Height /= 2;
                targets[i] = RenderTargets.NewTemporary(targetDesc);
            }

            var vertices = VertexArrays.GetScreen2D();

            _bloomProgram.Use(api);

            for (var i = 0; i < pass; i++)
            {
                targets[i + 1].Focus(api);
                targets[i + 1].SetDrawBuffers(api, 1);

                var texture = targets[i].GetTextureAttachment(FramebufferAttachment.ColorAttachment1);

                var delta = new Vector2(1f / texture.Width, 1f / texture.Height);
                var dataBuffer = Buffers.FromData(delta, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(TextureTarget.Texture2D, 0, texture.Object);
                vertices.DrawArrays(api, PrimitiveMode.TriangleStrip, 0, 4);
            }

            api.ApplyBlendMode(BlendMode.Add);

            for (var i = pass; i > 0; i--)
            {
                targets[i - 1].Focus(api);
                targets[i - 1].SetDrawBuffers(api, 1);

                var texture = targets[i].GetTextureAttachment(FramebufferAttachment.ColorAttachment1);

                var delta = new Vector2(0.5f / texture.Width, 0.5f / texture.Height);
                var dataBuffer = Buffers.FromData(delta, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(TextureTarget.Texture2D, 0, texture.Object);
                vertices.DrawArrays(api, PrimitiveMode.TriangleStrip, 0, 4);

                ResourcePool.Instance.Remove(targets[i]);
            }

            api.ApplyBlendMode(BlendMode.None);
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct UniformData : IEquatable<UniformData>
        {
            public Vector2 Resolution;
            public float BloomIntensity;
            public float GammaInv;
            public float Exposure;

            public static bool operator ==(in UniformData a, in UniformData b) => a.Equals(b);
            public static bool operator !=(in UniformData a, in UniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Resolution.GetHashCode());
                hash.Combine(BloomIntensity.GetHashCode());
                hash.Combine(GammaInv.GetHashCode());
                hash.Combine(Exposure.GetHashCode());
                return hash;
            }

            public bool Equals(UniformData other)
            {
                return Resolution == other.Resolution &&
                    BloomIntensity == other.BloomIntensity &&
                    GammaInv == other.GammaInv &&
                    Exposure == other.Exposure;
            }

            public override bool Equals(object obj)
            {
                return obj is UniformData other && Equals(other);
            }
        }

        public void Draw(GraphicsApi api, RenderTarget src, RenderTarget dest, int bloomPass, float bloomIntensity, float exposure, float gamma)
        {
            Debug.Assert(!src.HasViwport, "source target must not have viewport");

            RenderTarget captureTarget0 = null;
            RenderTarget captureTarget1 = null;
            Texture colorMap = null;
            Texture bloomMap = null;

            if (src != dest) colorMap = src.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            var bloomSupported = bloomIntensity > 0 && src.BloomSupported;

            if (bloomSupported && src.Samples == 1) bloomMap = src.GetTextureAttachment(FramebufferAttachment.ColorAttachment1);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            if (colorMap == null || (bloomSupported && bloomMap == null))
            {
                var captureDesc = new RenderTargetDescription()
                {
                    Width = src.Width,
                    Height = src.Height,
                    Attachments = new RenderTargetAttachmentDescription[]
                    {
                        new RenderTargetAttachmentDescription
                        {
                            Texture = true,
                            TextureWrapS = TextureWrapMode.ClampToEdge,
                            TextureWrapR = TextureWrapMode.ClampToEdge,
                            TextureWrapT = TextureWrapMode.ClampToEdge
                        }
                    }
                };
                if (colorMap == null)
                {
                    captureDesc.Attachments[0].Format = src.GetFormat(FramebufferAttachment.ColorAttachment0);
                    captureDesc.Attachments[0].Attachment = FramebufferAttachment.ColorAttachment0;
                    captureTarget0 = RenderTargets.NewTemporary(captureDesc);
                    src.Blit(captureTarget0, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest, 0, 0);
                    colorMap = captureTarget0.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
                }
                if (bloomSupported)
                {
                    if (bloomMap == null)
                    {
                        captureDesc.Attachments[0].Format = src.GetFormat(FramebufferAttachment.ColorAttachment1);
                        captureDesc.Attachments[0].Attachment = FramebufferAttachment.ColorAttachment1;
                        captureDesc.Attachments[0].TextureMinFilter = TextureMinFilter.Linear;
                        captureDesc.Attachments[0].TextureMagFilter = TextureMagFilter.Linear;
                        captureTarget1 = RenderTargets.NewTemporary(captureDesc);
                        src.Blit(captureTarget1, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest, 1, 1);
                        Bloom(api, captureTarget1, ref bloomPass);
                        bloomMap = captureTarget1.GetTextureAttachment(FramebufferAttachment.ColorAttachment1);
                    }
                    else
                    {
                        Bloom(api, src, ref bloomPass);
                    }
                    bloomIntensity /= bloomPass + 1;
                }
            }

            dest.Focus(api);
            dest.SetDrawBuffers(api, 0);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);

            _programs.AddBranch("UsingMultisample", colorMap.Samples, 17, ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBloom", bloomMap != null, ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingGamma", gamma != 1, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var data = new UniformData
            {
                Resolution = new Vector2(src.Width, src.Height),
                BloomIntensity = bloomIntensity,
                GammaInv = 1.0f / gamma,
                Exposure = exposure
            };
            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
            api.BindTextureBase(colorMap.Target, 0, colorMap.Object);
            if (bloomMap != null) api.BindTextureBase(bloomMap.Target, 1, bloomMap.Object);

            VertexArrays.GetScreen2D().DrawArrays(api, PrimitiveMode.TriangleStrip, 0, 4);

            if (captureTarget0 != null) ResourcePool.Instance.Remove(captureTarget0);
            if (captureTarget1 != null) ResourcePool.Instance.Remove(captureTarget1);
        }
    }
}

