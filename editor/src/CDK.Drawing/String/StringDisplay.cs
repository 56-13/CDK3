using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

using Icu;

namespace CDK.Drawing
{
    public class StringDisplay
    {
        private char[] _content;
        private Boundary[] _characters;
        private StringParagraph[] _paragraphs;
        private bool _rtl;

        internal char[] Content => _content;
        internal Boundary[] Characters => _characters;
        internal StringParagraph[] Paragraphs => _paragraphs;
        internal bool IsRTL => _rtl;

        public const char FormatStringFuncPrefix = '#';
        public const int FormatStringFuncParamCount = 4;
        public const int FormatStringFuncNameLength = 16;
        public const int FormatStringFuncParamLength = 16;

        public StringDisplay(string str)
        {
            //TODO:shapeArabic not supported (icu.net)

            _content = str.ToCharArray();
            _characters = BreakIterator.GetBoundaries(BreakIterator.UBreakIteratorType.CHARACTER, null, str).ToArray();
            var words = BreakIterator.GetWordBoundaries(null, str, true).ToArray();

            var pci = 0;
            var ci = 0;
            var wi = 0;

            var texttype = StringParagraphType.Neutral;

            var paragraphs = new List<StringParagraph>();

            while (ci < _characters.Length)
            {
                switch (_content[_characters[ci].Start])
                {
                    case '\r':
                        if (_characters[ci].End - _characters[ci].Start >= 2 && _content[_characters[ci].Start + 1] == '\n')
                        {
                            var p = new StringParagraph
                            {
                                Type = StringParagraphType.Linebreak,
                                Start = ci,
                                End = ci + 1
                            };
                            paragraphs.Add(p);
                        }
                        pci = ++ci;
                        while (wi < words.Length && _characters[ci].Start >= words[wi].End) wi++;
                        continue;
                    case '\n':
                        {
                            var p = new StringParagraph
                            {
                                Type = StringParagraphType.Linebreak,
                                Start = ci,
                                End = ci + 1
                            };
                            paragraphs.Add(p);
                        }
                        pci = ++ci;
                        while (wi < words.Length && _characters[ci].Start >= words[wi].End) wi++;
                        continue;
                    case FormatStringFuncPrefix:
                        if (ReadTag(paragraphs, ref ci))
                        {
                            pci = ci;
                            while (wi < words.Length && _characters[ci].Start >= words[wi].End) wi++;
                            continue;
                        }
                        break;
                }

                var cb = _characters[ci];

                var ntexttype = StringParagraphType.Neutral;
                switch (BiDi.GetBaseDirection(str.Substring(cb.Start, cb.End - cb.Start)))
                {
                    case BiDi.BiDiDirection.LTR:
                        ntexttype = StringParagraphType.LTR;
                        break;
                    case BiDi.BiDiDirection.RTL:
                        ntexttype = StringParagraphType.RTL;
                        _rtl = true;
                        break;
                    case BiDi.BiDiDirection.NEUTRAL:
                        if (Character.IsSpace(str.Substring(cb.Start, cb.End - cb.Start))) ntexttype = StringParagraphType.Space;
                        break;
                }
                if (ntexttype != texttype && pci < ci)
                {
                    var p = new StringParagraph
                    {
                        Type = texttype,
                        Start = pci,
                        End = ci
                    };
                    paragraphs.Add(p);
                    pci = ci;
                }

                texttype = ntexttype;

                if (wi < words.Length && cb.End >= words[wi].End)
                {
                    var p = new StringParagraph
                    {
                        Type = texttype,
                        Start = pci,
                        End = ci + 1
                    };
                    paragraphs.Add(p);

                    pci = ci + 1;
                    wi++;
                }
                ci++;
            }
            _paragraphs = paragraphs.ToArray();
        }

        private static int ParseTag(char[] str, int i, Func<string, string[], int, bool> cb) 
        {
	        var prev = i;

            string func = null;
            var ps = new string[FormatStringFuncParamCount];
            var paramCount = 0;

            var flag = true;
            do {
                switch (str[i])
                {
                    case '\0':
                        return 0;
                    case '(':
                        if (prev == i) return 0;
                        func = new string(str, prev, i - prev);
                        i++;
                        prev = i;
                        flag = false;
                        break;
                    default:
                        if (++i - prev > FormatStringFuncNameLength) return 0;
                        break;
                }
            } while (flag);

            flag = true;

            do
            {
                switch (str[i])
                {
                    case '\0':
                        return 0;
                    case ')':
                        flag = false;
                        if (i == prev)
                        {
                            i++;
                            prev = i;
                            break;
                        }
                        goto case ',';
                    case ',':
                        if (paramCount == FormatStringFuncParamCount) return 0;
                        while (str[prev] == ' ') prev++;
                        if (i > prev)
                        {
                            var end = i;
                            while (str[end - 1] == ' ') end--;
					        ps[paramCount] = new string(str, prev, end - prev);
                        }
                        paramCount++;
                        i++;
                        prev = i;
                        break;
                    default:
                        if (++i - prev > FormatStringFuncParamLength) return 0;
                        break;
                }
            } while (flag);

            return cb(func, ps, paramCount) ? i : 0;
        }

        private bool ReadTag(List<StringParagraph> paragraphs, ref int ci)
        {
            var next = ParseTag(_content, _characters[ci].Start + 1, (string func, string[] ps, int paramCount) => {
                if (func.Equals("color", StringComparison.InvariantCultureIgnoreCase))
                {
                    var color = Color4.Transparent;
                    if (ps[0] != null && uint.TryParse(ps[0], NumberStyles.HexNumber, null, out var rgba)) color = new Color4(rgba);
                    var p = new StringParagraph
                    {
                        Type = StringParagraphType.Color,
                        Color0 = color
                    };
                    paragraphs.Add(p);
                    return true;
                }
                else if (func.Equals("stroke", StringComparison.InvariantCultureIgnoreCase))
                {
                    var color = Color4.Transparent;
                    if (ps[0] != null && uint.TryParse(ps[0], NumberStyles.HexNumber, null, out var c)) color = new Color4(c);
                    var width = 0;
                    if (ps[1] != null && int.TryParse(ps[1], out var w)) width = w;
                    var p = new StringParagraph
                    {
                        Type = StringParagraphType.Stroke,
                        Color0 = color,
                        StrokeWidth = width
                    };
                    paragraphs.Add(p);
                    return true;
                }
                else if (func.Equals("gradient", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ps[0] != null && ps[1] != null)
                    {
                        var color0 = Color4.Transparent;
                        if (ps[0] != null && uint.TryParse(ps[0], NumberStyles.HexNumber, null, out var c0)) color0 = new Color4(c0);
                        var color1 = Color4.Transparent;
                        if (ps[1] != null && uint.TryParse(ps[1], NumberStyles.HexNumber, null, out var c1)) color1 = new Color4(c1);
                        var horizontal = false;
                        if (ps[2] != null && bool.TryParse(ps[2], out var h)) horizontal = h;

                        var p = new StringParagraph
                        {
                            Type = StringParagraphType.Gradient,
                            Color0 = color0,
                            Color1 = color1,
                            GradientHorizontal = horizontal
                        };
                        paragraphs.Add(p);
                    }
                    else
                    {
                        var p = new StringParagraph
                        {
                            Type = StringParagraphType.GradientReset
                        };
                        paragraphs.Add(p);
                    }
                    return true;
                }
                else if (func.Equals("align", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ps[0] != null) 
                    {
                        int align;
                        if (ps[0].Equals("center", StringComparison.InvariantCultureIgnoreCase)) align = 1;
                        else if (ps[0].Equals("right", StringComparison.InvariantCultureIgnoreCase)) align = 2;
                        else align = 0;

                        var p = new StringParagraph
                        {
                            Type = StringParagraphType.Align,
                            Align = align
                        };
                        paragraphs.Add(p);

                        return true;
                    }
                }
                else if (func.Equals("font", StringComparison.InvariantCultureIgnoreCase))
                {
                    var name = ps[0];
                    var size = 0f;
                    if (ps[1] != null && float.TryParse(ps[1], out var s)) size = s;
                    var style = -1;
                    if (ps[2] != null) {
                        if (ps[2].Equals("normal", StringComparison.InvariantCultureIgnoreCase)) {
                            style = (int)FontStyle.Regular;
                        }
                        else if (ps[2].Equals("bold", StringComparison.InvariantCultureIgnoreCase)) {
                            style = (int)FontStyle.Bold;
                        }
                        else if (ps[2].Equals("italic", StringComparison.InvariantCultureIgnoreCase))
                        {
                            style = (int)FontStyle.Italic;
                        }
                        else if (ps[2].Equals("medium", StringComparison.InvariantCultureIgnoreCase))
                        {
                            style = (int)FontStyle.Regular;     //not supported
                        }
                        else if (ps[2].Equals("semibold", StringComparison.InvariantCultureIgnoreCase))
                        {
                            style = (int)FontStyle.Bold;     //not supported
                        }
                    }
                    var p = new StringParagraph
                    {
                        Type = StringParagraphType.Font,
                        FontName = name,
                        FontSize = size,
                        FontStyle = style
                    };
                    paragraphs.Add(p);
                    return true;
                }
                return false;
            });

            if (next != 0)
            {
                while (_characters[ci].Start < next) ci++;
                return true;
            }
            return false;
        }

        internal string GetGrapheme(int i) => new string(_content, _characters[i].Start, _characters[i].End - _characters[i].Start);
    }
}

