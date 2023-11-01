using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace CDK.Assets.Components
{
    public partial class Vector2Control : UserControl
    {
        private bool _raiseValueChanged;

        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 Value
        {
            set
            {
                if (Value != value)
                {
                    _raiseValueChanged = false;

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Vector2((float)xUpDown.Value, (float)yUpDown.Value);
        }

        public event EventHandler ValueChanged;

        [DefaultValue(typeof(Point), "0, 0")]
        public Point PointGDI
        {
            set
            {
                if (PointGDI != value)
                {
                    _raiseValueChanged = false;

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Point((int)xUpDown.Value, (int)yUpDown.Value);
        }
        public event EventHandler PointGDIChanged;

        [DefaultValue(typeof(PointF), "0, 0")]
        public PointF PointFGDI
        {
            set
            {
                if (PointFGDI != value)
                {
                    _raiseValueChanged = false;

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new PointF((float)xUpDown.Value, (float)yUpDown.Value);
        }
        public event EventHandler PointFGDIChanged;

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
                }
            }
            get => _DecimalPlaces;
        }

        public Vector2Control()
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
                PointGDIChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
