using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public partial class EnvironmentControl : UserControl
    {
        private Environment _Object;
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Environment Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        prefConfigComboBox.DataBindings.Clear();
                        prefConfigComboBox.DataSource = null;
                        lightConfigComboBox.DataBindings.Clear();
                        lightConfigComboBox.DataSource = null;

                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;
                        {
                            if (sceneConfig != null) prefConfigComboBox.DataSource = sceneConfig.Preferences;
                            else prefConfigComboBox.DataSource = new PreferenceConfig[] { _Object.PreferenceConfig };
                        }
                        prefConfigComboBox.DisplayMember = "Name";
                        prefConfigComboBox.DataBindings.Add("SelectedItem", _Object, "PreferenceConfig", true, DataSourceUpdateMode.OnPropertyChanged);

                        {
                            if (sceneConfig != null) lightConfigComboBox.DataSource = sceneConfig.Environments;
                            else lightConfigComboBox.DataSource = new EnvironmentConfig[] { _Object.EnvironmentConfig };
                        }
                        lightConfigComboBox.DisplayMember = "Name";
                        lightConfigComboBox.DataBindings.Add("SelectedItem", _Object, "LightConfig", true, DataSourceUpdateMode.OnPropertyChanged);

                        _Object.PropertyChanged += Object_PropertyChanged;
                    }

                    BindPreferenceConfig();
                    BindLightConfig();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private TextBox _prefConfigRenameTextBox;
        private TextBox _lightConfigRenameTextBox;

        public EnvironmentControl()
        {
            InitializeComponent();

            multisampleComboBox.Items.Add(1);
            multisampleComboBox.Items.Add(2);
            multisampleComboBox.Items.Add(4);
            multisampleComboBox.Items.Add(8);
            multisampleComboBox.Items.Add(16);

            maxShadowResolutionComboBox.Items.Add(512);
            maxShadowResolutionComboBox.Items.Add(1024);
            maxShadowResolutionComboBox.Items.Add(2048);

            lightModeComboBox.DataSource = Enum.GetValues(typeof(LightMode));

            _prefConfigRenameTextBox = new TextBox();
            _prefConfigRenameTextBox.KeyDown += PrefConfigRenameTextBox_KeyDown;
            _prefConfigRenameTextBox.LostFocus += PrefConfigRenameTextBox_LostFocus;
            _prefConfigRenameTextBox.Location = prefConfigComboBox.Location;
            _prefConfigRenameTextBox.Size = prefConfigComboBox.Size;
            _prefConfigRenameTextBox.Anchor = prefConfigComboBox.Anchor;
            _prefConfigRenameTextBox.Visible = false;
            Controls.Add(_prefConfigRenameTextBox);

            _lightConfigRenameTextBox = new TextBox();
            _lightConfigRenameTextBox.KeyDown += LightConfigRenameTextBox_KeyDown;
            _lightConfigRenameTextBox.LostFocus += LightConfigRenameTextBox_LostFocus;
            _lightConfigRenameTextBox.Location = lightConfigComboBox.Location;
            _lightConfigRenameTextBox.Size = lightConfigComboBox.Size;
            _lightConfigRenameTextBox.Anchor = lightConfigComboBox.Anchor;
            _lightConfigRenameTextBox.Visible = false;
            Controls.Add(_lightConfigRenameTextBox);

            Disposed += EnvironmentControl_Disposed;
        }

        private void EnvironmentControl_Disposed(object sender, EventArgs e)
        {
            _prefConfigRenameTextBox.Dispose();

            if (_Object != null)
            {
                _Object.PropertyChanged -= Object_PropertyChanged;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "PreferenceConfig":
                    BindPreferenceConfig();
                    break;
                case "LightConfig":
                    BindLightConfig();
                    break;
            }
        }

        private void BindPreferenceConfig()
        {
            cameraFovControl.DataBindings.Clear();
            cameraNearControl.DataBindings.Clear();
            cameraFarControl.DataBindings.Clear();
            lightModeComboBox.DataBindings.Clear();
            allowShadowCheckBox.DataBindings.Clear();
            shadowPanel.DataBindings.Clear();
            allowShadowPixel32CheckBox.DataBindings.Clear();
            maxShadowResolutionComboBox.DataBindings.Clear();
            multisampleComboBox.DataBindings.Clear();
            bloomSizeControl.DataBindings.Clear();
            bloomIntensityControl.DataBindings.Clear();
            bloomThresholdControl.DataBindings.Clear();
            exposureControl.DataBindings.Clear();
            gammaControl.DataBindings.Clear();

            if (_Object != null)
            {
                var config = _Object.PreferenceConfig;

                cameraFovControl.DataBindings.Add("Value", config, "CameraFov", false, DataSourceUpdateMode.OnPropertyChanged);
                cameraNearControl.DataBindings.Add("Value", config, "CameraNear", false, DataSourceUpdateMode.OnPropertyChanged);
                cameraFarControl.DataBindings.Add("Value", config, "CameraFar", false, DataSourceUpdateMode.OnPropertyChanged);

                lightModeComboBox.DataBindings.Add("SelectedItem", config, "LightMode", true, DataSourceUpdateMode.OnPropertyChanged);
                allowShadowCheckBox.DataBindings.Add("Checked", config, "AllowShadow", false, DataSourceUpdateMode.OnPropertyChanged);
                shadowPanel.DataBindings.Add("Visible", config, "AllowShadow", false, DataSourceUpdateMode.Never);
                allowShadowPixel32CheckBox.DataBindings.Add("Checked", config, "AllowShadowPixel32", false, DataSourceUpdateMode.OnPropertyChanged);
                maxShadowResolutionComboBox.DataBindings.Add("SelectedItem", config, "MaxShadowResolution", false, DataSourceUpdateMode.OnPropertyChanged);

                multisampleComboBox.DataBindings.Add("SelectedItem", config, "Multisample", false, DataSourceUpdateMode.OnPropertyChanged);
                bloomSizeControl.DataBindings.Add("Value", config, "BloomSize", false, DataSourceUpdateMode.OnPropertyChanged);
                bloomIntensityControl.DataBindings.Add("Value", config, "BloomIntensity", false, DataSourceUpdateMode.OnPropertyChanged);
                bloomThresholdControl.DataBindings.Add("Value", config, "BloomThreshold", false, DataSourceUpdateMode.OnPropertyChanged);
                exposureControl.DataBindings.Add("Value", config, "Exposure", false, DataSourceUpdateMode.OnPropertyChanged);
                gammaControl.DataBindings.Add("Value", config, "Gamma", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        private void BindLightConfig()
        {
            ambientLightControl.DataBindings.Clear();
            fogCheckBox.DataBindings.Clear();
            fogPanel.DataBindings.Clear();
            fogColorControl.DataBindings.Clear();
            fogNearControl.DataBindings.Clear();
            fogFarControl.DataBindings.Clear();
            skyboxControl.DataBindings.Clear();

            if (_Object != null)
            {
                var config = _Object.EnvironmentConfig;

                ambientLightControl.DataBindings.Add("Value3", config, "AmbientLight", true, DataSourceUpdateMode.OnPropertyChanged);
                fogCheckBox.DataBindings.Add("Checked", config, "UsingFog", false, DataSourceUpdateMode.OnPropertyChanged);
                fogPanel.DataBindings.Add("Visible", config, "UsingFog", false, DataSourceUpdateMode.Never);
                fogColorControl.DataBindings.Add("Value3", config, "FogColor", false, DataSourceUpdateMode.OnPropertyChanged);
                fogNearControl.DataBindings.Add("Value", config, "FogNear", false, DataSourceUpdateMode.OnPropertyChanged);
                fogFarControl.DataBindings.Add("Value", config, "FogFar", false, DataSourceUpdateMode.OnPropertyChanged);
                //skyboxControl.DataBindings.Add("RootAsset", config, "Project", true, DataSourceUpdateMode.Never);
                skyboxControl.DataBindings.Add("SelectedAsset", config, "Skybox", true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        private void BeginRenamePrefConfig()
        {
            prefConfigComboBox.Visible = false;
            _prefConfigRenameTextBox.Text = _Object?.PreferenceConfig.Name;
            _prefConfigRenameTextBox.Visible = true;
            _prefConfigRenameTextBox.Focus();
        }

        private void EndRenamePrefConfig(bool update)
        {
            if (update && _Object != null)
            {
                var str = _prefConfigRenameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(str)) _Object.PreferenceConfig.Name = str;
            }
            prefConfigComboBox.Visible = true;
            _prefConfigRenameTextBox.Visible = false;
        }

        private void PrefConfigRenameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (!prefConfigComboBox.Visible) EndRenamePrefConfig(true);
        }

        private void PrefConfigRenameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) EndRenamePrefConfig(true);
            else if (e.KeyCode == Keys.Escape) EndRenamePrefConfig(false);
        }

        private void PrefConfigRenameButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) BeginRenamePrefConfig();
        }

        private void PrefConfigAddButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (sceneConfig != null)
                {
                    var config = new PreferenceConfig(sceneConfig, _Object.PreferenceConfig);
                    sceneConfig.Preferences.Add(config);
                    _Object.PreferenceConfig = config;
                }
            }
        }

        private void PrefConfigRemoveButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (_Object.PreferenceConfig.Parent == sceneConfig && sceneConfig.Preferences.Count > 1)
                {
                    sceneConfig.Preferences.Remove(_Object.PreferenceConfig);
                    _Object.PreferenceConfig = sceneConfig.Preferences[sceneConfig.Preferences.Count - 1];
                }
            }
        }
        
        private void BeginRenameLightConfig()
        {
            lightConfigComboBox.Visible = false;
            _lightConfigRenameTextBox.Text = _Object?.EnvironmentConfig.Name;
            _lightConfigRenameTextBox.Visible = true;
            _lightConfigRenameTextBox.Focus();
        }

        private void EndRenameLightConfig(bool update)
        {
            if (update && _Object != null)
            {
                var str = _lightConfigRenameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(str)) _Object.EnvironmentConfig.Name = str;
            }
            lightConfigComboBox.Visible = true;
            _lightConfigRenameTextBox.Visible = false;
        }

        private void LightConfigRenameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (!lightConfigComboBox.Visible) EndRenameLightConfig(true);
        }

        private void LightConfigRenameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) EndRenameLightConfig(true);
            else if (e.KeyCode == Keys.Escape) EndRenameLightConfig(false);
        }

        private void LightConfigAddButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (sceneConfig != null)
                {
                    var config = new EnvironmentConfig(sceneConfig, _Object.EnvironmentConfig);
                    sceneConfig.Environments.Add(config);
                    _Object.EnvironmentConfig = config;
                }
            }
        }

        private void LightConfigRemoveButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (_Object.EnvironmentConfig.Parent == sceneConfig && sceneConfig.Environments.Count > 1)
                {
                    sceneConfig.Environments.Remove(_Object.EnvironmentConfig);
                    _Object.EnvironmentConfig = sceneConfig.Environments[sceneConfig.Environments.Count - 1];
                }
            }
        }

        private void LightConfigRenameButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) BeginRenameLightConfig();
        }
    }
}
