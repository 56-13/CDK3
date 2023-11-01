using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationColorChannelControl : UserControl
    {
        private AnimationColorChannel _Impl;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationColorChannel Impl
        {
            set
            {
                if (_Impl != value)
                {
                    if (_Impl != null)
                    {
                        fixedChannelCheckBox.DataBindings.Clear();
                    }

                    _Impl = value;

                    redControl.Value = _Impl?.Red;
                    greenControl.Value = _Impl?.Green;
                    blueControl.Value = _Impl?.Blue;
                    alphaControl.Value = _Impl?.Alpha;
                    screenControl.Impl = _Impl;

                    if (_Impl != null)
                    {
                        fixedChannelCheckBox.DataBindings.Add("Checked", _Impl, "FixedChannel", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ImplChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Impl;
        }
        public event EventHandler ImplChanged;

        public AnimationColorChannelControl()
        {
            InitializeComponent();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
