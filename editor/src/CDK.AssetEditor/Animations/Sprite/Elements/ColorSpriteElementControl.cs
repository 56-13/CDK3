using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class ColorSpriteElementControl : UserControl
    {
        private SpriteElementColor _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementColor Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        colorControl.Color = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        colorControl.Color = _Element.Color;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public ColorSpriteElementControl()
        {
            InitializeComponent();
        }

        private void ColorControl_SizeChanged(object sender, EventArgs e)
        {
            Height = colorControl.Height;
        }
    }
}
