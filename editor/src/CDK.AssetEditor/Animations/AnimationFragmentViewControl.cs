using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Animations.Components;

using CDK.Assets.Components;
using CDK.Assets.Meshing;
using CDK.Assets.Animations.Derivations;

namespace CDK.Assets.Animations
{
    public partial class AnimationFragmentViewControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private AnimationObjectFragment _Object;

        private AnimationFragment _fragParent;

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
                        var animation = _Object.Origin;

                        keysTextBox.DataBindings.Clear();
                        closingCheckBox.DataBindings.Clear();
                        pivotCheckBox.DataBindings.Clear();
                        stencilCheckBox.DataBindings.Clear();
                        durationControl.DataBindings.Clear();
                        latencyControl.DataBindings.Clear();
                        derivationTypeComboBox.DataBindings.Clear();
                        derivationFinishCheckBox.DataBindings.Clear();
                        randomWeightUpDown.DataBindings.Clear();

                        targetComboBox.DataBindings.Clear();
                        bindingComboBox.DataBindings.Clear();
                        bindingComboBox.DataSource = null;
                        radialControl.Value = null;
                        tangentialControl.Value = null;
                        tangentialAngleControl.Value = null;
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        pathSpeedRadioButton.DataBindings.Clear();
                        pathDurationRadioButton.DataBindings.Clear();
                        pathLoopControl.DataBindings.Clear();
                        reverseCheckBox.DataBindings.Clear();
                        facingCheckBox.DataBindings.Clear();
                        billboardOffsetCheckBox.DataBindings.Clear();

                        rotationXControl.Value = null;
                        rotationYControl.Value = null;
                        rotationZControl.Value = null;
                        rotationDurationControl.DataBindings.Clear();
                        rotationLoopControl.DataBindings.Clear();

                        scaleXControl.Value = null;
                        scaleYControl.Value = null;
                        scaleZControl.Value = null;
                        scaleYControl.DataBindings.Clear();
                        scaleZControl.DataBindings.Clear();
                        scaleEachCheckBox.DataBindings.Clear();
                        scaleDurationControl.DataBindings.Clear();
                        scaleLoopControl.DataBindings.Clear();

                        soundVolumeControl.DataBindings.Clear();
                        soundControlComboBox.DataBindings.Clear();
                        soundLoopUpDown.DataBindings.Clear();
                        soundPriorityUpDown.DataBindings.Clear(); 
                        soundLatencyControl.DataBindings.Clear();
                        soundDurationControl.DataBindings.Clear();
                        soundDuplicationControl.DataBindings.Clear();
                        soundStopCheckBox.DataBindings.Clear();

                        localeVisibleCheckBox.DataBindings.Clear();

                        animation.PropertyChanged -= Animation_PropertyChanged;
                        animation.Tangential.PropertyChanged -= Tangential_PropertyChanged;
                        animation.LocaleChanged -= Animation_LocaleChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        var animation = _Object.Origin;

                        var keysTextBoxBinding = new Binding("Text", animation, "Keys", true, DataSourceUpdateMode.Never);
                        keysTextBoxBinding.Format += KeysTextBoxBinding_Format;
                        keysTextBox.DataBindings.Add(keysTextBoxBinding);

                        closingCheckBox.DataBindings.Add("Checked", animation, "Closing", false, DataSourceUpdateMode.OnPropertyChanged);
                        pivotCheckBox.DataBindings.Add("Checked", animation, "Pivot", false, DataSourceUpdateMode.OnPropertyChanged);
                        stencilCheckBox.DataBindings.Add("Checked", animation, "Stencil", false, DataSourceUpdateMode.OnPropertyChanged);
                        durationControl.DataBindings.Add("Value", animation, "Duration", false, DataSourceUpdateMode.OnPropertyChanged);
                        latencyControl.DataBindings.Add("Value", animation, "Latency", false, DataSourceUpdateMode.OnPropertyChanged);
                        derivationTypeComboBox.DataBindings.Add("SelectedItem", animation, "DerivationType", false, DataSourceUpdateMode.OnPropertyChanged);
                        derivationFinishCheckBox.DataBindings.Add("Checked", animation, "DerivationFinish", false, DataSourceUpdateMode.OnPropertyChanged);
                        randomWeightUpDown.DataBindings.Add("Value", animation, "RandomWeight", false, DataSourceUpdateMode.OnPropertyChanged);

                        substanceButton.Text = animation.SubstanceType.ToString();

                        targetComboBox.DataBindings.Add("SelectedItem", animation, "Target", false, DataSourceUpdateMode.OnPropertyChanged);

                        ResetBinding();
                        bindingComboBox.DataBindings.Add(new NullableComboBoxBinding(animation, "NodeName"));

                        radialControl.Value = animation.Radial;
                        tangentialControl.Value = animation.Tangential;
                        tangentialAngleControl.Value = animation.TangentialAngle;
                        tangentialAngleControl.Visible = animation.Tangential.Type != AnimationFloatType.None;
                        xControl.Value = animation.X;
                        yControl.Value = animation.Y;
                        zControl.Value = animation.Z;
                        pathSpeedRadioButton.DataBindings.Add("Checked", animation, "UsingPathSpeed", false, DataSourceUpdateMode.OnPropertyChanged);
                        pathSpeedRadioButton.DataBindings.Add("Visible", pathCheckBox, "Checked", false, DataSourceUpdateMode.Never);
                        pathDurationRadioButton.DataBindings.Add("Visible", pathCheckBox, "Checked", false, DataSourceUpdateMode.Never);
                        pathCheckBox.Checked = (animation.UsingPathSpeed ? animation.PathSpeed : animation.PathDuration) != 0;
                        pathLoopControl.DataBindings.Add("Loop", animation, "PathLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        reverseCheckBox.DataBindings.Add("Checked", animation, "Reverse", false, DataSourceUpdateMode.OnPropertyChanged);
                        facingCheckBox.DataBindings.Add("Checked", animation, "Facing", false, DataSourceUpdateMode.OnPropertyChanged);
                        billboardOffsetCheckBox.DataBindings.Add("Checked", animation, "BillboardOffset", false, DataSourceUpdateMode.OnPropertyChanged);

                        rotationXControl.Value = animation.RotationX;
                        rotationYControl.Value = animation.RotationY;
                        rotationZControl.Value = animation.RotationZ;
                        rotationDurationControl.DataBindings.Add("Visible", rotationDurationCheckBox, "Checked", false, DataSourceUpdateMode.Never);
                        rotationDurationControl.DataBindings.Add("Value", animation, "RotationDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        rotationLoopControl.DataBindings.Add("Loop", animation, "RotationLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        rotationDurationCheckBox.Checked = animation.RotationDuration != 0;

                        scaleXControl.Value = animation.ScaleX;
                        scaleYControl.Value = animation.ScaleY;
                        scaleZControl.Value = animation.ScaleZ;
                        scaleYControl.DataBindings.Add("Visible", animation, "ScaleEach", false, DataSourceUpdateMode.Never);
                        scaleZControl.DataBindings.Add("Visible", animation, "ScaleEach", false, DataSourceUpdateMode.Never);
                        scaleEachCheckBox.DataBindings.Add("Checked", animation, "ScaleEach", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleDurationControl.DataBindings.Add("Visible", scaleDurationCheckBox, "Checked", false, DataSourceUpdateMode.Never);
                        scaleDurationControl.DataBindings.Add("Value", animation, "ScaleDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleLoopControl.DataBindings.Add("Loop", animation, "ScaleLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleDurationCheckBox.Checked = animation.ScaleDuration != 0;

                        soundVolumeControl.DataBindings.Add("Value", animation, "VolumSounde", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundControlComboBox.DataBindings.Add("SelectedItem", animation, "SoundControl", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundLoopUpDown.DataBindings.Add("Value", animation, "SoundLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundPriorityUpDown.DataBindings.Add("Value", animation, "SoundPriority", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundLatencyControl.DataBindings.Add("Value", animation, "SoundLatency", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundDurationControl.DataBindings.Add("Value", animation, "SoundDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundDuplicationControl.DataBindings.Add("Value", animation, "SoundDuplication", false, DataSourceUpdateMode.OnPropertyChanged);
                        soundStopCheckBox.DataBindings.Add("Value", animation, "SoundStop", false, DataSourceUpdateMode.OnPropertyChanged);

                        localeVisibleCheckBox.DataBindings.Add("Checked", animation, "LocaleVisible", false, DataSourceUpdateMode.OnPropertyChanged);

                        animation.PropertyChanged += Animation_PropertyChanged;
                        animation.Tangential.PropertyChanged += Tangential_PropertyChanged;
                        animation.LocaleChanged += Animation_LocaleChanged;
                    }
                    else
                    {
                        pathCheckBox.Checked = false;

                        tangentialAngleControl.Visible = false;

                        soundTextBox.Text = string.Empty;
                        soundAttrPanel.Visible = false;
                    }

                    var newFragParent = _Object?.Origin.Parent is AnimationFragment frag ? frag : null;

                    if (_fragParent != newFragParent)
                    {
                        if (_fragParent != null) _fragParent.PropertyChanged -= FragParent_PropertyChanged;
                        _fragParent = newFragParent;
                        if (_fragParent != null) _fragParent.PropertyChanged += FragParent_PropertyChanged;
                    }
                    randomWeightPanel.Visible = _fragParent != null && _fragParent.DerivationType == AnimationDerivationType.Random;

                    BindPathDegree();

                    ResetDerivation();
                    ResetSound();
                    ResetLocales();
                    CollapseDefault();

                    _keysForm.Animation = _soundForm.Animation = _Object?.Origin;

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private Control[] _derivationControls;

        private const int DerivationControlLinked = 0;
        private const int DerivationControlEmission = 1;
        private const int DerivationControlRandom = 2;
        private const int DerivationControlCount = 3;

        private AnimationKeysForm _keysForm;
        private AnimationSoundForm _soundForm;

        private bool _raiseLocaleCheckChanged;

        public AnimationFragmentViewControl()
        {
            InitializeComponent();

            derivationTypeComboBox.DataSource = Enum.GetValues(typeof(AnimationDerivationType));
            targetComboBox.DataSource = Enum.GetValues(typeof(AnimationTarget));
            pathDegreePanel.DataBindings.Add("Visible", pathCheckBox, "Checked", false, DataSourceUpdateMode.Never);
            rotationDurationPanel.DataBindings.Add("Visible", rotationDurationCheckBox, "Checked", false, DataSourceUpdateMode.Never);
            scaleDurationPanel.DataBindings.Add("Visible", scaleDurationCheckBox, "Checked", false, DataSourceUpdateMode.Never);

            _derivationControls = new Control[DerivationControlCount];

            _derivationControls[DerivationControlLinked] = new AnimationDerivationLinkedControl();
            _derivationControls[DerivationControlEmission] = new AnimationDerivationEmissionControl();
            _derivationControls[DerivationControlRandom] = new AnimationDerivationRandomControl();
            _derivationControls[DerivationControlLinked].ParentChanged += AnimationDerivationLinkedControl_ParentChanged;
            _derivationControls[DerivationControlEmission].ParentChanged += AnimationDerivationEmissionControl_ParentChanged;
            _derivationControls[DerivationControlRandom].ParentChanged += AnimationDerivationRandomControl_ParentChanged;

            foreach (var derivationControl in _derivationControls)
            {
                derivationControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }

            soundControlComboBox.DataSource = Enum.GetValues(typeof(AudioControl));

            foreach (var locale in AssetManager.Instance.Config.Locales)
            {
                localeCheckedListBox.Items.Add(locale, false);
            }
            _raiseLocaleCheckChanged = true;

            _keysForm = new AnimationKeysForm();
            _soundForm = new AnimationSoundForm();

            Disposed += AnimationControl_Disposed;
        }

        private void AnimationControl_Disposed(object sender, EventArgs e)
        {
            _keysForm.Dispose();
            _soundForm.Dispose();

            foreach (var derivationControl in _derivationControls)
            {
                derivationControl.Dispose();
            }

            if (_Object != null)
            {
                _Object.Origin.PropertyChanged -= Animation_PropertyChanged;
                _Object.Origin.Tangential.PropertyChanged -= Tangential_PropertyChanged;
                if (_fragParent != null) _fragParent.PropertyChanged -= FragParent_PropertyChanged;
            }
        }

        private void AnimationDerivationLinkedControl_ParentChanged(object sender, EventArgs e)
        {
            var control = (AnimationDerivationLinkedControl)sender;
            if (control.Parent == null) control.Derivation = null;
        }

        private void AnimationDerivationEmissionControl_ParentChanged(object sender, EventArgs e)
        {
            var control = (AnimationDerivationEmissionControl)sender;
            if (control.Parent == null) control.Derivation = null;
        }

        private void AnimationDerivationRandomControl_ParentChanged(object sender, EventArgs e)
        {
            var control = (AnimationDerivationRandomControl)sender;
            if (control.Parent == null) control.Derivation = null;
        }

        private void BindPathDegree()
        {
            pathDegreeControl.DataBindings.Clear();
            pathDegreeControl.DataBindings.Add("Visible", pathCheckBox, "Checked", false, DataSourceUpdateMode.Never);

            if (_Object != null)
            {
                if (_Object.Origin.UsingPathSpeed)
                {
                    pathDegreeControl.Maximum = 1000;
                    pathDegreeControl.DecimalPlaces = 2;
                    pathDegreeControl.Increment = 1;
                    pathDegreeControl.ScaleType = TrackBarScaleType.Linear;

                    pathDegreeControl.DataBindings.Add("Value", _Object.Origin, "PathSpeed", false, DataSourceUpdateMode.OnPropertyChanged);
                }
                else
                {
                    pathDegreeControl.Maximum = 60;
                    pathDegreeControl.DecimalPlaces = 2;
                    pathDegreeControl.Increment = 0.1f;
                    pathDegreeControl.ScaleType = TrackBarScaleType.None;

                    pathDegreeControl.DataBindings.Add("Value", _Object.Origin, "PathDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                }
            }
        }

        private void ResetDerivation()
        {
            Control subControl = null;

            if (_Object != null)
            {
                switch (_Object.Origin.DerivationType)
                {
                    case AnimationDerivationType.Linked:
                        subControl = _derivationControls[DerivationControlLinked];
                        ((AnimationDerivationLinkedControl)subControl).Derivation = (AnimationDerivationLinked)_Object.Origin.Derivation;
                        break;
                    case AnimationDerivationType.Emission:
                        subControl = _derivationControls[DerivationControlEmission];
                        ((AnimationDerivationEmissionControl)subControl).Derivation = (AnimationDerivationEmission)_Object.Origin.Derivation;
                        break;
                    case AnimationDerivationType.Random:
                        subControl = _derivationControls[DerivationControlRandom];
                        ((AnimationDerivationRandomControl)subControl).Derivation = (AnimationDerivationRandom)_Object.Origin.Derivation;
                        break;
                }
            }
            if (subControl != null)
            {
                if (subControl.Parent == null)
                {
                    derivationPanel.Visible = true;
                    derivationPanel.SuspendLayout();
                    derivationPanel.Height = subControl.Height + 28;
                    subControl.Width = derivationPanel.Width;
                    derivationPanel.Controls.Clear();
                    derivationPanel.Controls.Add(subControl);
                    derivationPanel.ResumeLayout();
                }
            }
            else
            {
                derivationPanel.Controls.Clear();
                derivationPanel.Visible = false;
            }
        }

        private void ResetSound()
        {
            soundTextBox.DataBindings.Clear();

            if (_Object?.Origin.SoundSource != null)
            {
                soundTextBox.DataBindings.Add("Text", _Object.Origin.SoundSource, "Location", false, DataSourceUpdateMode.Never);
                soundAttrPanel.Visible = true;
            }
            else
            {
                soundTextBox.Text = string.Empty;
                soundAttrPanel.Visible = false;
            }
        }

        private void ResetLocales()
        {
            _raiseLocaleCheckChanged = false;

            if (_Object != null)
            {
                for (var i = 0; i < localeCheckedListBox.Items.Count; i++)
                {
                    var locale = (string)localeCheckedListBox.Items[i];

                    localeCheckedListBox.SetItemChecked(i, _Object.Origin.GetLocaleChecked(locale));
                }
            }
            else
            {
                for (var i = 0; i < localeCheckedListBox.Items.Count; i++)
                {
                    localeCheckedListBox.SetItemChecked(i, false);
                }
            }
            _raiseLocaleCheckChanged = true;
        }

        private void Animation_LocaleChanged(object sender, AnimationLocaleEventArgs e)
        {
            if (_raiseLocaleCheckChanged)
            {
                _raiseLocaleCheckChanged = false;

                var index = localeCheckedListBox.Items.IndexOf(e.Locale);

                if (index >= 0)
                {
                    localeCheckedListBox.SetItemChecked(index, _Object.Origin.GetLocaleChecked(e.Locale));
                }

                _raiseLocaleCheckChanged = true;
            }
        }

        private void LocaleCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_Object != null && _raiseLocaleCheckChanged)
            {
                var locale = (string)localeCheckedListBox.Items[e.Index];

                var check = e.NewValue == CheckState.Checked;

                _Object.Origin.SetLocaleChecked(locale, check);
            }
        }

        private void KeysTextBoxBinding_Format(object sender, ConvertEventArgs e)
        {
            var keys = (string[])e.Value;

            if (keys != null)
            {
                var str = new StringBuilder();
                str.Append(keys[0]);
                for (var i = 1; i < keys.Length; i++)
                {
                    str.Append(',');
                    str.Append(keys[i]);
                }
                e.Value = str.ToString();
            }
            else
            {
                e.Value = string.Empty;
            }
        }

        private void FragParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DerivationType")
            {
                randomWeightPanel.Visible = _fragParent.DerivationType == AnimationDerivationType.Random;
            }
        }

        private void Animation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UsingPathSpeed":
                    BindPathDegree();
                    break;
                case "SubstanceType":
                    substanceButton.Text = _Object.Origin.SubstanceType.ToString();
                    break;
                case "DerivationType":
                    ResetDerivation();
                    break;
                case "Sound":
                    ResetSound();
                    break;
            }
        }

        private void Tangential_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                tangentialAngleControl.Visible = _Object.Origin.Tangential.Type != AnimationFloatType.None;
            }
        }

        private void SoundImportButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _soundForm.ShowDialog(this);
        }

        private void SoundClearButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Origin.SoundSource = null;
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = Splitted ? Math.Max(mainPanel.Height, subPanel.Height) : mainPanel.Height;
        }

        private void PathCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && !pathCheckBox.Checked && (_Object.Origin.UsingPathSpeed ? _Object.Origin.PathSpeed : _Object.Origin.PathDuration) != 0)
            {
                pathCheckBox.Checked = true;
            }
        }

        private void RotationDurationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && !rotationDurationCheckBox.Checked && _Object.Origin.RotationDuration != 0)
            {
                rotationDurationCheckBox.Checked = true;
            }
        }

        private void ScaleDurationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && !scaleDurationCheckBox.Checked && _Object.Origin.ScaleDuration != 0)
            {
                scaleDurationCheckBox.Checked = true;
            }
        }

        private void KeysButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _keysForm.ShowDialog(this);
        }

        private void SubstanceButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _Object.SubstanceView = true;
        }

        public bool Splitted
        {
            set
            {
                if (value != Splitted)
                {
                    mainPanel.SuspendLayout();
                    subPanel.SuspendLayout();

                    splitContainer.Panel2Collapsed = !value;

                    if (value)
                    {
                        mainPanel.Controls.Remove(rotationPanel);
                        mainPanel.Controls.Remove(scalePanel);
                        mainPanel.Controls.Remove(soundPanel);
                        mainPanel.Controls.Remove(localePanel);

                        subPanel.Width = splitContainer.Panel2.Width;       //사이즈변경이 제대로 안됨

                        subPanel.Controls.Add(rotationPanel);
                        subPanel.Controls.Add(scalePanel);
                        subPanel.Controls.Add(soundPanel);
                        subPanel.Controls.Add(localePanel);
                    }
                    else
                    {
                        subPanel.Controls.Remove(rotationPanel);
                        subPanel.Controls.Remove(scalePanel);
                        subPanel.Controls.Remove(soundPanel);
                        subPanel.Controls.Remove(localePanel);

                        rotationPanel.Location = new System.Drawing.Point(0, positionPanel.Bottom);
                        scalePanel.Location = new System.Drawing.Point(0, rotationPanel.Bottom);
                        soundPanel.Location = new System.Drawing.Point(0, scalePanel.Bottom);
                        localePanel.Location = new System.Drawing.Point(0, soundPanel.Bottom);

                        mainPanel.Controls.Add(rotationPanel);
                        mainPanel.Controls.Add(scalePanel);
                        mainPanel.Controls.Add(soundPanel);
                        mainPanel.Controls.Add(localePanel);
                    }

                    mainPanel.ResumeLayout();
                    subPanel.ResumeLayout();
                }
            }
            get => !splitContainer.Panel2Collapsed;
        }

        public void CollapseAll()
        {
            mainPanel.SuspendLayout();
            subPanel.SuspendLayout();

            instancePanel.Collapsed =
            positionPanel.Collapsed =
            rotationPanel.Collapsed =
            scalePanel.Collapsed =
            soundPanel.Collapsed =
            localePanel.Collapsed = true;
            
            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Object != null)
            {
                mainPanel.SuspendLayout();
                subPanel.SuspendLayout();

                instancePanel.Collapsed = false;

                var animation = _Object.Origin;

                positionPanel.Collapsed =
                    animation.Target == AnimationTarget.Origin &&
                    animation.Radial.Type == AnimationFloatType.None &&
                    animation.Tangential.Type == AnimationFloatType.None &&
                    animation.X.Type == AnimationFloatType.None &&
                    animation.Y.Type == AnimationFloatType.None &&
                    animation.Z.Type == AnimationFloatType.None;

                rotationPanel.Collapsed =
                    animation.RotationX.Type == AnimationFloatType.None &&
                    animation.RotationY.Type == AnimationFloatType.None &&
                    animation.RotationZ.Type == AnimationFloatType.None;

                scalePanel.Collapsed =
                    animation.ScaleX.Type == AnimationFloatType.None &&
                    (!animation.ScaleEach || animation.ScaleY.Type == AnimationFloatType.None) &&
                    (!animation.ScaleEach || animation.ScaleZ.Type == AnimationFloatType.None);

                soundPanel.Collapsed = animation.SoundSource == null;

                localePanel.Collapsed = animation.LocaleVisible && !animation.GetLocaleChecked();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }

        private void ResetBinding()
        {
            if (_Object.Origin.Target == AnimationTarget.Origin && _fragParent?.Substance is MeshObject mesh && mesh.Selection.Geometry != null)
            {
                bindingComboBox.DataSource = mesh.Selection.Geometry.Origin.GetBoneNames();
            }
            else bindingComboBox.DataSource = null;
        }

        private void BindingComboBox_DropDown(object sender, EventArgs e)
        {
            var item = bindingComboBox.SelectedItem;
            ResetBinding();
            bindingComboBox.SelectedItem = item;
        }

        private void BindingClearButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) _Object.Origin.Binding = null;
        }
    }
}
