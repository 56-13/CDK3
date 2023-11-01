using System;
using System.Numerics;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;

namespace CDK.Assets.Scenes
{
    //TODO

    public partial class GizmoControl : UserControl
    {
        private SceneObject _GizmoObject;

        private Gizmo _Gizmo;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Gizmo Gizmo
        {
            set
            {
                if (_Gizmo != value)
                {
                    if (_Gizmo != null)
                    {
                        positionControl.DataBindings.Clear();
                        gridPositionControl.DataBindings.Clear();
                        rotationControl.DataBindings.Clear();
                        scaleControl.DataBindings.Clear();
                        fromGroundCheckBox.DataBindings.Clear();

                        _Gizmo.PropertyChanged -= Gizmo_PropertyChanged;
                    }

                    _Gizmo = value;

                    if (_Gizmo != null)
                    {
                        positionControl.DataBindings.Add("Value", _Gizmo, "Position", false, DataSourceUpdateMode.OnPropertyChanged);
                        gridPositionControl.DataBindings.Add("Value", _Gizmo, "GridPosition", false, DataSourceUpdateMode.OnPropertyChanged);
                        rotationControl.DataBindings.Add("Quaternion", _Gizmo, "Rotation", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleControl.DataBindings.Add("Value", _Gizmo, "Scale", false, DataSourceUpdateMode.OnPropertyChanged);
                        fromGroundCheckBox.DataBindings.Add("Checked", _Gizmo, "FromGround", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Gizmo.PropertyChanged += Gizmo_PropertyChanged;
                    }

                    BindObject();

                    GizmoChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Gizmo;
        }

        public event EventHandler GizmoChanged;

        private SceneComponentSelectForm _selectForm;

        public GizmoControl()
        {
            InitializeComponent();

            _selectForm = new SceneComponentSelectForm
            {
                Types = new SceneComponentType[]
                {
                    SceneComponentType.Spawn,
                    SceneComponentType.Mesh,
                    SceneComponentType.Animation
                }
            };

            Disposed += GizmoBindingControl_Disposed;
        }

        private void GizmoBindingControl_Disposed(object sender, EventArgs e)
        {
            _selectForm.Dispose();

            if (_Gizmo != null)
            {
                _Gizmo.PropertyChanged -= Gizmo_PropertyChanged;
            }
        }

        private void Gizmo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Object") BindObject();
        }

        private void BindObject()
        {
            var obj = _Gizmo?.Target;

            if (obj != _GizmoObject)
            {
                if (_GizmoObject != null)
                {
                    bindingComboBox.DataBindings.Clear();
                    bindingComboBox.DataSource = null;
                    targetTextBox.DataBindings.Clear();
                }

                _GizmoObject = obj;
                /*
                if (_GizmoObject != null)
                {
                    bindingComboBox.DataSource = _Gizmo.Target?.GetTransformNames();
                    bindingComboBox.DataBindings.Add(new NullableComboBoxBinding(_Gizmo, "NodeName"));
                    objectTextBox.DataBindings.Add("Text", _GizmoObject, "Name", false, DataSourceUpdateMode.Never);
                }
                else objectTextBox.Text = null;
                */
                bindingComboBox.Enabled = _GizmoObject != null;
                positionControl.Enabled = _GizmoObject == null;
            }

            //objectPanel.Visible = _Gizmo?.UsingObject ?? true;
            bindingPanel.Visible = objectPanel.Visible && obj != null;
            positionPanel.Visible = obj == null;
        }

        private void ObjectAddButton_Click(object sender, EventArgs e)
        {
            if (_Gizmo != null)
            {
                _selectForm.Scene = _Gizmo.GetAncestor<Scene>(false);
                _selectForm.SelectedComponent = _Gizmo.Target;

                if (_selectForm.ShowDialog(this) == DialogResult.OK)
                {
                    //_Gizmo.Target = _selectForm.SelectedComponent;
                }
            }
        }

        private void ObjectRemoveButton_Click(object sender, EventArgs e)
        {
            if (_Gizmo != null) _Gizmo.Target = null;
        }

        private void NodeNameComboBox_DropDown(object sender, EventArgs e)
        {
            var item = bindingComboBox.SelectedItem;
            //bindingComboBox.DataSource = _Gizmo?.Target?.GetTransformNames();
            bindingComboBox.SelectedItem = item;
        }

        private void NodeNameClearButton_Click(object sender, EventArgs e)
        {
            if (_Gizmo != null) _Gizmo.Binding = null;
        }

        private void ObjectTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (_Gizmo != null)
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                if (data != null)
                {
                    var obj = _Gizmo.GetAncestor<Scene>(false)?.GetObject(data);

                    if (obj != null)
                    {
                        e.Effect = DragDropEffects.Link;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void ObjectTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (_Gizmo != null)
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                if (data != null)
                {
                    var obj = _Gizmo.GetAncestor<Scene>(false)?.GetObject(data);

                    if (obj != null) _Gizmo.Target = obj;
                }
            }
        }

        private void RotationResetButton_Click(object sender, EventArgs e)
        {
            if (_Gizmo != null) _Gizmo.Rotation = Quaternion.Identity;
        }

        private void ScaleResetButton_Click(object sender, EventArgs e)
        {
            if (_Gizmo != null) _Gizmo.Scale = Vector3.One;
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }
    }
}
