using System;
using System.Numerics;

namespace CDK.Drawing
{
    public partial class Graphics
    {
        public void DrawPoint(in Vector3 point) 
        {
            var command = new StreamRenderCommand(this, PrimitiveMode.Points, 1, 1);

            command.AddVertex(new FVertex(point));

            command.AddIndex(0);

            Command(command);
        }

        public void DrawLine(in Vector3 point1, in Vector3 point2)
        {
            var command = new StreamRenderCommand(this, PrimitiveMode.Lines, 2, 2);

            command.AddVertex(new FVertex(point1));
            command.AddVertex(new FVertex(point2));

            command.AddIndex(0);
            command.AddIndex(1);

            Command(command);
        }

        public void DrawGradientLine(in Vector3 point1, in Color4 color1, in Vector3 point2, in Color4 color2)
        {
            var command = new StreamRenderCommand(this, PrimitiveMode.Lines, 2, 2);

            command.AddVertex(new FVertex(point1, color1));
            command.AddVertex(new FVertex(point2, color2));

            command.AddIndex(0);
            command.AddIndex(1);

            Command(command);
        }

        public void DrawRect(in ZRectangle rect, bool fill, in Rectangle uv)
        {
            var command = new StreamRenderCommand(this, fill ? PrimitiveMode.Triangles : PrimitiveMode.Lines, 4, fill ? 6 : 8);

            command.AddVertex(new FVertex(rect.LeftTop, uv.LeftTop));
            command.AddVertex(new FVertex(rect.RightTop, uv.RightTop));
            command.AddVertex(new FVertex(rect.LeftBottom, uv.LeftBottom));
            command.AddVertex(new FVertex(rect.RightBottom, uv.RightBottom));

            if (fill)
            {
                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(2);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(2);
            }
            else
            {
                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(3);
                command.AddIndex(2);
                command.AddIndex(2);
                command.AddIndex(0);
            }
            Command(command);
        }

        public void DrawRect(in ZRectangle rect, bool fill) => DrawRect(rect, fill, Rectangle.ZeroToOne);

        private void DrawGradientRect(in ZRectangle rect, in Color4 leftTopColor, in Color4 rightTopColor, in Color4 leftBottomColor, in Color4 rightBottomColor, bool fill, in Rectangle uv)
        {
            var command = new StreamRenderCommand(this, fill ? PrimitiveMode.Triangles : PrimitiveMode.Lines, 4, fill ? 6 : 8);

            command.AddVertex(new FVertex(rect.LeftTop, leftTopColor, uv.LeftTop));
            command.AddVertex(new FVertex(rect.RightTop, rightTopColor, uv.RightTop));
            command.AddVertex(new FVertex(rect.LeftBottom, leftBottomColor, uv.LeftBottom));
            command.AddVertex(new FVertex(rect.RightBottom, rightBottomColor, uv.RightBottom));

            if (fill)
            {
                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(2);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(2);
            }
            else
            {
                command.AddIndex(0);
                command.AddIndex(1);
                command.AddIndex(1);
                command.AddIndex(3);
                command.AddIndex(3);
                command.AddIndex(2);
                command.AddIndex(2);
                command.AddIndex(0);
            }
            Command(command);
        }

        public void DrawGradientRectH(in ZRectangle rect, in Color4 leftColor, in Color4 rightColor, bool fill, in Rectangle uv) => DrawGradientRect(rect, leftColor, rightColor, leftColor, rightColor, fill, uv);
        public void DrawGradientRectV(in ZRectangle rect, in Color4 topColor, in Color4 bottomColor, bool fill, in Rectangle uv) => DrawGradientRect(rect, topColor, topColor, bottomColor, bottomColor, fill, uv);
        public void DrawGradientRectH(in ZRectangle rect, in Color4 leftColor, in Color4 rightColor, bool fill) => DrawGradientRectH(rect, leftColor, rightColor, fill, Rectangle.ZeroToOne);
        public void DrawGradientRectV(in ZRectangle rect, in Color4 topColor, in Color4 bottomColor, bool fill) => DrawGradientRectV(rect, topColor, bottomColor, fill, Rectangle.ZeroToOne);

        public static float GetRadianDistance(float radius)
        {
            return radius > 1 ? (float)Math.Acos((radius - 1) / radius) : MathUtil.PiOverTwo;
        }

        public void DrawRoundRect(in ZRectangle rect, float radius, bool fill, Corner corner, in Rectangle uv)
        {
            var minRadius = Math.Min(rect.Width, rect.Height) * 0.5f;

            if (radius > minRadius) radius = minRadius;

            var d = GetRadianDistance(radius);

            var vertexCapacity = (int)Math.Ceiling(MathUtil.TwoPi / d);
            var indexCapacity = fill ? (vertexCapacity - 2) * 3 : vertexCapacity * 2;

            var command = new StreamRenderCommand(this, fill ? PrimitiveMode.Triangles : PrimitiveMode.Lines, vertexCapacity, indexCapacity);

            float s, e;

            if ((corner & Corner.LeftTop) != 0)
            {
                s = MathUtil.Pi;
                e = MathUtil.Pi * 1.5f;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Left + radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Top + radius + (float)Math.Sin(a) * radius;
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), uv.LeftTop));
                }
                command.AddVertex(new FVertex(new Vector3(rect.Left + radius, rect.Top, rect.Z), uv.LeftTop));
            }
            else
            {
                command.AddVertex(new FVertex(rect.LeftTop, uv.LeftTop));
            }

            if ((corner & Corner.RightTop) != 0)
            {
                s = MathUtil.Pi * 1.5f;
                e = MathUtil.TwoPi;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Right - radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Top + radius + (float)Math.Sin(a) * radius;
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), uv.RightTop));
                }
                command.AddVertex(new FVertex(new Vector3(rect.Right, rect.Top + radius, rect.Z), uv.RightTop));
            }
            else
            {
                command.AddVertex(new FVertex(rect.RightTop, uv.RightTop));
            }

            if ((corner & Corner.RightBottom) != 0)
            {
                s = 0;
                e = MathUtil.PiOverTwo;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Right - radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Bottom - radius + (float)Math.Sin(a) * radius;
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), uv.RightBottom));
                }
                command.AddVertex(new FVertex(new Vector3(rect.Right - radius, rect.Bottom, rect.Z), uv.RightBottom));
            }
            else
            {
                command.AddVertex(new FVertex(rect.RightBottom, uv.RightBottom));
            }

            if ((corner & Corner.LeftBottom) != 0)
            {
                s = MathUtil.PiOverTwo;
                e = MathUtil.Pi;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Left + radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Bottom - radius + (float)Math.Sin(a) * radius;
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), uv.LeftBottom));
                }
                command.AddVertex(new FVertex(new Vector3(rect.Left, rect.Bottom - radius, rect.Z), uv.LeftBottom));
            }
            else
            {
                command.AddVertex(new FVertex(rect.LeftBottom, uv.LeftBottom));
            }

            if (fill)
            {
                for (var i = 0; i < command.VertexCount - 2; i++)
                {
                    command.AddIndex(0);
                    command.AddIndex(i + 1);
                    command.AddIndex(i + 2);
                }
            }
            else
            {
                for (var i = 0; i < command.VertexCount - 1; i++)
                {
                    command.AddIndex(i);
                    command.AddIndex(i + 1);
                }
                command.AddIndex(command.VertexCount - 1);
                command.AddIndex(0);
            }
            Command(command);
        }

        public void DrawRoundRect(in ZRectangle rect, float radius, bool fill, in Rectangle uv) => DrawRoundRect(rect, radius, fill, Corner.All, uv);
        public void DrawRoundRect(in ZRectangle rect, float radius, bool fill, Corner corner) => DrawRoundRect(rect, radius, fill, corner, Rectangle.ZeroToOne);
        public void DrawRoundRect(in ZRectangle rect, float radius, bool fill) => DrawRoundRect(rect, radius, fill, Corner.All, Rectangle.ZeroToOne);

        private void DrawGradientRoundRect(in ZRectangle rect, float radius, in Color4 leftTopColor, in Color4 rightTopColor, in Color4 leftBottomColor, in Color4 rightBottomColor, bool fill, Corner corner, in Rectangle uv)
        {
            var minRadius = Math.Min(rect.Width, rect.Height) * 0.5f;

            if (radius > minRadius) radius = minRadius;

            var d = GetRadianDistance(radius);

            var vertexCapacity = (int)Math.Ceiling(MathUtil.TwoPi / d) + 4;
            var indexCapacity = fill ? (vertexCapacity - 1) * 3 : vertexCapacity * 2;

            var command = new StreamRenderCommand(this, fill ? PrimitiveMode.Triangles : PrimitiveMode.Lines, vertexCapacity, indexCapacity);

            if (fill)
            {
                command.AddVertex(new FVertex(rect.CenterMiddle, (leftTopColor + rightTopColor + leftBottomColor + rightBottomColor) * 0.25f));
            }
            float s, e;

            if ((corner & Corner.LeftTop) != 0)
            {
                s = MathUtil.Pi;
                e = MathUtil.Pi * 1.5f;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Left + radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Top + radius + (float)Math.Sin(a) * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var color = Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr);
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), color, uv.LeftTop));
                }
                {
                    var r = radius / rect.Width;
                    var color = Color4.Lerp(leftTopColor, rightTopColor, r);
                    command.AddVertex(new FVertex(new Vector3(rect.Left + radius, rect.Top, rect.Z), color, uv.LeftTop));
                }
            }
            else
            {
                command.AddVertex(new FVertex(rect.LeftTop, leftTopColor, uv.LeftTop));
            }

            if ((corner & Corner.RightTop) != 0)
            {
                s = MathUtil.Pi * 1.5f;
                e = MathUtil.TwoPi;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Right - radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Top + radius + (float)Math.Sin(a) * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var color = Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr);
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), color, uv.RightTop));
                }
                {
                    var r = radius / rect.Height;
                    var color = Color4.Lerp(rightTopColor, rightBottomColor, r);
                    command.AddVertex(new FVertex(new Vector3(rect.Right, rect.Top + radius, rect.Z), color, uv.RightTop));
                }
            }
            else
            {
                command.AddVertex(new FVertex(rect.RightTop, rightTopColor, uv.RightTop));
            }

            if ((corner & Corner.RightBottom) != 0)
            {
                s = 0;
                e = MathUtil.PiOverTwo;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Right - radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Bottom - radius + (float)Math.Sin(a) * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var color = Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr);
                    command.AddVertex(new FVertex(new Vector3(sx, sy, rect.Z), color, uv.RightBottom));
                }
                {
                    var r = (rect.Width - radius) / rect.Width;
                    var color = Color4.Lerp(leftBottomColor, rightBottomColor, r);
                    command.AddVertex(new FVertex(new Vector3(rect.Right - radius, rect.Bottom, rect.Z), color, uv.RightBottom));
                }
            }
            else
            {
                command.AddVertex(new FVertex(rect.RightBottom, rightBottomColor, uv.RightBottom));
            }

            if ((corner & Corner.LeftBottom) != 0)
            {
                s = MathUtil.PiOverTwo;
                e = MathUtil.Pi;
                for (var a = s; a < e; a += d)
                {
                    var sx = rect.Left + radius + (float)Math.Cos(a) * radius;
                    var sy = rect.Bottom - radius + (float)Math.Sin(a) * radius;
                    var xr = (sx - rect.Left) / rect.Width;
                    var yr = (sy - rect.Top) / rect.Height;
                    var color = Color4.Lerp(Color4.Lerp(leftTopColor, rightTopColor, xr), Color4.Lerp(leftBottomColor, rightBottomColor, xr), yr);
                    command.AddVertex(new FVertex(new Vector3(sx, sy, 0), color, uv.LeftBottom));
                }
                {
                    var r = (rect.Height - radius) / rect.Height;
                    var color = Color4.Lerp(leftTopColor, leftBottomColor, r);
                    command.AddVertex(new FVertex(new Vector3(rect.Left, rect.Bottom - radius, rect.Z), color, uv.LeftBottom));
                }
            }
            else
            {
                command.AddVertex(new FVertex(rect.LeftBottom, leftBottomColor, uv.LeftBottom));
            }

            if (fill)
            {
                for (var i = 0; i < command.VertexCount - 2; i++)
                {
                    command.AddIndex(0);
                    command.AddIndex(i + 1);
                    command.AddIndex(i + 2);
                }
                command.AddIndex(0);
                command.AddIndex(command.VertexCount - 1);
                command.AddIndex(1);
            }
            else
            {
                for (var i = 0; i < command.VertexCount - 1; i++)
                {
                    command.AddIndex(i);
                    command.AddIndex(i + 1);
                }
                command.AddIndex(command.VertexCount - 1);
                command.AddIndex(0);
            }
            Command(command);
        }

        public void DrawGradientRoundRectH(in ZRectangle rect, float radius, in Color4 leftColor, in Color4 rightColor, bool fill, Corner corner, in Rectangle uv) => DrawGradientRoundRect(rect, radius, leftColor, rightColor, leftColor, rightColor, fill, corner, uv);
        public void DrawGradientRoundRectV(in ZRectangle rect, float radius, in Color4 topColor, in Color4 bottomColor, bool fill, Corner corner, in Rectangle uv) => DrawGradientRoundRect(rect, radius, topColor, topColor, bottomColor, bottomColor, fill, corner, uv);
        public void DrawGradientRoundRectH(in ZRectangle rect, float radius, in Color4 leftColor, in Color4 rightColor, bool fill, in Rectangle uv) => DrawGradientRoundRectH(rect, radius, leftColor, rightColor, fill, Corner.All, uv);
        public void DrawGradientRoundRectH(in ZRectangle rect, float radius, in Color4 leftColor, in Color4 rightColor, bool fill, Corner corner) => DrawGradientRoundRectH(rect, radius, leftColor, rightColor, fill, corner, Rectangle.ZeroToOne);
        public void DrawGradientRoundRectH(in ZRectangle rect, float radius, in Color4 leftColor, in Color4 rightColor, bool fill) => DrawGradientRoundRectH(rect, radius, leftColor, rightColor, fill, Corner.All, Rectangle.ZeroToOne);
        public void DrawGradientRoundRectV(in ZRectangle rect, float radius, in Color4 topColor, in Color4 bottomColor, bool fill, in Rectangle uv) => DrawGradientRoundRectV(rect, radius, topColor, bottomColor, fill, Corner.All, uv);
        public void DrawGradientRoundRectV(in ZRectangle rect, float radius, in Color4 topColor, in Color4 bottomColor, bool fill, Corner corner) => DrawGradientRoundRectV(rect, radius, topColor, bottomColor, fill, corner, Rectangle.ZeroToOne);
        public void DrawGradientRoundRectV(in ZRectangle rect, float radius, in Color4 topColor, in Color4 bottomColor, bool fill) => DrawGradientRoundRectV(rect, radius, topColor, bottomColor, fill, Corner.All, Rectangle.ZeroToOne);

        public void DrawArc(in ZRectangle rect, float angle1, float angle2, bool fill, in Rectangle uv)
        {
            if (angle1 >= angle2) return;

            var d = GetRadianDistance(Math.Max(rect.Width, rect.Height) * 0.5f);

            var vertexCapacity = (int)Math.Ceiling((angle2 - angle1) / d) + 2;
            var indexCapacity = fill ? (vertexCapacity - 1) * 3 : (vertexCapacity - 1) * 2;

            var command = new StreamRenderCommand(this, fill ? PrimitiveMode.Triangles : PrimitiveMode.Lines, vertexCapacity, indexCapacity);

            var c = rect.CenterMiddle;
            var hs = rect.HalfSize;
            var uvc = uv.CenterMiddle;
            var uvhs = uv.HalfSize;

            if (fill)
            {
                command.AddVertex(new FVertex(c, uvc));
            }

            float x, y, t, v, cosq, sinq;

            for (var a = angle1; a < angle2; a += d)
            {
                cosq = (float)Math.Cos(a);
                sinq = (float)Math.Sin(a);
                x = c.X + hs.X * cosq;
                y = c.Y + hs.Y * sinq;
                t = uvc.X + uvhs.X * cosq;
                v = uvc.Y + uvhs.Y * sinq;
                command.AddVertex(new FVertex(new Vector3(x, y, c.Z), new Vector2(t, v)));
            }

            cosq = (float)Math.Cos(angle2);
            sinq = (float)Math.Sin(angle2);
            x = c.X + hs.X * cosq;
            y = c.Y + hs.Y * sinq;
            t = uvc.X + uvhs.X * cosq;
            v = uvc.Y + uvhs.Y * sinq;
            command.AddVertex(new FVertex(new Vector3(x, y, c.Z), new Vector2(t, v)));

            if (fill)
            {
                for (var i = 0; i < command.VertexCount - 2; i++)
                {
                    command.AddIndex(0);
                    command.AddIndex(i + 1);
                    command.AddIndex(i + 2);
                }
            }
            else
            {
                for (var i = 0; i < command.VertexCount - 1; i++)
                {
                    command.AddIndex(i);
                    command.AddIndex(i + 1);
                }
            }

            Command(command);
        }

        public void DrawArc(in ZRectangle rect, float angle1, float angle2, bool fill) => DrawArc(rect, angle1, angle2, fill, Rectangle.ZeroToOne);
        public void DrawCircle(in ZRectangle rect, bool fill, in Rectangle uv) => DrawArc(rect, 0, MathUtil.TwoPi, fill, uv);
        public void DrawCircle(in ZRectangle rect, bool fill) => DrawCircle(rect, fill, Rectangle.ZeroToOne);

        public void DrawArc(in Vector3 pos, float radius, float angle1, float angle2, bool fill, in Rectangle uv) 
        {
            DrawArc(new ZRectangle(pos.X - radius, pos.Y - radius, pos.Z, radius * 2, radius * 2), angle1, angle2, fill, uv);
        }
        public void DrawArc(in Vector3 pos, float radius, float angle1, float angle2, bool fill) => DrawArc(pos, radius, angle1, angle2, fill, Rectangle.ZeroToOne);
        public void DrawCircle(in Vector3 pos, float radius, bool fill, in Rectangle uv) => DrawArc(pos, radius, 0, MathUtil.TwoPi, fill, uv);
        public void DrawCircle(in Vector3 pos, float radius, bool fill) => DrawCircle(pos, radius, fill, Rectangle.ZeroToOne);

        public void DrawGradientArc(in ZRectangle rect, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv)
        {
            if (angle1 >= angle2) return;

            var d = GetRadianDistance(Math.Max(rect.Width, rect.Height) * 0.5f);

            var vertexCapacity = (int)Math.Ceiling((angle2 - angle1) / d) + 2;
            var indexCapacity = (vertexCapacity - 1) * 3;

            var command = new StreamRenderCommand(this, PrimitiveMode.Triangles, vertexCapacity, indexCapacity);

            var c = rect.CenterMiddle;
            var hs = rect.HalfSize;
            var uvc = uv.CenterMiddle;
            var uvhs = uv.HalfSize;

            command.AddVertex(new FVertex(c, centerColor, uvc));

            float x, y, t, v, cosq, sinq;

            for (var a = angle1; a < angle2; a += d)
            {
                cosq = (float)Math.Cos(a);
                sinq = (float)Math.Sin(a);
                x = c.X + hs.X * cosq;
                y = c.Y + hs.Y * sinq;
                t = uvc.X + uvhs.X * cosq;
                v = uvc.Y + uvhs.Y * sinq;
                command.AddVertex(new FVertex(new Vector3(x, y, c.Z), surroundColor, new Vector2(t, v)));
            }

            cosq = (float)Math.Cos(angle2);
            sinq = (float)Math.Sin(angle2);
            x = c.X + hs.X * cosq;
            y = c.Y + hs.Y * sinq;
            t = uvc.X + uvhs.X * cosq;
            v = uvc.Y + uvhs.Y * sinq;
            command.AddVertex(new FVertex(new Vector3(x, y, c.Z), surroundColor, new Vector2(t, v)));

            for (var i = 0; i < command.VertexCount - 2; i++)
            {
                command.AddIndex(0);
                command.AddIndex(i + 1);
                command.AddIndex(i + 2);
            }

            Command(command);
        }

        public void DrawGradientArc(in ZRectangle rect, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor) => DrawGradientArc(rect, angle1, angle2, centerColor, surroundColor, Rectangle.ZeroToOne);
        public void DrawGradientCircle(in ZRectangle rect, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv) => DrawGradientArc(rect, 0, MathUtil.TwoPi, centerColor, surroundColor, uv);
        public void DrawGradientCircle(in ZRectangle rect, in Color4 centerColor, in Color4 surroundColor) => DrawGradientCircle(rect, centerColor, surroundColor, Rectangle.ZeroToOne);

        public void DrawGradientArc(in Vector3 pos, float radius, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv)
        {
            DrawGradientArc(new ZRectangle(pos.X - radius, pos.Y - radius, pos.Z, radius * 2, radius * 2), angle1, angle2, centerColor, surroundColor, uv);
        }
        public void DrawGradientArc(in Vector3 pos, float radius, float angle1, float angle2, in Color4 centerColor, in Color4 surroundColor) => DrawGradientArc(pos, radius, angle1, angle2, centerColor, surroundColor, Rectangle.ZeroToOne);
        public void DrawGradientCircle(in Vector3 pos, float radius, in Color4 centerColor, in Color4 surroundColor, in Rectangle uv) => DrawGradientArc(pos, radius, 0, MathUtil.TwoPi, centerColor, surroundColor, uv);
        public void DrawGradientCircle(in Vector3 pos, float radius, in Color4 centerColor, in Color4 surroundColor) => DrawGradientCircle(pos, radius, centerColor, surroundColor, Rectangle.ZeroToOne);

        public void DrawSphere(in Vector3 pos, float radius) => DrawSphere(pos, radius, Rectangle.ZeroToOne);
        public void DrawSphere(in Vector3 pos, float radius, in Rectangle uv)
        {
            var d = GetRadianDistance(radius);

            var div = (int)(MathUtil.Pi / d);
            var div2 = div * 2;

            var vertexCapacity = (div + 1) * (div2 + 1);
            var indexCapacity = div * div2 * 6;
            var command = new StreamRenderCommand(this, PrimitiveMode.Triangles, vertexCapacity, indexCapacity);

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
                    command.AddVertex(new FVertex(pos + normal * radius, new Vector2(u, v), normal, tangent));
                    u += ud;
                }
                v += vd;
            }

            var vi = 0;

            for (var i = 0; i < div; i++)
            {
                for (var j = 0; j < div2; j++)
                {
                    command.AddIndex(vi);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi + div2 + 1);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi + div2 + 2);
                    command.AddIndex(vi + div2 + 1);

                    vi++;
                }
                vi++;
            }

            Command(command);
        }

        public void DrawCapsule(in Vector3 pos, float height, float radius) => DrawCapsule(pos, height, radius, Rectangle.ZeroToOne);
        public void DrawCapsule(in Vector3 pos, float height, float radius, in Rectangle uv)
        {
            var d = GetRadianDistance(radius);

            var div = (int)(MathUtil.PiOverTwo / d);
            var div2 = div * 2;
            var div4 = div * 4;

            var vertexCapacity = (div + 1) * (div4 + 1) * 2;
            var indexCapacity = (div2 + 1) * div4 * 6;
            var command = new StreamRenderCommand(this, PrimitiveMode.Triangles, vertexCapacity, indexCapacity);

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

                    command.AddVertex(new FVertex(tp + normal * radius, new Vector2(u, v), normal, tangent));
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

                    command.AddVertex(new FVertex(bp + normal * radius, new Vector2(u, v), normal, tangent));
                    u += ud;
                }
                v += vd;
            }

            var vi = 0;

            for (var i = 0; i <= div2; i++)
            {
                for (var j = 0; j < div4; j++)
                {
                    command.AddIndex(vi);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi + div4 + 1);
                    command.AddIndex(vi + 1);
                    command.AddIndex(vi + div4 + 2);
                    command.AddIndex(vi + div4 + 1);

                    vi++;
                }
                vi++;
            }

            Command(command);
        }

        public void DrawCylinder(in Vector3 pos, float topRadius, float bottomRadius, float height) => DrawCylinder(pos, topRadius, bottomRadius, height, Rectangle.ZeroToOne);
        public void DrawCylinder(in Vector3 pos, float topRadius, float bottomRadius, float height, in Rectangle uv)
        {
            var d = GetRadianDistance(Math.Max(topRadius, bottomRadius));

            var div2 = (int)(MathUtil.TwoPi / d);

            var vertexCapacity = (div2 + 1) * 6;
            var indexCapacity = div2 * 18;
            var command = new StreamRenderCommand(this, PrimitiveMode.Triangles, vertexCapacity, indexCapacity);

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

                command.AddVertex(new FVertex(new Vector3(pos.X, pos.Y, top), new Vector2(u, tv), Vector3.UnitZ, Vector3.UnitX));
                command.AddVertex(new FVertex(new Vector3(x * topRadius, y * topRadius, top), new Vector2(u, wtv), Vector3.UnitZ, Vector3.UnitX));
                command.AddVertex(new FVertex(new Vector3(x * topRadius, y * topRadius, top), new Vector2(u, wtv), normal, tangent));
                command.AddVertex(new FVertex(new Vector3(x * bottomRadius, y * bottomRadius, bottom), new Vector2(u, wbv), normal, tangent));
                command.AddVertex(new FVertex(new Vector3(x * bottomRadius, y * bottomRadius, bottom), new Vector2(u, wbv), -Vector3.UnitZ, -Vector3.UnitX));
                command.AddVertex(new FVertex(new Vector3(pos.X, pos.Y, bottom), new Vector2(u, bv), -Vector3.UnitZ, -Vector3.UnitX));

                u += ud;
            }

            var vi = 0;

            for (var i = 0; i < div2; i++)
            {
                command.AddIndex(vi);
                command.AddIndex(vi + 6);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 6);
                command.AddIndex(vi + 7);

                command.AddIndex(vi + 2);
                command.AddIndex(vi + 2 + 6);
                command.AddIndex(vi + 2 + 1);
                command.AddIndex(vi + 2 + 1);
                command.AddIndex(vi + 2 + 6);
                command.AddIndex(vi + 2 + 7);

                command.AddIndex(vi + 4);
                command.AddIndex(vi + 4 + 6);
                command.AddIndex(vi + 4 + 1);
                command.AddIndex(vi + 4 + 1);
                command.AddIndex(vi + 4 + 6);
                command.AddIndex(vi + 4 + 7);

                vi += 6;
            }

            Command(command);
        }

        public void DrawPyramid(in Vector3 pos, float size, float height, bool reverse) => DrawPyramid(pos, size, height, reverse, Rectangle.ZeroToOne);
        public void DrawPyramid(in Vector3 pos, float size, float height, bool reverse, in Rectangle uv)
        {
            DrawHexahedron(pos, reverse ? new Vector2(size, size) : Vector2.Zero, reverse ? Vector2.Zero : new Vector2(size, size), height, uv);
        }

        public void DrawBox(in Vector3 min, in Vector3 max) => DrawBox(min, max, Rectangle.ZeroToOne);
        public void DrawBox(in Vector3 min, in Vector3 max, in Rectangle uv)
        {
            var topRect = new ZRectangle(min.X, min.Y, max.Z, max.X - min.X, max.Y - min.Y);
            var bottomRect = new ZRectangle(min.X, min.Y, min.Z, max.X - min.X, max.Y - min.Y);

            DrawHexahedron(topRect, bottomRect, uv);
        }

        public void DrawCube(in Vector3 pos, float size) => DrawCube(pos, size, Rectangle.ZeroToOne);
        public void DrawCube(in Vector3 pos, float size, in Rectangle uv)
        {
            var topRect = new ZRectangle(pos.X - size, pos.Y - size, pos.Z + size, size * 2, size * 2);
            var bottomRect = new ZRectangle(pos.X - size, pos.Y - size, pos.Z - size, size * 2, size * 2);

            DrawHexahedron(topRect, bottomRect, uv);
        }

        public void DrawHexahedron(in Vector3 pos, in Vector2 topSize, in Vector2 bottomSize, float height)
        {
            DrawHexahedron(pos, topSize, bottomSize, height, Rectangle.ZeroToOne);
        }

        public void DrawHexahedron(in Vector3 pos, in Vector2 topSize, in Vector2 bottomSize, float height, in Rectangle uv)
        {
            var topRect = new ZRectangle(pos.X - topSize.X, pos.Y - topSize.Y, pos.Z + height, topSize.X * 2, topSize.Y * 2);
            var bottomRect = new ZRectangle(pos.X - bottomSize.X, pos.Y - bottomSize.Y, pos.Z - height, bottomSize.X * 2, bottomSize.Y * 2);

            DrawHexahedron(topRect, bottomRect, uv);
        }

        public void DrawHexahedron(in ZRectangle topRect, in ZRectangle bottomRect)
        {
            DrawHexahedron(topRect, bottomRect, Rectangle.ZeroToOne);
        }

        public void DrawHexahedron(in ZRectangle topRect, in ZRectangle bottomRect, in Rectangle uv)
        {
            var command = new StreamRenderCommand(this, PrimitiveMode.Triangles, 24, 36);

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

            var vi = 0;
            for (var i = 0; i < 6; i++)
            {
                var pos0 = positions[quads[i, 0]];
                var pos1 = positions[quads[i, 1]];
                var pos2 = positions[quads[i, 2]];
                var pos3 = positions[quads[i, 3]];

                var normal = Vector3.Normalize(Vector3.Cross(pos1 - pos0, pos2 - pos0));
                var tangent = Vector3.Normalize(pos1 - pos0);

                command.AddVertex(new FVertex(pos0, uv0, normal, tangent));
                command.AddVertex(new FVertex(pos1, uv1, normal, tangent));
                command.AddVertex(new FVertex(pos2, uv2, normal, tangent));
                command.AddVertex(new FVertex(pos3, uv3, normal, tangent));

                command.AddIndex(vi);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 2);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 3);
                command.AddIndex(vi + 2);

                vi += 4;
            }

            Command(command);
        }
    }
}
