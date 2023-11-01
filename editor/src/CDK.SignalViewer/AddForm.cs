using System;
using System.Windows.Forms;

namespace CDK.SignalViewer
{
    public partial class AddForm : Form
    {
        public new string Text
        {
            set => textBox.Text = value;
            get => textBox.Text;
        }

        public AddForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
