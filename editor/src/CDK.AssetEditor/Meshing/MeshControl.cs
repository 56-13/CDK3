using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Meshing
{
    public partial class MeshControl : UserControl
    {
        private MeshObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MeshObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        scaleControl.DataBindings.Clear();
                        axisComboBox.DataBindings.Clear();
                        flipUVCheckBox.DataBindings.Clear();
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        scaleControl.DataBindings.Add("Value", _Object.Asset, "Scale", false, DataSourceUpdateMode.OnPropertyChanged);
                        axisComboBox.DataBindings.Add("SelectedItem", _Object.Asset, "Axis", false, DataSourceUpdateMode.OnPropertyChanged);
                        flipUVCheckBox.DataBindings.Add("Checked", _Object.Asset, "FlipUV", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    selectionControl.Selection = _Object?.Selection;

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        public MeshControl()
        {
            InitializeComponent();

            axisComboBox.DataSource = Enum.GetValues(typeof(MeshAxis));
        }

        private void SelectionControl_SizeChanged(object sender, EventArgs e)
        {
            var action = (Action)(() => { Height = selectionControl.Bottom; });
            //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
            //스크롤바에 추가되는 컨트롤에만 이 코드 추가

            if (IsHandleCreated) BeginInvoke(action);
            else action.Invoke();
        }
    }
}
