using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using CDK.Assets.Components;
using CDK.Assets.Texturing;

namespace CDK.Assets.Meshing
{
    public partial class MeshSelectionControl : UserControl
    {
        private MeshSelection _Selection;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MeshSelection Selection
        {
            set
            {
                if (_Selection != value)
                {
                    if (_Selection != null)
                    {
                        assetControl.DataBindings.Clear();
                        assetControl.Enabled = false;
                        geometryComboBox.DataBindings.Clear();
                        geometryComboBox.DataSource = null;
                        animationAssetControl.DataBindings.Clear();
                        animationAssetControl.Enabled = false;
                        animationComboBox.DataBindings.Clear();
                        animationComboBox.DataSource = null;
                        frameDivisionComboBox.DataBindings.Clear();
                        collisionCheckBox.DataBindings.Clear();

                        _Selection.PropertyChanged -= Selection_PropertyChanged;
                    }

                    _Selection = value;

                    if (_Selection != null)
                    {
                        var assetEdit = !(_Selection.Parent is MeshAsset);

                        //assetControl.DataBindings.Add("RootAsset", _Selection, "Project", true, DataSourceUpdateMode.Never);
                        assetControl.DataBindings.Add("SelectedAsset", _Selection, "Asset", true, DataSourceUpdateMode.OnPropertyChanged);
                        assetControl.Enabled = assetEdit;

                        geometryComboBox.DataSource = _Selection.Asset?.Geometries;
                        geometryComboBox.DisplayMember = "Name";
                        geometryComboBox.DataBindings.Add(new NullableComboBoxBinding(_Selection, "Geometry"));

                       // animationAssetControl.DataBindings.Add("RootAsset", _Selection, "Project", true, DataSourceUpdateMode.Never);
                        animationAssetControl.DataBindings.Add("SelectedAsset", _Selection, "AnimationAsset", true, DataSourceUpdateMode.OnPropertyChanged);
                        animationAssetControl.Enabled = assetEdit;

                        animationComboBox.DataSource = _Selection.AnimationAsset?.Animations;
                        animationComboBox.DisplayMember = "Name";
                        animationComboBox.DataBindings.Add(new NullableComboBoxBinding(_Selection, "Animation"));
                        frameDivisionComboBox.DataBindings.Add("SelectedItem", _Selection, "FrameDivision", false, DataSourceUpdateMode.OnPropertyChanged);
                        collisionCheckBox.DataBindings.Add("Checked", _Selection, "Collision", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Selection.PropertyChanged += Selection_PropertyChanged;
                    }

                    ResetMaterials();

                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Selection;
        }

        public event EventHandler SelectionChanged;

        private List<Control> _materialControls;

        private MaterialForm _materialForm;

        public MeshSelectionControl()
        {
            InitializeComponent();

            _materialControls = new List<Control>();

            frameDivisionComboBox.Items.Add(0);
            frameDivisionComboBox.Items.Add(10);
            frameDivisionComboBox.Items.Add(15);
            frameDivisionComboBox.Items.Add(20);
            frameDivisionComboBox.Items.Add(30);
            frameDivisionComboBox.Items.Add(60);

            _materialForm = new MaterialForm();
            _materialForm.FormClosing += MaterialForm_FormClosing;

            Disposed += MeshSelectionControl_Disposed;
        }

        private void MeshSelectionControl_Disposed(object sender, EventArgs e)
        {
            _materialForm.Dispose();

            if (_Selection != null)
            {
                _Selection.PropertyChanged -= Selection_PropertyChanged;
            }
        }

        private void MaterialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _materialForm.Material = null;

            if (e.CloseReason == CloseReason.UserClosing)
            {
                _materialForm.Hide();
                e.Cancel = true;
            }
        }

        private void ResetMaterials()
        {
            panel.SuspendLayout();

            foreach (var control in _materialControls) control.Dispose();
            _materialControls.Clear();

            var geometry = _Selection?.Geometry;

            if (geometry != null)
            {
                var y = geometryPanel.Bottom + geometryPanel.Margin.Bottom;
                var i = 1;

                foreach (var material in _Selection.Materials)
                {
                    var materialControl = new MeshSelectionMaterialControl
                    {
                        Material = material,
                        Margin = new Padding(0, 3, 0, 3),
                        Location = new Point(0, y)
                    };
                    materialControl.MaterialEdit += MaterialControl_MaterialEdit;

                    panel.Controls.Add(materialControl);
                    panel.Controls.SetChildIndex(materialControl, i++);

                    _materialControls.Add(materialControl);

                    y += materialControl.Height + 3;
                }
            }
            panel.ResumeLayout();
        }

        private void MaterialControl_MaterialEdit(object sender, EventArgs e)
        {
            var material = ((MeshSelectionMaterialControl)sender).Material;

            _materialForm.Material = material;

            if (!_materialForm.Visible) _materialForm.Show();
        }

        private void Selection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Asset":
                    geometryComboBox.DataSource = _Selection.Asset?.Geometries;
                    geometryComboBox.DisplayMember = "Name";
                    break;
                case "Geometry":
                    ResetMaterials();
                    break;
                case "AnimationAsset":
                    animationComboBox.DataSource = _Selection.AnimationAsset?.Animations;
                    animationComboBox.DisplayMember = "Name";
                    break;
            }
        }

        private void AnimationClearButton_Click(object sender, EventArgs e)
        {
            if (_Selection != null) _Selection.Animation = null;
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            var action = (Action)(() => { Height = panel.Height; });
            //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
            //스크롤바에 추가되는 컨트롤에만 이 코드 추가

            if (IsHandleCreated) BeginInvoke(action);
            else action.Invoke();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!Visible && _materialForm.Visible) _materialForm.Hide();
        }
    }
}
