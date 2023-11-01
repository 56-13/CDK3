using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CDK.Assets.Terrain
{
    public partial class TerrainWaterControl : UserControl
    {
        private TerrainWaterComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainWaterComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        altitudeControl.DataBindings.Clear();
                        listBox.DataBindings.Clear();
                        listBox.DataSource = null;
                        materialControl.DataBindings.Clear();
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        altitudeControl.DataBindings.Add("Value", _Object, "Altitude", false, DataSourceUpdateMode.OnPropertyChanged);
                        listBox.DataSource = _Object.Asset.Waters;
                        listBox.DisplayMember = "Name";
                        listBox.DataBindings.Add("SelectedItem", _Object, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.DataBindings.Add("Water", _Object, "Selection", true, DataSourceUpdateMode.Never);
                        var materialControlVisibleBinding = new Binding("Visible", _Object, "Selection", true, DataSourceUpdateMode.Never);
                        materialControlVisibleBinding.Format += MaterialControlVisibleBinding_Format;
                        materialControl.DataBindings.Add(materialControlVisibleBinding);
                    }
                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        private void MaterialControlVisibleBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = ((TerrainWater)e.Value != null);
        }

        public event EventHandler ObjectChanged;

        private TextBox _nameTextBox;

        public TerrainWaterControl()
        {
            InitializeComponent();

            _nameTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            _nameTextBox.LostFocus += NameTextBox_LostFocus;
            _nameTextBox.KeyDown += NameTextBox_KeyDown;

            Disposed += TerrainWaterControl_Disposed;
        }

        private void TerrainWaterControl_Disposed(object sender, EventArgs e)
        {
            _nameTextBox.Dispose();
        }

        private void EndEditName(bool commit)
        {
            if (commit)
            {
                var water = (TerrainWater)_nameTextBox.Tag;

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

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Waters.IndexOf(selection) + 1;

                    _Object.Asset.Waters.Insert(i, new TerrainWater(_Object.Asset));
                }
                else
                {
                    _Object.Asset.Waters.Add(new TerrainWater(_Object.Asset));
                }
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    _Object.Asset.Waters.Remove(selection);
                }
            }
        }

        private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Waters.IndexOf(selection);

                    if (i > 0)
                    {
                        _Object.Asset.Waters.Move(i, i - 1);
                    }
                }
            }
        }

        private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Waters.IndexOf(selection);

                    if (i < _Object.Asset.Waters.Count - 1)
                    {
                        _Object.Asset.Waters.Move(i, i + 1);
                    }
                }
            }
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_nameTextBox.Parent == null)
            {
                var i = listBox.SelectedIndex;

                if (i != -1)
                {
                    var water = (TerrainWater)listBox.SelectedItem;

                    _nameTextBox.Location = new Point(0, i * listBox.ItemHeight);
                    _nameTextBox.Size = new Size(listBox.Width, 21);
                    _nameTextBox.Tag = water;
                    _nameTextBox.Text = water.Name;
                    listBox.Controls.Add(_nameTextBox);

                    _nameTextBox.Focus();
                }
            }
        }

        private void MaterialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !materialCheckBox.Checked;
        }
    }
}
