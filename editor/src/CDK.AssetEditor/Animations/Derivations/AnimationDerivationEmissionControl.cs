using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationEmissionControl : UserControl
    {
        private AnimationDerivationEmission _Derivation;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationDerivationEmission Derivation
        {
            set
            {
                if (_Derivation != value)
                {
                    if (_Derivation != null)
                    {
                        emissionDelayUpDown.DataBindings.Clear();
                        emissionCountUpDown.DataBindings.Clear();
                        prewarmCheckBox.DataBindings.Clear();
                    }

                    _Derivation = value;

                    if (_Derivation != null)
                    {
                        emissionDelayUpDown.DataBindings.Add("Value", _Derivation, "EmissionDelay", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionCountUpDown.DataBindings.Add("Value", _Derivation, "EmissionCount", false, DataSourceUpdateMode.OnPropertyChanged);
                        prewarmCheckBox.DataBindings.Add("Checked", _Derivation, "Prewarm", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    DerivationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Derivation;
        }

        public event EventHandler DerivationChanged;

        public AnimationDerivationEmissionControl()
        {
            InitializeComponent();
        }
    }
}
