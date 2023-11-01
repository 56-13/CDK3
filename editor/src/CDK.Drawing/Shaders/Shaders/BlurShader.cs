using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.IO;

namespace CDK.Drawing
{
    public class BlurShader
    {
        private ProgramBranch _programs;

        private const int KernelRadius = 2;

        private const float IntensityToSigma = 3f;

        private const int BlurModeNormal = 0;
        private const int BlurModeCube = 1;
        private const int BlurModeDepth = 2;
        private const int BlurModeDirection = 3;
        private const int BlurModeCenter = 4;
        private const int BlurModeCount = 5;

        public BlurShader()
        {
            var vertexShaderCode = File.ReadAllText(Path.Combine("Resources", "blur_vs.glsl"));
            var geometryShaderCode = File.ReadAllText(Path.Combine("Resources", "blur_gs.glsl"));
            var fragmentShaderCode = File.ReadAllText(Path.Combine("Resources", "blur_fs.glsl"));

            _programs = new ProgramBranch();
            _programs.Attach(ShaderType.VertexShader, vertexShaderCode);
            _programs.Attach(ShaderType.GeometryShader, geometryShaderCode);
            _programs.Attach(ShaderType.FragmentShader, fragmentShaderCode);
        }

        public void Dispose() => _programs.Dispose();

        private Bounds2 QuadToBounds(RenderTarget target, Vector2[] quads)
        {
            var min = Vector2.One;
            var max = -Vector2.One;

            for (var i = 0; i < 4; i++)
            {
                min = Vector2.Min(min, quads[i]);
                max = Vector2.Max(max, quads[i]);
            }
            min = Vector2.Clamp(min * 0.5f + new Vector2(0.5f), Vector2.Zero, Vector2.One);
            max = Vector2.Clamp(max * 0.5f + new Vector2(0.5f), Vector2.Zero, Vector2.One);

            var viewport = target.Viewport;

            return new Bounds2(
                (int)Math.Floor(min.X * viewport.Width) + viewport.X,
                (int)Math.Floor(min.Y * viewport.Height) + viewport.Y,
                (int)Math.Ceiling((max.X - min.X) * viewport.Width),
                (int)Math.Ceiling((max.Y - min.Y) * viewport.Height));
        }

        private static Rectangle BoundsToQuad(RenderTarget target, in Bounds2 bounds)
        {
            var viewport = target.Viewport;

            return new Rectangle(
                (float)(bounds.X - viewport.X) / viewport.Width,
                (float)(bounds.Y - viewport.Y) / viewport.Height,
                (float)bounds.Width / viewport.Width,
                (float)bounds.Height / viewport.Height);
        }

        public void Draw(GraphicsApi api, Vector2[] quad, float intensity)
        {
            Draw(api, VertexArrayDraw.Array(VertexArrays.Get2D(quad), PrimitiveMode.Triangles, 0, 4), QuadToBounds(api.CurrentTarget, quad), intensity, false);
        }

        public void Draw(GraphicsApi api, float intensity)
        {
            Draw(api, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), api.CurrentTarget.Viewport, intensity, false);
        }

        public void Draw(GraphicsApi api, in Bounds2 bounds, float intensity)
        {
            Draw(api, VertexArrayDraw.Array(VertexArrays.Get2D(BoundsToQuad(api.CurrentTarget, bounds)), PrimitiveMode.TriangleStrip, 0, 4), bounds, intensity, false);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UniformData : IEquatable<UniformData>
        {
            public unsafe fixed float Kernel[4];    //with pad
            public Vector2 Direction;

            public static bool operator ==(in UniformData a, in UniformData b) => a.Equals(b);
            public static bool operator !=(in UniformData a, in UniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                unsafe
                {
                    for (var i = 0; i <= KernelRadius; i++) hash.Combine(Kernel[i].GetHashCode());
                }
                hash.Combine(Direction.GetHashCode());
                return hash;
            }

            public bool Equals(UniformData other)
            {
                unsafe
                {
                    for (var i = 0; i <= KernelRadius; i++)
                    {
                        if (Kernel[i] != other.Kernel[i]) return false;
                    }
                }
                return Direction == other.Direction;
            }

            public override bool Equals(object obj)
            {
                return obj is UniformData other && Equals(other);
            }
        }

        private void Draw(GraphicsApi api, VertexArrayDraw vertices, in Bounds2 bounds, float intensity, bool cube)
        {
            var target = api.CurrentTarget;

            target.SetDrawBuffers(api, 0);

            var viewport = target.Viewport;

            var sampleTargetDesc = new RenderTargetDescription()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Attachments = new RenderTargetAttachmentDescription[] 
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment0),
                        TextureTarget = cube ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D,
                        TextureWrapS = TextureWrapMode.ClampToEdge,
                        TextureWrapT = TextureWrapMode.ClampToEdge,
                        TextureWrapR = TextureWrapMode.ClampToEdge,
                        Texture = true
                    }
                }
            };

            RenderTarget sampleTarget0;

            var texture0 = target.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            if (texture0 == null || texture0.Samples > 1)
            {
                var r = KernelRadius * Math.Max((int)intensity, 1);

                var srcbounds = bounds;
                srcbounds.Inflate(r, r);
                srcbounds.Intersect(viewport);

                sampleTarget0 = RenderTargets.NewTemporary(sampleTargetDesc);

                target.Blit(sampleTarget0, srcbounds, srcbounds.OffsetBounds(-viewport.Origin), ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

                texture0 = sampleTarget0.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
            }
            else
            {
                sampleTarget0 = target;
            }

            var sampleTarget1 = RenderTargets.NewTemporary(sampleTargetDesc);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            if (cube) _programs.AddLink(ProgramBranchMask.Geometry);
            _programs.AddBranch("UsingBlurModeCube", cube, ProgramBranchMask.Vertex);
            _programs.AddBranch("BlurMode", cube ? BlurModeCube : BlurModeNormal, BlurModeCount, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var textureScale = new Vector2(1f / sampleTargetDesc.Width, 1f / sampleTargetDesc.Height);
            var texture1 = sampleTarget1.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            var data = new UniformData();

            while (intensity > 0)
            {
                var sigma = Math.Min(intensity, 1) * IntensityToSigma;
                var kernel = MathUtil.GaussKernel(KernelRadius, sigma);
                unsafe
                {
                    Marshal.Copy(kernel, 0, (IntPtr)data.Kernel, kernel.Length);
                }

                sampleTarget1.Focus(api);
                data.Direction = new Vector2(textureScale.X, 0);
                var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(texture0.Target, 0, texture0.Object);
                vertices.Draw(api);

                sampleTarget0.Focus(api);
                data.Direction = new Vector2(0, textureScale.Y);
                dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(texture1.Target, 0, texture1.Object);
                vertices.Draw(api);

                intensity -= 1;
            }

            if (sampleTarget0 != target)
            {
                target.Focus(api);

                Shaders.Blit.Draw(api, texture0, vertices, cube);

                ResourcePool.Instance.Remove(sampleTarget0);
            }

            ResourcePool.Instance.Remove(sampleTarget1);
        }

        public void DrawCube(GraphicsApi api, float intensity)
        {
            Draw(api, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), api.CurrentTarget.Viewport, intensity, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DepthUniformData : IEquatable<DepthUniformData>
        {
            public unsafe fixed float Kernel[4];        //with pad
            public Vector2 Direction;
            public float Range;
            public float Distance;
            public float Near;
            public float Far;

            public static bool operator ==(in DepthUniformData a, in DepthUniformData b) => a.Equals(b);
            public static bool operator !=(in DepthUniformData a, in DepthUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                unsafe
                {
                    for (var i = 0; i <= KernelRadius; i++) hash.Combine(Kernel[i].GetHashCode());
                }
                hash.Combine(Direction.GetHashCode());
                hash.Combine(Range.GetHashCode());
                hash.Combine(Distance.GetHashCode());
                hash.Combine(Near.GetHashCode());
                hash.Combine(Far.GetHashCode());
                return hash;
            }

            public bool Equals(DepthUniformData other)
            {
                unsafe
                {
                    for (var i = 0; i <= KernelRadius; i++)
                    {
                        if (Kernel[i] != other.Kernel[i]) return false;
                    }
                }
                return Direction == other.Direction &&
                    Range == other.Range &&
                    Distance == other.Distance &&
                    Near == other.Near &&
                    Far == other.Far;
            }

            public override bool Equals(object obj)
            {
                return obj is DepthUniformData other && Equals(other);
            }
        }

        public void DrawDepth(GraphicsApi api, Vector2[] quad, in Camera camera, float distance, float range, float intensity)
        {
            DrawDepth(api, VertexArrayDraw.Array(VertexArrays.Get2D(quad), PrimitiveMode.Triangles, 0, 4), QuadToBounds(api.CurrentTarget, quad), camera, distance, range, intensity);
        }

        public void DrawDepth(GraphicsApi api, in Camera camera, float distance, float range, float intensity)
        {
            DrawDepth(api, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), api.CurrentTarget.Viewport, camera, distance, range, intensity);
        }

        private void DrawDepth(GraphicsApi api, VertexArrayDraw vertices, in Bounds2 bounds, in Camera camera, float distance, float range, float intensity)
        {
            var target = api.CurrentTarget;

            var depthFormat = target.GetFormat(FramebufferAttachment.DepthStencilAttachment);

            if (depthFormat == 0) return;

            target.SetDrawBuffers(api, 0);

            var viewport = target.Viewport;

            var depthTargetDesc = new RenderTargetDescription()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.DepthStencilAttachment,
                        Format = depthFormat,
                        Texture = true
                    }
                }
            };
            var depthTarget = RenderTargets.NewTemporary(depthTargetDesc);
            target.Blit(depthTarget, bounds, bounds.OffsetBounds(-viewport.Origin), ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, BlitFramebufferFilter.Nearest);
            var depthMap = depthTarget.GetTextureAttachment(FramebufferAttachment.DepthStencilAttachment);

            var sampleTargetDesc = new RenderTargetDescription()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment0),
                        Texture = true
                    }
                }
            };

            RenderTarget sampleTarget0;

            var texture0 = target.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            if (texture0 == null || texture0.Samples > 1)
            {
                var r = KernelRadius * Math.Max((int)intensity, 1);

                Bounds2 srcbounds = bounds;
                srcbounds.Inflate(r, r);
                srcbounds.Intersect(viewport);

                sampleTarget0 = RenderTargets.NewTemporary(sampleTargetDesc);

                target.Blit(sampleTarget0, srcbounds, srcbounds.OffsetBounds(-viewport.Origin), ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

                texture0 = sampleTarget0.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
            }
            else
            {
                sampleTarget0 = target;
            }

            var sampleTarget1 = RenderTargets.NewTemporary(sampleTargetDesc);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBlurModeCube", false, ProgramBranchMask.Vertex);
            _programs.AddBranch("BlurMode", BlurModeDepth, BlurModeCount, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var data = new DepthUniformData
            {
                Distance = distance,
                Range = range,
                Near = camera.Near,
                Far = camera.Far
            };

            api.BindTextureBase(TextureTarget.Texture2D, 1, depthMap.Object);

            var texture1 = sampleTarget1.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            while (intensity > 0)
            {
                var sigma = Math.Min(intensity, 1) * IntensityToSigma;
                var kernel = MathUtil.GaussKernel(KernelRadius, sigma);
                unsafe
                {
                    Marshal.Copy(kernel, 0, (IntPtr)data.Kernel, kernel.Length);
                }

                sampleTarget1.Focus(api);
                data.Direction = new Vector2(1f / viewport.Width, 0);
                var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(TextureTarget.Texture2D, 0, texture0.Object);
                vertices.Draw(api);

                sampleTarget0.Focus(api);
                data.Direction = new Vector2(0, 1f / viewport.Height);
                dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
                api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
                api.BindTextureBase(TextureTarget.Texture2D, 0, texture1.Object);
                vertices.Draw(api);

                intensity -= 1;
            }

            if (sampleTarget0 != target)
            {
                target.Focus(api);

                Shaders.Blit.Draw(api, texture0, vertices, false);

                ResourcePool.Instance.Remove(sampleTarget0);
            }

            ResourcePool.Instance.Remove(sampleTarget1);
            ResourcePool.Instance.Remove(depthTarget);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DirectionUniformData : IEquatable<DirectionUniformData>
        {
            public Vector2 Resolution;
            public Vector2 Direction;

            public static bool operator ==(in DirectionUniformData a, in DirectionUniformData b) => a.Equals(b);
            public static bool operator !=(in DirectionUniformData a, in DirectionUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Resolution.GetHashCode());
                hash.Combine(Direction.GetHashCode());
                return hash;
            }

            public bool Equals(DirectionUniformData other)
            {
                return Resolution == other.Resolution &&
                    Direction == other.Direction;
            }

            public override bool Equals(object obj)
            {
                return obj is DirectionUniformData other && Equals(other);
            }
        }

        public void DrawDirection(GraphicsApi api, Vector2[] quad, in Vector2 dir)
        {
            DrawDirection(api, VertexArrayDraw.Array(VertexArrays.Get2D(quad), PrimitiveMode.Triangles, 0, 4), QuadToBounds(api.CurrentTarget, quad), dir);
        }

        public void DrawDirection(GraphicsApi api, in Vector2 dir)
        {
            DrawDirection(api, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), api.CurrentTarget.Viewport, dir);
        }

        private void DrawDirection(GraphicsApi api, VertexArrayDraw vertices, in Bounds2 bounds, in Vector2 dir)
        {
            var target = api.CurrentTarget;

            target.SetDrawBuffers(api, 0);

            var viewport = target.Viewport;

            var sampleTargetDesc = new RenderTargetDescription()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment0),
                        Texture = true
                    }
                }
            };

            var sampleTarget0 = RenderTargets.NewTemporary(sampleTargetDesc);

            Bounds2 srcbounds = bounds;
            srcbounds.X += ((int)Math.Ceiling(Math.Abs(dir.X)) + 1) * Math.Sign(dir.X);
            srcbounds.Y += ((int)Math.Ceiling(Math.Abs(dir.Y)) + 1) * Math.Sign(dir.Y);
            srcbounds.Intersect(viewport);

            target.Blit(sampleTarget0, srcbounds, srcbounds.OffsetBounds(-viewport.Origin), ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            var texture0 = target.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBlurModeCube", false, ProgramBranchMask.Vertex);
            _programs.AddBranch("BlurMode", BlurModeDirection, BlurModeCount, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var data = new DirectionUniformData
            {
                Resolution = new Vector2(viewport.Width, viewport.Height),
                Direction = dir,
            };

            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
            api.BindTextureBase(TextureTarget.Texture2D, 0, texture0.Object);

            vertices.Draw(api);

            ResourcePool.Instance.Remove(sampleTarget0);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CenterUniformData : IEquatable<CenterUniformData>
        {
            public Vector2 Resolution;
            public Vector2 CenterCoord;
            public float Range;

            public static bool operator ==(in CenterUniformData a, in CenterUniformData b) => a.Equals(b);
            public static bool operator !=(in CenterUniformData a, in CenterUniformData b) => !a.Equals(b);
            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Resolution.GetHashCode());
                hash.Combine(CenterCoord.GetHashCode());
                hash.Combine(Range.GetHashCode());
                return hash;
            }

            public bool Equals(CenterUniformData other)
            {
                return Resolution == other.Resolution && 
                    CenterCoord == other.CenterCoord &&
                    Range == other.Range;
            }

            public override bool Equals(object obj)
            {
                return obj is CenterUniformData other && Equals(other);
            }
        }

        public void DrawCenter(GraphicsApi api, Vector2[] quad, in Vector2 center, float range)
        {
           DrawCenter(api, VertexArrayDraw.Array(VertexArrays.Get2D(quad), PrimitiveMode.Triangles, 0, 4), QuadToBounds(api.CurrentTarget, quad), center, range);
        }

        public void DrawCenter(GraphicsApi api, in Vector2 center, float range)
        {
            DrawCenter(api, VertexArrayDraw.Array(VertexArrays.GetScreen2D(), PrimitiveMode.TriangleStrip, 0, 4), api.CurrentTarget.Viewport, center, range);
        }

        private void DrawCenter(GraphicsApi api, VertexArrayDraw vertices, in Bounds2 bounds, in Vector2 center, float range)
        {
            var target = api.CurrentTarget;

            target.SetDrawBuffers(api, 0);

            var viewport = target.Viewport;

            var sampleTargetDesc = new RenderTargetDescription()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Attachments = new RenderTargetAttachmentDescription[]
                {
                    new RenderTargetAttachmentDescription()
                    {
                        Attachment = FramebufferAttachment.ColorAttachment0,
                        Format = target.GetFormat(FramebufferAttachment.ColorAttachment0),
                        Texture = true
                    }
                }
            };

            var sampleTarget0 = RenderTargets.NewTemporary(sampleTargetDesc);

            var r = (int)Math.Ceiling(range);
            Bounds2 srcbounds = bounds;
            srcbounds.Inflate(r, r);
            srcbounds.Intersect(viewport);

            target.Blit(sampleTarget0, srcbounds, srcbounds.OffsetBounds(-viewport.Origin), ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            var texture0 = target.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

            api.ApplyPolygonMode(PolygonMode.Fill);
            api.ApplyBlendMode(BlendMode.None);
            api.ApplyCullMode(CullMode.None);
            api.ApplyDepthMode(DepthMode.None);
            api.ApplyScissor(Bounds2.Zero);

            _programs.AddLink(ProgramBranchMask.Vertex | ProgramBranchMask.Fragment);
            _programs.AddBranch("UsingBlurModeCube", false, ProgramBranchMask.Vertex);
            _programs.AddBranch("BlurMode", BlurModeCenter, BlurModeCount, ProgramBranchMask.Fragment);

            var program = _programs.EndBranch();

            program.Use(api);

            var data = new CenterUniformData
            {
                Resolution = new Vector2(viewport.Width, viewport.Height),
                CenterCoord = center * 0.5f + new Vector2(0.5f),
                Range = range
            };

            var dataBuffer = Buffers.FromData(data, BufferTarget.UniformBuffer);
            api.BindBufferBase(BufferTarget.UniformBuffer, 0, dataBuffer.Object);
            api.BindTextureBase(TextureTarget.Texture2D, 0, texture0.Object);

            vertices.Draw(api);

            ResourcePool.Instance.Remove(sampleTarget0);
        }
    }
}
