using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Drawing;

using CDK.Assets.Animations.Components;
using CDK.Assets.Components;

namespace CDK.Assets.Texturing
{
    public partial class MaterialControl : UserControl, ICollapsibleControl
    {
        private Material _Material;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Material Material
        {
            set
            {
                if (_Material != value)
                {
                    if (_Material != null)
                    {
                        originControl.DataBindings.Clear();
                        localCheckBox.DataBindings.Clear();
                        shaderComboBox.DataBindings.Clear();
                        shaderComboBox.Items.Clear();
                        distortionScaleControl.DataBindings.Clear();
                        blendLayerComboBox.DataBindings.Clear();
                        blendModeComboBox.DataBindings.Clear();
                        cullModeComboBox.DataBindings.Clear();
                        depthTestCheckBox.DataBindings.Clear();
                        depthBiasControl.DataBindings.Clear();
                        alphaTestCheckBox.DataBindings.Clear();
                        alphaTestBiasControl.DataBindings.Clear();
                        displacementScaleControl.DataBindings.Clear();
                        colorControl.Color = null;
                        colorDurationControl.DataBindings.Clear();
                        colorLoopControl.DataBindings.Clear();
                        bloomCheckBox.DataBindings.Clear();
                        reflectionCheckBox.DataBindings.Clear();
                        receiveShadowCheckBox.DataBindings.Clear();
                        receiveShadow2DcheckBox.DataBindings.Clear();
                        metallicControl.DataBindings.Clear();
                        roughnessControl.DataBindings.Clear();
                        ambientOcclusionControl.DataBindings.Clear();
                        rimControl.DataBindings.Clear();
                        emissionControl.Color = null;
                        emissionDurationControl.DataBindings.Clear();
                        emissionLoopControl.DataBindings.Clear();
                        uvScrollControl.DataBindings.Clear();
                        uvScrollAngleControl.DataBindings.Clear();

                        _Material.PropertyChanged -= Material_PropertyChanged;
                        _Material.Color.PropertyChanged -= MaterialColor_PropertyChanged;
                        _Material.Emission.PropertyChanged -= MaterialColor_PropertyChanged;
                    }

                    _Material = value;

                    if (_Material != null)
                    {
                        switch (_Material.Usage)
                        {
                            case MaterialUsage.Origin:
                            case MaterialUsage.Mesh:
                                shaderComboBox.Items.Add(MaterialShader.Light);
                                shaderComboBox.Items.Add(MaterialShader.NoLight);
                                shaderComboBox.Items.Add(MaterialShader.Distortion);
                                break;
                            case MaterialUsage.Image:
                            case MaterialUsage.Ground:
                            case MaterialUsage.TerrainSurface:
                            case MaterialUsage.TerrainWater:
                                shaderComboBox.Items.Add(MaterialShader.Light);
                                shaderComboBox.Items.Add(MaterialShader.NoLight);
                                break;
                        }

                        //originControl.DataBindings.Add("RootAsset", _Material, "Project", true, DataSourceUpdateMode.Never);
                        originControl.DataBindings.Add("SelectedAsset", _Material, "Origin", true, DataSourceUpdateMode.OnPropertyChanged);
                        localCheckBox.DataBindings.Add("Checked", _Material, "Local", false, DataSourceUpdateMode.OnPropertyChanged);
                        shaderComboBox.DataBindings.Add("SelectedItem", _Material, "Shader", false, DataSourceUpdateMode.OnPropertyChanged);
                        distortionScaleControl.DataBindings.Add("Value", _Material, "DistortionScale", false, DataSourceUpdateMode.OnPropertyChanged);
                        blendLayerComboBox.DataBindings.Add("SelectedItem", _Material, "BlendLayer", false, DataSourceUpdateMode.OnPropertyChanged);
                        blendModeComboBox.DataBindings.Add("SelectedItem", _Material, "BlendMode", false, DataSourceUpdateMode.OnPropertyChanged);
                        cullModeComboBox.DataBindings.Add("SelectedItem", _Material, "CullMode", false, DataSourceUpdateMode.OnPropertyChanged);
                        depthTestCheckBox.DataBindings.Add("Checked", _Material, "DepthTest", false, DataSourceUpdateMode.OnPropertyChanged);
                        depthBiasControl.DataBindings.Add("Visible", _Material, "DepthTest", false, DataSourceUpdateMode.Never);
                        depthBiasControl.DataBindings.Add("Value", _Material, "DepthBias", false, DataSourceUpdateMode.OnPropertyChanged);
                        alphaTestCheckBox.DataBindings.Add("Checked", _Material, "AlphaTest", false, DataSourceUpdateMode.OnPropertyChanged);
                        alphaTestBiasControl.DataBindings.Add("Visible", _Material, "AlphaTest", false, DataSourceUpdateMode.Never);
                        alphaTestBiasControl.DataBindings.Add("Value", _Material, "AlphaTestBias", false, DataSourceUpdateMode.OnPropertyChanged);
                        displacementScaleControl.DataBindings.Add("Value", _Material, "DisplacementScale", false, DataSourceUpdateMode.OnPropertyChanged);
                        colorControl.Color = _Material.Color;
                        colorDurationControl.DataBindings.Add("Value", _Material, "ColorDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        colorLoopControl.DataBindings.Add("Loop", _Material, "ColorLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        bloomCheckBox.DataBindings.Add("Checked", _Material, "Bloom", false, DataSourceUpdateMode.OnPropertyChanged);
                        reflectionCheckBox.DataBindings.Add("Checked", _Material, "Reflection", false, DataSourceUpdateMode.OnPropertyChanged);
                        receiveShadowCheckBox.DataBindings.Add("Checked", _Material, "ReceiveShadow", false, DataSourceUpdateMode.OnPropertyChanged);
                        receiveShadow2DcheckBox.DataBindings.Add("Checked", _Material, "ReceiveShadow2D", false, DataSourceUpdateMode.OnPropertyChanged);
                        metallicControl.DataBindings.Add("Value", _Material, "Metallic", false, DataSourceUpdateMode.OnPropertyChanged);
                        roughnessControl.DataBindings.Add("Value", _Material, "Roughness", false, DataSourceUpdateMode.OnPropertyChanged);
                        ambientOcclusionControl.DataBindings.Add("Value", _Material, "AmbientOcclusion", false, DataSourceUpdateMode.OnPropertyChanged);
                        rimControl.DataBindings.Add("Value", _Material, "Rim", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionControl.Color = _Material.Emission;
                        emissionDurationControl.DataBindings.Add("Value", _Material, "EmissionDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionLoopControl.DataBindings.Add("Loop", _Material, "EmissionLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                        uvScrollControl.DataBindings.Add("Value", _Material, "UVScroll", false, DataSourceUpdateMode.OnPropertyChanged);
                        uvScrollAngleControl.DataBindings.Add("Value", _Material, "UVScrollAngle", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Material.PropertyChanged += Material_PropertyChanged;
                        _Material.Color.PropertyChanged += MaterialColor_PropertyChanged;
                        _Material.Emission.PropertyChanged += MaterialColor_PropertyChanged;

                        ResetControlVisible();

                        CollapseDefault();
                    }

                    MaterialChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Material;
        }

        public event EventHandler MaterialChanged;

        public MaterialControl()
        {
            InitializeComponent();

            blendModeComboBox.DataSource = Enum.GetValues(typeof(BlendMode));
            blendLayerComboBox.DataSource = Enum.GetValues(typeof(InstanceBlendLayer));
            cullModeComboBox.DataSource = Enum.GetValues(typeof(CullMode));

            Disposed += MaterialControl_Disposed;
        }

        private void MaterialControl_Disposed(object sender, EventArgs e)
        {
            if (_Material != null)
            {
                _Material.PropertyChanged -= Material_PropertyChanged;
                _Material.Color.PropertyChanged -= MaterialColor_PropertyChanged;
                _Material.Emission.PropertyChanged -= MaterialColor_PropertyChanged;
            }
        }

        private void Material_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Shader":
                case "Origin":
                case "Local":
                    ResetControlVisible();
                    break;
            }
        }

        private void MaterialColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type") ResetControlVisible();
        }

        private void ResetControlVisible()
        {
            panel.SuspendLayout();

            if (_Material.Local)
            {
                switch (_Material.Usage)
                {
                    case MaterialUsage.Origin:
                        originPanel.Visible = false;
                        blendLayerPanel.Visible = blendModePanel.Visible = _Material.Shader != MaterialShader.Distortion;
                        cullModePanel.Visible = true;
                        clipPanel.Visible = true;
                        receiveShadowCheckBox.Visible = true;
                        receiveShadow2DcheckBox.Visible = true;
                        uvAnimationPanel.Visible = _Material.HasTexture;
                        break;
                    case MaterialUsage.Ground:
                        originPanel.Visible = true;
                        blendLayerPanel.Visible = false;
                        blendModePanel.Visible = false;
                        cullModePanel.Visible = false;
                        clipPanel.Visible = false;
                        receiveShadowCheckBox.Visible = true;
                        receiveShadow2DcheckBox.Visible = true;
                        uvAnimationPanel.Visible = _Material.HasTexture;
                        break;
                    case MaterialUsage.Mesh:
                        originPanel.Visible = true;
                        blendLayerPanel.Visible = blendModePanel.Visible = _Material.Shader != MaterialShader.Distortion;
                        cullModePanel.Visible = true;
                        clipPanel.Visible = true;
                        receiveShadowCheckBox.Visible = true;
                        receiveShadow2DcheckBox.Visible = true;
                        uvAnimationPanel.Visible = _Material.HasTexture;
                        break;
                    case MaterialUsage.Image:
                        originPanel.Visible = false;
                        blendLayerPanel.Visible = blendModePanel.Visible = _Material.Shader != MaterialShader.Distortion;
                        cullModePanel.Visible = true;
                        clipPanel.Visible = true;
                        receiveShadowCheckBox.Visible = true;
                        receiveShadow2DcheckBox.Visible = true;
                        uvAnimationPanel.Visible = _Material.HasTexture;
                        break;
                    case MaterialUsage.TerrainSurface:
                        originPanel.Visible = true;
                        blendLayerPanel.Visible = false;
                        blendModePanel.Visible = false;
                        cullModePanel.Visible = false;
                        clipPanel.Visible = false;
                        receiveShadowCheckBox.Visible = false;
                        receiveShadow2DcheckBox.Visible = false;
                        uvAnimationPanel.Visible = _Material.HasTexture;
                        break;
                    case MaterialUsage.TerrainWater:
                        originPanel.Visible = true;
                        blendLayerPanel.Visible = false;
                        blendModePanel.Visible = false;
                        cullModePanel.Visible = false;
                        clipPanel.Visible = false;
                        receiveShadowCheckBox.Visible = false;
                        receiveShadow2DcheckBox.Visible = false;
                        uvAnimationPanel.Visible = false;
                        break;
                }
                distortionPanel.Visible = _Material.Shader == MaterialShader.Distortion;
                displacementPanel.Visible = _Material.Origin != null && _Material.Origin.HasDisplacement;
                lightPanel.Visible = _Material.ReceiveLight;
                colorPanel.Visible = emissionPanel.Visible = _Material.UsingColor;
                colorAnimationPanel.Visible = _Material.Color.IsAnimating;
                emissionAnimationPanel.Visible = _Material.Emission.IsAnimating;

                loadButton.Visible = true;
                saveButton.Visible = true;
            }
            else
            {
                shaderPanel.Visible = false;
                shapePanel.Visible = false;
                colorPanel.Visible = false;
                lightPanel.Visible = false;
                emissionPanel.Visible = false;
                uvAnimationPanel.Visible = false;

                loadButton.Visible = false;
                saveButton.Visible = false;
            }
            panel.ResumeLayout();
        }

        private void TextureControl_Importing(object sender, AssetSelectImportEventArgs e)
        {
            if (_Material != null)
            {
                switch (_Material.Usage)
                {
                    case MaterialUsage.TerrainSurface:
                    case MaterialUsage.TerrainWater:
                        {
                            var texture = (MaterialAsset)e.Asset;

                            if (texture.TextureDescription.WrapS == TextureWrapMode.ClampToEdge ||
                                texture.TextureDescription.WrapS == TextureWrapMode.ClampToBorder ||
                                texture.TextureDescription.WrapT == TextureWrapMode.ClampToEdge ||
                                texture.TextureDescription.WrapT == TextureWrapMode.ClampToBorder)
                            {
                                e.Cancel = true;

                                MessageBox.Show(this, "지형 편집에 사용할 텍스쳐는 좌표 잘라내기를 사용할 수 없습니다.");
                            }
                        }
                        break;
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (_Material != null && _Material.Origin != null && MessageBox.Show(this, "원본의 기본 설정으로 되돌리시겠습니까?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _Material.LoadTexture();

                CollapseDefault();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_Material != null && _Material.Origin != null && MessageBox.Show(this, "원본의 기본 설정을 이 설정으로 저장하시겠습니까?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _Material.SaveTexture();

                CollapseDefault();
            }
        }

        public void CollapseAll()
        {
            panel.SuspendLayout();
            originPanel.Collapsed =
            colorPanel.Collapsed =
            lightPanel.Collapsed =
            emissionPanel.Collapsed =
            uvAnimationPanel.Collapsed = true;
            panel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Material != null)
            {
                panel.SuspendLayout();

                originPanel.Collapsed = false;

                if (_Material.Origin != null && _Material.Usage != MaterialUsage.Origin)
                {
                    var origin = _Material.Origin.Material;

                    shaderPanel.Collapsed = _Material.Shader == origin.Shader &&
                        _Material.BlendLayer == origin.BlendLayer &&
                        _Material.BlendMode == origin.BlendMode;

                    shapePanel.Collapsed = _Material.CullMode == origin.CullMode &&
                        _Material.DepthTest == origin.DepthTest &&
                        _Material.DepthBias == origin.DepthBias &&
                        _Material.AlphaTest == origin.AlphaTest &&
                        _Material.AlphaTestBias == origin.AlphaTestBias &&
                        _Material.DisplacementScale == origin.DisplacementScale;

                    colorPanel.Collapsed = _Material.Color.Equals(origin.Color) && 
                        _Material.ColorDuration == origin.ColorDuration && 
                        _Material.ColorLoop == origin.ColorLoop;

                    lightPanel.Collapsed = Material.Reflection == origin.Reflection &&
                        Material.ReceiveShadow == origin.ReceiveShadow &&
                        Material.ReceiveShadow2D == origin.ReceiveShadow2D &&
                        Material.Metallic == origin.Metallic &&
                        Material.Roughness == origin.Roughness &&
                        Material.AmbientOcclusion == origin.AmbientOcclusion &&
                        Material.Rim == origin.Rim;

                    emissionPanel.Collapsed = _Material.Emission.Equals(origin.Emission) && 
                        _Material.EmissionDuration == origin.EmissionDuration && 
                        _Material.EmissionLoop == origin.EmissionLoop;

                    uvAnimationPanel.Collapsed = _Material.UVScroll == origin.UVScroll && 
                        _Material.UVScrollAngle == origin.UVScrollAngle;
                }
                else
                {
                    shaderPanel.Collapsed = (_Material.Shader == MaterialShader.Light || _Material.Shader == MaterialShader.NoLight) &&
                        _Material.BlendLayer == InstanceBlendLayer.Middle &&
                        _Material.BlendMode == BlendMode.None;

                    shapePanel.Collapsed = _Material.CullMode == CullMode.Back &&
                        _Material.DepthTest &&
                        _Material.DepthBias == 0 &&
                        !_Material.AlphaTest &&
                        _Material.AlphaTestBias == 0 &&
                        _Material.DisplacementScale == 0;

                    colorPanel.Collapsed = _Material.Color.Type == AnimationColorType.None && 
                        _Material.ColorDuration == 0 && 
                        _Material.ColorLoop.IsDefault;
                    
                    lightPanel.Collapsed = Material.Reflection == Drawing.Material.Default.Reflection &&
                        Material.ReceiveShadow == Drawing.Material.Default.ReceiveShadow &&
                        Material.ReceiveShadow2D == Drawing.Material.Default.ReceiveShadow2D &&
                        Material.Metallic == Drawing.Material.Default.Metallic &&
                        Material.Roughness == Drawing.Material.Default.Roughness &&
                        Material.AmbientOcclusion == Drawing.Material.Default.AmbientOcclusion &&
                        Material.Rim == Drawing.Material.Default.Rim;

                    emissionPanel.Collapsed = _Material.Emission.Type == AnimationColorType.None && 
                        _Material.EmissionDuration == 0 && 
                        _Material.EmissionLoop.IsDefault;
                    
                    uvAnimationPanel.Collapsed = _Material.UVScroll == 0 &&
                        _Material.UVScrollAngle == 0;
                }
                panel.ResumeLayout();
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
