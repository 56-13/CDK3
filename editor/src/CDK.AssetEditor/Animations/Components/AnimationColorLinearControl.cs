using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationColorLinearControl : UserControl
    {
        private AnimationColorLinear _Impl;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationColorLinear Impl
        {
            set
            {
                if (_Impl != value)
                {
                    if (_Impl != null)
                    {
                        startColorControl.DataBindings.Clear();
                        endColorControl.DataBindings.Clear();
                    }

                    _Impl = value;

                    if (_Impl != null)
                    {
                        startColorControl.AlphaChannel = endColorControl.AlphaChannel = _Impl.AlphaChannel;

                        startColorControl.DataBindings.Add("Value4", _Impl, "LinearStartColor", false, DataSourceUpdateMode.OnPropertyChanged);
                        endColorControl.DataBindings.Add("Value4", _Impl, "LinearEndColor", false, DataSourceUpdateMode.OnPropertyChanged);
                        smoothCheckBox.DataBindings.Add("Checked", _Impl, "LinearSmooth", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ImplChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Impl;
        }
        public event EventHandler ImplChanged;

        public AnimationColorLinearControl()
        {
            InitializeComponent();
        }
    }
}
