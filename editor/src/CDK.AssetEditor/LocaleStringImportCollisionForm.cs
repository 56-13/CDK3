using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using CDK.Assets.Support;

namespace CDK.Assets
{
    public partial class LocaleStringImportCollisionForm : Form
    {
        private List<LocaleString.ImportCollision> collisions;

        public LocaleStringImportCollisionForm(List<LocaleString.ImportCollision> collisions)
        {
            this.collisions = collisions;

            InitializeComponent();

            localeExportFileDialog.Filter = FileFilters.ExcelOrCsv;

            foreach (var str in collisions)
            {
                var item = new ListViewItem
                {
                    Tag = str,
                    Text = str.Key
                };
                item.SubItems.Add(str.Origin);
                item.SubItems.Add(str.Value);

                listView.Items.Add(item);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (localeExportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeExportFileDialog.FileName;

                using (var fs = new FileStream(path, FileMode.Create))
                {
                    using (var writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        foreach (var collision in collisions)
                        {
                            writer.WriteLine(CSV.Make(new string[] { collision.Key, collision.Origin, collision.Value }));
                        }
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void IgnoreButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "충돌을 무시하고 변환을 진행하시겠습니까?", "무시", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Ignore;

                Close();
            }
        }
    }
}
