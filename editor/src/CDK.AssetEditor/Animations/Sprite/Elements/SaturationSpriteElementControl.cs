using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class SaturationSpriteElementControl : UserControl
    {
        private SpriteElementSaturation _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementSaturation Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        saturationControl.Value = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        saturationControl.Value = _Element.Value;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public SaturationSpriteElementControl()
        {
            InitializeComponent();
        }

        private void SaturationControl_SizeChanged(object sender, EventArgs e)
        {
            Height = saturationControl.Height;
        }
    }
}
