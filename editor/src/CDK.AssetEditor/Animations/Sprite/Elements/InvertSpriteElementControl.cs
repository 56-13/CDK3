using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class InvertSpriteElementControl : UserControl
    {
        private SpriteElementInvert _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementInvert Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        xCheckBox.DataBindings.Clear();
                        yCheckBox.DataBindings.Clear();
                        zCheckBox.DataBindings.Clear();
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        xCheckBox.DataBindings.Add("Checked", _Element, "X", false, DataSourceUpdateMode.OnPropertyChanged);
                        yCheckBox.DataBindings.Add("Checked", _Element, "Y", false, DataSourceUpdateMode.OnPropertyChanged);
                        zCheckBox.DataBindings.Add("Checked", _Element, "Z", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public InvertSpriteElementControl()
        {
            InitializeComponent();
        }
    }
}
