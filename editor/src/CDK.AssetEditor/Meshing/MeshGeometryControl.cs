using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Meshing
{
    public partial class MeshGeometryControl : UserControl
    {
        private MeshGeometryComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MeshGeometryComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    panel.SuspendLayout();

                    if (_Object != null)
                    {
                        foreach (Control materialConfigControl in materialConfigsPanel.Controls) materialConfigControl.Dispose();

                        colliderListBox.DataBindings.Clear();
                        colliderListBox.DataSource = null;
                        colliderControl.DataBindings.Clear();
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        vertexCountUpDown.Value = _Object.Geometry.Origin.VertexCount;
                        boneCountUpDown.Value = _Object.Geometry.Origin.BoneCount;

                        var y = 24;
                        foreach (var materialConfig in _Object.Geometry.MaterialConfigs)
                        {
                            var materialConfigControl = new MaterialConfigControl
                            {
                                Config = materialConfig,
                                Location = new System.Drawing.Point(0, y)
                            };
                            materialConfigsPanel.Controls.Add(materialConfigControl);
                            y += materialConfigControl.Height;
                        }

                        colliderListBox.DataSource = _Object.Geometry.Colliders;
                        colliderListBox.DisplayMember = "Name";
                        colliderListBox.DataBindings.Add("SelectedItem", _Object, "SelectedCollider", true, DataSourceUpdateMode.OnPropertyChanged);

                        colliderControl.DataBindings.Add("Collider", _Object, "SelectedCollider", true, DataSourceUpdateMode.OnPropertyChanged);
                        var subControlVisibleBinding = new Binding("Visible", _Object, "SelectedCollider", true, DataSourceUpdateMode.Never);
                        subControlVisibleBinding.Format += (s, e) =>
                        {
                            e.Value = ((MeshCollider)e.Value != null);
                        };
                        colliderControl.DataBindings.Add(subControlVisibleBinding);
                    }
                    else
                    {
                        colliderControl.Visible = false;
                    }

                    panel.ResumeLayout();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        public MeshGeometryControl()
        {
            InitializeComponent();
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                _Object.Geometry.Colliders.Add(new MeshCollider(_Object.Geometry));
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null && colliderListBox.SelectedIndex >= 0)
            {
                _Object.Geometry.Colliders.RemoveAt(colliderListBox.SelectedIndex);
            }
        }

        private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var index = colliderListBox.SelectedIndex;

                if (index > 0) _Object.Geometry.Colliders.Move(index, index - 1);
            }
        }

        private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var index = colliderListBox.SelectedIndex;

                if (index >= 0 && index < _Object.Geometry.Colliders.Count - 1) _Object.Geometry.Colliders.Move(index, index + 1);
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            var action = (Action)(() => { Height = panel.Height; });
            //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
            //스크롤바에 추가되는 컨트롤에만 이 코드 추가

            if (IsHandleCreated) BeginInvoke(action);
            else action.Invoke();
        }
    }
}
