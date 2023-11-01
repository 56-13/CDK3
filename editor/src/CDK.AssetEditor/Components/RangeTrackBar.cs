using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDK.Assets.Components
{
    public partial class RangeTrackBar : Control
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

                    Invalidate();
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

                    Invalidate();
                }
            }
            get => _Maximum;
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

                    _scale = int.MaxValue;

                    ResetScale();
                }
            }
            get => _ScaleType;
        }

        private float _Value;
        
        [DefaultValue(0)]
        public float Value
        {
            set
            {
                if (_Value != value)
                {
                    _Value = value;

                    if (_ScaleType == TrackBarScaleType.Linear)
                    {
                        if (_Value <= _displayMinimum || _Value >= _displayMaximum) ApplyScale();
                    }

                    Invalidate();

                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Value;
        }
        public event EventHandler ValueChanged;

        private float _Range;

        [DefaultValue(0)]
        public float Range
        {
            set
            {
                if (_Range != value)
                {
                    _Range = value;

                    Invalidate();

                    RangeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Range;
        }
        public event EventHandler RangeChanged;

        private float _Increment;

        [DefaultValue(1)]
        public float Increment
        {
            set
            {
                if (_Increment != value)
                {
                    _Increment = value;

                    ResetScale();
                }
            }
            get => _Increment;
        }

        private int _dragging;
        private int _dragX;
        private int _scale;
        private int _maxScale;
        private float _displayMinimum;
        private float _displayMaximum;

        private void ResetScale()
        {
            if (Width != 0 && _ScaleType == TrackBarScaleType.Linear && _Maximum > _Minimum)
            {
                _maxScale = Math.Max((int)((_Maximum - _Minimum) * 5 / Width / _Increment), 1);
                _scale = (_maxScale + 1) / 2;
            }
            else
            {
                _maxScale = 1;
                _scale = 1;
            }
            ApplyScale();
        }

        private void ApplyScale()
        {
            if (_scale != 1 && _ScaleType == TrackBarScaleType.Linear)
            {
                int i = Math.Min((int)((_Value - _Minimum) * _scale / (_Maximum - _Minimum)), _scale - 1);
                _displayMinimum = _Minimum + (_Maximum - _Minimum) * i / _scale;
                _displayMaximum = _Minimum + (_Maximum - _Minimum) * (i + 1) / _scale;
            }
            else
            {
                _displayMinimum = _Minimum;
                _displayMaximum = _Maximum;
            }
        }

        private Color _MinColor;
        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MinColor
        {
            set
            {
                if (_MinColor != value)
                {
                    _MinColor = value;
                    Invalidate();
                }
            }
            get => _MinColor;
        }

        private Color _MaxColor;
        [DefaultValue(typeof(Color), "0xFF000000")]
        public Color MaxColor
        {
            set
            {
                if (_MaxColor != value)
                {
                    _MaxColor = value;
                    Invalidate();
                }
            }
            get => _MaxColor;
        }

        public RangeTrackBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true); 

            InitializeComponent();

            _Increment = 1;
            _Maximum = 100;
            _MinColor = Color.Black;
            _MaxColor = Color.Black;

            ResetScale();
        }

        protected override void OnResize(EventArgs e)
        {
            ResetScale();

            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Width > 2 && Height > 2 && _displayMaximum > _displayMinimum)
            {
                using (var linearBrush = new LinearGradientBrush(Point.Empty, new Point(Width, 0), _MinColor, _MaxColor))
                {
                    using (var pen = new Pen(linearBrush))
                    {
                        pe.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

                        if (_ScaleType == TrackBarScaleType.Linear)
                        {
                            var tickFrequency = (float)(Width - 3) * _scale / _maxScale;
                            for (float x = 1; x < Width - 2; x += tickFrequency)
                            {
                                pe.Graphics.DrawLine(pen, x, Height - 3, x, Height - 2);
                            }
                        }
                    }
                    double dmin = _displayMinimum;
                    double dmax = _displayMaximum;
                    double vmin = _Value - _Range;
                    double vmax = _Value + _Range;

                    if (_ScaleType == TrackBarScaleType.Log)
                    {
                        dmin = Math.Log(dmin);
                        dmax = Math.Log(dmax);
                        vmin = Math.Log(vmin);
                        vmax = Math.Log(vmax);
                    }

                    var min = Drawing.MathUtil.Clamp((vmin - dmin) / (dmax - dmin), 0, 1);
                    var max = Drawing.MathUtil.Clamp((vmax - dmin) / (dmax - dmin), 0, 1);

                    if (min <= max)
                    {
                        var minx = 1 + (int)((Width - 3) * min);
                        var maxx = 1 + (int)((Width - 3) * max);
                        pe.Graphics.FillRectangle(linearBrush, minx, 1, maxx - minx + 1, Height - 2);
                    }
                }
            }
            base.OnPaint(pe);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_ScaleType == TrackBarScaleType.Linear)
            {
                ((HandledMouseEventArgs)e).Handled = true;

                var d = Math.Max(_maxScale / 100, 1) * Math.Sign(e.Delta);

                _scale = CDK.Drawing.MathUtil.Clamp(_scale + d, 1, _maxScale);

                ApplyScale();

                Invalidate();
            }

            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            if (e.Button == MouseButtons.Left)
            {
                if (_dragging == 0)
                {
                    AssetManager.Instance.BeginCommands();
                }
                _dragX = e.X;
                _dragging = 1;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (_dragging == 0)
                {
                    AssetManager.Instance.BeginCommands();
                }
                _dragX = e.X;
                _dragging = 2;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging != 0)
            {
                if (e.X != _dragX && Width > 1)
                {
                    double dmin = _displayMinimum;
                    double dmax = _displayMaximum;
                    
                    if (_ScaleType == TrackBarScaleType.Log)
                    {
                        dmin = Math.Log(dmin);
                        dmax = Math.Log(dmax);
                    }
                    var d = (e.X - _dragX) * (dmax - dmin) / (Width - 1);

                    double vmin, vmax;

                    if (_ScaleType == TrackBarScaleType.Log)
                    {
                        vmin = Math.Exp(Drawing.MathUtil.Clamp(Math.Log(_Value - _Range) + (_dragging == 1 ? d : -d), dmin, dmax));
                        vmax = Math.Max(Math.Exp(Drawing.MathUtil.Clamp(Math.Log(_Value + _Range) + d, dmin, dmax)), vmin);
                    }
                    else
                    {
                        vmin = Drawing.MathUtil.Clamp(_Value - _Range + (_dragging == 1 ? d : -d), _Minimum, _Maximum);
                        vmax = Drawing.MathUtil.Clamp(_Value + _Range + d, vmin, _Maximum);
                    }

                    Value = (float)((vmin + vmax) / 2);
                    Range = (float)((vmax - vmin) / 2);

                    _dragX = e.X;
                }
            }
            
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                if (_dragging != 0)
                {
                    _dragging = 0;
                    AssetManager.Instance.EndCommands();
                }
            }
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
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
                            Invalidate();
                        }
                        break;
                    case Keys.W:
                    case Keys.Add:
                        if (_scale < _maxScale)
                        {
                            _scale++;
                            ApplyScale();
                            Invalidate();
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (_dragging != 0)
            {
                _dragging = 0;
                AssetManager.Instance.EndCommands();
            }
            base.OnLeave(e);
        }
    }
}
