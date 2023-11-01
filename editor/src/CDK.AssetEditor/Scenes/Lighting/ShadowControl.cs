using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Scenes
{
    public partial class ShadowControl : UserControl
    {
        private Shadow _Shadow;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Shadow Shadow
        {
            set
            {
                if (_Shadow != value)
                {
                    if (_Shadow != null)
                    {
                        resolutionComboBox.DataBindings.Clear();
                        pixel32RadioButton.DataBindings.Clear();
                        blurControl.DataBindings.Clear();
                        biasControl.DataBindings.Clear();
                        bleedingControl.DataBindings.Clear();
                    }

                    _Shadow = value;

                    if (_Shadow != null)
                    {
                        resolutionComboBox.DataBindings.Add("SelectedItem", _Shadow, "Resolution", false, DataSourceUpdateMode.OnPropertyChanged);
                        pixel32RadioButton.DataBindings.Add("Checked", _Shadow, "Pixel32", false, DataSourceUpdateMode.OnPropertyChanged);
                        blurControl.DataBindings.Add("Value", _Shadow, "Blur", false, DataSourceUpdateMode.OnPropertyChanged);
                        biasControl.DataBindings.Add("Value", _Shadow, "Bias", false, DataSourceUpdateMode.OnPropertyChanged);
                        bleedingControl.DataBindings.Add("Value", _Shadow, "Bleeding", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    ShadowChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Shadow;
        }

        public event EventHandler ShadowChanged;

        public ShadowControl()
        {
            InitializeComponent();

            resolutionComboBox.Items.Add(256);
            resolutionComboBox.Items.Add(512);
            resolutionComboBox.Items.Add(1024);
            resolutionComboBox.Items.Add(2048);
        }
    }
}
