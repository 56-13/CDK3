using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Meshing
{
    public partial class MaterialConfigControl : UserControl
    {
        private MaterialConfig _Config;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MaterialConfig Config
        {
            set
            {
                if (_Config != value)
                {
                    if (_Config != null)
                    {
                        assetControl.DataBindings.Clear();
                    }

                    _Config = value;

                    if (_Config != null)
                    {
                        nameLabel.Text = _Config.Name;
                        //assetControl.DataBindings.Add("RootAsset", _Config, "Project", true, DataSourceUpdateMode.Never);
                        assetControl.DataBindings.Add("SelectedAsset", _Config, "Origin", true, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ConfigChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Config;
        }

        public event EventHandler ConfigChanged;

        public MaterialConfigControl()
        {
            InitializeComponent();
        }
    }
}
