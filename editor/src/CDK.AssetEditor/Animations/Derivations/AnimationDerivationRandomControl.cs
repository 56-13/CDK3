using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationRandomControl : UserControl
    {
        private AnimationDerivationRandom _Derivation;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationDerivationRandom Derivation
        {
            set
            {
                if (_Derivation != value)
                {
                    if (_Derivation != null)
                    {
                        loopCheckBox.DataBindings.Clear();
                    }

                    _Derivation = value;

                    if (_Derivation != null)
                    {
                        loopCheckBox.DataBindings.Add("Checked", _Derivation, "Loop", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    DerivationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Derivation;
        }

        public event EventHandler DerivationChanged;

        public AnimationDerivationRandomControl()
        {
            InitializeComponent();
        }
    }
}
