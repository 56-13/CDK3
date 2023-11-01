using System;

namespace CDK.Drawing
{
    [Flags]
    public enum PolygonMode
    {
        Fill = 0,
        Line = LineFront | LineBack,
        Point = PointFront | PointBack,
        LineFront = 1 << 2,
        PointFront = 2 << 2,
        LineBack = 1,
        PointBack = 2
    }
    
    public enum StrokeMode
    {
        Normal,
        Lighten
    }

    public enum CullMode
    {
        None,
        Back,
        Front
    }

    public enum DepthMode
    {
        None,
        Read,
        Write,
        ReadWrite
    }

    public enum StencilMode
    {
        None,
        Inclusive,
        Exclusive
    }

    public enum BlendMode
    {
        None,
        Alpha,
        Add,
        Substract,
        Multiply,
        Screen
    }

    public enum LightMode
    {
        None,
        Phong,
        Blinn,
        CookBlinn,
        CookBeckmann,
        CookGGX
    }

    [Flags]
    public enum Corner
    {
        LeftTop = 1,
        RightTop = 2,
        LeftBottom = 4,
        RightBottom = 8,
        All = 15
    }

    public enum InstanceLayer
    {
        None,
        Shadow,
        Shadow2D,
        Base,
        BlendBottom,
        BlendMiddle,
        BlendTop,
        Distortion,
        Cursor
    }

    public enum InstanceBlendLayer
    {
        Bottom = InstanceLayer.BlendBottom,
        Middle = InstanceLayer.BlendMiddle,
        Top = InstanceLayer.BlendTop
    }

    public enum PrimitiveMode
    {
        Points,
        Lines,
        LineLoop,
        LineStrip,
        Triangles,
        TriangleStrip,
        TriangleFan
    }

    public enum VertexAttribType
    {
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float,
        Double,
        HalfFloat
    }

    public enum BufferTarget
    {
        ArrayBuffer,
        ElementArrayBuffer,
        PixelPackBuffer,
        PixelUnpackBuffer,
        UniformBuffer,
        TextureBuffer,
        TransformFeedbackBuffer,
        CopyReadBuffer,
        CopyWriteBuffer,
        DrawIndirectBuffer,
        ShaderStorageBuffer,
        DispatchIndirectBuffer,
        QueryBuffer,
        AtomicCounterBuffer
    }

    public enum BufferUsageHint
    {
        StreamDraw,
        StreamRead,
        StreamCopy,
        StaticDraw,
        StaticRead,
        StaticCopy,
        DynamicDraw,
        DynamicRead,
        DynamicCopy
    }

    public enum DrawElementsType
    {
        UnsignedByte,
        UnsignedShort,
        UnsignedInt
    }

    public enum ShaderType
    {
        FragmentShader,
        VertexShader,
        GeometryShader,
        TessEvaluationShader,
        TessControlShader,
        ComputeShader
    }

    public enum FramebufferAttachment
    {
        ColorAttachment0,
        ColorAttachment1,
        ColorAttachment2,
        ColorAttachment3,
        ColorAttachment4,
        ColorAttachment5,
        ColorAttachment6,
        ColorAttachment7,
        DepthStencilAttachment,
        DepthAttachment,
        StencilAttachment
    }

    public enum FramebufferTarget
    {
        ReadFramebuffer,
        DrawFramebuffer,
        Framebuffer
    }

    [Flags]
    public enum ClearBufferMask
    {
        DepthBufferBit = 0x0100,
        AccumBufferBit = 0x0200,
        StencilBufferBit = 0x0400,
        ColorBufferBit = 0x4000
    }

    public enum BlitFramebufferFilter
    {
        Nearest,
        Linear
    }

    public enum TextureTarget
    {
        Texture2D,
        Texture3D,
        TextureCubeMap,
        TextureCubeMapPositiveX,
        TextureCubeMapNegativeX,
        TextureCubeMapPositiveY,
        TextureCubeMapNegativeY,
        TextureCubeMapPositiveZ,
        TextureCubeMapNegativeZ,
        Texture2DMultisample
    }

    public enum PixelFormat
    {
        None,
        Red,
        Green,
        Blue,
        Alpha,
        Rgb,
        Rgba,
        Luminance,
        LuminanceAlpha,
        Rg,
        RgInteger,
        DepthComponent,
        DepthStencil,
        RedInteger,
        GreenInteger,
        BlueInteger,
        AlphaInteger,
        RgbInteger,
        RgbaInteger
    }

    public enum PixelType
    {
        None,
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float,
        HalfFloat,
        UnsignedShort4444,
        UnsignedShort5551,
        UnsignedInt8888,
        UnsignedShort565,
        UnsignedInt248,
        Float32UnsignedInt248Rev
    }

    public enum TextureWrapMode
    {
        Repeat,
        ClampToBorder,
        ClampToEdge,
        MirroredRepeat
    }

    public enum TextureMinFilter
    {
        Nearest,
        Linear,
        NearestMipmapNearest,
        LinearMipmapNearest,
        NearestMipmapLinear,
        LinearMipmapLinear,
    }

    public enum TextureMagFilter
    {
        Nearest,
        Linear
    }

    public enum TextureAccess
    {
        ReadOnly,
        WriteOnly,
        ReadWrite
    }
}
