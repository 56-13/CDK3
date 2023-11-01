using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Media;

namespace CDK.Assets.Animations
{
    //TODO

    partial class AnimationSoundForm : Form
    {
        private AnimationFragment _Animation;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationFragment Animation
        {
            set
            {
                if (_Animation != value)
                {
                    if (_Animation != null)
                    {
                        treeView.DataBindings.Clear();
                    }
                    
                    _Animation = value;
                    
                    if (_Animation != null)
                    {
                        //treeView.DataBindings.Add("RootAsset", _Animation, "Project", true, DataSourceUpdateMode.Never);
                        treeView.DataBindings.Add("SelectedAsset", _Animation, "SoundSource", true, DataSourceUpdateMode.Never);
                    }
                }
            }
            get => _Animation;
        }

        public AnimationSoundForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_Animation != null && treeView.SelectedAsset != null)
            {
                _Animation.SoundSource = (MediaAsset)treeView.SelectedAsset;

                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void TreeView_Confirm(object sender, EventArgs e)
        {
            OkButton_Click(this, EventArgs.Empty);
        }
    }
}
