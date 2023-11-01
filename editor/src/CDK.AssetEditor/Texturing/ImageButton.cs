using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    partial class ImageButton : Button
    {
        private ImageAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        _Asset.PropertyChanged -= Asset_PropertyChanged;
                        _Asset.Content.PropertyChanged -= Content_PropertyChanged;
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        BackColor = _Asset.ScreenColor.Background;

                        _Asset.PropertyChanged += Asset_PropertyChanged;
                        _Asset.Content.PropertyChanged += Content_PropertyChanged;
                    }

                    _imageDirty = true;

                    Invalidate();

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }
        
        public event EventHandler AssetChanged;

        private Bitmap _image;

        private bool _imageDirty;

        public ImageButton()
        {
            InitializeComponent();

            Disposed += ImageAssetButton_Disposed;
        }

        private void ImageAssetButton_Disposed(object sender, EventArgs e)
        {
            _image?.Dispose();

            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
                _Asset.Content.PropertyChanged -= Content_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Frame":
                    _imageDirty = true;
                    Invalidate();
                    break;
            }
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Bitmap":
                    _imageDirty = true;
                    Invalidate();
                    break;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            _imageDirty = true;

            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (_imageDirty)
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }

                if (_Asset != null)
                {
                    var originImage = _Asset.Content.Bitmap;

                    if (originImage != null && Width > 12 && Height > 12)
                    {
                        _image = new Bitmap(Width - 12, Height - 12);

                        using (var g = Graphics.FromImage(_image))
                        {
                            g.DrawImage(originImage, new Rectangle(0, 0, _image.Width, _image.Height), _Asset.Frame, GraphicsUnit.Pixel);
                        }
                    }
                }
                _imageDirty = false;
            }

            if (_image != null)
            {
                pe.Graphics.DrawImage(_image, 6, 6);
            }
        }
    }
}
