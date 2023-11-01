using System.Numerics;

using CDK.Drawing;

using CDK.Assets.Scenes;

using GDIRectangle = System.Drawing.Rectangle;

namespace CDK.Assets.Texturing
{
    public abstract class ImageAsset : Asset
    {
        public abstract GDIRectangle Frame { get; }
        public abstract Vector2 Pivot { get; }
        public abstract TextureSlot Content { get; }
        public abstract float ContentScale { set; get; }
        public abstract Rectangle UV { get; }
        public abstract ScreenColor ScreenColor { get; }

        protected ImageAsset()
        {

        }

        protected ImageAsset(ImageAsset asset, bool content)
            : base(asset, content)
        {
        }

        public ZRectangle GetDisplayFrame(in Vector3 pos, Align align)
        {
            var pivot = Pivot;
            var originFrame = Frame;
            var contentScale = ContentScale;

            var frame = new ZRectangle(
                pos,
                originFrame.Width * contentScale,
                originFrame.Height * contentScale);

            if (((int)align & AlignComponent.Center) != 0)
            {
                frame.X -= frame.Width * 0.5f + pivot.X * contentScale;
            }
            else if (((int)align & AlignComponent.Right) != 0)
            {
                frame.X -= frame.Width;
            }
            if (((int)align & AlignComponent.Middle) != 0)
            {
                frame.Y -= frame.Height * 0.5f + pivot.Y * contentScale;
            }
            else if (((int)align & AlignComponent.Bottom) != 0)
            {
                frame.Y -= frame.Height;
            }
            return frame;
        }

        public bool GetDisplay(in Vector3 pos, Align align, out Texture texture, out Rectangle uv, out ZRectangle frame)
        {
            texture = Content.Texture;

            if (texture == null) 
            {
                uv = Rectangle.Zero;
                frame = Rectangle.Zero;
                return false;
            }

            uv = UV;
            
            frame = GetDisplayFrame(pos, align);

            return true;
        }

        public override SceneComponent NewSceneComponent() => new ImageObject(this);

        public override Scene NewScene()
        {
            var obj = new ImageObject(this) { Fixed = true };

            var backColor = ScreenColor.Background;

            var scene = new Scene(this)
            {
                Seperated = true,
                BackColor = new Color4(backColor.R, backColor.G, backColor.B, backColor.A),
                CameraGizmo = false
            };
            
            scene.Children.Add(obj);

            scene.SelectedComponent = obj;
            return scene;
        }
    }
}
