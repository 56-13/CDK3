using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Terrain
{
    public partial class TerrainAltitudeControl : UserControl
    {
        private TerrainAltitudeComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainAltitudeComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        slopeRadioButton.DataBindings.Clear();
                        sizeControl.DataBindings.Clear();
                        degreeControl.DataBindings.Clear();
                        attenuationControl.DataBindings.Clear();
                        modeComboBox.DataBindings.Clear();
                        allDegreeUpDown.DataBindings.Clear();
                    }

                    _Object = value;

                    if (_Object != null)
                    {
                        slopeRadioButton.DataBindings.Add("Checked", _Object, "Slope", false, DataSourceUpdateMode.OnPropertyChanged);
                        sizeControl.DataBindings.Add("Value", _Object, "Size", false, DataSourceUpdateMode.OnPropertyChanged);
                        degreeControl.DataBindings.Add("Maximum", _Object, "DegreeMax", false, DataSourceUpdateMode.Never);
                        degreeControl.DataBindings.Add("Minimum", _Object, "DegreeMin", false, DataSourceUpdateMode.Never);
                        degreeControl.DataBindings.Add("Increment", _Object, "DegreeIncrement", false, DataSourceUpdateMode.Never);
                        degreeControl.DataBindings.Add("Value", _Object, "Degree", false, DataSourceUpdateMode.OnPropertyChanged);
                        attenuationControl.DataBindings.Add("Value", _Object, "Attenuation", false, DataSourceUpdateMode.OnPropertyChanged);
                        modeComboBox.DataBindings.Add("SelectedItem", _Object, "Mode", false, DataSourceUpdateMode.OnPropertyChanged);
                        allDegreeUpDown.DataBindings.Add("Value", _Object, "AllDegree", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }
        
        public event EventHandler ObjectChanged;

        public TerrainAltitudeControl()
        {
            InitializeComponent();

            modeComboBox.DataSource = Enum.GetValues(typeof(TerrainModifyMode));

            panel.DataBindings.Add("Visible", modifyRadioButton, "Checked", false, DataSourceUpdateMode.Never);
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var degree = (float)allDegreeUpDown.Value;

                _Object.Asset.ModifyAltitude(true, degree);
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var degree = (float)allDegreeUpDown.Value;

                _Object.Asset.ModifyAltitude(false, degree);
            }
        }

        private void FillButton_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var degree = (float)allDegreeUpDown.Value;

                _Object.Asset.FillAltitude(degree);
            }
        }
    }
}
