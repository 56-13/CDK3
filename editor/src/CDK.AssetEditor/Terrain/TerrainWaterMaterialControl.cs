using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Terrain
{
    public partial class TerrainWaterMaterialControl : UserControl, ICollapsibleControl
    {
        private TerrainWater _Water;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainWater Water
        {
            set
            {
                if (_Water != value)
                {
                    if (_Water != null)
                    {
                        perturbIntensityControl.DataBindings.Clear();
                        textureScaleControl.DataBindings.Clear();
                        foamTextureControl.DataBindings.Clear();
                        foamIntensityControl.DataBindings.Clear();
                        foamScaleControl.DataBindings.Clear();
                        foamDepthControl.DataBindings.Clear();
                        angleControl.DataBindings.Clear();
                        forwardSpeedControl.DataBindings.Clear();
                        crossSpeedControl.DataBindings.Clear();
                        waveDistanceControl.DataBindings.Clear();
                        waveAltitudeControl.DataBindings.Clear();
                        depthMaxControl.DataBindings.Clear();
                        shallowColorControl.DataBindings.Clear();
                        materialControl.Material = null;
                    }

                    _Water = value;

                    if (_Water != null)
                    {
                        perturbIntensityControl.DataBindings.Add("Value", _Water, "PerturbIntensity", false, DataSourceUpdateMode.OnPropertyChanged);
                        textureScaleControl.DataBindings.Add("Value", _Water, "TextureScale", false, DataSourceUpdateMode.OnPropertyChanged);
                        //foamTextureControl.DataBindings.Add("RootAsset", _Water, "Project", true, DataSourceUpdateMode.Never);
                        foamTextureControl.DataBindings.Add("SelectedAsset", _Water, "FoamTexture", true, DataSourceUpdateMode.OnPropertyChanged);
                        foamIntensityControl.DataBindings.Add("Value", _Water, "FoamIntensity", false, DataSourceUpdateMode.OnPropertyChanged);
                        foamScaleControl.DataBindings.Add("Value", _Water, "FoamScale", false, DataSourceUpdateMode.OnPropertyChanged);
                        foamDepthControl.DataBindings.Add("Value", _Water, "FoamDepth", false, DataSourceUpdateMode.OnPropertyChanged);
                        angleControl.DataBindings.Add("Value", _Water, "Angle", false, DataSourceUpdateMode.OnPropertyChanged);
                        forwardSpeedControl.DataBindings.Add("Value", _Water, "ForwardSpeed", false, DataSourceUpdateMode.OnPropertyChanged);
                        crossSpeedControl.DataBindings.Add("Value", _Water, "CrossSpeed", false, DataSourceUpdateMode.OnPropertyChanged);
                        waveDistanceControl.DataBindings.Add("Value", _Water, "WaveDistance", false, DataSourceUpdateMode.OnPropertyChanged);
                        waveAltitudeControl.DataBindings.Add("Value", _Water, "WaveAltitude", false, DataSourceUpdateMode.OnPropertyChanged);
                        depthMaxControl.DataBindings.Add("Value", _Water, "DepthMax", false, DataSourceUpdateMode.OnPropertyChanged);
                        shallowColorControl.DataBindings.Add("Value4", _Water, "ShallowColor", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Water.Material;

                        CollapseDefault();
                    }

                    WaterChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Water;
        }

        public event EventHandler WaterChanged;

        public TerrainWaterMaterialControl()
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
            foamPanel.Collapsed = true;
            movementPanel.Collapsed = true;
            depthPanel.Collapsed = true;
            panel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Water != null)
            {
                panel.SuspendLayout();
                shapePanel.Collapsed = _Water.TextureScale == 1 &&
                    _Water.PerturbIntensity == TerrainWater.PerturbIntensityDefault;
                materialControl.CollapseDefault();
                foamPanel.Collapsed = _Water.FoamTexture == null;
                movementPanel.Collapsed = _Water.Angle == 0 && 
                    _Water.ForwardSpeed == 0 && 
                    _Water.CrossSpeed == 0 && 
                    _Water.WaveAltitude == 0 && 
                    _Water.WaveDistance == 0;
                depthPanel.Collapsed = _Water.DepthMax == 0;
                panel.ResumeLayout();
            }
        }
    }
}
