using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

using CDK.Assets.Support;

namespace CDK.Assets.Texturing
{
    partial class ImageScreenControl : Control
    {
        private RootImageAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RootImageAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        _Asset.PropertyChanged -= Asset_PropertyChanged;
                        _Asset.ScreenColor.PropertyChanged -= ScreenColor_PropertyChanged;
                    }

                    SubAsset = null;
                    _Asset = value;
                    FitSize();

                    if (_Asset != null)
                    {
                        _Asset.PropertyChanged += Asset_PropertyChanged;
                        _Asset.ScreenColor.PropertyChanged += ScreenColor_PropertyChanged;
                    }
                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private SubImageAsset _SubAsset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubImageAsset SubAsset
        {
            set
            {
                if (_SubAsset != value)
                {
                    _SubAsset = value;
                    Invalidate();

                    SubAssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SubAsset;
        }
        public event EventHandler SubAssetChanged;

        private bool _Pivot;

        [DefaultValue(false)]
        public bool Pivot
        {
            set
            {
                if (_Pivot != value)
                {
                    _Pivot = value;
                    Invalidate();

                    PivotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Pivot;
        }
        public event EventHandler PivotChanged;

        public ScreenRatio Ratio { private set; get; }

        private float _scale;

        public ImageScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);  

            InitializeComponent();

            Ratio = new ScreenRatio();
            Ratio.PropertyChanged += Ratio_PropertyChanged;

            Disposed += RootImageAssetScreenControl_Disposed;
        }

        private void RootImageAssetScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                _Asset.PropertyChanged -= Asset_PropertyChanged;
            }
        }
        private void FitSize()
        {
            _scale = Ratio;

            if (_Asset != null)
            {
                var image = _Asset.Content.Bitmap;

                if (image != null)
                {
                    _scale *= _Asset.ContentScale;

                    var width = image.Width * _scale;
                    var height = image.Height * _scale;

                    var s = AssetControl.MaxControlSize / Math.Max(width, height);

                    if (s < 1) _scale *= s;

                    Width = (int)(image.Width * _scale);
                    Height = (int)(image.Height * _scale);
                }
                else
                {
                    Width = 0;
                    Height = 0;
                }
            }
            else
            {
                Width = 0;
                Height = 0;
            }
            Invalidate();
        }
        private void Ratio_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem") FitSize();
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ContentScale":
                case "Frame":
                    FitSize();
                    break;
            }
        }

        private void ScreenColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem") Invalidate();
        }

        private void DrawSubImages(Graphics g)
        {
            foreach (var sub in _Asset.SubImages)
            {
                foreach (var se in sub.Elements)
                {
                    var frame = se.Frame;

                    var x = frame.X * _scale;
                    var y = frame.Y * _scale;
                    var w = frame.Width * _scale;
                    var h = frame.Height * _scale;

                    if (_SubAsset == sub)
                    {
                        using (var pen = new Pen(_Asset.ScreenColor.Foreground, 5))
                        {
                            g.DrawRectangle(pen, x - 2, y - 2, w + 3, h + 3);
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(_Asset.ScreenColor.Foreground))
                        {
                            g.DrawRectangle(pen, x - 1, y - 1, w + 1, h + 1);
                        }
                    }
                    if (_Pivot)
                    {
                        using (var pen = new Pen(_Asset.ScreenColor.Foreground))
                        {
                            var px = x + w * 0.5f + se.Pivot.X * _scale;
                            var py = y + h * 0.5f + se.Pivot.Y * _scale;
                            g.DrawLine(pen, x, py, x + w, py);
                            g.DrawLine(pen, px, y, px, y + h);
                        }
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_Asset != null)
            {
                pe.Graphics.Clear(_Asset.ScreenColor.Background);

                var image = _Asset.Content.Bitmap;

                if (image != null)
                {
                    pe.Graphics.SmoothingMode = SmoothingMode.None;
                    pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    pe.Graphics.PixelOffsetMode = PixelOffsetMode.None;
                    pe.Graphics.DrawImage(image, new RectangleF(0, 0, image.Width * _scale, image.Height * _scale));

                    DrawSubImages(pe.Graphics);
                }
            }
            base.OnPaint(pe);
        }

        private bool SelectChild(int x, int y)
        {
            foreach (var sub in _Asset.SubImages)
            {
                foreach (var se in sub.Elements)
                {
                    if (se.Frame.Contains(x, y))
                    {
                        SubAsset = sub;
                        SubAsset.SelectedElement = se;
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_Asset != null)
            {
                Focus();

                var x = (int)(e.X / _scale);
                var y = (int)(e.Y / _scale);

                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        SelectChild(x, y);
                        break;
                    case MouseButtons.Right:
                        if (controlKey && _SubAsset != null)
                        {
                            var se = _SubAsset.SelectedElement;

                            if (se != null) se.AutoSelect(x, y, shiftKey);
                        }
                        else if (!SelectChild(x, y) && _Asset.AutoSelect(null, x, y, false, out var frame, out var pivot))
                        {
                            var sub = new SubImageAsset();
                            _Asset.Children.Add(sub);
                            sub.SelectedElement.Frame = frame;
                            sub.SelectedElement.Pivot = pivot;
                        }
                        break;
                }
            }

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
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (_SubAsset != null && MessageBox.Show(this, "리소스를 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (_SubAsset.IsRetained(out var from, out var to))
                        {
                            AssetControl.ShowRetained(this, from, to);
                            return;
                        }
                        _SubAsset.Parent.Children.Remove(_SubAsset);
                    }
                    break;
            }
            base.OnKeyDown(e);
        }
    }
}
