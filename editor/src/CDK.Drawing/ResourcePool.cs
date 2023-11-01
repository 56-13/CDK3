using System;
using System.Collections.Generic;

using Timer = System.Windows.Forms.Timer;

namespace CDK.Drawing
{
    public interface IResource : IDisposable
    {
        int Cost { get; }
    }

    public class ResourcePool
    {
        private class ResourceEntry
        {
            public object key;
            public IResource resource;
            public List<RenderFrame> blocks;
            public int life;
            public int elapsed;
            public bool recycle;
            public ResourceEntry prev;
            public ResourceEntry next;
        }

        private Dictionary<object, ResourceEntry> _resources;
        private List<ResourceEntry> _recycles;
        private ResourceEntry _first;
        private ResourceEntry _last;
        private Timer _timer;
        private int _elapsed;

        public const int FramePerSecond = 10;

        private ResourcePool()
        {
            _resources = new Dictionary<object, ResourceEntry>();
            _recycles = new List<ResourceEntry>();
            _timer = new Timer
            {
                Interval = 1000 / FramePerSecond
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();     
            //단일타겟 클라이언트라면 메인루프에서 직접 호출(60~FPS)해서 정확히 리소스관리가 가능
        }

        private void Dispose()
        {
            foreach (var e in _resources) e.Value.resource.Dispose();
            foreach (var e in _recycles) e.resource.Dispose();
            _timer.Dispose();
        }

        private void Timer_Tick(object obj, EventArgs e)
        {
            Purge(0);
            _elapsed++;
        }

        private void UseEntry(ResourceEntry e)
        {
            e.elapsed = _elapsed + Math.Max(e.life, 1);

            if (e != _first)
            {
                DetachEntry(e);

                if (_first == null) _first = _last = e;
                else
                {
                    var current = _first;

                    while (current != null && e.elapsed < current.elapsed) current = current.next;

                    if (current != null)
                    {
                        if (current.prev != null) current.prev.next = e;
                        else _first = e;
                        e.prev = current.prev;
                        e.next = current;
                        current.prev = e;
                    }
                    else
                    {
                        _last.next = e;
                        e.prev = _last;
                        _last = e;
                    }
                }
            }
        }

        private void DetachEntry(ResourceEntry e)
        {
            if (e.prev != null) e.prev.next = e.next;
            if (e.next != null) e.next.prev = e.prev;
            if (_first == e) _first = e.next;
            if (_last == e) _last = e.prev;
            e.next = null;
            e.prev = null;
        }

        private void RemoveEntry(ResourceEntry e, bool recycle)
        {
            if (e.key != null && _resources.Remove(e.key) && recycle && e.recycle)
            {
                e.key = null;
                e.blocks = null;
                e.life = 1;
                _recycles.Add(e);
                UseEntry(e);
            }
            else
            {
                DetachEntry(e);
#if (DEBUG && RESOURCE_POOL_LOG)
                Console.WriteLine($"resource remove:{(double)e.resource.Cost / 1024768:0.00000}mb");
#endif
                _recycles.Remove(e);
                e.resource.Dispose();
            }
        }

        private void BlockEntry(ResourceEntry e)
        {
            var frame = GraphicsContext.Instance.CurrentGraphics?.Queue.Frame;

            var add = frame != null;

            if (e.blocks == null)
            {
                if (add) e.blocks = new List<RenderFrame>(1) { frame };
            }
            else
            {
                var i = 0;
                while (i < e.blocks.Count)
                {
                    if (e.blocks[i] == frame)
                    {
                        add = false;
                        i++;
                    }
                    else if (e.blocks[i].Finished) e.blocks.RemoveAt(i);
                    else i++;
                }
                if (add) e.blocks.Add(frame);
            }
        }

        public long TotalCost
        {
            get
            {
                var cost = 0L;
                lock (this)
                {
                    var e = _first;
                    while (e != null)
                    {
                        cost += e.resource.Cost;
                        e = e.next;
                    }
                }
                return cost;
            }
        }

        public IResource Get(object key)
        {
            lock (this)
            {
                if (_resources.TryGetValue(key, out var e))
                {
                    UseEntry(e);
                    BlockEntry(e);
                    return e.resource;
                }
            }
            return null;
        }

        public void Add(object key, IResource resource, int life, bool recycle)
        {
            lock (this)
            {
                if (key == null) key = resource;

                if (!_resources.TryGetValue(key, out var e))
                {
                    e = new ResourceEntry
                    {
                        key = key
                    };
                    _resources.Add(key, e);
                }
                else if (resource != e.resource) e.resource.Dispose();

                e.resource = resource;
                e.life = life;
                e.recycle = recycle;
                UseEntry(e);
                BlockEntry(e);
#if (DEBUG && RESOURCE_POOL_LOG)
                Console.WriteLine($"resource add:{key}");
#endif
            }
        }

        public void Add(IResource resource, int life, bool recycle) => Add(null, resource, life, recycle);

        public void Remove(object key)
        {
            lock (this)
            {
                if (_resources.TryGetValue(key, out var e)) RemoveEntry(e, true);
            }
        }

        public delegate bool RecycleMatch(IResource candidate);

        public bool Recycle(RecycleMatch match, object key, int life, out IResource resource)
        {
            lock (this)
            {
                for (var i = 0; i < _recycles.Count; i++)
                {
                    var e = _recycles[i];

                    if (match(e.resource))
                    {
                        e.key = key ?? e.resource;
                        e.life = life;
                        _recycles.RemoveAt(i);
                        _resources.Add(e.key, e);
                        UseEntry(e);
                        BlockEntry(e);
                        resource = e.resource;
                        return true;
                    }
                }
            }
            resource = null;
            return false;
        }

        public bool Recycle(RecycleMatch match, int life, out IResource resource) => Recycle(match, null, life, out resource);

        public void Purge(long cost)
        {
#if (DEBUG && RESOURCE_POOL_LOG)
            if (cost > 0) Console.WriteLine($"resource purge manual:{cost / 1024768}mb:{TotalCost / 1024768}mb");
            var purge = false;
#endif
            lock (this)
            {
                var e = _last;
                while (e != null && e.elapsed <= _elapsed)
                {
                    var prev = e.prev;

                    var released = true;
                    if (e.blocks != null)
                    {
                        var i = 0;
                        while (i < e.blocks.Count)
                        {
                            if (e.blocks[i].Finished) e.blocks.RemoveAt(i);
                            else
                            {
                                i++;
                                released = false;
                            }
                        }
                    }
                    if (released)
                    {
                        var disposing = cost > 0;
                        if (disposing || e.life > 0)
                        {
                            cost -= e.resource.Cost;
                            RemoveEntry(e, !disposing);
#if (DEBUG && RESOURCE_POOL_LOG)
                            purge = true;
#endif
                        }
                    }

                    e = prev;
                }
            }
#if (DEBUG && RESOURCE_POOL_LOG)
            if (purge) Console.WriteLine($"resource purge:{TotalCost / 1024768}mb");
#endif
        }

        public static ResourcePool Instance { private set; get; }

        internal static void CreateShared()
        {
            if (Instance == null) Instance = new ResourcePool();
        }

        internal static void DisposeShared()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}
