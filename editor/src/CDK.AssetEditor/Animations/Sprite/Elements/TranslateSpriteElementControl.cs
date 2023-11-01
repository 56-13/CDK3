using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class TranslateSpriteElementControl : UserControl
    {
        private SpriteElementTranslate _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementTranslate Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        xControl.Value = _Element.X;
                        yControl.Value = _Element.Y;
                        zControl.Value = _Element.Z;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public TranslateSpriteElementControl()
        {
            InitializeComponent();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
