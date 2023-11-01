using System.Numerics;

namespace CDK.Drawing
{
    public static class AlignComponent
    {
        public const int Center = 1;
        public const int Right = 2;
        public const int Middle = 4;
        public const int Bottom = 8;
    }

    public enum Align : byte
    {
        LeftTop = 0,
        LeftMiddle = AlignComponent.Middle,
        LeftBottom = AlignComponent.Bottom,
        CenterTop = AlignComponent.Center,
        CenterMiddle = AlignComponent.Center | AlignComponent.Middle,
        CenterBottom = AlignComponent.Center | AlignComponent.Bottom,
        RightTop = AlignComponent.Right,
        RightMiddle = AlignComponent.Right | AlignComponent.Middle,
        RightBottom = AlignComponent.Right | AlignComponent.Bottom
    }

    public static class AlignUtil
    {
        public static Vector3 Adjust(this Align align, Vector3 pos, float width, float height)
        {
            if (((int)align & AlignComponent.Center) != 0)
            {
                pos.X -= width * 0.5f;
            }
            else if (((int)align & AlignComponent.Right) != 0)
            {
                pos.X -= width;
            }
            if (((int)align & AlignComponent.Middle) != 0)
            {
                pos.Y -= height * 0.5f;
            }
            else if (((int)align & AlignComponent.Bottom) != 0)
            {
                pos.Y -= height;
            }
            return pos;
        }

        public static Vector2 Adjust(this Align align, Vector2 pos, float width, float height)
        {
            if (((int)align & AlignComponent.Center) != 0)
            {
                pos.X -= width * 0.5f;
            }
            else if (((int)align & AlignComponent.Right) != 0)
            {
                pos.X -= width;
            }
            if (((int)align & AlignComponent.Middle) != 0)
            {
                pos.Y -= height * 0.5f;
            }
            else if (((int)align & AlignComponent.Bottom) != 0)
            {
                pos.Y -= height;
            }
            return pos;
        }

        public static Vector3 Adjust(this Align align, in Vector3 pos, in Vector2 size) => Adjust(align, pos, size.X, size.Y);
        public static Vector2 Adjust(this Align align, in Vector2 pos, in Vector2 size) => Adjust(align, pos, size.X, size.Y);
    }
}
