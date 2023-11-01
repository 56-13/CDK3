using System;
using System.Windows.Forms;
using System.IO;

namespace CDK.Assets
{
    public partial class NewProjectForm : Form
    {
        public string ProjectName => nameTextBox.Text.Trim();
        public string ProjectPath => pathTextBox.Text.Trim();

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                pathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var name = ProjectName;
            var path = ProjectPath;

            if (name.Length != 0 && path.Length != 0)
            {
                var filepath = path + "\\" + name + ".xml";

                if (File.Exists(filepath))
                {
                    MessageBox.Show(this, "이미 파일이 존재합니다.");

                }
                else
                {
                    DialogResult = DialogResult.OK;

                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void NewProjectForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;

                Close();
            }
        }
    }
}
