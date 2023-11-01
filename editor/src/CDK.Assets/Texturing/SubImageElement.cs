using System;
using System.Numerics;
using System.Text;
using System.Drawing;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Support;

using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    public class SubImageElement : AssetElement
    {
        public SubImageAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public string Name
        {
            get
            {
                if (_Locales != null)
                {
                    var strbuf = new StringBuilder();
                    var first = true;
                    foreach (var locale in _Locales)
                    {
                        if (first) first = false;
                        else strbuf.Append(',');
                        strbuf.Append(locale);
                    }
                    return strbuf.ToString();
                }
                else
                {
                    return "(default)";
                }
            }
        }

        private string[] _Locales;
        public string[] Locales
        {
            set
            {
                if (SetProperty(ref _Locales, value)) OnPropertyChanged("Name");
            }
            get => _Locales;
        }

        private GDIRectangle _Frame;
        public GDIRectangle Frame
        {
            set
            {
                if (SetProperty(ref _Frame, value)) Parent.OnPropertyChanged("Frame");
            }
            get => _Frame;
        }

        private Vector2 _Pivot;
        public Vector2 Pivot
        {
            set
            {
                if (SetProperty(ref _Pivot, value)) Parent.OnPropertyChanged("Pivot");
            }
            get => _Pivot;
        }

        public SubImageElement(SubImageAsset parent)
        {
            Parent = parent;
        }

        public SubImageElement(SubImageAsset parent, SubImageElement element)
        {
            Parent = parent;

            _Locales = element._Locales;
            _Frame = element._Frame;
            _Pivot = element._Pivot;

            AssetManager.Instance.AddRedirection(element, this);
        }

        internal SubImageElement(SubImageAsset parent, XmlNode node)
        {
            Parent = parent;

            if (node.HasAttribute("locales")) _Locales = node.ReadAttributeStrings("locales");

            _Frame = node.ReadAttributeGDIRectangle("frame");
            _Pivot = node.ReadAttributeVector2("pivot");
        }

        public void Import(string path)
        {
            if (Parent.RootImage != null)
            {
                AssetManager.Instance.Purge();

                var prevImg = Parent.RootImage.Content.Bitmap;

                var image = BitmapTexture.Load(path);

                if (prevImg != null)
                {
                    Bitmap nextImg;

                    if (image.Width <= _Frame.Width && image.Height <= _Frame.Height)
                    {
                        nextImg = new Bitmap(prevImg);

                        using (var s = new BitmapScratch(nextImg))
                        {
                            s.Clear(_Frame);
                            s.Copy(image, _Frame.X, _Frame.Y);
                        }
                        Frame = new GDIRectangle(_Frame.X, _Frame.Y, image.Width, image.Height);
                    }
                    else
                    {
                        nextImg = new Bitmap(Math.Max(prevImg.Width, image.Height), prevImg.Height + image.Height + SubImageAsset.Padding);

                        using (var s = new BitmapScratch(nextImg))
                        {
                            s.Copy(prevImg, 0, 0);
                            s.Clear(_Frame);
                            s.Copy(image, 0, prevImg.Height + SubImageAsset.Padding);
                        }
                        Frame = new GDIRectangle(0, prevImg.Height + SubImageAsset.Padding, image.Width, image.Height);
                    }

                    image.Dispose();

                    Parent.RootImage.Content.Bitmap = nextImg;
                }
                else
                {
                    Parent.RootImage.Content.Bitmap = image;

                    Frame = new GDIRectangle(0, 0, image.Width, image.Height);
                }
            }
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("subImageElement");
            if (_Locales != null) writer.WriteAttributes("locales", _Locales);
            writer.WriteAttribute("frame", _Frame);
            writer.WriteAttribute("pivot", _Pivot);
            writer.WriteEndElement();
        }
        
        public bool AutoSelect(int x, int y, bool merge)
        {
            if (Parent.RootImage != null)
            {
                if (Parent.RootImage.AutoSelect(this, x, y, merge, out var frame, out var pivot))
                {
                    Frame = frame;
                    Pivot = pivot;
                    return true;
                }
            }
            return false;
        }
    }
}
