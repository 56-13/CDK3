using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    public partial class SubImageScreenControl : Control
    {
        private SubImageElement _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageElement Element
        {
            set
            {
                if (_Element != value)
                {
                    var prev = _Element?.Parent;

                    _Element = value;

                    var next = _Element?.Parent;

                    if (prev != next)
                    {
                        if (prev != null)
                        {
                            prev.PropertyChanged -= Asset_PropertyChanged;
                            prev.Content.PropertyChanged -= Content_PropertyChanged;
                            prev.ScreenColor.PropertyChanged -= Content_PropertyChanged;
                        }
                        if (next != null)
                        {
                            next.PropertyChanged += Asset_PropertyChanged;
                            next.Content.PropertyChanged += Content_PropertyChanged;
                            next.ScreenColor.PropertyChanged += Content_PropertyChanged;
                        }
                    }

                    Invalidate();

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public SubImageScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();

            Disposed += SubImageScreenControl_Disposed;
        }

        private void SubImageScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.Parent.PropertyChanged -= Asset_PropertyChanged;
                _Element.Parent.Content.PropertyChanged -= Content_PropertyChanged;
                _Element.Parent.ScreenColor.PropertyChanged -= Content_PropertyChanged;
            }
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
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
            if (_Element != null)
            {
                pe.Graphics.Clear(_Element.Parent.ScreenColor.Background);

                var image = _Element.Parent.Content.Bitmap;

                var frame = _Element.Frame;

                if (image != null && frame.Width > 0 && frame.Height > 0)
                {
                    var scale = Math.Min((float)Width / frame.Width, (float)Height / frame.Height);

                    var w = (int)(frame.Width * scale);
                    var h = (int)(frame.Height * scale);

                    var rect = new Rectangle((int)(Width - w) / 2, (int)(Height - h) / 2, w, h);

                    pe.Graphics.DrawImage(image, rect, frame, GraphicsUnit.Pixel);
                }
            }
            base.OnPaint(pe);
        }
    }
}
