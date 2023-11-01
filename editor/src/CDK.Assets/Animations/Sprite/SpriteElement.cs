using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Sprite.Elements;

namespace CDK.Assets.Animations.Sprite
{
    public enum SpriteElementType
    {
        Image,
        Mesh,
        String,         //TODO
        Line,
        GradientLine,
        Rect,
        GradientRect,
        RoundRect,         //TODO
        GradientRoundRect,         //TODO
        Arc,
        GradientArc,
        Box,
        Sphere,
        Capsule,         //TODO
        Cylinder,         //TODO
        Extern,         //TODO
        Translate,
        Rotate,
        Scale,
        Invert,
        Color,
        Stroke,
        Brightness,
        Contrast,
        Saturation,
        Blur,
        Lens,
        Wave
    }

    public abstract class SpriteElement : AssetElement
    {
        public struct TransformParam
        {
            public SpriteObject Parent;
            public Matrix4x4 Transform;
            public float Progress;
            public float Duration;
            public int Random;
        }

        public struct TransformUpdatedParam
        {
            public SpriteObject Parent;
            public float Progress0;
            public float Progress1;
            public float Duration;
            public UpdateFlags Inflags;
        }

        public struct DrawParam
        {
            public Graphics Graphics;
            public InstanceLayer Layer;
            public SpriteObject Parent;
            public float Progress;
            public float Duration;
            public int Random;
            public bool Visible;
        }

        public SpriteTimeline Parent { internal set; get; }
        public override AssetElement GetParent() => Parent;
        public abstract SpriteElementType Type { get; }
        public abstract SpriteElement Clone();
        internal virtual bool AddAABB(ref TransformParam param, ref ABoundingBox result) => false;
        internal virtual void AddCollider(ref TransformParam param, ref Collider result) { }
        internal virtual bool GetTransform(ref TransformParam param, string name, ref Matrix4x4 result) => false;
        internal virtual void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags) { }
        internal virtual void GetTransformNames(ICollection<string> names) { }
        internal virtual bool Pick(ref TransformParam param, in Ray ray, ref float distance)
        {
            var aabb = ABoundingBox.None;
            if (AddAABB(ref param, ref aabb) && ray.Intersects(aabb, CollisionFlags.None, out var d, out _) && d < distance)
            {
                distance = d;
                return true;
            }
            return false;
        }
        internal virtual ShowFlags ShowFlags => ShowFlags.None;
        internal abstract void Draw(ref DrawParam param);
        internal virtual bool KeyDown(SpriteObject parent, KeyEventArgs e) => false;
        internal virtual bool KeyUp(SpriteObject parent, KeyEventArgs e) => false;
        internal virtual bool MouseDown(SpriteObject parent, MouseEventArgs e, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseUp(SpriteObject parent, MouseEventArgs e, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseMove(SpriteObject parent, MouseEventArgs e, int px, int py, bool controlKey, bool shiftKey) => false;
        internal virtual bool MouseWheel(SpriteObject parent, MouseEventArgs e, bool controlKey, bool shiftKey) => false;
        internal abstract void Save(XmlWriter writer);
        internal abstract void Load(XmlNode node);
        internal abstract void Build(BinaryWriter writer);
        internal virtual void GetLocaleStrings(ICollection<LocaleString> strings) { }

        public static SpriteElement Create(SpriteElementType type)
        {
            switch(type)
            {
                case SpriteElementType.Image:
                    return new SpriteElementImage();
                case SpriteElementType.Mesh:
                    return new SpriteElementMesh();
                case SpriteElementType.Line:
                    return new SpriteElementLine();
                case SpriteElementType.GradientLine:
                    return new SpriteElementGradientLine();
                case SpriteElementType.Rect:
                    return new SpriteElementRect();
                case SpriteElementType.GradientRect:
                    return new SpriteElementGradientRect();
                case SpriteElementType.Arc:
                    return new SpriteElementArc();
                case SpriteElementType.GradientArc:
                    return new SpriteElementGradientArc();
                case SpriteElementType.Sphere:
                    return new SpriteElementSphere();
                case SpriteElementType.Box:
                    return new SpriteElementBox();
                case SpriteElementType.Translate:
                    return new SpriteElementTranslate();
                case SpriteElementType.Rotate:
                    return new SpriteElementRotate();
                case SpriteElementType.Scale:
                    return new SpriteElementScale();
                case SpriteElementType.Invert:
                    return new SpriteElementInvert();
                case SpriteElementType.Color:
                    return new SpriteElementColor();
                case SpriteElementType.Brightness:
                    return new SpriteElementBrightness();
                case SpriteElementType.Contrast:
                    return new ContrastSpriteElement();
                case SpriteElementType.Saturation:
                    return new SpriteElementSaturation();
                case SpriteElementType.Lens:
                    return new SpriteElementLens();
                case SpriteElementType.Wave:
                    return new SpriteElementWave();
            }

            throw new NotImplementedException();
        }
    }
}
