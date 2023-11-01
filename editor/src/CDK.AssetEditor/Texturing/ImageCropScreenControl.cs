using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class ImageCropScreenControl : Control
    {
        private SubImageAsset _Image;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageAsset Image
        {
            set
            {
                if (_Image != value)
                {
                    if (_Image != null)
                    {
                        DataBindings.Clear();
                    }

                    _Image = value;

                    if (_Image != null)
                    {
                        DataBindings.Add("BackColor", _Image.ScreenColor, "Background", false, DataSourceUpdateMode.Never);
                        DataBindings.Add("ForeColor", _Image.ScreenColor, "Foreground", false, DataSourceUpdateMode.Never);
                    }

                    Invalidate();

                    ImageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Image;
        }
        public event EventHandler ImageChanged;

        public ScreenRatio Ratio { private set; get; }

        public ImageCropScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();

            Ratio = new ScreenRatio();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(0, 0, Width, Height));

            if (_Image != null)
            {
                pe.Graphics.SmoothingMode = SmoothingMode.None;
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;

                var center = new PointF(Width * 0.5f, Height * 0.5f);

                float radio = Ratio;

                using (var pen = new Pen(Color.FromArgb(64, ForeColor)))
                {
                    var bitmap = _Image.Content.Bitmap;

                    if (bitmap != null)
                    {
                        var scale = _Image.ContentScale * radio;
                        var frame = _Image.Frame;
                        var pivot = _Image.Pivot;

                        var rect = new RectangleF(
                            center.X - (frame.Width * 0.5f + pivot.X) * scale,
                            center.Y - (frame.Height * 0.5f + pivot.Y) * scale,
                            frame.Width * scale,
                            frame.Height * scale);

                        pe.Graphics.DrawImage(bitmap, rect, frame, GraphicsUnit.Pixel);

                        pe.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }

                    pe.Graphics.DrawLine(pen, 0, center.Y, Width, center.Y);
                    pe.Graphics.DrawLine(pen, center.X, 0, center.X, Height);
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_Image != null)
            {
                int x = 0, y = 0;
                switch (e.KeyCode)
                {
                    case Keys.A:
                        x = -1;
                        break;
                    case Keys.D:
                        x = 1;
                        break;
                    case Keys.W:
                        y = -1;
                        break;
                    case Keys.S:
                        y = 1;
                        break;
                    default:
                        return;
                }
                foreach (var element in _Image.Elements)
                {
                    var pivot = element.Pivot;
                    pivot.X += x;
                    pivot.Y += y;
                    element.Pivot = pivot;
                }
                Invalidate();
            }
        }
    }
}

