using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationLoopControl : UserControl
    {
        private bool _raiseAnimationLoopChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationLoop Loop
        {
            set
            {
                if (Loop != value)
                {
                    _raiseAnimationLoopChanged = false;
                    
                    countUpDown.Value = value.Count;
                    roundTripCheckBox.Checked = value.RoundTrip;
                    finishCheckBox.Checked = value.Finish;

                    _raiseAnimationLoopChanged = true;

                    LoopChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new AnimationLoop((int)countUpDown.Value, roundTripCheckBox.Checked, finishCheckBox.Checked);
        }

        public event EventHandler LoopChanged;

        [DefaultValue(false)]
        public bool FinishEnabled
        {
            set => finishCheckBox.Visible = value;
            get => finishCheckBox.Visible;
        }
        
        public AnimationLoopControl()
        {
            InitializeComponent();

            _raiseAnimationLoopChanged = true;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            if (_raiseAnimationLoopChanged)
            {
                LoopChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
