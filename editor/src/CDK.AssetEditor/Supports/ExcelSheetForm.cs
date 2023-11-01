using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Support
{
    public partial class ExcelSheetForm : Form
    {
        private Excel _Excel;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Excel Excel
        {
            set
            {
                if (_Excel != value)
                {
                    _Excel = value;

                    raiseSheetChanged = false;
                    
                    sheetComboBox.DataSource = _Excel?.GetSheets();

                    ResetSheet();
                    
                    raiseSheetChanged = true;

                    ExcelChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Excel;
        }
        public event EventHandler ExcelChanged;

        private bool _ReadOnly;

        [DefaultValue(false)]
        public bool ReadOnly
        {
            set
            {
                if (_ReadOnly != value)
                {
                    _ReadOnly = value;

                    sheetTextBox.Visible = !_ReadOnly;
                    renameSheetButton.Visible = !_ReadOnly;
                    addSheetButton.Visible = !_ReadOnly;
                    removeSheetButton.Visible = !_ReadOnly;

                    ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _ReadOnly;
        }
        public event EventHandler ReadOnlyChanged;

        private bool raiseSheetChanged;

        public ExcelSheetForm()
        {
            InitializeComponent();

            raiseSheetChanged = true;
        }

        private void ResetSheet()
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            if (_Excel != null)
            {
                var sheetName = (string)sheetComboBox.SelectedItem;

                if (sheetName != null && _Excel.OpenSheet(sheetName, false))
                {
                    sheetTextBox.Text = sheetName;

                    sheetTextBox.ReadOnly = false;

                    var cells = _Excel.GetCells();

                    if (cells.Length > 0 && cells[0].Length > 0)
                    {
                        for (int c = 0; c < cells[0].Length; c++)
                        {
                            var columnName = (c + 1).ToString();

                            dataGridView.Columns.Add(columnName, columnName);
                        }
                        dataGridView.Rows.Add(cells.GetLength(0));

                        for (var r = 0; r < cells.Length; r++)
                        {
                            for (var c = 0; c < cells[r].Length; c++)
                            {
                                dataGridView.Rows[r].Cells[c].Value = cells[r][c];
                            }
                        }
                    }
                }
                else
                {
                    sheetTextBox.Text = null;
                }
            }
            else
            {
                sheetTextBox.Text = null;
            }
        }

        private void SheetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (raiseSheetChanged)
            {
                ResetSheet();
            }
        }

        private void RenameSheetButton_Click(object sender, EventArgs e)
        {
            if (_Excel != null && !_ReadOnly)
            {
                var sheetName = sheetTextBox.Text.Trim();

                if (sheetName != string.Empty)
                {
                    _Excel.RenameSheet(sheetName);

                    raiseSheetChanged = false;

                    sheetComboBox.DataSource = _Excel.GetSheets();

                    sheetComboBox.SelectedItem = sheetName;

                    raiseSheetChanged = true;
                }
            }
        }

        private void AddSheetButton_Click(object sender, EventArgs e)
        {
            if (_Excel != null && !_ReadOnly)
            {
                var sheetName = sheetTextBox.Text.Trim();

                if (sheetName != string.Empty)
                {
                    if (_Excel.OpenSheet(sheetName, true))
                    {
                        raiseSheetChanged = false;

                        sheetComboBox.DataSource = _Excel.GetSheets();

                        sheetComboBox.SelectedItem = sheetName;

                        ResetSheet();

                        raiseSheetChanged = true;
                    }
                }
            }
        }

        private void RemoveSheetButton_Click(object sender, EventArgs e)
        {
            if (_Excel != null && !_ReadOnly)
            {
                if (_Excel.RemoveSheet())
                {
                    sheetComboBox.DataSource = _Excel.GetSheets();
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
