using System;
using System.Numerics;

namespace CDK.Drawing
{
    internal partial class DirectionalShadowMap : IDisposable
    {
        private DirectionalLight _light;
        private ABoundingBox _space;
        private Camera _camera;
        private RenderTarget _renderTarget;
        private RenderTarget _renderTarget2D;

        public bool Visible { private set; get; }
        public Matrix4x4 ViewProjection { private set; get; }
        public Matrix4x4 ViewProjection2D { private set; get; }
        public Texture Texture => _renderTarget?.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
        public Texture Texture2D => _renderTarget2D?.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);
        
        public void Dispose(bool shadow2D)
        {
            if (shadow2D)
            {
                _renderTarget2D?.Dispose();
                _renderTarget2D = null;
            }
            else
            {
                _renderTarget?.Dispose();
                _renderTarget = null;
            }
        }
        
        public void Dispose()
        {
            _renderTarget?.Dispose();
            _renderTarget2D?.Dispose();
        }

        public bool Begin(Graphics graphics, in DirectionalLight light, in ABoundingBox space, bool shadow2D)
        {
            RenderTarget target;

            if (shadow2D)
            {
                if (_renderTarget2D == null || _light.Shadow.Resolution != light.Shadow.Resolution)
                {
                    _renderTarget2D?.Dispose();
                    _renderTarget2D = CreateRenderTarget2D(light.Shadow.Resolution);
                }
                target = _renderTarget2D;
            }
            else
            {
                if (_renderTarget == null || _light.Shadow.Resolution != light.Shadow.Resolution || _light.Shadow.Pixel32 != light.Shadow.Pixel32)
                {
                    _renderTarget?.Dispose();
                    _renderTarget = CreateRenderTarget(light.Shadow.Resolution, light.Shadow.Pixel32);
                }
                target = _renderTarget;
            }

            var resetView = (_light.Direction != light.Direction || _camera != graphics.Camera || _space != space);

            _light = light;

            if (resetView)
            {
                _camera = graphics.Camera;
                _space = space;

                UpdateView();
            }

            if (!Visible) return false;

            graphics.Push();
            graphics.Target = target;
            graphics.RenderOrder = false;
            graphics.Clear(Color4.White);

            if (shadow2D)
            {
                var renderer = Renderers.Shadow2D;
                renderer.Begin(graphics, ViewProjection2D, _light.Direction);
                graphics.Renderer = renderer;
                graphics.Color = Color4.Black;
            }
            else
            {
                var renderer = Renderers.Shadow;
                renderer.BeginDirectional(graphics, ViewProjection);
                graphics.Renderer = renderer;
            }
            return true;
        }

        public void End(Graphics graphics, bool shadow2D)
        {
            if (shadow2D)
            {
                Renderers.Shadow2D.End(graphics);
            }
            else 
            {
                Renderers.Shadow.End(graphics);

                var target = _renderTarget;
                var blur = _light.Shadow.Blur;

                var command = graphics.Command((GraphicsApi api) => 
                {
                    target.Focus(api);
                    Shaders.Blur.Draw(api, blur);
                });
                command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
            }
            graphics.Pop();
        }

        private static RenderTarget CreateRenderTarget2D(int resolution)
        {
            var colorTexture = new Texture(new TextureDescription()
            {
                Width = resolution,
                Height = resolution,
                Format = RawFormat.R8,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear,
                WrapS = TextureWrapMode.ClampToBorder,
                WrapT = TextureWrapMode.ClampToBorder,
                BorderColor = Color4.White
            });
            var target = new RenderTarget(resolution, resolution);
            target.Attach(FramebufferAttachment.ColorAttachment0, colorTexture, 0, true);
            return target;
        }

        private static RenderTarget CreateRenderTarget(int resolution, bool pixel32)
        {
            var colorTexture = new Texture(new TextureDescription()
            {
                Width = resolution,
                Height = resolution,
                Format = pixel32 ? RawFormat.Rg32f : RawFormat.Rg16f,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear,
                WrapS = TextureWrapMode.ClampToBorder,
                WrapT = TextureWrapMode.ClampToBorder,
                BorderColor = Color4.White
            });
            var target = new RenderTarget(resolution, resolution);
            target.Attach(FramebufferAttachment.ColorAttachment0, colorTexture, 0, true);
            target.Attach(FramebufferAttachment.DepthAttachment, new RenderBuffer(resolution, resolution, RawFormat.DepthComponent16), true);
            return target;
        }
    }
}
