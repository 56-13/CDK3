using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Scenes
{
    public partial class CameraControl : UserControl
    {
        private CameraObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CameraObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        targetCheckBox.DataBindings.Clear();
                        targetControlCheckBox.DataBindings.Clear();
                        targetControl.DataBindings.Clear();
                        targetControl.Gizmo = null;
                        frustumCheckBox.DataBindings.Clear();
                        frustumPanel.DataBindings.Clear();
                        fovControl.DataBindings.Clear();
                        nearControl.DataBindings.Clear();
                        farControl.DataBindings.Clear();
                        blurCheckBox.DataBindings.Clear();
                        blurPanel.DataBindings.Clear();
                        blurDistanceControl.DataBindings.Clear();
                        blurRangeControl.DataBindings.Clear();
                        blurIntensityControl.DataBindings.Clear();
                        focusCheckBox.DataBindings.Clear();
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        targetCheckBox.DataBindings.Add("Checked", _Object, "UsingTarget", false, DataSourceUpdateMode.OnPropertyChanged);
                        targetControlCheckBox.DataBindings.Add("Visible", _Object, "UsingTarget", false, DataSourceUpdateMode.Never);
                        targetControlCheckBox.DataBindings.Add("Checked", _Object, "TargetControl", false, DataSourceUpdateMode.OnPropertyChanged);
                        targetControl.DataBindings.Add("Visible", _Object, "UsingTarget", false, DataSourceUpdateMode.Never);
                        targetControl.Gizmo = _Object.Target;

                        frustumCheckBox.DataBindings.Add("Checked", _Object, "UsingFrustum", false, DataSourceUpdateMode.OnPropertyChanged);
                        frustumPanel.DataBindings.Add("Visible", _Object, "UsingFrustum", false, DataSourceUpdateMode.Never);
                        fovControl.DataBindings.Add("Value", _Object, "Fov", false, DataSourceUpdateMode.OnPropertyChanged);
                        nearControl.DataBindings.Add("Value", _Object, "Near", false, DataSourceUpdateMode.OnPropertyChanged);
                        farControl.DataBindings.Add("Value", _Object, "Far", false, DataSourceUpdateMode.OnPropertyChanged);

                        blurCheckBox.DataBindings.Add("Checked", _Object, "UsingBlur", false, DataSourceUpdateMode.OnPropertyChanged);
                        blurPanel.DataBindings.Add("Visible", _Object, "UsingBlur", false, DataSourceUpdateMode.Never);
                        blurDistanceControl.DataBindings.Add("Value", _Object, "BlurDistance", false, DataSourceUpdateMode.OnPropertyChanged);
                        blurRangeControl.DataBindings.Add("Value", _Object, "BlurRange", false, DataSourceUpdateMode.OnPropertyChanged);
                        blurIntensityControl.DataBindings.Add("Value", _Object, "BlurIntensity", false, DataSourceUpdateMode.OnPropertyChanged);
                        focusCheckBox.DataBindings.Add("Checked", _Object, "Focused", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;


        public CameraControl()
        {
            InitializeComponent();
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
