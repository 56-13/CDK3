using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public partial class GroundControl : UserControl
    {
        private Ground _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Ground Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        terrainControl.DataBindings.Clear();
                        configComboBox.DataBindings.Clear();
                        configComboBox.DataSource = null;

                        _Object.PropertyChanged -= Object_PropertyChanged;
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                        //groundPanel.Visible = configPanel.Visible = materialControl.Visible = _Object.Terrain == null;
                        //TODO

                        //terrainControl.DataBindings.Add("RootAsset", _Object, "Project", true, DataSourceUpdateMode.Never);
                        terrainControl.DataBindings.Add("SelectedAsset", _Object, "Terrain", true, DataSourceUpdateMode.OnPropertyChanged);

                        if (sceneConfig != null) configComboBox.DataSource = sceneConfig.Grounds;
                        else configComboBox.DataSource = new GroundConfig[] { _Object.Config };

                        configComboBox.DisplayMember = "Name";
                        configComboBox.DataBindings.Add("SelectedItem", _Object, "Config", true, DataSourceUpdateMode.OnPropertyChanged);

                        _Object.PropertyChanged += Object_PropertyChanged;
                    }

                    BindConfig();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private TextBox _configRenameTextBox;

        public GroundControl()
        {
            InitializeComponent();

            _configRenameTextBox = new TextBox();
            _configRenameTextBox.KeyDown += ConfigRenameTextBox_KeyDown;
            _configRenameTextBox.LostFocus += ConfigRenameTextBox_LostFocus;
            _configRenameTextBox.Location = configComboBox.Location;
            _configRenameTextBox.Size = configComboBox.Size;
            _configRenameTextBox.Anchor = configComboBox.Anchor;
            _configRenameTextBox.Visible = false;
            Controls.Add(_configRenameTextBox);

            Disposed += GroundControl_Disposed;
        }

        private void GroundControl_Disposed(object sender, EventArgs e)
        {
            _configRenameTextBox.Dispose();

            if (_Object != null)
            {
                _Object.PropertyChanged -= Object_PropertyChanged;
            }
        }

        private void BindConfig()
        {
            widthUpDown.DataBindings.Clear();
            heightUpDown.DataBindings.Clear();
            gridUpDown.DataBindings.Clear();
            altitudeUpDown.DataBindings.Clear();
            gridColorControl.DataBindings.Clear();
            gridVisibleCheckBox.DataBindings.Clear();
            materialControl.Material = null;

            if (_Object != null)
            {
                var config = _Object.Config;

                widthUpDown.DataBindings.Add("Value", config, "Width", false, DataSourceUpdateMode.OnPropertyChanged);
                heightUpDown.DataBindings.Add("Value", config, "Height", false, DataSourceUpdateMode.OnPropertyChanged);
                gridUpDown.DataBindings.Add("Value", config, "Grid", false, DataSourceUpdateMode.OnPropertyChanged);
                altitudeUpDown.DataBindings.Add("Value", config, "Altitude", false, DataSourceUpdateMode.OnPropertyChanged);
                gridColorControl.DataBindings.Add("Value3", config, "GridColor", true, DataSourceUpdateMode.OnPropertyChanged);
                gridVisibleCheckBox.DataBindings.Add("Checked", _Object, "GridVisible", false, DataSourceUpdateMode.OnPropertyChanged);
                materialControl.Material = config.Material;
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Terrain":
                    //configPanel.Visible = groundPanel.Visible = materialControl.Visible = _Object.Terrain == null;
                    //TODO
                    break;
                case "Config":
                    BindConfig();
                    break;
            }
        }

        private void BeginRenameConfig()
        {
            configComboBox.Visible = false;
            _configRenameTextBox.Text = _Object?.Config.Name;
            _configRenameTextBox.Visible = true;
            _configRenameTextBox.Focus();
        }

        private void EndRenameConfig(bool update)
        {
            if (update && _Object != null)
            {
                var str = _configRenameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(str)) _Object.Config.Name = str;
            }
            configComboBox.Visible = true;
            _configRenameTextBox.Visible = false;
        }

        private void ConfigRenameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (!configComboBox.Visible) EndRenameConfig(true);
        }

        private void ConfigRenameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) EndRenameConfig(true);
            else if (e.KeyCode == Keys.Escape) EndRenameConfig(false);
        }

        private void ConfigRenameButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) BeginRenameConfig();
        }

        private void ConfigAddButton_Click(object sender, EventArgs e)
        {
            if (_Object != null) 
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (sceneConfig != null)
                {
                    var config = new GroundConfig(sceneConfig, _Object.Config);
                    sceneConfig.Grounds.Add(config);
                    _Object.Config = config;
                }
            }
        }

        private void ConfigRemoveButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var sceneConfig = _Object.GetAncestor<Scene>(false)?.Config;

                if (_Object.Config.Parent == sceneConfig && sceneConfig.Grounds.Count > 1)
                {
                    sceneConfig.Grounds.Remove(_Object.Config);
                }
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
