using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class WaveSpriteElementControl : UserControl
    {
        private SpriteElementWave _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementWave Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        positionControl.DataBindings.Clear();
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        radiusControl.Value = null;
                        thicknessControl.Value = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        positionControl.DataBindings.Add("Value", _Element, "Position", false, DataSourceUpdateMode.OnPropertyChanged);
                        xControl.Value = _Element.X;
                        yControl.Value = _Element.Y;
                        zControl.Value = _Element.Z;
                        radiusControl.Value = _Element.Radius;
                        thicknessControl.Value = _Element.Thickness;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public WaveSpriteElementControl()
        {
            InitializeComponent();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
