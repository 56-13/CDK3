using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Attributes
{
    public partial class AttributeElementSelectControl : UserControl
    {
        private AttributeElement _SelectedElement;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AttributeElement SelectedElement
        {
            set
            {
                if (_SelectedElement != value)
                {
                    if (_SelectedElement != null)
                    {
                        _SelectedElement.PropertyChanged -= SelectedElement_PropertyChanged;
                        _SelectedElement.Owner.PropertyChanged -= SelectedElement_Owner_PropertyChanged;
                    }

                    _SelectedElement = value;

                    if (_SelectedElement != null)
                    {
                        ResetTextBox();

                        _SelectedElement.PropertyChanged += SelectedElement_PropertyChanged;
                        _SelectedElement.Owner.PropertyChanged += SelectedElement_Owner_PropertyChanged;
                    }
                    else textBox.Text = null;

                    SelectedElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SelectedElement;
        }

        public event EventHandler SelectedElementChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => _selectForm.RootAsset = value;
            get => _selectForm.RootAsset;
        }

        [DefaultValue(-1)]
        public int Depth
        {
            set => _selectForm.Depth = value;
            get => _selectForm.Depth;
        }

        private AttributeElementSelectForm _selectForm;

        public AttributeElementSelectControl()
        {
            InitializeComponent();

            _selectForm = new AttributeElementSelectForm();

            Disposed += AttributeElementSelectControl_Disposed;
        }

        private void AttributeElementSelectControl_Disposed(object sender, EventArgs e)
        {
            _selectForm.Dispose();

            if (_SelectedElement != null)
            {
                _SelectedElement.PropertyChanged -= SelectedElement_PropertyChanged;
                _SelectedElement.Owner.PropertyChanged -= SelectedElement_Owner_PropertyChanged;
            }
        }

        private void SelectedElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name") ResetTextBox();
        }

        private void SelectedElement_Owner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TagName") ResetTextBox();
        }

        private void ResetTextBox()
        {
            textBox.Text = $"{_SelectedElement.Owner.TagName}.{_SelectedElement.Parent.ParentPropertyName}.{_SelectedElement.Name}";
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            _selectForm.SelectedElement = _SelectedElement;

            if (_selectForm.ShowDialog(this) == DialogResult.OK)
            {
                var element = _selectForm.SelectedElement;

                if (element != null) SelectedElement = element;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            SelectedElement = null;
        }
    }
}
