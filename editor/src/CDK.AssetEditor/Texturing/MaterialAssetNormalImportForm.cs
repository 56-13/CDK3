using System;
using System.Windows.Forms;

namespace CDK.Assets.Texturing
{
    public partial class MaterialAssetNormalImportForm : Form
    {
        public string NormalPath
        {
            set => normalPathTextBox.Text = value;
            get => normalPathTextBox.Text != string.Empty ? normalPathTextBox.Text : null;
        }
        public string DisplacementPath
        {
            set => displacementPathTextBox.Text = value;
            get => displacementPathTextBox.Text != string.Empty ? displacementPathTextBox.Text : null;
        }

        public MaterialAssetNormalImportForm()
        {
            InitializeComponent();

            openFileDialog.Filter = FileFilters.Image;
        }

        private void NormalImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                NormalPath = openFileDialog.FileName;
            }
        }

        private void NormalClearButton_Click(object sender, EventArgs e)
        {
            NormalPath = null;
        }

        private void DisplacementImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                DisplacementPath = openFileDialog.FileName;
            }
        }

        private void DisplacementClearButton_Click(object sender, EventArgs e)
        {
            DisplacementPath = null;
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
