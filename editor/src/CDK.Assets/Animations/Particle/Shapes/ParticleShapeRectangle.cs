using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public class ParticleShapeRectangle : ParticleShape
    {
        private Vector2 _Range;
        public Vector2 Range
        {
            set => SetProperty(ref _Range, value);
            get => _Range;
        }
        public ParticleShapeRectangle(Particle parent) : base(parent)
        {
            _Range = new Vector2(100);
        }

        public ParticleShapeRectangle(Particle parent, ParticleShapeRectangle other) : base(parent)
        {
            _Range = other._Range;
        }

        public override ParticleShapeType Type => ParticleShapeType.Rectangle;
        public override ParticleShape Clone(Particle parent) => new ParticleShapeRectangle(parent, this);

        internal override void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb)
        {
            aabb.Append(Vector3.Transform(new Vector3(-_Range.X, -_Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(_Range.X, -_Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(-_Range.X, _Range.Y, 0), transform));
            aabb.Append(Vector3.Transform(new Vector3(_Range.X, _Range.Y, 0), transform));
        }

        internal override void Issue(out Vector3 position, out Vector3 direction, bool shell)
        {
            if (shell)
            {
                var x = RandomUtil.Next(0, 1) == 0;
                var neg = RandomUtil.Next(0, 1) == 1;
                
                if (x)
                {
                    position = new Vector3(
                        neg ? -_Range.X : _Range.X,
                        RandomUtil.NextFloat(-_Range.Y, _Range.Y),
                        0);
                    direction = new Vector3(neg ? -1 : 1, 0, 0);
                }
                else {
                    position = new Vector3(
                        RandomUtil.NextFloat(-_Range.X, _Range.X),
                        neg ? -_Range.Y : _Range.Y,
                        0);
                    direction = new Vector3(0, neg ? -1 : 1, 0);
                }
            }
            else
            {
                position = new Vector3(
                    RandomUtil.NextFloat(-_Range.X, _Range.X),
                    RandomUtil.NextFloat(-_Range.Y, _Range.Y),
                    0);

                direction = new Vector3(0, 0, 1);
            }
        }

        internal override void Draw(Graphics graphics)
        {
            graphics.DrawRect(new Rectangle(-_Range.X, -_Range.Y, _Range.X * 2, _Range.Y * 2), false);
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
