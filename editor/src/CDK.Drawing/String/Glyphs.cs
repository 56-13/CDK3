using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;

using Icu;

namespace CDK.Drawing
{
    internal class Glyphs
    {
        private class Key : IEquatable<Key>
        {
            private string _character;
            private string _fontName;
            private FontStyle _fontStyle;
            private float _fontSize;

            public Key(string charater, Font font)
            {
                _character = charater;
                _fontName = font.Name;
                _fontStyle = font.Style;
                _fontSize = font.Size;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Combine(_character.GetHashCode());
                hash.Combine(_fontName.GetHashCode());
                hash.Combine(_fontStyle.GetHashCode());
                hash.Combine(_fontSize.GetHashCode());
                return hash;
            }

            public static bool operator ==(Key a, Key b) => a.Equals(b);
            public static bool operator !=(Key a, Key b) => !a.Equals(b);

            public bool Equals(Key other)
            {
                return _character == other._character &&
                    _fontName == other._fontName &&
                    _fontStyle == other._fontStyle &&
                    _fontSize == other._fontSize;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }
        }

        private struct Scratch
        {
            public List<Texture> Textures;
            public int X;
            public int Y;
            public int H;

            public void Clear()
            {
                foreach (var texture in Textures) texture.Dispose();
                Textures.Clear();
                X = 0;
                Y = 0;
                H = 0;
            }
        }

        private Dictionary<Key, (Texture texture, Rectangle frame)> _images;
        private Dictionary<string, Vector2> _sizes;
        private Scratch[] _scratch;
        private Bitmap _bitmap;
        private System.Drawing.Graphics _graphics;

        public const int GlyphsTextureSize = 2048;

        private Glyphs()
        {
            _images = new Dictionary<Key, (Texture, Rectangle)>(8192);
            _sizes = new Dictionary<string, Vector2>(8192);
            _scratch = new Scratch[2];
            _scratch[0].Textures = new List<Texture>();
            _scratch[1].Textures = new List<Texture>();

            _bitmap = new Bitmap(256, 256);
            _graphics = System.Drawing.Graphics.FromImage(_bitmap);

            Wrapper.Init();
        }

        private void Dispose()
        {
            Clear();
            _graphics.Dispose();
            _bitmap.Dispose();

            Wrapper.Cleanup();
        }

        public void Clear()
        {
            _images.Clear();
            _scratch[0].Clear();
            _scratch[1].Clear();
        }

        public Vector2 GetSize(string character, Font font)
        {
            if (_sizes.TryGetValue(character, out var size)) return size;

            var gdisize = _graphics.MeasureString(character, font);
            size = new Vector2(gdisize.Width, gdisize.Height);
            _sizes.Add(character, size);
            return size;
        }

        private void AddTexture(bool color)
        {
            var desc = new TextureDescription
            {
                Width = GlyphsTextureSize,
                Height = GlyphsTextureSize,
                Format = color ? RawFormat.Rgba8 : RawFormat.Alpha8,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                MinFilter = TextureMinFilter.Linear,
                MagFilter = TextureMagFilter.Linear
            };

            var texture = new Texture(desc, true);

            //var texture = new Texture(desc, false);           //TODO:zero fill?
            //var bpp = color ? 4 : 1;
            //var emptyRaw = new byte[GlyphsTextureSize * GlyphsTextureSize * bpp];
            //texture.Upload(emptyRaw, 0, GlyphsTextureSize, GlyphsTextureSize, 0);

            _scratch[color ? 1 : 0].Textures.Add(texture);
        }

        public bool GetImage(string character, Font font, out Texture texture, out Rectangle frame)
        {
            //TODO:CHECK GDI SHOULD BE MAIN THREAD?

            var ch = Char.ConvertToUtf32(character, 0);

            if (Character.IsSpace(ch) || Character.IsPunct(ch) || Character.IsControl(ch))
            {
                texture = null;
                frame = Rectangle.Zero;
                return false;
            }

            var key = new Key(character, font);

            if (_images.TryGetValue(key, out var e))
            {
                texture = e.texture;
                frame = e.frame;
                return true;
            }

            _graphics.Clear(Color.Transparent);
            _graphics.DrawString(character, font, Brushes.White, PointF.Empty);

            var size = _graphics.MeasureString(character, font);
            var w = (int)Math.Ceiling(size.Width);
            var h = (int)Math.Ceiling(size.Height);
            var bitmapData = _bitmap.LockBits(new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var color = false;
            byte[] raw;

            unsafe
            {
                var sptr = (byte*)bitmapData.Scan0;
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var p = &sptr[y * bitmapData.Stride + x * 4];
                        var a = p[0];
                        if (p[1] != a || p[2] != a || p[3] != a)
                        {
                            color = true;
                            break;
                        }
                    }
                }
                if (color)
                {
                    raw = new byte[w * h * 4];

                    fixed (byte* dptr = raw)
                    {
                        for (var y = 0; y < h; y++)
                        {
                            for (var x = 0; x < w; x++)
                            {
                                byte* s = &sptr[y * bitmapData.Stride + x * 4];
                                byte* d = &dptr[(y * w + x) * 4];
                                d[0] = s[2];
                                d[1] = s[1];
                                d[2] = s[0];
                                d[3] = s[3];
                            }
                        }
                    }
                }
                else
                {
                    raw = new byte[w * h];

                    fixed (byte* dptr = raw)
                    {
                        for (var y = 0; y < h; y++)
                        {
                            for (var x = 0; x < w; x++)
                            {
                                byte* s = &sptr[y * bitmapData.Stride + x * 4];
                                byte* d = &dptr[(y * w + x)];
                                *d = s[3];
                            }
                        }
                    }
                }
            }

            var t = _scratch[color ? 1 : 0];

            if (t.X + w > GlyphsTextureSize)
            {
                t.Y += t.H + 1;
                t.X = 0;
                t.H = 0;
            }
            if (t.Y + h > GlyphsTextureSize)
            {
                AddTexture(color);
                t.X = 0;
                t.Y = 0;
                t.H = 0;
            }
            else if (t.Textures.Count == 0)
            {
                AddTexture(color);
            }

            texture = t.Textures.Last();
            texture.UploadSub(raw, 0, t.X, t.Y, 0, w, h, 0);
            frame = new Rectangle(t.X, t.Y, w, h);

            _images.Add(key, (texture, frame));

            if (t.H < h) t.H = h;
            t.X += w + 1;

            return true;
        }

        public static Glyphs Instance { private set; get; }
        public static bool IsCreated => Instance != null;
        public static void CreateShared()
        {
            if (Instance == null) Instance = new Glyphs();
        }

        public static void DisposeShared()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}
