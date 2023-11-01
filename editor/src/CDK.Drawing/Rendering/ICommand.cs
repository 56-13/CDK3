using System.Collections.Generic;

namespace CDK.Drawing
{
    public interface ICommand
    {
        int Layer { get; }
        RenderTarget Target { get; }
        IEnumerable<Rectangle> Bounds { get; }
        bool Submit();
        bool Parallel(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes);
        bool FindBatch(ICommand command, ref ICommand candidate);
        void Batch(ICommand command);
        void Render(GraphicsApi api, bool background, bool foreground);
    }
}
