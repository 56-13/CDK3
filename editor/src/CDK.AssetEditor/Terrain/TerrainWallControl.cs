using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace CDK.Assets.Terrain
{
    public partial class TerrainWallControl : UserControl
    {
        private TerrainWallComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainWallComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        sourceListBox.DataBindings.Clear();
                        sourceListBox.DataSource = null;
                        sourceControl.DataBindings.Clear();

                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;
                    
                    if (_Object != null)
                    {
                        sourceListBox.DataSource = _Object.Asset.Walls;
                        sourceListBox.DisplayMember = "Name";
                        sourceListBox.DataBindings.Add("SelectedItem", _Object, "SelectedSource", true, DataSourceUpdateMode.OnPropertyChanged);
                        sourceControl.DataBindings.Add("Wall", _Object, "SelectedSource", true, DataSourceUpdateMode.OnPropertyChanged);
                        var sourceControlVisibleBinding = new Binding("Visible", _Object, "SelectedSource", true, DataSourceUpdateMode.Never);
                        sourceControlVisibleBinding.Format += (object sender, ConvertEventArgs e) => e.Value = ((TerrainWall)e.Value != null);
                        sourceControl.DataBindings.Add(sourceControlVisibleBinding);

                        _Object.PropertyChanged += Object_PropertyChanged;
                    }
                    else
                    {
                        sourceControl.Visible = false;
                    }

                    BindSelectedInstance();
                    BindSelectedInstancePoint();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private TextBox _nameTextBox;

        public TerrainWallControl()
        {
            InitializeComponent();

            _nameTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            _nameTextBox.LostFocus += NameTextBox_LostFocus;
            _nameTextBox.KeyDown += NameTextBox_KeyDown;

            Disposed += TerrainWallControl_Disposed;
        }

        private void TerrainWallControl_Disposed(object sender, EventArgs e)
        {
            _nameTextBox.Dispose();

            if (_Object != null) _Object.PropertyChanged -= Object_PropertyChanged;
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "SelectedInstance":
                    BindSelectedInstance();
                    break;
                case "SelectedInstancePoint":
                    BindSelectedInstancePoint();
                    break;
            }
        }

        private void BindSelectedInstance()
        {
            instanceTextBox.DataBindings.Clear();
            instanceLoopUpDown.DataBindings.Clear();
            
            var instance = _Object?.SelectedInstance;

            if (instance != null)
            {
                instanceTextBox.DataBindings.Add("Text", instance.Wall, "Name", false, DataSourceUpdateMode.Never);
                instanceLoopUpDown.DataBindings.Add("Value", instance, "Loop", false, DataSourceUpdateMode.OnPropertyChanged);

                instancePanel.Visible = true;
            }
            else instancePanel.Visible = false;
        }

        private void BindSelectedInstancePoint()
        {
            instancePointXUpDown.DataBindings.Clear();
            instancePointYUpDown.DataBindings.Clear();
            instancePointZUpDown.DataBindings.Clear();

            var instancePoint = _Object?.SelectedInstancePoint;

            if (instancePoint != null)
            {
                instancePointXUpDown.DataBindings.Add("Value", instancePoint, "X", false, DataSourceUpdateMode.Never);
                instancePointYUpDown.DataBindings.Add("Value", instancePoint, "Y", false, DataSourceUpdateMode.Never);
                instancePointZUpDown.DataBindings.Add("Value", instancePoint, "Z", false, DataSourceUpdateMode.OnPropertyChanged);
                instancePointDeleteButton.Visible = instancePoint.Parent.Points.Count > 1 && (instancePoint == instancePoint.Parent.Points.First() || instancePoint == instancePoint.Parent.Points.Last());
                instancePointPanel.Visible = true;
            }
            else instancePointPanel.Visible = false;
        }

        private void EndEditName(bool commit)
        {
            if (commit)
            {
                var water = (TerrainWall)_nameTextBox.Tag;

                water.Name = _nameTextBox.Text;
            }
            _nameTextBox.Tag = null;
            _nameTextBox.Parent.Controls.Remove(_nameTextBox);
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) EndEditName(true);
            else if (e.KeyCode == Keys.Escape) EndEditName(false);
        }

        private void NameTextBox_LostFocus(object sender, EventArgs e)
        {
            EndEditName(true);
        }

        private void SourceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !sourceCheckBox.Checked;
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.SelectedSource;

                if (selection != null)
                {
                    var i = _Object.Asset.Walls.IndexOf(selection) + 1;

                    _Object.Asset.Walls.Insert(i, new TerrainWall(_Object.Asset));
                }
                else
                {
                    _Object.Asset.Walls.Add(new TerrainWall(_Object.Asset));
                }
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.SelectedSource;

                if (selection != null)
                {
                    _Object.Asset.Walls.Remove(selection);
                }
            }
        }

        private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.SelectedSource;

                if (selection != null)
                {
                    var i = _Object.Asset.Walls.IndexOf(selection);

                    if (i > 0)
                    {
                        _Object.Asset.Walls.Move(i, i - 1);
                    }
                }
            }
        }

        private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.SelectedSource;

                if (selection != null)
                {
                    var i = _Object.Asset.Walls.IndexOf(selection);

                    if (i < _Object.Asset.Walls.Count - 1)
                    {
                        _Object.Asset.Walls.Move(i, i + 1);
                    }
                }
            }
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nameTextBox.Parent == null)
            {
                var i = sourceListBox.SelectedIndex;

                if (i != -1)
                {
                    var wall = (TerrainWall)sourceListBox.SelectedItem;

                    _nameTextBox.Location = new Point(0, i * sourceListBox.ItemHeight);
                    _nameTextBox.Size = new Size(sourceListBox.Width, 21);
                    _nameTextBox.Tag = wall;
                    _nameTextBox.Text = wall.Name;
                    sourceListBox.Controls.Add(_nameTextBox);

                    _nameTextBox.Focus();
                }
            }
        }

        private void InstanceUpButton_Click(object sender, EventArgs e)
        {
            _Object?.SelectedInstance?.MoveZ(1);
        }

        private void InstanceDownButton_Click(object sender, EventArgs e)
        {
            _Object?.SelectedInstance?.MoveZ(-1);
        }

        private void InstanceFlipButton_Click(object sender, EventArgs e)
        {
            _Object?.SelectedInstance?.Flip();
        }

        private void InstanceDeleteButton_Click(object sender, EventArgs e)
        {
            var instance = _Object?.SelectedInstance;

            if (instance != null) _Object.Asset.WallInstances.Remove(instance);
        }

        private void InstancePointDeleteButton_Click(object sender, EventArgs e)
        {
            _Object?.RemoveInstancePoint();
        }
    }
}
