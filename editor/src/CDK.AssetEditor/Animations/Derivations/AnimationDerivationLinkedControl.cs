using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationLinkedControl : UserControl
    {
        private AnimationDerivationLinked _Derivation;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationDerivationLinked Derivation
        {
            set
            {
                if (_Derivation != value)
                {
                    if (_Derivation != null)
                    {
                        loopCountUpDown.DataBindings.Clear();
                    }

                    _Derivation = value;

                    if (_Derivation != null)
                    {
                        loopCountUpDown.DataBindings.Add("Value", _Derivation, "LoopCount", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    DerivationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Derivation;
        }

        public event EventHandler DerivationChanged;

        public AnimationDerivationLinkedControl()
        {
            InitializeComponent();
        }
    }
}
