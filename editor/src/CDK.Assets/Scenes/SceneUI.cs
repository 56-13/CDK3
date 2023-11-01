using System;
using System.Numerics;

using CDK.Drawing;

namespace CDK.Assets.Scenes
{
    public static class SceneUI
    {
        public static Texture LightIcon
        {
            get
            {
                var key = "LightIcon";

                var texture = (Texture)ResourcePool.Instance.Get(key);

                if (texture == null)
                {
                    texture = new Texture(Properties.Resources.light, new TextureDescription()
                    {
                        Format = RawFormat.Luminance8Alpha8,
                        MinFilter = TextureMinFilter.Linear,
                        MagFilter = TextureMagFilter.Linear
                    }, null);

                    ResourcePool.Instance.Add(key, texture, 0, false);
                }

                return texture;
            }
        }

        public const float DirectionalLightCursorRadius0 = 40;
        public const float DirectionalLightCursorRadius1 = 20;
        public const float DirectionalLightCursorLength = 80;

        public static ABoundingBox GetDirectionalLightCursorAABB() => new ABoundingBox(new Vector3(-DirectionalLightCursorRadius0, -DirectionalLightCursorRadius0, -DirectionalLightCursorLength), new Vector3(DirectionalLightCursorRadius0, DirectionalLightCursorRadius0, 0));

        public static VertexArray GetDirectionalLightCursor()
        {
            var key = "DirectionalLightCursor";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices == null)
            {
                vertices = new VertexArray(1, true, new VertexLayout[]
                {
                    new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 12, 0, 0, true)
                });

                var d = Graphics.GetRadianDistance(DirectionalLightCursorRadius0);

                var vertexData = new BufferData<Vector3>((int)(MathUtil.TwoPi / d) + 12);
                var indexData = new VertexIndexData(vertexData.Capacity, (vertexData.Capacity - 1) * 2);

                for (var t = 0f; t < MathUtil.TwoPi; t += d)
                {
                    var cos = (float)Math.Cos(t);
                    var sin = (float)Math.Sin(t);
                    var x = DirectionalLightCursorRadius0 * cos;
                    var y = DirectionalLightCursorRadius0 * sin;
                    vertexData.Add(new Vector3(x, y, 0));
                }

                for (var i = 0; i < vertexData.Count; i++)
                {
                    indexData.Add(i);
                    indexData.Add((i + 1) % vertexData.Count);
                }

                for (var i = 0; i < 6; i++)
                {
                    var t = i * MathUtil.Pi / 3;
                    var cos = (float)Math.Cos(t);
                    var sin = (float)Math.Sin(t);
                    var x0 = DirectionalLightCursorRadius0 * cos;
                    var y0 = DirectionalLightCursorRadius0 * sin;
                    var x1 = DirectionalLightCursorRadius1 * cos;
                    var y1 = DirectionalLightCursorRadius1 * sin;

                    indexData.Add(vertexData.Count);
                    indexData.Add(vertexData.Count + 1);

                    vertexData.Add(new Vector3(x0, y0, 0));
                    vertexData.Add(new Vector3(x1, y1, -DirectionalLightCursorLength));
                }

                vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
                vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

                ResourcePool.Instance.Add(key, vertices, 0, false);
            }

            return vertices;
        }

        public static void DrawDirectionalLightCursor(Graphics graphics, in Matrix4x4 transform, in Color3 color)
        {
            graphics.Push();
            graphics.Transform(transform);
            graphics.Material.Shader = MaterialShader.NoLight;
            graphics.Material.BlendMode = BlendMode.Alpha;
            graphics.Material.Color = Color4.Yellow;

            graphics.DrawVertices(GetDirectionalLightCursor(), PrimitiveMode.Lines, GetDirectionalLightCursorAABB());

            var icon = LightIcon;

            graphics.Material.Color = color.Normalized;
            graphics.Transform(graphics.Camera.View.Billboard(graphics.World));
            graphics.DrawImage(icon, Rectangle.ZeroToOne, new ZRectangle(-icon.Width / 2, -icon.Height / 2, 0, icon.Width, icon.Height));

            graphics.Pop();
        }

        public const float PointLightCursorRadius = 100;

        public static ABoundingBox GetPointLightCursorAABB(float range) => new ABoundingBox(new Vector3(-range), new Vector3(range));

        public static VertexArray GetPointLightCursor(out ABoundingBox aabb)
        {
            aabb = GetPointLightCursorAABB(PointLightCursorRadius);

            var key = "PointLightCursor";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices == null)
            {
                vertices = new VertexArray(1, true, new VertexLayout[]
                {
                    new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 12, 0, 0, true)
                });

                var d = Graphics.GetRadianDistance(PointLightCursorRadius);

                var vertexData = new BufferData<Vector3>((int)(MathUtil.TwoPi / d) * 3 + 4);
                var indexData = new VertexIndexData(vertexData.Capacity, (vertexData.Capacity - 5) * 2 + 4);

                for (var t = 0f; t < MathUtil.TwoPi; t += d)
                {
                    var cos = (float)Math.Cos(t);
                    var sin = (float)Math.Sin(t);
                    var x = PointLightCursorRadius * cos;
                    var y = PointLightCursorRadius * sin;
                    vertexData.Add(new Vector3(x, y, 0));
                    vertexData.Add(new Vector3(x, 0, y));
                    vertexData.Add(new Vector3(0, x, y));
                }

                for (var i = 0; i < vertexData.Count; i++)
                {
                    indexData.Add(i);
                    indexData.Add((i + 3) % vertexData.Count);
                }

                indexData.Add(vertexData.Count);
                indexData.Add(vertexData.Count + 1);
                indexData.Add(vertexData.Count + 2);
                indexData.Add(vertexData.Count + 3);

                vertexData.Add(new Vector3(-1, 0, 0));
                vertexData.Add(new Vector3(1, 0, 0));
                vertexData.Add(new Vector3(0, -1, 0));
                vertexData.Add(new Vector3(0, 1, 0));

                vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
                vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

                ResourcePool.Instance.Add(key, vertices, 0, false);
            }
            return vertices;
        }

        public static void DrawPointLightCursor(Graphics graphics, in Matrix4x4 transform, in Color3 color, float range)
        {
            graphics.Push();
            graphics.Transform(transform);
            graphics.Material.Shader = MaterialShader.NoLight;
            graphics.Material.BlendMode = BlendMode.Alpha;

            if (range > 0)
            {
                graphics.Material.Color = Color4.Yellow;
                graphics.PushTransform();
                graphics.Scale(range / PointLightCursorRadius);
                graphics.DrawVertices(GetPointLightCursor(out var aabb), PrimitiveMode.Lines, aabb);
                graphics.PopTransform();
            }

            var icon = LightIcon;

            graphics.Material.Color = color.Normalized;
            graphics.Transform(graphics.Camera.View.Billboard(graphics.World));
            graphics.DrawImage(icon, Rectangle.ZeroToOne, new ZRectangle(-icon.Width / 2, -icon.Height / 2, 0, icon.Width, icon.Height));

            graphics.Pop();
        }

        public const float SpotLightCursorRadius = 100;

        public static ABoundingBox GetSpotLightCursorAABB(float range, float angle)
        {
            var r = range * (float)Math.Tan(angle / 2);
            return new ABoundingBox(new Vector3(-r, -r, -range), new Vector3(r, r, 0));
        }

        public static VertexArray GetSpotLightCursor(float angle, out ABoundingBox aabb)
        {
            aabb = GetSpotLightCursorAABB(SpotLightCursorRadius, angle);

            var key = $"SpotLightCursor,{angle}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices == null)
            {
                vertices = new VertexArray(1, true, new VertexLayout[]
                {
                    new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 12, 0, 0, true)
                });

                var d = Graphics.GetRadianDistance(SpotLightCursorRadius);

                var vertexData = new BufferData<Vector3>((int)(MathUtil.TwoPi / d) + 13);
                var indexData = new VertexIndexData(vertexData.Capacity, (int)(MathUtil.TwoPi / d) * 2 + 24);

                var r = SpotLightCursorRadius * (float)Math.Tan(angle / 2);

                for (var t = 0f; t < MathUtil.TwoPi; t += d)
                {
                    var cos = (float)Math.Cos(t);
                    var sin = (float)Math.Sin(t);
                    var x = r * cos;
                    var y = r * sin;
                    vertexData.Add(new Vector3(x, y, -SpotLightCursorRadius));
                }
                for (var i = 0; i < vertexData.Count; i++)
                {
                    indexData.Add(i);
                    indexData.Add((i + 1) % vertexData.Count);
                }

                for (var i = 1; i <= 12; i++)
                {
                    indexData.Add(vertexData.Count);
                    indexData.Add(vertexData.Count + i);
                }

                vertexData.Add(Vector3.Zero);
                for (var i = 0; i < 12; i++)
                {
                    var t = i * MathUtil.TwoPi / 12;
                    var cos = (float)Math.Cos(t);
                    var sin = (float)Math.Sin(t);
                    var x = r * cos;
                    var y = r * sin;
                    vertexData.Add(new Vector3(x, y, -SpotLightCursorRadius));
                }

                vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
                vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

                ResourcePool.Instance.Add(key, vertices, 1, true);
            }
            return vertices;
        }

        public static void DrawSpotLightCursor(Graphics graphics, in Matrix4x4 transform, in Color3 color, float range, float angle)
        {
            graphics.Push();
            graphics.Transform(transform);
            graphics.Material.Shader = MaterialShader.NoLight;
            graphics.Material.BlendMode = BlendMode.Alpha;

            graphics.Material.Color = Color4.Yellow;
            graphics.PushTransform();
            graphics.Scale(range / SpotLightCursorRadius);
            graphics.DrawVertices(GetSpotLightCursor(angle, out var aabb), PrimitiveMode.Lines, aabb);
            graphics.PopTransform();

            var icon = LightIcon;

            graphics.Material.Color = color.Normalized;
            graphics.Transform(graphics.Camera.View.Billboard(graphics.World));
            graphics.DrawImage(icon, Rectangle.ZeroToOne, new ZRectangle(-icon.Width / 2, -icon.Height / 2, 0, icon.Width, icon.Height));

            graphics.Pop();
        }
    }
}
