using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Drawing.WinForms
{
    public class GraphicsUpdateEventArgs : EventArgs
    {
        public float Delta { private set; get; }

        public GraphicsUpdateEventArgs(float delta)
        {
            Delta = delta;
        }
    }

    public class GraphicsControl : Control
    {
        private Control _internal;
        private Timer _timer;
        private bool _updating;
        private long _elapsed;
        private float _delta;

        public Graphics Graphics { private set; get; }      //lock graphics for updating

        public event EventHandler GraphicsCreated;
        public event EventHandler GraphicsDestroying;
        public event EventHandler GraphicsResize;
        public event EventHandler<GraphicsUpdateEventArgs> GraphicsUpdate;
        public event EventHandler GraphicsDraw;

        public GraphicsControl()
        {
            if (GraphicsContext.IsCreated)
            {
                _internal = GraphicsContext.Instance.CreateControl();
                _internal.HandleCreated += Internal_HandleCreated;
                _internal.HandleDestroyed += Internal_HandleDestroyed;
                _internal.VisibleChanged += Internal_VisibleChanged;
                _internal.Resize += Internal_Resize;
                _internal.Paint += Internal_Paint;
                _internal.MouseClick += (object sender, MouseEventArgs e) => OnMouseClick(e);
                _internal.MouseDoubleClick += (object sender, MouseEventArgs e) => OnMouseDoubleClick(e);
                _internal.MouseDown += (object sender, MouseEventArgs e) => OnMouseDown(e);
                _internal.MouseUp += (object sender, MouseEventArgs e) => OnMouseUp(e);
                _internal.MouseMove += (object sender, MouseEventArgs e) => OnMouseMove(e);
                _internal.MouseWheel += (object sender, MouseEventArgs e) => OnMouseWheel(e);
                _internal.MouseEnter += (object sender, EventArgs e) => OnMouseEnter(e);
                _internal.MouseHover += (object sender, EventArgs e) => OnMouseHover(e);
                _internal.MouseLeave += (object sender, EventArgs e) => OnMouseLeave(e);
                _internal.Dock = DockStyle.Fill;
                Controls.Add(_internal);

                _timer = new Timer
                {
                    Interval = 1
                };
                _timer.Tick += Timer_Tick;
            }

            _updating = true;

            Disposed += GraphicsControl_Disposed;
        }

        private void GraphicsControl_Disposed(object sender, EventArgs e)
        {
            _internal?.Dispose();
            _timer?.Dispose();
            Graphics?.Dispose();
        }

        private void Internal_VisibleChanged(object sender, EventArgs e)
        {
            if (_internal.IsHandleCreated && _updating)
            {
                if (_timer.Enabled = _internal.Visible)
                {
                    _elapsed = DateTime.Now.Ticks;
                    _delta = 0;
                }
            }
        }

        private void Internal_HandleCreated(object sender, EventArgs e)
        {
            GraphicsContext.Instance.MakeCurrent(_internal);     //glControl 내부에서 current를 메인컨텍스트로 교체하므로 리소스공유가 안됨. 리셋해줘야 함
            Graphics = new Graphics(GraphicsContext.Instance.CreateControlRenderTarget(_internal));
            Graphics.Focus();
            OnGraphicsCreated();
            Graphics.UnFocus();
            GraphicsContext.Instance.ResetCurrent();

            if (_internal.Visible && _updating)
            {
                _elapsed = DateTime.Now.Ticks;
                _delta = 0;
                _timer.Enabled = true;
            }
        }

        private void Internal_HandleDestroyed(object sender, EventArgs e)
        {
            if (_updating) _timer.Enabled = false;

            GraphicsContext.Instance.MakeCurrent(_internal);     //glControl 내부에서 current를 메인컨텍스트로 교체하므로 리소스공유가 안됨. 리셋해줘야 함
            OnGraphicsDestroying();
            Graphics.Dispose();
            GraphicsContext.Instance.ResetCurrent();
            GraphicsContext.Instance.Finish();
        }

        private void Internal_Resize(object sender, EventArgs e)
        {
            if (_internal.IsHandleCreated)
            {
                GraphicsContext.Instance.MakeCurrent(_internal);     //glControl 내부에서 current를 메인컨텍스트로 교체하므로 리소스공유가 안됨. 리셋해줘야 함
                Graphics.Focus();
                Graphics.Target.Resize(Width, Height);
                Graphics.Camera.Width = Width;
                Graphics.Camera.Height = Height;
                OnGraphicsResize();
                Graphics.UnFocus();
                GraphicsContext.Instance.ResetCurrent();
            }
        }

        [DefaultValue(0)]
        public int SyncFrame { set; get; }

        [DefaultValue(true)]
        public bool Updating
        {
            set
            {
                if (_updating != value)
                {
                    _updating = value;

                    if (_internal != null && _internal.IsHandleCreated && _internal.Visible)
                    {
                        if (_timer.Enabled = _updating)
                        {
                            _elapsed = DateTime.Now.Ticks;
                            _delta = 0;
                        }
                    }
                }
            }
            get => _updating;
        }

        private void Update(float delta)
        {
            GraphicsContext.Instance.MakeCurrent(_internal);
            Graphics.Focus();
            OnGraphicsUpdate(delta);
            OnGraphicsDraw();
            Graphics.Render();
            GraphicsContext.Instance.SwapBuffers();
            GraphicsContext.Instance.ResetCurrent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Graphics.Remaining <= SyncFrame)
            {
                Update(_delta);

                GraphicsContext.Instance.Invoke(false, (GraphicsApi api) =>
                {
                    var current = DateTime.Now.Ticks;
                    _delta = ((current - _elapsed) / TimeSpan.TicksPerMillisecond) * 0.001f;
                    _elapsed = current;
                });
            }
        }

        private void RefreshInternal()
        {
            if (_internal != null && !_updating && Graphics.Remaining == 0) Update(0);
        }

        private void Internal_Paint(object sender, PaintEventArgs e)
        {
            RefreshInternal();
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            RefreshInternal();
            base.OnInvalidated(e);
        }

        public override void Refresh()
        {
            RefreshInternal();
            //base.Refresh();       //실제 랜더를 랜더쓰레드에서 하므로 불필요 화면 갱신을 하지 않는다.
        }

        protected virtual void OnGraphicsCreated()
        {
            GraphicsCreated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGraphicsDestroying()
        {
            GraphicsDestroying?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGraphicsResize()
        {
            GraphicsResize?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnGraphicsUpdate(float delta)
        {
            GraphicsUpdate?.Invoke(this, new GraphicsUpdateEventArgs(delta));
        }

        protected virtual void OnGraphicsDraw()
        {
            GraphicsDraw?.Invoke(this, EventArgs.Empty);
        }
    }
}
