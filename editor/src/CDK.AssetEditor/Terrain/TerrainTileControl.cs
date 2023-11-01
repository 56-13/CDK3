using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using Cyotek.Windows.Forms;

namespace CDK.Assets.Terrain
{
    //TODO

    public partial class TerrainTileControl : UserControl
    {
        private TerrainTileComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainTileComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        tileComboBox.DataBindings.Clear();
                        tileComboBox.DataSource = null;

                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        //tileComboBox.DataSource = _Object.Asset.Tiles;
                        tileComboBox.DisplayMember = "Name";
                        tileComboBox.DataBindings.Add("SelectedItem", _Object, "SelectedTile", true, DataSourceUpdateMode.OnPropertyChanged);

                        _Object.PropertyChanged += Object_PropertyChanged;
                    }

                    BindTile();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }
        public event EventHandler ObjectChanged;

        private ColorPickerDialog _colorDialog;

        public TerrainTileControl()
        {
            InitializeComponent();

            boundaryComboBox.DataSource = Enum.GetValues(typeof(IntegerElementType));

            dataGridView.AutoGenerateColumns = false;

            _colorDialog = new ColorPickerDialog()
            {
                Color = Color.White,
                ShowAlphaChannel = true
            };

            Disposed += TerrainTileControl_Disposed;
        }

        private void TerrainTileControl_Disposed(object sender, EventArgs e)
        {
            _colorDialog.Dispose();

            if (_Object != null)
            {
                _Object.PropertyChanged -= Object_PropertyChanged;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedTile") BindTile();
        }

        private void BindTile()
        {
            nameTextBox.DataBindings.Clear();
            boundaryComboBox.DataBindings.Clear();
            selectionComboBox.DataBindings.Clear();
            selectionComboBox.DataSource = null;
            dataGridView.DataBindings.Clear();
            dataGridView.DataSource = null;

            /*
            var tile = _Object?.SelectedTile;

            if (tile != null)
            {
                subPanel.Visible = true;

                nameTextBox.DataBindings.Add("Enabled", AssetManager.Instance, "IsDeveloper", false, DataSourceUpdateMode.Never);
                boundaryComboBox.DataBindings.Add("Enabled", AssetManager.Instance, "IsDeveloper", false, DataSourceUpdateMode.Never);
                dataGridView.DataBindings.Add("AllowUserToAddRows", AssetManager.Instance, "IsDeveloper", false, DataSourceUpdateMode.Never);
                dataGridView.DataBindings.Add("AllowUserToDeleteRows", AssetManager.Instance, "IsDeveloper", false, DataSourceUpdateMode.Never);

                nameTextBox.DataBindings.Add("Text", tile, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
                boundaryComboBox.DataBindings.Add("SelectedItem", tile, "Boundary", false, DataSourceUpdateMode.OnPropertyChanged);
                selectionComboBox.DataSource = tile.Elements;
                selectionComboBox.DisplayMember = "Name";
                selectionComboBox.DataBindings.Add("SelectedItem", _Object, "SelectedElement", true, DataSourceUpdateMode.OnPropertyChanged);

                dataGridView.DataSource = tile.Elements;
            }
            else subPanel.Visible = false;
            */
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            /*
            if (_Object != null)
            {
                var newTile = new TerrainTile(_Object.Asset);
                _Object.Asset.Tiles.Add(newTile);
                _Object.SelectedTile = newTile;
            }
            */
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            /*
            if (_Object != null)
            {
                var index = tileComboBox.SelectedIndex;

                if (index >= 0) _Object.Asset.Tiles.RemoveAt(index);
            }
            */
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            /*
            var tile = _Object?.SelectedTile;

            if (tile != null && e.ColumnIndex == 2 && e.RowIndex >= 0 && e.RowIndex < tile.Elements.Count)
            {
                var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

                cell.Style.BackColor = cell.Style.SelectionBackColor = (Color)tile.Elements[e.RowIndex].Color;
            }
            */
        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if (AssetManager.Instance.IsDeveloper && e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                var tile = _Object?.SelectedTile;

                if (tile != null)
                {
                    _colorDialog.Color = (Color)tile.Elements[e.RowIndex].Color;

                    if (_colorDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        tile.Elements[e.RowIndex].Color = _colorDialog.Color;
                    }
                }
            }
            */
        }

        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            dataGridView.AllowUserToAddRows = true;
        }

        private void DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            dataGridView.AllowUserToAddRows = false;
        }

        private void DataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!AssetManager.Instance.IsDeveloper) e.Cancel = true;
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (AssetManager.Instance.IsDeveloper && dataGridView.SelectedRows.Count != 0)
            {
                var tile = _Object?.SelectedTile;

                if (tile != null)
                {
                    var index = dataGridView.SelectedRows[0].Index;

                    tile.Elements.Insert(index, new TerrainTileElement(tile));
                }
            }
            */
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (AssetManager.Instance.IsDeveloper && dataGridView.SelectedRows.Count != 0)
            {
                var tile = _Object?.SelectedTile;

                if (tile != null)
                {
                    var indices = new int[dataGridView.SelectedRows.Count];
                    for (var i = 0; i < indices.Length; i++)
                    {
                        indices[i] = dataGridView.SelectedRows[i].Index;
                    }
                    Array.Sort(indices);

                    if (indices[0] > 0)
                    {
                        foreach (var idx in indices)
                        {
                            tile.Elements.Move(idx, idx - 1);
                        }
                    }
                }
            }
            */
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (AssetManager.Instance.IsDeveloper && dataGridView.SelectedRows.Count != 0)
            {
                var tile = _Object?.SelectedTile;

                if (tile != null)
                {
                    var indices = new int[dataGridView.SelectedRows.Count];
                    for (var i = 0; i < indices.Length; i++)
                    {
                        indices[i] = dataGridView.SelectedRows[i].Index;
                    }
                    Array.Sort(indices);

                    if (indices[indices.Length - 1] < dataGridView.Rows.Count - 1)
                    {
                        for (var i = indices.Length - 1; i >= 0; i--)
                        {
                            tile.Elements.Move(indices[i], indices[i] + 1);
                        }
                    }
                }
            }
            */
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!AssetManager.Instance.IsDeveloper) e.Cancel = true;
        }
    }
}
