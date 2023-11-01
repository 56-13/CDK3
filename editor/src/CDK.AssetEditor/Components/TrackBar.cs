using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Components
{
    public enum TrackBarScaleType
    {
        None,
        Linear,
        Log
    }

    public partial class TrackBar : UserControl
    {

        private float _Minimum;

        [DefaultValue(0)]
        public float Minimum
        {
            set
            {
                if (_Minimum != value)
                {
                    _Minimum = value;

                    ResetScale();
                }
            }
            get => _Minimum;
        }

        private float _Maximum;

        [DefaultValue(100)]
        public float Maximum
        {
            set
            {
                if (_Maximum != value)
                {
                    _Maximum = value;

                    ResetScale();
                }
            }
            get => _Maximum;
        }

        private int _DecimalPlaces;

        [DefaultValue(0)]
        public int DecimalPlaces
        {
            set
            {
                if (_DecimalPlaces != value)
                {
                    _raiseValueChanged = false;

                    double prev = Value;

                    _DecimalPlaces = value;

                    ResetScale();

                    if (_ScaleType == TrackBarScaleType.Log) prev = Math.Log(prev);

                    _trackBar.Value = (int)Math.Round(prev * Math.Pow(10, _DecimalPlaces));

                    _raiseValueChanged = true;
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

                    ResetScale();

                    _trackBar.TickStyle = _ScaleType == TrackBarScaleType.Linear ? TickStyle.BottomRight : TickStyle.None;
                }
            }
            get => _ScaleType;
        }

        [DefaultValue(0)]
        public float Value
        {
            set
            {
                if (value >= _Minimum && value <= _Maximum && Value != value)
                {
                    _raiseValueChanged = false;

                    var pow = Math.Pow(10, _DecimalPlaces);

                    int v;
                    switch (_ScaleType)
                    {
                        case TrackBarScaleType.Linear:
                            v = (int)Math.Round(value * pow);
                            if (v <= _trackBar.Minimum || v >= _trackBar.Maximum) ApplyScale(value);
                            break;
                        case TrackBarScaleType.Log:
                            v = (int)(Math.Log(value) * pow);
                            break;
                        default:
                            v = (int)Math.Round(value * pow);
                            break;
                    }

                    _trackBar.Value = v;

                    _raiseValueChanged = true;

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get
            {
                var pow = Math.Pow(10, _DecimalPlaces);

                switch (_ScaleType)
                {
                    case TrackBarScaleType.Log:
                        return (float)Math.Exp(_trackBar.Value / pow);
                    default:
                        return (float)(_trackBar.Value / pow);
                }
            }
        }
        public event EventHandler ValueChanged;

        private int _scale;
        private int _maxScale;
        private bool _scrollStarted;
        private bool _raiseValueChanged;

        public void ResetScale()
        {
            if (Width != 0 && _ScaleType == TrackBarScaleType.Linear)
            {
                if (_Maximum > _Minimum)
                {
                    _maxScale = Math.Max((int)((_Maximum - _Minimum) * Math.Pow(10, _DecimalPlaces) / Width), 1);
                }
                else
                {
                    _maxScale = 1;
                }
                if (_scale > _maxScale)
                {
                    _scale = _maxScale;
                }
            }
            else
            {
                _scale = 1;
                _maxScale = 1;
            }
            ApplyScale();
        }

        private void ApplyScale() => ApplyScale(Value);

        private void ApplyScale(float value)
        {
            var pow = Math.Pow(10, _DecimalPlaces);

            if (_ScaleType == TrackBarScaleType.Log)
            {
                _trackBar.Minimum = (int)Math.Round(Math.Log(_Minimum) * pow);
                _trackBar.Maximum = (int)Math.Round(Math.Log(_Maximum) * pow);
                _trackBar.TickFrequency = 1;
            }
            else
            {
                if (_scale != 1)
                {
                    var r = (_Maximum - _Minimum) / _scale * 0.5f;
                    _trackBar.Minimum = (int)Math.Round(Math.Max(value - r, _Minimum) * pow);
                    _trackBar.Maximum = (int)Math.Round(Math.Min(value + r, _Maximum) * pow);
                }
                else
                {
                    _trackBar.Minimum = (int)Math.Round(_Minimum * pow);
                    _trackBar.Maximum = (int)Math.Round(_Maximum * pow);
                }
                _trackBar.TickFrequency = Math.Max(Width * _scale / _maxScale, 1);
            }
        }

        public TrackBar()
        {
            InitializeComponent();

            _Maximum = 100;

            ResetScale();

            _raiseValueChanged = true;

            _trackBar.MouseWheel += TrackBar_MouseWheel;
        }

        protected override void OnResize(EventArgs e)
        {
            ResetScale();

            base.OnResize(e);
        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (_raiseValueChanged)
            {
                if (_ScaleType == TrackBarScaleType.Linear)
                {
                    if (_trackBar.Value <= _trackBar.Minimum + 1)
                    {
                        var min = (int)Math.Round(Math.Pow(10, _DecimalPlaces) * _Minimum);

                        if (_trackBar.Minimum > min)
                        {
                            _trackBar.Minimum--;
                            _trackBar.Maximum--;
                        }
                    }
                    else if (_trackBar.Value >= _trackBar.Maximum - 1)
                    {
                        var max = (int)Math.Round(Math.Pow(10, _DecimalPlaces) * _Maximum);

                        if (_trackBar.Maximum < max)
                        {
                            _trackBar.Minimum++;
                            _trackBar.Maximum++;
                        }
                    }
                }

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void TrackBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (_ScaleType == TrackBarScaleType.Linear)
            {
                switch (e.KeyCode)
                {
                    case Keys.Q:
                    case Keys.Subtract:
                        if (_scale > 1)
                        {
                            _scale--;
                            ApplyScale();
                        }
                        break;
                    case Keys.W:
                    case Keys.Add:
                        if (_scale < _maxScale)
                        {
                            _scale++;
                            ApplyScale();
                        }
                        break;
                }
            }
        }

        private void TrackBar_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_ScaleType == TrackBarScaleType.Linear)
            {
                ((HandledMouseEventArgs)e).Handled = true;

                var d = Math.Max(_maxScale / 100, 1) * Math.Sign(e.Delta);

                _scale = Drawing.MathUtil.Clamp(_scale + d, 1, _maxScale);

                ApplyScale();
            }
        }

        private void TrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _scrollStarted = true;
                AssetManager.Instance.BeginCommands();
            }
        }

        private void TrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _scrollStarted)
            {
                _scrollStarted = false;
                AssetManager.Instance.EndCommands();
            }
        }

        private void TrackBar_Leave(object sender, EventArgs e)
        {
            if (_scrollStarted)
            {
                _scrollStarted = false;
                AssetManager.Instance.EndCommands();
            }
        }
    }
}
