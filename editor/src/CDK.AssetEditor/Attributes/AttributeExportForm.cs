using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    public partial class AttributeExportForm : Form
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
                    _Asset = value;

                    _raiseChecked = false;

                    checkedListBox.Items.Clear();

                    if (_Asset != null)
                    {
                        for (int i = 0; i < _Asset.Attribute.Elements.Count; i++)
                        {
                            checkedListBox.Items.Add(_Asset.Attribute.Elements[i].Name);
                            checkedListBox.SetItemChecked(i, true);
                        }
                        allCheckBox.Checked = true;
                    }
                    else
                    {
                        allCheckBox.Checked = false;
                    }

                    _raiseChecked = true;

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }
        public event EventHandler AssetChanged;

        public string[] ElementNames
        {
            get
            {
                var names = new string[checkedListBox.CheckedItems.Count];
                checkedListBox.CheckedItems.CopyTo(names, 0);
                return names;
            }
        }

        private bool _raiseChecked;

        public AttributeExportForm()
        {
            InitializeComponent();

            _raiseChecked = true;
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_raiseChecked)
            {
                _raiseChecked = false;

                var checkCount = checkedListBox.CheckedItems.Count;

                if (e.NewValue == CheckState.Checked) checkCount++;
                else checkCount--;

                CheckState state;
                if (checkCount == 0) state = CheckState.Unchecked;
                else if (checkCount == checkedListBox.Items.Count) state = CheckState.Checked;
                else state = CheckState.Indeterminate;

                allCheckBox.CheckState = state;

                _raiseChecked = true;
            }
        }

        private void AllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_raiseChecked)
            {
                _raiseChecked = false;

                if (allCheckBox.CheckState != CheckState.Indeterminate)
                {
                    var check = allCheckBox.CheckState == CheckState.Checked;

                    for (var i = 0; i < checkedListBox.Items.Count; i++)
                    {
                        checkedListBox.SetItemChecked(i, check);
                    }
                }

                _raiseChecked = true;
            }
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
