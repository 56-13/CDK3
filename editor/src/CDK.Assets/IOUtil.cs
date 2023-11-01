using System.Numerics;
using System.Drawing;
using System.IO;

using CDK.Drawing;

using Rectangle = CDK.Drawing.Rectangle;
using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets
{
    public static class IOUtil
    {
        public static void Write(this BinaryWriter writer, in Point value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
        }

        public static void Write(this BinaryWriter writer, in GDIRectangle value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Width);
            writer.Write(value.Height);
        }

        public static void Write(this BinaryWriter writer, in Rectangle value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Width);
            writer.Write(value.Height);
        }

        public static void Write(this BinaryWriter writer, in Vector2 value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
        }

        public static void Write(this BinaryWriter writer, in Vector3 value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }

        public static void Write(this BinaryWriter writer, in Quaternion value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
            writer.Write(value.W);
        }

        public static void Write(this BinaryWriter writer, in Color4 value, bool normalized)
        {
            if (normalized)
            {
                writer.Write(value.ToRgba());
            }
            else
            {
                writer.Write(value.R);
                writer.Write(value.G);
                writer.Write(value.B);
                writer.Write(value.A);
            }
        }

        public static void Write(this BinaryWriter writer, in Color3 value, bool normalized)
        {
            if (normalized)
            {
                writer.Write(value.ToRgba());
            }
            else
            {
                writer.Write(value.R);
                writer.Write(value.G);
                writer.Write(value.B);
            }
        }
    }
}
