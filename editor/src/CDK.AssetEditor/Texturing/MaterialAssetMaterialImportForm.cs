using System;
using System.Windows.Forms;

namespace CDK.Assets.Texturing
{
    public partial class MaterialAssetMaterialImportForm : Form
    {
        public string MetallicPath
        {
            set => metallicPathTextBox.Text = value;
            get => metallicPathTextBox.Text != string.Empty ? metallicPathTextBox.Text : null;
        }
        public string RoughnessPath
        {
            set => roughnessPathTextBox.Text = value;
            get => roughnessPathTextBox.Text != string.Empty ? roughnessPathTextBox.Text : null;
        }
        public string OcclusionPath
        {
            set => occlusionPathTextBox.Text = value;
            get => occlusionPathTextBox.Text != string.Empty ? occlusionPathTextBox.Text : null;
        }

        public MaterialAssetMaterialImportForm()
        {
            InitializeComponent();

            openFileDialog.Filter = FileFilters.Image;
        }

        private void MetallicImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                MetallicPath = openFileDialog.FileName;
            }
        }

        private void MetallicClearButton_Click(object sender, EventArgs e)
        {
            MetallicPath = null;
        }

        private void RoughnessImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                RoughnessPath = openFileDialog.FileName;
            }
        }

        private void RoughnessClearButton_Click(object sender, EventArgs e)
        {
            RoughnessPath = null;
        }

        private void OcclusionImportButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                OcclusionPath = openFileDialog.FileName;
            }
        }

        private void OcclusionClearButton_Click(object sender, EventArgs e)
        {
            OcclusionPath = null;
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
