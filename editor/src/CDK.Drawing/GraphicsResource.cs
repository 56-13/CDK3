using System;
using System.Collections.Generic;

namespace CDK.Drawing
{
    [Flags]
    public enum BatchFlag
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3,
        Retrieve = 4
    }

    public abstract class GraphicsResource : IResource
    {
        public abstract void Dispose();
        public abstract int Cost { get; }
        protected internal virtual bool Batch(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes, BatchFlag flags)
        {
            var conf0 = (flags & BatchFlag.Read) != 0 && writes.Add(this);
            var conf1 = (flags & BatchFlag.Write) != 0 && reads.Add(this);
            if ((flags & BatchFlag.Read) != 0) reads.Add(this);
            if ((flags & BatchFlag.Write) != 0) writes.Add(this);
            return !conf0 && !conf1;
        }

        public virtual void Flush()
        {
            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) => { });
            command?.AddFence(this, BatchFlag.Read | BatchFlag.Retrieve);
        }
    }
}
