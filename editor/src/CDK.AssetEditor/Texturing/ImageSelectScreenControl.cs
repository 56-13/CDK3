using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class ImageSelectScreenControl : Control
    {
        private ImageAsset _Image;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAsset Image
        {
            set
            {
                if (_Image != null)
                {
                    _Image.PropertyChanged -= Image_PropertyChanged;
                    _Image.Content.PropertyChanged -= Content_PropertyChanged;
                    _Image.ScreenColor.PropertyChanged -= ScreenColor_PropertyChanged;
                }
                _Image = value;
                if (_Image != null)
                {
                    _Image.PropertyChanged += Image_PropertyChanged;
                    _Image.Content.PropertyChanged += Content_PropertyChanged;
                    _Image.ScreenColor.PropertyChanged += ScreenColor_PropertyChanged;
                }
                Invalidate();

                ImageChanged?.Invoke(this, EventArgs.Empty);
            }
            get => _Image;
        }
        public event EventHandler ImageChanged;
        
        public ScreenRatio Ratio { private set; get; }

        public ImageSelectScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();

            Ratio = new ScreenRatio();
            Ratio.PropertyChanged += Ratio_PropertyChanged;
            Disposed += ImageSelectScreenControl_Disposed;
        }

        private void ImageSelectScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Image != null)
            {
                _Image.PropertyChanged -= Image_PropertyChanged;
                _Image.Content.PropertyChanged -= Content_PropertyChanged;
                _Image.ScreenColor.PropertyChanged -= ScreenColor_PropertyChanged;
            }
        }

        private void Ratio_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedItem":
                    Invalidate();
                    break;
            }
        }

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Frame":
                    Invalidate();
                    break;
            }
        }

        private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Bitmap":
                    Invalidate();
                    break;
            }
        }

        private void ScreenColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedItem":
                    Invalidate();
                    break;
            }
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_Image != null)
            {
                pe.Graphics.Clear(_Image.ScreenColor.Background);

                var bitmap = _Image.Content.Bitmap;

                if (bitmap != null)
                {
                    pe.Graphics.TranslateTransform(Width / 2, Height / 2);
                    pe.Graphics.ScaleTransform(Ratio, Ratio);
                    var frame = _Image.Frame;
                    pe.Graphics.DrawImage(bitmap, -frame.Width / 2, -frame.Height / 2, frame, GraphicsUnit.Pixel);
                }
            }
            base.OnPaint(pe);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;

            if (e.Delta < 0) Ratio.Down();
            else if (e.Delta > 0) Ratio.Up();

            base.OnMouseWheel(e);
        }
    }
}
