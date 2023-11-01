using System;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Scenes
{
    public enum GizmoMode
    {
        None,
        Translation,
        Rotation,
        Scaling
    }

    public enum GizmoAxis
    {
        None,
        X,
        Y,
        Z
    }

    public class Gizmo : AssetElement
    {
        public SceneObject Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _TargetKey;
        [Binding]
        public string TargetKey
        {
            set
            {
                if (SetProperty(ref _TargetKey, value))
                {
                    _updated = true;
                    OnPropertyChanged("Target");
                }
            }
            get => _TargetKey;
        }

        public SceneObject Target
        {
            set => TargetKey = value?.Key;
            get => _TargetKey != null ? Parent.Scene?.GetObject(_TargetKey) : null;
        }

        private string _Binding;
        [Binding]
        public string Binding
        {
            set => _updated |= SetProperty(ref _Binding, value);
            get => _Binding;
        }

        private Vector3 _Position;
        [Binding]
        public Vector3 Position
        {
            set
            {
                if (SetProperty(ref _Position, value))
                {
                    _updated = true;
                    OnPropertyChanged("GridPosition");
                }
            }
            get => _Position;
        }

        public Vector3 GridPosition
        {
            set => Position = value * _grid;
            get => _Position / _grid;
        }

        private bool _FromGround;
        [Binding]
        public bool FromGround
        {
            set => _updated |= SetProperty(ref _FromGround, value);
            get => _FromGround;
        }

        private Quaternion _Rotation;
        [Binding]
        public Quaternion Rotation
        {
            set => _updated |= SetProperty(ref _Rotation, value);
            get => _Rotation;
        }

        private Vector3 _Scale;
        [Binding]
        public Vector3 Scale
        {
            set
            {
                value = Vector3.Clamp(value, new Vector3(MinScale), new Vector3(MaxScale));

                _updated |= SetProperty(ref _Scale, value);
            }
            get => _Scale;
        }

        public GizmoMode Mode { private set; get; }
        public GizmoAxis Axis { private set; get; }

        private float _grid;
        private bool _updated;
        private bool _commanding;
        private float _rotationDegree;
        private float _z;

        private const float MinScale = 0.01f;
        private const float MaxScale = 100;
        private const float Range = 100;
        private const float MouseToRogtation = 2 * MathUtil.ToRadians;

        private int Grid => Parent.Scene?.World?.Grid ?? Parent.Scene?.Config?.Grounds[0].Grid ?? AssetManager.Instance.Config.Scene.Grounds[0].Grid;

        public Gizmo(SceneObject parent)
        {
            Parent = parent;

            _Rotation = Quaternion.Identity;
            _Scale = Vector3.One;
            _grid = Grid;
        }

        public Gizmo(SceneObject parent, Gizmo other, bool binding) : base(other, binding)
        {
            Parent = parent;

            _TargetKey = other._TargetKey;
            _Binding = other._Binding;
            _Position = other._Position;
            _FromGround = other._FromGround;
            _Rotation = other._Rotation;
            _Scale = other._Scale;
            _grid = Grid;
            _updated = true;
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_TargetKey != null && element is SceneObject obj && obj.Key == _TargetKey)
            {
                from = this;
                return true;
            }
            from = null;
            return false;
        }

        public bool GetTransform(float progress, out Matrix4x4 result)
        {
            var target = Target;

            var fromGround = false;
            if (target != null)
            {
                if (!target.GetTransform(progress + target.Progress - Parent.Progress, _Binding, out result)) return false;
            }
            else
            {
                fromGround = _FromGround && Parent.Scene != null;
                if (Parent.Parent is SceneObject parent)
                {
                    if (!parent.GetTransform(progress + parent.Progress - Parent.Progress, _Binding, out result)) return false;
                    fromGround &= !parent.FromGround;
                }
                else result = Matrix4x4.Identity;
            }

            if (_Position != Vector3.Zero) result = Matrix4x4.CreateTranslation(_Position) * result;
            if (_Rotation != Quaternion.Identity) result = Matrix4x4.CreateFromQuaternion(_Rotation) * result;
            if (_Scale != Vector3.One) result = Matrix4x4.CreateScale(_Scale) * result;
            if (fromGround) result.M43 += Parent.Scene.GetZ(Parent, result.Translation);
            return true;
        }

        public bool GetTransform(out Matrix4x4 result) => GetTransform(Parent.Progress, out result);

        internal bool Update()
        {
            var grid = Grid;

            if (grid != _grid)
            {
                _grid = grid;
                AssetManager.Instance.Invoke(() => OnPropertyChanged("GridPosition"));
            }

            var target = Target;
            var parent = Parent.Parent is SceneObject p ? p : null;

            var z = 0f;
            if (_FromGround && target == null && Parent.Scene != null)
            {
                if (parent == null) z = Parent.Scene.GetZ(Parent, _Position);
                else if (!parent.FromGround && parent.GetTransform(_Binding, out var pt))
                {
                    if (_Position != Vector3.Zero) pt = Matrix4x4.CreateTranslation(_Position) * pt;
                    z = Parent.Scene.GetZ(Parent, pt.Translation);
                }
            }
            _updated |= z != _z;
            _z = z;

            if (_updated)
            {
                _updated = false;
                return true;
            }

            if (target != null) return target.TransformUpdated;
            if (parent != null) return parent.TransformUpdated;
            return false;
        }

        internal bool GetUpdatePass(ref UpdatePass pass)
        {
            if (!pass.Remaining) return false;
            var target = Target;
            if (target != null) return pass.AddPrecedence(target);
            return true;
        }

        private bool GetPrevTransform(out Matrix4x4 result)
        {
            var prev = Target;
            
            if (prev == null && Parent.Parent is SceneObject p) prev = p;

            if (prev != null) return prev.GetTransform(prev.Progress, _Binding, out result);

            result = Matrix4x4.Identity;
            return true;
        }

        internal bool Draw(Graphics graphics)
        {
            if (Mode == GizmoMode.None) return false;

            if (!GetPrevTransform(out var transform)) return false;

            graphics.Push();

            graphics.Material.Shader = MaterialShader.NoLight;
            graphics.Material.BlendMode = BlendMode.Alpha;
            graphics.Material.DepthTest = false;
            
            graphics.Transform(transform);
            graphics.Translate(_Position);

            switch (Mode)
            {
                case GizmoMode.Translation:
                    {
                        var cursor = VertexArrays.GetCylinder(0, Vector3.Zero, 0, 8, 16, Rectangle.ZeroToOne, out var aabb);

                        graphics.SetColor(new Color4(Color3.Red, Axis == GizmoAxis.X ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(-Range, 0, 0), new Vector3(Range, 0, 0));
                        graphics.PushTransform();
                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(cursor, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();

                        graphics.SetColor(new Color4(Color3.Green, Axis == GizmoAxis.Y ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(0, -Range, 0), new Vector3(0, Range, 0));
                        graphics.PushTransform();
                        graphics.RotateX(-MathUtil.PiOverTwo);
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(cursor, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();

                        graphics.SetColor(new Color4(Color3.Blue, Axis == GizmoAxis.Z ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(0, 0, -Range), new Vector3(0, 0, Range));
                        graphics.PushTransform();
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(cursor, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();
                    }
                    break;
                case GizmoMode.Rotation:
                    {
                        graphics.Transform(Matrix4x4.CreateFromQuaternion(_Rotation));

                        var rect = new Rectangle(-Range, -Range, Range * 2, Range * 2);

                        graphics.PushTransform();

                        graphics.SetColor(new Color4(Color3.Blue, Axis == GizmoAxis.Z ? 1f : 0.5f), false);
                        graphics.DrawArc(rect, MathUtil.PiOverFour, 3 * MathUtil.PiOverFour, false);

                        graphics.RotateX(MathUtil.PiOverTwo);
                        graphics.SetColor(new Color4(Color3.Green, Axis == GizmoAxis.Y ? 1f : 0.5f), false);
                        graphics.DrawArc(rect, -MathUtil.PiOverFour, MathUtil.PiOverFour, false);

                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.SetColor(new Color4(Color3.Red, Axis == GizmoAxis.X ? 1f : 0.5f), false);
                        graphics.DrawArc(rect, -MathUtil.PiOverFour, MathUtil.PiOverFour, false);

                        graphics.PopTransform();

                        switch (Axis)
                        {
                            case GizmoAxis.X:
                                graphics.RotateX(MathUtil.PiOverTwo);
                                graphics.RotateY(MathUtil.PiOverTwo);
                                graphics.SetColor(new Color4(Color3.Red, 0.25f), false);
                                graphics.DrawArc(rect, Math.Min(_rotationDegree, 0), Math.Max(_rotationDegree, 0), true);
                                break;
                            case GizmoAxis.Y:
                                graphics.SetColor(new Color4(Color3.Green, 0.25f), false);
                                graphics.RotateX(MathUtil.PiOverTwo);
                                graphics.DrawArc(rect, Math.Min(_rotationDegree, 0), Math.Max(_rotationDegree, 0), true);
                                break;
                            case GizmoAxis.Z:
                                graphics.SetColor(new Color4(Color3.Blue, 0.25f), false);
                                graphics.RotateZ(MathUtil.PiOverTwo);
                                graphics.DrawArc(rect, Math.Min(_rotationDegree, 0), Math.Max(_rotationDegree, 0), true);
                                break;
                        }
                    }
                    break;
                case GizmoMode.Scaling:
                    {
                        var box = VertexArrays.GetCube(0, Vector3.Zero, 4, Rectangle.ZeroToOne, out var aabb);

                        graphics.Transform(Matrix4x4.CreateFromQuaternion(_Rotation));

                        graphics.SetColor(new Color4(Color3.Red, Axis == GizmoAxis.X ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(-Range, 0, 0), new Vector3(Range, 0, 0));
                        graphics.PushTransform();
                        graphics.RotateY(MathUtil.PiOverTwo);
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(box, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();

                        graphics.SetColor(new Color4(Color3.Green, Axis == GizmoAxis.Y ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(0, -Range, 0), new Vector3(0, Range, 0));
                        graphics.PushTransform();
                        graphics.RotateX(-MathUtil.PiOverTwo);
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(box, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();

                        graphics.SetColor(new Color4(Color3.Blue, Axis == GizmoAxis.Z ? 1f : 0.5f), false);
                        graphics.DrawLine(new Vector3(0, 0, -Range), new Vector3(0, 0, Range));
                        graphics.PushTransform();
                        graphics.Translate(new Vector3(0, 0, Range));
                        graphics.DrawVertices(box, PrimitiveMode.Triangles, aabb);
                        graphics.PopTransform();
                    }
                    break;
            }
            graphics.Pop();

            return true;
        }

        internal void Select(bool focus)
        {
            if (!focus)
            {
                if (_commanding)
                {
                    AssetManager.Instance.EndCommands();
                    _commanding = false;
                }

                Mode = GizmoMode.None;
                Axis = GizmoAxis.None;
            }
        }

        internal bool KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    Axis = GizmoAxis.None;
                    Mode = Mode != GizmoMode.Translation ? GizmoMode.Translation : GizmoMode.None;
                    return true;
                case Keys.W:
                    Axis = GizmoAxis.None;
                    Mode = Mode != GizmoMode.Rotation ? GizmoMode.Rotation : GizmoMode.None;
                    return true;
                case Keys.E:
                    Axis = GizmoAxis.None;
                    Mode = Mode != GizmoMode.Scaling ? GizmoMode.Scaling : GizmoMode.None;
                    return true;
                case Keys.Escape:
                    Axis = GizmoAxis.None;
                    Mode = GizmoMode.None;
                    return true;
            }
            return false;
        }

        internal bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey)
            {
                if (!GetPrevTransform(out var transform)) return false;

                transform.Translation += _Position;

                var ray = Parent.Scene.Camera.PickRay(new Vector2(e.X, e.Y));

                float x, y, z;

                switch (Mode)
                {
                    case GizmoMode.Rotation:
                        {
                            var px = new Plane(Vector3.UnitX, 0);
                            var py = new Plane(Vector3.UnitY, 0);
                            var pz = new Plane(Vector3.UnitZ, 0);

                            transform = Matrix4x4.CreateFromQuaternion(_Rotation) * transform;

                            px = Plane.Transform(px, transform);
                            py = Plane.Transform(py, transform);
                            pz = Plane.Transform(pz, transform);

                            x = float.MaxValue;
                            y = float.MaxValue;
                            z = float.MaxValue;

                            var p = transform.Translation;

                            if (ray.Intersects(px, out var xd)) x = Math.Abs(Vector3.Distance(p, ray.Position + ray.Direction * xd) - Range);
                            if (ray.Intersects(py, out var yd)) y = Math.Abs(Vector3.Distance(p, ray.Position + ray.Direction * yd) - Range);
                            if (ray.Intersects(pz, out var zd)) z = Math.Abs(Vector3.Distance(p, ray.Position + ray.Direction * zd) - Range);

                            _rotationDegree = 0;
                        }
                        break;
                    case GizmoMode.Scaling:
                        transform = Matrix4x4.CreateFromQuaternion(_Rotation) * transform;
                        goto case GizmoMode.Translation;
                    case GizmoMode.Translation:
                        {
                            ray.Intersects(new Segment(
                                Vector3.Transform(new Vector3(-Range, 0, 0), transform),
                                Vector3.Transform(new Vector3(Range, 0, 0), transform)), out var dx, out var nearx);
                            ray.Intersects(new Segment(
                                Vector3.Transform(new Vector3(0, -Range, 0), transform),
                                Vector3.Transform(new Vector3(0, Range, 0), transform)), out var dy, out var neary);
                            ray.Intersects(new Segment(
                                Vector3.Transform(new Vector3(0, 0, -Range), transform),
                                Vector3.Transform(new Vector3(0, 0, Range), transform)), out var dz, out var nearz);

                            x = Vector3.Distance(ray.Position + ray.Direction * dx, nearx);
                            y = Vector3.Distance(ray.Position + ray.Direction * dy, neary);
                            z = Vector3.Distance(ray.Position + ray.Direction * dz, nearz);
                        }
                        break;
                    default:
                        return false;
                }
                if (x <= y)
                {
                    if (x <= z) Axis = GizmoAxis.X;
                    else Axis = GizmoAxis.Z;
                }
                else
                {
                    if (y <= z) Axis = GizmoAxis.Y;
                    else Axis = GizmoAxis.Z;
                }

                AssetManager.Instance.BeginCommands();
                _commanding = true;

                return true;
            }

            return false;
        }

        internal bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey)
            {
                if (_commanding)
                {
                    AssetManager.Instance.EndCommands();
                    _commanding = false;
                }

                Axis = GizmoAxis.None;
                return true;
            }
            return false;
        }

        internal bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left)
            {
                var scene = Parent.Scene;

                if (Mode == GizmoMode.None)
                {
                    if (_TargetKey == null)
                    {
                        var ray1 = scene.Camera.PickRay(new Vector2(e.X, e.Y));

                        var pick = controlKey;

                        if (scene.Intersects(ray1, Parent, pick, CollisionFlags.None, out var obj, out var d1, out _))
                        {
                            if (pick && obj != null)
                            {
                                Target = obj;
                            }
                            else
                            {
                                var ray0 = scene.Camera.PickRay(new Vector2(prevX, prevY));

                                if (scene.Intersects(ray0, Parent, false, CollisionFlags.None, out _, out var d0, out _))
                                {
                                    var p0 = ray0.Position + ray0.Direction * d0;
                                    var p1 = ray1.Position + ray1.Direction * d1;

                                    var dp = p1 - p0;
                                    if (_FromGround) dp.Z = 0;
                                    var p = _Position + dp;

                                    if (scene.World == null || scene.World.Space.Intersects(p, CollisionFlags.None, out _) != CollisionResult.Front)
                                    {
                                        Target = null;
                                        Position = p;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!GetPrevTransform(out var transform)) return false;

                    var mouseDiff = new Vector2(e.X - prevX, e.Y - prevY);

                    switch (Mode)
                    {
                        case GizmoMode.Translation:
                            {
                                var diff = mouseDiff.X * scene.Camera.Right - mouseDiff.Y * scene.Camera.Up;

                                if (Matrix4x4.Decompose(transform, out _, out var rotation, out _))
                                {
                                    diff = Vector3.Transform(diff, rotation);
                                }

                                var pos = _Position;
                                switch (Axis)
                                {
                                    case GizmoAxis.X:
                                        pos.X += diff.X;
                                        break;
                                    case GizmoAxis.Y:
                                        pos.Y += diff.Y;
                                        break;
                                    case GizmoAxis.Z:
                                        pos.Z += diff.Z;
                                        break;
                                }
                                Position = pos;
                            }
                            break;
                        case GizmoMode.Rotation:
                            switch (Axis)
                            {
                                case GizmoAxis.X:
                                    {
                                        var d = (Math.Abs(mouseDiff.X) >= Math.Abs(mouseDiff.Y) ? -mouseDiff.X : -mouseDiff.Y) * MouseToRogtation;
                                        Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, d);
                                        _rotationDegree -= d;
                                    }
                                    break;
                                case GizmoAxis.Y:
                                    {
                                        var d = (Math.Abs(mouseDiff.X) >= Math.Abs(mouseDiff.Y) ? mouseDiff.X : mouseDiff.Y) * MouseToRogtation;
                                        Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, d);
                                        _rotationDegree += d;
                                    }
                                    break;
                                case GizmoAxis.Z:
                                    {
                                        var d = (Math.Abs(mouseDiff.X) >= Math.Abs(mouseDiff.Y) ? -mouseDiff.X : mouseDiff.Y) * MouseToRogtation;
                                        Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, d);
                                        _rotationDegree -= d;
                                    }
                                    break;
                            }
                            break;
                        case GizmoMode.Scaling:
                            {
                                Matrix4x4.Decompose(transform, out _, out var rotation, out _);

                                rotation = _Rotation * rotation;

                                var diff = mouseDiff.X * scene.Camera.Right - mouseDiff.Y * scene.Camera.Up;
                                diff = Vector3.Transform(diff, rotation);

                                var r = 1f / Range;
                                var scale = Vector3.Zero;
                                switch (Axis)
                                {
                                    case GizmoAxis.X:
                                        scale.X += diff.X * r;
                                        break;
                                    case GizmoAxis.Y:
                                        scale.Y += diff.Y * r;
                                        break;
                                    case GizmoAxis.Z:
                                        scale.Z += diff.Z * r;
                                        break;
                                }
                                if (shiftKey)
                                {
                                    var min = Math.Min(Math.Min(scale.X, scale.Y), scale.Z);
                                    var max = Math.Max(Math.Max(scale.X, scale.Y), scale.Z);
                                    if (max > 0) Scale += new Vector3(max);
                                    else if (min < 0) Scale += new Vector3(min);
                                }
                                else Scale += scale;
                            }
                            break;
                    }
                }

                return true;
            }

            return false;
        }

        internal void Save(XmlWriter writer, string name)
        {
            var strbuf = new StringBuilder();
            if (_TargetKey != null) strbuf.Append(_TargetKey);
            strbuf.Append(',');
            if (_Binding != null) strbuf.Append(_Binding);
            strbuf.Append(',');
            strbuf.Append(_Position.X);
            strbuf.Append(',');
            strbuf.Append(_Position.Y);
            strbuf.Append(',');
            strbuf.Append(_Position.Z);
            strbuf.Append(',');
            strbuf.Append(_Rotation.X);
            strbuf.Append(',');
            strbuf.Append(_Rotation.Y);
            strbuf.Append(',');
            strbuf.Append(_Rotation.Z);
            strbuf.Append(',');
            strbuf.Append(_Rotation.W);
            strbuf.Append(',');
            strbuf.Append(_Scale.X);
            strbuf.Append(',');
            strbuf.Append(_Scale.Y);
            strbuf.Append(',');
            strbuf.Append(_Scale.Z);
            strbuf.Append(',');
            strbuf.Append(_FromGround);

            writer.WriteAttribute(name, strbuf.ToString());
        }


        internal void Load(XmlNode node, string name)
        {
            var value = node.ReadAttributeString(name);

            if (value == null)
            {
                TargetKey = null;
                Binding = null;
                Position = Vector3.Zero;
                Rotation = Quaternion.Identity;
                Scale = Vector3.One;
                FromGround = false;
            }
            else
            {
                var ps = value.Split(',');
                if (ps[0] != string.Empty)
                {
                    TargetKey = ps[0];
                    Binding = ps[1] != string.Empty ? ps[1] : null;
                }
                else
                {
                    TargetKey = null;
                    Binding = null;
                }

                Position = new Vector3(
                    float.Parse(ps[2]),
                    float.Parse(ps[3]),
                    float.Parse(ps[4]));

                Rotation = new Quaternion(
                    float.Parse(ps[5]),
                    float.Parse(ps[6]),
                    float.Parse(ps[7]),
                    float.Parse(ps[8]));

                Scale = new Vector3(
                    float.Parse(ps[9]),
                    float.Parse(ps[10]),
                    float.Parse(ps[11]));

                FromGround = bool.Parse(ps[12]);
            }
        }

        internal void Build(BinaryWriter writer, SceneBuildParam param)
        {
            writer.Write(param.KeyToId(_TargetKey));
            writer.WriteString(_Binding);
            writer.Write(_Position);
            writer.Write(_Rotation);
            writer.Write(_Scale);
            writer.Write(_FromGround);
        }
    }
}
