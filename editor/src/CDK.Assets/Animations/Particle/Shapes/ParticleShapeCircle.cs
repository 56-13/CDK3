using System;
using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public class ParticleShapeCircle : ParticleShape
    {
        private Vector2 _Range;
        public Vector2 Range
        {
            set => SetProperty(ref _Range, value);
            get => _Range;
        }
        public ParticleShapeCircle(Particle parent) : base(parent)
        {
            _Range = new Vector2(100);
        }

        public ParticleShapeCircle(Particle parent, ParticleShapeCircle other) : base(parent)
        {
            _Range = other._Range;
        }

        public override ParticleShapeType Type => ParticleShapeType.Circle;
        public override ParticleShape Clone(Particle parent) => new ParticleShapeCircle(parent, this);

        internal override void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb)
        {
            aabb.Append(Vector3.Transform(new Vector3(-_Range.X, -_Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(_Range.X, -_Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(-_Range.X, _Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(_Range.X, _Range.Y, 0), transform));
        }

        internal override void Issue(out Vector3 position, out Vector3 direction, bool shell)
        {
            var a = RandomUtil.NextFloat(-MathUtil.Pi, MathUtil.Pi);
            if (shell)
            {
                position = new Vector3((float)Math.Cos(a) * _Range.X, (float)Math.Sin(a) * _Range.Y, 0);
            }
            else
            {
                float r = RandomUtil.NextFloat(0, 1);
                position = new Vector3((float)Math.Cos(a) * r * _Range.X, (float)Math.Sin(a) * r * _Range.Y, 0);
            }
            direction = Vector3.Normalize(position);
        }

        internal override void Draw(Graphics graphics)
        {
            graphics.DrawCircle(new Rectangle(-_Range.X, -_Range.Y, _Range.X * 2, _Range.Y * 2), false);
        }

        protected override string SaveToString() => $"{_Range.X},{_Range.Y}";

        protected override void LoadFromString(string str)
        {
            var ps = str.Split(',');
            Range = new Vector2(float.Parse(ps[0]), float.Parse(ps[1]));
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Range.X);
            writer.Write(_Range.Y);
        }
    }
}
