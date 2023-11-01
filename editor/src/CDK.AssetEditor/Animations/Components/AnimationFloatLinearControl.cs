using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationFloatLinearControl : UserControl
    {
        private AnimationFloatLinear _Linear;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationFloatLinear Linear
        {
            set
            {
                if (_Linear != value)
                {
                    if (_Linear != null)
                    {
                        svUpDown.DataBindings.Clear();
                        svvUpDown.DataBindings.Clear();
                        sTrackBar.DataBindings.Clear();
                        evUpDown.DataBindings.Clear();
                        evvUpDown.DataBindings.Clear();
                        eTrackBar.DataBindings.Clear();
                        smoothCheckBox.DataBindings.Clear();
                    }
                    _Linear = value;
                    if (_Linear != null)
                    {
                        svUpDown.Minimum = (decimal)_Linear.MinValue;
                        svUpDown.Maximum = (decimal)_Linear.MaxValue;
                        svvUpDown.Maximum = (decimal)((_Linear.MaxValue - _Linear.MinValue) / 2);
                        sTrackBar.Minimum = _Linear.MinValue;
                        sTrackBar.Maximum = _Linear.MaxValue;
                        evUpDown.Minimum = (decimal)_Linear.MinValue;
                        evUpDown.Maximum = (decimal)_Linear.MaxValue;
                        evvUpDown.Maximum = (decimal)((_Linear.MaxValue - _Linear.MinValue) / 2);
                        eTrackBar.Minimum = _Linear.MinValue;
                        eTrackBar.Maximum = _Linear.MaxValue;

                        svUpDown.DataBindings.Add("Value", _Linear, "StartValue", false, DataSourceUpdateMode.OnPropertyChanged);
                        svvUpDown.DataBindings.Add("Value", _Linear, "StartValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                        sTrackBar.DataBindings.Add("Value", _Linear, "StartValue", false, DataSourceUpdateMode.OnPropertyChanged);
                        sTrackBar.DataBindings.Add("Range", _Linear, "StartValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                        evUpDown.DataBindings.Add("Value", _Linear, "EndValue", false, DataSourceUpdateMode.OnPropertyChanged);
                        evvUpDown.DataBindings.Add("Value", _Linear, "EndValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                        eTrackBar.DataBindings.Add("Value", _Linear, "EndValue", false, DataSourceUpdateMode.OnPropertyChanged);
                        eTrackBar.DataBindings.Add("Range", _Linear, "EndValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                        smoothCheckBox.DataBindings.Add("Checked", _Linear, "Smooth", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    LinearChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Linear;
        }
        public event EventHandler LinearChanged;

        public TrackBarScaleType ScaleType
        {
            set
            {
                sTrackBar.ScaleType = value;
                eTrackBar.ScaleType = value;
            }
            get => sTrackBar.ScaleType;
        }

        public float Increment
        {
            set
            {
                sTrackBar.Increment = value;
                eTrackBar.Increment = value;
                svUpDown.Increment = (decimal)value;
                svvUpDown.Increment = (decimal)value;
                evUpDown.Increment = (decimal)value;
                evvUpDown.Increment = (decimal)value;
            }
            get => sTrackBar.Increment;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MaxColor
        {
            set
            {
                sTrackBar.MaxColor = value;
                eTrackBar.MaxColor = value;
            }
            get => sTrackBar.MaxColor;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MinColor
        {
            set
            {
                sTrackBar.MinColor = value;
                eTrackBar.MinColor = value;
            }
            get => sTrackBar.MinColor;
        }

        private string _Title;
        public string Title
        {
            set
            {
                label1.Text = "Start " + value;
                label2.Text = "End " + value;
            }
            get => _Title;
        }
        public AnimationFloatLinearControl()
        {
            InitializeComponent();

            _Title = string.Empty;
        }
    }
}
