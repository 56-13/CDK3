using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using CDK.Assets.Attributes;
using CDK.Assets.Spawn;

namespace CDK.Assets.Spawn
{
    public partial class SpawnControl : UserControl
    {
        private SpawnObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpawnObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        dataGridView.DataSource = null;

                        _Object.PropertyChanged -= Object_PropertyChanged;
                        _Object.Attribute.PropertyChanged -= Attribute_PropertyChanged;
                        _Object.Asset.Attribute.Elements.ListChanged -= Elements_ListChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        var attr = _Object.Attribute;
                        var elements = attr.Asset.Attribute.Elements;

                        dataGridView.DataSource = elements;

                        for (var i = 0; i < dataGridView.RowCount; i++)
                        {
                            var element = elements[i];
                            var cell = dataGridView.Rows[i].Cells[1];

                            cell.Value = attr.GetValue(element.Key);
                            cell.ReadOnly = element.Items != null;
                        }

                        dataGridView.Visible = elements.Count != 0;

                        _Object.PropertyChanged += Object_PropertyChanged;
                        _Object.Attribute.PropertyChanged += Attribute_PropertyChanged;
                        _Object.Asset.Attribute.Elements.ListChanged += Elements_ListChanged;
                    }

                    ResetSubControl();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private Control _subControl;
        private ComboBox _valueComboBox;
        private int _valueComboBoxRow;

        public SpawnControl()
        {
            InitializeComponent();

            dataGridView.AutoGenerateColumns = false;

            _valueComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Disposed += SpawnObjectControl_Disposed;
        }

        private void SpawnObjectControl_Disposed(object sender, EventArgs e)
        {
            _valueComboBox.Dispose();

            if (_Object != null)
            {
                if (_subControl != null)
                {
                    _subControl.SizeChanged -= SubControl_SizeChanged;
                    Controls.Remove(_subControl);
                }

                _Object.PropertyChanged -= Object_PropertyChanged;
                _Object.Attribute.PropertyChanged -= Attribute_PropertyChanged;
                _Object.Asset.Attribute.Elements.ListChanged -= Elements_ListChanged;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View") ResetSubControl();
        }

        private void Attribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var key = e.PropertyName;
            var element = _Object.Asset.Attribute.Elements.FirstOrDefault(ae => ae.Key == key);
            if (element != null) dataGridView.Rows[element.Index].Cells[1].Value = _Object.Attribute.GetValue(key);
        }

        private void ResetSubControl()
        {
            SuspendLayout();

            if (_subControl != null)
            {
                _subControl.SizeChanged -= SubControl_SizeChanged;
                Controls.Remove(_subControl);
                _subControl = null;
            }

            var obj = _Object?.ViewObject;

            //_subControl = obj != null ? AssetControl.Instance.GetSceneObjectReferenceControl(obj) : null;
            //TODO

            if (_subControl != null)
            {
                _subControl.Location = Point.Empty;
                _subControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                dataGridView.Top = _subControl.Height + _subControl.Margin.Bottom + dataGridView.Margin.Top;
                Height = dataGridView.Bottom;

                _subControl.Width = Width;
                _subControl.SizeChanged += SubControl_SizeChanged;
                Controls.Add(_subControl);
            }

            ResumeLayout();
        }

        private void SubControl_SizeChanged(object sender, EventArgs e)
        {
            dataGridView.Top = _subControl.Bottom + _subControl.Margin.Bottom + dataGridView.Margin.Top;
            Height = dataGridView.Bottom;
        }

        private void Elements_ListChanged(object sender, ListChangedEventArgs e)
        {
            var attr = _Object.Attribute;
            var elements = attr.Asset.Attribute.Elements;

            if (e.PropertyDescriptor != null)
            {
                if (e.PropertyDescriptor.Name == "Items")
                {
                    var element = elements[e.NewIndex];
                    var cell = dataGridView.Rows[e.NewIndex].Cells[1];

                    cell.ReadOnly = element.Items != null;
                }
            }
            else dataGridView.Visible = elements.Count != 0;
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_Object != null)
            {
                var attr = _Object.Attribute;
                var element = attr.Asset.Attribute.Elements[e.RowIndex];
                var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                attr.SetValue(element.Key, (string)cell.Value);
                cell.Value = attr.GetValue(element.Key);
            }
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_Object != null && e.ColumnIndex == 1)
            {
                var attr = _Object.Attribute;
                var element = attr.Asset.Attribute.Elements[e.RowIndex];

                if (element.Items != null)
                {
                    if (element.ListType != AttributeElementListType.None)
                    {
                        using (var form = new AttributeElementSelectListItemForm())
                        {
                            form.Element = element;

                            form.ShowDialog(this);
                        }
                    }
                    else
                    {
                        Rectangle cellRectangle = dataGridView.GetCellDisplayRectangle(1, e.RowIndex, false);     //VS2008 ZERO를 리턴할 때가 있음

                        //VS2008 ZERO를 리턴할 때가 있음. 현재는 괜찮은 것으로 보이고 코드 남김
                        /*
                        var x = dataGridView.RowHeadersWidth + dataGridView.Columns[0].Width;
                        var y = dataGridView.ColumnHeadersHeight;
                        for (var i = 0; i < e.RowIndex; i++)
                        {
                            y += dataGridView.Rows[i].Height;
                        }
                        var w = dataGridView.Columns[1].Width;
                        var h = dataGridView.Rows[e.RowIndex].Height;

                        var cellRectangle = new Rectangle(x, y, w, h);
                        */
                        _valueComboBoxRow = e.RowIndex;

                        _valueComboBox.Bounds = cellRectangle;
                        _valueComboBox.DataSource = element.Items;
                        _valueComboBox.SelectedItem = element.Items.FirstOrDefault(item => item.Name == element.Value);
                        Controls.Add(_valueComboBox);
                        _valueComboBox.BringToFront();
                        _valueComboBox.Focus();
                        _valueComboBox.SelectedIndexChanged += ValueComboBox_SelectedIndexChanged;
                        _valueComboBox.PreviewKeyDown += ValueComboBox_PreviewKeyDown;
                        _valueComboBox.LostFocus += ValueComboBox_LostFocus;

                        dataGridView.Enabled = false;
                    }
                }
            }
        }

        private void HideComboBox()
        {
            _valueComboBox.SelectedIndexChanged -= ValueComboBox_SelectedIndexChanged;
            _valueComboBox.PreviewKeyDown -= ValueComboBox_PreviewKeyDown;
            _valueComboBox.LostFocus -= ValueComboBox_LostFocus;
            _valueComboBox.DataSource = null;
            Controls.Remove(_valueComboBox);

            dataGridView.Enabled = true;
        }

        private void ValueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var attr = _Object.Attribute;
                var element = attr.Asset.Attribute.Elements[_valueComboBoxRow];

                var item = (AttributeElementItem)_valueComboBox.SelectedItem;

                if (item != null)
                {
                    attr.SetValue(element.Key, item.Name);
                }
            }

            HideComboBox();
        }

        private void ValueComboBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                HideComboBox();
                e.IsInputKey = true;
            }
        }

        private void ValueComboBox_LostFocus(object sender, EventArgs e)
        {
            HideComboBox();
        }
    }
}
