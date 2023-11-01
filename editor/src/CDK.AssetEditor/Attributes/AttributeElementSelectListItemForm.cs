using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    partial class AttributeElementSelectListItemForm : Form
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
                    if (_Element != null)
                    {
                        _Element.PropertyChanged -= Element_PropertyChanged;
                    }
                    _Element = value;
                    if (_Element != null)
                    {
                        _Element.PropertyChanged += Element_PropertyChanged;
                    }
                    ResetItems();
                }
            }
            get => _Element;
        }

        private bool _raiseValueChanged;

        public AttributeElementSelectListItemForm()
        {
            InitializeComponent();

            Disposed += AttributeElementSelectListItemForm_Disposed;
        }

        private void AttributeElementSelectListItemForm_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.PropertyChanged -= Element_PropertyChanged;
            }
        }


        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Items":
                    ResetItems();
                    break;
                case "Value":
                    if (_raiseValueChanged) ResetValue();
                    break;
            }
        }

        private void ResetItems()
        {
            checkedListBox.Items.Clear();

            if (_Element != null && _Element.Items != null)
            {
                var values = _Element.Value.Split(',');

                foreach (var item in _Element.Items)
                {
                    var isChecked = false;

                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            if (value == item.Name)
                            {
                                isChecked = true;
                                break;
                            }
                        }
                    }

                    checkedListBox.Items.Add(item, isChecked);
                }
            }
        }

        private void ResetValue()
        {
            var values = _Element.Value.Split(',');

            for (var i = 0; i < checkedListBox.Items.Count; i++)
            {
                var item = (AttributeElementItem)checkedListBox.Items[i];

                checkedListBox.SetItemChecked(i, values != null && Array.IndexOf(values, item.Name) != -1);
            }
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _raiseValueChanged = false;

                var str = new StringBuilder();

                foreach (AttributeElementItem item in checkedListBox.CheckedItems)
                {
                    if (str.Length != 0) str.Append(',');
                    str.Append(item.Name);
                }
                _Element.Value = str.ToString();

                _raiseValueChanged = true;

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
