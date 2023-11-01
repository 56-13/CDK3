using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationFloatControl : UserControl
    {
        private AnimationFloat _Value;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationFloat Value
        {
            set
            {
                if (_Value != value)
                {
                    if (_Value != null)
                    {
                        typeComboBox.DataBindings.Clear();

                        _Value.PropertyChanged -= Value_PropertyChanged;
                    }
                    _Value = value;

                    if (_Value != null)
                    {
                        typeComboBox.DataBindings.Add("SelectedItem", _Value, "Type", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Value.PropertyChanged += Value_PropertyChanged;
                    }
                    ResetControl();

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Value;
        }

        public event EventHandler ValueChanged;

        private TrackBarScaleType _ScaleType;

        [DefaultValue(TrackBarScaleType.None)]
        public TrackBarScaleType ScaleType
        {
            set
            {
                if (_ScaleType != value)
                {
                    _ScaleType = value;
                    ScaleTypeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _ScaleType;
        }
        public event EventHandler ScaleTypeChanged;

        private float _Increment;
        [DefaultValue(1f)]
        public float Increment
        {
            set
            {
                if (_Increment != value)
                {
                    _Increment = value;
                    IncrementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Increment;
        }
        public event EventHandler IncrementChanged;

        private Color _MinColor;
        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MinColor
        {
            set
            {
                if (_MinColor != value)
                {
                    _MinColor = value;
                    MinColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _MinColor;
        }
        public event EventHandler MinColorChanged;

        private Color _MaxColor;
        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MaxColor
        {
            set
            {
                if (_MaxColor != value)
                {
                    _MaxColor = value;
                    MaxColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _MaxColor;
        }
        public event EventHandler MaxColorChanged;

        [DefaultValue("")]
        public string Title
        {
            set => label1.Text = value;
            get => label1.Text;
        }

        private Control _control;

        public AnimationFloatControl()
        {
            InitializeComponent();

            typeComboBox.DataSource = Enum.GetValues(typeof(AnimationFloatType));

            _Increment = 1;
            _MinColor = Color.Black;
            _MaxColor = Color.Black;
            
            Disposed += MovementControl_Disposed;
        }

        private void MovementControl_Disposed(object sender, EventArgs e)
        {
            if (_Value != null)
            {
                _Value.PropertyChanged -= Value_PropertyChanged;
            }
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Type":
                    ResetControl();
                    break;
            }
        }

        private void ResetControl()
        {
            Control newControl = null;

            if (_Value != null)
            {
                switch (_Value.Type)
                {
                    case AnimationFloatType.Constant:
                        {
                            if (!(_control is AnimationFloatConstantControl control))
                            { 
                                control = new AnimationFloatConstantControl();
                                control.DataBindings.Add("ScaleType", this, "ScaleType", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("Increment", this, "Increment", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MinColor", this, "MinColor", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MaxColor", this, "MaxColor", false, DataSourceUpdateMode.Never);
                            }
                            control.Constant = (AnimationFloatConstant)_Value.Impl;
                            newControl = control;
                        }
                        break;
                    case AnimationFloatType.Linear:
                        {
                            if (!(_control is AnimationFloatLinearControl control))
                            {
                                control = new AnimationFloatLinearControl();
                                control.DataBindings.Add("ScaleType", this, "ScaleType", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("Increment", this, "Increment", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MinColor", this, "MinColor", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MaxColor", this, "MaxColor", false, DataSourceUpdateMode.Never);
                            }
                            control.Linear = (AnimationFloatLinear)_Value.Impl;
                            newControl = control;
                        }
                        break;
                    case AnimationFloatType.Curve:
                        {
                            if (!(_control is AnimationFloatCurveControl control))
                            {
                                control = new AnimationFloatCurveControl();
                                control.DataBindings.Add("Increment", this, "Increment", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MinColor", this, "MinColor", false, DataSourceUpdateMode.Never);
                                control.DataBindings.Add("MaxColor", this, "MaxColor", false, DataSourceUpdateMode.Never);
                            }
                            control.Curve = (AnimationFloatCurve)_Value.Impl;
                            newControl = control;
                        }
                        break;
                }
            }

            if (_control != newControl)
            {
                SuspendLayout();

                _control?.Dispose();

                if (newControl != null)
                {
                    newControl.Location = new Point(0, 23);
                    newControl.Width = Width;
                    newControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Controls.Add(newControl);

                    Height = 23 + newControl.Height;
                }
                else
                {
                    Height = 20;
                }

                _control = newControl;

                ResumeLayout();
            }
        }
    }
}
