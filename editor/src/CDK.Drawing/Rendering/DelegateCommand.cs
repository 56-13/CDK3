using System;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public class DelegateCommand : ICommand
    {
        private struct Fence
        {
            public GraphicsResource Resource;
            public BatchFlag Flags;

            public Fence(GraphicsResource resource, BatchFlag flags)
            {
                Resource = resource;
                Flags = flags;
            }
        }

        private Action<GraphicsApi> _invocation;
        private List<Fence> _fences;

        public DelegateCommand(Action<GraphicsApi> inv)
        {
            _invocation = inv;

            _fences = new List<Fence>();
        }

        public void AddFence(GraphicsResource resource, BatchFlag flags)
        {
            _fences.Add(new Fence(resource, flags));
        }

        public int Layer => 0;
        public RenderTarget Target => null;
        public IEnumerable<Rectangle> Bounds => null;
        public bool Submit() => true;
        public bool Parallel(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes)
        {
            var result = true;
            foreach (var e in _fences) result &= e.Resource.Batch(reads, writes, e.Flags);
            return result;
        }
        public bool FindBatch(ICommand command, ref ICommand candidate) => false;
        public void Batch(ICommand command) { }
        public void Render(GraphicsApi api, bool background, bool foreground)
        {
            Debug.Assert(background && foreground);
            _invocation(api);
        }
    }
}
