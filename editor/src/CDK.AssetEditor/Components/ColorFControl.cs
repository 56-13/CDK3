using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using CDK.Drawing;

namespace CDK.Assets.Components
{
    public partial class ColorFControl : UserControl
    {
        private bool _raiseValueChanged;

        [DefaultValue(typeof(Color), "0xFFFFFFFF")]
        public Color ValueGDI
        {
            set => Value4 = new Color4(value.R, value.G, value.B, value.A);
            get
            {
                var color = Value4;
                return Color.FromArgb(color.NormalA, color.NormalR, color.NormalG, color.NormalB);
            }
        }

        public event EventHandler ValueGDIChanged;

        [DefaultValue(typeof(Color3), "1F, 1F, 1F")]
        public Color3 Value3
        {
            set => Value4 = value;
            get => (Color3)Value4;
        }

        public event EventHandler Value3Changed;

        [DefaultValue(typeof(Color4), "1F, 1F, 1F, 1F")]
        public Color4 Value4
        {
            set
            {
                if (Value4 != value)
                {
                    _raiseValueChanged = false;

                    colorControl.Value4 = value.Normalized;
                    amplificationControl.Value = value.Amplification;

                    _raiseValueChanged = true;

                    OnValueChanged();
                }
            }
            get => new Color4(colorControl.Value4, amplificationControl.Value);
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
            set => colorControl.AlphaChannel = value;
            get => colorControl.AlphaChannel;
        }

        public ColorFControl()
        {
            InitializeComponent();
                        
            _raiseValueChanged = true;
        }

        private void Component_ValueChanged(object sender, EventArgs e)
        {
            if (_raiseValueChanged) OnValueChanged();
        }
    }
}
