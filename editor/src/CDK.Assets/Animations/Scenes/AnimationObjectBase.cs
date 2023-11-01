using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public enum AnimationTarget
    {
        Origin,
        Target1,
        Target2,
        Target3
    }

    public abstract class AnimationObjectBase : SceneObject, ISceneAnimation
    {
        private AnimationFragment _Animation;
        [Binding]
        public AnimationFragment Animation
        {
            internal set
            {
                if (_Animation != value)
                {
                    _Animation?.RemoveWeakRefresh(Animation_Refresh);
                    _Animation = value;
                    _Animation?.AddWeakRefresh(Animation_Refresh);

                    AddUpdateFlags(UpdateFlags.Transform | UpdateFlags.AABB);

                    Object = _Animation != null ? new AnimationObjectFragment(_Animation) : null;

                    OnPropertyChanged("Animation");
                }
            }
            get => _Animation;
        }

        public Gizmo Target1 { private set; get; }

        private bool _UsingTarget1;
        [Binding]
        public bool UsingTarget1
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled && Parent is AnimationObjectFragment) value = false;

                if (SetProperty(ref _UsingTarget1, value)) AddUpdateFlags(UpdateFlags.Transform);
            }
            get => _UsingTarget1;
        }

        public Gizmo Target2 { private set; get; }

        private bool _UsingTarget2;
        [Binding]
        public bool UsingTarget2
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled && Parent is AnimationObjectFragment) value = false;

                if (SetProperty(ref _UsingTarget2, value)) AddUpdateFlags(UpdateFlags.Transform);
            }
            get => _UsingTarget2;
        }

        public Gizmo Target3 { private set; get; }

        private bool _UsingTarget3;
        [Binding]
        public bool UsingTarget3
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled && Parent is AnimationObjectFragment) value = false;

                if (SetProperty(ref _UsingTarget3, value)) AddUpdateFlags(UpdateFlags.Transform);
            }
            get => _UsingTarget3;
        }

        private AnimationTarget _SelectedTarget;
        public AnimationTarget SelectedTarget
        {
            set => SetProperty(ref _SelectedTarget, value, false);
            get => _SelectedTarget;
        }

        private string[] _Keys;
        [Binding]
        public string[] Keys
        {
            set => SetProperty(ref _Keys, value);
            get => _Keys;
        }

        public uint KeyMask
        {
            get
            {
                var keyMask = 0u;
                if (_Keys != null)
                {
                    var keyConstants = Project.GetAnimationKeyConstants();
                    foreach (var key in _Keys)
                    {
                        var constant = keyConstants.FirstOrDefault(c => c.Name == key);
                        if (constant == null || !uint.TryParse(constant.Value, out var value)) throw new AssetException(Owner, $"애니메이션 키({key})가 없거나 적절한 값이 아닙니다.");
                        keyMask |= value;
                    }
                }
                return keyMask;
            }
        }

        public AnimationObjectFragment Object { private set; get; }

        protected AnimationObjectBase()
        {
            Target1 = new Gizmo(this);
            Target2 = new Gizmo(this);
            Target3 = new Gizmo(this);

            PropertyChanged += AnimationObjectBase_PropertyChanged;
        }

        protected AnimationObjectBase(AnimationObjectBase other, bool binding, bool children) : base(other, binding, children)
        {
            _UsingTarget1 = other._UsingTarget1;
            Target1 = new Gizmo(this, other.Target1, binding);
            _UsingTarget2 = other._UsingTarget2;
            Target2 = new Gizmo(this, other.Target2, binding);
            _UsingTarget3 = other._UsingTarget3;
            Target3 = new Gizmo(this, other.Target3, binding);
            _Keys = other._Keys;

            PropertyChanged += AnimationObjectBase_PropertyChanged;
        }

        private void AnimationObjectBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent")
            {
                UsingTarget1 = _UsingTarget1;
                UsingTarget2 = _UsingTarget2;
                UsingTarget3 = _UsingTarget3;
                //reset usingTarget
            }
        }

        private void Animation_Refresh(object sender, EventArgs e)
        {
            AddUpdateFlags(UpdateFlags.Transform | UpdateFlags.AABB);

            OnRefresh();
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Animation != null && _Animation.Parent == this && _Animation.IsRetaining(element, out from)) return true;

            if (_UsingTarget1 && Target1.IsRetaining(element, out from)) return true;
            if (_UsingTarget2 && Target2.IsRetaining(element, out from)) return true;
            if (_UsingTarget3 && Target3.IsRetaining(element, out from)) return true;

            from = null;
            return false;
        }

        public Gizmo GetTarget(AnimationTarget target)
        {
            switch (target)
            {
                case AnimationTarget.Origin:
                    return Transform;
                case AnimationTarget.Target1:
                    return Target1;
                case AnimationTarget.Target2:
                    return Target2;
                case AnimationTarget.Target3:
                    return Target3;
            }
            return null;
        }

        public bool GetTargetTransform(AnimationTarget target, float progress, out Matrix4x4 result)
        {
            if (target == AnimationTarget.Origin) return GetTransform(progress, null, out result);

            if (Parent is AnimationObjectFragment animation) return animation.Root.GetTargetTransform(target, progress, out result);

            switch (target)
            {
                case AnimationTarget.Target1:
                    if (_UsingTarget1) return Target1.GetTransform(progress, out result);
                    break;
                case AnimationTarget.Target2:
                    if (_UsingTarget2) return Target2.GetTransform(progress, out result);
                    break;
                case AnimationTarget.Target3:
                    if (_UsingTarget3) return Target3.GetTransform(progress, out result);
                    break;
            }

            result = Matrix4x4.Identity;
            return false;
        }

        public bool GetTargetTransform(AnimationTarget target, out Matrix4x4 result) => GetTargetTransform(target, Progress, out result);
        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (name != null)
            {
                if (Object != null) return Object.GetTransform(progress, name, out result);
                result = Matrix4x4.Identity;
                return false;
            }
            return base.GetTransform(progress, null, out result);
        }

        public override void GetTransformNames(ICollection<string> names) => _Animation?.GetTransformNames(names);
        public override float Progress => Object?.Progress ?? 0f;
        internal override bool AddAABB(ref ABoundingBox result) => Object != null && Object.AddAABB(ref result);
        internal override void AddCollider(ref Collider result) => Object?.AddCollider(ref result);
        internal override bool CameraCapture(ref Camera camera) => Object?.CameraCapture(ref camera) ?? base.CameraCapture(ref camera);
        internal override bool CameraFilter(Graphics graphics) => Object?.CameraFilter(graphics) ?? base.CameraFilter(graphics);
        internal override bool AfterCameraUpdate() => Object?.AfterCameraUpdate() ?? base.AfterCameraUpdate();
        internal override void Rewind() => Object?.Rewind();
        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            var inflags = flags;

            if (_Animation != null)
            {
                if (_UsingTarget1 && IsTargetUsing(_Animation, AnimationTarget.Target1) && Target1.Update()) inflags |= UpdateFlags.Transform;
                if (_UsingTarget2 && IsTargetUsing(_Animation, AnimationTarget.Target2) && Target2.Update()) inflags |= UpdateFlags.Transform;
                if (_UsingTarget3 && IsTargetUsing(_Animation, AnimationTarget.Target3) && Target3.Update()) inflags |= UpdateFlags.Transform;
            }

            return Object != null ? Object.Update(lightSpace, delta, alive, inflags, ref flags) : UpdateState.None;
        }
        protected override void OnUpdatePass(ref UpdatePass pass)
        {
            if (_UsingTarget1 && !Target1.GetUpdatePass(ref pass)) return;
            if (_UsingTarget2 && !Target2.GetUpdatePass(ref pass)) return;
            if (_UsingTarget3 && !Target3.GetUpdatePass(ref pass)) return;
        }
        internal override ShowFlags Show() => Object?.Show(true) ?? ShowFlags.None;

        private static bool IsTargetUsing(AnimationFragment animation, AnimationTarget target)
        {
            if (animation.Target == target) return true;
            
            if (animation.Substance is AnimationReferenceObject substance && substance.Animation != null && IsTargetUsing(substance.Animation, target)) return true;

            foreach (var child in animation.Children)
            {
                if (IsTargetUsing(child, target)) return true;
            }
            return false;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == InstanceLayer.Cursor)
            {
                if (GetTransform(out var t0))
                {
                    var p0 = t0.Translation;

                    graphics.Push();

                    graphics.DepthMode = DepthMode.ReadWrite;
                    graphics.Material = Material.Default;
                    graphics.Material.Metallic = 0;
                    graphics.Material.Roughness = 1;

                    graphics.DrawSphere(p0, 10);

                    for (var target = AnimationTarget.Target1; target <= AnimationTarget.Target3; target++)
                    {
                        if (target == _SelectedTarget) graphics.Color = Color4.White;
                        else if (_Animation != null && IsTargetUsing(_Animation, target)) graphics.Color = Color4.DarkGray;
                        else continue;

                        if (GetTargetTransform(target, out var t1))
                        {
                            var p1 = t1.Translation;

                            if (p0 != p1)
                            {
                                graphics.Material.Shader = MaterialShader.NoLight;
                                graphics.DrawLine(p0, p1);
                                graphics.Material.Shader = MaterialShader.Light;
                                graphics.DrawSphere(p1, 10);
                            }
                        }
                    }
                    graphics.Pop();
                }
            }
            else Object?.Draw(graphics, layer, true);
        }

        internal override void Select(bool focus)
        {
            base.Select(focus);

            if (_UsingTarget1) Target1.Select(focus);
            if (_UsingTarget2) Target2.Select(focus);
            if (_UsingTarget3) Target3.Select(focus);
        }

        internal override void Offset(in Vector3 offset, bool retrieving)
        {
            if (_UsingTarget1 && Target1.TargetKey == null) Target1.Position += offset;
            if (_UsingTarget2 && Target1.TargetKey == null) Target2.Position += offset;
            if (_UsingTarget3 && Target1.TargetKey == null) Target3.Position += offset;

            base.Offset(offset, retrieving);
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            switch (_SelectedTarget)
            {
                case AnimationTarget.Target1:
                    if (_UsingTarget1 && Target1.KeyDown(e)) return true;
                    break;
                case AnimationTarget.Target2:
                    if (_UsingTarget2 && Target2.KeyDown(e)) return true;
                    break;
                case AnimationTarget.Target3:
                    if (_UsingTarget3 && Target3.KeyDown(e)) return true;
                    break;
            }
            return base.KeyDown(e);
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (_SelectedTarget)
            {
                case AnimationTarget.Target1:
                    if (_UsingTarget1 && Target1.MouseDown(e, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target2:
                    if (_UsingTarget2 && Target2.MouseDown(e, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target3:
                    if (_UsingTarget3 && Target3.MouseDown(e, controlKey, shiftKey)) return true;
                    break;
            }
            return base.MouseDown(e, controlKey, shiftKey);
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            switch (_SelectedTarget)
            {
                case AnimationTarget.Target1:
                    if (_UsingTarget1 && Target1.MouseMove(e, prevX, prevY, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target2:
                    if (_UsingTarget2 && Target2.MouseMove(e, prevX, prevY, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target3:
                    if (_UsingTarget3 && Target3.MouseMove(e, prevX, prevY, controlKey, shiftKey)) return true;
                    break;
            }
            return base.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        }

        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            switch (_SelectedTarget)
            {
                case AnimationTarget.Target1:
                    if (_UsingTarget1 && Target1.MouseUp(e, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target2:
                    if (_UsingTarget2 && Target2.MouseUp(e, controlKey, shiftKey)) return true;
                    break;
                case AnimationTarget.Target3:
                    if (_UsingTarget3 && Target3.MouseUp(e, controlKey, shiftKey)) return true;
                    break;
            }
            return base.MouseUp(e, controlKey, shiftKey);
        }

        protected override void SaveContent(XmlWriter writer)
        {
            base.SaveContent(writer);

            if (_UsingTarget1) Target1.Save(writer, "target1");
            if (_UsingTarget2) Target2.Save(writer, "target2");
            if (_UsingTarget3) Target3.Save(writer, "target3");
            if (_Keys != null) writer.WriteAttributes("keys", _Keys);
        }

        protected override void LoadContent(XmlNode node)
        {
            base.LoadContent(node);

            UsingTarget1 = node.HasAttribute("target1");
            if (_UsingTarget1) Target1.Load(node, "target1");
            UsingTarget2 = node.HasAttribute("target2");
            if (_UsingTarget2) Target2.Load(node, "target2");
            UsingTarget3 = node.HasAttribute("target3");
            if (_UsingTarget3) Target3.Load(node, "target3");
            Keys = node.HasAttribute("keys") ? node.ReadAttributeStrings("keys") : null;
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            writer.Write(_UsingTarget1);
            if (_UsingTarget1) Target1.Build(writer, param);
            writer.Write(_UsingTarget2);
            if (_UsingTarget2) Target2.Build(writer, param);
            writer.Write(_UsingTarget3);
            if (_UsingTarget3) Target3.Build(writer, param);

            writer.Write(KeyMask);
        }

        float ISceneAnimation.Progress => Progress;
        float ISceneAnimation.GetDuration(DurationParam param) => Object?.GetDuration(param) ?? 0f;
    }
}
