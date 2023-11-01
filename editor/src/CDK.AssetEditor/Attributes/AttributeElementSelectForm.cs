using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    public partial class AttributeElementSelectForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => treeView.RootAsset = value;
            get => treeView.RootAsset;
        }

        [DefaultValue(-1)]
        public int Depth
        {
            set => treeView.Depth = value;
            get => treeView.Depth;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AttributeElement SelectedElement
        {
            set
            {
                if (value != elementListBox.SelectedItem)
                {
                    if (value != null)
                    {
                        treeView.SelectedAsset = value.Owner;

                        elementListBox.SelectedItem = value;
                    }
                    else treeView.SelectedAsset = null;

                    SelectedElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => (AttributeElement)elementListBox.SelectedItem;
        }
        public event EventHandler SelectedElementChanged;

        public AttributeElementSelectForm()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedAssetChanged(object sender, EventArgs e)
        {
            var asset = (AttributeAsset)treeView.SelectedAsset;

            if (asset != null)
            {
                elementListBox.DataSource = asset.Attribute.Elements;
                elementListBox.DisplayMember = "NameValue";
            }
            else elementListBox.DataSource = null;
        }

        private void ElementListBox_DoubleClick(object sender, EventArgs e)
        {
            OkButton_Click(this, EventArgs.Empty);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (elementListBox.SelectedItem != null)
            {
                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
