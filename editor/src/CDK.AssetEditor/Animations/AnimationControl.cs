using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Animations
{
    public partial class AnimationControl : UserControl
    {
        private AnimationObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        _Object.PropertyChanged += Object_PropertyChanged;
                    }

                    ResetKeys();
                    ResetTarget();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private bool _keyBinding;

        public AnimationControl()
        {
            InitializeComponent();

            Disposed += AnimationRootControl_Disposed;
        }

        private void AnimationRootControl_Disposed(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.PropertyChanged -= Object_PropertyChanged;
            }
        }

        private void ResetKeys()
        {
            if (_keyBinding) return;

            _keyBinding = true;

            keyListBox.Items.Clear();

            var project = _Object?.Project;

            if (project != null)
            {
                var i = 0;
                foreach(var key in project.GetAnimationKeyConstants())
                {
                    keyListBox.Items.Add(key);
                    if (_Object.Keys == null || Array.IndexOf(_Object.Keys, key) != -1) keyListBox.SetItemChecked(i, true);
                    i++;
                }
            }

            _keyBinding = false;
        }

        private void ResetTarget()
        {
            switch (_Object?.SelectedTarget ?? AnimationTarget.Origin)
            {
                case AnimationTarget.Origin:
                    originRadioButton.Checked = true;
                    targetCheckBox.DataBindings.Clear();
                    targetCheckBox.Visible = false;
                    targetControl.DataBindings.Clear();
                    targetControl.Visible = false;
                    targetControl.Gizmo = null;
                    break;
                case AnimationTarget.Target1:
                    target1RadioButton.Checked = true;
                    targetCheckBox.DataBindings.Add("Checked", _Object, "UsingTarget1", false, DataSourceUpdateMode.OnPropertyChanged);
                    targetCheckBox.Visible = true;
                    targetControl.DataBindings.Add("Visible", _Object, "UsingTarget1", false, DataSourceUpdateMode.Never);
                    targetControl.Gizmo = _Object.GetTarget(AnimationTarget.Target1);
                    break;
                case AnimationTarget.Target2:
                    target2RadioButton.Checked = true;
                    targetCheckBox.DataBindings.Add("Checked", _Object, "UsingTarget2", false, DataSourceUpdateMode.OnPropertyChanged);
                    targetCheckBox.Visible = true;
                    targetControl.DataBindings.Add("Visible", _Object, "UsingTarget2", false, DataSourceUpdateMode.Never);
                    targetControl.Gizmo = _Object.GetTarget(AnimationTarget.Target2);
                    break;
                case AnimationTarget.Target3:
                    target3RadioButton.Checked = true;
                    targetCheckBox.DataBindings.Add("Checked", _Object, "UsingTarget3", false, DataSourceUpdateMode.OnPropertyChanged);
                    targetCheckBox.Visible = true;
                    targetControl.DataBindings.Add("Visible", _Object, "UsingTarget3", false, DataSourceUpdateMode.Never);
                    targetControl.Gizmo = _Object.GetTarget(AnimationTarget.Target3);
                    break;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedTarget":
                    ResetTarget();
                    break;
                case "Keys":
                    ResetKeys();
                    break;
            }
        }

        private void OriginRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && originRadioButton.Checked)
            {
                _Object.SelectedTarget = AnimationTarget.Origin;
            }
        }

        private void Target1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && target1RadioButton.Checked)
            {
                _Object.SelectedTarget = AnimationTarget.Target1;
            }
        }

        private void Target2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && target2RadioButton.Checked)
            {
                _Object.SelectedTarget = AnimationTarget.Target2;
            }
        }

        private void Target3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_Object != null && target3RadioButton.Checked)
            {
                _Object.SelectedTarget = AnimationTarget.Target3;
            }
        }

        private void KeyListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_Object != null && !_keyBinding)
            {
                _keyBinding = true;

                var checkedItems = keyListBox.CheckedItems;
                if (checkedItems.Count == keyListBox.Items.Count) _Object.Keys = null;
                else
                {
                    var keys = new string[checkedItems.Count];
                    checkedItems.CopyTo(keys, 0);
                    _Object.Keys = keys;
                }

                _keyBinding = false;
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            var action = (Action)(() => { Height = panel.Height; });
            //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
            //스크롤바에 추가되는 컨트롤에만 이 코드 추가

            if (IsHandleCreated) BeginInvoke(action);
            else action.Invoke();
        }
    }
}
