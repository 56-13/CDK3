using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Components
{
    partial class AnimationColorChannelScreenControl : Control
    {
        private AnimationColorChannel _Impl;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationColorChannel Impl
        {
            set
            {
                if (_Impl != null)
                {
                    _Impl.Red.Refresh -= Impl_Refresh;
                    _Impl.Green.Refresh -= Impl_Refresh;
                    _Impl.Blue.Refresh -= Impl_Refresh;
                    _Impl.Alpha.Refresh -= Impl_Refresh;

                    _Impl.PropertyChanged -= Impl_PropertyChanged;
                }
                _Impl = value;
                if (_Impl != null)
                {
                    _Impl.Red.Refresh += Impl_Refresh;
                    _Impl.Green.Refresh += Impl_Refresh;
                    _Impl.Blue.Refresh += Impl_Refresh;
                    _Impl.Alpha.Refresh += Impl_Refresh;

                    _Impl.PropertyChanged += Impl_PropertyChanged;
                }
                Invalidate();

                ImplChanged?.Invoke(this, EventArgs.Empty);
            }
            get => _Impl;
        }
        public event EventHandler ImplChanged;

        public AnimationColorChannelScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true); 

            Disposed += AnimationColorScreenControl_Disposed;
        }

        private void AnimationColorScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Impl != null)
            {
                _Impl.Red.Refresh -= Impl_Refresh;
                _Impl.Green.Refresh -= Impl_Refresh;
                _Impl.Blue.Refresh -= Impl_Refresh;
                _Impl.Alpha.Refresh -= Impl_Refresh;

                _Impl.PropertyChanged -= Impl_PropertyChanged;
            }
        }

        private void Impl_Refresh(object sender, EventArgs e) => Invalidate();

        private void Impl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FixedComponent") Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            using (var brush = new HatchBrush(HatchStyle.LargeCheckerBoard, System.Drawing.Color.DarkGray, System.Drawing.Color.LightGray))
            {
                pe.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, Height));
            }

            if (_Impl != null)
            {
                for (var y = 0; y < Height; y++)
                {
                    var rate = (float)(Height - y) / Height;

                    for (var x = 0; x < Width; x++)
                    {
                        var t = (float)x / Width;

                        var r = Math.Min(_Impl.Red.GetValue(t, rate), 1);
                        var g = Math.Min(_Impl.Green.GetValue(t, rate), 1);
                        var b = Math.Min(_Impl.Blue.GetValue(t, rate), 1);
                        var a = Math.Min(_Impl.Alpha.GetValue(t, rate), 1);

                        var c = System.Drawing.Color.FromArgb(
                            (int)(a * 255),
                            (int)(r * 255),
                            (int)(g * 255),
                            (int)(b * 255));

                        using (var brush = new SolidBrush(c))
                        {
                            pe.Graphics.FillRectangle(brush, x, y, 1, 1);
                        }
                    }
                }
            }

            pe.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));

            base.OnPaint(pe);
        }
    }
}
