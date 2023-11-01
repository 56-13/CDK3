using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Terrain
{
    public partial class TerrainSurfaceMaterialControl : UserControl, ICollapsibleControl
    {
        private TerrainSurface _Surface;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainSurface Surface
        {
            set
            {
                if (_Surface != value)
                {
                    if (_Surface != null)
                    {
                        rotationControl.DataBindings.Clear();
                        scaleControl.DataBindings.Clear();
                        triPlanerCheckBox.DataBindings.Clear();
                        materialControl.Material = null;
                    }

                    _Surface = value;

                    if (_Surface != null)
                    {
                        rotationControl.DataBindings.Add("Value", _Surface, "Rotation", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleControl.DataBindings.Add("Value", _Surface, "Scale", false, DataSourceUpdateMode.OnPropertyChanged);
                        triPlanerCheckBox.DataBindings.Add("Checked", _Surface, "TriPlaner", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Surface.Material;

                        CollapseDefault();
                    }

                    SurfaceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Surface;
        }

        public event EventHandler SurfaceChanged;

        public TerrainSurfaceMaterialControl()
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

        public void CollapseAll()
        {
            panel.SuspendLayout();
            shapePanel.Collapsed = true;
            materialControl.CollapseAll();
            panel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Surface != null)
            {
                panel.SuspendLayout();
                shapePanel.Collapsed = _Surface.Scale == 1 &&
                    _Surface.Rotation == 0 &&
                    !_Surface.TriPlaner;
                materialControl.CollapseDefault();
                panel.ResumeLayout();
            }
        }
    }
}
