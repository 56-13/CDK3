using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Sources
{
    public partial class AnimationSourcesControl : UserControl
    {
        private AssetElementList<AnimationSource> _Sources;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AssetElementList<AnimationSource> Sources
        {
            set
            {
                if (_Sources != value)
                {
                    _Sources = value;

                    listBox.DataSource = _Sources;
                    listBox.DisplayMember = "Type";

                    SourcesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Sources;
        }

        public event EventHandler SourcesChanged;


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationSource SelectedSource
        {
            set => listBox.SelectedItem = value;
            get => (AnimationSource)listBox.SelectedItem;
        }

        [DefaultValue(false)]
        public bool UsingBones
        {
            set => _meshControl.UsingBones = value;
            get => _meshControl.UsingBones;
        }

        public event EventHandler SelectedSourceChanged;

        private AnimationSourceImageControl _imageControl;
        private AnimationSourceMeshControl _meshControl;

        public AnimationSourcesControl()
        {
            InitializeComponent();

            var p = new System.Drawing.Point(0, listBox.Bottom + 4);
            _imageControl = new AnimationSourceImageControl
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Location = p
            };
            _meshControl = new AnimationSourceMeshControl
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Location = p
            };
            _imageControl.SizeChanged += Control_SizeChanged;
            _meshControl.SizeChanged += Control_SizeChanged;

            Disposed += AnimationSourcesControl_Disposed;
        }

        private void AnimationSourcesControl_Disposed(object sender, EventArgs e)
        {
            _imageControl.Source = null;
            _meshControl.Source = null;
            _imageControl.Dispose();
            _meshControl.Dispose();
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;

            if (control.Parent == this) Height = control.Bottom;
        }

        private void AddImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sources != null)
            {
                var i = listBox.SelectedIndex;

                _Sources.Insert(i + 1, new AnimationSourceImage(_Sources.Parent));
            }
        }

        private void AddMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sources != null)
            {
                var i = listBox.SelectedIndex;

                _Sources.Insert(i + 1, new AnimationSourceMesh(_Sources.Parent));
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sources != null && listBox.SelectedIndices.Count > 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                for (var i = indices.Length - 1; i >= 0; i--)
                {
                    _Sources.RemoveAt(indices[i]);
                }
            }
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sources != null && listBox.SelectedIndices.Count > 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices[0] > 0)
                {
                    foreach (var index in indices)
                    {
                        _Sources.Move(index, index - 1);
                    }
                }
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sources != null && listBox.SelectedIndices.Count > 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices.Last() < _Sources.Count - 1)
                {
                    for (var i = indices.Length - 1; i >= 0; i--)
                    {
                        _Sources.Move(indices[i], indices[i] + 1);
                    }
                }
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var source = (AnimationSource)listBox.SelectedItem;

            SuspendLayout();

            Control control = null;

            if (source != null)
            {
                switch (source.Type)
                {
                    case AnimationSourceType.Image:
                        _imageControl.Source = (AnimationSourceImage)source;
                        Controls.Remove(_meshControl);
                        _meshControl.Source = null;
                        if (_imageControl.Parent == null) Controls.Add(_imageControl);
                        control = _imageControl;
                        break;
                    case AnimationSourceType.Mesh:
                        _meshControl.Source = (AnimationSourceMesh)source;
                        Controls.Remove(_imageControl);
                        _imageControl.Source = null;
                        if (_meshControl.Parent == null) Controls.Add(_meshControl);
                        control = _meshControl;
                        break;
                }
            }
            else
            {
                Controls.Remove(_meshControl);
                Controls.Remove(_imageControl);
                _imageControl.Source = null;
                _meshControl.Source = null;
            }

            if (control != null)
            {
                Height = control.Bottom;
                control.Width = Width;
            }
            else
            {
                Height = listBox.Height;
            }

            ResumeLayout();

            SelectedSourceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                listBox.SelectedIndices.Clear();
            }
        }
    }
}
