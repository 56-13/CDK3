using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Scenes
{
    public class SpotLightObject : SceneObject, IAnimationSubstance
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

        private float _Angle;
        [Binding]
        public float Angle
        {
            set
            {
                if (SetProperty(ref _Angle, value)) AddUpdateFlags(UpdateFlags.AABB);
            }
            get => _Angle;
        }

        private float _Dispersion;
        [Binding]
        public float Dispersion
        {
            set => SetProperty(ref _Dispersion, value);
            get => _Dispersion;
        }

        private LightAttenuation _Attenuation;
        [Binding]
        public LightAttenuation Attenuation
        {
            set
            {
                if (SetProperty(ref _Attenuation, value)) AddUpdateFlags(UpdateFlags.AABB);
            }
            get => _Attenuation;
        }

        private bool _CastShadow;
        [Binding]
        public bool CastShadow
        {
            set => SetProperty(ref _CastShadow, value);
            get => _CastShadow;
        }

        public Shadow Shadow { private set; get; }

        private float _progress;
        private int _random;

        private const float DefaultAttenuationRange = 10000;

        private static readonly Color3 DefaultColor = new Color3(5f, 5f, 5f);

        public SpotLightObject()
        {
            Color = new AnimationColor(this, false, false, DefaultColor);
            _Angle = 20;
            _Dispersion = 5;
            _Attenuation = LightAttenuation.Items.First(a => a.Range == DefaultAttenuationRange);
            _CastShadow = true;

            Shadow = new Shadow(this);

            AddUpdateFlags(UpdateFlags.AABB);

            _random = RandomUtil.Next();
        }

        public SpotLightObject(SpotLightObject other, bool binding) : base(other, binding, true)
        {
            if (binding)
            {
                Color = other.Color;
                Color.AddWeakRefresh(Color_BindingRefresh);
            }
            else Color = new AnimationColor(this, other.Color);

            _Duration = other._Duration;
            _Loop = other._Loop;
            _Angle = other._Angle;
            _Dispersion = other._Dispersion;
            _Attenuation = other._Attenuation;
            _CastShadow = other._CastShadow;
            
            Shadow = new Shadow(this, other.Shadow, binding);

            _random = RandomUtil.Next();
        }

        private void Color_BindingRefresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        public override SceneComponentType Type => SceneComponentType.SpotLight;
        public override SceneComponent Clone(bool binding) => new SpotLightObject(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
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

            var aabb = SceneUI.GetSpotLightCursorAABB(_Attenuation.Range, _Angle * MathUtil.ToRadians);
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
                var light = new SpotLight(
                    this,
                    transform.Translation,
                    Vector3.Normalize(transform.Forward()),
                    _Angle * MathUtil.ToRadians,
                    _Dispersion * MathUtil.ToRadians,
                    GetColor(),
                    _Attenuation,
                    _CastShadow,
                    Shadow);

                lightSpace.SetSpotLight(light);
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
                SceneUI.DrawSpotLightCursor(graphics, transform, GetColor(), _Attenuation.Range, _Angle * MathUtil.ToRadians);
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            Color.Save(writer, "color");
            writer.WriteAttribute("angle", _Angle);
            writer.WriteAttribute("dispersion", _Dispersion);
            writer.WriteAttribute("attenuation", _Attenuation.Range, DefaultAttenuationRange);
            writer.WriteAttribute("castShadow", _CastShadow, true);
            Shadow.Save(writer);
        }

        protected override void LoadContent(XmlNode node)
        {
            Color.Load(node, "color");
            Angle = node.ReadAttributeFloat("angle");
            Dispersion = node.ReadAttributeFloat("dispersion");
            var range = node.ReadAttributeFloat("attenuation", DefaultAttenuationRange);
            Attenuation = LightAttenuation.Items.First(a => a.Range == range);
            CastShadow = node.ReadAttributeBool("castShadow", true);
            Shadow.Load(node.GetChildNode("shadow"));
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Color.Build(writer);
            writer.Write(_Angle * MathUtil.ToRadians);
            writer.Write(_Dispersion * MathUtil.ToRadians);
            _Attenuation.Build(writer);
            writer.Write(_CastShadow);
            Shadow.Build(writer);
        }

        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new SpotLightObject(this, true) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new SpotLightObject(this, false) { Parent = parent };
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => BuildContent(writer, null);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
