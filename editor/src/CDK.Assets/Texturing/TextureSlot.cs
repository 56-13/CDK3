using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Texturing
{
    public class TextureSlot : AssetElement
    {
        public override AssetElement GetParent() => Description.Parent;

        private int _BitmapKey;
        internal int BitmapKey
        {
            set
            {
                var flag = _BitmapKey == 0 || value == 0;

                if (SetProperty(ref _BitmapKey, value))
                {
                    _AverageColor = null;

                    OnPropertyChanged("Bitmap");
                    OnPropertyChanged("Texture");

                    if (flag) OnPropertyChanged("Exists");
                }
            }
            get => _BitmapKey;
        }

        public bool Exists => _BitmapKey != 0;

        public Bitmap Bitmap
        {
            set
            {
                var prev = ResourceManager.Instance.GetBitmap(_BitmapKey, false);

                if (prev != value)
                {
                    BitmapKey = ResourceManager.Instance.AddBitmap(value);
                }
            }
            get => ResourceManager.Instance.GetBitmap(_BitmapKey);
        }

        private Color4? _AverageColor;
        public Color4 AverageColor
        {
            get
            {
                if (_AverageColor == null)
                {
                    var bitmap = Bitmap;
                    if (bitmap != null)
                    {
                        _AverageColor = BitmapTexture.Average(bitmap);
                    }
                }
                return _AverageColor != null ? _AverageColor.Value : Color4.Transparent;
            }
        }

        public Size BuildSize
        {
            get
            {
                var bitmap = Bitmap;
                if (bitmap != null)
                {
                    var size = bitmap.Size;
                    Texture.GetEncodedSize(ref size, _Formats);
                    return size;
                }
                else return Size.Empty;
            }
        }

        public Texture Texture => ResourceManager.Instance.GetTexture(Description.GetDescription(_Formats), _BitmapKey);

        private RawFormat[] _Formats;
        internal RawFormat[] Formats
        {
            set
            {
                if (SetProperty(ref _Formats, value)) OnPropertyChanged("Texture");
            }
            get => _Formats;
        }

        public TextureSlotDescription Description { private set; get; }

        public string Name { private set; get; }

        public TextureSlot(TextureSlotDescription desc, string name)
        {
            Description = desc;
            Name = name;

            _Formats = new RawFormat[] { RawFormat.Rgba8 };

            Description.PropertyChanged += Description_PropertyChanged;
        }

        public TextureSlot(TextureSlotDescription format, TextureSlot other)
        {
            Description = format;
            Name = other.Name;
            _BitmapKey = other._BitmapKey;
            _Formats = other._Formats;

            Description.PropertyChanged += Description_PropertyChanged;
        }

        private void Description_PropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged("Texture");
        
        internal void Load()
        {
            var path = $"{Owner.ContentPath}.{Name}";
            var bitmapPath = $"{path}.png";

            if (File.Exists(bitmapPath))
            {
                AssetManager.Instance.Purge();

                Bitmap = BitmapTexture.Load(bitmapPath);

                ResourceManager.Instance.LoadTextureCache(_BitmapKey, path);
            }
            else Bitmap = null;
        }

        internal void Save()
        {
            var bitmap = Bitmap;

            var path = $"{Owner.ContentPath}.{Name}";

            if (bitmap != null)
            {
                bitmap.Save($"{path}.png");

                ResourceManager.Instance.SaveTextureCache(_BitmapKey, _Formats, Description.MipmapCount, path);
            }
            else
            {
                File.Delete($"{path}.png");

                ResourceManager.Instance.ClearTextureCache(path);
            }
        }

        internal void Build(BinaryWriter writer, AssetBuildPlatform platform)
        {
            var bitmap = Bitmap;

            if (bitmap != null && _Formats.Length != 0)
            {
                var path = $"{Owner.ContentPath}.{Name}";

                if (bitmap.Width > 2048 || bitmap.Height > 2048)
                {
                    throw new AssetException(Owner, "파일의 사이즈가 2048를 초과했습니다.");
                }

                writer.Write(true);

                RawFormat[] formats;

                if (platform == AssetBuildPlatform.All)
                {
                    formats = _Formats;
                }
                else 
                { 
                    var temp = new List<RawFormat>();
                    foreach (var format in _Formats)
                    {
                        switch (format)
                        {
                            case RawFormat.CompressedRgbaS3tcDxt1Ext:
                            case RawFormat.CompressedRgbaS3tcDxt3Ext:
                            case RawFormat.CompressedRgbaS3tcDxt5Ext:
                            case RawFormat.CompressedSrgbAlphaS3tcDxt1Ext:
                            case RawFormat.CompressedSrgbAlphaS3tcDxt3Ext:
                            case RawFormat.CompressedSrgbAlphaS3tcDxt5Ext:
                                if (platform == AssetBuildPlatform.Windows) temp.Add(format);
                                break;
                            case RawFormat.CompressedRgb8Etc2:
                            case RawFormat.CompressedSrgb8Etc2:
                            case RawFormat.CompressedRgb8PunchthroughAlpha1Etc2:
                            case RawFormat.CompressedSrgb8PunchthroughAlpha1Etc2:
                            case RawFormat.CompressedRgba8Etc2Eac:
                            case RawFormat.CompressedSrgb8Alpha8Etc2Eac:
                                if (platform == AssetBuildPlatform.Android || platform == AssetBuildPlatform.iOS) temp.Add(format);
                                break;
                            default:
                                temp.Add(format);
                                break;
                        }
                    }
                    if (temp.Count == 0)
                    {
                        throw new AssetException(Owner, "이미지는 존재하지만 빌드 중인 플랫폼에 맞는 이미지가 없습니다.");
                    }
                    formats = temp.ToArray();
                }

                Texture.Build(writer, bitmap, formats, Description.GetDescription(), path);
            }
            else
            {
                writer.Write(false);
            }
        }

        internal void GetDataPath(List<string> paths)
        {
            Texture.GetCachePaths($"{Owner.ContentPath}.{Name}", _Formats, Description.MipmapCount, paths);
        }
    }
}
