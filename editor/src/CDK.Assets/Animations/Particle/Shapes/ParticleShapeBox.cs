using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public class ParticleShapeBox : ParticleShape
    {
        private Vector3 _Range;
        public Vector3 Range
        {
            set => SetProperty(ref _Range, value);
            get => _Range;
        }

        public ParticleShapeBox(Particle parent) : base(parent)
        {
            _Range = new Vector3(100);
        }

        public ParticleShapeBox(Particle parent, ParticleShapeBox other) : base(parent)
        {
            _Range = other._Range;
        }

        public override ParticleShapeType Type => ParticleShapeType.Box;
        public override ParticleShape Clone(Particle parent) => new ParticleShapeBox(parent, this);

        internal override void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb)
        {
            foreach (var p in ABoundingBox.GetCorners(-_Range, _Range))
            {
                aabb.Append(Vector3.Transform(p, transform));
            }
        }

        internal override void Issue(out Vector3 position, out Vector3 direction, bool shell)
        {
            if (shell)
            {
                var plane = RandomUtil.Next(0, 2);
                var neg = RandomUtil.Next(0, 1) == 1;
                
                switch (plane)
                {
                    case 0:
                        position = new Vector3(
                            neg ? -_Range.X : _Range.X,
                            RandomUtil.NextFloat(-_Range.Y, _Range.Y),
                            RandomUtil.NextFloat(-_Range.Z, _Range.Z));
                        direction = new Vector3(neg ? -1 : 1, 0, 0);
                        break;
                    case 1:
                        position = new Vector3(
                            RandomUtil.NextFloat(-_Range.X, _Range.X),
                            neg ? -_Range.Y : _Range.Y,
                            RandomUtil.NextFloat(-_Range.Z, _Range.Z));
                        direction = new Vector3(0, neg ? -1 : 1, 0);
                        break;
                    default:
                        position = new Vector3(
                            RandomUtil.NextFloat(-_Range.X, _Range.X),
                            RandomUtil.NextFloat(-_Range.Y, _Range.Y),
                            neg ? -_Range.Z : _Range.Z);
                        direction = new Vector3(0, 0, neg ? -1 : 1);
                        break;
                }
            }
            else
            {
                position = new Vector3(
                    RandomUtil.NextFloat(-_Range.X, _Range.X),
                    RandomUtil.NextFloat(-_Range.Y, _Range.Y),
                    RandomUtil.NextFloat(-_Range.Z, _Range.Z));
                direction = new Vector3(0, 0, 1);
            }
        }

        internal override void Draw(Graphics graphics)
        {
            graphics.DrawLine(new Vector3(-_Range.X, -_Range.Y, _Range.Z), new Vector3(_Range.X, -_Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(-_Range.X, _Range.Y, _Range.Z), new Vector3(_Range.X, _Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(-_Range.X, -_Range.Y, _Range.Z), new Vector3(-_Range.X, _Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(_Range.X, -_Range.Y, _Range.Z), new Vector3(_Range.X, _Range.Y, _Range.Z));

            graphics.DrawLine(new Vector3(-_Range.X, -_Range.Y, -_Range.Z), new Vector3(_Range.X, -_Range.Y, -_Range.Z));
            graphics.DrawLine(new Vector3(-_Range.X, _Range.Y, -_Range.Z), new Vector3(_Range.X, _Range.Y, -_Range.Z));
            graphics.DrawLine(new Vector3(-_Range.X, -_Range.Y, -_Range.Z), new Vector3(-_Range.X, _Range.Y, -_Range.Z));
            graphics.DrawLine(new Vector3(_Range.X, -_Range.Y, -_Range.Z), new Vector3(_Range.X, _Range.Y, -_Range.Z));

            graphics.DrawLine(new Vector3(-_Range.X, -_Range.Y, -_Range.Z), new Vector3(-_Range.X, -_Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(_Range.X, -_Range.Y, -_Range.Z), new Vector3(_Range.X, -_Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(-_Range.X, _Range.Y, -_Range.Z), new Vector3(-_Range.X, _Range.Y, _Range.Z));
            graphics.DrawLine(new Vector3(_Range.X, _Range.Y, -_Range.Z), new Vector3(_Range.X, _Range.Y, _Range.Z));
        }

        protected override string SaveToString() => $"{_Range.X},{_Range.Y},{_Range.Z}";
        protected override void LoadFromString(string str)
        {
            var ps = str.Split(',');
            Range = new Vector3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_Range.X);
            writer.Write(_Range.Y);
            writer.Write(_Range.Z);
        }
    }
}
