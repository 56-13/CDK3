using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class ContrastSpriteElementControl : UserControl
    {
        private ContrastSpriteElement _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContrastSpriteElement Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        contrastControl.Value = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        contrastControl.Value = _Element.Value;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public ContrastSpriteElementControl()
        {
            InitializeComponent();
        }

        private void ContrastControl_SizeChanged(object sender, EventArgs e)
        {
            Height = contrastControl.Height;
        }
    }
}
