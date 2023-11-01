using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Components
{
    public partial class TrackControl : UserControl
    {
        [DefaultValue(0)]
        public float Minimum
        {
            set
            {
                valueUpDown.Minimum = (decimal)value;
                valueTrackBar.Minimum = value;
            }
            get => (float)valueUpDown.Minimum;
        }

        [DefaultValue(100)]
        public float Maximum
        {
            set
            {
                valueUpDown.Maximum = (decimal)value;
                valueTrackBar.Maximum = value;
            }
            get => (float)valueUpDown.Maximum;
        }

        [DefaultValue(1)]
        public float Increment
        {
            set => valueUpDown.Increment = (decimal)value;
            get => (float)valueUpDown.Increment;
        }

        [DefaultValue(0)]
        public int DecimalPlaces
        {
            set
            {
                valueUpDown.DecimalPlaces = value;
                valueTrackBar.DecimalPlaces = value;
            }
            get => valueUpDown.DecimalPlaces;
        }

        [DefaultValue(TrackBarScaleType.None)]
        public TrackBarScaleType ScaleType
        {
            set => valueTrackBar.ScaleType = value;
            get => valueTrackBar.ScaleType;
        }

        [DefaultValue(0)]
        public float Value
        {
            set => valueUpDown.Value = (decimal)value;
            get => (float)valueUpDown.Value;
        }
        public event EventHandler ValueChanged;

        public TrackControl()
        {
            InitializeComponent();

            valueTrackBar.DataBindings.Add("Value", valueUpDown, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ValueUpDown_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
