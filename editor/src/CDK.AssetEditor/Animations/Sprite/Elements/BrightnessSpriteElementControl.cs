using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class BrightnessSpriteElementControl : UserControl
    {
        private SpriteElementBrightness _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementBrightness Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        brightnessControl.Value = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        brightnessControl.Value = _Element.Value;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public BrightnessSpriteElementControl()
        {
            InitializeComponent();
        }

        private void BrightnessControl_SizeChanged(object sender, EventArgs e)
        {
            Height = brightnessControl.Height;
        }
    }
}
