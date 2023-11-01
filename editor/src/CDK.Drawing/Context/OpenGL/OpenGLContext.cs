using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CDK.Drawing.OpenGL
{
    internal class OpenGLContext : GraphicsContext
    {

        private static readonly string[] GLInternalFormatSupportExtensions =
        {
            null,   //None,
            null,   //Alpha8,
            null,   //Luminance8,
            null,   //Luminance8Alpha8,
            null,   //R8,
            null,   //R8i,
            null,   //R8ui,
            null,   //R8Snorm,
            null,   //R16,
            null,   //R16f,
            null,   //R16i,
            null,   //R16ui,
            null,   //R16Snorm,
            null,   //R32f,
            null,   //R32i,
            null,   //R32ui,
            null,   //Rg8,
            null,   //Rg8i,
            null,   //Rg8ui,
            null,   //Rg8Snorm,
            null,   //Rg16,
            null,   //Rg16f,
            null,   //Rg16i,
            null,   //Rg16ui,
            null,   //Rg16Snorm,
            null,   //Rg32f,
            null,   //Rg32i,
            null,   //Rg32ui,
            null,   //Rgb5,
            null,   //Rgb8,
            null,   //Rgb8i,
            null,   //Rgb8ui,
            null,   //Rgb8Snorm,
            null,   //Srgb8,
            null,   //Rgb16,
            null,   //Rgb16f,
            null,   //Rgb16i,
            null,   //Rgb16ui,
            null,   //Rgb16Snorm,
            null,   //Rgb32i,
            null,   //Rgb32ui,
            null,   //Rgba4,
            null,   //Rgb5A1,
            null,   //Rgba8,
            null,   //Rgba8i,
            null,   //Rgba8ui,
            null,   //Rgba8Snorm,
            null,   //Srgb8Alpha8,
            null,   //Rgba16,
            null,   //Rgba16f,
            null,   //Rgba16i,
            null,   //Rgba16ui,
            null,   //Rgba32f,
            null,   //Rgba32i,
            null,   //Rgba32ui,
            null,   //DepthComponent16,
            null,   //DepthComponent24,
            null,   //Depth24Stencil8,
            null,   //DepthComponent32f,
            null,   //Depth32fStencil8,
            "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt1Ext,
            "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt3Ext,
            "GL_EXT_texture_compression_s3tc",      //CompressedRgbaS3tcDxt5Ext,
            "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt1Ext,
            "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt3Ext,
            "GL_EXT_texture_compression_s3tc",      //CompressedSrgbAlphaS3tcDxt5Ext,
            "GL_OES_compressed_ETC2_RGB8_texture",                      //CompressedRgb8Etc2,
            "GL_OES_compressed_ETC2_sRGB8_texture",                     //CompressedSrgb8Etc2,
            "GL_OES_compressed_ETC2_punchthroughA_RGBA8_texture",       //CompressedRgb8PunchthroughAlpha1Etc2,
            "GL_OES_compressed_ETC2_punchthroughA_sRGB8_alpha_texture", //CompressedSrgb8PunchthroughAlpha1Etc2,
            "GL_OES_compressed_ETC2_RGBA8_texture",                     //CompressedRgba8Etc2Eac,
            "GL_OES_compressed_ETC2_sRGB8_alpha8_texture",              //CompressedSrgb8Alpha8Etc2Eac,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc4x4,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc5x5,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc6x6,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc8x8,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc10x10,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedRgbaAstc12x12,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc4x4,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc5x5,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc6x6,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc8x8,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc10x10,
            "GL_KHR_texture_compression_astc_ldr",  //CompressedSrgbaAstc12x12
        };

        private IGraphicsContext _context;
        private GLControl _control;
        private Thread _thread;
        private OpenGLApi _api;
        private LinkedList<Action<GraphicsApi>> _invocations;
        private int _maxUniformBlockSize;
        private string _extensions;
        private bool _alive;

        public OpenGLContext()
        {
            _control = new GLControl();
            _control.CreateControl();
            _invocations = new LinkedList<Action<GraphicsApi>>();

            _alive = true;
            _thread = new Thread(ThreadRun);

            lock (_invocations)
            {
                _thread.Start();

                Monitor.Wait(_invocations);
            }
        }

        protected override void Dispose()
        {
            base.Dispose();

            _alive = false;
            lock (_invocations)
            {
                Monitor.Pulse(_invocations);
            }
            _thread.Join();

            _control.Dispose();
        }

        private void ThreadRun()
        {
            var mode = GraphicsMode.Default;

            _context = new OpenTK.Graphics.GraphicsContext(mode, _control.WindowInfo);
            _context.MakeCurrent(_control.WindowInfo);

            _api = new OpenGLApi();

            GL.Enable(EnableCap.TextureCubeMapSeamless);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.GetInteger(GetPName.MaxUniformBlockSize, out _maxUniformBlockSize);
            _extensions = GL.GetString(StringName.Extensions);

            lock (_invocations)
            {
                Monitor.Pulse(_invocations);
            }

            for (; ; )
            {
                Action<GraphicsApi> inv = null;
                lock (_invocations)
                {
                    if (_invocations.Count != 0)
                    {
                        inv = _invocations.First();
                        _invocations.RemoveFirst();
                    }
                    else if (_alive) Monitor.Wait(_invocations);
                    else break;
                }

                inv?.Invoke(_api);
            }
            _context.Dispose();
        }

        internal override void ClearTargets(RenderTarget target)
        {
            if (_api.CurrentTarget == target) _api.CurrentTarget = null;
        }

        public override GraphicsPlatform Platform => GraphicsPlatform.OpenGL;
        public override bool IsSupportParallel => false;
        public override int MaxUniformBlockSize => _maxUniformBlockSize;
        public override bool IsSupportRawFormat(RawFormat format)
        {
            var ext = GLInternalFormatSupportExtensions[(int)format];

            return ext == null || _extensions.Contains(ext);
        }

        public override bool IsRenderThread(out GraphicsApi api)
        {
            if (Thread.CurrentThread == _thread)
            {
                api = _api;
                return true;
            }
            api = null;
            return false;
        }
        public override GraphicsApi AttachRenderThread()
        {
            throw new InvalidOperationException();
        }

        public override void DetachRenderThread()
        {
            throw new InvalidOperationException();
        }

        public override DelegateCommand Invoke(bool gsync, Action<GraphicsApi> inv)
        {
            if (IsRenderThread(out var api))
            {
                inv(api);
                return null;
            }
            if (gsync)
            {
                var graphics = CurrentGraphics;
                if (graphics != null) return graphics.Command(inv);
            }
            lock (_invocations)
            {
                _invocations.AddLast(inv);
                Monitor.Pulse(_invocations);
            }
            return null;
        }

        public override Control CreateControl() => new GLControl(_context.GraphicsMode);

        public override RenderTarget CreateControlRenderTarget(Control control)
        {
            var width = control.Width;
            var height = control.Height;

            var mode = ((GLControl)control).GraphicsMode;

            SystemRenderTargetDescription desc = new SystemRenderTargetDescription
            {
                RedBit = mode.ColorFormat.Red,
                GreenBit = mode.ColorFormat.Green,
                BlueBit = mode.ColorFormat.Blue,
                AlphaBit = mode.ColorFormat.Alpha,
                DepthBit =mode.Depth,
                StencilBit = mode.Stencil,
                Samples = mode.Samples
            };

            return new RenderTarget(width, height, 0, desc);
        }

        public override void MakeCurrent(Control control)
        {
            Invoke(false, (GraphicsApi api) =>
            {
                for (; ; )
                {
                    try
                    {
                        _context.MakeCurrent(((GLControl)control).WindowInfo);
                        return;
                    }
                    catch (GraphicsContextException e)
                    {
                        Console.WriteLine(e);

                        Thread.Sleep(1000);     //TODO:슬립모드에서 깨어날 시 MakeCurrent에 문제가 있다. 임시로 처치
                    }
                }
            });
        }

        public override void ResetCurrent() => MakeCurrent(_control);
        public override void SwapBuffers()
        {
            Invoke(false, (GraphicsApi api) => _context.SwapBuffers());
        }
    }
}
