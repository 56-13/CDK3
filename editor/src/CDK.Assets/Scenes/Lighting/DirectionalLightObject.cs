using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Animations;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Scenes
{
    public class DirectionalLightObject : SceneObject, IAnimationSubstance
    {
        public AnimationColor Color { private set; get; }
        
        private float _Duration;
        [Binding]
        public float Duration
        {
            set => SetProperty(ref _Duration, value);
            get => _Duration;
        }

        private AnimationLoop _Loop;
        [Binding]
        public AnimationLoop Loop
        {
            set => SetProperty(ref _Loop, value);
            get => _Loop;
        }

        private bool _CastShadow;
        [Binding]
        public bool CastShadow
        {
            set => SetProperty(ref _CastShadow, value);
            get => _CastShadow;
        }

        private bool _CastShadow2D;
        [Binding]
        public bool CastShadow2D
        {
            set => SetProperty(ref _CastShadow2D, value);
            get => _CastShadow2D;
        }

        public Shadow Shadow { private set; get; }

        private float _progress;
        private int _random;

        private static readonly Color3 DefaultColor = new Color3(5f, 5f, 5f);

        public DirectionalLightObject()
        {
            Color = new AnimationColor(this, false, false, DefaultColor);

            _CastShadow = true;

            Shadow = new Shadow(this);

            AddUpdateFlags(UpdateFlags.AABB);

            _random = RandomUtil.Next();
        }

        public DirectionalLightObject(DirectionalLightObject other, bool binding) : base(other, binding, true)
        {
            if (binding)
            {
                Color = other.Color;
                Color.AddWeakRefresh(Color_BindingRefresh);
            }
            else Color = new AnimationColor(this, other.Color);

            _Duration = other._Duration;
            _Loop = other._Loop;
            _CastShadow = other._CastShadow;
            _CastShadow2D = other._CastShadow2D;
            
            Shadow = new Shadow(this, other.Shadow, binding);

            _random = RandomUtil.Next();
        }

        private void Color_BindingRefresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        public override SceneComponentType Type => SceneComponentType.DirectionalLight;
        public override SceneComponent Clone(bool binding) => new DirectionalLightObject(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AllowDrag => true;

        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (!base.GetTransform(progress, name, out result)) return false;

            if (Matrix4x4.Decompose(result, out var scale, out var rotation, out var translation) && !VectorUtil.NearEqual(scale, Vector3.One))
            {
                result = Matrix4x4.CreateFromQuaternion(rotation);
                result.Translation = translation;
            }
            return true;
        }

        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (!GetTransform(out var transform)) return false;
            
            var aabb = SceneUI.GetDirectionalLightCursorAABB();
            ABoundingBox.Transform(aabb, transform, out aabb);
            result.Append(aabb);
            return true;
        }

        private Color3 GetColor()
        {
            if (_Duration > 0)
            {
                var cp = _Loop.GetProgress(_progress / _Duration, out var randomSeq0, out var randomSeq1);
                randomSeq0 *= 3;
                randomSeq1 *= 3;
                Color4 cr = new Color4(
                    RandomUtil.ToFloatSequenced(_random, randomSeq0, randomSeq1, cp),
                    RandomUtil.ToFloatSequenced(_random, randomSeq0 + 1, randomSeq1 + 1, cp),
                    RandomUtil.ToFloatSequenced(_random, randomSeq0 + 2, randomSeq1 + 2, cp),
                    1);

                return (Color3)Color.GetColor(cp, cr);
            }
            else
            {
                Color4 cr = new Color4(
                    RandomUtil.ToFloatSequenced(_random, 0),
                    RandomUtil.ToFloatSequenced(_random, 1),
                    RandomUtil.ToFloatSequenced(_random, 2),
                    1);

                return (Color3)Color.GetColor(1, cr);
            }
        }

        internal override void Rewind()
        {
            _progress = 0;
            _random = RandomUtil.Next();
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            _progress += delta;

            if ((flags & UpdateFlags.Transform) != 0) flags |= UpdateFlags.AABB;

            if (lightSpace != null && GetTransform(out var transform))
            {
                var light = new DirectionalLight(
                    this,
                    Vector3.Normalize(transform.Forward()),
                    GetColor(),
                    _CastShadow,
                    _CastShadow2D,
                    Shadow);

                lightSpace.SetDirectionalLight(light);
            }
            return UpdateState.None;
        }

        internal override bool CursorAlwaysVisible => true;
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer != InstanceLayer.Cursor || !Located) return;

            Transform?.Draw(graphics);

            if (GetTransform(out var transform))
            {
                SceneUI.DrawDirectionalLightCursor(graphics, transform, GetColor());
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            Color.Save(writer, "color");
            writer.WriteAttribute("castShadow", _CastShadow, true);
            writer.WriteAttribute("castShadow2D", _CastShadow2D);
        }

        protected override void LoadContent(XmlNode node)
        {
            Color.Load(node, "color");
            CastShadow = node.ReadAttributeBool("castShadow", true);
            CastShadow2D = node.ReadAttributeBool("castShadow2D");
            Shadow.Load(node.GetChildNode("shadow"));
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Color.Build(writer);
            writer.Write(_CastShadow);
            writer.Write(_CastShadow2D);
            Shadow.Build(writer);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new DirectionalLightObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new DirectionalLightObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => BuildContent(writer, null);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
