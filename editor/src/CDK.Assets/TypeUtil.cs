using System.Text;
using System.Globalization;
using System.IO;

namespace CDK.Assets
{
    public enum IntegerElementType
    {
        None,
        Byte,
        UByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
    }

    public enum NumericElementType
    {
        None = IntegerElementType.None,
        Byte = IntegerElementType.Byte,
        UByte = IntegerElementType.UByte,
        Short = IntegerElementType.Short,
        UShort = IntegerElementType.UShort,
        Int = IntegerElementType.Int,
        UInt = IntegerElementType.UInt,
        Long = IntegerElementType.Long,
        ULong = IntegerElementType.ULong,
        Float,
        Fixed,
        Double
    }
    
    public enum ElementType
    {
        None = NumericElementType.None,
        Byte = NumericElementType.Byte,
        UByte = NumericElementType.UByte,
        Short = NumericElementType.Short,
        UShort = NumericElementType.UShort,
        Int = NumericElementType.Int,
        UInt = NumericElementType.UInt,
        Long = NumericElementType.Long,
        ULong = NumericElementType.ULong,
        Float = NumericElementType.Float,
        Fixed = NumericElementType.Fixed,
        Double = NumericElementType.Double,
        String_UTF8,
        String_UTF16,
        String_UTF16BE,
        String_UTF32,
        String_UTF32BE
    }
    
    public static class TypeUtil
    {
        public static T As<T>(this IntegerElementType type) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), type.ToString(), false);
        }

        public static T As<T>(this NumericElementType type) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), type.ToString(), false);
        }

        public static T As<T>(this ElementType type) where T : struct
        {
            return (T)System.Enum.Parse(typeof(T), type.ToString(), false);
        }

        public static decimal Convert(this IntegerElementType type, decimal value)
        {
            switch (type)
            {
                case IntegerElementType.Byte:
                    return (sbyte)value;
                case IntegerElementType.UByte:
                    return (byte)value;
                case IntegerElementType.Short:
                    return (short)value;
                case IntegerElementType.UShort:
                    return (ushort)value;
                case IntegerElementType.Int:
                    return (int)value;
                case IntegerElementType.UInt:
                    return (uint)value;
                case IntegerElementType.Long:
                    return (long)value;
                case IntegerElementType.ULong:
                    return (ulong)value;
            }
            return value;
        }

        public static decimal Convert(this NumericElementType type, decimal value)
        {
            switch (type)
            {
                case NumericElementType.Float:
                    return (decimal)(float)value;
                case NumericElementType.Fixed:
                case NumericElementType.Double:
                    return (decimal)(double)value;
                default:
                    return type.As<IntegerElementType>().Convert(value);
            }
        }

        public static decimal MinValue(this IntegerElementType type)
        {
            switch (type)
            {
                case IntegerElementType.Byte:
                    return sbyte.MinValue;
                case IntegerElementType.Short:
                    return short.MinValue;
                case IntegerElementType.Int:
                    return int.MinValue;
                case IntegerElementType.Long:
                    return long.MinValue;
                case IntegerElementType.None:
                    return decimal.MinValue;
            }
            return 0;
        }

        public static decimal MinValue(this NumericElementType type)
        {
            switch (type)
            {
                case NumericElementType.Float:
                case NumericElementType.Double:
                    return decimal.MinValue;
                case NumericElementType.Fixed:
                    return int.MinValue;
                default:
                    return type.As<IntegerElementType>().MinValue();
            }
        }

        public static decimal MaxValue(this IntegerElementType type)
        {
            switch (type)
            {
                case IntegerElementType.Byte:
                    return sbyte.MaxValue;
                case IntegerElementType.UByte:
                    return byte.MaxValue;
                case IntegerElementType.Short:
                    return short.MaxValue;
                case IntegerElementType.UShort:
                    return ushort.MaxValue;
                case IntegerElementType.Int:
                    return int.MaxValue;
                case IntegerElementType.UInt:
                    return uint.MaxValue;
                case IntegerElementType.Long:
                    return long.MaxValue;
                case IntegerElementType.ULong:
                    return ulong.MaxValue;
            }
            return decimal.MaxValue;
        }

        public static decimal MaxValue(this NumericElementType type)
        {
            switch (type)
            {
                case NumericElementType.Float:
                case NumericElementType.Double:
                    return decimal.MaxValue;
                case NumericElementType.Fixed:
                    return int.MaxValue;
                default:
                    return type.As<IntegerElementType>().MaxValue();
            }
        }
        
        public static int GetSize(this IntegerElementType type)
        {
            switch (type)
            {
                case IntegerElementType.Byte:
                case IntegerElementType.UByte:
                    return 1;
                case IntegerElementType.Short:
                case IntegerElementType.UShort:
                    return 2;
                case IntegerElementType.Int:
                case IntegerElementType.UInt:
                    return 4;
                case IntegerElementType.Long:
                case IntegerElementType.ULong:
                    return 8;
            }
            return 0;
        }

        public static int GetSize(this NumericElementType type)
        {
            switch (type)
            {
                case NumericElementType.Float:
                    return 4;
                case NumericElementType.Double:
                case NumericElementType.Fixed:
                    return 8;
                default:
                    return type.As<IntegerElementType>().GetSize();
            }
        }

        public static void Write(this BinaryWriter writer, IntegerElementType type, decimal value)
        {
            switch (type)
            {
                case IntegerElementType.Byte:
                    writer.Write((sbyte)value);
                    break;
                case IntegerElementType.UByte:
                    writer.Write((byte)value);
                    break;
                case IntegerElementType.Short:
                    writer.Write((short)value);
                    break;
                case IntegerElementType.UShort:
                    writer.Write((ushort)value);
                    break;
                case IntegerElementType.Int:
                    writer.Write((int)value);
                    break;
                case IntegerElementType.UInt:
                    writer.Write((uint)value);
                    break;
                case IntegerElementType.Long:
                    writer.Write((long)value);
                    break;
                case IntegerElementType.ULong:
                    writer.Write((ulong)value);
                    break;
            }
        }

        public static void Write(this BinaryWriter writer, NumericElementType type, decimal value)
        {
            switch (type)
            {
                case NumericElementType.Float:
                    writer.Write((float)value);
                    break;
                case NumericElementType.Fixed:
                    writer.WriteFixed((float)value);
                    break;
                case NumericElementType.Double:
                    writer.Write((double)value);
                    break;
                case NumericElementType.None:
                    break;
                default:
                    writer.Write(type.As<IntegerElementType>(), value);
                    break;
            }
        }

        public static void WriteFixed(this BinaryWriter writer, float value)
        {
            writer.Write((long)((double)value * 65536));
        }

        public static void WriteString(this BinaryWriter writer, string value, string enc = "utf-8")
        {
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Encoding.GetEncoding(enc).GetBytes(value);
                writer.WriteLength(bytes.Length);
                writer.Write(bytes);
            }
            else
            {
                writer.Write((byte)0);
            }
        }

        public static int GetStringSize(string value, string enc = "utf-8")
        {
            var bytes = Encoding.GetEncoding(enc).GetBytes(value);
            
            return GetLengthSize(bytes.Length) + bytes.Length;
        }

        public static void Write(this BinaryWriter writer, ElementType type, string value)
        {
            switch (type)
            {
                case ElementType.String_UTF8:
                    writer.WriteString(value, "utf-8");
                    break;
                case ElementType.String_UTF16:
                    writer.WriteString(value, "utf-16");
                    break;
                case ElementType.String_UTF16BE:
                    writer.WriteString(value, "utf-16BE");
                    break;
                case ElementType.String_UTF32:
                    writer.WriteString(value, "utf-32");
                    break;
                case ElementType.String_UTF32BE:
                    writer.WriteString(value, "utf-32BE");
                    break;
                case ElementType.None:
                    break;
                default:
                    {
                        decimal v;

                        if (string.IsNullOrEmpty(value))
                        {
                            v = 0;
                        } 
                        else if (value.StartsWith("0x"))
                        {
                            v = uint.Parse(value.Substring(2), NumberStyles.HexNumber);
                        }
                        else
                        {
                            v = decimal.Parse(value);
                        }
                        writer.Write(type.As<NumericElementType>(), v);
                    }
                    break;
            }
        }

        public static int GetLengthSize(int length)
        {
            var lengthSize = 1;

            for (int i = 21; i > 0; i -= 7)
            {
                var current = (length >> i) & 0x7f;

                if (current > 0)
                {
                    lengthSize++;
                }
            }
            return lengthSize;
        }

        public static void WriteLength(this BinaryWriter writer, int length)
        {
            for (var i = 21; i > 0; i -= 7)
            {
                var current = length >> i;

                if (current > 0)
                {
                    writer.Write((byte)((current & 0x7f) | 0x80));
                }
            }
            writer.Write((byte)(length & 0x7f));
        }
    }
}
