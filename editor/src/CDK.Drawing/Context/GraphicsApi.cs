using System;

namespace CDK.Drawing
{
    public abstract class GraphicsApi
    {
        public abstract void ApplyPolygonMode(PolygonMode mode);
        public abstract void ApplyCullMode(CullMode mode);
        public abstract void ApplyDepthMode(DepthMode mode);
        public abstract void ApplyStencilMode(StencilMode mode, int depth);
        public abstract void ApplyBlendMode(BlendMode mode);
        public abstract void ApplyScissor(in Bounds2 scissor);
        public abstract void ApplyLineWidth(float width);

        public abstract int GenBuffer();
        public abstract void DeleteBuffer(int obj);
        public abstract void BindBuffer(BufferTarget target, int obj);
        public abstract void BindBufferBase(BufferTarget target, int binding, int obj);
        public abstract bool BufferData(BufferTarget target, int size, IntPtr data, BufferUsageHint hint);
        public abstract void BufferSubData(BufferTarget target, int offset, int size, IntPtr data);
        public abstract int GenVertexArray();
        public abstract void DeleteVertexArray(int obj);
        public abstract void BindVertexArray(int obj);
        public abstract int GetVertexArrayBinding();
        public abstract void VertexAttribDivisor(int index, int divisor);
        public abstract void VertexAttribPointer(int index, int size, VertexAttribType type, bool normalized, int stride, int offset);
        public abstract void SetVertexAttribEnabled(int index, bool enabled);
        public abstract void ClearBufferColor(int layer, in Color4 color);
        public abstract void ClearBufferDepthStencil();
        public abstract void ClearBufferDepth();
        public abstract void Clear(in Color4 color, ClearBufferMask mask);
        public abstract void ClearStencil(in Rectangle bounds, int depth);
        public abstract void DrawArrays(PrimitiveMode mode, int first, int count);
        public abstract void DrawElements(PrimitiveMode mode, int count, DrawElementsType type, int indices);
        public abstract void DrawElementsInstanced(PrimitiveMode mode, int count, DrawElementsType type, int indices, int instanceCount);
        public abstract int GenRenderbuffer();
        public abstract void DeleteRenderbuffer(int obj);
        public abstract void BindRenderbuffer(int obj);
        public abstract bool RenderbufferStorage(RawFormat format, int samples, int width, int height);
        public abstract int GenFramebuffer();
        public abstract void DeleteFramebuffer(int obj);
        public abstract void BindFramebuffer(FramebufferTarget target, int obj);
        public abstract void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, int renderbuffer);
        public abstract void FramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, int texture, int layer);
        public abstract void ReadBuffer(int buf);
        public abstract void DrawBuffer(int buf);
        public abstract void DrawBuffers(params int[] bufs);
        public abstract void Viewport(in Bounds2 bounds);
        public abstract void BindTexture(TextureTarget target, int texture);
        public abstract void BindTextureBase(TextureTarget target, int binding, int texture);
        public abstract void BindImageTexture(int binding, int texture, int level, bool layered, int layer, TextureAccess access, RawFormat format);
        public abstract bool CopyTexImage2D(TextureTarget target, int level, RawFormat format, in Bounds2 bounds);
        public abstract void BlitFramebuffer(in Bounds2 srcbounds, in Bounds2 destbounds, ClearBufferMask mask, BlitFramebufferFilter filter);
        public abstract void ReadPixels(in Bounds2 bounds, PixelFormat format, PixelType type, IntPtr pixels);
        public abstract void TexParameterWrap(TextureTarget target, int axis, TextureWrapMode param);
        public abstract void TexParameterMinFilter(TextureTarget target, TextureMinFilter param);
        public abstract void TexParameterMagFilter(TextureTarget target, TextureMagFilter param);
        public abstract void TexParameterBorderColor(TextureTarget target, in Color4 param);
        public abstract int GenTexture();
        public abstract void DeleteTexture(int obj);
        public abstract bool TexImage2D(TextureTarget target, int level, RawFormat format, int width, int height, IntPtr data);
        public abstract bool TexImage3D(TextureTarget target, int level, RawFormat format, int width, int height, int depth, IntPtr data);
        public abstract bool TexImage2DMultisample(TextureTarget target, int samples, RawFormat format, int width, int height);
        public abstract void TexSubImage2D(TextureTarget target, int level, RawFormat format, in Bounds2 bounds, IntPtr data);
        public abstract void TexSubImage3D(TextureTarget target, int level, RawFormat format, in Bounds3 bounds, IntPtr data);
        public abstract bool CompressedTexImage2D(TextureTarget target, int level, RawFormat format, int width, int height, int size, IntPtr data);
        public abstract bool CompressedTexImage3D(TextureTarget target, int level, RawFormat format, int width, int height, int depth, int size, IntPtr data);
        public abstract bool GenerateMipmap(TextureTarget target, int maxLevel);
        public abstract int CreateProgram();
        public abstract void DeleteProgram(int program);
        public abstract void AttachShader(int program, int shader);
        public abstract void DetachShader(int program, int shader);
        public abstract void LinkProgram(int program);
        public abstract void UseProgram(int program);
        public abstract int CreateShader(ShaderType type, params string[] sources);
        public abstract void DeleteShader(int shader);

        private WeakReference<RenderTarget> _CurrentTarget;
        public RenderTarget CurrentTarget
        {
            internal set
            {
                if (value == null) _CurrentTarget = null;
                else if (_CurrentTarget != null) _CurrentTarget.SetTarget(value);
                else _CurrentTarget = new WeakReference<RenderTarget>(value);
            }
            get
            {
                if (_CurrentTarget != null)
                {
                    if (_CurrentTarget.TryGetTarget(out var target)) return target;
                    _CurrentTarget = null;
                }
                return null;
            }
        }
    }
}

