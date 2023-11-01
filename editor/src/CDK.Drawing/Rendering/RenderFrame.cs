using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDK.Drawing
{
    public class RenderFrame
    {
        private List<ICommand> _commands;
        private HashSet<GraphicsResource> _reads;
        private HashSet<GraphicsResource> _writes;
        private List<ICommand> _parallelCommands;
        private List<ICommand> _parallelLocalCommands;
        public int CommandCapacity { private set; get; }
        public int ReadCapacity { private set; get; }
        public int WriteCapacity { private set; get; }
        public int ParallelCommandCapacity { private set; get; }
        public int ParallelLocalCommandCapacity { private set; get; }
        public bool Finished { private set; get; }
        
        public const int SequentialRenderLimit = 2;

        public RenderFrame()
        {
            _commands = new List<ICommand>();
            _reads = new HashSet<GraphicsResource>();
            _writes = new HashSet<GraphicsResource>();

            if (GraphicsContext.Instance.IsSupportParallel)
            {
                _parallelCommands = new List<ICommand>();
                _parallelLocalCommands = new List<ICommand>();
            }
        }

        public RenderFrame(int commandCapacity, int readCapacity, int writeCapacity, int parallelCommandCapacity, int parallelLocalCommandCapacity)
        {
            _commands = new List<ICommand>(commandCapacity);
            _reads = new HashSet<GraphicsResource>(readCapacity);
            _writes = new HashSet<GraphicsResource>(writeCapacity);

            if (GraphicsContext.Instance.IsSupportParallel)
            {
                _parallelCommands = new List<ICommand>(parallelCommandCapacity);
                _parallelLocalCommands = new List<ICommand>(parallelLocalCommandCapacity);
            }
        }

        public bool Command(ICommand command)
        {
            if (!command.Submit()) return false;

            lock (_commands)
            {
                _commands.Add(command);
            }
            return true;
        }
        
        private void Render(GraphicsApi api, int from, int to, bool background, bool foreground)
        {
            if (GraphicsContext.Instance.IsSupportParallel)
            {
                for (var i = from; i < to; i++) _parallelCommands.Add(_commands[i]);

                if (ParallelCommandCapacity < _parallelCommands.Count) ParallelCommandCapacity = _parallelCommands.Count;

                while (_parallelCommands.Count != 0)
                {
                    var i = 0;
                    while (i < _parallelCommands.Count)
                    {
                        var command = _parallelCommands[i];
                        if (command.Parallel(_reads, _writes))
                        {
                            _parallelLocalCommands.Add(command);
                            _parallelCommands.RemoveAt(i);
                        }
                        else i++;
                    }
                    if (ReadCapacity < _reads.Count) ReadCapacity = _reads.Count;
                    if (WriteCapacity < _writes.Count) WriteCapacity = _writes.Count;
                    _reads.Clear();
                    _writes.Clear();

                    if (ParallelLocalCommandCapacity < _parallelLocalCommands.Count) ParallelLocalCommandCapacity = _parallelLocalCommands.Count;

                    if (_parallelLocalCommands.Count > SequentialRenderLimit)
                    {
                        var tasks = new Task[_parallelLocalCommands.Count];

                        for (i = 0; i < _parallelLocalCommands.Count; i++)
                        {
                            var command = _parallelLocalCommands[i];

                            tasks[i] = Task.Run(() => {
                                //사용중인 명령버퍼가 없다면 멀티쓰레드에서 명령버퍼를 풀링하고 해당 쓰레드에 붙이고 뗌, 추후 코드체크
                                GraphicsApi papi = GraphicsContext.Instance.AttachRenderThread();
                                try
                                {
                                    command.Render(papi, background, foreground);       //TODO:공유자원의 fence문제. 추후 실제 Metal / Vulcan을 작업하게 되면 확인
                                }
                                finally
                                {
                                    GraphicsContext.Instance.DetachRenderThread();
                                }
                            });
                        }
                        Task.WaitAll(tasks);
                    }
                    else
                    {
                        foreach (var command in _parallelLocalCommands) command.Render(api, background, foreground);
                    }
                    _parallelLocalCommands.Clear();
                }
            }
            else
            {
                for (var i = from; i < to; i++) _commands[i].Render(api, background, foreground);
            }
        }


        internal void Render(GraphicsApi api)
        {
            CommandCapacity = _commands.Count;

            int i;

            for (i = _commands.Count - 1; i >= 0; i--)
            {
                var command = _commands[i];

                command.Parallel(_reads, _writes);

                ICommand batch = null;

                int j = i - 1;
                while (j >= 0)
                {
                    var nextCommand = _commands[j];

                    var flag0 = command.FindBatch(nextCommand, ref batch);
                    var flag1 = nextCommand.Parallel(_reads, _writes);
                    if (flag0 || flag1) j--;
                    else break;
                }

                if (batch != null)
                {
                    command.Batch(batch);
                    _commands.RemoveAt(i);
                }
                if (ReadCapacity < _reads.Count) ReadCapacity = _reads.Count;
                if (WriteCapacity < _writes.Count) WriteCapacity = _writes.Count;
                _reads.Clear();
                _writes.Clear();
            }


            i = 0;
            while (i < _commands.Count)
            {
                var command = _commands[i];
                var layer = command.Layer;
                var end = i + 1;
                while (end < _commands.Count)
                {
                    if (_commands[i].Layer == layer) end++;
                }
                if (layer == 0) Render(api, i, end, true, true);
                else
                {
                    Render(api, i, end, true, false);
                    Render(api, i, end, false, true);
                }
                i = end;
            }

            Finished = true;
        }
    }
}
