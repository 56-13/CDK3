using System;
using System.Windows.Forms;
using System.ComponentModel;

using Rectangle = CDK.Drawing.Rectangle;
using GDIRectangle = System.Drawing.Rectangle;
using GDIRectangleF = System.Drawing.RectangleF;

namespace CDK.Assets.Components
{
    public partial class RectangleControl : UserControl
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

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;
                    widthUpDown.Value = (decimal)value.Width;
                    heightUpDown.Value = (decimal)value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new GDIRectangleF((float)xUpDown.Value, (float)yUpDown.Value, (float)widthUpDown.Value, (float)heightUpDown.Value);
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

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;
                    widthUpDown.Value = (decimal)value.Width;
                    heightUpDown.Value = (decimal)value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new GDIRectangle((int)xUpDown.Value, (int)yUpDown.Value, (int)widthUpDown.Value, (int)heightUpDown.Value);
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

                    xUpDown.Value = (decimal)value.X;
                    yUpDown.Value = (decimal)value.Y;
                    widthUpDown.Value = (decimal)value.Width;
                    heightUpDown.Value = (decimal)value.Height;

                    _raiseValueChanged = true;

                    ValueFGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueGDIChanged?.Invoke(this, EventArgs.Empty);
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => new Rectangle((float)xUpDown.Value, (float)yUpDown.Value, (float)widthUpDown.Value, (float)heightUpDown.Value);
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
                    xUpDown.Minimum = (decimal)value;
                    yUpDown.Minimum = (decimal)value;
                    widthUpDown.Maximum = (decimal)(_Maximum - _Minimum);
                    heightUpDown.Maximum = (decimal)(_Maximum - _Minimum);
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
                    widthUpDown.Maximum = (decimal)(_Maximum - _Minimum);
                    heightUpDown.Maximum = (decimal)(_Maximum - _Minimum);
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
                    widthUpDown.Increment = (decimal)value;
                    heightUpDown.Increment = (decimal)value;
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
                    widthUpDown.DecimalPlaces = value;
                    heightUpDown.DecimalPlaces = value;
                }
            }
            get => _DecimalPlaces;
        }

        public RectangleControl()
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
