using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;

using CDK.Assets.Support;

using Rectangle = CDK.Drawing.Rectangle;
using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    public class SubImageAsset : ImageAsset
    {
        private RootImageAsset _RootImage;
        public RootImageAsset RootImage
        {
            internal set
            {
                Load();
                if (SetProperty(ref _RootImage, value))
                {
                    OnPropertyChanged("ContentScale");
                }
            }
            get
            {
                Load();
                return _RootImage;
            }
        }

        public override TextureSlot Content => _RootImage?.Content;

        public override float ContentScale 
        {
            set => throw new NotImplementedException();
            get 
            {
                Load();
                return _RootImage?.ContentScale ?? 1f;
            }
        }

        private AssetElementList<SubImageElement> _Elements;
        public AssetElementList<SubImageElement> Elements
        {
            get
            {
                Load();
                return _Elements;
            }
        }

        public SubImageElement MainElement
        {
            get
            {
                Load();
                foreach (var e in _Elements)
                {
                    if (e.Locales == null) return e;
                }
                return null;
            }
        }

        private SubImageElement _SelectedElement;
        public SubImageElement SelectedElement
        {
            set
            {
                if (_SelectedElement != value)
                {
                    if (value != null && value.Parent != this) throw new InvalidOperationException();

                    _SelectedElement = value;
                    OnPropertyChanged("SelectedElement");
                    OnPropertyChanged("Frame");
                    OnPropertyChanged("Pivot");
                }
            }
            get => _SelectedElement ?? MainElement;
        }

        public override GDIRectangle Frame
        {
            get
            {
                Load();

                SubImageElement de = null;

                foreach (var e in _Elements)
                {
                    if (e.Locales != null)
                    {
                        if (Array.IndexOf(e.Locales, AssetManager.Instance.Locale) >= 0) return e.Frame;
                    }
                    else if (de == null) de = e;
                }

                return de?.Frame ?? GDIRectangle.Empty;
            }
        }

        public override Vector2 Pivot
        {
            get
            {
                Load();
                 
                SubImageElement de = null;

                foreach (var e in _Elements)
                {
                    if (e.Locales != null)
                    {
                        if (Array.IndexOf(e.Locales, AssetManager.Instance.Locale) >= 0) return e.Pivot;
                    }
                    else if (de == null) de = e;
                }

                return de?.Pivot ?? Vector2.Zero;
            }
        }

        public override Rectangle UV
        {
            get
            {
                if (_RootImage != null)
                {
                    var texture = _RootImage.Content.Texture;

                    if (texture != null)
                    {
                        var frame = Frame;

                        return new Drawing.Rectangle(
                            (float)frame.X / texture.Description.Width,
                            (float)frame.Y / texture.Description.Height,
                            (float)frame.Width / texture.Description.Width,
                            (float)frame.Height / texture.Description.Height);
                    }
                }
                return Rectangle.ZeroToOne;
            }
        }

        public override ScreenColor ScreenColor => _RootImage?.ScreenColor;

        private bool _BorderX;
        public bool BorderX
        {
            set
            {
                Load();
                SetProperty(ref _BorderX, value);
            }
            get
            {
                Load();
                return _BorderX;
            }
        }

        private bool _BorderY;
        public bool BorderY
        {
            set
            {
                Load();
                SetProperty(ref _BorderY, value);
            }
            get
            {
                Load();
                return _BorderY;
            }
        }

        public override bool IsDirty
        {
            set
            {
                if (value && _RootImage != null) _RootImage.IsDirty = true;
            }
            get => false;
        }

        public SubImageAsset()
        {
            _Elements = new AssetElementList<SubImageElement>(this)
            {
                new SubImageElement(this)
            };
            _Elements.ListChanged += Elements_ListChanged;

            PropertyChanged += SubImageAsset_PropertyChanged;
        }
        
        private SubImageAsset(SubImageAsset other, bool content)
            : base(other, content)
        {
            other.Load();

            _BorderX = other._BorderX;
            _BorderY = other._BorderY;

            _Elements = new AssetElementList<SubImageElement>(this);

            if (content)
            {
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    _RootImage = AssetManager.Instance.GetRedirection(other._RootImage);
                });

                foreach (var e in other._Elements)
                {
                    _Elements.Add(new SubImageElement(this, e));
                }
            }
            else
            {
                _Elements.Add(new SubImageElement(this));
            }
            _Elements.ListChanged += Elements_ListChanged;

            PropertyChanged += SubImageAsset_PropertyChanged;
        }

        private static HashSet<SubImageAsset> __moves = new HashSet<SubImageAsset>();
        private static bool __moveReserved = false;

        private static void Move(SubImageAsset image)
        {
            if (!__moves.Contains(image))
            {
                __moves.Add(image);
            }
            if (image.Parent != null) image.Parent.IsDirty = true;

            if (!__moveReserved)
            {
                __moveReserved = true;

                AssetManager.Instance.Invoke(() =>
                {
                    AssetManager.Instance.Purge();

                    var rootImages = new HashSet<RootImageAsset>();
                    var movSubImages = new Dictionary<RootImageAsset, List<SubImageAsset>>();

                    foreach (var sub in __moves)
                    {
                        var newParent = (RootImageAsset)sub.Parent;

                        if (newParent != sub.RootImage)
                        {
                            if (newParent != null)
                            {
                                if (sub.RootImage != null)
                                {
                                    if (!rootImages.Contains(sub.RootImage)) rootImages.Add(sub.RootImage);
                                    if (!rootImages.Contains(newParent)) rootImages.Add(newParent);
                                    if (sub.RootImage.Content.Bitmap != null)
                                    {
                                        if (!movSubImages.TryGetValue(newParent, out List<SubImageAsset> curmovSubImages))
                                        {
                                            curmovSubImages = new List<SubImageAsset>();
                                            movSubImages.Add(newParent, curmovSubImages);
                                        }
                                        curmovSubImages.Add(sub);
                                    }
                                    else
                                    {
                                        sub.RootImage = newParent;
                                    }
                                }
                                else
                                {
                                    sub.RootImage = newParent;
                                    if (!rootImages.Contains(newParent)) rootImages.Add(newParent);
                                }
                            }
                            else
                            {
                                if (sub.RootImage != null && !rootImages.Contains(sub.RootImage)) rootImages.Add(sub.RootImage);
                                sub.RootImage = null;
                            }
                        }
                        else
                        {
                            if (newParent != null && rootImages.Contains(newParent)) rootImages.Add(newParent);
                        }
                    }

                    foreach (var root in rootImages)
                    {
                        root.HoldUpdateSubImages();
                    }
                    foreach (var root in movSubImages.Keys)
                    {
                        var subs = movSubImages[root];

                        var w = 0;
                        var h = 0;
                        var x = 0;
                        var y = 0;

                        if (root.Content.Bitmap != null)
                        {
                            w = root.Content.Bitmap.Width;
                            h = root.Content.Bitmap.Height;
                            y = h + Padding;
                        }

                        var col = 0;
                        var colCount = (int)Math.Sqrt(subs.Count);
                        var rh = 0;

                        foreach (var sub in subs)
                        {
                            foreach (var sube in sub.Elements)
                            {
                                var frame = sube.Frame;
                                if (rh < frame.Height) rh = frame.Height;
                                x += frame.Width;
                                if (++col <= colCount)
                                {
                                    x += Padding;
                                }
                                else
                                {
                                    if (w < x) w = x;
                                    h += rh + Padding;
                                    x = 0;
                                    rh = 0;
                                    col = 0;
                                }
                            }
                        }
                        if (col > 0)
                        {
                            if (w < x) w = x;
                            h += rh + Padding;
                        }

                        x = 0;
                        rh = 0;
                        col = 0;

                        var scratch = new Bitmap(w, h);
                        using (var scratchTarget = new BitmapScratch(scratch))
                        {
                            if (root.Content.Bitmap != null) scratchTarget.Copy(root.Content.Bitmap, 0, 0);
                            foreach (var sub in subs)
                            {
                                foreach (var sube in sub.Elements)
                                {
                                    var frame = sube.Frame;
                                    if (rh < frame.Height) rh = frame.Height;
                                    scratchTarget.Copy(sub.Content.Bitmap, x, y, 0, 0, sube.Frame);

                                    frame.X = x;
                                    frame.Y = y;
                                    sube.Frame = frame;

                                    if (rh < frame.Height) rh = frame.Height;
                                    x += frame.Width;
                                    if (++col <= colCount)
                                    {
                                        x += Padding;
                                    }
                                    else
                                    {
                                        y += rh + Padding;
                                        x = 0;
                                        rh = 0;
                                        col = 0;
                                    }
                                }
                                sub.RootImage = root;
                            }
                        }
                        root.Content.Bitmap = scratch;
                    }

                    foreach (var root in rootImages) root.OnPropertyChanged("SubImages");
                    
                    __moves.Clear();
                    __moveReserved = false;
                });
            }
        }

        private void SubImageAsset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Parent")) Move(this);
        }

        private void Elements_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemChanged:
                    if (_Elements[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.Reset:
                    foreach (SubImageElement element in _Elements)
                    {
                        if (element.Parent != this) throw new InvalidOperationException();
                    }
                    break;
                case ListChangedType.ItemMoved:
                    return;
            }

            if (_SelectedElement != null && !_Elements.Contains(_SelectedElement))
            {
                SelectedElement = null;
            }
        }
        
        public override AssetType Type => AssetType.SubImage;
        public override Asset Clone(bool content) => new SubImageAsset(this, content);
        public override void Import(string path)
        {
            Load();

            path = Path.Combine(path, $"{Name}.png");

            if (File.Exists(path))
            {
                MainElement?.Import(path);
            }
        }

        public override void Export(string path)
        {
            Load();

            var image = Content.Bitmap;

            if (image != null)
            {
                foreach (var e in _Elements)
                {
                    if (e.Frame.Width > 0 && e.Frame.Height > 0)
                    {
                        string subpath = Path.Combine(path, Name);
                        if (e.Locales != null) subpath += "_" + e.Name;
                        subpath += ".png";

                        using (Bitmap subimage = image.Clone(e.Frame, image.PixelFormat))
                        {
                            subimage.Save(subpath);
                        }
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            if (_RootImage != null) _RootImage.Load();
        }

        internal override int BuildProgress => 0;
        internal void Build(BinaryWriter writer)
        {
            Load();

            SubImageElement de = null;

            for (var i = 0; i < _Elements.Count; i++)
            {
                var e = _Elements[i];
                for (var j = i + 1; j < _Elements.Count; j++)
                {
                    var ne = _Elements[j];

                    bool collision;

                    if (e.Locales == null)
                    {
                        collision = ne.Locales == null;
                    }
                    else
                    {
                        collision = false;

                        foreach (var locale in e.Locales)
                        {
                            if (Array.IndexOf(ne.Locales, locale) >= 0)
                            {
                                collision = true;
                                break;
                            }
                        }
                    }
                    if (collision)
                    {
                        throw new AssetException(this, "중복된 서브이미지 요소가 있습니다.");
                    }
                }
                if (e.Locales == null) de = e;
            }
            if (de == null)
            {
                throw new AssetException(this, "서브이미지 기본 요소가 없습니다.");
            }
            writer.Write((short)de.Frame.X);
            writer.Write((short)de.Frame.Y);
            writer.Write((short)de.Frame.Width);
            writer.Write((short)de.Frame.Height);

            writer.WriteLength(_Elements.Count - 1);
            foreach (var e in _Elements)
            {
                if (e != de)
                {
                    writer.WriteLength(e.Locales.Length);
                    foreach (string locale in e.Locales)
                    {
                        writer.WriteString(locale);
                    }
                    writer.Write((short)e.Frame.X);
                    writer.Write((short)e.Frame.Y);
                    writer.Write((short)e.Frame.Width);
                    writer.Write((short)e.Frame.Height);
                }
            }
        }
        
        internal void Save(XmlWriter writer)
        {
            Load();

            writer.WriteStartElement("subImageAsset");
            writer.WriteAttribute("key", Key);
            writer.WriteAttribute("borderX", _BorderX);
            writer.WriteAttribute("borderY", _BorderY);

            foreach (var e in _Elements)
            {
                e.Save(writer);
            }
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            BorderX = node.ReadAttributeBool("borderX");
            BorderY = node.ReadAttributeBool("borderY");

            _Elements.Clear();
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName == "subImageElement")
                {
                    _Elements.Add(new SubImageElement(this, subnode));
                }
            }
        }

        public const int Padding = 1;
    }
}
