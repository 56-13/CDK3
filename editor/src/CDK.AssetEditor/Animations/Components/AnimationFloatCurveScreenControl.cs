using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace CDK.Assets.Animations.Components
{
    partial class AnimationFloatCurveScreenControl : Control
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
                    if (_Curve != null)
                    {
                        _Curve.Refresh -= Curve_Refresh;
                    }
                    _Curve = value;
                    if (_Curve != null)
                    {
                        _Curve.Refresh += Curve_Refresh;
                    }

                    ResetScale();

                    Invalidate();

                    CurvePoint = null;
                }
            }
            get => _Curve;
        }

        private void Curve_Refresh(object sender, EventArgs e)
        {
            if (_CurvePoint != null && !_Curve.ContainsPoint(_CurvePoint))
            {
                CurvePoint = null;
            }
            Refresh();
        }

        private AnimationFloatCurvePoint _CurvePoint;
        public AnimationFloatCurvePoint CurvePoint
        {
            set
            {
                if (_CurvePoint != value)
                {
                    _CurvePoint = value;
                    Invalidate();

                    CurvePointChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _CurvePoint;
        }

        public event EventHandler CurvePointChanged;

        private float _Increment;

        [DefaultValue(1f)]
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

        private int _timeScale;
        private int _valueScale;
        private int _maxValueScale;
        private float _displayMinimumValue;
        private float _displayMaximumValue;
        private float _displayMinimumTime;
        private float _displayMaximumTime;

        private const int MaxTimeScale = 3;

        private void ResetScale()
        {
            if (_Curve != null && Height != 0)
            {
                if (_Curve.MaxValue > _Curve.MinValue)
                {
                    _maxValueScale = Math.Max((int)((_Curve.MaxValue - _Curve.MinValue) * 5 / Height / _Increment), 1);
                    _valueScale = (_maxValueScale + 1) / 2;
                }
                else
                {
                    _maxValueScale = 1;
                    _valueScale = 1;
                }
                _timeScale = 1;
                _displayMinimumTime = 0;
                _displayMaximumTime = 1;

                for (; ; )
                {
                    if (ApplyValueScale() && _valueScale > 1) _valueScale--;
                    else break;
                }
            }
        }

        private void ApplyTimeScale()
        {
            if (_timeScale == 1)
            {
                _displayMinimumTime = 0;
                _displayMaximumTime = 1;
            }
            else
            {
                _displayMinimumTime = 0.5f - 0.5f / _timeScale;
                _displayMaximumTime = 0.5f + 0.5f / _timeScale;

                if (_CurvePoint != null)
                {
                    if (_CurvePoint.T < _displayMinimumTime + 0.1f)
                    {
                        _displayMinimumTime = Math.Max(_CurvePoint.T - 0.1f, 0.0f);
                        _displayMaximumTime = _displayMinimumTime + 1.0f / _timeScale;
                    }
                    else if (_CurvePoint.T > _displayMaximumTime - 0.1f)
                    {
                        _displayMaximumTime = Math.Min(_CurvePoint.T + 0.1f, 1.0f);
                        _displayMinimumTime = _displayMaximumTime - 1.0f / _timeScale;
                    }
                }
            }
        }

        private bool ApplyValueScale()
        {
            var d = (_Curve.MaxValue - _Curve.MinValue) / 2;
            var c = (_Curve.MaxValue + _Curve.MinValue) / 2;
            _displayMinimumValue = c - d / _valueScale;
            _displayMaximumValue = c + d / _valueScale;

            var min = _Curve.MaxValue;
            var max = _Curve.MinValue;

            for (var i = 0; i < _Curve.PointCount; i++)
            {
                var p = _Curve.GetPoint(i);
                var pmin = Math.Max(p.V - p.VVar, _Curve.MinValue);
                var pmax = Math.Min(p.V + p.VVar, _Curve.MaxValue);

                if (pmin < min)
                {
                    min = pmin;
                }
                if (pmax > max)
                {
                    max = pmax;
                }
            }
            for (var i = 0; i < _Curve.PointCount - 1; i++)
            {
                var p0 = _Curve.GetPoint(i);
                var p1 = _Curve.GetPoint(i + 1);

                var t = (p0.T + p1.T) / 2;

                var pmin = _Curve.GetValue(t, CurveValueType.Min);
                var pmax = _Curve.GetValue(t, CurveValueType.Max);

                if (pmin < min)
                {
                    min = pmin;
                }
                if (pmax > max)
                {
                    max = pmax;
                }
            }
            if (min < _displayMinimumValue)
            {
                var t = _displayMinimumValue - min;
                _displayMinimumValue -= t;
                _displayMaximumValue -= t;
            }
            else if (max > _displayMaximumValue)
            {
                var t = max - _displayMaximumValue;
                _displayMinimumValue += t;
                _displayMaximumValue += t;
            }
            return min < _displayMinimumValue || max > _displayMaximumValue;
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

        private int _prevX;
        private int _prevY;

        public AnimationFloatCurveScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);  

            InitializeComponent();

            ContextMenu = new ContextMenu();        //disable other context menu

            _Increment = 1;

            _MinColor = Color.Black;
            _MaxColor = Color.Black;

            Disposed += CurveScreenControl_Disposed;
        }

        private void CurveScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Curve != null)
            {
                _Curve.Refresh -= Curve_Refresh;
            }
        }

        private struct GridValue
        {
            public float Degree { private set; get; }
            public int DecimalPlaces { private set; get; }

            public GridValue(float degree, int decimalPlaces)
                : this()
            {
                Degree = degree;
                DecimalPlaces = decimalPlaces;
            }
        }

        private static GridValue[] gridValues = new GridValue[]{
            new GridValue(0.001f, 3),
            new GridValue(0.005f, 3),
            new GridValue(0.01f, 2),
            new GridValue(0.05f, 2),
            new GridValue(0.1f, 1),
            new GridValue(0.5f, 1),
            new GridValue(1, 0),
            new GridValue(5, 0),
            new GridValue(10, 0),
            new GridValue(50, 0),
            new GridValue(100, 0),
            new GridValue(500, 0),
            new GridValue(1000, 0)
        };

        private const float GridTime = 0.05f;

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_Curve != null && Width > 1 && Height > 1)
            {
                var points = new Point[Width];

                var ytov = (Height - 1) / (_displayMaximumValue - _displayMinimumValue);

                using (var brush = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(64, _MaxColor), Color.FromArgb(64, _MinColor)))
                {
                    var gi = 0;
                    while (gi < gridValues.Length - 1 && gridValues[gi].Degree * ytov < 12)
                    {
                        gi++;
                    }
                    using (var pen = new Pen(brush))
                    {
                        var w = 0f;
                        using (var font = new Font(Font.FontFamily, 8))
                        {
                            var gd = gridValues[gi].Degree;
                            var v = (int)(_displayMaximumValue / gd + 1) * gd;

                            for (; ; )
                            {
                                var y = Height - 1 - (v - _displayMinimumValue) * ytov;

                                if (y < Height)
                                {
                                    var str = string.Format("{0:F" + gridValues[gi].DecimalPlaces + "}", v);
                                    var size = pe.Graphics.MeasureString(str, font);
                                    pe.Graphics.DrawString(str, font, brush, 0, y - size.Height / 2);
                                    pe.Graphics.DrawLine(pen, size.Width, y, Width - 1, y);
                                    v -= gd;

                                    if (w < size.Width) w = size.Width;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        var t = (int)(_displayMinimumTime / GridTime) * GridTime;

                        for (; ; )
                        {
                            var x = (t - _displayMaximumTime) * (Width - 1) / (_displayMaximumTime - _displayMinimumTime);

                            if (x < Width)
                            {
                                if (x >= w)
                                {
                                    pe.Graphics.DrawLine(pen, x, 0, x, Height - 1);
                                }
                                t += GridTime;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (_Curve.Variant)
                    {
                        using (var pen = new Pen(brush))
                        {
                            for (var x = 0; x < Width; x++)
                            {
                                var t = Drawing.MathUtil.Lerp(_displayMinimumTime, _displayMaximumTime, (float)x / (Width - 1));
                                var sv = _Curve.GetValue(t, CurveValueType.Max);
                                var dv = _Curve.GetValue(t, CurveValueType.Min);

                                var sy = (int)Math.Round(Height - 1 - (sv - _displayMinimumValue) * ytov);
                                var dy = (int)Math.Round(Height - 1 - (dv - _displayMinimumValue) * ytov);

                                if (sy != dy)
                                {
                                    pe.Graphics.DrawLine(pen, x, sy, x, dy);
                                }
                            }
                        }
                    }
                }
                using (var brush = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), _MaxColor, _MinColor))
                {
                    using (var pen = new Pen(brush))
                    {
                        for (var x = 0; x < Width; x++)
                        {
                            var t = Drawing.MathUtil.Lerp(_displayMinimumTime, _displayMaximumTime, (float)x / (Width - 1));
                            var v = _Curve.GetValue(t, CurveValueType.Center);
                            var y = (int)Math.Round(Height - 1 - (v - _displayMinimumValue) * ytov);
                            points[x] = new Point(x, y);
                        }
                        pe.Graphics.DrawLines(pen, points);
                    }
                }
                for (var i = 0; i < _Curve.PointCount; i++)
                {
                    var p = _Curve.GetPoint(i);

                    var x = (p.T - _displayMinimumTime) * (Width - 1) / (_displayMaximumTime - _displayMinimumTime);
                    var y = Height - 1 - (p.V - _displayMinimumValue) * ytov;

                    if (p == _CurvePoint)
                    {
                        pe.Graphics.FillEllipse(Brushes.Black, x - 5, y - 5, 10, 10);

                        float ax, ay, aa;

                        pe.Graphics.TranslateTransform(x, y);
                        ax = (float)(Math.Cos(p.LeftAngle * Drawing.MathUtil.ToRadians) * Width);
                        ay = (float)(Math.Sin(p.LeftAngle * Drawing.MathUtil.ToRadians) * Height * _valueScale);
                        aa = (float)Math.Atan2(ay, ax);
                        pe.Graphics.RotateTransform(-aa * Drawing.MathUtil.ToDegrees);

                        pe.Graphics.DrawLine(Pens.Black, -30, 0, 0, 0);

                        pe.Graphics.ResetTransform();

                        pe.Graphics.TranslateTransform(x, y);
                        ax = (float)(Math.Cos(p.RightAngle * Drawing.MathUtil.ToRadians) * Width);
                        ay = (float)(Math.Sin(p.RightAngle * Drawing.MathUtil.ToRadians) * Height * _valueScale);
                        aa = (float)Math.Atan2(ay, ax);
                        pe.Graphics.RotateTransform(-aa * Drawing.MathUtil.ToDegrees);

                        pe.Graphics.DrawLine(Pens.Black, 0, 0, 30, 0);

                        pe.Graphics.ResetTransform();
                    }
                    else
                    {
                        using (var brush = new SolidBrush(ForeColor))
                        {
                            pe.Graphics.FillEllipse(brush, x - 3, y - 3, 6, 6);
                        }
                    }
                }
            }
        }
        
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_Curve != null && Width > 1 && Height > 1)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            var t = Drawing.MathUtil.Lerp(_displayMinimumTime, _displayMaximumTime, (float)e.X / (Width - 1));
                            
                            CurvePoint = _Curve.AddPoint(t);
                        }
                        break;
                    case MouseButtons.Right:
                        if (_CurvePoint != null)
                        {
                            var left = true;
                            var right = true;
                            if ((ModifierKeys & Keys.Control) != 0)
                            {
                                if (e.X >= _CurvePoint.T * (Width - 1)) left = false;
                                else right = false;
                            }
                            _CurvePoint.LinearRotate(left, right);
                        }
                        break;
                }
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_Curve != null && Width > 1 && Height > 1)
            {
                Focus();

                switch (e.Button)
                {
                    case MouseButtons.Left:
                    case MouseButtons.Right:
                        {
                            var t = Drawing.MathUtil.Lerp(_displayMinimumTime, _displayMaximumTime, (float)e.X / (Width - 1));

                            CurvePoint = _Curve.SelectPoint(t);

                            _prevX = e.X;
                            _prevY = e.Y;

                            Refresh();
                        }
                        break;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_Curve != null && Width > 1 && Height > 1)
            {
                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if (_CurvePoint != null)
                        {
                            if (shiftKey && _Curve.Variant)
                            {
                                var v = (float)(_prevY - e.Y) * (_displayMaximumValue - _displayMinimumValue) / (Height - 1);

                                _CurvePoint.VVar += v;
                            }
                            else
                            {
                                var mv = true;
                                var mt = true;
                                if (controlKey)
                                {
                                    if (Math.Abs(e.Y - _prevY) >= Math.Abs(e.X - _prevX))
                                    {
                                        mt = false;
                                    }
                                    else
                                    {
                                        mv = false;
                                    }
                                }
                                if (mv)
                                {
                                    var v = (float)(_prevY - e.Y) * (_displayMaximumValue - _displayMinimumValue) / (Height - 1);

                                    _CurvePoint.V += v;
                                }
                                if (mt)
                                {
                                    var t = (_displayMaximumTime - _displayMinimumTime) * (float)(e.X - _prevX) / (Width - 1);

                                    _CurvePoint.T += t;
                                }
                            }
                            _prevX = e.X;
                            _prevY = e.Y;
                        }
                        break;
                    case MouseButtons.Right:
                        if (_CurvePoint != null)
                        {
                            var a = (e.Y - _prevY) * 180.0f / ((Height - 1) * _valueScale);

                            var left = true;
                            var right = true;
                            if (e.X >= _CurvePoint.T * (Width - 1))
                            {
                                a = -a;
                                if (controlKey) left = false;
                            }
                            else
                            {
                                if (controlKey) right = false;
                            }
                            if (left)
                            {
                                _CurvePoint.LeftAngle += a;
                            }
                            if (right)
                            {
                                _CurvePoint.RightAngle += a;
                            }
                            _prevX = e.X;
                            _prevY = e.Y;
                        }
                        break;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (_Curve != null)
            {
                ((HandledMouseEventArgs)e).Handled = true;

                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                if (controlKey || shiftKey)
                {
                    var v = _Increment * Math.Sign(e.Delta);

                    for (var i = 0; i < _Curve.PointCount; i++)
                    {
                        var p = _Curve.GetPoint(i);

                        if (shiftKey && _Curve.Variant)
                        {
                            p.VVar += v;
                        }
                        else
                        {
                            p.V += v;
                        }
                    }
                }
                else
                {
                    int d = Math.Max(_maxValueScale / 100, 1) * Math.Sign(e.Delta);

                    _valueScale = CDK.Drawing.MathUtil.Clamp(_valueScale + d, 1, _maxValueScale);

                    ApplyValueScale();
                    Invalidate();
                }
            }
            base.OnMouseWheel(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_Curve != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.T -= 0.01f;
                        }
                        break;
                    case Keys.Right:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.T += 0.01f;
                        }
                        break;
                    case Keys.Up:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.V += _Increment;
                        }
                        break;
                    case Keys.Down:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.V -= _Increment;
                        }
                        break;
                    case Keys.Q:
                    case Keys.Subtract:
                        if (_valueScale > 1)
                        {
                            _valueScale--;
                            ApplyValueScale();
                            Invalidate();
                        }
                        break;
                    case Keys.W:
                    case Keys.Add:
                        if (_valueScale < _maxValueScale)
                        {
                            _valueScale++;
                            ApplyValueScale();
                            Invalidate();
                        }
                        break;
                    case Keys.E:
                        if (_timeScale > 1)
                        {
                            _timeScale--;
                            ApplyTimeScale();
                            Invalidate();
                        }
                        break;
                    case Keys.R:
                        if (_timeScale < MaxTimeScale)
                        {
                            _timeScale++;
                            ApplyTimeScale();
                            Invalidate();
                        }
                        break;
                    case Keys.A:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.LeftAngle -= 1.0f / _valueScale;
                            _CurvePoint.RightAngle -= 1.0f / _valueScale;
                        }
                        break;
                    case Keys.S:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.LeftAngle += 1.0f / _valueScale;
                            _CurvePoint.RightAngle += 1.0f / _valueScale;
                        }
                        break;
                    case Keys.D:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.LeftAngle -= 1.0f / _valueScale;
                        }
                        break;
                    case Keys.F:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.LeftAngle += 1.0f / _valueScale;
                        }
                        break;
                    case Keys.G:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.RightAngle -= 1.0f / _valueScale;
                        }
                        break;
                    case Keys.H:
                        if (_CurvePoint != null)
                        {
                            _CurvePoint.RightAngle += 1.0f / _valueScale;
                        }
                        break;
                    case Keys.Back:
                        if (MessageBox.Show(this, "수치를 초기화하시겠습니까?", "초기화", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            _Curve.ResetPoints();
                        }
                        break;
                    case Keys.Delete:
                        if (_CurvePoint != null)
                        {
                            _Curve.RemovePoint(_CurvePoint);
                        }
                        break;
                }
            }
            
            base.OnKeyDown(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ResetScale();

            base.OnSizeChanged(e);
        }
    }
}
