using System;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

using CDK.Drawing;

using Rectangle = CDK.Drawing.Rectangle;
using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets
{
    public static class XmlUtil
    {
        public static XmlNode GetChildNode(this XmlNode node, string name)
        {
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName == name) return subnode;
            }
            return null;
        }


        public static bool HasAttribute(this XmlNode node, string name)
        {
            return node.Attributes[name] != null;
        }

        public static Asset ReadAttributeAsset(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];

            return attr != null ? AssetManager.Instance.GetAsset(attr.Value) : null;
        }

        public static Asset[] ReadAttributeAssets(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];

            var result = new List<Asset>();
            if (attr != null)
            {
                foreach (var key in attr.Value.Split(','))
                {
                    var asset = AssetManager.Instance.GetAsset(key);

                    if (asset != null) result.Add(asset);
                }
            }
            return result.ToArray();
        }

        public static string ReadAttributeString(this XmlNode node, string name, string defaultValue = null)
        {
            var attr = node.Attributes[name];

            return attr != null ? attr.Value : defaultValue;
        }

        public static string[] ReadAttributeStrings(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];

            return attr != null ? attr.Value.Split(',') : new string[0];
        }

        public static float ReadAttributeFloat(this XmlNode node, string name, float defaultValue = 0)
        {
            var attr = node.Attributes[name];

            return attr != null ? float.Parse(attr.Value) : defaultValue;
        }

        public static float[] ReadAttributeFloats(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return new float[0];
            var ps = attr.Value.Split(',');
            var value = new float[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = float.Parse(ps[i]);
            return value;
        }

        public static int ReadAttributeInt(this XmlNode node, string name, int defaultValue = 0)
        {
            var attr = node.Attributes[name];

            return attr != null ? int.Parse(attr.Value) : defaultValue;
        }

        public static int[] ReadAttributeInts(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return new int[0];
            var ps = attr.Value.Split(',');
            var value = new int[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = int.Parse(ps[i]);
            return value;
        }

        public static uint ReadAttributeUInt(this XmlNode node, string name, uint defaultValue = 0)
        {
            var attr = node.Attributes[name];

            return attr != null ? uint.Parse(attr.Value) : defaultValue;
        }

        public static uint[] ReadAttributeUInts(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return new uint[0];
            var ps = attr.Value.Split(',');
            var value = new uint[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = uint.Parse(ps[i]);
            return value;
        }

        public static long ReadAttributeLong(this XmlNode node, string name, long defaultValue = 0)
        {
            var attr = node.Attributes[name];

            return attr != null ? long.Parse(attr.Value) : defaultValue;
        }

        public static long[] ReadAttributeLongs(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return new long[0];
            var ps = attr.Value.Split(',');
            var value = new long[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = long.Parse(ps[i]);
            return value;
        }

        public static bool ReadAttributeBool(this XmlNode node, string name, bool defaultValue = false)
        {
            var attr = node.Attributes[name];

            return attr != null ? bool.Parse(attr.Value) : defaultValue;
        }

        public static bool[] ReadAttributeBools(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return new bool[0];
            var ps = attr.Value.Split(',');
            var value = new bool[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = bool.Parse(ps[i]);
            return value;
        }

        public static decimal ReadAttributeDecimal(this XmlNode node, string name, decimal defaultValue = 0)
        {
            var attr = node.Attributes[name];

            return attr != null ? decimal.Parse(attr.Value) : defaultValue;
        }

        public static T ReadAttributeEnum<T>(this XmlNode node, string name) where T : Enum
        {
            var attr = node.Attributes[name];

            return (T)Enum.Parse(typeof(T), attr.Value);
        }

        public static T ReadAttributeEnum<T>(this XmlNode node, string name, T defaultValue) where T : Enum
        {
            var attr = node.Attributes[name];

            return attr != null ? (T)Enum.Parse(typeof(T), attr.Value) : defaultValue;
        }

        public static T[] ReadAttributeEnums<T>(this XmlNode node, string name) where T : Enum
        {
            var attr = node.Attributes[name];
            if (attr == null) return new T[0];
            var ps = attr.Value.Split(',');
            var value = new T[ps.Length];
            for (var i = 0; i < ps.Length; i++) value[i] = (T)Enum.Parse(typeof(T), ps[i]);
            return value;
        }

        public static Point ReadAttributeGDIPoint(this XmlNode node, string name) => ReadAttributeGDIPoint(node, name, Point.Empty);

        public static Point ReadAttributeGDIPoint(this XmlNode node, string name, in Point defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            var ps = attr.Value.Split(',');
            return new Point(int.Parse(ps[0]), int.Parse(ps[1]));
        }

        public static GDIRectangle ReadAttributeGDIRectangle(this XmlNode node, string name) => ReadAttributeGDIRectangle(node, name, GDIRectangle.Empty);

        public static GDIRectangle ReadAttributeGDIRectangle(this XmlNode node, string name, in GDIRectangle defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            var ps = attr.Value.Split(',');
            return new GDIRectangle(int.Parse(ps[0]), int.Parse(ps[1]), int.Parse(ps[2]), int.Parse(ps[3]));
        }

        public static Rectangle ReadAttributeRectangle(this XmlNode node, string name) => ReadAttributeRectangle(node, name, Rectangle.Zero);

        public static Rectangle ReadAttributeRectangle(this XmlNode node, string name, in Rectangle defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            var ps = attr.Value.Split(',');
            return new Rectangle(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]), float.Parse(ps[3]));
        }

        public static Vector2 ReadAttributeVector2(this XmlNode node, string name) => ReadAttributeVector2(node, name, Vector2.Zero);

        public static Vector2 ReadAttributeVector2(this XmlNode node, string name, in Vector2 defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            var ps = attr.Value.Split(',');
            return new Vector2(float.Parse(ps[0]), float.Parse(ps[1]));
        }

        public static Vector3 ReadAttributeVector3(this XmlNode node, string name) => ReadAttributeVector3(node, name, Vector3.Zero);

        public static Vector3 ReadAttributeVector3(this XmlNode node, string name, in Vector3 defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            var ps = attr.Value.Split(',');
            return new Vector3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
        }

        public static Quaternion ReadAttributeQuaternion(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null) return Quaternion.Identity;
            var ps = attr.Value.Split(',');
            return new Quaternion(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]), float.Parse(ps[3]));
        }

        public static Color4 ReadAttributeColor4(this XmlNode node, string name, bool normalized) => ReadAttributeColor4(node, name, normalized, Color4.White);

        public static Color4 ReadAttributeColor4(this XmlNode node, string name, bool normalized, in Color4 defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            if (normalized)
            {
                var rgba = uint.Parse(attr.Value, NumberStyles.HexNumber);
                return new Color4(rgba);
            }
            else
            {
                var ps = attr.Value.Split(',');
                return new Color4(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]), float.Parse(ps[3]));
            }
        }

        public static Color3 ReadAttributeColor3(this XmlNode node, string name, bool normalized) => ReadAttributeColor3(node, name, normalized, Color3.White);

        public static Color3 ReadAttributeColor3(this XmlNode node, string name, bool normalized, in Color3 defaultValue)
        {
            var attr = node.Attributes[name];
            if (attr == null) return defaultValue;
            if (normalized)
            {
                var rgba = uint.Parse(attr.Value, NumberStyles.HexNumber);
                return new Color3(rgba);
            }
            else
            {
                var ps = attr.Value.Split(',');
                return new Color3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Asset value)
        {
            if (value == null) return;

            writer.WriteAttributeString(name, value.Key);
        }

        public static void WriteAttribute(this XmlWriter writer, string name, string value) => WriteAttribute(writer, name, value, null);

        public static void WriteAttribute(this XmlWriter writer, string name, string value, string defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value);
        }
        
        public static void WriteAttribute(this XmlWriter writer, string name, int value, int defaultValue = 0)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, uint value, uint defaultValue = 0)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, long value, long defaultValue = 0)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, float value, float defaultValue = 0)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, bool value, bool defaultValue = false)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, decimal value, decimal defaultValue = 0)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute<T>(this XmlWriter writer, string name, T value) where T : Enum
        {
            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute<T>(this XmlWriter writer, string name, T value, T defaultValue) where T : Enum
        {
            if (value.Equals(defaultValue)) return;

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttributes<T>(this XmlWriter writer, string name, IEnumerable<T> value)
        {
            var strbuf = new StringBuilder();
            var first = true;
            foreach (var v in value)
            {
                if (first) first = false;
                else strbuf.Append(',');
                strbuf.Append(v.ToString());
            }
            if (first) return;
            writer.WriteAttributeString(name, strbuf.ToString());
        }

        public static void WriteAttributeAssets(this XmlWriter writer, string name, IEnumerable<Asset> value)
        {
            var strbuf = new StringBuilder();
            var first = true;
            foreach (var v in value)
            {
                if (first) first = false;
                else strbuf.Append(',');
                strbuf.Append(v.Key);
            }
            if (first) return;
            writer.WriteAttributeString(name, strbuf.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Point value) => WriteAttribute(writer, name, value, Point.Empty);

        public static void WriteAttribute(this XmlWriter writer, string name, in Point value, in Point defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in GDIRectangle value) => WriteAttribute(writer, name, value, GDIRectangle.Empty);

        public static void WriteAttribute(this XmlWriter writer, string name, in GDIRectangle value, in GDIRectangle defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y},{value.Width},{value.Height}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Rectangle value) => WriteAttribute(writer, name, value, Rectangle.Zero);

        public static void WriteAttribute(this XmlWriter writer, string name, in Rectangle value, in Rectangle defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y},{value.Width},{value.Height}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Vector2 value) => WriteAttribute(writer, name, value, Vector2.Zero);

        public static void WriteAttribute(this XmlWriter writer, string name, in Vector2 value, in Vector2 defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Vector3 value) => WriteAttribute(writer, name, value, Vector3.Zero);

        public static void WriteAttribute(this XmlWriter writer, string name, in Vector3 value, in Vector3 defaultValue)
        {
            if (value == defaultValue) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y},{value.Z}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Quaternion value)
        {
            if (value.IsIdentity) return;

            writer.WriteAttributeString(name, $"{value.X},{value.Y},{value.Z},{value.W}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Color4 value, bool normalized) => WriteAttribute(writer, name, value, normalized, Color4.White);

        public static void WriteAttribute(this XmlWriter writer, string name, in Color4 value, bool normalized, in Color4 defaultValue)
        {
            if (value == defaultValue) return;

            if (normalized) writer.WriteAttributeString(name, value.ToRgba().ToString("X8"));
            else writer.WriteAttributeString(name, $"{value.R},{value.G},{value.B},{value.A}");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, in Color3 value, bool normalized) => WriteAttribute(writer, name, value, normalized, Color3.White);

        public static void WriteAttribute(this XmlWriter writer, string name, in Color3 value, bool normalized, in Color3 defaultValue)
        {
            if (value == defaultValue) return;

            if (normalized) writer.WriteAttributeString(name, value.ToRgba().ToString("X8"));
            else writer.WriteAttributeString(name, $"{value.R},{value.G},{value.B}");
        }
    }
}
