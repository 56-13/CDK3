using System;
using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public class ParticleShapeCone : ParticleShape
    {
        private float _BottomRange;
        public float BottomRange
        {
            set => SetProperty(ref _BottomRange, value);
            get => _BottomRange;
        }

        private float _TopRange;
        public float TopRange
        {
            set => SetProperty(ref _TopRange, value);
            get => _TopRange;
        }

        private float _Height;
        public float Height
        {
            set => SetProperty(ref _Height, value);
            get => _Height;
        }

        public ParticleShapeCone(Particle parent) : base(parent)
        {
            _BottomRange = 100;
            _TopRange = 150;
            _Height = 200;
        }

        public ParticleShapeCone(Particle parent, ParticleShapeCone other) : base(parent)
        {
            _BottomRange = other._BottomRange;
            _TopRange = other._TopRange;
            _Height = other._Height;
        }

        public override ParticleShapeType Type => ParticleShapeType.Cone;
        public override ParticleShape Clone(Particle parent) => new ParticleShapeCone(parent, this);

        internal override void AddAABB(in Matrix4x4 transform, ref ABoundingBox aabb)
        {
            var m = Math.Max(_BottomRange, _TopRange);

            foreach (var p in ABoundingBox.GetCorners(new Vector3(-m, -m, 0), new Vector3(m, m, _Height)))
            {
                aabb.Append(Vector3.Transform(p, transform));
            }
        }

        internal override void Issue(out Vector3 position, out Vector3 direction, bool shell)
        {
            var a = RandomUtil.NextFloat(-MathUtil.Pi, MathUtil.Pi);
            if (shell)
            {
                position = new Vector3((float)Math.Cos(a), (float)Math.Sin(a), RandomUtil.NextFloat(0, 1));
            }
            else
            {
                var r = RandomUtil.NextFloat(0, 1);
                position = new Vector3((float)Math.Cos(a) * r, (float)Math.Sin(a) * r, RandomUtil.NextFloat(0, 1));
            }

            var top = new Vector3(position.X * _TopRange, position.Y * _TopRange, _Height);
            var bottom = new Vector3(position.X * _BottomRange, position.Y * _BottomRange, 0);
            direction = Vector3.Normalize(top - bottom);
            var range = MathUtil.Lerp(_BottomRange, _TopRange, position.Z);
            position *= new Vector3(range, range, _Height);
        }

        internal override void Draw(Graphics graphics)
        {
            var world = graphics.World;
            graphics.DrawCircle(new Rectangle(-_BottomRange, -_BottomRange, _BottomRange * 2, _BottomRange * 2), false);
            graphics.World = Matrix4x4.CreateTranslation(0, 0, _Height) * world;
            graphics.DrawCircle(new Rectangle(-_TopRange, -_TopRange, _TopRange * 2, _TopRange * 2), false);
            graphics.World = world;
            graphics.DrawLine(new Vector3(_BottomRange, 0, 0), new Vector3(_TopRange, 0, _Height));
            graphics.DrawLine(new Vector3(0, _BottomRange, 0), new Vector3(0, _TopRange, _Height));
            graphics.DrawLine(new Vector3(-_BottomRange, 0, 0), new Vector3(-_TopRange, 0, _Height));
            graphics.DrawLine(new Vector3(0, -_BottomRange, 0), new Vector3(0, -_TopRange, _Height));
        }

        protected override string SaveToString() => $"{_TopRange},{_BottomRange},{_Height}";
        protected override void LoadFromString(string str)
        {
            var ps = str.Split(',');
            TopRange = float.Parse(ps[0]);
            BottomRange = float.Parse(ps[1]);
            Height = float.Parse(ps[2]);
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write(_TopRange);
            writer.Write(_BottomRange);
            writer.Write(_Height);
        }
    }
}
