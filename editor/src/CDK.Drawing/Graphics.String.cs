using System;
using System.Linq;
using System.Numerics;
using System.Drawing;

namespace CDK.Drawing
{
    public partial class Graphics
    {
        private const float StringWidthUnlimited = 100000f;
        private const float LineBreakEpsilon = 0.001f;

        public struct StringParam
        {
            public StringDisplay Content { private set; get; }
            public int Start;
            public int End;

            public StringParam(StringDisplay content)
            {
                Content = content;
                Start = 0;
                End = Content.Content.Length;
            }

            public StringParam(StringDisplay content, int offset, int length)
            {
                Debug.Assert(offset >= 0 && offset < content.Content.Length);
                Content = content;
                Start = offset;
                End = Math.Min(offset + length, content.Content.Length);
            }

            public static implicit operator StringParam(StringDisplay content) => new StringParam(content);
        }

        private struct ParagraphDisplay
        {
            public enum Visible
            {
                Forward,
                Visible,
                Backward
            }
            public Font font;
            public Vector2 size;
            public Visible visible;
        }   

        private static ParagraphDisplay[] GetParagraphDisplays(in StringParam str, Font font)
        {
            var cs = Glyphs.Instance;

            var result = new ParagraphDisplay[str.Content.Paragraphs.Count()];

            var i = 0;

            foreach (var pa in str.Content.Paragraphs) 
            {
                switch (pa.Type)
                {
                    case StringParagraphType.Linebreak:
                        {
                            var d = new ParagraphDisplay
                            {
                                font = font
                            };
                            if (pa.End <= str.Start) d.visible = ParagraphDisplay.Visible.Forward;
                            else if (pa.Start >= str.End) d.visible = ParagraphDisplay.Visible.Backward;
                            else
                            {
                                d.size.Y = font.Size;
                                d.visible = ParagraphDisplay.Visible.Visible;
                            }
                            result[i++] = d;
                        }
                        break;
                    case StringParagraphType.Neutral:
                    case StringParagraphType.LTR:
                    case StringParagraphType.RTL:
                    case StringParagraphType.Space:
                        {
                            var d = new ParagraphDisplay
                            {
                                font = font
                            };
                            if (pa.End <= str.Start) d.visible = ParagraphDisplay.Visible.Forward;
                            else if (pa.Start >= str.End) d.visible = ParagraphDisplay.Visible.Backward;
                            else
                            {
                                for (int ci = pa.Start; ci < pa.End; ci++)
                                {
                                    var cc = str.Content.GetGrapheme(ci);

                                    var ccs = cs.GetSize(cc, font);

                                    d.size.X += ccs.X;
                                    if (d.size.Y < ccs.Y) d.size.Y = ccs.Y;
                                }
                                d.visible = ParagraphDisplay.Visible.Visible;
                            }
                        }
                        break;
                    case StringParagraphType.Font:
                        {
                            var style = pa.FontStyle >= 0 ? (FontStyle)pa.FontStyle : font.Style;
                            font = pa.FontName != null ? new Font(pa.FontName, pa.FontSize, style) : new Font(font.Name, pa.FontSize, style);
                        }
                        goto case default;
                    default:
                        {
                            var d = new ParagraphDisplay
                            {
                                font = font,
                                visible = ParagraphDisplay.Visible.Visible
                            };
                            result[i++] = d;
                        }
                        break;
                }
            }

            return result;
        }

        private void DrawStringCharacter(Texture texture, in Rectangle frame, in Vector3 pos, int capacity, ref StreamRenderCommand command) 
        {
            if (command == null || command.State.Material.ColorMap != texture) 
            {
                if (command != null) Command(command);
                command = new StreamRenderCommand(this, PrimitiveMode.Triangles, 4 * capacity, 6 * capacity);
                command.State.Material.ColorMap = texture;
            }

            var lu = frame.Left / texture.Width;
            var ru = frame.Right / texture.Width;
            var tv = frame.Top / texture.Height;
            var bv = frame.Bottom / texture.Height;

            var vi = command.VertexCount;

            command.AddVertex(new FVertex(pos, _state.FontColors[0], new Vector2(lu, tv)));
            command.AddVertex(new FVertex(new Vector3(pos.X + frame.Width, pos.Y, pos.Z), _state.FontColors[1], new Vector2(ru, tv)));
            command.AddVertex(new FVertex(new Vector3(pos.X, pos.Y + frame.Height, pos.Z), _state.FontColors[2], new Vector2(lu, bv)));
            command.AddVertex(new FVertex(new Vector3(pos.X + frame.Width, pos.Y + frame.Height, pos.Z), _state.FontColors[3], new Vector2(ru, bv)));

            command.AddIndex(vi + 0);
            command.AddIndex(vi + 1);
            command.AddIndex(vi + 2);
            command.AddIndex(vi + 1);
            command.AddIndex(vi + 3);
            command.AddIndex(vi + 2);
        }

        private void DrawStringCharacterEnd(ref StreamRenderCommand command)
        {
            if (command != null)
            {
                Command(command);
                command = null;
            }
        }


        private Vector2 GetStringSizeImpl(in StringParam str, float width, ParagraphDisplay[] paraDisplays) 
        {
            var size = Vector2.Zero;
            var lineSize = Vector2.Zero;
            var space = 0f;

            var paras = str.Content.Paragraphs;

            for (int pi = 0; pi < paras.Length; pi++)
            {
                var pa = paras[pi];
                var pd = paraDisplays[pi];

                if (pd.visible == ParagraphDisplay.Visible.Visible)
                {
                    switch (pa.Type)
                    {
                        case StringParagraphType.Linebreak:
                            {
                                if (size.X < lineSize.X) size.X = lineSize.X;
                                size.Y += Math.Max(lineSize.Y, pd.size.Y);
                                lineSize = Vector2.Zero;
                                space = 0;
                            }
                            break;
                        case StringParagraphType.Space:
                            if (lineSize.X > 0)
                            {
                                if (lineSize.Y < pd.size.Y) lineSize.Y = pd.size.Y;
                                space += pd.size.X;
                            }
                            break;
                        case StringParagraphType.Neutral:
                        case StringParagraphType.LTR:
                        case StringParagraphType.RTL:
                            {
                                if (lineSize.X > 0 && lineSize.X + space + pd.size.X > width)
                                {
                                    if (size.X < lineSize.X) size.X = lineSize.X;
                                    size.Y += Math.Max(lineSize.Y, pd.size.Y);
                                    lineSize = pd.size;
                                }
                                else
                                {
                                    if (lineSize.Y < pd.size.Y) lineSize.Y = pd.size.Y;
                                    lineSize.X += space + pd.size.X;
                                }
                                space = 0;
                            }
                            break;
                    }
                }
                else if (pd.visible == ParagraphDisplay.Visible.Backward) break;
            }

            if (size.X < lineSize.X) size.X = lineSize.X;
            if (lineSize.X > 0) size.Y += lineSize.Y;

            return size;
        }

        public Vector2 GetStringSize(in StringParam str, Font font, float width)
        {
            return GetStringSizeImpl(str, width, GetParagraphDisplays(str, font));
        }

        public Vector2 GetStringSize(in StringParam str, float width)
        {
            return Vector2.Zero;        //TODO
            //return GetStringSizeImpl(str, _state.Font, width, GetParagraphDisplays(str, _state.Font));
        }

        private Vector2 GetStringLineSizeImpl(in StringParam str, int pi, float width, ParagraphDisplay[] paraDisplays)
        {
            var size = Vector2.Zero;
            var space = 0f;

            var paras = str.Content.Paragraphs;
            while (pi < paras.Length)
            {
                var pa = paras[pi];
                var pd = paraDisplays[pi];

                if (pd.visible == ParagraphDisplay.Visible.Visible)
                {
                    switch (pa.Type)
                    {
                        case StringParagraphType.Linebreak:
                            if (size.Y < pd.size.Y) size.Y = pd.size.Y;
                            goto exit;
                        case StringParagraphType.Space:
                            if (size.X > 0)
                            {
                                if (size.Y < pd.size.Y) size.Y = pd.size.Y;
                                space += pd.size.X;
                            }
                            break;
                        case StringParagraphType.Neutral:
                        case StringParagraphType.LTR:
                        case StringParagraphType.RTL:
                            {
                                if (size.X > 0 && size.X + space + pd.size.X > width) goto exit;
                                if (size.Y < pd.size.Y) size.Y = pd.size.Y;
                                size.X += space + pd.size.X;
                                space = 0;
                            }
                            break;
                    }
                }
                else if (pd.visible == ParagraphDisplay.Visible.Backward) break;

                pi++;
            }
        exit:
            return size;
        }


        private static float GetStringX(int align, float width, float lineWidth, bool rtl)
        {
            switch (align)
            {
                case 0:
                    return rtl ? width - lineWidth : 0;
                case 1:
                    return (rtl ? width + lineWidth : width - lineWidth) * 0.5f;
                case 2:
                    return rtl ? width : width - lineWidth;
            }
            return 0;
        }

        private void DrawStringImpl(in StringParam str, in ZRectangle destRect, float scroll, ParagraphDisplay[] paraDisplays)
        {
            /*
            Push();

            _state.Batch.Material.ColorMap = null;
            _state.Batch.Material.NormalMap = null;
            _state.Batch.Material.MaterialMap = null;
            _state.Batch.Material.EmissiveMap = null;
            _state.Batch.Layer = 1;

            var cs = Glyphs.Instance;

            var originColor = _state.Color;
            var currentColor = originColor;

            var rtl = str.Content.IsRTL;
            var align = rtl ? 2 : 0;
            var x = rtl ? destRect.Width : 0;
            var y = -scroll;
            var space = 0f;
            var lineWidth = 0f;

            var lineSize = GetStringLineSizeImpl(str, 0, destRect.Width, paraDisplays);

            StreamRenderCommand command = null;

            for (int pi = 0; pi < str.Content.Paragraphs.Length; pi++)
            {
                var pa = str.Content.Paragraphs[pi];
                var pd = paraDisplays[pi];

                if (pd.visible == ParagraphDisplay.Visible.Visible)
                {
                    switch (pa.Type)
                    {
                        case StringParagraphType.Color:
                            if (pa.Color0 != Color4.Transparent)
                            {
                                _state.Color = currentColor = pa.Color0 * originColor;
                            }
                            else
                            {
                                _state.Color = currentColor = originColor;
                            }
                            if (command != null) command.Color = currentColor;
                            break;
                        case StringParagraphType.Stroke:
                            DrawStringCharacterEnd(ref command);
                            if (pa.Color0 != Color4.Transparent) _state.Batch.StrokeColor = pa.Color0;
                            _state.Batch.StrokeWidth = pa.StrokeWidth;
                            break;
                        case StringParagraphType.Align:
                            if (align != pa.Align)
                            {
                                align = pa.Align;
                                x = GetStringX(align, destRect.Width, lineSize.X, rtl);
                            }
                            break;
                        case StringParagraphType.Gradient:
                            _state.Color = originColor;
                            if (pa.GradientHorizontal) SetFontColorH(pa.Color0, pa.Color1);
                            else SetFontColorV(pa.Color0, pa.Color1);
                            if (command != null) command.Color = originColor;
                            break;
                        case StringParagraphType.GradientReset:
                            _state.Color = originColor;
                            ResetFontColor();
                            if (command != null) command.Color = currentColor;
                            break;
                        case StringParagraphType.Linebreak:
                            {
                                y += lineSize.Y;
                                lineSize = GetStringLineSizeImpl(str, pi + 1, destRect.Width, paraDisplays);
                                if (y + lineSize.Y > destRect.Height) goto exit;
                                x = GetStringX(align, destRect.Width, lineSize.X, rtl);
                                lineWidth = 0;
                                space = 0;
                            }
                            break;
                        case StringParagraphType.Space:
                            if (lineWidth > 0) space += pd.size.X;
                            break;
                        case StringParagraphType.Neutral:
                        case StringParagraphType.LTR:
                        case StringParagraphType.RTL:
                            if (lineWidth > 0 && lineWidth + space + pd.size.X > destRect.Width + LineBreakEpsilon)
                            {
                                y += lineSize.Y;
                                lineSize = GetStringLineSizeImpl(str, pi, destRect.Width, paraDisplays);
                                if (y + lineSize.Y > destRect.Height) goto exit;
                                x = GetStringX(align, destRect.Width, lineSize.X, rtl);
                                lineWidth = 0;
                                space = 0;
                            }
                            if (y + pd.size.Y > 0)
                            {
                                var cx = destRect.X + x + (rtl ? -space : space);
                                var cy = destRect.Y + y;
                                var crtl = pa.Type == StringParagraphType.RTL;
                                if (rtl && !crtl) cx -= pd.size.X;

                                var start = Math.Max(str.Start, pa.Start);
                                var end = Math.Min(str.End, pa.End);

                                for (var ci = start; ci < end; ci++)
                                {
                                    var cc = str.Content.GetGrapheme(ci);

                                    var ccw = cs.GetSize(cc, pd.font).X;

                                    if (cs.GetImage(cc, pd.font, out var texture, out var frame))
                                    {
                                        DrawStringCharacter(texture, frame, new Vector3(crtl ? cx - ccw : cx, cy, destRect.Z), str.Content.Characters.Length, ref command);
                                    }
                                    cx += crtl ? -ccw : ccw;
                                }
                            }
                            {
                                var pw = space + pd.size.X;
                                lineWidth += pw;
                                x += rtl ? -pw : pw;
                                space = 0;
                            }
                            break;
                    }
                }
                else if (pd.visible == ParagraphDisplay.Visible.Backward) goto exit;
            }
        exit:
            DrawStringCharacterEnd(ref command);

            Pop();
            */
            //TODO
        }


        public void DrawStringScrolled(in StringParam str, in ZRectangle destRect, float scroll) 
        {
            //DrawStringImpl(str, destRect, scroll, GetParagraphDisplays(str, _state.Font));
            //TODO
        }

        public void DrawString(in StringParam str, in ZRectangle destRect) 
        {
            DrawStringScrolled(str, destRect, 0);
        }

        public void DrawString(in StringParam str, ZRectangle destRect, Align align)
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            if (((int)align & (AlignComponent.Center | AlignComponent.Right | AlignComponent.Middle | AlignComponent.Bottom)) != 0)
            {
                var size = GetStringSizeImpl(str, destRect.Width, paraDisplays);

                float scroll = 0;

                if (((int)align & AlignComponent.Center) != 0)
                {
                    destRect.X += (destRect.Width - size.X) * 0.5f;
                    destRect.Width = size.X;
                }
                else if (((int)align & AlignComponent.Right) != 0)
                {
                    destRect.X += destRect.Width - size.X;
                    destRect.Width = size.X;
                }
                if (((int)align & AlignComponent.Middle) != 0)
                {
                    if (size.Y > destRect.Height)
                    {
                        scroll = (size.Y - destRect.Height) * 0.5f;
                    }
                    else
                    {
                        destRect.Y += (destRect.Height - size.Y) * 0.5f;
                        destRect.Height = size.Y;
                    }
                }
                else if (((int)align & AlignComponent.Bottom) != 0)
                {
                    if (size.Y > destRect.Height)
                    {
                        scroll = size.Y - destRect.Height;
                    }
                    else
                    {
                        destRect.Y += destRect.Height - size.Y;
                        destRect.Height = size.Y;
                    }
                }
                DrawStringImpl(str, destRect, scroll, paraDisplays);
            }
            else
            {
                DrawStringImpl(str, destRect, 0, paraDisplays);
            }
            */
            //TODO
        }

        public float DrawString(in StringParam str, in Vector3 point, float width = StringWidthUnlimited) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            if (width >= StringWidthUnlimited) width = GetStringSizeImpl(str, StringWidthUnlimited, paraDisplays).X;

            DrawStringImpl(str, new ZRectangle(point, new Vector2(width, StringWidthUnlimited)), 0, paraDisplays);

            return width;
            */
            return 0;       //TODO
        }

        public float DrawString(in StringParam str, Vector3 point, Align align, float width = StringWidthUnlimited) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var size = GetStringSizeImpl(str, width, paraDisplays);

            point = align.Adjust(point, size);

            DrawStringImpl(str, new ZRectangle(point, size), 0, paraDisplays);

            return width >= StringWidthUnlimited ? size.X : width;
            */
            //TODO
            return 0;
        }
        public void DrawStringScaled(in StringParam str, in Vector3 point, float width) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var size = GetStringSizeImpl(str, StringWidthUnlimited, paraDisplays);

            if (size.X > width)
            {
                var s = width / size.X;

                PushTransform();
                Translate(point);
                Scale(s);
                DrawStringImpl(str, new ZRectangle(Vector3.Zero, new Vector2(width / s, size.Y)), 0, paraDisplays);
                PopTransform();
            }
            else
            {
                size.X = width;
                DrawStringImpl(str, new ZRectangle(point, size), 0, paraDisplays);
            }
            */
            //TODO
        }

        public void DrawStringScaled(in StringParam str, Vector3 point, Align align, float width) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var size = GetStringSizeImpl(str, StringWidthUnlimited, paraDisplays);

            if (size.X > width)
            {
                var s = width / size.X;

                point = align.Adjust(point, size * s);

                PushTransform();
                Translate(point);
                Scale(s);
                DrawStringImpl(str, new ZRectangle(Vector3.Zero, new Vector2(width / s, size.Y)), 0, paraDisplays);
                PopTransform();
            }
            else
            {
                point = align.Adjust(point, size);
                DrawStringImpl(str, new ZRectangle(point, size), 0, paraDisplays);
            }
            */
            //TODO
        }

        public void DrawStringScaled(in StringParam str, in ZRectangle destRect) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var size = GetStringSizeImpl(str, destRect.Width, paraDisplays);

            if (size.Y > destRect.Height)
            {
                float s = 0;

                for (; ; )
                {
                    float ns = Math.Min(destRect.Width / size.X, destRect.Height / size.Y);
                    if (ns > s)
                    {
                        s = ns;
                        size = GetStringSizeImpl(str, destRect.Width / s, paraDisplays);
                    }
                    else
                    {
                        break;
                    }
                }
                PushTransform();
                Translate(destRect.LeftTop);
                Scale(s);
                DrawStringImpl(str, new ZRectangle(Vector3.Zero, size), 0, paraDisplays);
                PopTransform();
            }
            else
            {
                DrawStringImpl(str, destRect, 0, paraDisplays);
            }
            */
            //TODO
        }

        public void DrawStringScaled(in StringParam str, ZRectangle destRect, Align align) 
        {
            /*
            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var size = GetStringSizeImpl(str, destRect.Width, paraDisplays);

            if (size.Y > destRect.Height)
            {
                float s = 0;

                for (; ; )
                {
                    float ns = Math.Min(destRect.Width / size.X, destRect.Height / size.Y);
                    if (ns > s)
                    {
                        s = ns;
                        size = GetStringSizeImpl(str, destRect.Width / s, paraDisplays);
                    }
                    else
                    {
                        break;
                    }
                }

                if (((int)align & AlignComponent.Center) != 0)
                {
                    destRect.X += (destRect.Width - size.X * s) * 0.5f;
                }
                else if (((int)align & AlignComponent.Right) != 0)
                {
                    destRect.X += destRect.Width - size.X * s;
                }
                if (((int)align & AlignComponent.Middle) != 0)
                {
                    destRect.Y += (destRect.Height - size.Y * s) * 0.5f;
                }
                else if (((int)align & AlignComponent.Bottom) != 0)
                {
                    destRect.Y += destRect.Height - size.Y * s;
                }
                destRect.Width = size.X;
                destRect.Height = size.Y;

                PushTransform();
                Translate(destRect.LeftTop);
                Scale(s);

                destRect.X = 0;
                destRect.Y = 0;
                destRect.Z = 0;

                DrawStringImpl(str, destRect, 0, paraDisplays);
                PopTransform();
            }
            else
            {
                if (((int)align & AlignComponent.Center) != 0)
                {
                    destRect.X += (destRect.Width - size.X) * 0.5f;
                }
                else if (((int)align & AlignComponent.Right) != 0)
                {
                    destRect.X += destRect.Width - size.X;
                }
                if (((int)align & AlignComponent.Middle) != 0)
                {
                    destRect.Y += (destRect.Height - size.Y) * 0.5f;
                }
                else if (((int)align & AlignComponent.Bottom) != 0)
                {
                    destRect.Y += destRect.Height - size.Y;
                }
                destRect.Width = size.X;
                destRect.Height = size.Y;

                DrawStringImpl(str, destRect, 0, paraDisplays);
            }
            */
            //TODO
        }
        public void DrawStringTruncated(in StringParam str, in Vector3 point, float width) 
        {
            DrawStringTruncated(str, point, Align.LeftTop, width);
        }

        private static readonly StringDisplay truncateStr = new StringDisplay("...");
        public void DrawStringTruncated(StringParam str, Vector3 point, Align align, float width) {
            /*
            var cs = Glyphs.Instance;

            var trstr = new StringDisplay("...");
            var trpd = GetParagraphDisplays(trstr, _state.Font);
            float trw = GetStringSizeImpl(trstr, StringWidthUnlimited, trpd).X;

            var paraDisplays = GetParagraphDisplays(str, _state.Font);

            var rtl = str.Content.IsRTL;

            var lineSize = Vector2.Zero;

            for (var pi = 0; pi < str.Content.Paragraphs.Length; pi++)
            {
                var pa = str.Content.Paragraphs[pi];
                var pd = paraDisplays[pi];

                if (pd.visible == ParagraphDisplay.Visible.Visible)
                {
                    switch (pa.Type)
                    {
                        case StringParagraphType.Linebreak:
                            return;
                        case StringParagraphType.Neutral:
                        case StringParagraphType.LTR:
                        case StringParagraphType.RTL:
                        case StringParagraphType.Space:
                            //if (lineSize.x + pd.size.x + trw > width) {
                            if (lineSize.X + pd.size.X > width)
                            {
                                point = align.Adjust(point, lineSize);
                                str.End = pa.Start;
                                if (str.Start < str.End)
                                {
                                    DrawStringImpl(str, new ZRectangle(point, new Vector2(width, StringWidthUnlimited)), 0, paraDisplays);
                                }
                                DrawStringImpl(trstr, new ZRectangle(rtl ? point.X - trw : point.X + lineSize.X, point.Y, point.Z, trw, StringWidthUnlimited), 0, trpd);
                                return;
                            }
                            if (lineSize.Y < pd.size.Y) lineSize.Y = pd.size.Y;
                            lineSize.X += pd.size.X;
                            break;
                    }
                }
                else if (pd.visible == ParagraphDisplay.Visible.Backward) break;
            }
            point = align.Adjust(point, lineSize);
            DrawStringImpl(str, new ZRectangle(point, new Vector2(width, StringWidthUnlimited)), 0, paraDisplays);
            */
            //TODO
        }
    }
}
