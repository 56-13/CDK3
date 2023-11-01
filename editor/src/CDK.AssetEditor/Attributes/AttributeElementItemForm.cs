using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    partial class AttributeElementItemForm : Form
    {
        private AttributeElement _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AttributeElement Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        _Element.PropertyChanged -= Element_PropertyChanged;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        _Element.PropertyChanged += Element_PropertyChanged;
                    }
                    ResetItems();

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }
        public event EventHandler ElementChanged;

        private bool _raiseItemsChanged;

        public AttributeElementItemForm()
        {
            InitializeComponent();

            _raiseItemsChanged = true;

            Disposed += AttributeElementItemForm_Disposed;
        }

        private void AttributeElementItemForm_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.PropertyChanged -= Element_PropertyChanged;
            }
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Items" && _raiseItemsChanged) ResetItems();
        }

        private void ResetItems()
        {
            dataGridView.Rows.Clear();

            if (_Element != null && _Element.Items != null)
            {
                foreach (var item in _Element.Items)
                {
                    dataGridView.Rows.Add(item.Name, item.Value);
                }
            }
            RestoreText();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _raiseItemsChanged = false;

                var items = new List<AttributeElementItem>(dataGridView.RowCount);

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var item = new AttributeElementItem
                        {
                            Name = (string)row.Cells[0].Value,
                            Value = (string)row.Cells[1].Value
                        };
                        items.Add(item);
                    }
                }
                _Element.Items = items.Count != 0 ? items.ToArray() : null;

                _raiseItemsChanged = true;

                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            RestoreText();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ApplyText();
        }

        private void RestoreText()
        {
            var builder = new StringBuilder();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (row.Cells[0].Value != null)
                    {
                        builder.Append(((string)row.Cells[0].Value).Trim());
                    }
                    builder.Append('\t');
                    if (row.Cells[1].Value != null)
                    {
                        builder.Append(((string)row.Cells[1].Value).Trim());
                    }
                    builder.Append("\r\n");
                }
            }
            textBox.Text = builder.ToString();
        }

        private void ApplyText()
        {
            var lines = textBox.Text.Split('\n');

            dataGridView.Rows.Clear();

            foreach (var line in lines)
            {
                var str = line.Split('\t');
                if (str.Length == 2)
                {
                    dataGridView.Rows.Add(str[0].Trim(), str[1].Trim());
                }
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                var index = dataGridView.SelectedRows[0].Index;

                dataGridView.Rows.Insert(index, 1);
            }
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                var row = dataGridView.SelectedRows[0];
                var index = row.Index;

                if (index > 0)
                {
                    dataGridView.Rows.RemoveAt(index);
                    dataGridView.Rows.Insert(index - 1, row);
                }
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                var row = dataGridView.SelectedRows[0];
                var index = row.Index;

                if (index < dataGridView.Rows.Count - 1)
                {
                    dataGridView.Rows.RemoveAt(index);
                    dataGridView.Rows.Insert(index + 1, row);
                }
            }
        }
    }
}
