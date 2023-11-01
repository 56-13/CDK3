using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Scenes
{
    public partial class DirectionalLightControl : UserControl
    {
        private DirectionalLightObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DirectionalLightObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        colorControl.DataBindings.Clear();
                        castShadowCheckBox.DataBindings.Clear();
                        castShadow2DCheckBox.DataBindings.Clear();
                        shadowControl.DataBindings.Clear();
                        shadowControl.Shadow = null;
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        colorControl.DataBindings.Add("Value3", _Object, "Color", false, DataSourceUpdateMode.OnPropertyChanged);
                        castShadowCheckBox.DataBindings.Add("Checked", _Object, "CastShadow", false, DataSourceUpdateMode.OnPropertyChanged);
                        castShadow2DCheckBox.DataBindings.Add("Checked", _Object, "CastShadow2D", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowControl.DataBindings.Add("Visible", _Object, "CastShadow", false, DataSourceUpdateMode.Never);
                        shadowControl.Shadow = _Object.Shadow;
                    }

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        public DirectionalLightControl()
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
