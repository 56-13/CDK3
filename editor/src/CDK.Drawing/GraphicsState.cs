using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    internal class GraphicsState
    {
        public Matrix4x4 World;
        public Color4 Color;
        public List<Matrix4x4> WorldReserved;
        public List<Color4> ColorReserved;
        //public const CSFont* font;
        public Color4[] FontColors;
        public RenderState Batch;
        public Rectangle StencilBounds;
        public bool UsingStencil;
        public bool RenderOrder;
        public bool Mark;
        public GraphicsState Prev;

        public GraphicsState(RenderTarget target, GraphicsState prev)
        {
            Prev = prev;

            Reset(target);
        }

        public void Reset(RenderTarget target)
        {
            Debug.Assert(!UsingStencil);

            WorldReserved = null;
            ColorReserved = null;

            if (Prev != null)
            {
                World = Prev.World;
                Color = Prev.Color;
                //retain(font, prev->font);
                if (Prev.FontColors != null)
                {
                    if (FontColors == null) FontColors = new Color4[4];
                    FontColors[0] = Prev.FontColors[0];
                    FontColors[1] = Prev.FontColors[1];
                    FontColors[2] = Prev.FontColors[2];
                    FontColors[3] = Prev.FontColors[3];
                }
                else
                {
                    FontColors = null;
                }
                RenderOrder = Prev.RenderOrder;
                Batch = Prev.Batch;
            }
            else
            {
                World = Matrix4x4.Identity;
                Color = Color4.White;
                //retain(font, CSGraphics::defaultFont());
                FontColors = null;
                StencilBounds = Rectangle.ScreenNone;
                RenderOrder = true;
                Batch.Renderer = Renderers.Standard;
                Batch.Target = target;
                Batch.Camera = new Camera(0, target.Width, target.Height, 10, 10000);
                Batch.Material = Material.Default;
                Batch.FogColor = Color3.Black;
                Batch.FogNear = 0;
                Batch.FogFar = 0;
                Batch.BloomThreshold = 1;
                Batch.Brightness = 0;
                Batch.Contrast = 1;
                Batch.Saturation = 1;
                Batch.PolygonMode = PolygonMode.Fill;
                Batch.DepthMode = DepthMode.None;
                Batch.StencilMode = StencilMode.None;
                Batch.StencilDepth = 0;
                Batch.Layer = 0;
                Batch.StrokeWidth = 0;
                Batch.StrokeMode = StrokeMode.Normal;
                Batch.StrokeColor = Color4.Black;
                Batch.LineWidth = 1;
                Batch.Scissor = Rectangle.Zero;
                Batch.LightSpaceState = null;
                Batch.RendererParam = null;
            }
        }
    }
}
