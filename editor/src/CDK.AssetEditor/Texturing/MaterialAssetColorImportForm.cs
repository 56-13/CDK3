using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    public partial class MaterialAssetColorImportForm : Form
    {
        public string ColorPath
        {
            set => colorPathTextBox.Text = value;
            get => colorPathTextBox.Text != string.Empty ? colorPathTextBox.Text : null;
        }

        public string OpacityPath
        {
            set => opacityPathTextBox.Text = value;
            get => opacityPathTextBox.Text != string.Empty ? opacityPathTextBox.Text : null;
        }

        public MaterialAssetColorImportForm()
        {
            InitializeComponent();

            openFileDialog.Filter = FileFilters.Image;
        }

        private void ColorImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ColorPath = openFileDialog.FileName;
            }
        }

        private void ColorClearButton_Click(object sender, EventArgs e)
        {
            ColorPath = null;
        }

        private void OpacityImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                OpacityPath = openFileDialog.FileName;
            }
        }

        private void OpacityClearButton_Click(object sender, EventArgs e)
        {
            OpacityPath = null;
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
