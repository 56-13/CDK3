using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace CDK.Assets.Attributes
{
    partial class AttributeControl : UserControl
    {
        private Attribute _Attribute;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Attribute Attribute
        {
            set
            {
                if (_Attribute != value)
                {
                    if (_Attribute != null)
                    {
                        dataGridView.DataSource = null;

                        if (_valueComboBox.Parent != null)
                        {
                            _valueComboBox.SelectedIndexChanged -= ValueComboBox_SelectedIndexChanged;
                            _valueComboBox.DataSource = null;
                            Controls.Remove(_valueComboBox);
                        }
                        dataGridView.Enabled = true;

                        _Attribute.Elements.ListChanged -= Elements_ListChanged;
                    }
                    _Attribute = value;

                    if (_Attribute != null)
                    {
                        dataGridView.DataSource = _Attribute.Elements;

                        for (var i = 0; i < _Attribute.Elements.Count; i++)
                        {
                            dataGridView.Rows[i].Cells[1].ReadOnly = _Attribute.Elements[i].Items != null;
                        }

                        _Attribute.Elements.ListChanged += Elements_ListChanged;
                    }

                    _replaceValueForm.Element = null;

                    AttributeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Attribute;
        }

        public event EventHandler AttributeChanged;

        private ComboBox _valueComboBox;
        private int _valueComboBoxRow;
        private AttributeElementReplaceValueForm _replaceValueForm;

        public AttributeControl()
        {
            InitializeComponent();

            typeColumn.DataSource = Enum.GetValues(typeof(ElementType));
            listColumn.DataSource = Enum.GetValues(typeof(AttributeElementListType));

            dataGridView.AutoGenerateColumns = false;

            _valueComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _replaceValueForm = new AttributeElementReplaceValueForm();

            if (AssetManager.IsCreated)
            {
                ApplyAuthority();
                AssetManager.Instance.PropertyChanged += AssetManager_PropertyChanged;
            }

            Disposed += AttributeAssetControl_Disposed;
        }

        private void AttributeAssetControl_Disposed(object sender, EventArgs e)
        {
            _valueComboBox.Dispose();
            _replaceValueForm.Dispose();

            if (AssetManager.IsCreated)
            {
                AssetManager.Instance.PropertyChanged -= AssetManager_PropertyChanged;
            }

            if (_Attribute != null)
            {
                _Attribute.Elements.ListChanged -= Elements_ListChanged;
            }
        }

        private void Elements_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.PropertyDescriptor != null && e.PropertyDescriptor.Name == "Items")
            {
                dataGridView.Rows[e.NewIndex].Cells[1].ReadOnly = _Attribute.Elements[e.NewIndex].Items != null;
            }
        }

        private void ApplyAuthority()
        {
            var isDeveloper = AssetManager.Instance.IsDeveloper;
            dataGridView.AllowUserToAddRows = isDeveloper;
            dataGridView.AllowUserToDeleteRows = isDeveloper;
            nameColumn.ReadOnly = !isDeveloper;
            typeColumn.ReadOnly = !isDeveloper;
            itemButtonColumn.ReadOnly = !isDeveloper;
            listColumn.ReadOnly = !isDeveloper;
            localeColumn.ReadOnly = !isDeveloper;
            upToolStripMenuItem.Enabled = isDeveloper;
            downToolStripMenuItem.Enabled = isDeveloper;
        }

        private void AssetManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsDeveloper")) ApplyAuthority();
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Attribute != null && dataGridView.SelectedRows.Count > 0)
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
                        _Attribute.Elements.Move(idx, idx - 1);
                    }
                    foreach (var idx in indices)
                    {
                        dataGridView.Rows[idx - 1].Selected = true;
                    }
                }
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Attribute != null && dataGridView.SelectedRows.Count > 0)
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
                        _Attribute.Elements.Move(indices[i], indices[i] + 1);
                    }
                    for (var i = indices.Length - 1; i >= 0; i--)
                    {
                        dataGridView.Rows[indices[i] + 1].Selected = true;
                    }
                }
            }
        }

        private void ReplaceValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Attribute != null)
            {
                int rowIndex;
                if (dataGridView.SelectedRows.Count == 1)
                {
                    rowIndex = dataGridView.SelectedRows[0].Index;
                }
                else if (dataGridView.SelectedCells.Count == 1 && dataGridView.SelectedCells[0].ColumnIndex == 1)
                {
                    rowIndex = dataGridView.SelectedCells[0].RowIndex;
                }
                else return;

                var row = dataGridView.Rows[rowIndex];

                if (!row.IsNewRow)
                {
                    _replaceValueForm.Element = _Attribute.Elements[row.Index];
                    _replaceValueForm.ShowDialog(this);
                }
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_Attribute != null && e.RowIndex >= 0 && !dataGridView.Rows[e.RowIndex].IsNewRow)
            {
                var element = _Attribute.Elements[e.RowIndex];

                if (e.ColumnIndex == 1)
                {
                    if (element.Items != null)
                    {
                        if (element.ListType != AttributeElementListType.None)
                        {
                            using (var form = new AttributeElementSelectListItemForm())
                            {
                                form.Element = element;

                                form.ShowDialog(this);
                            }
                        }
                        else
                        {
                            var cellRectangle = dataGridView.GetCellDisplayRectangle(1, e.RowIndex, false);
                            
                            //VS2008 ZERO를 리턴할 때가 있음. 현재는 괜찮은 것으로 보이고 코드 남김
                            /*
                            var x = dataGridView.RowHeadersWidth + dataGridView.Columns[0].Width;
                            var y = dataGridView.ColumnHeadersHeight;
                            for (var i = 0; i < e.RowIndex; i++)
                            {
                                y += dataGridView.Rows[i].Height;
                            }
                            var w = dataGridView.Columns[1].Width;
                            var h = dataGridView.Rows[e.RowIndex].Height;

                            var cellRectangle = new Rectangle(x, y, w, h);
                            */
                            _valueComboBoxRow = e.RowIndex;

                            _valueComboBox.Bounds = cellRectangle;
                            _valueComboBox.DataSource = element.Items;
                            _valueComboBox.DisplayMember = "Name";
                            _valueComboBox.SelectedItem = element.Items.FirstOrDefault(item => item.Name == element.Value);
                            Controls.Add(_valueComboBox);
                            _valueComboBox.BringToFront();
                            _valueComboBox.Focus();
                            _valueComboBox.SelectedIndexChanged += ValueComboBox_SelectedIndexChanged;
                            _valueComboBox.PreviewKeyDown += ValueComboBox_PreviewKeyDown;
                            _valueComboBox.LostFocus += ValueComboBox_LostFocus;

                            dataGridView.Enabled = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 3 && AssetManager.Instance.IsDeveloper)
                {
                    using (var form = new AttributeElementItemForm())
                    {
                        form.Element = element;

                        form.ShowDialog(this);
                    }
                }
            }
        }

        private void HideComboBox()
        {
            _valueComboBox.SelectedIndexChanged -= ValueComboBox_SelectedIndexChanged;
            _valueComboBox.PreviewKeyDown -= ValueComboBox_PreviewKeyDown;
            _valueComboBox.LostFocus -= ValueComboBox_LostFocus;
            _valueComboBox.DataSource = null;
            Controls.Remove(_valueComboBox);

            dataGridView.Enabled = true;
        }

        private void ValueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_Attribute != null)
            {
                var item = (AttributeElementItem)_valueComboBox.SelectedItem;

                if (item != null)
                {
                    _Attribute.Elements[_valueComboBoxRow].Value = item.Name;
                }
            }

            HideComboBox();
        }

        private void ValueComboBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                HideComboBox();
                e.IsInputKey = true;
            }
        }

        private void ValueComboBox_LostFocus(object sender, EventArgs e)
        {
            HideComboBox();
        }

        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            dataGridView.AllowUserToAddRows = AssetManager.Instance.IsDeveloper;
        }

        private void DataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (_Attribute != null)
            {
                if (_Attribute.Elements[e.Row.Index].IsRetained(out var from, out var to))
                {
                    AssetControl.ShowRetained(this, from, to);

                    e.Cancel = true;
                }
                else 
                {
                    dataGridView.AllowUserToAddRows = false;
                }
            }
        }
    }
}
