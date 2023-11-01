using System.Collections.Generic;
using System.Numerics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations;

namespace CDK.Assets.Scenes
{
    public class CameraObject : SceneObject, IAnimationSubstance
    {
        private bool _UsingTarget;
        [Binding]
        public bool UsingTarget
        {
            set
            {
                if (SetProperty(ref _UsingTarget, value)) TargetControl = false;
            }
            get => _UsingTarget;
        }
        public Gizmo Target { private set; get; }

        private AnimationTarget _AnimationTarget;
        [Binding]
        public AnimationTarget AnimationTarget
        {
            set => SetProperty(ref _AnimationTarget, value);
            get => _AnimationTarget;
        }

        private bool _TargetControl;
        public bool TargetControl
        {
            set => SetProperty(ref _TargetControl, value, false);
            get => _TargetControl;
        }

        private bool _UsingFrustum;
        [Binding]
        public bool UsingFrustum
        {
            set => SetProperty(ref _UsingFrustum, value);
            get => _UsingFrustum;
        }

        private float _Fov;
        [Binding]
        public float Fov
        {
            set => SetProperty(ref _Fov, value);
            get => _Fov;
        }

        private float _Near;
        [Binding]
        public float Near
        {
            set
            {
                if (value > _Far - 1) value = _Far - 1;

                SetProperty(ref _Near, value);
            }
            get => _Near;
        }

        private float _Far;
        [Binding]
        public float Far
        {
            set
            {
                if (value < _Near + 1) value = _Near + 1;

                SetProperty(ref _Far, value);
            }
            get => _Far;
        }

        private bool _UsingBlur;
        [Binding]
        public bool UsingBlur
        {
            set => SetProperty(ref _UsingBlur, value);
            get => _UsingBlur;
        }

        private float _BlurDistance;
        [Binding]
        public float BlurDistance
        {
            set => SetProperty(ref _BlurDistance, value);
            get => _BlurDistance;
        }

        private float _BlurRange;
        [Binding]
        public float BlurRange
        {
            set => SetProperty(ref _BlurRange, value);
            get => _BlurRange;
        }

        private float _BlurIntensity;
        [Binding]
        public float BlurIntensity
        {
            set => SetProperty(ref _BlurIntensity, value);
            get => _BlurIntensity;
        }

        public bool Focused
        {
            set
            {
                if (Scene != null && Focused != value)
                {
                    Scene.CameraObject = value ? (Parent is AnimationObjectFragment animation ? (SceneObject)animation.Root : this) : null;
                }
            }
            get
            {
                var obj = Scene?.CameraObject;
                if (obj == null) return false;
                return obj.Contains(this);
            }
        }

        public CameraObject()
        {
            Target = new Gizmo(this);

            var config = AssetManager.Instance.Config.Scene.Environments[0];

            _Fov = config.CameraFov;
            _Near = config.CameraNear;
            _Far = config.CameraFar;

            _BlurDistance = 1000;
            _BlurRange = 1000;
            _BlurIntensity = 4;
        }

        public CameraObject(CameraObject other, bool binding) : base(other, binding, true)
        {
            _UsingTarget = other._UsingTarget;
            Target = new Gizmo(this, other.Target, binding);
            _AnimationTarget = other._AnimationTarget;
            _UsingFrustum = other._UsingFrustum;
            _Fov = other._Fov;
            _Near = other._Near;
            _Far = other._Far;
            _UsingBlur = other._UsingBlur;
            _BlurDistance = other._BlurDistance;
            _BlurRange = other._BlurRange;
            _BlurIntensity = other._BlurIntensity;
        }

        public override SceneComponentType Type => SceneComponentType.Camera;
        public override SceneComponent Clone(bool binding) => new CameraObject(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AllowDrag => true;

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_UsingTarget && Target.IsRetaining(element, out from)) return true;

            return base.IsRetaining(element, out from);
        }

        protected override void OnLink()
        {
            base.OnLink();

            Scene.AddWeakPropertyChanged(Scene_PropertyChanged);
            OnPropertyChanged("Focused");
        }

        protected override void OnUnlink()
        {
            Scene.RemoveWeakPropertyChanged(Scene_PropertyChanged);
            OnPropertyChanged("Focused");

            base.OnUnlink();
        }

        private void Scene_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CameraObject") OnPropertyChanged("Focused");
        }

        private bool GetTargetTransform(out Matrix4x4 result)
        {
            if (Parent is AnimationObjectFragment animation)
            {
                if (_AnimationTarget != AnimationTarget.Origin) return animation.Root.GetTargetTransform(_AnimationTarget, out result);
            }
            else
            {
                if (_UsingTarget) return Target.GetTransform(out result);
            }
            result = Matrix4x4.Identity;
            return false;
        }

        internal override bool CameraCapture(ref Camera camera)
        {
            if (_UsingFrustum)
            {
                camera.Fov = _Fov * MathUtil.ToRadians;
                camera.Near = _Near;
                camera.Far = _Far;
            }
            if (!GetTransform(out var transform)) return false;

            camera.Position = transform.Translation;

            if (GetTargetTransform(out var targetTransform))
            {
                camera.Target = targetTransform.Translation;

                var forward = Vector3.Normalize(camera.Target - camera.Position);
                if (VectorUtil.NearEqual(forward, Vector3.UnitZ)) camera.Up = Vector3.UnitY;
                else if (VectorUtil.NearEqual(forward, -Vector3.UnitZ)) camera.Up = -Vector3.UnitY;
                else
                {
                    var right = Vector3.Normalize(Vector3.Cross(forward, -Vector3.UnitZ));
                    var up = Vector3.Normalize(Vector3.Cross(forward, right));
                    camera.Up = up;
                }
            }
            else
            {
                camera.Target = transform.Translation + transform.Forward() * camera.GetDefaultDistance();
                camera.Up = -transform.Up();
            }
            return true;
        }

        internal override bool CameraFilter(Graphics graphics)
        {
            if (_UsingBlur)
            {
                var distance = _BlurDistance > 0 ? _BlurDistance : Vector3.Distance(graphics.Camera.Position, graphics.Camera.Target);
                graphics.BlurDepth(distance, _BlurRange, _BlurIntensity);
            }
            return true;
        }

        internal override void Select(bool focus)
        {
            TargetControl = false;
            if (UsingTransform) Transform.Select(focus);
            if (_UsingTarget) Target.Select(focus);
        }

        internal override void Offset(in Vector3 offset, bool retrieving)
        {
            if (_UsingTarget && Target.TargetKey == null) Target.Position += offset;

            base.Offset(offset, retrieving);
        }

        //TODO:카메라커서를 화면에 항상 표시하려면 AABB 업데이트 필요함 (CursorAlwaysVisible)

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor || Focused) return;

            var camera = graphics.Camera;

            if (!CameraCapture(ref camera)) return;

            Transform?.Draw(graphics);
            
            var command = new StreamRenderCommand(graphics, PrimitiveMode.Lines);

            command.State.Material.Shader = MaterialShader.NoLight;
            command.State.Material.BlendMode = BlendMode.Alpha;

            foreach (var p in camera.BoundingFrustum().GetCorners()) command.AddVertex(new FVertex(p, Color4.TranslucentWhite));

            command.AddVertex(new FVertex(camera.Position));
            command.AddVertex(new FVertex(camera.Target));

            command.AddIndex(0);
            command.AddIndex(1);
            command.AddIndex(1);
            command.AddIndex(2);
            command.AddIndex(2);
            command.AddIndex(3);
            command.AddIndex(3);
            command.AddIndex(0);

            command.AddIndex(4);
            command.AddIndex(5);
            command.AddIndex(5);
            command.AddIndex(6);
            command.AddIndex(6);
            command.AddIndex(7);
            command.AddIndex(7);
            command.AddIndex(4);

            command.AddIndex(0);
            command.AddIndex(4);
            command.AddIndex(1);
            command.AddIndex(5);
            command.AddIndex(2);
            command.AddIndex(6);
            command.AddIndex(3);
            command.AddIndex(7);

            command.AddIndex(8);
            command.AddIndex(9);

            graphics.Command(command);
        }

        private void Orbit(in Vector3 axis, float angle)
        {
            if (UsingTransform && Transform.Target == null)
            {
                if (_UsingTarget)
                {
                    if (Target.GetTransform(out var targetTransform))
                    {
                        var target = targetTransform.Translation;

                        var p = Transform.Position - target;
                        var rotation = Quaternion.CreateFromAxisAngle(axis, angle);
                        p = Vector3.Transform(p, rotation);
                        Transform.Position = p + target;
                    }
                }
                else
                {
                    var rotation = Quaternion.CreateFromAxisAngle(axis, angle);

                    Transform.Rotation = rotation * Transform.Rotation;
                }
            }
        }

        private void Move(in Vector3 diff)
        {
            if (UsingTransform && Transform.Target == null) Transform.Position += diff;
            if (_UsingTarget && Target.Target == null) Target.Position += diff;
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    TargetControl = !_TargetControl;
                    return true;
            }
            if (!Focused)
            {
                if (_UsingTarget && _TargetControl) return Target.KeyDown(e);
                else if (UsingTransform) return Transform.KeyDown(e);
            }
            return false;
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (!Focused)
            {
                if (_UsingTarget && _TargetControl) return Target.MouseDown(e, controlKey, shiftKey);
                else if (UsingTransform) return Transform.MouseDown(e, controlKey, shiftKey);
            }
            return false;
        }

        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (!Focused)
            {
                if (_UsingTarget && _TargetControl) return Target.MouseUp(e, controlKey, shiftKey);
                else if (UsingTransform) return Transform.MouseUp(e, controlKey, shiftKey);
            }
            return false;
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (Focused)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        if (controlKey)
                        {
                            if (e.X != prevX) Orbit(Vector3.UnitZ, (e.X - prevX) * MathUtil.Pi / Scene.Camera.Width);
                            if (e.Y != prevY) Orbit(Scene.Camera.Right, (e.Y - prevY) * MathUtil.Pi / Scene.Camera.Height);
                        }
                        else Move(Scene.Camera.Right * (prevX - e.X) + Scene.Camera.Up * (e.Y - prevY));
                        return true;
                }
            }
            else
            {
                if (_UsingTarget && _TargetControl) return Target.MouseMove(e, prevX, prevY, controlKey, shiftKey);
                else if (UsingTransform) return Transform.MouseMove(e, prevX, prevY, controlKey, shiftKey);
            }
            return false;
        }

        internal override bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (Focused && UsingTransform && Transform.Target == null)
            {
                Vector3 forward;
                if (_UsingTarget)
                {
                    if (!Target.GetTransform(out var targetTransform)) return false;
                    forward = Vector3.Normalize(targetTransform.Translation - Transform.Position);
                }
                else forward = Matrix4x4.CreateFromQuaternion(Transform.Rotation).Forward();

                Transform.Position += forward * e.Delta * Scene.MouseWheelCameraDelta;

                return true;
            }

            return false;
        }
        
        protected override void SaveContent(XmlWriter writer)
        {
            if (_UsingTarget) Target.Save(writer, "target");
            writer.WriteAttribute("animationTarget", _AnimationTarget, AnimationTarget.Origin);
            if (_UsingFrustum) writer.WriteAttribute("frustum", $"{_Fov},{_Near},{_Far}");
            if (_UsingBlur) writer.WriteAttribute("blur", $"{_BlurDistance},{_BlurRange},{_BlurIntensity}");
        }

        protected override void LoadContent(XmlNode node)
        {
            UsingTarget = node.HasAttribute("target");
            if (_UsingTarget) Target.Load(node, "target");
            AnimationTarget = node.ReadAttributeEnum("animationTarget", AnimationTarget.Origin);
            UsingFrustum = node.HasAttribute("frustum");
            if (_UsingFrustum)
            {
                var frustum = node.ReadAttributeFloats("frustum");
                Fov = frustum[0];
                Near = frustum[1];
                Far = frustum[2];
            }
            UsingBlur = node.HasAttribute("blur");
            if (_UsingBlur)
            {
                var blur = node.ReadAttributeFloats("blur");
                BlurDistance = blur[0];
                BlurRange = blur[1];
                BlurIntensity = blur[2];
            }
        }

        private void Build(BinaryWriter writer)
        {
            writer.Write((byte)_AnimationTarget);
            writer.Write(_UsingFrustum);
            if (_UsingFrustum)
            {
                writer.Write(_Fov * MathUtil.ToRadians);
                writer.Write(_Near);
                writer.Write(_Far);
            }
            writer.Write(_UsingBlur);
            if (_UsingBlur)
            {
                writer.Write(_BlurDistance);
                writer.Write(_BlurRange);
                writer.Write(_BlurIntensity);
            }
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            writer.Write(_UsingTarget);
            if (_UsingTarget) Target.Build(writer, param);
            Build(writer);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new CameraObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new CameraObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => Build(writer);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
