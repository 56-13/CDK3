using System;
using System.Windows.Forms;
using System.ComponentModel;

using Rectangle = CDK.Drawing.Rectangle;
using GDIRectangle = System.Drawing.Rectangle;
using GDIRectangleF = System.Drawing.RectangleF;

namespace CDK.Assets.Components
{
    public partial class RectangleTrackControl : UserControl
    {
        private bool _raiseValueChanged;

        [DefaultValue(typeof(GDIRectangleF), "0, 0, 0, 0")]
        public GDIRectangleF ValueFGDI
        {
            set
            {
                if (ValueFGDI != value)
                {
                    _raiseValueChanged = false;

                    xControl.Value = value.X;
                    yControl.Value = value.Y;
                    widthControl.Value = value.Width;
                    heightControl.Value = value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new GDIRectangleF(xControl.Value, yControl.Value, widthControl.Value, heightControl.Value);
        }
        public event EventHandler ValueFGDIChanged;

        [DefaultValue(typeof(GDIRectangle), "0, 0, 0, 0")]
        public GDIRectangle ValueGDI
        {
            set
            {
                if (ValueGDI != value)
                {
                    _raiseValueChanged = false;

                    xControl.Value = value.X;
                    yControl.Value = value.Y;
                    widthControl.Value = value.Width;
                    heightControl.Value = value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new GDIRectangle((int)xControl.Value, (int)yControl.Value, (int)widthControl.Value, (int)heightControl.Value);
        }
        public event EventHandler ValueGDIChanged;

        [DefaultValue(typeof(Rectangle), "0, 0, 0, 0")]
        public Rectangle Value
        {
            set
            {
                if (Value != value)
                {
                    _raiseValueChanged = false;

                    xControl.Value = value.X;
                    yControl.Value = value.Y;
                    widthControl.Value = value.Width;
                    heightControl.Value = value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Rectangle((float)xControl.Value, (float)yControl.Value, (float)widthControl.Value, (float)heightControl.Value);
        }

        public event EventHandler ValueChanged;

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
                    widthControl.Maximum = (_Maximum - _Minimum);
                    heightControl.Maximum = (_Maximum - _Minimum);
                }
            }
            get  => _Minimum;
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
                    widthControl.Maximum = (_Maximum - _Minimum);
                    heightControl.Maximum = (_Maximum - _Minimum);
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
                    widthControl.Increment = value;
                    heightControl.Increment = value;
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
                    widthControl.DecimalPlaces = value;
                    heightControl.DecimalPlaces = value;
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
                    widthControl.ScaleType = value;
                    heightControl.ScaleType = value;
                }
            }
            get => _ScaleType;
        }

        public RectangleTrackControl()
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
                ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
