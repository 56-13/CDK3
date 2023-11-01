using System.Threading;

namespace CDK.Drawing
{
    public class RenderQueue
    {
        public RenderFrame Frame { private set; get; }
        public int Remaining => _remaining;

        private int _remaining;
        private int _commandCapacity;
        private int _readCapacity;
        private int _writeCapacity;
        private int _parallelCommandCapacity;
        private int _parallelLocalCommandCapacity;
        private bool _firstDone;

        public RenderQueue()
        {
            Frame = new RenderFrame();
        }

        public void Render()
        {
            Interlocked.Increment(ref _remaining);

            var frame = Frame;

            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) =>
            {
                frame.Render(api);

                _commandCapacity = frame.CommandCapacity + 8;
                _readCapacity = frame.ReadCapacity + 8;
                _writeCapacity = frame.WriteCapacity + 8;
                _parallelCommandCapacity = frame.ParallelCommandCapacity + 8;
                _parallelLocalCommandCapacity = frame.ParallelLocalCommandCapacity + 2;
                _firstDone = true;

                Interlocked.Decrement(ref _remaining);
            });

            Frame = _firstDone ? new RenderFrame(_commandCapacity, _readCapacity, _writeCapacity, _parallelCommandCapacity, _parallelLocalCommandCapacity) : new RenderFrame();
        }
    }
}
