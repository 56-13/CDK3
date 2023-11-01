using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class ImageReplaceScreenControl : Control
    {
        private ImageAsset _Source;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAsset Source
        {
            set
            {
                if (_Source != value)
                {
                    DataBindings.Clear();

                    _Source = value;

                    DataBindings.Add("BackColor", _Source.ScreenColor, "Background", false, DataSourceUpdateMode.Never);
                    DataBindings.Add("ForeColor", _Source.ScreenColor, "Foreground", false, DataSourceUpdateMode.Never);

                    Invalidate();

                    SourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Source;
        }
        public event EventHandler SourceChanged;

        private Bitmap _Destination;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap Destination
        {
            set
            {
                if (_Destination != value)
                {
                    _Destination?.Dispose();
                    _Destination = value;
                    Invalidate();

                    DestinationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Destination;
        }
        public event EventHandler DestinationChanged;

        public ScreenRatio Ratio { private set; get; }

        public ImageReplaceScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();

            Ratio = new ScreenRatio();

            Disposed += RootImageAssetReplaceScreenControl_Disposed;
        }

        private void RootImageAssetReplaceScreenControl_Disposed(object sender, EventArgs e)
        {
            _Destination?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(0, 0, Width, Height));

            if (_Source != null)
            {
                pe.Graphics.SmoothingMode = SmoothingMode.None;
                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;

                if (_Source.Content.Bitmap != null)
                {
                    var rect = new Rectangle(
                        (int)(Width / 2 - _Source.Frame.Width * Ratio / 2),
                        (int)(Height / 4 - _Source.Frame.Height * Ratio / 2),
                        (int)(_Source.Frame.Width * Ratio),
                        (int)(_Source.Frame.Height * Ratio));

                    pe.Graphics.DrawImage(_Source.Content.Bitmap, rect, _Source.Frame, GraphicsUnit.Pixel);
                }

                if (_Destination != null)
                {
                    pe.Graphics.DrawImage(_Destination, new RectangleF(
                        Width / 2 - _Destination.Width / 2 * Ratio,
                        Height * 3 / 4 - _Destination.Height / 2 * Ratio,
                        _Destination.Width * Ratio,
                        _Destination.Height * Ratio));
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
