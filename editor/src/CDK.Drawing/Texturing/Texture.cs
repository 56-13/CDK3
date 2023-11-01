using System;

namespace CDK.Drawing
{
    public partial class Texture : GraphicsResource
    {
        private TextureDescription _Description;
        public TextureDescription Description => _Description;
        public TextureTarget Target => _Description.Target;
        public RawFormat Format => _Description.Format;
        public int Object { private set; get; }
        public int Width => _Description.Width;
        public int Height => _Description.Height;
        public int Depth => _Description.Depth;
        public int Samples => _Description.Samples;
        public bool Allocated { private set; get; }

        ~Texture()
        {
            if (GraphicsContext.IsCreated)
            {
                GraphicsContext.Instance.Invoke(false, (GraphicsApi api) =>
                {
                    if (Object != 0) api.DeleteTexture(Object);
                });
            }
        }

        public override void Dispose()
        {
            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) =>
            {
                if (Object != 0) api.DeleteTexture(Object);
            });

            GC.SuppressFinalize(this);
        }

        private int GetCost2D()
        {
            var encoding = Format.GetEncoding();

            if (encoding.Compressed)
            {
                var width = Width;
                var height = Height;
                var cost = 0;
                for (var i = 0; i < _Description.MipmapCount; i++)
                {
                    var blockWidth = (width + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    var blockHeight = (height + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    cost += blockWidth * blockHeight * encoding.CompressBlockLength;

                    width = Math.Max(1, width >> 1);
                    height = Math.Max(1, height >> 1);
                }
                return cost;
            }
            else
            {
                var current = Width * Height * encoding.PixelBpp;
                var cost = current;
                for (var i = 1; i < _Description.MipmapCount; i++)
                {
                    current >>= 2;
                    cost += current;
                }
                return cost;
            }
        }

        private int GetCost3D()
        {
            var encoding = Format.GetEncoding();

            if (encoding.Compressed)
            {
                var width = Width;
                var height = Height;
                var depth = Depth;
                var cost = 0;
                for (var i = 0; i < _Description.MipmapCount; i++)
                {
                    var blockWidth = (width + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    var blockHeight = (height + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    var blockDepth = (depth + encoding.CompressBlock - 1) / encoding.CompressBlock;
                    cost += blockWidth * blockHeight * blockDepth * encoding.CompressBlockLength;

                    width = Math.Max(1, width >> 1);
                    height = Math.Max(1, height >> 1);
                    depth = Math.Max(1, depth >> 1);
                }
                return cost;
            }
            else
            {
                var current = Width * Height * Depth * encoding.PixelBpp;
                var cost = current;
                for (var i = 1; i < _Description.MipmapCount; i++)
                {
                    current >>= 3;
                    cost += current;
                }
                return cost;
            }
        }

        public override int Cost
        {
            get
            {
                if (Allocated)
                {
                    switch (Target)
                    {
                        case TextureTarget.Texture2D:
                            return GetCost2D();
                        case TextureTarget.Texture3D:
                            return GetCost3D();
                        case TextureTarget.TextureCubeMap:
                            return GetCost2D() * 6;
                        case TextureTarget.Texture2DMultisample:
                            return GetCost2D() * Samples;
                        default:
                            throw new NotSupportedException();
                    }
                }
                return 0;
            }
        }

    }
}
