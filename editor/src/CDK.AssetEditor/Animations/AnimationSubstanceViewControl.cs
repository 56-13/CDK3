using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;
using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public partial class AnimationSubstanceViewControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private AnimationObjectFragment _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationObjectFragment Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        _Object.Origin.PropertyChanged -= Animation_PropertyChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        typeComboBox.SelectedItem = _Object.Origin.SubstanceType;

                        _Object.Origin.PropertyChanged += Animation_PropertyChanged;
                    }

                    ResetSubstance();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }
        public event EventHandler ObjectChanged;

        public AnimationSubstanceViewControl()
        {
            InitializeComponent();

            typeComboBox.DataSource = AnimationSubstance.SubstanceTypes;
            
            Disposed += AnimationSubstanceViewControl_Disposed;
        }

        private void AnimationSubstanceViewControl_Disposed(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.Origin.PropertyChanged -= Animation_PropertyChanged;
            }
        }

        private void Animation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "SubstanceType":
                    typeComboBox.SelectedItem = _Object.Origin.SubstanceType;
                    ResetSubstance();
                    break;
            }
        }

        private void SubControl_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Top + ((Control)sender).Height;
        }

        private void ResetSubstance()
        {
            panel.SuspendLayout();
            panel.Controls.Clear();

            var newControl = _Object.Substance != null ? AssetControl.Instance.GetControl(_Object.Substance) : null;

            if (newControl != null)
            {
                if (newControl is ISplittableControl splttableControl) splttableControl.Splitted = _Splitted;

                Height = panel.Top + newControl.Height;
                newControl.Width = panel.Width;
                newControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                panel.Controls.Add(newControl);
            }
            else Height = panel.Top;

            panel.ResumeLayout();

            SceneControl.ValidateBottomControl(this);
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _Object.SubstanceView = false;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (_Object != null && typeComboBox.SelectedItem != null)
            {
                _Object.Origin.SubstanceType = (SceneComponentType)typeComboBox.SelectedItem;
            }
        }

        private Control SubstanceControl => panel.Controls.Count > 0 ? panel.Controls[0] : null;

        private bool _Splitted;
        public bool Splitted
        {
            set
            {
                if (_Splitted != value)
                {
                    _Splitted = value;
                    if (SubstanceControl is ISplittableControl splttableControl) splttableControl.Splitted = value;
                }
            }
            get => _Splitted;
        }

        public void CollapseAll()
        {
            if (SubstanceControl is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseAll();
        }

        public void CollapseDefault()
        {
            if (SubstanceControl is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseDefault();
        }
    }
}
