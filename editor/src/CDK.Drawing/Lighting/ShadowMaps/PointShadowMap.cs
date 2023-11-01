using System;
using System.Numerics;

namespace CDK.Drawing
{
    internal class PointShadowMap : IDisposable
    {
        private PointLight _light;
        private float _range;
        private RenderTarget _renderTarget;
        private Matrix4x4[] _viewProjections;
        public Texture Texture => _renderTarget?.GetTextureAttachment(FramebufferAttachment.ColorAttachment0);

        public PointShadowMap()
        {
            _viewProjections = new Matrix4x4[6];
        }

        public void Dispose()
        {
            _renderTarget?.Dispose();
        }

        public void Begin(Graphics graphics, in PointLight light, float range)
        {
            if (_renderTarget == null || _light.Shadow.Resolution != light.Shadow.Resolution || _light.Shadow.Pixel32 != light.Shadow.Pixel32)
            {
                _renderTarget?.Dispose();
                _renderTarget = CreateRenderTarget(light.Shadow.Resolution, _light.Shadow.Pixel32);
            }

            var resetView = _light.Position != light.Position || _range != range;

            _light = light;
            _range = range;

            if (resetView)
            {
                //far ~ near 비율이 너무 크면 정밀도가 매우 떨어짐
                var projection = MatrixUtil.CreatePerspectiveFovRH(MathUtil.PiOverTwo, 1, Math.Max(range / 1000, 1), range);
                _viewProjections[0] = MatrixUtil.CreateLookAtRH(light.Position, light.Position + Vector3.UnitX, -Vector3.UnitY) * projection;
                _viewProjections[1] = MatrixUtil.CreateLookAtRH(light.Position, light.Position - Vector3.UnitX, -Vector3.UnitY) * projection;
                _viewProjections[2] = MatrixUtil.CreateLookAtRH(light.Position, light.Position + Vector3.UnitY, Vector3.UnitZ) * projection;
                _viewProjections[3] = MatrixUtil.CreateLookAtRH(light.Position, light.Position - Vector3.UnitY, -Vector3.UnitZ) * projection;
                _viewProjections[4] = MatrixUtil.CreateLookAtRH(light.Position, light.Position + Vector3.UnitZ, -Vector3.UnitY) * projection;
                _viewProjections[5] = MatrixUtil.CreateLookAtRH(light.Position, light.Position - Vector3.UnitZ, -Vector3.UnitY) * projection;
            }
            graphics.Push();
            graphics.Target = _renderTarget;
            graphics.Clear(Color4.White);

            var renderer = Renderers.Shadow;
            renderer.BeginPoint(graphics, _viewProjections, light.Position, range);
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
                Shaders.Blur.DrawCube(api, blur);
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
                Target = TextureTarget.TextureCubeMap,
                Format = pixel32 ? RawFormat.Rg32f : RawFormat.Rg16f,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                WrapR = TextureWrapMode.ClampToEdge,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear
            });
            var depthTexture = new Texture(new TextureDescription()
            {
                Width = resolution,
                Height = resolution,
                Target = TextureTarget.TextureCubeMap,
                Format = RawFormat.DepthComponent16
            });
            var target = new RenderTarget(resolution, resolution);
            target.Attach(FramebufferAttachment.ColorAttachment0, colorTexture, 0, true);
            target.Attach(FramebufferAttachment.DepthAttachment, depthTexture, 0, true);
            return target;
        }
    }
}
