using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

namespace CDK.Assets.Components
{
    public partial class Vector3Control : UserControl
    {
        private bool _raiseValueChanged;

        [DefaultValue(typeof(Vector3), "0, 0, 0")]
        public Vector3 Value
        {
            set
            {
                if (Value != value)
                {
                    _raiseValueChanged = false;

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;
                    zUpDown.Value = (decimal)value.Z;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    QuaternionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Vector3((float)xUpDown.Value, (float)yUpDown.Value, (float)zUpDown.Value);
        }

        public event EventHandler ValueChanged;

        [DefaultValue(typeof(Quaternion), "0, 0, 0, 1")]
        public Quaternion Quaternion
        {
            set
            {
                if (Quaternion != value)
                {
                    _raiseValueChanged = false;

                    value.GetYawPitchRoll(out var yaw, out var pitch, out var roll);

                    xUpDown.Value = (decimal)(pitch * MathUtil.ToDegrees);
                    yUpDown.Value = (decimal)(yaw * MathUtil.ToDegrees);
                    zUpDown.Value = (decimal)(roll * MathUtil.ToDegrees);

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    QuaternionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => Quaternion.CreateFromYawPitchRoll(
                (float)yUpDown.Value * MathUtil.ToRadians, 
                (float)xUpDown.Value * MathUtil.ToRadians, 
                (float)zUpDown.Value * MathUtil.ToRadians);
        }

        public event EventHandler QuaternionChanged;

        private float _Minimum;

        [DefaultValue(-1000)]
        public float Minimum
        {
            set
            {
                if (_Minimum != value)
                {
                    _Minimum = value;
                    xUpDown.Minimum = (decimal)value;
                    yUpDown.Minimum = (decimal)value;
                    zUpDown.Minimum = (decimal)value;
                }
            }
            get => _Minimum;
        }

        private float _Maximum;

        [DefaultValue(1000)]
        public float Maximum
        {
            set
            {
                if (_Maximum != value)
                {
                    _Maximum = value;
                    xUpDown.Maximum = (decimal)value;
                    yUpDown.Maximum = (decimal)value;
                    zUpDown.Maximum = (decimal)value;
                }
            }
            get => _Maximum;
        }

        private float _Increment;
        
        [DefaultValue(1)]
        public float Increment
        {
            set
            {
                if (_Increment != value)
                {
                    _Increment = value;
                    xUpDown.Increment = (decimal)value;
                    yUpDown.Increment = (decimal)value;
                    zUpDown.Increment = (decimal)value;
                }
            }
            get => _Increment;
        }

        private int _DecimalPlaces;

        [DefaultValue(0)]
        public int DecimalPlaces
        {
            set
            {
                if (_DecimalPlaces != value)
                {
                    _DecimalPlaces = value;
                    xUpDown.DecimalPlaces = value;
                    yUpDown.DecimalPlaces = value;
                    zUpDown.DecimalPlaces = value;
                }
            }
            get => _DecimalPlaces;
        }

        public Vector3Control()
        {
            InitializeComponent();

            _Minimum = -1000;
            _Maximum = 1000;
            _Increment = 1;

            _raiseValueChanged = true;
        }

        private void Component_ValueChanged(object sender, EventArgs e)
        {
            if (_raiseValueChanged)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
                QuaternionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
