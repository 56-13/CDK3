using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace CDK.Drawing
{
    public enum GraphicsPlatform
    {
        OpenGL,
        Vulcan,
        Metal
    }

    public abstract class GraphicsContext
    {
        private Dictionary<int, WeakReference<Graphics>> _graphics;

        protected GraphicsContext()
        {
            _graphics = new Dictionary<int, WeakReference<Graphics>>();
        }

        protected virtual void Dispose()
        {
            
        }

        public Graphics CurrentGraphics
        {
            get
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                lock (_graphics)
                {
                    if (_graphics.TryGetValue(threadId, out var e))
                    {
                        if (e.TryGetTarget(out var graphics)) return graphics;
                        _graphics.Remove(threadId);
                    }
                    return null;
                }
            }
        }

        internal void AttachGraphics(Graphics graphics)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (_graphics)
            {
                if (_graphics.TryGetValue(threadId, out var e)) e.SetTarget(graphics);
                else _graphics.Add(threadId, new WeakReference<Graphics>(graphics));
            }
        }

        internal void DetachGraphics(Graphics graphics)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            lock (_graphics)
            {
                if (_graphics.TryGetValue(threadId, out var e))
                {
                    if (!e.TryGetTarget(out var prev) || prev == graphics) _graphics.Remove(threadId);
                }
            }
        }

        internal void RemoveGraphics(Graphics graphics)
        {
            lock (_graphics)
            {
                foreach (var key in _graphics.Where(e => !e.Value.TryGetTarget(out var g) || g == graphics).Select(e => e.Key))
                {
                    _graphics.Remove(key);
                }
            }
        }

        internal abstract void ClearTargets(RenderTarget target);
        
        public abstract GraphicsPlatform Platform { get; }
        public abstract bool IsSupportParallel { get; }
        public abstract int MaxUniformBlockSize { get; }
        public abstract bool IsSupportRawFormat(RawFormat format);
        public abstract bool IsRenderThread(out GraphicsApi api);
        public abstract GraphicsApi AttachRenderThread();
        public abstract void DetachRenderThread();
        public abstract DelegateCommand Invoke(bool gsync, Action<GraphicsApi> inv);
        public abstract Control CreateControl();
        public abstract RenderTarget CreateControlRenderTarget(Control control);
        public abstract void MakeCurrent(Control control);
        public abstract void ResetCurrent();
        public abstract void SwapBuffers();

        private object _finishLock = new object();
        public void Finish()
        {
            if (!IsRenderThread(out _))
            {
                lock (_finishLock)
                {
                    Invoke(false, (GraphicsApi api) =>
                    {
                        lock (_finishLock)
                        {
                            Monitor.Pulse(_finishLock);
                        }
                    });
                    Monitor.Wait(_finishLock);
                }
            }
        }

        public static GraphicsContext Instance { private set; get; }
        public static bool IsCreated => Instance != null;
        public static void CreateShared()
        {
            if (Instance == null)
            {
                Instance = new OpenGL.OpenGLContext();       //currently, only support OpenGL
                ResourcePool.CreateShared();
                Buffers.CreateShared();
                Glyphs.CreateShared();
            }
        }

        public static void DisposeShared()
        {
            if (Instance != null)
            {
                Glyphs.DisposeShared();
                ResourcePool.DisposeShared();
                Buffers.DisposeShared();
                Shaders.DisposeShared();
                Renderers.DisposeShared();
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}
