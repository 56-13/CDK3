using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Support;
using CDK.Assets.Updaters;

using Rectangle = CDK.Drawing.Rectangle;
using GDISize = System.Drawing.Size;
using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    public enum ImageAssetEncoding
    {
        Rgba8,
        Rgb8,
        Srgb8A8,
        Srgb8,
        Rgba4,
        Rgb5,
        R8,
        CompressedRgba,
        CompressedRgb,
        CompressedSrgbA,
        CompressedSrgb
    }

    public class RootImageAsset : ImageAsset
    {
        private TextureSlot _Content;
        public override TextureSlot Content
        {
            get
            {
                Load();
                return _Content;
            }
        }

        public ImageAssetEncoding _Encoding;
        public ImageAssetEncoding Encoding
        {
            set
            {
                Load();
                if (SetProperty(ref _Encoding, value))
                {
                    if (AssetManager.Instance.CommandEnabled) _Content.Formats = GetFormats();
                }
            }
            get
            {
                Load();
                return _Encoding;
            }
        }

        public RawFormat[] GetFormats()
        {
            switch (_Encoding)
            {
                case ImageAssetEncoding.Rgba8:
                    return new RawFormat[] { RawFormat.Rgba8 };
                case ImageAssetEncoding.Rgb8:
                    return new RawFormat[] { RawFormat.Rgb8 };
                case ImageAssetEncoding.Srgb8A8:
                    return new RawFormat[] { RawFormat.Srgb8Alpha8 };
                case ImageAssetEncoding.Srgb8:
                    return new RawFormat[] { RawFormat.Srgb8 };
                case ImageAssetEncoding.Rgba4:
                    return new RawFormat[] { RawFormat.Rgba4 };
                case ImageAssetEncoding.Rgb5:
                    return new RawFormat[] { RawFormat.Rgb5 };
                case ImageAssetEncoding.R8:
                    return new RawFormat[] { RawFormat.R8 };
                case ImageAssetEncoding.CompressedRgba:
                    return AssetManager.Instance.Config.Texture.CompressedRgba;
                case ImageAssetEncoding.CompressedRgb:
                    return AssetManager.Instance.Config.Texture.CompressedRgb;
                case ImageAssetEncoding.CompressedSrgbA:
                    return AssetManager.Instance.Config.Texture.CompressedSrgbA;
                case ImageAssetEncoding.CompressedSrgb:
                    return AssetManager.Instance.Config.Texture.CompressedSrgb;
            }
            throw new InvalidOperationException();
        }

        public bool UsingTransparency
        {
            get
            {
                switch (_Encoding)
                {
                    case ImageAssetEncoding.Rgba8:
                    case ImageAssetEncoding.Srgb8A8:
                    case ImageAssetEncoding.Rgba4:
                    case ImageAssetEncoding.CompressedRgba:
                    case ImageAssetEncoding.CompressedSrgbA:
                        return true;
                }
                return false;
            }
        }

        private TextureSlotDescription _TextureDescription;
        public TextureSlotDescription TextureDescription
        {
            get
            {
                Load();
                return _TextureDescription;
            }
        }

        public override GDIRectangle Frame
        {
            get
            {
                Load();

                var image = _Content.Bitmap;

                return new GDIRectangle(0, 0, image != null ? image.Width : 0, image != null ? image.Height : 0);
            }
        }

        public override Vector2 Pivot => Vector2.Zero;

        public override Rectangle UV => Rectangle.ZeroToOne;

        private float _ContentScale;
        public override float ContentScale {
            set
            {
                Load();
                if (SetProperty(ref _ContentScale, value))
                {
                    foreach (var sub in Children) sub.OnPropertyChanged("ContentScale");
                }
            }
            get
            {
                Load();
                return _ContentScale;
            }
        }

        private ScreenColor _ScreenColor;
        public override ScreenColor ScreenColor => _ScreenColor;

        private bool _HasSubImages;
        public bool HasSubImages
        {
            set
            {
                Load();
                SetSharedProperty(ref _HasSubImages, value);
            }
            get
            {
                Load();
                return _HasSubImages;
            }
        }

        private bool _holdUpdateSubImages;

        internal void UpdateSubImages()
        {
            if (!_holdUpdateSubImages) OnPropertyChanged("SubImages");
        }

        internal void HoldUpdateSubImages()
        {
            _holdUpdateSubImages = true;
        }

        internal void ReleaseUpdateSubImages()
        {
            _holdUpdateSubImages = false;
            OnPropertyChanged("SubImages");
        }

        public SubImageAsset[] SubImages
        {
            get
            {
                var subImages = new SubImageAsset[Children.Count];
                for (var i = 0; i < subImages.Length; i++) subImages[i] = (SubImageAsset)Children[i];
                return subImages;
            }
        }

        public RootImageAsset()
        {
            using (new AssetCommandHolder())
            {
                _TextureDescription = new TextureSlotDescription(this, TextureTarget.Texture2D)
                {
                    WrapS = TextureWrapMode.ClampToEdge,
                    WrapR = TextureWrapMode.ClampToEdge,
                    WrapT = TextureWrapMode.ClampToEdge
                };
            }

            _Content = new TextureSlot(_TextureDescription, "content");
            _Content.PropertyChanged += Content_PropertyChanged;

            _ContentScale = 1;

            _ScreenColor = new ScreenColor();
        }

        public RootImageAsset(RootImageAsset other, bool content)
            : base(other, content)
        {
            other.Load();

            _HasSubImages = other._HasSubImages;

            if (content)
            {
                _TextureDescription = new TextureSlotDescription(this, other._TextureDescription);
                _Content = new TextureSlot(_TextureDescription, other._Content);
                _ContentScale = other._ContentScale;
                _Encoding = other._Encoding;
            }
            else
            {
                using (new AssetCommandHolder())
                {
                    _TextureDescription = new TextureSlotDescription(this, TextureTarget.Texture2D)
                    {
                        WrapS = TextureWrapMode.ClampToEdge,
                        WrapR = TextureWrapMode.ClampToEdge,
                        WrapT = TextureWrapMode.ClampToEdge
                    };
                }
                _Content = new TextureSlot(_TextureDescription, "content");
                _ContentScale = 1;
            }
            _Content.PropertyChanged += Content_PropertyChanged;

            _ScreenColor = new ScreenColor();
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Bitmap") OnPropertyChanged("Frame");
        }

        private struct PackingOrigin
        {
            public int X;
            public int Y;
            public bool Bottom;

            public PackingOrigin(int x, int y, bool bottom) : this()
            {
                X = x;
                Y = y;
                Bottom = bottom;
            }
        };

        private class PackingEntry
        {
            public SubImageElement Element;
            public GDIRectangle Frame;
            public Point Border;

            public int BorderWidth => Frame.Width + Border.X * 2;
            public int BorderHeight => Frame.Height + Border.Y * 2;
            public GDIRectangle BorderFrame => new GDIRectangle(Frame.X - Border.X, Frame.Y - Border.Y, Frame.Width + 2 * Border.X, Frame.Height + 2 * Border.Y);

            public PackingEntry(SubImageElement element)
            {
                Element = element;
                
                Frame = element.Frame;

                Border.X = element.Parent.BorderX ? 1 : 0;
                Border.Y = element.Parent.BorderY ? 1 : 0;
            }
        };

        private struct PackingHeuristic
        {
            private int _distance;
            private int _imageSize;
            private int _textureSize;
            private bool _overflow;

            public PackingHeuristic(TextureSlot texture, int x, int y, int w, int h)
                : this()
            {
                _distance = x * x + y * y;
                _imageSize = w * h;
                var ts = new GDISize(w, h);
                Texture.GetEncodedSize(ref ts, texture.Formats);
                _textureSize = ts.Width * ts.Height;
                _overflow = ts.Width > 2048 || ts.Height > 2048;
            }

            public bool Compare(PackingHeuristic other)
            {
                if (_overflow != other._overflow)
                {
                    return other._overflow;
                }
                if (_textureSize != other._textureSize)
                {
                    return _textureSize < other._textureSize;
                }
                if (_imageSize != other._imageSize)
                {
                    return _imageSize < other._imageSize;
                }
                if (_distance != other._distance)
                {
                    return _distance < other._distance;
                }
                return false;
            }
        }

        public float Pack()
        {
            AssetManager.Instance.Purge();

            Load();

            var hclamp = _TextureDescription.WrapS == TextureWrapMode.ClampToEdge || _TextureDescription.WrapS == TextureWrapMode.ClampToBorder;
            var vclamp = _TextureDescription.WrapT == TextureWrapMode.ClampToEdge || _TextureDescription.WrapT == TextureWrapMode.ClampToBorder;

            if (!hclamp && !vclamp) return 1;

            var image = _Content.Bitmap;

            if (image == null) return 1;

            var images = SubImages;

            if (images.Length == 0) return 1;

            var entries = new List<PackingEntry>();
            for (int i = 0; i < images.Length; i++)
            {
                foreach (var element in images[i].Elements)
                {
                    var entry = new PackingEntry(element);

                    entries.Add(entry);
                }
            }
            entries.Sort(new Comparison<PackingEntry>(delegate (PackingEntry a, PackingEntry b)
            {
                return Math.Max(b.BorderWidth, b.BorderHeight) - Math.Max(a.BorderWidth, a.BorderHeight);
            }));


            AssetManager.Instance.BeginProgress(entries.Count);

            entries[0].Frame.X = entries[0].Border.X;
            entries[0].Frame.Y = entries[0].Border.Y;

            var width = entries[0].BorderWidth;
            var height = entries[0].BorderHeight;

            var origins = new List<PackingOrigin>();
            if (hclamp) origins.Add(new PackingOrigin(width, 0, false));
            if (vclamp) origins.Add(new PackingOrigin(0, height, true));

            var heuristic = new PackingHeuristic();

            for (var i = 1; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (!AssetManager.Instance.Progress(entry.Element.Parent.Location, 1))
                {
                    AssetManager.Instance.EndProgress();

                    return 1;
                }

                var nextWidth = width;
                var nextHeight = height;
                var selection = -1;

                for (var j = 0; j < origins.Count; j++)
                {
                    var origin = origins[j];

                    var x = origin.X + entry.Border.X;
                    var y = origin.Y + entry.Border.Y;
                    var d = 0;

                    if (origin.Bottom)
                    {
                        if (origin.Y > 0) y += SubImageAsset.Padding;

                        for (; ; )
                        {
                            var flag = true;
                            for (var k = 0; k < i; k++)
                            {
                                var px = entry.Border.X + SubImageAsset.Padding;
                                var py = entry.Border.Y + SubImageAsset.Padding;

                                if (x < d + entry.Border.X || entries[k].BorderFrame.IntersectsWith(
                                    new GDIRectangle(
                                        x - d - px,
                                        y - py,
                                        entry.Frame.Width + 2 * px,
                                        entry.Frame.Height + 2 * py)))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag) d++;
                            else break;
                        }
                        if (d != 0) x = x - d + 1;
                        else continue;
                    }
                    else
                    {
                        if (origin.X > 0) x += SubImageAsset.Padding;

                        for (; ; )
                        {
                            var flag = true;
                            for (var k = 0; k < i; k++)
                            {
                                var px = entry.Border.X + SubImageAsset.Padding;
                                var py = entry.Border.Y + SubImageAsset.Padding;

                                if (y < d + entry.Border.Y || entries[k].BorderFrame.IntersectsWith(
                                    new GDIRectangle(
                                        x - px,
                                        y - d - py,
                                        entry.Frame.Width + 2 * px,
                                        entry.Frame.Height + 2 * py)))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag) d++;
                            else break;
                        }
                        if (d != 0) y = y - d + 1;
                        else continue;
                    }
                    var w = Math.Max(x + entry.Frame.Width + entry.Border.X, width);
                    var h = Math.Max(y + entry.Frame.Height + entry.Border.Y, height);

                    var currentHeuristic = new PackingHeuristic(_Content, x, y, w, h);

                    if (selection == -1 || currentHeuristic.Compare(heuristic))
                    {
                        nextWidth = w;
                        nextHeight = h;
                        heuristic = currentHeuristic;
                        entry.Frame.X = x;
                        entry.Frame.Y = y;

                        selection = j;
                    }
                }
                width = nextWidth;
                height = nextHeight;

                origins.RemoveAt(selection);

                var frame = entry.BorderFrame;
                if (hclamp) origins.Add(new PackingOrigin(frame.Right, frame.Top, false));
                if (vclamp) origins.Add(new PackingOrigin(frame.Left, frame.Bottom, true));
            }

            var oldWidth = image.Width;
            var oldHeight = image.Height;

            var scratch = new Bitmap(width, height);

            using (var scratchTarget = new BitmapScratch(scratch))
            {
                foreach (var entry in entries)
                {
                    scratchTarget.Copy(image, entry.Frame.X, entry.Frame.Y, entry.Border.X, entry.Border.Y, entry.Element.Frame);
                }
            }

            _Content.Bitmap = scratch;

            foreach (var entry in entries)
            {
                entry.Element.Frame = entry.Frame;
            }

            AssetManager.Instance.EndProgress();

            return (float)(width * height) / (oldWidth * oldHeight);
        }

        private bool IsCorrupted(SubImageElement element, int x, int y)
        {
            foreach (var sub in SubImages)
            {
                if (element == null || sub != element.Parent)
                {
                    foreach (var sube in sub.Elements)
                    {
                        if (sube.Frame.Contains(new Point(x, y))) return true;
                    }
                }
            }
            return false;
        }

        private bool CropImpl(Bitmap image, ref GDIRectangle frame, ref Vector2 pivot)
        {
            var transparency = UsingTransparency;

            var sx = frame.Left;
            var sy = frame.Top;
            var dx = frame.Right - 1;
            var dy = frame.Bottom - 1;

            var changed = false;

            while (sx < dx)
            {
                var flag = true;

                for (var y = sy; y <= dy; y++)
                {
                    var c = image.GetPixel(sx, y);

                    if (c.A != 0 && (transparency || c.ToArgb() != -16777216))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    sx++;
                    changed = true;
                }
                else break;
            }
            while (sx < dx)
            {
                var flag = true;

                for (var y = sy; y <= dy; y++)
                {
                    var c = image.GetPixel(dx, y);

                    if (c.A != 0 && (transparency || c.ToArgb() != -16777216))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    dx--;
                    changed = true;
                }
                else break;
            }
            while (sy < dy)
            {
                var flag = true;

                for (var x = sx; x <= dx; x++)
                {
                    var c = image.GetPixel(x, sy);

                    if (c.A != 0 && (transparency || c.ToArgb() != -16777216))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    sy++;
                    changed = true;
                }
                else break;
            }
            while (sy < dy)
            {
                var flag = true;

                for (var x = sx; x <= dx; x++)
                {
                    var c = image.GetPixel(x, dy);

                    if (c.A != 0 && (transparency || c.ToArgb() != -16777216))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    dy--;
                    changed = true;
                }
                else break;
            }
            if (changed)
            {
                pivot = new Vector2(
                    pivot.X + ((frame.Right - 1 - dx) - (sx - frame.X)) * 0.5f,
                    pivot.Y + ((frame.Bottom - 1 - dy) - (sy - frame.Y)) * 0.5f);

                frame = new GDIRectangle(sx, sy, dx - sx + 1, dy - sy + 1);
            }
            return changed;
        }

        public bool Crop(Bitmap image, out GDIRectangle frame, out Vector2 pivot)
        {
            frame = new GDIRectangle(0, 0, image.Width, image.Height);
            pivot = Vector2.Zero;
            return CropImpl(image, ref frame, ref pivot);
        }

        public bool Crop(SubImageElement element, out GDIRectangle frame, out Vector2 pivot)
        {
            var image = _Content.Bitmap;
            frame = element.Frame;
            pivot = element.Pivot;
            if (image == null) return false;
            return CropImpl(image, ref frame, ref pivot);
        }

        public bool AutoSelect(SubImageElement element, int x, int y, bool merge, out GDIRectangle frame, out Vector2 pivot)
        {
            var image = _Content.Bitmap;

            frame = GDIRectangle.Empty;
            pivot = Vector2.Zero;

            if (image != null && x >= 0 && y >= 0 && x < image.Width && y < image.Height && image.GetPixel(x, y).A != 0)
            {
                var width = image.Width;
                var height = image.Height;

                if (element != null && element.Frame.Contains(new Point(x, y)))
                {
                    frame = element.Frame;
                    pivot = element.Pivot;
                    return CropImpl(image, ref frame, ref pivot);
                }
                else if (!IsCorrupted(element, x, y))
                {
                    var sx = Math.Max(x - 1, 0);
                    var sy = Math.Max(y - 1, 0);
                    var dx = Math.Min(x + 1, width - 1);
                    var dy = Math.Min(y + 1, height - 1);

                    for (; ; )
                    {
                        var sxFlag = true;
                        var syFlag = true;
                        var dxFlag = true;
                        var dyFlag = true;

                        for (x = sx; x <= dx; x++)
                        {
                            var c = image.GetPixel(x, sy);

                            if (c.A != 0 && !IsCorrupted(element, x, sy))
                            {
                                syFlag = false;
                                break;
                            }
                        }
                        for (x = sx; x <= dx; x++)
                        {
                            var c = image.GetPixel(x, dy);

                            if (c.A != 0 && !IsCorrupted(element, x, dy))
                            {
                                dyFlag = false;
                                break;
                            }
                        }
                        for (y = sy; y <= dy; y++)
                        {
                            var c = image.GetPixel(sx, y);

                            if (c.A != 0 && !IsCorrupted(element, sx, y))
                            {
                                sxFlag = false;
                                break;
                            }
                        }
                        for (y = sy; y <= dy; y++)
                        {
                            var c = image.GetPixel(dx, y);

                            if (c.A != 0 && !IsCorrupted(element, dx, y))
                            {
                                dxFlag = false;
                                break;
                            }
                        }
                        if (!sxFlag && sx > 0)
                        {
                            sx--;
                            continue;
                        }
                        if (!syFlag && sy > 0)
                        {
                            sy--;
                            continue;
                        }
                        if (!dxFlag && dx < width - 1)
                        {
                            dx++;
                            continue;
                        }
                        if (!dyFlag && dy < height - 1)
                        {
                            dy++;
                            continue;
                        }
                        if (sxFlag) sx++;
                        if (syFlag) sy++;
                        if (dxFlag) dx--;
                        if (dyFlag) dy--;

                        frame = new GDIRectangle(sx, sy, dx - sx + 1, dy - sy + 1);

                        if (element != null)
                        {
                            pivot = element.Pivot;

                            if (merge)
                            {
                                x = Math.Max(element.Frame.Right, frame.Right);
                                y = Math.Max(element.Frame.Bottom, frame.Bottom);

                                frame.X = Math.Min(frame.X, element.Frame.X);
                                frame.Y = Math.Min(frame.Y, element.Frame.Y);
                                frame.Width = x - frame.X;
                                frame.Height = y - frame.Y;

                                for (y = frame.Top; y < frame.Bottom; y++)
                                {
                                    for (x = frame.Left; x < frame.Right; x++)
                                    {
                                        if (IsCorrupted(element, x, y)) return false;
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        public override AssetType Type => AssetType.RootImage;
        public override Asset Clone(bool content) => new RootImageAsset(this, content);
        public override bool IsListed => true;
        protected override bool AddChildTypeEnabled(AssetType type) => type == AssetType.SubImage;
        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (RootImageAsset)asset;

            other.Load();

            if (_HasSubImages != other._HasSubImages)
            {
                return false;
            }
            return true;
        }

        public override void Import(string path)
        {
            AssetManager.Instance.Purge();

            Load();

            _Content.Bitmap = Drawing.BitmapTexture.Load(path);
        }

        public override void Export(string dirpath)
        {
            Load();

            var image = _Content.Bitmap;

            if (image != null)
            {
                dirpath = Path.Combine(dirpath, $"{Name}.png");

                image.Save(dirpath);
            }
        }

        private void BuildImpl(BinaryWriter writer, AssetBuildPlatform platform)
        {
            _Content.Build(writer, platform);

            if (_HasSubImages)
            {
                writer.WriteLength(Children.Count);
                foreach (SubImageAsset sub in Children)
                {
                    sub.Build(writer);
                }
            }
            else if (Children.Count != 0)
            {
                throw new AssetException(this, "빌드 설정에 서브이미지가 포함되어 있지 않습니다.");
            }
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        BuildImpl(writer, platform);
                    }
                }
            }
            else BuildImpl(writer, platform);
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("rootImageAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("encoding", _Encoding);
                _TextureDescription.Save(writer, "format");
                writer.WriteAttribute("hasSubImages", _HasSubImages);
                writer.WriteAttribute("contentScale", _ContentScale, 1);

                foreach (SubImageAsset sub in Children) sub.Save(writer);

                writer.WriteEndElement();
            }

            _Content.Save();

            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                AssetManager.Instance.Purge();

                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "rootImageAsset") throw new XmlException();

                Updater.ValidateRootImageAsset(node);

                Encoding = node.ReadAttributeEnum<ImageAssetEncoding>("encoding");
                _TextureDescription.Load(node, "format");
                HasSubImages = node.ReadAttributeBool("hasSubImages");
                ContentScale = node.ReadAttributeFloat("contentScale", 1);

                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    var subnode = node.ChildNodes[i];

                    var key = subnode.ReadAttributeString("key");

                    for (var j = 0; j < Children.Count; j++)
                    {
                        var subImage = (SubImageAsset)Children[(i + j) % Children.Count];

                        if (subImage.Key.Equals(key))
                        {
                            subImage.Load(subnode);
                            break;
                        }
                    }
                }

                _Content.Load();
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
            _Content.GetDataPath(paths);
        }
    }
}
