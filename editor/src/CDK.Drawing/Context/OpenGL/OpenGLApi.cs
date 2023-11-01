using System;
using System.Linq;

using OpenTK.Graphics.OpenGL;

using GLPolygonMode = OpenTK.Graphics.OpenGL.PolygonMode;
using GLTextureParameterName = OpenTK.Graphics.OpenGL.TextureParameterName;
using GLTextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using GLTextureMinFilter = OpenTK.Graphics.OpenGL.TextureMinFilter;
using GLTextureMagFilter = OpenTK.Graphics.OpenGL.TextureMagFilter;

namespace CDK.Drawing.OpenGL
{
    internal partial class OpenGLApi : GraphicsApi
    {
        private PolygonMode _polygonMode;
        private CullMode _cullMode;
        private DepthMode _depthMode;
        private StencilMode _stencilMode;
        private int _stencilDepth;
        private BlendMode _blendMode;
        private Bounds2 _scissor;
        private float _lineWidth;
        private int _vertexArrayBinding;

        private const string ShaderVersion = "#version 430\n";

        public OpenGLApi()
        {
            _lineWidth = 1;
        }

        private static bool CheckMemoryAlloc()
        {
            var err = GL.GetError();
            Debug.Assert(err == ErrorCode.NoError || err == ErrorCode.OutOfMemory, $"gl error:{err}");
            return err == ErrorCode.NoError;
        }

        private static void AssertError()
        {
#if DEBUG
            var err = GL.GetError();
            Debug.Assert(err == ErrorCode.NoError, $"gl error:{err}");
#endif
        }

        private static void AssertFramebufferStatus()
        {
#if DEBUG
            var status = GL.CheckFramebufferStatus(OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer);
            Debug.Assert(status == FramebufferErrorCode.FramebufferComplete, $"invalid frame buffer status:{status}");
#endif
        }

        private static GLPolygonMode GetGLPolygonMode(int v)
        {
            switch (v)
            {
                case 1:
                    return GLPolygonMode.Line;
                case 2:
                    return GLPolygonMode.Point;
            }
            return GLPolygonMode.Fill;
        }

        public override void ApplyPolygonMode(PolygonMode mode)
        {
            if (mode != _polygonMode)
            {
                var front = (int)mode >> 2;
                var back = (int)mode & 3;

                if (front == back)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, GetGLPolygonMode(front));
                }
                else
                {
                    GL.PolygonMode(MaterialFace.Front, GetGLPolygonMode(front));
                    GL.PolygonMode(MaterialFace.Back, GetGLPolygonMode(back));
                }
                _polygonMode = mode;
            }
        }

        public override void ApplyCullMode(CullMode mode)
        {
            if (_cullMode != mode)
            {
                switch (mode)
                {
                    case CullMode.None:
                        GL.Disable(EnableCap.CullFace);
                        break;
                    case CullMode.Back:
                        GL.Enable(EnableCap.CullFace);
                        GL.FrontFace(FrontFaceDirection.Cw);
                        break;
                    case CullMode.Front:
                        GL.Enable(EnableCap.CullFace);
                        GL.FrontFace(FrontFaceDirection.Ccw);
                        break;
                }
                _cullMode = mode;
            }
        }

        public override void ApplyDepthMode(DepthMode mode)
        {
            if (_depthMode != mode)
            {
                if (mode != DepthMode.None)
                {
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthFunc((((int)mode & (int)DepthMode.Read) != 0) ? DepthFunction.Lequal : DepthFunction.Always);
                    GL.DepthMask(((int)mode & (int)DepthMode.Write) != 0);
                }
                else
                {
                    GL.Disable(EnableCap.DepthTest);
                    GL.DepthMask(false);
                }
                _depthMode = mode;
            }
        }

        public override void ApplyStencilMode(StencilMode mode, int depth)
        {
            if (_stencilMode != mode || _stencilDepth != depth)
            {
                switch (mode)
                {
                    case StencilMode.None:
                        if (depth != 0)
                        {
                            GL.Enable(EnableCap.StencilTest);
                            GL.StencilFunc(StencilFunction.Equal, _stencilDepth, 0xff);
                        }
                        else
                        {
                            GL.Disable(EnableCap.StencilTest);
                        }
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                        GL.ColorMask(true, true, true, true);
                        break;
                    case StencilMode.Inclusive:
                        GL.Enable(EnableCap.StencilTest);
                        GL.StencilFunc(StencilFunction.Equal, depth - 1, 0xff);
                        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Incr);
                        GL.ColorMask(false, false, false, false);
                        break;
                    case StencilMode.Exclusive:
                        if (depth != 0)
                        {
                            GL.Enable(EnableCap.StencilTest);
                            GL.StencilFunc(StencilFunction.Equal, depth, 0xff);
                            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Decr);
                        }
                        else
                        {
                            GL.Disable(EnableCap.StencilTest);
                            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
                        }
                        GL.ColorMask(false, false, false, false);
                        break;
                }

                _stencilMode = mode;
                _stencilDepth = depth;
            }
        }

        public override void ApplyBlendMode(BlendMode mode)
        {
            if (_blendMode != mode)
            {
                switch (mode)
                {
                    case BlendMode.None:
                        GL.Disable(EnableCap.Blend);
                        break;
                    case BlendMode.Add:
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.One, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                        GL.BlendEquation(BlendEquationMode.FuncAdd);
                        break;
                    case BlendMode.Substract:
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.One, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                        GL.BlendEquationSeparate(BlendEquationMode.FuncReverseSubtract, BlendEquationMode.FuncAdd);
                        break;
                    case BlendMode.Multiply:
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFuncSeparate(BlendingFactorSrc.DstColor, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                        GL.BlendEquation(BlendEquationMode.FuncAdd);
                        break;
                    case BlendMode.Screen:
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcColor, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                        GL.BlendEquation(BlendEquationMode.FuncAdd);
                        break;
                    default:
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        GL.BlendEquation(BlendEquationMode.FuncAdd);
                        break;
                }
                _blendMode = mode;
            }
        }

        public override void ApplyScissor(in Bounds2 scissor)
        {
            if (_scissor != scissor)
            {
                if (scissor != Bounds2.Zero)
                {
                    GL.Enable(EnableCap.ScissorTest);
                    GL.Scissor(scissor.X, CurrentTarget.Height - scissor.Height - scissor.Y, scissor.Width, scissor.Height);      //opengl coordinate system (bottom zero)
                }
                else
                {
                    GL.Disable(EnableCap.ScissorTest);
                }
                
                _scissor = scissor;
            }
        }

        public override void ApplyLineWidth(float width)
        {
            if (_lineWidth != width)
            {
                GL.LineWidth(width);
                _lineWidth = width;
            }
        }

        public override int GenBuffer()
        {
            return GL.GenBuffer();
        }

        public override void DeleteBuffer(int obj)
        {
            GL.DeleteBuffer(obj);
            AssertError();
        }

        public override void BindBuffer(BufferTarget target, int obj)
        {
            GL.BindBuffer(GLBufferTargets[(int)target], obj);
            AssertError();
        }

        public override void BindBufferBase(BufferTarget target, int binding, int obj)
        {
            GL.BindBufferBase((BufferRangeTarget)GLBufferTargets[(int)target], binding, obj);
            AssertError();
        }

        public override bool BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint hint)
        {
            GL.BufferData(GLBufferTargets[(int)target], size, data, GLBufferUsageHints[(int)hint]);
            return CheckMemoryAlloc();
        }

        public override void BufferSubData(BufferTarget target, int offset, int size, IntPtr data)
        {
            GL.BufferSubData(GLBufferTargets[(int)target], (IntPtr)offset, size, data);
            AssertError();
        }

        public override int GenVertexArray()
        {
            return GL.GenVertexArray();
        }
        public override void DeleteVertexArray(int obj)
        {
            GL.DeleteVertexArray(obj);
            AssertError();
        }

        public override void BindVertexArray(int obj)
        {
            GL.BindVertexArray(obj);
            _vertexArrayBinding = obj;
            AssertError();
        }

        public override int GetVertexArrayBinding()
        {
            return _vertexArrayBinding;
        }

        public override void VertexAttribDivisor(int index, int divisor)
        {
            GL.VertexAttribDivisor(index, divisor);
            AssertError();
        }

        public override void VertexAttribPointer(int index, int size, VertexAttribType type, bool normalized, int stride, int pointer)
        {
            switch (type)
            {
                case VertexAttribType.Float:
                case VertexAttribType.Double:
                case VertexAttribType.HalfFloat:
                    GL.VertexAttribPointer(index, size, (VertexAttribPointerType)GLVertexAttribTypes[(int)type], normalized, stride, pointer);
                    break;
                default:
                    GL.VertexAttribIPointer(index, size, (VertexAttribIntegerType)GLVertexAttribTypes[(int)type], stride, (IntPtr)pointer);
                    break;
            }
            AssertError();
        }

        public override void SetVertexAttribEnabled(int index, bool enabled)
        {
            if (enabled) GL.EnableVertexAttribArray(index);
            else GL.DisableVertexAttribArray(index);
            AssertError();
        }

        public override void ClearBufferColor(int layer, in Color4 color)
        {
            GL.ClearBuffer(ClearBuffer.Color, layer, color.ToArray());
            AssertError();
        }

        public override void ClearBufferDepthStencil()
        {
            GL.ClearBuffer(ClearBufferCombined.DepthStencil, 0, 1, 0);
            AssertError();
        }

        public override void ClearBufferDepth()
        {
            var depth = 1f;
            GL.ClearBuffer(ClearBuffer.Depth, 0, ref depth);
            AssertError();
        }

        public override void Clear(in Color4 color, ClearBufferMask mask)
        {
            var colorFlag = (mask & ClearBufferMask.ColorBufferBit) != 0;
            var depthFlag = (mask & ClearBufferMask.DepthBufferBit) != 0;
            var stencilFlag = (mask & ClearBufferMask.StencilBufferBit) != 0;

            if (colorFlag)
            {
                GL.ColorMask(true, true, true, true);
                GL.ClearColor(color.R, color.G, color.B, color.A);
            }
            if (depthFlag) { 
                GL.DepthMask(true);
                GL.ClearDepth(1);
            }
            if (stencilFlag)
            {
                GL.ClearStencil(0);
            }
            GL.Clear((OpenTK.Graphics.OpenGL.ClearBufferMask)mask);
            if (depthFlag && ((int)_depthMode & (int)DepthMode.Write) == 0) GL.DepthMask(false);
            if (colorFlag && _stencilMode != StencilMode.None) GL.ColorMask(false, false, false, false);
        }

        public override void ClearStencil(in Rectangle bounds, int depth)
        {
            var clearByShader = depth > 1;

            if (!clearByShader)
            {
                var size = (bounds.Right - bounds.Left) * (bounds.Bottom - bounds.Top);

                const float clearByShaderMaxSize = 4 * 0.36f;        //벤치마크에서 전체화면의 36% 정도까지 쉐이더가 더 빠름 (ipad 4)

                if (size <= clearByShaderMaxSize)
                {
                    clearByShader = true;
                }
            }
            if (clearByShader)
            {
                ApplyStencilMode(StencilMode.Exclusive, depth);

                Shaders.Empty.Draw(this, VertexArrayDraw.Array(VertexArrays.Get2D(bounds), PrimitiveMode.TriangleStrip, 0, 4));
            }
            else
            {
                GL.ClearStencil(0);
                GL.Clear(OpenTK.Graphics.OpenGL.ClearBufferMask.StencilBufferBit);
            }
            AssertError();
        }

        public override void DrawArrays(PrimitiveMode mode, int first, int count) 
        {
            GL.DrawArrays(GLPrimitiveTypes[(int)mode], first, count);
            AssertError();
        }

        public override void DrawElements(PrimitiveMode mode, int count, DrawElementsType type, int indices)
        {
            GL.DrawElements(GLPrimitiveTypes[(int)mode], count, GLDrawElementsTypes[(int)type], indices);
            AssertError();
        }

        public override void DrawElementsInstanced(PrimitiveMode mode, int count, DrawElementsType type, int indices, int instanceCount)
        {
            GL.DrawElementsInstanced(GLPrimitiveTypes[(int)mode], count, GLDrawElementsTypes[(int)type], (IntPtr)indices, instanceCount);
            AssertError();
        }

        public override int GenRenderbuffer()
        {
            return GL.GenRenderbuffer();
        }

        public override void DeleteRenderbuffer(int obj)
        {
            GL.DeleteRenderbuffer(obj);
            AssertError();
        }

        public override void BindRenderbuffer(int obj)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, obj);
            AssertError();
        }

        public override bool RenderbufferStorage(RawFormat format, int samples, int width, int height)
        {
            var internalFormat = (RenderbufferStorage)GLInternalFormats[(int)format];
            if (samples <= 1) GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, internalFormat, width, height);
            else GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, internalFormat, width, height);
            return CheckMemoryAlloc();
        }

        public override int GenFramebuffer()
        {
            return GL.GenFramebuffer();
        }

        public override void DeleteFramebuffer(int obj)
        {
            GL.DeleteFramebuffer(obj);
            AssertError();
        }

        public override void BindFramebuffer(FramebufferTarget target, int obj)
        {
            GL.BindFramebuffer(GLFramebufferTargets[(int)target], obj);
            AssertError();
        }

        public override void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, int renderbuffer)
        {
            GL.FramebufferRenderbuffer(GLFramebufferTargets[(int)target], GLFramebufferAttachments[(int)attachment], RenderbufferTarget.Renderbuffer, renderbuffer);
            AssertError();
        }

        public override void FramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, int texture, int layer)
        {
            GL.FramebufferTexture(GLFramebufferTargets[(int)target], GLFramebufferAttachments[(int)attachment], texture, layer);
            AssertError();
        }

        public override void ReadBuffer(int buf)
        {
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + buf);
            AssertError();
        }

        public override void DrawBuffer(int buf)
        {
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0 + buf);
            AssertError();
        }

        public override void DrawBuffers(params int[] bufs)
        {
            GL.DrawBuffers(bufs.Length, bufs.Select(b => DrawBuffersEnum.ColorAttachment0 + b).ToArray());
            AssertError();
        }
        
        public override void Viewport(in Bounds2 viewport)
        {
            GL.Viewport(viewport.X, CurrentTarget.Height - viewport.Height - viewport.Y, viewport.Width, viewport.Height);      //opengl coordinate system (bottom zero)
            AssertError();
        }

        public override void BindTexture(TextureTarget target, int texture)
        {
            GL.BindTexture(GLTextureTargets[(int)target], texture);
            AssertError();
        }

        public override void BindTextureBase(TextureTarget target, int binding, int texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + binding);
            GL.BindTexture(GLTextureTargets[(int)target], texture);
            AssertError();
        }

        public override void BindImageTexture(int binding, int texture, int level, bool layered, int layer, TextureAccess access, RawFormat format)
        {
            GL.BindImageTexture(binding, texture, level, layered, layer, GLTextureAccesses[(int)access], (SizedInternalFormat)GLInternalFormats[(int)format]);
        }

        public override bool CopyTexImage2D(TextureTarget target, int level, RawFormat format, in Bounds2 bounds)
        {
            GL.CopyTexImage2D(GLTextureTargets[(int)target], level, GLInternalFormats[(int)format], bounds.X, bounds.Y, bounds.Width, bounds.Height, 0);
            return CheckMemoryAlloc();
        }

        public override void BlitFramebuffer(in Bounds2 srcbounds, in Bounds2 destbounds, ClearBufferMask mask, BlitFramebufferFilter filter)
        {
            GL.BlitFramebuffer(srcbounds.Left, srcbounds.Top, srcbounds.Right, srcbounds.Bottom, destbounds.Left, destbounds.Top, destbounds.Right, destbounds.Bottom, (OpenTK.Graphics.OpenGL.ClearBufferMask)mask, GLBlitFramebufferFilters[(int)filter]);
            AssertError();
        }

        public override void ReadPixels(in Bounds2 bounds, PixelFormat format, PixelType type, IntPtr pixels)
        {
            GL.ReadPixels(bounds.X, bounds.Y, bounds.Width, bounds.Height, GLPixelFormats[(int)format], GLPixelTypes[(int)type], pixels);
            AssertError();
        }

        public override void TexParameterWrap(TextureTarget target, int axis, TextureWrapMode param)
        {
            GL.TexParameter(GLTextureTargets[(int)target], GLTextureParameterAxisNames[axis], (int)GLTextureWrapModes[(int)param]);
            AssertError();
        }

        public override void TexParameterMinFilter(TextureTarget target, TextureMinFilter param)
        {
            GL.TexParameter(GLTextureTargets[(int)target], GLTextureParameterName.TextureMinFilter, (int)GLTextureMinFilters[(int)param]);
            AssertError();
        }

        public override void TexParameterMagFilter(TextureTarget target, TextureMagFilter param)
        {
            GL.TexParameter(GLTextureTargets[(int)target], GLTextureParameterName.TextureMagFilter, (int)GLTextureMagFilters[(int)param]);
            AssertError();
        }

        public override void TexParameterBorderColor(TextureTarget target, in Color4 param)
        {
            GL.TexParameter(GLTextureTargets[(int)target], GLTextureParameterName.TextureBorderColor, param.ToArray());
            AssertError();
        }

        public override int GenTexture()
        {
            return GL.GenTexture();
        }

        public override void DeleteTexture(int obj)
        {
            GL.DeleteTexture(obj);
            AssertError();
        }

        public override bool TexImage2D(TextureTarget target, int level, RawFormat format, int width, int height, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(!enc.Compressed);
            GL.TexImage2D(GLTextureTargets[(int)target], level, (PixelInternalFormat)GLInternalFormats[(int)format], width, height, 0, GLPixelFormats[(int)enc.PixelFormat], GLPixelTypes[(int)enc.PixelType], data);
            return CheckMemoryAlloc();
        }

        public override bool TexImage3D(TextureTarget target, int level, RawFormat format, int width, int height, int depth, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(!enc.Compressed);
            GL.TexImage3D(GLTextureTargets[(int)target], level, (PixelInternalFormat)GLInternalFormats[(int)format], width, height, depth, 0, GLPixelFormats[(int)enc.PixelFormat], GLPixelTypes[(int)enc.PixelType], data);
            return CheckMemoryAlloc();
        }

        public override bool TexImage2DMultisample(TextureTarget target, int samples, RawFormat format, int width, int height)
        {
            var enc = format.GetEncoding();
            Debug.Assert(!enc.Compressed);
            GL.TexImage2DMultisample((TextureTargetMultisample)GLTextureTargets[(int)target], samples, (PixelInternalFormat)GLInternalFormats[(int)format], width, height, true);
            return CheckMemoryAlloc();
        }

        public override void TexSubImage2D(TextureTarget target, int level, RawFormat format, in Bounds2 bounds, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(!enc.Compressed);
            GL.TexSubImage2D(GLTextureTargets[(int)target], level, bounds.X, bounds.Y, bounds.Width, bounds.Height, GLPixelFormats[(int)enc.PixelFormat], GLPixelTypes[(int)enc.PixelType], data);
            AssertError();
        }

        public override void TexSubImage3D(TextureTarget target, int level, RawFormat format, in Bounds3 bounds, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(!enc.Compressed);
            GL.TexSubImage3D(GLTextureTargets[(int)target], level, bounds.X, bounds.Y, bounds.Z, bounds.Width, bounds.Height, bounds.Depth, GLPixelFormats[(int)enc.PixelFormat], GLPixelTypes[(int)enc.PixelType], data);
            AssertError();
        }

        public override bool CompressedTexImage2D(TextureTarget target, int level, RawFormat format, int width, int height, int size, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(enc.Compressed);
            GL.CompressedTexImage2D(GLTextureTargets[(int)target], level, GLInternalFormats[(int)format], width, height, 0, size, data);
            return CheckMemoryAlloc();
        }

        public override bool CompressedTexImage3D(TextureTarget target, int level, RawFormat format, int width, int height, int depth, int size, IntPtr data)
        {
            var enc = format.GetEncoding();
            Debug.Assert(enc.Compressed);
            GL.CompressedTexImage3D(GLTextureTargets[(int)target], level, GLInternalFormats[(int)format], width, height, depth, 0, size, data);
            return CheckMemoryAlloc();
        }

        public override bool GenerateMipmap(TextureTarget target, int maxLevel)
        {
            var gltarget = GLTextureTargets[(int)target];
            GL.TexParameter(gltarget, GLTextureParameterName.TextureMaxLevel, maxLevel);
            GL.GenerateMipmap((GenerateMipmapTarget)gltarget);
            return CheckMemoryAlloc();
        }

        public override int CreateProgram()
        {
            return GL.CreateProgram();
        }

        public override void DeleteProgram(int program)
        {
            GL.DeleteProgram(program);
            AssertError();
        }

        public override void AttachShader(int program, int shader)
        {
            GL.AttachShader(program, shader);
            AssertError();
        }

        public override void DetachShader(int program, int shader)
        {
            GL.DetachShader(program, shader);
            AssertError();
        }

        public override void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            AssertError();

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var status);

            if (status == 0)
            {
                var log = GL.GetProgramInfoLog(program);
                throw new InvalidOperationException(log);
            }
        }

        public override void UseProgram(int program)
        {
            GL.UseProgram(program);
            AssertError();
        }

        public override int CreateShader(ShaderType type, params string[] sources)
        {
            sources = sources.Prepend(ShaderVersion).ToArray();

            var shader = GL.CreateShader(GLShaderTypes[(int)type]);
            GL.ShaderSource(shader, sources.Length, sources, sources.Select(s => s.Length).ToArray());
            GL.CompileShader(shader);
            AssertError();

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);
            if (status == 0)
            {
                var log = GL.GetShaderInfoLog(shader);
                GL.DeleteShader(shader);
                throw new InvalidOperationException(log);
            }
            return shader;
        }

        public override void DeleteShader(int shader)
        {
            GL.DeleteShader(shader);
            AssertError();
        }
    }
}
