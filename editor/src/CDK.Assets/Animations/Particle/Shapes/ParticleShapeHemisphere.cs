using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public class ParticleShapeHemisphere : ParticleShape
    {
        private float _Range;
        public float Range
        {
            set => SetProperty(ref _Range, value);
            get => _Range;
        }

        public ParticleShapeHemisphere(Particle parent) : base(parent)
        {
            _Range = 100;
        }

        public ParticleShapeHemisphere(Particle parent, ParticleShapeHemisphere other) : base(parent)
        {
            _Range = other._Range;
        }

        public override ParticleShapeType Type => ParticleShapeType.Hemisphere;
        public override ParticleShape Clone(Particle parent) => new ParticleShapeHemisphere(parent, this);

        internal override void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb)
        {
            foreach (var p in ABoundingBox.GetCorners(new Vector3(-_Range, -_Range, 0), new Vector3(_Range)))
            {
                aabb.Append(Vector3.Transform(p, transform));
            }
        }

        internal override void Issue(out Vector3 position, out Vector3 direction, bool shell)
        {
            if (shell) position = new Vector3(0, 0, _Range);
            else position = new Vector3(0, 0, RandomUtil.NextFloat(0, _Range));

            var rotation = Quaternion.CreateFromYawPitchRoll(
                RandomUtil.NextFloat(-MathUtil.PiOverTwo, MathUtil.PiOverTwo),
                RandomUtil.NextFloat(-MathUtil.PiOverTwo, MathUtil.PiOverTwo),
                0);

            position = Vector3.Transform(position, rotation);
            direction = Vector3.Normalize(position);
        }

        internal override void Draw(Graphics g)
        {
            var world = g.World;
            g.DrawCircle(new Rectangle(-_Range, -_Range, _Range * 2, _Range * 2), false);
            g.World = Matrix4x4.CreateRotationX(MathUtil.PiOverTwo) * world;
            g.DrawArc(new Rectangle(-_Range, -_Range, _Range * 2, _Range * 2), 0, MathUtil.Pi, false);
            g.World = Matrix4x4.CreateRotationY(-MathUtil.PiOverTwo) * world;
            g.DrawArc(new Rectangle(-_Range, -_Range, _Range * 2, _Range * 2), -MathUtil.PiOverTwo, MathUtil.PiOverTwo, false);
            g.World = world;
        }

        protected override string SaveToString() => _Range.ToString();
        protected override void LoadFromString(string str) => Range = float.Parse(str);
        internal override void Build(BinaryWriter writer) => writer.Write(_Range);
    }
}
