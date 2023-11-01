using GLPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using GLVertexAttribType = OpenTK.Graphics.OpenGL.VertexAttribType;
using GLBufferTarget = OpenTK.Graphics.OpenGL.BufferTarget;
using GLBufferUsageHint = OpenTK.Graphics.OpenGL.BufferUsageHint;
using GLDrawElementsType = OpenTK.Graphics.OpenGL.DrawElementsType;
using GLShaderType = OpenTK.Graphics.OpenGL.ShaderType;
using GLInternalFormat = OpenTK.Graphics.OpenGL.InternalFormat;
using GLFramebufferAttachment = OpenTK.Graphics.OpenGL.FramebufferAttachment;
using GLFramebufferTarget = OpenTK.Graphics.OpenGL.FramebufferTarget;
using GLBlitFramebufferFilter = OpenTK.Graphics.OpenGL.BlitFramebufferFilter;
using GLTextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
using GLPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using GLPixelType = OpenTK.Graphics.OpenGL.PixelType;
using GLTextureParameterName = OpenTK.Graphics.OpenGL.TextureParameterName;
using GLTextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using GLTextureMinFilter = OpenTK.Graphics.OpenGL.TextureMinFilter;
using GLTextureMagFilter = OpenTK.Graphics.OpenGL.TextureMagFilter;
using GLTextureAccess = OpenTK.Graphics.OpenGL.TextureAccess;

namespace CDK.Drawing.OpenGL
{
    internal partial class OpenGLApi
    {
        private static readonly GLPrimitiveType[] GLPrimitiveTypes =
        {
            GLPrimitiveType.Points,
            GLPrimitiveType.Lines,
            GLPrimitiveType.LineLoop,
            GLPrimitiveType.LineStrip,
            GLPrimitiveType.Triangles,
            GLPrimitiveType.TriangleStrip,
            GLPrimitiveType.TriangleFan
        };

        private static readonly GLVertexAttribType[] GLVertexAttribTypes =
        {
            GLVertexAttribType.Byte,
            GLVertexAttribType.UnsignedByte,
            GLVertexAttribType.Short,
            GLVertexAttribType.UnsignedShort,
            GLVertexAttribType.Int,
            GLVertexAttribType.UnsignedInt,
            GLVertexAttribType.Float,
            GLVertexAttribType.Double,
            GLVertexAttribType.HalfFloat
        };

        private static readonly GLBufferTarget[] GLBufferTargets =
        {
            GLBufferTarget.ArrayBuffer,
            GLBufferTarget.ElementArrayBuffer,
            GLBufferTarget.PixelPackBuffer,
            GLBufferTarget.PixelUnpackBuffer,
            GLBufferTarget.UniformBuffer,
            GLBufferTarget.TextureBuffer,
            GLBufferTarget.TransformFeedbackBuffer,
            GLBufferTarget.CopyReadBuffer,
            GLBufferTarget.CopyWriteBuffer,
            GLBufferTarget.DrawIndirectBuffer,
            GLBufferTarget.ShaderStorageBuffer,
            GLBufferTarget.DispatchIndirectBuffer,
            GLBufferTarget.QueryBuffer,
            GLBufferTarget.AtomicCounterBuffer
        };

        private static readonly GLBufferUsageHint[] GLBufferUsageHints =
        {
            GLBufferUsageHint.StreamDraw,
            GLBufferUsageHint.StreamRead,
            GLBufferUsageHint.StreamCopy,
            GLBufferUsageHint.StaticDraw,
            GLBufferUsageHint.StaticRead,
            GLBufferUsageHint.StaticCopy,
            GLBufferUsageHint.DynamicDraw,
            GLBufferUsageHint.DynamicRead,
            GLBufferUsageHint.DynamicCopy
        };

        private static readonly GLDrawElementsType[] GLDrawElementsTypes =
        {
            GLDrawElementsType.UnsignedByte,
            GLDrawElementsType.UnsignedShort,
            GLDrawElementsType.UnsignedInt
        };

        private static readonly GLShaderType[] GLShaderTypes =
        {
            GLShaderType.FragmentShader,
            GLShaderType.VertexShader,
            GLShaderType.GeometryShader,
            GLShaderType.TessEvaluationShader,
            GLShaderType.TessControlShader,
            GLShaderType.ComputeShader
        };

        private static readonly GLInternalFormat[] GLInternalFormats =
        {
            0,
            GLInternalFormat.Alpha8,
            GLInternalFormat.Luminance8,
            GLInternalFormat.Luminance8Alpha8,
            GLInternalFormat.R8,
            GLInternalFormat.R8i,
            GLInternalFormat.R8ui,
            GLInternalFormat.R8Snorm,
            GLInternalFormat.R16,
            GLInternalFormat.R16f,
            GLInternalFormat.R16i,
            GLInternalFormat.R16ui,
            GLInternalFormat.R16Snorm,
            GLInternalFormat.R32f,
            GLInternalFormat.R32i,
            GLInternalFormat.R32ui,
            GLInternalFormat.Rg8,
            GLInternalFormat.Rg8i,
            GLInternalFormat.Rg8ui,
            GLInternalFormat.Rg8Snorm,
            GLInternalFormat.Rg16,
            GLInternalFormat.Rg16f,
            GLInternalFormat.Rg16i,
            GLInternalFormat.Rg16ui,
            GLInternalFormat.Rg16Snorm,
            GLInternalFormat.Rg32f,
            GLInternalFormat.Rg32i,
            GLInternalFormat.Rg32ui,
            GLInternalFormat.Rgb5,
            GLInternalFormat.Rgb8,
            GLInternalFormat.Rgb8i,
            GLInternalFormat.Rgb8ui,
            GLInternalFormat.Rgb8Snorm,
            GLInternalFormat.Srgb8,
            GLInternalFormat.Rgb16,
            GLInternalFormat.Rgb16f,
            GLInternalFormat.Rgb16i,
            GLInternalFormat.Rgb16ui,
            GLInternalFormat.Rgb16Snorm,
            GLInternalFormat.Rgb32i,
            GLInternalFormat.Rgb32ui,
            GLInternalFormat.Rgba4,
            GLInternalFormat.Rgb5A1,
            GLInternalFormat.Rgba8,
            GLInternalFormat.Rgba8i,
            GLInternalFormat.Rgba8ui,
            GLInternalFormat.Rgba8Snorm,
            GLInternalFormat.Srgb8Alpha8,
            GLInternalFormat.Rgba16,
            GLInternalFormat.Rgba16f,
            GLInternalFormat.Rgba16i,
            GLInternalFormat.Rgba16ui,
            GLInternalFormat.Rgba32f,
            GLInternalFormat.Rgba32i,
            GLInternalFormat.Rgba32ui,
            GLInternalFormat.DepthComponent16,
            GLInternalFormat.DepthComponent24Oes,
            GLInternalFormat.Depth24Stencil8,
            GLInternalFormat.DepthComponent32f,
            GLInternalFormat.Depth32fStencil8,
            GLInternalFormat.CompressedRgbaS3tcDxt1Ext,
            GLInternalFormat.CompressedRgbaS3tcDxt3Ext,
            GLInternalFormat.CompressedRgbaS3tcDxt5Ext,
            GLInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext,
            GLInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext,
            GLInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext,
            GLInternalFormat.CompressedRgb8Etc2,
            GLInternalFormat.CompressedSrgb8Etc2,
            GLInternalFormat.CompressedRgb8PunchthroughAlpha1Etc2,
            GLInternalFormat.CompressedSrgb8PunchthroughAlpha1Etc2,
            GLInternalFormat.CompressedRgba8Etc2Eac,
            GLInternalFormat.CompressedSrgb8Alpha8Etc2Eac,
            (GLInternalFormat)0x93B0,
            (GLInternalFormat)0x93B2,
            (GLInternalFormat)0x93B4,
            (GLInternalFormat)0x93B7,
            (GLInternalFormat)0x93BB,
            (GLInternalFormat)0x93BD,
            (GLInternalFormat)0x93D0,
            (GLInternalFormat)0x93D2,
            (GLInternalFormat)0x93D4,
            (GLInternalFormat)0x93D7,
            (GLInternalFormat)0x93DB,
            (GLInternalFormat)0x93DD
        };


        private static readonly GLFramebufferAttachment[] GLFramebufferAttachments =
        {
            GLFramebufferAttachment.ColorAttachment0,
            GLFramebufferAttachment.ColorAttachment1,
            GLFramebufferAttachment.ColorAttachment2,
            GLFramebufferAttachment.ColorAttachment3,
            GLFramebufferAttachment.ColorAttachment4,
            GLFramebufferAttachment.ColorAttachment5,
            GLFramebufferAttachment.ColorAttachment6,
            GLFramebufferAttachment.ColorAttachment7,
            GLFramebufferAttachment.DepthStencilAttachment,
            GLFramebufferAttachment.DepthAttachment,
            GLFramebufferAttachment.StencilAttachment
        };

        private static readonly GLFramebufferTarget[] GLFramebufferTargets =
        {
            GLFramebufferTarget.ReadFramebuffer,
            GLFramebufferTarget.DrawFramebuffer,
            GLFramebufferTarget.Framebuffer
        };

        private static readonly GLBlitFramebufferFilter[] GLBlitFramebufferFilters =
        {
            GLBlitFramebufferFilter.Nearest,
            GLBlitFramebufferFilter.Linear
        };

        private static readonly GLTextureTarget[] GLTextureTargets =
        {
            GLTextureTarget.Texture2D,
            GLTextureTarget.Texture3D,
            GLTextureTarget.TextureCubeMap,
            GLTextureTarget.TextureCubeMapPositiveX,
            GLTextureTarget.TextureCubeMapNegativeX,
            GLTextureTarget.TextureCubeMapPositiveY,
            GLTextureTarget.TextureCubeMapNegativeY,
            GLTextureTarget.TextureCubeMapPositiveZ,
            GLTextureTarget.TextureCubeMapNegativeZ,
            GLTextureTarget.Texture2DMultisample
        };

        private static readonly GLPixelFormat[] GLPixelFormats =
        {
            0,
            GLPixelFormat.Red,
            GLPixelFormat.Green,
            GLPixelFormat.Blue,
            GLPixelFormat.Alpha,
            GLPixelFormat.Rgb,
            GLPixelFormat.Rgba,
            GLPixelFormat.Luminance,
            GLPixelFormat.LuminanceAlpha,
            GLPixelFormat.Rg,
            GLPixelFormat.RgInteger,
            GLPixelFormat.DepthComponent,
            GLPixelFormat.DepthStencil,
            GLPixelFormat.RedInteger,
            GLPixelFormat.GreenInteger,
            GLPixelFormat.BlueInteger,
            GLPixelFormat.AlphaInteger,
            GLPixelFormat.RgbInteger,
            GLPixelFormat.RgbaInteger
        };

        private static readonly GLPixelType[] GLPixelTypes =
        {
            0,
            GLPixelType.Byte,
            GLPixelType.UnsignedByte,
            GLPixelType.Short,
            GLPixelType.UnsignedShort,
            GLPixelType.Int,
            GLPixelType.UnsignedInt,
            GLPixelType.Float,
            GLPixelType.HalfFloat,
            GLPixelType.UnsignedShort4444,
            GLPixelType.UnsignedShort5551,
            GLPixelType.UnsignedInt8888,
            GLPixelType.UnsignedShort565,
            GLPixelType.Float32UnsignedInt248Rev
        };

        private static readonly GLTextureWrapMode[] GLTextureWrapModes =
        {
            GLTextureWrapMode.Repeat,
            GLTextureWrapMode.ClampToBorder,
            GLTextureWrapMode.ClampToEdge,
            GLTextureWrapMode.MirroredRepeat
        };

        private static readonly GLTextureMinFilter[] GLTextureMinFilters =
        {
            GLTextureMinFilter.Nearest,
            GLTextureMinFilter.Linear,
            GLTextureMinFilter.NearestMipmapNearest,
            GLTextureMinFilter.LinearMipmapNearest,
            GLTextureMinFilter.NearestMipmapLinear,
            GLTextureMinFilter.LinearMipmapLinear,
        };

        private static readonly GLTextureMagFilter[] GLTextureMagFilters =
        {
            GLTextureMagFilter.Nearest,
            GLTextureMagFilter.Linear
        };

        private static readonly GLTextureParameterName[] GLTextureParameterAxisNames =
        {
            GLTextureParameterName.TextureWrapS,
            GLTextureParameterName.TextureWrapT,
            GLTextureParameterName.TextureWrapR
        };

        private static readonly GLTextureAccess[] GLTextureAccesses =
        {
            GLTextureAccess.ReadOnly,
            GLTextureAccess.WriteOnly,
            GLTextureAccess.ReadWrite
        };
    }
}
