using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Scenes
{
    public partial class SceneObjectControl : UserControl
    {
        private SceneObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        locatedCheckBox.DataBindings.Clear();
                        transformCheckBox.DataBindings.Clear();
                        transformControl.DataBindings.Clear();
                        transformControl.Gizmo = null;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        locatedCheckBox.DataBindings.Add("Checked", _Object, "Located", false, DataSourceUpdateMode.OnPropertyChanged);
                        transformCheckBox.DataBindings.Add("Checked", _Object, "UsingTransform", false, DataSourceUpdateMode.OnPropertyChanged);
                        transformControl.DataBindings.Add("Visible", _Object, "UsingTransform", false, DataSourceUpdateMode.Never);
                        transformControl.Gizmo = _Object.Transform;
                    }

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        public SceneObjectControl()
        {
            InitializeComponent();
        }

        private void TransformControl_VisibleChanged(object sender, EventArgs e)
        {
            Height = transformControl.Visible ? transformControl.Bottom : transformControl.Top;
        }
    }
}
