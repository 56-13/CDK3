using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationFloatConstantControl : UserControl
    {
        private AnimationFloatConstant _Constant;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationFloatConstant Constant
        {
            set
            {
                if (_Constant != value)
                {
                    if (_Constant != null)
                    {
                        vUpDown.DataBindings.Clear();
                        vvUpDown.DataBindings.Clear();
                        trackBar.DataBindings.Clear();
                    }
                    _Constant = value;
                    if (_Constant != null)
                    {
                        vUpDown.Minimum = (decimal)_Constant.MinValue;
                        vUpDown.Maximum = (decimal)_Constant.MaxValue;
                        vvUpDown.Maximum = (decimal)((_Constant.MaxValue - _Constant.MinValue) / 2);
                        trackBar.Minimum = _Constant.MinValue;
                        trackBar.Maximum = _Constant.MaxValue;

                        vUpDown.DataBindings.Add("Value", _Constant, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
                        vvUpDown.DataBindings.Add("Value", _Constant, "ValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                        trackBar.DataBindings.Add("Value", _Constant, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
                        trackBar.DataBindings.Add("Range", _Constant, "ValueVar", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ConstantChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Constant;
        }
        public event EventHandler ConstantChanged;

        public TrackBarScaleType ScaleType
        {
            set => trackBar.ScaleType = value;
            get => trackBar.ScaleType;
        }

        public float Increment
        {
            set
            {
                trackBar.Increment = value;
                vUpDown.Increment = (decimal)value;
                vvUpDown.Increment = (decimal)value;
                vUpDown.Increment = (decimal)value;
                vvUpDown.Increment = (decimal)value;
            }
            get => trackBar.Increment;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MaxColor
        {
            set => trackBar.MaxColor = value;
            get => trackBar.MaxColor;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MinColor
        {
            set => trackBar.MinColor = value;
            get => trackBar.MinColor;
        }

        public string Title
        {
            set => label1.Text = value;
            get => label1.Text;
        }

        public AnimationFloatConstantControl()
        {
            InitializeComponent();
        }
    }
}
