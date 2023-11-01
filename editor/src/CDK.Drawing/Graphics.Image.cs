using System;
using System.Numerics;

namespace CDK.Drawing
{
    public partial class Graphics
    {
        private const float ShadowFlatMinThickness = 0.75f;

        private static readonly float ShadowFlatMaxSin = (float)Math.Sin(Math.Atan(ShadowFlatMinThickness));

        private struct ImageBlock : IDisposable
        {
            private Graphics _graphics;
            private Texture _colorMap;
            private Texture _normalMap;
            private Texture _materialMap;
            private Texture _emissiveMap;

            public ImageBlock(Graphics graphics, Texture image)
            {
                _graphics = graphics;
                _colorMap = graphics.Material.ColorMap;
                _normalMap = graphics.Material.NormalMap;
                _materialMap = graphics.Material.MaterialMap;
                _emissiveMap = graphics.Material.EmissiveMap;

                graphics.Material.ColorMap = image;
                graphics.Material.NormalMap = null;
                graphics.Material.MaterialMap = null;
                graphics.Material.EmissiveMap = null;
            }

            public void Dispose()
            {
                _graphics.Material.ColorMap = _colorMap;
                _graphics.Material.NormalMap = _normalMap;
                _graphics.Material.MaterialMap = _materialMap;
                _graphics.Material.EmissiveMap = _emissiveMap;
            }
        }

        public void DrawImage(Texture image, in Rectangle uv, in ZRectangle rect)
        {
            using (var block = new ImageBlock(this, image))
            {
                var command = new StreamRenderCommand(this, PrimitiveMode.Triangles);

                command.AddVertex(new FVertex(rect.LeftTop, uv.LeftTop));
                command.AddVertex(new FVertex(rect.RightTop, uv.RightTop));
                command.AddVertex(new FVertex(rect.LeftBottom, uv.LeftBottom));
                command.AddVertex(new FVertex(rect.RightBottom, uv.RightBottom));

                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(2);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(2);

                Command(command);
            }
        }

        public void DrawShadowFlatImage(Texture image, in Rectangle uv, in ZRectangle rect, in Vector2 offset, bool xflip, bool yflip)
        {
            if (_state.Batch.Renderer != Renderers.Shadow2D) return;

            var param = (Shadow2DRendererParam)RendererParam;

            var lightDir = param.LightDirection;

            var lz = Math.Abs(lightDir.Z);
            if (lz == 0 || lz == 1) return;

            var ld = -lightDir.ToVector2();
            var ll = ld.Length();
            if (ll == 0) return;
            ld /= ll;

            var length = ll / lz;

            var cameraRight = Vector2.Normalize(_state.Batch.Camera.Right.ToVector2());
            var cameraDown = new Vector2(-cameraRight.Y, cameraRight.X);
            var bottomLeft = (xflip ? -rect.Right : rect.Left) * cameraRight + offset.X * rect.Width * cameraDown;
            var bottomRight = (xflip ? -rect.Left : rect.Right) * cameraRight + offset.Y * rect.Width * cameraDown;
            var bottom = yflip ? rect.Top : -rect.Bottom;
            if (bottom > 0) bottom *= length;
            bottom += rect.Z * length;
            var dir = bottom * ld;
            bottomLeft += dir;
            bottomRight += dir;

            var lr = new Vector2(ld.Y, -ld.X);
            var sin = Vector2.Dot(lr, Vector2.Normalize(bottomRight - bottomLeft));
            if (sin < 0)
            {
                lr = -lr;
                sin = -sin;
                yflip = false;
            }
            if (sin < ShadowFlatMaxSin)
            {
                var tan = sin / (float)Math.Sqrt(1 - sin * sin);
                var ar = lr * ((ShadowFlatMinThickness - tan) * rect.Width * 0.5f);
                bottomLeft -= ar;
                bottomRight += ar;
            }
            var topLeft = bottomLeft;
            var topRight = bottomRight;
            dir = ld * (-bottom + ((yflip ? rect.Bottom : -rect.Top) + rect.Z) * length);
            topLeft += dir;
            topRight += dir;

            float lu, ru, tv, lbv, rbv;
            if (xflip)
            {
                lu = uv.Right;
                ru = uv.Left;
            }
            else
            {
                lu = uv.Left;
                ru = uv.Right;
            }
            if (yflip)
            {
                tv = uv.Bottom;
                lbv = (uv.Top - Math.Min(offset.X, 0));
                rbv = (uv.Top - Math.Min(offset.Y, 0));
            }
            else
            {
                tv = uv.Top;
                lbv = (uv.Bottom + Math.Min(offset.X, 0));
                rbv = (uv.Bottom + Math.Min(offset.Y, 0));
            }

            using (var block = new ImageBlock(this, image))
            {
                var command = new StreamRenderCommand(this, PrimitiveMode.Triangles);

                command.AddVertex(new FVertex(topLeft.ToVector3(), Color4.Black, new Vector2(lu, tv)));
                command.AddVertex(new FVertex(topRight.ToVector3(), Color4.Black, new Vector2(ru, tv)));
                command.AddVertex(new FVertex(bottomLeft.ToVector3(), Color4.Black, new Vector2(lu, lbv)));
                command.AddVertex(new FVertex(bottomRight.ToVector3(), Color4.Black, new Vector2(ru, rbv)));

                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(2);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(2);

                Command(command);
            }
        }

        public void DrawShadowRotateImage(Texture image, in Rectangle uv, in ZRectangle rect, in Vector2 offset, float flatness)
        {
            if (_state.Batch.Renderer != Renderers.Shadow2D) return;

            var param = (Shadow2DRendererParam)RendererParam;

            var lightDir = param.LightDirection;

            var lz = Math.Abs(lightDir.Z);
            if (lz == 0 || lz == 1) return;

            var ld = -lightDir.ToVector2();
            var ll = ld.Length();
            if (ll == 0) return;
            ld /= ll;

            var length = ll / lz;

            var cameraRight = Vector2.Normalize(_state.Batch.Camera.Right.ToVector2());
            var cameraDown = new Vector2(-cameraRight.Y, cameraRight.X);

            var lr = new Vector2(ld.Y, -ld.X);
            var sin = Vector2.Dot(lr, cameraRight);
            if (sin < 0) lr = -lr;

            var bottomLeft = rect.Left * lr;
            var bottomRight = rect.Right * lr;
            bottomLeft = Vector2.Lerp(bottomLeft, rect.Left * cameraRight, flatness);
            bottomRight = Vector2.Lerp(bottomRight, rect.Right * cameraRight, flatness);
            var bottom = -rect.Bottom;
            if (bottom > 0) bottom *= length;
            bottom += rect.Z * length;
            var dir = bottom * ld + offset.X * cameraRight + offset.Y * cameraDown;
            bottomLeft += dir;
            bottomRight += dir;

            var topLeft = bottomLeft;
            var topRight = bottomRight;
            dir = ld * (-bottom + (-rect.Top + rect.Z) * length);
            topLeft += dir;
            topRight += dir;

            using (var block = new ImageBlock(this, image))
            {
                var command = new StreamRenderCommand(this, PrimitiveMode.Triangles);

                command.AddVertex(new FVertex(topLeft.ToVector3(), Color4.Black, uv.LeftTop));
                command.AddVertex(new FVertex(topRight.ToVector3(), Color4.Black, uv.RightTop));
                command.AddVertex(new FVertex(bottomLeft.ToVector3(), Color4.Black, uv.LeftBottom));
                command.AddVertex(new FVertex(bottomRight.ToVector3(), Color4.Black, uv.RightBottom));

                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(2);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(2);

                Command(command);
            }
        }
    }
}
