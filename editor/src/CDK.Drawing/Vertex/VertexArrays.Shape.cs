using System;
using System.Numerics;

namespace CDK.Drawing
{
    public static partial class VertexArrays
    {
        public static VertexArray Get2D(params Vector2[] positions)
        {
            var data = new BufferData<Vector2>(positions.Length);
            data.AddRange(positions);

            var vertices = (VertexArray)ResourcePool.Instance.Get(data);

            if (vertices != null) return vertices;

            vertices = New(data, 1, true, 1, true, new VertexLayout(0, 0, 2, VertexAttribType.Float, false, 8, 0, 0, true));

            vertices.GetVertexBuffer(0).Upload(data, BufferUsageHint.DynamicDraw);

            return vertices;
        }

        public static VertexArray Get2D(in Rectangle bounds)
        {
            return Get2D(
                bounds.LeftTop,
                bounds.RightTop,
                bounds.LeftBottom,
                bounds.RightBottom
            );
        }

        public static VertexArray GetScreen2D()
        {
            return Get2D(
                new Vector2(-1, -1),
                new Vector2(1, -1),
                new Vector2(-1, 1),
                new Vector2(1, 1)
            );
        }

        public static VertexArray Get3D(params Vector3[] positions)
        {
            var data = new BufferData<Vector3>(positions.Length);
            data.AddRange(positions);

            var vertices = (VertexArray)ResourcePool.Instance.Get(data);

            if (vertices != null) return vertices;

            vertices = New(data, 1, true, 1, true, new VertexLayout(0, 0, 3, VertexAttribType.Float, false, 12, 0, 0, true));

            vertices.GetVertexBuffer(0).Upload(data, BufferUsageHint.DynamicDraw);

            return vertices;
        }

        public static VertexArray GetRect(int life, in ZRectangle rect, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"Rect;{rect};{uv};{fill}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var vertexData = new BufferData<VertexTNT>(4);
            var indexData = new VertexIndexData(4, fill ? 6 : 8);

            vertexData.Add(new VertexTNT(rect.LeftTop, uv.LeftTop));
            vertexData.Add(new VertexTNT(rect.RightTop, uv.RightTop));
            vertexData.Add(new VertexTNT(rect.LeftBottom, uv.LeftBottom));
            vertexData.Add(new VertexTNT(rect.RightBottom, uv.RightBottom));

            if (fill)
            {
                indexData.Add(0);
                indexData.Add(1);
                indexData.Add(2);
                indexData.Add(1);
                indexData.Add(3);
                indexData.Add(2);
            }
            else
            {
                indexData.Add(0);
                indexData.Add(1);
                indexData.Add(1);
                indexData.Add(3);
                indexData.Add(3);
                indexData.Add(2);
                indexData.Add(2);
                indexData.Add(0);
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        private static VertexArray GetGradientRect(int life, in ZRectangle rect, in Color4 leftTopColor, in Color4 rightTopColor, in Color4 leftBottomColor, in Color4 rightBottomColor, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"GradientRect;{rect};{leftTopColor};{rightTopColor};{leftBottomColor};{leftTopColor};{uv};{fill}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, Vertex.SingleBufferVertexLayouts);
            
            var vertexData = new BufferData<Vertex>(4);
            var indexData = new VertexIndexData(4, fill ? 6 : 8);

            vertexData.Add(new Vertex(rect.LeftTop, leftTopColor, uv.LeftTop));
            vertexData.Add(new Vertex(rect.RightTop, rightTopColor, uv.RightTop));
            vertexData.Add(new Vertex(rect.LeftBottom, leftBottomColor, uv.LeftBottom));
            vertexData.Add(new Vertex(rect.RightBottom, rightBottomColor, uv.RightBottom));

            if (fill)
            {
                indexData.Add(0);
                indexData.Add(1);
                indexData.Add(2);
                indexData.Add(1);
                indexData.Add(3);
                indexData.Add(2);
            }
            else
            {
                indexData.Add(0);
                indexData.Add(1);
                indexData.Add(1);
                indexData.Add(3);
                indexData.Add(3);
                indexData.Add(2);
                indexData.Add(2);
                indexData.Add(0);
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetGradientRectH(int life, in ZRectangle rect, in Color4 leftColor, in Color4 rightColor, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientRect(life, rect, leftColor, rightColor, leftColor, rightColor, fill, uv, out aabb);
        }

        public static VertexArray GetGradientRectV(int life, in ZRectangle rect, in Color4 topColor, in Color4 bottomColor, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientRect(life, rect, topColor, topColor, bottomColor, bottomColor, fill, uv, out aabb);
        }

        public static VertexArray GetRoundRect(int life, in ZRectangle rect, float radius, bool fill, Corner corner, in Rectangle uv, out ABoundingBox aabb)
        {
            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            var minRadius = Math.Min(rect.Width, rect.Height) * 0.5f;

            if (radius > minRadius) radius = minRadius;

            var key = $"RoundRect;{rect};{radius};{uv};{fill};{corner}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);
            
            var d = Drawing.Graphics.GetRadianDistance(radius);

            var vertexData = new BufferData<VertexTNT>((int)(MathUtil.Pi / d) + 1 + 4);
            var indexData = new VertexIndexData(vertexData.Capacity, fill ? (vertexData.Capacity - 2) * 3 : vertexData.Capacity * 2);

            var tw = radius * uv.Width / rect.Width;
            var th = radius * uv.Height / rect.Height;

            float s, e;

            if ((corner & Corner.LeftTop) != 0)
            {
                s = MathUtil.Pi;
                e = MathUtil.Pi * 1.5f;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Left + radius + cos * radius;
                    var sy = rect.Top + radius + sin * radius;
                    var u = uv.Left + tw + cos * tw;
                    var v = uv.Top + th + sin * th;

                    vertexData.Add(new VertexTNT(new Vector3(sx, sy, rect.Z), new Vector2(u, v)));
                }

                vertexData.Add(new VertexTNT(new Vector3(rect.Left + radius, rect.Top, rect.Z), new Vector2(uv.Left + tw, uv.Top)));
            }
            else
            {
                vertexData.Add(new VertexTNT(rect.LeftTop, uv.LeftTop));
            }

            if ((corner & Corner.RightTop) != 0)
            {
                s = MathUtil.Pi * 1.5f;
                e = MathUtil.TwoPi;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Right - radius + cos * radius;
                    var sy = rect.Top + radius + sin * radius;
                    var u = uv.Right - tw + cos * tw;
                    var v = uv.Top + th + sin * th;

                    vertexData.Add(new VertexTNT(new Vector3(sx, sy, rect.Z), new Vector2(u, v)));
                }

                vertexData.Add(new VertexTNT(new Vector3(rect.Right, rect.Top + radius, rect.Z), new Vector2(uv.Right, uv.Top + th)));
            }
            else
            {
                vertexData.Add(new VertexTNT(rect.RightTop, uv.RightTop));
            }

            if ((corner & Corner.RightBottom) != 0)
            {
                s = 0;
                e = MathUtil.PiOverTwo;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Right - radius + cos * radius;
                    var sy = rect.Bottom - radius + sin * radius;
                    var u = uv.Right - tw + cos * tw;
                    var v = uv.Bottom - th + sin * th;
                    vertexData.Add(new VertexTNT(new Vector3(sx, sy, rect.Z), new Vector2(u, v)));
                }

                vertexData.Add(new VertexTNT(new Vector3(rect.Right - radius, rect.Bottom, rect.Z), new Vector2(uv.Right - tw, uv.Bottom)));
            }
            else
            {
                vertexData.Add(new VertexTNT(rect.RightBottom, uv.RightBottom));
            }

            if ((corner & Corner.LeftBottom) != 0)
            {
                s = MathUtil.PiOverTwo;
                e = MathUtil.Pi;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Left + radius + cos * radius;
                    var sy = rect.Bottom - radius + sin * radius;
                    var u = uv.Left + tw + cos * tw;
                    var v = uv.Bottom - th + sin * th;
                    vertexData.Add(new VertexTNT(new Vector3(sx, sy, rect.Z), new Vector2(u, v)));
                }

                vertexData.Add(new VertexTNT(new Vector3(rect.Left, rect.Bottom - radius, rect.Z), new Vector2(uv.Left, uv.Bottom - th)));
            }
            else
            {
                vertexData.Add(new VertexTNT(rect.LeftBottom, uv.LeftBottom));
            }

            if (fill)
            {
                for (var i = 0; i < vertexData.Count - 2; i++)
                {
                    indexData.Add(0);
                    indexData.Add(i + 1);
                    indexData.Add(i + 2);
                }
            }
            else
            {
                for (int i = 0; i < vertexData.Count - 1; i++)
                {
                    indexData.Add(i);
                    indexData.Add(i + 1);
                }
                indexData.Add(vertexData.Count - 1);
                indexData.Add(0);
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        private static VertexArray GetGradientRoundRect(int life, in ZRectangle rect, float radius, in Color4 leftTopColor, in Color4 rightTopColor, in Color4 leftBottomColor, in Color4 rightBottomColor, bool fill, Corner corner, in Rectangle uv, out ABoundingBox aabb)
        {
            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            var minRadius = Math.Min(rect.Width, rect.Height) * 0.5f;

            if (radius > minRadius) radius = minRadius;

            var key = $"GradientRoundRect;{rect};{radius};{leftTopColor};{rightTopColor};{leftBottomColor};{leftTopColor};{uv};{fill};{corner}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, Vertex.SingleBufferVertexLayouts);

            var d = Drawing.Graphics.GetRadianDistance(radius);

            var vertexData = new BufferData<Vertex>((int)(MathUtil.Pi / d) + 1 + 4 + 1);
            var indexData = new VertexIndexData(vertexData.Capacity, fill ? (vertexData.Capacity - 1) * 3 : vertexData.Capacity * 2);

            var tw = radius * uv.Width / rect.Width;
            var th = radius * uv.Height / rect.Height;

            if (fill)
            {
                vertexData.Add(new Vertex(rect.CenterMiddle,
                    (leftTopColor + rightTopColor + leftBottomColor + rightBottomColor) * 0.25f,
                    uv.CenterMiddle));
            }

            float s, e;

            if ((corner & Corner.LeftTop) != 0)
            {
                s = MathUtil.Pi;
                e = MathUtil.Pi * 1.5f;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Left + radius + cos * radius;
                    var sy = rect.Top + radius + sin * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var u = uv.Left + tw + cos * tw;
                    var v = uv.Top + th + sin * th;

                    vertexData.Add(new Vertex(new Vector3(sx, sy, rect.Z),
                        Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr),
                        new Vector2(u, v)));
                }
                {
                    var r = radius / rect.Width;
                    vertexData.Add(new Vertex(new Vector3(rect.Left + radius, rect.Top, rect.Z),
                        Color4.Lerp(leftTopColor, rightTopColor, r),
                        new Vector2(uv.Left + tw, uv.Top)));
                }
            }
            else
            {
                vertexData.Add(new Vertex(rect.LeftTop, leftTopColor, uv.LeftTop));
            }

            if ((corner & Corner.RightTop) != 0)
            {
                s = MathUtil.Pi * 1.5f;
                e = MathUtil.TwoPi;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Right - radius + cos * radius;
                    var sy = rect.Top + radius + sin * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var u = uv.Right - tw + cos * tw;
                    var v = uv.Top + th + sin * th;

                    vertexData.Add(new Vertex(new Vector3(sx, sy, rect.Z),
                        Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr),
                        new Vector2(u, v)));
                }
                {
                    var r = radius / rect.Height;
                    vertexData.Add(new Vertex(new Vector3(rect.Right, rect.Top + radius, rect.Z),
                        Color4.Lerp(rightTopColor, rightBottomColor, r),
                        new Vector2(uv.Right, uv.Top + th)));
                }
            }
            else
            {
                vertexData.Add(new Vertex(rect.RightTop, rightTopColor, uv.RightTop));
            }

            if ((corner & Corner.RightBottom) != 0)
            {
                s = 0;
                e = MathUtil.PiOverTwo;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Right - radius + cos * radius;
                    var sy = rect.Bottom - radius + sin * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var u = uv.Right - tw + cos * tw;
                    var v = uv.Bottom - th + sin * th;

                    vertexData.Add(new Vertex(new Vector3(sx, sy, rect.Z),
                        Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr),
                        new Vector2(u, v)));
                }
                {
                    var r = (rect.Width - radius) / rect.Width;
                    vertexData.Add(new Vertex(new Vector3(rect.Right - radius, rect.Bottom, rect.Z),
                        Color4.Lerp(leftBottomColor, rightBottomColor, r),
                        new Vector2(uv.Right - tw, uv.Bottom)));
                }
            }
            else
            {
                vertexData.Add(new Vertex(rect.RightBottom, rightBottomColor, uv.RightBottom));
            }

            if ((corner & Corner.LeftBottom) != 0)
            {
                s = MathUtil.PiOverTwo;
                e = MathUtil.Pi;
                for (var a = s; a < e; a += d)
                {
                    var cos = (float)Math.Cos(a);
                    var sin = (float)Math.Sin(a);
                    var sx = rect.Left + radius + cos * radius;
                    var sy = rect.Bottom - radius + sin * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var u = uv.Left + tw + cos * tw;
                    var v = uv.Bottom - th + sin * th;

                    vertexData.Add(new Vertex(new Vector3(sx, sy, rect.Z),
                        Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr),
                        new Vector2(u, v)));
                }
                {
                    var r = (rect.Height - radius) / rect.Height;
                    vertexData.Add(new Vertex(new Vector3(rect.Left, rect.Bottom - radius, rect.Z),
                        Color4.Lerp(leftTopColor, leftBottomColor, r),
                        new Vector2(uv.Left, uv.Bottom - th)));
                }
            }
            else
            {
                vertexData.Add(new Vertex(rect.LeftBottom, leftBottomColor, uv.LeftBottom));
            }

            if (fill)
            {
                for (var i = 0; i < vertexData.Count - 2; i++)
                {
                    indexData.Add(0);
                    indexData.Add(i + 1);
                    indexData.Add(i + 2);
                }
                indexData.Add(0);
                indexData.Add(vertexData.Count - 1);
                indexData.Add(1);
            }
            else
            {
                for (var i = 0; i < vertexData.Count - 1; i++)
                {
                    indexData.Add(i);
                    indexData.Add(i + 1);
                }
                indexData.Add(vertexData.Count - 1);
                indexData.Add(0);
            }
            
            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetGradientRoundRectH(int life, in ZRectangle rect, float radius, in Color4 leftColor, in Color4 rightColor, bool fill, Corner corner, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientRoundRect(life, rect, radius, leftColor, rightColor, leftColor, rightColor, fill, corner, uv, out aabb);
        }

        public static VertexArray GetGradientRoundRectV(int life, in ZRectangle rect, float radius, in Color4 topColor, in Color4 bottomColor, bool fill, Corner corner, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientRoundRect(life, rect, radius, topColor, topColor, bottomColor, bottomColor, fill, corner, uv, out aabb);
        }

        public static VertexArray GetArc(int life, in ZRectangle rect, float angle1, float angle2, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            if (angle1 >= angle2) return null;

            var key = $"Arc;{rect};{angle1};{angle2};{uv};{fill}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var d = Graphics.GetRadianDistance(Math.Max(rect.Width, rect.Height) * 0.5f);

            var vertexData = new BufferData<VertexTNT>((int)((angle2 - angle1) / d) + 3);
            var indexData = new VertexIndexData(vertexData.Capacity, fill ? (vertexData.Capacity - 2) * 3 : (vertexData.Capacity - 1) * 2);

            var c = rect.CenterMiddle;
            var hs = rect.HalfSize;
            var uvc = uv.CenterMiddle;
            var uvhs = uv.HalfSize;

            if (fill)
            {
                vertexData.Add(new VertexTNT(c, uvc));
            }

            float x, y, u, v, cosq, sinq;

            for (var a = angle1; a < angle2; a += d)
            {
                cosq = (float)Math.Cos(a);
                sinq = (float)Math.Sin(a);
                x = c.X + hs.X * cosq;
                y = c.Y + hs.Y * sinq;
                u = uvc.X + uvhs.X * cosq;
                v = uvc.Y + uvhs.Y * sinq;
                vertexData.Add(new VertexTNT(new Vector3(x, y, rect.Z), new Vector2(u, v)));
            }

            cosq = (float)Math.Cos(angle2);
            sinq = (float)Math.Sin(angle2);
            x = c.X + hs.X * cosq;
            y = c.Y + hs.Y * sinq;
            u = uvc.X + uvhs.X * cosq;
            v = uvc.Y + uvhs.Y * sinq;
            vertexData.Add(new VertexTNT(new Vector3(x, y, rect.Z), new Vector2(u, v)));

            if (fill)
            {
                for (var i = 0; i < vertexData.Count - 2; i++)
                {
                    indexData.Add(0);
                    indexData.Add((ushort)(i + 1));
                    indexData.Add((ushort)(i + 2));
                }
            }
            else
            {
                for (var i = 0; i < vertexData.Count - 1; i++)
                {
                    indexData.Add((ushort)i);
                    indexData.Add((ushort)(i + 1));
                }
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetArc(int life, in Vector3 pos, float radius, float angle1, float angle2, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetArc(life, new ZRectangle(pos, radius * 2, radius * 2), angle1, angle2, fill, uv, out aabb);
        }

        public static VertexArray GetCircle(int life, in ZRectangle rect, bool fill, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetArc(life, rect, 0, MathUtil.TwoPi, fill, uv, out aabb);
        }

        public static VertexArray GetCircle(int life, in Vector3 pos, float radius, in Rectangle uv, bool fill, out ABoundingBox aabb)
        {
            return GetArc(life, new ZRectangle(pos, radius * 2, radius * 2), 0, MathUtil.TwoPi, fill, uv, out aabb);
        }

        public static VertexArray GetGradientArc(int life, in ZRectangle rect, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv, out ABoundingBox aabb)
        {
            aabb = new ABoundingBox(rect.LeftTop, rect.RightBottom);

            if (angle1 >= angle2) return null;

            var key = $"GradientArc;{rect};{angle1};{angle2};{centerColor};{surroundColor};{uv}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, Vertex.SingleBufferVertexLayouts);

            var d = Graphics.GetRadianDistance(Math.Max(rect.Width, rect.Height) * 0.5f);

            var vertexData = new BufferData<Vertex>((int)((angle2 - angle1) / d) + 3);
            var indexData = new VertexIndexData(vertexData.Capacity, (vertexData.Capacity - 2) * 3);

            var c = rect.CenterMiddle;
            var hs = rect.HalfSize;
            var uvc = uv.CenterMiddle;
            var uvhs = uv.HalfSize;

            vertexData.Add(new Vertex(c, centerColor, uvc));

            float x, y, u, v, cosq, sinq;

            for (var a = angle1; a < angle2; a += d)
            {
                cosq = (float)Math.Cos(a);
                sinq = (float)Math.Sin(a);
                x = c.X + hs.X * cosq;
                y = c.Y + hs.Y * sinq;
                u = uvc.X + uvhs.X * cosq;
                v = uvc.Y + uvhs.Y * sinq;
                vertexData.Add(new Vertex(new Vector3(x, y, rect.Z), surroundColor, new Vector2(u, v)));
            }

            cosq = (float)Math.Cos(angle2);
            sinq = (float)Math.Sin(angle2);
            x = c.X + hs.X * cosq;
            y = c.Y + hs.Y * sinq;
            u = uvc.X + uvhs.X * cosq;
            v = uvc.Y + uvhs.Y * sinq;
            vertexData.Add(new Vertex(new Vector3(x, y, rect.Z), surroundColor, new Vector2(u, v)));

            for (var i = 0; i < vertexData.Count - 2; i++)
            {
                indexData.Add(0);
                indexData.Add(i + 1);
                indexData.Add(i + 2);
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetGradientArc(int life, in Vector3 pos, float radius, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientArc(life, new ZRectangle(pos, radius * 2, radius * 2), angle1, angle2, centerColor, surroundColor, uv, out aabb);
        }

        public static VertexArray GetGradientCircle(int life, in ZRectangle rect, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetGradientArc(life, rect, 0, MathUtil.TwoPi, centerColor, surroundColor, uv, out aabb);
        }

        public static VertexArray GetSphere(int life, in Vector3 pos, float radius, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"Sphere;{pos};{radius};{uv}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            aabb = new ABoundingBox(pos - new Vector3(radius), pos + new Vector3(radius));

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var d = Graphics.GetRadianDistance(radius);

            var div = (int)(MathUtil.Pi / d);
            var div2 = div * 2;

            var vertexData = new BufferData<VertexTNT>((div + 1) * (div2 + 1));
            var indexData = new VertexIndexData(vertexData.Capacity, div * div2 * 6);

            var v = uv.Top;
            var vd = uv.Height / div;
            var ud = uv.Width / div2;

            for (var i = 0; i <= div; i++)
            {
                var ir = MathUtil.Pi * i / div;
                var z = (float)Math.Cos(ir);
                var hd = (float)Math.Sin(ir);
                var u = uv.Left;
                for (var j = 0; j <= div2; j++)
                {
                    var jr = MathUtil.TwoPi * (j % div2) / div2;
                    var cosq = (float)Math.Cos(jr);
                    var sinq = (float)Math.Sin(jr);
                    var x = hd * sinq;
                    var y = hd * cosq;
                    var normal = new Vector3(x, y, z);
                    var tangent = new Vector3(cosq, -sinq, 0);

                    vertexData.Add(new VertexTNT(pos + normal * radius, new Vector2(u, v), normal, tangent));
                    u += ud;
                }
                v += vd;
            }

            var vi = 0;

            for (var i = 0; i < div; i++)
            {
                for (var j = 0; j < div2; j++)
                {
                    indexData.Add(vi);
                    indexData.Add(vi + 1);
                    indexData.Add(vi + div2 + 1);
                    indexData.Add(vi + 1);
                    indexData.Add(vi + div2 + 2);
                    indexData.Add(vi + div2 + 1);

                    vi++;
                }
                vi++;
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetCapsule(int life, in Vector3 pos, float height, float radius, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"Capsule;{pos};{radius};{height};{uv}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            var abbr = new Vector3(radius, radius, height + radius);
            aabb = new ABoundingBox(pos - abbr, pos + abbr);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var d = Graphics.GetRadianDistance(radius);

            var div = (int)(MathUtil.PiOverTwo / d);
            var div2 = div * 2;
            var div4 = div * 4;

            var vertexData = new BufferData<VertexTNT>(((div + 1) * (div4 + 1)) * 2);
            var indexData = new VertexIndexData(vertexData.Capacity, (div2 + 1) * div4 * 6);

            var v = uv.Top;
            var vd = uv.Height / (div2 + 1);
            var ud = uv.Width / div4;

            var tp = new Vector3(pos.X, pos.Y, pos.Z + height);
            for (var i = 0; i <= div; i++)
            {
                var ir = MathUtil.PiOverTwo * i / div;
                var z = (float)Math.Cos(ir);
                var hd = (float)Math.Sin(ir);
                var u = uv.Left;
                for (var j = 0; j <= div4; j++)
                {
                    var jr = MathUtil.TwoPi * (j % div4) / div4;
                    var cosq = (float)Math.Cos(jr);
                    var sinq = (float)Math.Sin(jr);
                    var x = hd * sinq;
                    var y = hd * cosq;
                    var normal = new Vector3(x, y, z);
                    var tangent = new Vector3(cosq, -sinq, 0);

                    vertexData.Add(new VertexTNT(tp + normal * radius, new Vector2(u, v), normal, tangent));
                    u += ud;
                }
                v += vd;
            }

            var bp = new Vector3(pos.X, pos.Y, pos.Z - height);
            for (var i = 0; i <= div; i++)
            {
                var ir = MathUtil.PiOverTwo * (i + div) / div;
                var z = (float)Math.Cos(ir);
                var hd = (float)Math.Sin(ir);
                var u = uv.Left;
                for (var j = 0; j <= div4; j++)
                {
                    var jr = MathUtil.TwoPi * (j % div4) / div4;
                    var cosq = (float)Math.Cos(jr);
                    var sinq = (float)Math.Sin(jr);
                    var x = hd * sinq;
                    var y = hd * cosq;
                    var normal = new Vector3(x, y, z);
                    var tangent = new Vector3(cosq, -sinq, 0);

                    vertexData.Add(new VertexTNT(bp + normal * radius, new Vector2(u, v), normal, tangent));
                    u += ud;
                }
                v += vd;
            }

            var vi = 0;

            for (var i = 0; i <= div2; i++)
            {
                for (var j = 0; j < div4; j++)
                {
                    indexData.Add(vi);
                    indexData.Add(vi + 1);
                    indexData.Add(vi + div4 + 1);
                    indexData.Add(vi + 1);
                    indexData.Add(vi + div4 + 2);
                    indexData.Add(vi + div4 + 1);

                    vi++;
                }
                vi++;
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetCylinder(int life, in Vector3 pos, float topRadius, float bottomRadius, float height, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"Cylinder;{pos};{topRadius};{bottomRadius};{height};{uv}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            var maxr = Math.Max(topRadius, bottomRadius);
            var aabbr = new Vector3(maxr, maxr, height);
            aabb = new ABoundingBox(pos - aabbr, pos + aabbr);

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var d = Drawing.Graphics.GetRadianDistance(Math.Max(topRadius, bottomRadius));

            var div2 = (int)(MathUtil.TwoPi / d);

            var vertexData = new BufferData<VertexTNT>((div2 + 1) * 6);
            var indexData = new VertexIndexData(vertexData.Capacity, div2 * 18);

            var tv = uv.Top;
            var wtv = uv.Top + uv.Height * topRadius / (topRadius + bottomRadius + height);
            var wbv = uv.Top + uv.Height * (topRadius + height) / (topRadius + bottomRadius + height);
            var bv = uv.Bottom;
            var u = uv.Left;
            var ud = uv.Width / div2;

            var top = pos.Z + height;
            var bottom = pos.Z - height;

            for (var i = 0; i <= div2; i++)
            {
                var ir = MathUtil.TwoPi * (i % div2) / div2;
                var x = (float)Math.Sin(ir);
                var y = (float)Math.Cos(ir);

                var normal = new Vector3(x, y, 0);
                var tangent = new Vector3(y, -x, 0);

                vertexData.Add(new VertexTNT(new Vector3(pos.X, pos.Y, top), new Vector2(u, tv), Vector3.UnitZ, Vector3.UnitX));
                vertexData.Add(new VertexTNT(new Vector3(x * topRadius, y * topRadius, top), new Vector2(u, wtv), Vector3.UnitZ, Vector3.UnitX));
                vertexData.Add(new VertexTNT(new Vector3(x * topRadius, y * topRadius, top), new Vector2(u, wtv), normal, tangent));
                vertexData.Add(new VertexTNT(new Vector3(x * bottomRadius, y * bottomRadius, bottom), new Vector2(u, wbv), normal, tangent));
                vertexData.Add(new VertexTNT(new Vector3(x * bottomRadius, y * bottomRadius, bottom), new Vector2(u, wbv), -Vector3.UnitZ, -Vector3.UnitX));
                vertexData.Add(new VertexTNT(new Vector3(pos.X, pos.Y, bottom), new Vector2(u, bv), -Vector3.UnitZ, -Vector3.UnitX));

                u += ud;
            }

            var vi = 0;

            for (var i = 0; i < div2; i++)
            {
                indexData.Add(vi);
                indexData.Add(vi + 6);
                indexData.Add(vi + 1);
                indexData.Add(vi + 1);
                indexData.Add(vi + 6);
                indexData.Add(vi + 7);

                indexData.Add(vi + 2);
                indexData.Add(vi + 2 + 6);
                indexData.Add(vi + 2 + 1);
                indexData.Add(vi + 2 + 1);
                indexData.Add(vi + 2 + 6);
                indexData.Add(vi + 2 + 7);

                indexData.Add(vi + 4);
                indexData.Add(vi + 4 + 6);
                indexData.Add(vi + 4 + 1);
                indexData.Add(vi + 4 + 1);
                indexData.Add(vi + 4 + 6);
                indexData.Add(vi + 4 + 7);

                vi += 6;
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }

        public static VertexArray GetPyramid(int life, in Vector3 pos, float size, float height, bool reverse, in Rectangle uv, out ABoundingBox aabb)
        {
            return GetHexahedron(life, pos, reverse ? new Vector2(size, size) : Vector2.Zero, reverse ? Vector2.Zero : new Vector2(size, size), height, uv, out aabb);
        }

        public static VertexArray GetBox(int life, in Vector3 min, in Vector3 max, in Rectangle uv, out ABoundingBox aabb)
        {
            var topRect = new ZRectangle(min.X, min.Y, max.Z, max.X - min.X, max.Y - min.Y);
            var bottomRect = new ZRectangle(min.X, min.Y, min.Z, max.X - min.X, max.Y - min.Y);

            return GetHexahedron(life, topRect, bottomRect, uv, out aabb);
        }

        public static VertexArray GetCube(int life, in Vector3 pos, float radius, in Rectangle uv, out ABoundingBox aabb)
        {
            var topRect = new ZRectangle(pos.X - radius, pos.Y - radius, pos.Z + radius, radius * 2, radius * 2);
            var bottomRect = new ZRectangle(pos.X - radius, pos.Y - radius, pos.Z - radius, radius * 2, radius * 2);

            return GetHexahedron(life, topRect, bottomRect, uv, out aabb);
        }

        public static VertexArray GetHexahedron(int life, in Vector3 pos, in Vector2 topSize, in Vector2 bottomSize, float height, in Rectangle uv, out ABoundingBox aabb)
        {
            var topRect = new ZRectangle(pos.X - topSize.X, pos.Y - topSize.Y, pos.Z + height, topSize.X * 2, topSize.Y * 2);
            var bottomRect = new ZRectangle(pos.X - bottomSize.X, pos.Y - bottomSize.Y, pos.Z - height, bottomSize.X * 2, bottomSize.Y * 2);

            return GetHexahedron(life, topRect, bottomRect, uv, out aabb);
        }

        public static VertexArray GetHexahedron(int life, in ZRectangle topRect, in ZRectangle bottomRect, in Rectangle uv, out ABoundingBox aabb)
        {
            var key = $"Hexahedron;{topRect};{bottomRect};{uv}";

            var vertices = (VertexArray)ResourcePool.Instance.Get(key);

            aabb = new ABoundingBox(Vector3.Min(topRect.LeftTop, bottomRect.LeftTop), Vector3.Max(topRect.RightBottom, bottomRect.RightBottom));

            if (vertices != null) return vertices;

            vertices = New(key, life, false, 1, true, VertexTNT.SingleBufferVertexLayouts);

            var positions = new Vector3[8];
            positions[0] = topRect.LeftTop;
            positions[1] = topRect.RightTop;
            positions[2] = topRect.LeftBottom;
            positions[3] = topRect.RightBottom;
            positions[4] = bottomRect.LeftTop;
            positions[5] = bottomRect.RightTop;
            positions[6] = bottomRect.LeftBottom;
            positions[7] = bottomRect.RightBottom;

            var quads = new int[6, 4]{
                {   0,1,2,3   },
                {   2,3,6,7   },
                {   0,2,4,6   },
                {   1,0,5,4   },
                {   3,1,7,5   },
                {   5,4,7,6   }
            };

            var uv0 = uv.LeftTop;
            var uv1 = uv.RightTop;
            var uv2 = uv.LeftBottom;
            var uv3 = uv.RightBottom;

            var vertexData = new BufferData<VertexTNT>(24);
            var indexData = new VertexIndexData(vertexData.Capacity, 36);

            var vi = 0;
            for (var i = 0; i < 6; i++)
            {
                var pos0 = positions[quads[i, 0]];
                var pos1 = positions[quads[i, 1]];
                var pos2 = positions[quads[i, 2]];
                var pos3 = positions[quads[i, 3]];

                var normal = Vector3.Normalize(Vector3.Cross(pos1 - pos0, pos2 - pos0));
                var tangent = Vector3.Normalize(pos1 - pos0);

                vertexData.Add(new VertexTNT(pos0, uv0, normal, tangent));
                vertexData.Add(new VertexTNT(pos1, uv1, normal, tangent));
                vertexData.Add(new VertexTNT(pos2, uv2, normal, tangent));
                vertexData.Add(new VertexTNT(pos3, uv3, normal, tangent));

                indexData.Add(vi);
                indexData.Add(vi + 1);
                indexData.Add(vi + 2);
                indexData.Add(vi + 1);
                indexData.Add(vi + 3);
                indexData.Add(vi + 2);

                vi += 4;
            }

            vertices.GetVertexBuffer(0).Upload(vertexData, BufferUsageHint.StaticDraw);
            vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);

            return vertices;
        }
    }
}
