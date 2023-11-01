using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace CDK.Assets.Components
{
    public partial class Vector2TrackControl : UserControl
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

                    xControl.Value = value.X;
                    yControl.Value = value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Vector2(xControl.Value, yControl.Value);
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

                    xControl.Value = value.X;
                    xControl.Value = value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Point((int)xControl.Value, (int)xControl.Value);
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

                    xControl.Value = value.X;
                    xControl.Value = value.Y;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    PointGDIChanged?.Invoke(this, EventArgs.Empty);
                    PointFGDIChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new PointF((float)xControl.Value, (float)xControl.Value);
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
                    xControl.Minimum = value;
                    yControl.Minimum = value;
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
                }
            }
            get => _ScaleType;
        }

        public Vector2TrackControl()
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
