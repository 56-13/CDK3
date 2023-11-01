using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class ScaleSpriteElementControl : UserControl
    {
        private SpriteElementScale _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementScale Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        eachCheckBox.DataBindings.Clear();
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        yControl.DataBindings.Clear();
                        zControl.DataBindings.Clear();
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        eachCheckBox.DataBindings.Add("Checked", _Element, "Each", false, DataSourceUpdateMode.OnPropertyChanged);
                        xControl.Value = _Element.X;
                        yControl.Value = _Element.Y;
                        zControl.Value = _Element.Z;
                        yControl.DataBindings.Add("Visible", _Element, "Each", false, DataSourceUpdateMode.Never);
                        zControl.DataBindings.Add("Visible", _Element, "Each", false, DataSourceUpdateMode.Never);
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public ScaleSpriteElementControl()
        {
            InitializeComponent();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
