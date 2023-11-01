using System;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;
using CDK.Drawing.WinForms;

namespace CDK.Assets.Scenes
{
    public partial class SceneScreenControl : GraphicsControl
    {
        private Scene _Scene;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            set
            {
                if (_Scene != value)
                {
                    if (_lightSpace != null)
                    {
                        _lightSpace.Dispose();
                        _lightSpace = null;
                    }

                    if (_Scene != null) _Scene.Refresh -= Scene_Refresh;

                    _Scene = value;

                    if (_Scene != null)
                    {
                        _Scene.Rewind();
                        _Scene.Refresh += Scene_Refresh;
                        
                        _Scene.ResetCamera();
                        if (_FitScreen) _Scene.FitCamera();
                    }

                    Loop = true;
                    Updating = true;
                    Invalidate();

                    SceneChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Scene;
        }

        public event EventHandler SceneChanged;

        private bool _FitScreen;

        [DefaultValue(false)]
        public bool FitScreen
        {
            set
            {
                if (value != _FitScreen)
                {
                    _FitScreen = value;

                    if (_FitScreen && _Scene != null && Graphics != null) _Scene.FitCamera();

                    FitScreenChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _FitScreen;
        }
        public event EventHandler FitScreenChanged;

        private bool _Loop;

        [DefaultValue(false)]
        public bool Loop 
        { 
            set
            {
                if (value != _Loop)
                {
                    _Loop = value;
                    LoopChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Loop;
        }
        public event EventHandler LoopChanged;

        private LightSpace _lightSpace;
        private int _mouseX;
        private int _mouseY;

        private enum AxisView
        {
            None, 
            X, 
            Y, 
            Z
        };
        private AxisView _axisView;

        public SceneScreenControl()
        {
            InitializeComponent();

            ContextMenu = new ContextMenu();        //disable other context menu

            Disposed += SceneScreenControl_Disposed;
        }

        public void Update(float delta)
        {
            _Scene?.Update(Width, Height, ref _lightSpace, delta, Loop);
        }

        private void SceneScreenControl_Disposed(object sender, EventArgs e)
        {
            if (_Scene != null) _Scene.Refresh -= Scene_Refresh;
        }

        private void Scene_Refresh(object sender, EventArgs e)
        {
            if (!Updating) Invalidate();
        }

        protected override void OnGraphicsCreated()
        {
            Update(0);
        }

        protected override void OnGraphicsDestroying()
        {
            if (_lightSpace != null)
            {
                _lightSpace.Dispose();
                _lightSpace = null;
            }
        }

        protected override void OnGraphicsResize()
        {
            Update(0);
        }

        protected override void OnGraphicsUpdate(float delta)
        {
            Update(delta);
        }

        protected override void OnGraphicsDraw()
        {
            _Scene?.Draw(Graphics, _lightSpace);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            _axisView = AxisView.None;

            _mouseX = e.X;
            _mouseY = e.Y;

            if (_Scene != null)
            {
                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                if (_Scene.MouseDown(e, controlKey, shiftKey)) Refresh();
                else if (shiftKey && _Scene.SelectedComponent is SceneObject obj && obj.AllowDrag) DoDragDrop(obj.Key, DragDropEffects.Copy);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _axisView = AxisView.None;

            if (_Scene != null)
            {
                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                if (_Scene.MouseMove(e, controlKey, shiftKey)) Refresh();
                else if (_Scene.CameraObject == null)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Right:
                            if (controlKey)
                            {
                                if (e.X != _mouseX)
                                {
                                    _Scene.Camera.Orbit(Vector3.UnitZ, (e.X - _mouseX) * MathUtil.Pi / Width);
                                }
                                if (e.Y != _mouseY)
                                {
                                    _Scene.Camera.Orbit(_Scene.Camera.Right, (e.Y - _mouseY) * MathUtil.Pi / Height);
                                }
                                Refresh();
                            }
                            else
                            {
                                _Scene.Camera.Move(_Scene.Camera.Right * (_mouseX - e.X) + _Scene.Camera.Up * (e.Y - _mouseY));
                                Refresh();
                            }
                            break;
                    }
                }
            }
            _mouseX = e.X;
            _mouseY = e.Y;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _axisView = AxisView.None;

            if (_Scene != null)
            {
                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                if (_Scene.MouseUp(e, controlKey, shiftKey)) Refresh();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _axisView = AxisView.None;

            if (_Scene != null)
            {
                var controlKey = (ModifierKeys & Keys.Control) != 0;
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;

                if (_Scene.MouseWheel(e, controlKey, shiftKey)) Refresh();
                else if (_Scene.CameraObject == null)
                {
                    var d = Vector3.Distance(_Scene.Camera.Position, _Scene.Camera.Target);

                    var delta = 0f;

                    if (e.Delta > 0)
                    {
                        if (d > _Scene.Camera.Near) delta = Math.Min(d - _Scene.Camera.Near, e.Delta * Scene.MouseWheelCameraDelta);
                    }
                    else if (e.Delta < 0)
                    {
                        if (d < _Scene.Camera.Far) delta = Math.Max(d - _Scene.Camera.Far, e.Delta * Scene.MouseWheelCameraDelta);
                    }
                    if (!MathUtil.NearZero(delta))
                    {
                        _Scene.Camera.Position += _Scene.Camera.Forward * delta;

                        Refresh();
                    }
                }
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _axisView = AxisView.None;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_Scene != null)
            {
                if (_Scene.KeyDown(e))
                {
                    Refresh();
                    _axisView = AxisView.None;
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Escape:
                            if (_Scene.CameraObject == null)
                            {
                                _Scene.ResetCamera();
                                if (_FitScreen) _Scene.FitCamera();
                                Refresh();
                            }
                            _axisView = AxisView.None;
                            break;
                        case Keys.Space:
                            _Scene.SelectedObjectOnly = !_Scene.SelectedObjectOnly;
                            Refresh();

                            _axisView = AxisView.None;
                            break;
                        case Keys.D1:
                            if (_Scene.CameraObject == null)
                            {
                                var d = Vector3.Distance(_Scene.Camera.Position, _Scene.Camera.Target);
                                var p = _Scene.Camera.Target;
                                if (_axisView == AxisView.X)
                                {
                                    p.X -= d;
                                    _axisView = AxisView.None;
                                }
                                else
                                {
                                    p.X += d;
                                    _axisView = AxisView.X;
                                }
                                _Scene.Camera.Position = p;
                                _Scene.Camera.Up = Vector3.UnitZ;
                                Refresh();
                            }
                            break;
                        case Keys.D2:
                            if (_Scene.CameraObject == null)
                            {
                                var d = Vector3.Distance(_Scene.Camera.Position, _Scene.Camera.Target);
                                var p = _Scene.Camera.Target;
                                if (_axisView == AxisView.Y)
                                {
                                    p.Y -= d;
                                    _axisView = AxisView.None;
                                }
                                else
                                {
                                    p.Y += d;
                                    _axisView = AxisView.Y;
                                }
                                _Scene.Camera.Position = p;
                                _Scene.Camera.Up = Vector3.UnitZ;
                                Refresh();
                            }
                            break;
                        case Keys.D3:
                            if (_Scene.CameraObject == null)
                            {
                                var d = Vector3.Distance(_Scene.Camera.Position, _Scene.Camera.Target);
                                var p = _Scene.Camera.Target;
                                if (_axisView == AxisView.Z)
                                {
                                    p.Z -= d;
                                    _axisView = AxisView.None;
                                }
                                else
                                {
                                    p.Z += d;
                                    _axisView = AxisView.Z;
                                }
                                _Scene.Camera.Position = p;
                                _Scene.Camera.Up = -Vector3.UnitY;
                                Refresh();
                            }
                            break;
                        default:
                            _axisView = AxisView.None;
                            break;
                    }
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_Scene != null && _Scene.KeyUp(e)) Refresh();
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (_Scene != null)
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                if (data != null)
                {
                    var controlKey = (ModifierKeys & Keys.Control) != 0;
                    var shiftKey = (ModifierKeys & Keys.Shift) != 0;
                    var p = PointToClient(new System.Drawing.Point(e.X, e.Y));
                    var ray = _Scene.Camera.PickRay(new Vector2(p.X, p.Y));

                    if (_Scene.DragOver(data, ray, controlKey, shiftKey))
                    {
                        e.Effect = DragDropEffects.Copy;
                        Refresh();
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        protected override void OnDragLeave(EventArgs e)
        {
            _Scene?.DragLeave();
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (_Scene != null)
            {
                var data = (string)e.Data.GetData(DataFormats.Text);

                if (data != null)
                {
                    var controlKey = (ModifierKeys & Keys.Control) != 0;
                    var shiftKey = (ModifierKeys & Keys.Shift) != 0;
                    var p = PointToClient(new System.Drawing.Point(e.X, e.Y));
                    var ray = _Scene.Camera.PickRay(new Vector2(p.X, p.Y));

                    _Scene.DragDrop(data, ray, controlKey, shiftKey);
                }
            }
        }
    }
}
