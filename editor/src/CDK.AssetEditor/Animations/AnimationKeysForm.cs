using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Configs;

namespace CDK.Assets.Animations
{
    partial class AnimationKeysForm : Form
    {
        private AnimationFragment _Animation;
        public AnimationFragment Animation
        {
            set
            {
                if (_Animation != value)
                {
                    if (_Animation != null)
                    {
                        checkedListBox.Items.Clear();

                        _Animation.PropertyChanged -= Animation_PropertyChanged;
                    }

                    _Animation = value;
                    
                    if (_Animation != null)
                    {
                        var project = _Animation.Project;

                        if (project != null)
                        {
                            var keyConstants = project.GetAnimationKeyConstants();

                            foreach (var key in keyConstants)
                            {
                                checkedListBox.Items.Add(key);

                                if (_Animation.Keys != null && Array.IndexOf(_Animation.Keys, key.Name) != -1) checkedListBox.SetItemChecked(checkedListBox.Items.Count - 1, true);
                            }
                        }
                        
                        _Animation.PropertyChanged += Animation_PropertyChanged;
                    }
                    
                    AnimationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Animation;
        }

        public event EventHandler AnimationChanged;
        
        public AnimationKeysForm()
        {
            InitializeComponent();

            Disposed += AnimationKeyForm_Disposed;
        }

        private void AnimationKeyForm_Disposed(object sender, EventArgs e)
        {
            if (_Animation != null)
            {
                _Animation.PropertyChanged -= Animation_PropertyChanged;
            }
        }

        private void Animation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Project":
                    checkedListBox.Items.Clear();

                    var project = _Animation.Project;

                    if (project != null)
                    {
                        var keyConstants = project.GetAnimationKeyConstants();

                        foreach (var key in keyConstants)
                        {
                            checkedListBox.Items.Add(key);

                            if (_Animation.Keys != null && Array.IndexOf(_Animation.Keys, key.Name) != -1) checkedListBox.SetItemChecked(checkedListBox.Items.Count - 1, true);
                        }
                    }
                    break;
                case "Keys":
                    for (var i = 0; i < checkedListBox.Items.Count; i++)
                    {
                        var key = (Constant)checkedListBox.Items[i];

                        checkedListBox.SetItemChecked(i, _Animation.Keys != null && Array.IndexOf(_Animation.Keys, key.Name) != -1);
                    }
                    break;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_Animation != null)
            {
                string[] keys = null;
                if (checkedListBox.CheckedItems.Count != 0)
                {
                    keys = new string[checkedListBox.CheckedItems.Count];
                    for (var i = 0; i < checkedListBox.CheckedItems.Count; i++)
                    {
                        keys[i] = ((Constant)checkedListBox.CheckedItems[i]).Name;
                    }
                }
                _Animation.Keys = keys;

                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
