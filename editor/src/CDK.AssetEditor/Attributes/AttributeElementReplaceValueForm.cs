using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    public partial class AttributeElementReplaceValueForm : Form
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
                    _Element = value;

                    fromTextBox.Text = toTextBox.Text = _Element?.Value ?? string.Empty;
                }
            }
            get => _Element;
        }

        public AttributeElementReplaceValueForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var from = fromTextBox.Text;
            var to = toTextBox.Text;

            if (from != to)
            {
                var msg = "다른 데이터의 모든 값을 이 값으로 교체하시겠습니까?";

                if (MessageBox.Show(this, msg, string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _Element.ReplaceValue(from, to);
                }
            }
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
