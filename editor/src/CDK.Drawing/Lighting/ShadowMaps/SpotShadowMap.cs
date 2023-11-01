using System;
using System.Numerics;

namespace CDK.Drawing
{
    internal class SpotShadowMap : IDisposable
    {
        private SpotLight _light;
        private float _range;
        private RenderTarget _renderTarget;
        public Matrix4x4 ViewProjection { private set; get; }
        public Texture Texture => _renderTarget?.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

        public void Dispose()
        {
            _renderTarget?.Dispose();
        }

        public void Begin(Graphics graphics, in SpotLight light, float range)
        {
            if (_renderTarget == null || _light.Shadow.Resolution != light.Shadow.Resolution || _light.Shadow.Pixel32 != light.Shadow.Pixel32)
            {
                _renderTarget?.Dispose();
                _renderTarget = CreateRenderTarget(light.Shadow.Resolution, light.Shadow.Pixel32);
            }

            var resetView = _light.Position != light.Position || _light.Direction != light.Direction || _light.Angle != light.Angle || _light.Dispersion != light.Dispersion || _range != range;

            _light = light;
            _range = range;

            if (resetView)
            {
                //far ~ near 비율이 너무 크면 정밀도가 매우 떨어짐
                var projection = MatrixUtil.CreatePerspectiveFovLH(light.Angle + light.Dispersion * 2, 1, Math.Max(range / 1000, 1), range);
                var target = light.Position + light.Direction;
                var up = !VectorUtil.NearEqual(light.Direction, Vector3.UnitX) ? Vector3.Normalize(Vector3.Cross(light.Direction, Vector3.UnitX)) : Vector3.UnitZ;
                ViewProjection = MatrixUtil.CreateLookAtLH(light.Position, target, up) * projection;
            }
            graphics.Push();
            graphics.Target = _renderTarget;
            graphics.Clear(Color4.White);

            var renderer = Renderers.Shadow;
            renderer.BeginSpot(graphics, ViewProjection, light.Position, range);
            graphics.Renderer = renderer;
        }

        public void End(Graphics graphics)
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

            graphics.Pop();
        }

        private static RenderTarget CreateRenderTarget(int resolution, bool pixel32)
        {
            var colorTexture = new Texture(new TextureDescription()
            {
                Width = resolution,
                Height = resolution,
                Format = pixel32 ? RawFormat.Rg32f : RawFormat.Rg16f,
                WrapS = TextureWrapMode.ClampToBorder,
                WrapT = TextureWrapMode.ClampToBorder,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear,
                BorderColor = Color4.White
            });
            var target = new RenderTarget(resolution, resolution);
            target.Attach(FramebufferAttachment.ColorAttachment0, colorTexture, 0, true);
            target.Attach(FramebufferAttachment.DepthAttachment, new RenderBuffer(resolution, resolution, RawFormat.DepthComponent16), true);
            return target;
        }
    }
}
