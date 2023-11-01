using System;
using System.Windows.Forms;

namespace CDK.Assets
{
    public partial class ProgressForm : Form
    {
        public int Maximum
        {
            set => progressBar.Maximum = value;
            get => progressBar.Maximum;
        }

        public int Progress
        {
            set => progressBar.Value = value;
            get => progressBar.Value;
        }

        private const int MaxMessageLength = 60;

        public string Message
        {
            set
            {
                if (value.Length > MaxMessageLength)
                {
                    value = "..." + value.Substring(value.Length - MaxMessageLength);
                }
                label.Text = value;
            }
            get => label.Text;
        }


        public ProgressForm()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            AssetManager.Instance.CancelProgress();
        }
    }
}
