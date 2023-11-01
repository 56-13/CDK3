using System;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Sources;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public enum ImageSpriteElementShadowType
    {
        None,
        Flat,
        Rotate
    }

    public class SpriteElementImage : SpriteElement
    {
        public AnimationSourceImage Image { private set; get; }
        
        private Vector3 _Position;
        public Vector3 Position
        {
            set => SetProperty(ref _Position, value);
            get => _Position;
        }
        public AnimationFloat X { private set; get; }
        public AnimationFloat Y { private set; get; }
        public AnimationFloat Z { private set; get; }

        private Align _Align;
        public Align Align
        {
            set => SetProperty(ref _Align, value);
            get => _Align;
        }

        private ImageSpriteElementShadowType _ShadowType;
        public ImageSpriteElementShadowType ShadowType
        {
            set => SetProperty(ref _ShadowType, value);
            get => _ShadowType;
        }

        private float _ShadowDistance;
        public float ShadowDistance
        {
            set => SetProperty(ref _ShadowDistance, value);
            get => _ShadowDistance;
        }

        private Vector2 _ShadowFlatOffset;
        public Vector2 ShadowFlatOffset
        {
            set => SetProperty(ref _ShadowFlatOffset, value);
            get => _ShadowFlatOffset;
        }

        private bool _ShadowFlatXFlip;
        public bool ShadowFlatXFlip
        {
            set => SetProperty(ref _ShadowFlatXFlip, value);
            get => _ShadowFlatXFlip;
        }

        private bool _ShadowFlatYFlip;
        public bool ShadowFlatYFlip
        {
            set => SetProperty(ref _ShadowFlatYFlip, value);
            get => _ShadowFlatYFlip;
        }

        private Vector2 _ShadowRotateOffset;
        public Vector2 ShadowRotateOffset
        {
            set => SetProperty(ref _ShadowRotateOffset, value);
            get => _ShadowRotateOffset;
        }

        private float _ShadowRotateFlatness;
        public float ShadowRotateFlatness
        {
            set => SetProperty(ref _ShadowRotateFlatness, value); 
            get => _ShadowRotateFlatness;
        }

        public SpriteElementImage()
        {
            Image = new AnimationSourceImage(this);

            X = new AnimationFloat(this, -10000, 10000, 0);
            Y = new AnimationFloat(this, -10000, 10000, 0);
            Z = new AnimationFloat(this, -10000, 10000, 0);

            _Align = Align.CenterMiddle;
        }

        public SpriteElementImage(SpriteElementImage other)
        {
            Image = new AnimationSourceImage(this, other.Image);

            _Position = other._Position;

            X = new AnimationFloat(this, other.X);
            Y = new AnimationFloat(this, other.Y);
            Z = new AnimationFloat(this, other.Z);

            _Align = other._Align;
            _ShadowType = other._ShadowType;
            _ShadowDistance = other._ShadowDistance;
            _ShadowFlatOffset = other._ShadowFlatOffset;
            _ShadowFlatXFlip = other._ShadowFlatXFlip;
            _ShadowFlatYFlip = other._ShadowFlatYFlip;
            _ShadowRotateOffset = other._ShadowRotateOffset;
            _ShadowRotateFlatness = other._ShadowRotateFlatness;
        }

        public override SpriteElementType Type => SpriteElementType.Image;
        public override SpriteElement Clone() => new SpriteElementImage(this);
        internal override void AddRetains(ICollection<string> retains) => Image.AddRetains(retains);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => Image.IsRetaining(element, out from);

        private Vector3 GetPosition(float progress, int random)
        {
            var pos = _Position;
            pos.X += X.GetValue(progress, RandomUtil.ToFloatSequenced(random, 0));
            pos.Y += Y.GetValue(progress, RandomUtil.ToFloatSequenced(random, 1));
            pos.Z += Z.GetValue(progress, RandomUtil.ToFloatSequenced(random, 2));
            return pos;
        }

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result)
        {
            var image = Image.GetImage(param.Progress);

            if (image == null) return false;
            
            var pos = GetPosition(param.Progress, param.Random);

            var frame = image.GetDisplayFrame(pos, _Align);

            result.Append(Vector3.Transform(frame.LeftTop, param.Transform));
            result.Append(Vector3.Transform(frame.RightTop, param.Transform));
            result.Append(Vector3.Transform(frame.LeftBottom, param.Transform));
            result.Append(Vector3.Transform(frame.RightBottom, param.Transform));

            return true;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            var image1 = Image.GetImage(param.Progress1);
            if (image1 != null && ((param.Inflags & UpdateFlags.Transform) != 0 || X.IsAnimating || Y.IsAnimating || Z.IsAnimating)) outflags |= UpdateFlags.AABB;
            var image0 = Image.GetImage(param.Progress0);
            if (image0 != image1) outflags |= UpdateFlags.AABB;
        }

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible)
            {
                switch (param.Layer)
                {
                    case InstanceLayer.None:
                    case InstanceLayer.Shadow:
                    case InstanceLayer.Base:
                    case InstanceLayer.BlendBottom:
                    case InstanceLayer.BlendMiddle:
                    case InstanceLayer.BlendTop:
                        {
                            var image = Image.GetImage(param.Progress);
                            var texture = image?.Content.Texture;

                            if (texture != null && Image.Material.Apply(param.Graphics, param.Progress, param.Random, param.Layer, false))
                            {
                                var pos = GetPosition(param.Progress, param.Random);

                                param.Graphics.DrawImage(texture, image.UV, image.GetDisplayFrame(pos, _Align));
                            }
                        }
                        break;
                    case InstanceLayer.Shadow2D:
                        if (Image.Material.Shader == MaterialShader.NoLight && _ShadowType != ImageSpriteElementShadowType.None)
                        {
                            var image = Image.GetImage(param.Progress);

                            var texture = image?.Content.Texture;

                            if (texture != null)
                            {
                                if (Matrix4x4.Decompose(param.Graphics.World, out var scale, out var rotation, out var translation))
                                {
                                    var pos = GetPosition(param.Progress, param.Random);

                                    pos.Z += _ShadowDistance + translation.Z;

                                    rotation.X = 0;
                                    rotation.Y = 0;
                                    rotation.Z = -rotation.Z;
                                    rotation = Quaternion.Normalize(rotation);

                                    translation.Z = 0;

                                    param.Graphics.PushTransform();
                                    param.Graphics.World = Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateTranslation(translation);

                                    var uv = image.UV;
                                    var frame = image.GetDisplayFrame(pos, _Align);

                                    switch (_ShadowType)
                                    {
                                        case ImageSpriteElementShadowType.Flat:
                                            param.Graphics.DrawShadowFlatImage(texture, uv, frame, _ShadowFlatOffset, _ShadowFlatXFlip, _ShadowFlatYFlip);
                                            break;
                                        case ImageSpriteElementShadowType.Rotate:
                                            param.Graphics.DrawShadowRotateImage(texture, uv, frame, _ShadowRotateOffset, _ShadowRotateFlatness);
                                            break;
                                    }

                                    param.Graphics.PopTransform();
                                }
                            }
                        }
                        break;
                    case InstanceLayer.Cursor:
                        {
                            var image = Image.GetImage(param.Progress);

                            if (image != null)
                            {
                                var pos = GetPosition(param.Progress, param.Random);

                                param.Graphics.Material.Shader = MaterialShader.NoLight;
                                param.Graphics.DrawRect(image.GetDisplayFrame(pos, _Align), false);
                            }
                        }
                        break;
                }
            }
        }

        internal override bool MouseMove(SpriteObject parent, MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey && !shiftKey)
            {
                Position += parent.Scene.Camera.Right * (e.X - prevX) - parent.Scene.Camera.Up * (e.Y - prevY);
                return true;
            }
            return false;
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("image");
            Image.Save(writer);
            writer.WriteAttribute("position", _Position);
            X.Save(writer, "x");
            Y.Save(writer, "y");
            Z.Save(writer, "z");
            writer.WriteAttribute("align", _Align, Align.CenterMiddle);
            writer.WriteAttribute("shadowType", _ShadowType, ImageSpriteElementShadowType.None);
            writer.WriteAttribute("shadowDistance", _ShadowDistance);
            writer.WriteAttribute("shadowFlatOffset", _ShadowFlatOffset);
            writer.WriteAttribute("shadowFlatXFlip", _ShadowFlatXFlip);
            writer.WriteAttribute("shadowFlatYFlip", _ShadowFlatYFlip);
            writer.WriteAttribute("shadowRotateOffset", _ShadowRotateOffset);
            writer.WriteAttribute("shadowRotateFlatness", _ShadowRotateFlatness);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Image.Load(node);
            Position = node.ReadAttributeVector3("position");
            X.Load(node, "x");
            Y.Load(node, "y");
            Z.Load(node, "z");
            Align = node.ReadAttributeEnum("align", Align.CenterMiddle);

            ShadowType = node.ReadAttributeEnum("shadowType", ImageSpriteElementShadowType.None);
            ShadowDistance = node.ReadAttributeFloat("shadowDistance");
            ShadowFlatOffset = node.ReadAttributeVector2("shadowFlatOffset");
            ShadowFlatXFlip = node.ReadAttributeBool("shadowFlatXFlip");
            ShadowFlatYFlip = node.ReadAttributeBool("shadowFlatYFlip");
            ShadowRotateOffset = node.ReadAttributeVector2("shadowRotateOffset");
            ShadowRotateFlatness = node.ReadAttributeFloat("shadowRotateFlatness");
        }

        internal override void Build(BinaryWriter writer)
        {
            Image.Build(writer);
            writer.Write(_Position);
            X.Build(writer, false);
            Y.Build(writer, false);
            Z.Build(writer, false);
            writer.Write((byte)_Align);
            writer.Write((byte)_ShadowType);
            
            switch(_ShadowType)
            {
                case ImageSpriteElementShadowType.Flat:
                    writer.Write(_ShadowDistance);
                    writer.Write(_ShadowFlatOffset);
                    writer.Write(_ShadowFlatXFlip);
                    writer.Write(_ShadowFlatYFlip);
                    break;
                case ImageSpriteElementShadowType.Rotate:
                    writer.Write(_ShadowDistance);
                    writer.Write(_ShadowRotateOffset);
                    writer.Write(_ShadowRotateFlatness);
                    break;
            }
        }
    }
}
