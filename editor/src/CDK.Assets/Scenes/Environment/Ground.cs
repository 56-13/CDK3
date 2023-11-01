using System;
using System.Linq;
using System.Numerics;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public class Ground : World
    {
        public override string Name
        {
            set { }
            get => "Ground";
        }

        private GroundConfig _Config;
        [Binding]
        public GroundConfig Config
        {
            set
            {
                if (value == null) throw new InvalidOperationException();

                var prev = _Config;
                if (SetProperty(ref _Config, value))
                {
                    prev.RemoveWeakRefresh(Config_Refresh);
                    _Config.AddWeakRefresh(Config_Refresh);
                }
            }
            get => _Config;
        }

        private bool _GridVisible;
        public bool GridVisible
        {
            set => SetProperty(ref _GridVisible, value, false);
            get => _GridVisible;
        }

        private float _progress;
        private int _random;

        public Ground(GroundConfig config)
        {
            _Config = config;
            _Config.AddWeakRefresh(Config_Refresh);

            _GridVisible = true;

            _random = RandomUtil.Next();
        }

        public Ground(Ground other, bool binding) : base(other, binding)
        {
            _Config = other.Config;
            _Config.AddWeakRefresh(Config_Refresh);
            _GridVisible = other._GridVisible;

            _random = RandomUtil.Next();
        }

        private void Config_Refresh(object sender, EventArgs e) => OnRefresh();

        public override SceneComponentType Type => SceneComponentType.Ground;
        public override SceneComponent Clone(bool binding) => new Ground(this, binding);
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override ABoundingBox Space => _Config.Space;
        public override int Grid => _Config.Grid;

        public override bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            if (ray.Intersects(new Plane(0, 0, 1, 0), out distance))
            {
                if ((flags & CollisionFlags.Hit) != 0)
                {
                    hit.Position = ray.Position + ray.Direction * distance;
                    hit.Direction = Vector3.UnitZ;
                }
                return true;
            }

            return false;
        }

        public override float GetZ(in Vector3 pos) => 0;

        internal override void Rewind()
        {
            _progress = 0;
            _random = RandomUtil.Next();
        }

        internal override void Update(float delta)
        {
            _progress += delta;
        }

        private void DrawCursor(Graphics graphics)
        {
            var w = _Config.Width * _Config.Grid * 0.5f;
            var h = _Config.Height * _Config.Grid * 0.5f;
            var z = _Config.Altitude * _Config.Grid;

            graphics.DrawRect(new ZRectangle(-w, -h, 0, w * 2, h * 2), false);
            graphics.DrawRect(new ZRectangle(-w, -h, z, w * 2, h * 2), false);
            graphics.DrawLine(new Vector3(-w, -h, 0), new Vector3(-w, -h, z));
            graphics.DrawLine(new Vector3(w, -h, 0), new Vector3(w, -h, z));
            graphics.DrawLine(new Vector3(-w, h, 0), new Vector3(-w, h, z));
            graphics.DrawLine(new Vector3(w, h, 0), new Vector3(w, h, z));
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            switch (layer)
            {
                case InstanceLayer.Shadow:
                    if (_Config.Material.ReceiveLight)
                    {
                        _Config.Draw(graphics, _progress, _random, _GridVisible);
                    }
                    break;
                case InstanceLayer.None:
                case InstanceLayer.Base:
                    _Config.Draw(graphics, _progress, _random, _GridVisible);
                    break;
                case InstanceLayer.Cursor:
                    DrawCursor(graphics);
                    break;
            }
        }

        protected override void SaveContent(XmlWriter writer)
        {
            writer.WriteAttribute("config", _Config.Key);
        }

        protected override void LoadContent(XmlNode node)
        {
            var config = Scene.Config;
            var key = node.ReadAttributeString("config");
            Config = config.Grounds.FirstOrDefault(e => e.Key == key) ?? config.Grounds[0];
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write((byte)BuildType.Ground);
            _Config.Build(writer);
        }
    }
}
