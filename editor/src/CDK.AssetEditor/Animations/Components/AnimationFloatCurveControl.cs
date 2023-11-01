using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationFloatCurveControl : UserControl
    {
        private AnimationFloatCurve _Curve;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationFloatCurve Curve
        {
            set
            {
                if (_Curve != value)
                {
                    _Curve = value;

                    screenControl.Curve = value;
                    
                    if (_Curve != null)
                    {
                        vUpDown.Minimum = (decimal)_Curve.MinValue;
                        vUpDown.Maximum = (decimal)_Curve.MaxValue;
                        vvUpDown.Maximum = (decimal)(_Curve.MaxValue - _Curve.MinValue);
                    }
                    windowButton.Visible = _Curve != null;
                }
            }
            get => _Curve;
        }

        [DefaultValue(1f)]
        public float Increment
        {
            set
            {
                screenControl.Increment = value;
                vUpDown.Increment = (decimal)value;
                vvUpDown.Increment = (decimal)value;
            }
            get => screenControl.Increment;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MinColor
        {
            set => screenControl.MinColor = value;
            get => screenControl.MinColor;
        }

        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MaxColor
        {
            set => screenControl.MaxColor = value;
            get => screenControl.MaxColor;
        }

        public AnimationFloatCurveControl()
        {
            InitializeComponent();

            screenControl.CurvePointChanged += ScreenControl_CurvePointChanged;
        }

        private void ScreenControl_CurvePointChanged(object sender, EventArgs e)
        {
            tUpDown.DataBindings.Clear();
            vUpDown.DataBindings.Clear();
            laUpDown.DataBindings.Clear();
            raUpDown.DataBindings.Clear();
            vvUpDown.DataBindings.Clear();

            if (screenControl.CurvePoint != null)
            {
                tUpDown.DataBindings.Add("Value", screenControl.CurvePoint, "T", false, DataSourceUpdateMode.OnPropertyChanged);
                vUpDown.DataBindings.Add("Value", screenControl.CurvePoint, "V", false, DataSourceUpdateMode.OnPropertyChanged);
                laUpDown.DataBindings.Add("Value", screenControl.CurvePoint, "LeftAngle", false, DataSourceUpdateMode.OnPropertyChanged);
                raUpDown.DataBindings.Add("Value", screenControl.CurvePoint, "RightAngle", false, DataSourceUpdateMode.OnPropertyChanged);

                tLabel.Visible = true;
                tUpDown.Visible = true;
                vLabel.Visible = true;
                vUpDown.Visible = true;
                laLabel.Visible = true;
                laUpDown.Visible = true;
                raLabel.Visible = true;
                raUpDown.Visible = true;

                if (_Curve.Variant)
                {
                    vvLabel.Visible = true;
                    vvUpDown.Visible = true;
                    vvUpDown.DataBindings.Add("Value", screenControl.CurvePoint, "VVar", false, DataSourceUpdateMode.OnPropertyChanged);
                }
                else
                {
                    vvLabel.Visible = false;
                    vvUpDown.Visible = false;
                }
            }
            else
            {
                tLabel.Visible = false;
                tUpDown.Visible = false;
                vLabel.Visible = false;
                vUpDown.Visible = false;
                laLabel.Visible = false;
                laUpDown.Visible = false;
                raLabel.Visible = false;
                raUpDown.Visible = false;
                vvLabel.Visible = false;
                vvUpDown.Visible = false;
            }
        }

        private void WindowButton_Click(object sender, EventArgs e)
        {
            var form = new AssetForm
            {
                Size = new Size(1280, 768)
            };
            var control = new AnimationFloatCurveControl
            {
                Increment = Increment,
                MinColor = MinColor,
                MaxColor = MaxColor,
                Curve = _Curve,
                Dock = DockStyle.Fill
            };
            control.windowButton.Visible = false;
            form.Controls.Add(control);
            form.Show(this);
        }
    }
}
