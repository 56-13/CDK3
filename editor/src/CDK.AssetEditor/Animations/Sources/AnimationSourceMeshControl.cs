using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sources
{
    public partial class AnimationSourceMeshControl : UserControl
    {
        private AnimationSourceMesh _Source;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationSourceMesh Source
        {
            set
            {
                if (_Source != value)
                {
                    if (_Source != null)
                    {
                        selectionControl.Selection = null;
                        boneListBox.DataSource = null;
                    }

                    _Source = value;

                    if (_Source != null)
                    {
                        selectionControl.Selection = _Source.Selection;
                        boneListBox.DataSource = _Source.Bones;
                    }

                    SourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Source;
        }

        public event EventHandler SourceChanged;

        [DefaultValue(false)]
        public bool UsingBones
        {
            set => bonePanel.Visible = value;
            get => bonePanel.Visible;
        }
        
        public AnimationSourceMeshControl()
        {
            InitializeComponent();
        }

        private void BoneContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            boneAddToolStripMenuItem.DropDownItems.Clear();

            if (_Source != null && _Source.Selection.Geometry != null)
            {
                foreach (var bone in _Source.Selection.Geometry.GetTransformNames())
                {
                    var boneAddSubToolStripMenuItem = new ToolStripMenuItem
                    {
                        Text = bone
                    };
                    boneAddSubToolStripMenuItem.Click += BoneAddSubToolStripMenuItem_Click;
                    boneAddToolStripMenuItem.DropDownItems.Add(boneAddSubToolStripMenuItem);
                }
            }
        }

        private void BoneAddSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Source != null)
            {
                var bone = ((ToolStripMenuItem)sender).Text;

                _Source.Bones.Add(bone);
            }
        }

        private void BoneRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Source != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                for (var i = indices.Length - 1; i >= 0; i--) _Source.Bones.RemoveAt(indices[i]);
            }
        }

        private void BoneUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Source != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                if (indices[0] > 0)
                {
                    foreach (var i in indices) _Source.Bones.Move(i, i - 1);
                }
            }
        }

        private void BoneDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Source != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                if (indices[0] > 0)
                {
                    for (var i = indices.Length - 1; i >= 0; i--) _Source.Bones.Move(indices[i], indices[i] + 1);
                }
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
