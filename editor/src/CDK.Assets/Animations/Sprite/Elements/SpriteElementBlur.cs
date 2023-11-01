using System;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public enum BlurSpriteMode
    {
        Normal,
        Depth,
        Direction,
        Center
    }

    public class SpriteElementBlur : SpriteElement
    {
        private InstanceBlendLayer _Layer;
        public InstanceBlendLayer Layer
        {
            set => SetProperty(ref _Layer, value);
            get => _Layer;
        }

        private BlurSpriteMode _Mode;
        public BlurSpriteMode Mode
        {
            set => SetProperty(ref _Mode, value);
            get => _Mode;
        }

        private bool _UsingFrame;
        public bool UsingFrame
        {
            set => SetProperty(ref _UsingFrame, value);
            get => _UsingFrame;
        }

        private Rectangle _Frame;
        public Rectangle Frame
        {
            set => SetProperty(ref _Frame, value);
            get => _Frame;
        }

        public AnimationFloat Intensity { private set; get; }

        public AnimationFloat DepthDistance { private set; get; }
        public AnimationFloat DepthRange { private set; get; }
        public AnimationFloat DirectionX { private set; get; }
        public AnimationFloat DirectionY { private set; get; }
        public AnimationFloat CenterX { private set; get; }
        public AnimationFloat CenterY { private set; get; }
        public AnimationFloat CenterRange { private set; get; }

        public SpriteElementBlur()
        {
            _Layer = InstanceBlendLayer.Middle;
            
            Intensity = new AnimationFloat(this, 0, 8, 4);
            DepthDistance = new AnimationFloat(this, 1, 10000, 1000);
            DepthRange = new AnimationFloat(this, 1, 10000, 1000);
            DirectionX = new AnimationFloat(this, 0, 8, 4);
            DirectionY = new AnimationFloat(this, 0, 8, 4);
            CenterX = new AnimationFloat(this, -1000, 1000, 0);
            CenterY = new AnimationFloat(this, -1000, 1000, 0);
            CenterRange = new AnimationFloat(this, 0, 1000, 500);
        }

        public SpriteElementBlur(SpriteElementBlur other)
        {
            _Layer = other._Layer;
            _Mode = other._Mode;
            _UsingFrame = other._UsingFrame;
            _Frame = other._Frame;
            Intensity = new AnimationFloat(this, other.Intensity);
            DepthDistance = new AnimationFloat(this, other.DepthDistance);
            DepthRange = new AnimationFloat(this, other.DepthRange);
            DirectionX = new AnimationFloat(this, other.DirectionX);
            DirectionY = new AnimationFloat(this, other.DirectionY);
            CenterX = new AnimationFloat(this, other.CenterX);
            CenterY = new AnimationFloat(this, other.CenterY);
            CenterRange = new AnimationFloat(this, other.CenterRange);
        }
        public override SpriteElementType Type => SpriteElementType.Blur;
        public override SpriteElement Clone() => new SpriteElementBlur(this);

        internal override bool AddAABB(ref TransformParam param, ref ABoundingBox result) 
        {
            if (_UsingFrame)
            {
                result.Append(Vector3.Transform(_Frame.LeftTop.ToVector3(), param.Transform));
                result.Append(Vector3.Transform(_Frame.RightTop.ToVector3(), param.Transform));
                result.Append(Vector3.Transform(_Frame.LeftBottom.ToVector3(), param.Transform));
                result.Append(Vector3.Transform(_Frame.RightBottom.ToVector3(), param.Transform));
            }
            else
            {
                //max range
                result.Minimum = new Vector3(-100000);
                result.Maximum = new Vector3(100000);
            }
            return true;
        }

        internal override void GetTransformUpdated(ref TransformUpdatedParam param, ref UpdateFlags outflags)
        {
            if ((param.Inflags & UpdateFlags.Transform) != 0 && _UsingFrame) outflags |= UpdateFlags.AABB;
        }

        internal override void Draw(ref DrawParam param)
        {
            if (param.Visible)
            {
                if (param.Layer == (InstanceLayer)_Layer)
                {
                    switch (_Mode)
                    {
                        case BlurSpriteMode.Normal:
                            {
                                var intensity = Intensity.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 0));

                                if (_UsingFrame) param.Graphics.Blur(_Frame, intensity);
                                else param.Graphics.Blur(intensity);
                            }
                            break;
                        case BlurSpriteMode.Depth:
                            {
                                var distance = DepthDistance.Type != AnimationFloatType.None ?
                                    DepthDistance.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 0)) :
                                    Vector3.Distance(param.Graphics.Camera.Position, param.Graphics.Camera.Target);
                                var range = DepthRange.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 1));
                                var intensity = Intensity.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 2));

                                if (_UsingFrame) param.Graphics.BlurDepth(_Frame, distance, range, intensity);
                                else param.Graphics.BlurDepth(distance, range, intensity);
                            }
                            break;
                        case BlurSpriteMode.Direction:
                            {
                                var dir = new Vector2(
                                    DirectionX.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 0)),
                                    DirectionY.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 1)));

                                if (_UsingFrame) param.Graphics.BlurDirection(_Frame, dir);
                                else param.Graphics.BlurDirection(dir);
                            }
                            break;
                        case BlurSpriteMode.Center:
                            {
                                var center = new Vector2(
                                    CenterX.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 0)),
                                    CenterY.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 1)));

                                var range = CenterRange.GetValue(param.Progress, RandomUtil.ToFloatSequenced(param.Random, 2));

                                if (_UsingFrame) param.Graphics.BlurCenter(_Frame, center, range);
                                else param.Graphics.BlurCenter(center, range);
                            }
                            break;

                    }
                }
                else if (param.Layer == InstanceLayer.Cursor && _UsingFrame)
                {
                    param.Graphics.DrawRect(_Frame, false);
                }
            }
        }

        internal override bool MouseMove(SpriteObject parent, MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey && !shiftKey && _UsingFrame)
            {
                var frame = _Frame;
                frame.Offset((parent.Scene.Camera.Right * (e.X - prevX) - parent.Scene.Camera.Up * (e.Y - prevY)).ToVector2());
                Frame = frame;
                return true;
            }
            return false;
        }

        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("blur");
            writer.WriteAttribute("layer", _Layer, InstanceBlendLayer.Middle);
            writer.WriteAttribute("mode", _Mode, BlurSpriteMode.Normal);
            writer.WriteAttribute("usingFrame", _UsingFrame);
            writer.WriteAttribute("frame", _Frame);
            Intensity.Save(writer, "intensity");
            DepthDistance.Save(writer, "depthDistance");
            DepthRange.Save(writer, "depthRange");
            DirectionX.Save(writer, "directionX");
            DirectionY.Save(writer, "directionY");
            CenterX.Save(writer, "centerX");
            CenterY.Save(writer, "centerY");
            CenterRange.Save(writer, "centerRange");
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Layer = node.ReadAttributeEnum("layer", InstanceBlendLayer.Middle);
            Mode = node.ReadAttributeEnum("mode", BlurSpriteMode.Normal);
            UsingFrame = node.ReadAttributeBool("usingFrame");
            Frame = node.ReadAttributeRectangle("frame");
            Intensity.Load(node, "intensity");
            DepthDistance.Load(node, "depthDistance");
            DepthRange.Load(node, "depthRange");
            DirectionX.Load(node, "directionX");
            DirectionY.Load(node, "directionY");
            CenterX.Load(node, "centerX");
            CenterY.Load(node, "centerY");
            CenterRange.Load(node, "centerRange");
        }

        internal override void Build(BinaryWriter writer)
        {
            writer.Write((byte)_Layer);
            writer.Write((byte)_Mode);
            writer.Write(_UsingFrame);
            if (_UsingFrame) writer.Write(_Frame);
            switch (_Mode)
            {
                case BlurSpriteMode.Normal:
                    Intensity.Build(writer, false);
                    break;
                case BlurSpriteMode.Depth:
                    Intensity.Build(writer, false);
                    DepthDistance.Build(writer, false);
                    DepthRange.Build(writer, false);
                    break;
                case BlurSpriteMode.Direction:
                    DirectionX.Build(writer, false);
                    DirectionY.Build(writer, false);
                    break;
                case BlurSpriteMode.Center:
                    CenterX.Build(writer, false);
                    CenterY.Build(writer, false);
                    CenterRange.Build(writer, false);
                    break;
            }
        }
    }
}
