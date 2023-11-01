using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using Cyotek.Windows.Forms;

using CDK.Drawing;

namespace CDK.Assets.Components
{
    public partial class ColorControl : UserControl
    {
        [DefaultValue(typeof(Color), "0xFFFFFFFF")]
        public Color ValueGDI
        {
            set
            {
                if (_colorDialog.Color != value)
                {
                    _colorDialog.Color = value;

                    if (!_colorDialog.Visible) OnValueChanged();
                }
            }
            get => _colorDialog.Color;
        }

        public event EventHandler ValueGDIChanged;

        [DefaultValue(typeof(Color3), "1F, 1F, 1F")]
        public Color3 Value3
        {
            set
            {
                if (Value3 != value)
                {
                    _colorDialog.Color = Color.FromArgb(value.NormalR, value.NormalG, value.NormalB);

                    if (!_colorDialog.Visible) OnValueChanged();
                }
            }
            get
            {
                var color = _colorDialog.Color;
                return new Color3(color.R, color.G, color.B);
            }
        }

        public event EventHandler Value3Changed;

        [DefaultValue(typeof(Color4), "1F, 1F, 1F, 1F")]
        public Color4 Value4
        {
            set
            {
                if (Value4 != value)
                {
                    _colorDialog.Color = Color.FromArgb(value.NormalA, value.NormalR, value.NormalG, value.NormalB);

                    if (!_colorDialog.Visible) OnValueChanged();
                }
            }
            get
            {
                var color = _colorDialog.Color;
                return new Color4(color.R, color.G, color.B, color.A);
            }
        }

        public event EventHandler Value4Changed;

        private void OnValueChanged()
        {
            ValueGDIChanged?.Invoke(this, EventArgs.Empty);
            Value3Changed?.Invoke(this, EventArgs.Empty);
            Value4Changed?.Invoke(this, EventArgs.Empty);
        }

        [DefaultValue(false)]
        public bool AlphaChannel
        {
            set => _colorDialog.ShowAlphaChannel = value;
            get => _colorDialog.ShowAlphaChannel;
        }

        private ColorPickerDialog _colorDialog;

        public ColorControl()
        {
            InitializeComponent();

            _colorDialog = new ColorPickerDialog()
            {
                Color = Color.White,
                ShowAlphaChannel = false
            };
            _colorDialog.PreviewColorChanged += ColorDialog_PreviewColorChanged;

            Disposed += Color3Control_Disposed;
        }

        private void Color3Control_Disposed(object sender, EventArgs e)
        {
            _colorDialog.Dispose();
        }

        private void ColorDialog_PreviewColorChanged(object sender, EventArgs e)
        {
            if (_colorDialog.Visible) OnValueChanged();

            Invalidate();
        }

        private Color _prevColor;

        private void Color3Control_Click(object sender, EventArgs e)
        {
            _prevColor = _colorDialog.Color;

            if (_colorDialog.ShowDialog(this) != DialogResult.OK)
            {
                _colorDialog.Color = _prevColor;

                OnValueChanged();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.LightGray))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
            using (var brush = new SolidBrush(_colorDialog.Color))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
        }
    }
}
