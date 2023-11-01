using System.Drawing;

namespace CDK.Drawing
{
    internal enum StringParagraphType
    {
        Neutral,
        LTR,
        RTL,
        Space,
        Linebreak,
        Color,
        Stroke,
        Gradient,
        GradientReset,
        Align,
        Font
    }

    internal class StringParagraph
    {
        public StringParagraphType Type;
        public int Start;
        public int End;
        public Color4 Color0;
        public Color4 Color1;
        public byte StrokeWidth;
        public bool GradientHorizontal;
        public int Align;
        public string FontName;
        public float FontSize;
        public int FontStyle;
    }
}
