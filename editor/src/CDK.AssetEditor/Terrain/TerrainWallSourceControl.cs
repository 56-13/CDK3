using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Terrain
{
    public partial class TerrainWallSourceControl : UserControl
    {
        private TerrainWall _Wall;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainWall Wall
        {
            set
            {
                if (_Wall != value)
                {
                    if (_Wall != null)
                    {
                        selectionControl.Selection = null;
                        boneListBox.DataSource = null;
                    }

                    _Wall = value;

                    if (_Wall != null)
                    {
                        selectionControl.Selection = _Wall.Selection;
                        boneListBox.DataSource = _Wall.Bones;
                    }

                    WallChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Wall;
        }

        public event EventHandler WallChanged;

        public TerrainWallSourceControl()
        {
            InitializeComponent();
        }

        private void BoneContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            boneAddToolStripMenuItem.DropDownItems.Clear();

            if (_Wall != null && _Wall.Selection.Geometry != null)
            {
                foreach (var bone in _Wall.Selection.Geometry.GetTransformNames())
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
            if (_Wall != null)
            {
                var bone = ((ToolStripMenuItem)sender).Text;

                _Wall.Bones.Add(bone);
            }
        }

        private void BoneRemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Wall != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                for (var i = indices.Length - 1; i >= 0; i--) _Wall.Bones.RemoveAt(indices[i]);
            }
        }

        private void BoneUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Wall != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                if (indices[0] > 0)
                {
                    foreach (var i in indices) _Wall.Bones.Move(i, i - 1);
                }
            }
        }

        private void BoneDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Wall != null)
            {
                var indices = new int[boneListBox.SelectedIndices.Count];
                boneListBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);
                if (indices[0] > 0)
                {
                    for (var i = indices.Length - 1; i >= 0; i--) _Wall.Bones.Move(indices[i], indices[i] + 1);
                }
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
