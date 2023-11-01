using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

namespace CDK.Assets.Components
{
    public partial class Vector3TrackControl : UserControl
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

                    xControl.Value = value.X;
                    yControl.Value = value.Y;
                    zControl.Value = value.Z;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    QuaternionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Vector3(xControl.Value, yControl.Value, zControl.Value);
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

                    xControl.Value = pitch * MathUtil.ToDegrees;
                    yControl.Value = yaw * MathUtil.ToDegrees;
                    zControl.Value = roll * MathUtil.ToDegrees;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    QuaternionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => Quaternion.CreateFromYawPitchRoll(
                yControl.Value * MathUtil.ToRadians,
                xControl.Value * MathUtil.ToRadians,
                zControl.Value * MathUtil.ToRadians);
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
                    xControl.Minimum = value;
                    yControl.Minimum = value;
                    zControl.Minimum = value;
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
                    xControl.Maximum = value;
                    yControl.Maximum = value;
                    zControl.Maximum = value;
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
                    xControl.Increment = value;
                    yControl.Increment = value;
                    zControl.Increment = value;
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
                    xControl.DecimalPlaces = value;
                    yControl.DecimalPlaces = value;
                    zControl.DecimalPlaces = value;
                }
            }
            get => _DecimalPlaces;
        }

        private TrackBarScaleType _ScaleType;

        [DefaultValue(TrackBarScaleType.None)]
        public TrackBarScaleType ScaleType
        {
            set
            {
                if (_ScaleType != value)
                {
                    _ScaleType = value;
                    xControl.ScaleType = value;
                    yControl.ScaleType = value;
                    zControl.ScaleType = value;
                }
            }
            get => _ScaleType;
        }

        public Vector3TrackControl()
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
