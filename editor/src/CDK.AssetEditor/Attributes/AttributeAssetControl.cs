using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

using CDK.Assets.Support;

namespace CDK.Assets.Attributes
{
    public partial class AttributeAssetControl : UserControl
    {
        private AttributeAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AttributeAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        buildCheckBox.DataBindings.Clear();
                    }
                    _Asset = value;

                    if (_Asset != null)
                    {
                        buildCheckBox.DataBindings.Add("Enabled", AssetManager.Instance, "IsDeveloper", false, DataSourceUpdateMode.Never);
                        buildCheckBox.DataBindings.Add("Checked", _Asset, "BuildChecked", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    attributeControl.Attribute = _Asset?.Attribute;

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }
        public event EventHandler AssetChanged;

        public AttributeAssetControl()
        {
            InitializeComponent();

            exportFileDialog.Filter = importFileDialog.Filter = FileFilters.ExcelOrCsv;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                try
                {
                    using (var exportForm = new AttributeExportForm())
                    {
                        exportForm.Asset = _Asset;

                        if (exportForm.ShowDialog(this) == DialogResult.OK && exportFileDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            var path = exportFileDialog.FileName;

                            var cells = _Asset.ExportToCells(exportForm.ElementNames);

                            if (path.EndsWith(".xlsx"))
                            {
                                using (var excel = new Excel(path))
                                {
                                    using (var sheetForm = new ExcelSheetForm())
                                    {
                                        sheetForm.Excel = excel;

                                        if (sheetForm.ShowDialog(this) == DialogResult.OK)
                                        {
                                            for (var r = 0; r < cells.Length; r++)
                                            {
                                                for (var c = 0; c < cells[r].Length; c++)
                                                {
                                                    excel.SetCell(r, c, cells[r][c]);
                                                }
                                            }
                                            excel.Save();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var fs = new FileStream(path, FileMode.Create))
                                {
                                    using (var writer = new StreamWriter(fs, Encoding.UTF8))
                                    {
                                        foreach (var row in cells) writer.WriteLine(CSV.Make(row));
                                    }
                                }
                            }
                            MessageBox.Show(this, "추출이 완료되었습니다.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Record(ex);

                    MessageBox.Show(this, "추출을 진행할 수 없습니다.");
                }
            }
        }

        private void Importbutton_Click(object sender, EventArgs e)
        {
            if (_Asset != null && importFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = importFileDialog.FileName;

                try
                {
                    string[][] cells;

                    if (path.EndsWith(".xlsx"))
                    {
                        using (var excel = new Excel(path))
                        {
                            using (var form = new ExcelSheetForm())
                            {
                                form.Excel = excel;
                                form.ReadOnly = true;

                                if (form.ShowDialog(this) == DialogResult.OK) cells = excel.GetCells();
                                else return;
                            }
                        }
                    }
                    else
                    {
                        var csv = File.ReadAllText(path, FileEncoding.GetFileEncoding(path));

                        cells = CSV.ParseAll(csv);
                    }

                    _Asset.ImportFromCells(cells);

                    MessageBox.Show(this, "변환이 완료되었습니다.");
                }
                catch (Exception ex)
                {
                    ErrorHandler.Record(ex);

                    MessageBox.Show(this, "변환을 진행할 수 없습니다.");
                }
            }
        }
    }
}
