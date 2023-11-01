using System.Numerics;

namespace CDK.Drawing
{
    public partial class Graphics
    {
        private Vector2[] GetQuad(in Rectangle rect, in Matrix4x4 wvp)
        {
            var quad = rect.GetQuad();
            for (var i = 0; i < 4; i++) quad[i] = VectorUtil.TransformCoordinate(quad[i].ToVector3(), wvp).ToVector2();
            return quad;
        }

        private Vector2[] GetQuad(in Rectangle rect) => GetQuad(rect, WorldViewProjection);

        public void Blur(float intensity)
        {
            var target = Target;

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.Draw(api, intensity);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void Blur(in Rectangle rect, float intensity)
        {
            var target = Target;
            var quad = GetQuad(rect);

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.Draw(api, quad, intensity);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurDepth(float distance, float range, float intensity)
        {
            var target = Target;
            var camera = Camera;

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawDepth(api, camera, distance, range, intensity);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurDepth(in Rectangle rect, float distance, float range, float intensity)
        {
            var target = Target;
            var quad = GetQuad(rect);
            var camera = Camera;

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawDepth(api, quad, camera, distance, range, intensity);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurDirection(Vector2 dir)
        {
            var target = Target;

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawDirection(api, dir);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurDirection(in Rectangle rect, Vector2 dir)
        {
            var target = Target;
            var quad = GetQuad(rect);

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawDirection(api, quad, dir);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurCenter(Vector2 center, float range)
        {
            var target = Target;
            center = VectorUtil.TransformCoordinate(center.ToVector3(), WorldViewProjection).ToVector2();

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawCenter(api, center, range);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void BlurCenter(in Rectangle rect, Vector2 center, float range)
        {
            var target = Target;
            var wvp = WorldViewProjection;
            var quad = GetQuad(rect, wvp);
            center = VectorUtil.TransformCoordinate((center + rect.CenterMiddle).ToVector3(), wvp).ToVector2();

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Blur.DrawCenter(api, quad, center, range);
            });

            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void Lens(Vector3 center, float radius, float convex)
        {
            if (Renderer is DistortionRenderer distortionRenderer)
            {
                var target = Target;
                var screenTexture = (Texture)RendererParam;
                var wvp = WorldViewProjection;

                var command = Command((GraphicsApi api) =>
                {
                    target.Focus(api);

                    Shaders.Lens.Draw(api, screenTexture, wvp, center, radius, convex);
                });

                command.AddFence(target, BatchFlag.Write | BatchFlag.Retrieve);
            }
        }

        public void Wave(Vector3 center, float radius, float thickness)
        {
            if (Renderer is DistortionRenderer distortionRenderer)
            {
                var target = Target;
                var screenTexture = (Texture)RendererParam;
                var wvp = WorldViewProjection;

                var command = Command((GraphicsApi api) =>
                {
                    target.Focus(api);

                    Shaders.Wave.Draw(api, screenTexture, wvp, center, radius, thickness);
                });

                command.AddFence(target, BatchFlag.Write | BatchFlag.Retrieve);
            }
        }
    }
}
