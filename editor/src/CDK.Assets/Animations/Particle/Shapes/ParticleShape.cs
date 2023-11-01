using System;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public enum ParticleShapeType
    {
        Sphere,
        Hemisphere,
        Cone,
        Circle,
        Box,
        Rectangle
    }

    public abstract class ParticleShape : AssetElement
    {
        public Particle Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        protected ParticleShape(Particle parent)
        {
            Parent = parent;
        }

        public abstract ParticleShapeType Type { get; }
        public abstract ParticleShape Clone(Particle parent);
        internal abstract void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb);
        internal abstract void Issue(out Vector3 position, out Vector3 direction, bool shell);
        internal abstract void Draw(Graphics g);
        public static ParticleShape Create(Particle parent, ParticleShapeType type)
        {
            switch (type)
            {
                case ParticleShapeType.Sphere:
                    return new ParticleShapeSphere(parent);
                case ParticleShapeType.Hemisphere:
                    return new ParticleShapeHemisphere(parent);
                case ParticleShapeType.Cone:
                    return new ParticleShapeCone(parent);
                case ParticleShapeType.Circle:
                    return new ParticleShapeCircle(parent);
                case ParticleShapeType.Box:
                    return new ParticleShapeBox(parent);
                case ParticleShapeType.Rectangle:
                    return new ParticleShapeRectangle(parent);
            }
            throw new NotImplementedException();
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteAttributeString("shape", $"{Type};{SaveToString()}");
        }

        internal static ParticleShape Load(Particle parent, XmlNode node)
        {
            var str = node.ReadAttributeString("shape");
            var i = str.IndexOf(';');
            var type = (ParticleShapeType)Enum.Parse(typeof(ParticleShapeType), str.Substring(0, i));
            var shape = Create(parent, type);
            shape.LoadFromString(str.Substring(i + 1));
            return shape;
        }

        protected abstract string SaveToString();
        protected abstract void LoadFromString(string str);
        internal abstract void Build(BinaryWriter writer);
    }
}
