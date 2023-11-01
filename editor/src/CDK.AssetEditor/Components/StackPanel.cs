using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace CDK.Assets.Components
{
    public class StackPanel : Panel
    {
        private bool _Collapsible;
        [DefaultValue(false)]
        public bool Collapsible
        {
            set
            {
                if (_Collapsible != value)
                {
                    _Collapsible = value;

                    UpdateLayout();

                    CollapsibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Collapsible;
        }
        public event EventHandler CollapsibleChanged;

        private bool _Collapsed;
        [DefaultValue(false)]
        public bool Collapsed
        {
            set
            {
                if (_Collapsed != value)
                {
                    _Collapsed = value;

                    if (_Collapsible)
                    {
                        UpdateLayout();

                        Invalidate();
                    }

                    CollapsedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Collapsible && _Collapsed;
        }
        public event EventHandler CollapsedChanged;

        private bool _AutoSize;
        [DefaultValue(false)]
        public override bool AutoSize
        {
            set
            {
                if (value != _AutoSize)
                {
                    _AutoSize = value;

                    if (_AutoSize) UpdateLayout();

                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
            get => _AutoSize;
        }

        private string _Title;
        [DefaultValue("")]
        public string Title
        {
            set
            {
                if (_Title != value)
                {
                    if (value == null) value = string.Empty;

                    _Title = value;

                    if (_Collapsible) Invalidate();

                    TitleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Title;
        }
        public event EventHandler TitleChanged;

        private int _RowHeight;
        [DefaultValue(DefaultRowHeight)]
        public int RowHeight
        {
            set
            {
                if (_RowHeight != value)
                {
                    _RowHeight = value;

                    if (_Collapsible) UpdateLayout();

                    RowHeightChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RowHeight;
        }
        public event EventHandler RowHeightChanged;

        private Color _RowColor0;
        [DefaultValue(typeof(Color), "0xFFFFFFFF")]
        public Color RowColor0
        {
            set
            {
                if (_RowColor0 != value)
                {
                    _RowColor0 = value;

                    if (_Collapsible) Invalidate();

                    RowColor0Changed?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RowColor0;
        }
        public event EventHandler RowColor0Changed;

        private Color _RowColor1;
        [DefaultValue(typeof(Color), "0xFF696969")]
        public Color RowColor1
        {
            set
            {
                if (_RowColor1 != value)
                {
                    _RowColor1 = value;

                    if (_Collapsible) Invalidate();

                    RowColor1Changed?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RowColor1;
        }
        public event EventHandler RowColor1Changed;

        private Color _RowColor2;
        [DefaultValue(typeof(Color), "0xFFD3D3D3")]
        public Color RowColor2
        {
            set
            {
                if (_RowColor2 != value)
                {
                    _RowColor2 = value;

                    if (_Collapsible) Invalidate();

                    RowColor2Changed?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RowColor2;
        }
        public event EventHandler RowColor2Changed;

        private const int DefaultRowHeight = 21;

        private List<Control> _controls;
        private int _prevHeight;
        private bool _updateLayout;

        public StackPanel()
        {
            _Title = string.Empty;

            _RowHeight = DefaultRowHeight;
            _RowColor0 = Color.White;
            _RowColor1 = Color.DimGray;
            _RowColor2 = Color.LightGray;

            _controls = new List<Control>();

            _updateLayout = true;
        }

        private void UpdateLayout()
        {
            _updateLayout = false;

            SuspendLayout();

            if (Collapsed)
            {
                if (_prevHeight == 0) _prevHeight = Height;

                foreach (Control control in Controls)
                {
                    _controls.Add(control);
                }
                Controls.Clear();

                Height = _RowHeight;
            }
            else
            {
                foreach (Control control in _controls)
                {
                    Controls.Add(control);
                }
                _controls.Clear();

                var controls = new Control[Controls.Count];
                Controls.CopyTo(controls, 0);
                for (var i = 0; i < controls.Length; i++)
                {
                    var y = controls[i].Top;
                    var index = 0;
                    for (var j = 0; j < controls.Length; j++)
                    {
                        if (i != j) {
                            var top = controls[j].Top;
                            if (y > top || (y == top && i > j)) index++;
                        }
                    }
                    Controls.SetChildIndex(controls[i], index);
                }
                {
                    var padding = Padding;
                    var y = padding.Top;
                    
                    if (_Collapsible) y += _RowHeight;

                    foreach (Control control in Controls)
                    {
                        var margin = control.Margin;

                        control.Location = new Point(padding.Left + margin.Left, y + margin.Top);
                        control.Width = Width - control.Left - (margin.Right + padding.Right);
                        if (control.Visible) y = control.Bottom + margin.Bottom;
                    }

                    if (_AutoSize) Height = y + padding.Bottom;
                    else if (_Collapsible && _prevHeight != 0) Height = _prevHeight;
                    _prevHeight = 0;
                }
            }

            ResumeLayout();

            _updateLayout = true;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (_updateLayout) UpdateLayout();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (_updateLayout && Collapsed)
            {
                _controls.Add(e.Control);
                Controls.Remove(e.Control);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_Collapsible)
            {
                var rect = new Rectangle(0, 0, Width, _RowHeight);

                using (var brush = new LinearGradientBrush(rect, _RowColor1, _RowColor2, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                var y = (_RowHeight - e.Graphics.MeasureString(_Title, Font).Height) / 2;

                using (var brush = new SolidBrush(_RowColor0))
                {
                    e.Graphics.DrawString(_Collapsed ? "▽" : "▼", Font, Brushes.White, 6, y);
                    e.Graphics.DrawString(_Title, Font, Brushes.White, 22, y);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_Collapsible && e.Y <= _RowHeight)
            {
                Collapsed = !_Collapsed;
            }
        }
    }
}
