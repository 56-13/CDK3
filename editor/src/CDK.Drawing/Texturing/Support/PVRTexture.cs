using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace CDK.Drawing
{
    internal class PVRTexture
    {
        public uint Version { private set; get; }
        public uint Flags { private set; get; }
        public ulong PixelFormat { private set; get; }
        public uint ColourSpace{private set;get;}
        public uint ChannelType{private set;get;}
        public uint Height{private set;get;}
        public uint Width{private set;get;}
        public uint Depth{private set;get;}
        public uint NumSurfaces{private set;get;}
        public uint NumFaces{private set;get;}
        public uint MipmapCount{private set;get;}
        public byte[] MetaData { private set; get; }
        public byte[] Data { private set; get; }
        

        public PVRTexture(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(fs))
                {
                    Version = reader.ReadUInt32();
                    Flags = reader.ReadUInt32();
                    PixelFormat = reader.ReadUInt64();
                    ColourSpace = reader.ReadUInt32();
                    ChannelType = reader.ReadUInt32();
                    Height = reader.ReadUInt32();
                    Width = reader.ReadUInt32();
                    Depth = reader.ReadUInt32();
                    NumSurfaces = reader.ReadUInt32();
                    NumFaces = reader.ReadUInt32();
                    MipmapCount = reader.ReadUInt32();
                    MetaData = reader.ReadBytes(reader.ReadInt32());
                    Data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                }
            }
        }

        private static readonly string SourceFileName = "in.png";
        private static readonly string DestFileName = "out.pvr";
        private static readonly string DecodedFileName = "out.png";

        public static PVRTexture Encode(Bitmap image, string format, int mipmapCount, BitmapTextureColor color, BitmapTextureResizing resizing, string cachePath)
        {
            if (cachePath != null && File.Exists(cachePath)) return new PVRTexture(cachePath);

            try
            {
                var flag = color != BitmapTextureColor.None || BitmapTexture.ConvertSize(image.Size, resizing) != image.Size;

                if (flag) image = BitmapTexture.Convert(image, color, resizing);

                image.Save(SourceFileName);

                try
                {
                    if (EncodeToFile(SourceFileName, format, mipmapCount, true, false))
                    {
                        return new PVRTexture(DestFileName);
                    }
                    else return null;
                }
                finally
                {
                    if (flag) image.Dispose();
                }
            }
            finally
            {
                if (File.Exists(SourceFileName)) File.Delete(SourceFileName);
                if (File.Exists(DestFileName))
                {
                    if (cachePath != null) File.Move(DestFileName, cachePath);
                    else File.Delete(DestFileName);
                }
            }
        }

        public static Bitmap Decode(Bitmap image, string format, int mipmapCount, BitmapTextureColor color, BitmapTextureResizing resizing, string cachePath)
        {
            try
            {
                if (cachePath != null && File.Exists(cachePath))
                {
                    if (EncodeToFile(cachePath, format, 1, false, true))
                    {
                        return new Bitmap(new MemoryStream(File.ReadAllBytes(DecodedFileName)));
                    }
                    else return null;
                }
                else
                {
                    var flag = color != BitmapTextureColor.None || BitmapTexture.ConvertSize(image.Size, resizing) != image.Size;

                    if (flag) image = BitmapTexture.Convert(image, color, resizing);

                    try
                    {
                        image.Save(SourceFileName);

                        if (EncodeToFile(SourceFileName, format, mipmapCount, true, true))
                        {
                            return new Bitmap(new MemoryStream(File.ReadAllBytes(DecodedFileName)));
                        }
                        else return null;
                    }
                    finally
                    {
                        if (flag) image.Dispose();
                    }
                }
            }
            finally
            {
                if (File.Exists(SourceFileName)) File.Delete(SourceFileName);
                if (File.Exists(DestFileName))
                {
                    if (cachePath != null) File.Move(DestFileName, cachePath);
                    else File.Delete(DestFileName);
                }
                if (File.Exists(DecodedFileName)) File.Delete(DecodedFileName);
            }
        }
        
        private static bool EncodeToFile(string path, string format, int mipmapCount, bool encodeOutput, bool decodeImage)
        {
            if (!Environment.Is64BitOperatingSystem) return false;

            using (var process = new Process())
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = "PVRTexToolCLI.exe";

                var args = new StringBuilder();

                args.Append(" -f ");
                args.Append(format);
                
                if (mipmapCount > 1)
                {
                    args.Append(" -m ");
                    args.Append(mipmapCount);
                }

                if (format.Contains("PVRTC")) args.Append(" -q pvrtcbest");
                else if (format.Contains("ETC")) args.Append(" -q etcfast");

                args.Append(" -l ");
                args.Append(" -i ");
                args.Append(path);
                if (decodeImage)
                {
                    args.Append(" -d ");
                    args.Append(DecodedFileName);
                }
                if (encodeOutput)
                {
                    args.Append(" -o ");
                    args.Append(DestFileName);
                }
                process.StartInfo.Arguments = args.ToString();

                Console.WriteLine(process.StartInfo.FileName + " " + process.StartInfo.Arguments);

                process.Start();
                var reader = process.StandardOutput;
                var output = reader.ReadToEnd();
                //Console.WriteLine(output);
                process.WaitForExit();
            }

            return File.Exists(DestFileName) || File.Exists(DecodedFileName);
        }
    }
}
