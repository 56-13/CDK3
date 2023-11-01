using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Meshing
{
    public partial class MeshColliderControl : UserControl
    {

        private MeshCollider _Collider;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MeshCollider Collider
        {
            set
            {
                if (_Collider != value)
                {
                    if (_Collider != null)
                    {
                        positionControl.DataBindings.Clear();
                        rotationControl.DataBindings.Clear();
                        nodeNameComboBox.DataBindings.Clear();
                        nodeNameComboBox.DataSource = null;
                        relativeCheckBox.DataBindings.Clear();
                        inclusiveCheckBox.DataBindings.Clear();
                        shapeComboBox.DataBindings.Clear();
                        radius0Control.DataBindings.Clear();
                        radius1Control.DataBindings.Clear();
                        radius2Control.DataBindings.Clear();
                    }

                    _Collider = value;

                    if (_Collider != null)
                    {
                        positionControl.DataBindings.Add("Value", _Collider, "Position", false, DataSourceUpdateMode.OnPropertyChanged);
                        rotationControl.DataBindings.Add("Quaternion", _Collider, "Rotation", false, DataSourceUpdateMode.OnPropertyChanged);

                        nodeNameComboBox.DataSource = _Collider.Parent.Origin.GetBoneNames();
                        nodeNameComboBox.DataBindings.Add(new NullableComboBoxBinding(_Collider, "NodeName"));

                        relativeCheckBox.DataBindings.Add("Checked", _Collider, "Relative", false, DataSourceUpdateMode.OnPropertyChanged);
                        inclusiveCheckBox.DataBindings.Add("Checked", _Collider, "Inclusive", false, DataSourceUpdateMode.OnPropertyChanged);
                        shapeComboBox.DataBindings.Add("SelectedItem", _Collider, "Shape", false, DataSourceUpdateMode.OnPropertyChanged);
                        radius0Control.DataBindings.Add("Value", _Collider, "Radius0", false, DataSourceUpdateMode.OnPropertyChanged);
                        radius1Control.DataBindings.Add("Value", _Collider, "Radius1", false, DataSourceUpdateMode.OnPropertyChanged);
                        radius2Control.DataBindings.Add("Value", _Collider, "Radius2", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ColliderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Collider;
        }

        public event EventHandler ColliderChanged;

        public MeshColliderControl()
        {
            InitializeComponent();

            shapeComboBox.DataSource = Enum.GetValues(typeof(ColliderShape));
        }

        private void NodeNameComboBoxBinding_Format(object sender, ConvertEventArgs e)
        {
            var nodeName = (string)e.Value;

            e.Value = nodeName == null ? 0 : nodeNameComboBox.Items.IndexOf(nodeName);
        }

        private void NodeNameComboBoxBinding_Parse(object sender, ConvertEventArgs e)
        {
            var index = (int)e.Value;

            e.Value = index <= 0 ? null : nodeNameComboBox.Items[index];
        }

        private void NodeNameClearButton_Click(object sender, EventArgs e)
        {
            if (_Collider != null) _Collider.NodeName = null;
        }

        private void ShapeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_Collider != null)
            {
                switch (_Collider.Shape)
                {
                    case ColliderShape.Box:
                        radius1Control.Visible = true;
                        radius2Control.Visible = true;
                        break;
                    case ColliderShape.Sphere:
                        radius1Control.Visible = false;
                        radius2Control.Visible = false;
                        break;
                    case ColliderShape.Capsule:
                        radius1Control.Visible = true;
                        radius2Control.Visible = false;
                        break;
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            _Collider?.Reset();
        }
    }
}
