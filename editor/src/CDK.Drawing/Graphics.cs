using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public partial class Graphics : IDisposable
    {
        public RenderQueue Queue { private set; get; }

        private RenderTarget _target;
        private bool _targetOwn;
        private GraphicsState _state;

        public Graphics(RenderQueue queue, RenderTarget target, bool targetOwn = true)
        {
            Queue = queue;

            _target = target;
            _targetOwn = targetOwn;

            _state = new GraphicsState(target, null);
        }

        public Graphics(RenderTarget target, bool targetOwn = true) 
            : this(new RenderQueue(), target, targetOwn)
        {
        }

        ~Graphics()
        {
            if (GraphicsContext.IsCreated) GraphicsContext.Instance.RemoveGraphics(this);
        }

        public void Dispose()
        {
            GraphicsContext.Instance.RemoveGraphics(this);

            if (_targetOwn) _target.Dispose();

            GC.SuppressFinalize(this);
        }

        public void Clear(Color4 color) => Clear(color, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        public void Clear(Color4 color, ClearBufferMask mask)
        {
#if DEBUG
            var state = _state;
            do
            {
                Debug.Assert(!state.UsingStencil);
                state = state.Prev;
            } while (state != null);
#endif
            var target = _state.Batch.Target;
            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);
                api.Clear(color, mask);
            });
            command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
        }

        public void ClearStencil()
        {
            if (_state.UsingStencil)
            {
                if (_state.StencilBounds.OnScreen)
                {
                    var target = _state.Batch.Target;
                    var bounds = _state.StencilBounds;
                    var depth = _state.Batch.StencilDepth;

                    var command = Command((GraphicsApi api) =>
                    {
                        target.Focus(api);
                        api.ClearStencil(bounds, depth);
                    });
                    command.AddFence(target, BatchFlag.ReadWrite | BatchFlag.Retrieve);
                }
                _state.Batch.StencilDepth--;
                _state.StencilBounds = Rectangle.ScreenNone;
                _state.UsingStencil = false;
            }
        }

        public void Reset()
        {
            ClearStencil();

            _state.Reset(_target);
        }

        public void Push(bool mark = false)
        {
            _state.Mark = mark;
            _state = new GraphicsState(_target, _state);
        }

        public void Pop(bool mark = false)
        {
            if (mark)
            {
                while (_state.Prev != null && !_state.Prev.Mark)
                {
                    ClearStencil();

                    _state = _state.Prev;
                }
            }
            if (_state.Prev != null)
            {
                if (_state.Prev.Mark == mark)
                {
                    ClearStencil();

                    _state = _state.Prev;
                }
            }
            else
            {
                ClearStencil();

                _state.Reset(_target);
            }
        }

        public int PushCount
        {
            get
            {
                var count = 0;
                var current = _state;
                while (current.Prev != null)
                {
                    count++;
                    current = current.Prev;
                }
                return count;
            }
        }

        public RenderTarget Target
        {
            set => _state.Batch.Target = value;
            get => _state.Batch.Target;
        }

        public IRenderer Renderer
        {
            set => _state.Batch.Renderer = value;
            get => _state.Batch.Renderer;
        }
        public ref Camera Camera => ref _state.Batch.Camera;
        public ref Matrix4x4 World => ref _state.World;
        public Matrix4x4 WorldViewProjection => _state.World * _state.Batch.Camera.ViewProjection;
        public void Transform(in Matrix4x4 world) => _state.World = world * _state.World;
        public void Translate(in Vector3 v) => Transform(Matrix4x4.CreateTranslation(v));
        public void RotateYawPitchRoll(float yaw, float pitch, float roll) => Transform(Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll));
        public void RotateAxis(in Vector3 axis, float v) => Transform(Matrix4x4.CreateFromAxisAngle(axis, v));
        public void RotateX(float v) => Transform(Matrix4x4.CreateRotationX(v));
        public void RotateY(float v) => Transform(Matrix4x4.CreateRotationY(v));
        public void RotateZ(float v) => Transform(Matrix4x4.CreateRotationZ(v));
        public void Scale(in Vector3 v) => Transform(Matrix4x4.CreateScale(v));
        public void Scale(float v) => Transform(Matrix4x4.CreateScale(v));

        public void PushTransform()
        {
            if (_state.WorldReserved == null) _state.WorldReserved = new List<Matrix4x4>(1);
            _state.WorldReserved.Add(_state.World);
        }

        private void PeekTransform(bool pop)
        {
            if (_state.WorldReserved != null && _state.WorldReserved.Count != 0)
            {
                var last = _state.WorldReserved.Count - 1;
                _state.World = _state.WorldReserved[last];
                if (pop) _state.WorldReserved.RemoveAt(last);
            }
            else if (_state.Prev != null) _state.World = _state.Prev.World;
            else _state.World = Matrix4x4.Identity;
        }

        public void PopTransform() => PeekTransform(true);
        public void ResetTransform() => PeekTransform(false);

        public Color4 Color
        {
            set => SetColor(value, true);
            get => _state.Color;
        }

        public void SetColor(in Color4 color, bool inherit)
        {
            _state.Color = color;

            if (inherit)
            {
                if (_state.ColorReserved != null && _state.ColorReserved.Count != 0) _state.Color *= _state.ColorReserved[_state.ColorReserved.Count - 1];
                else if (_state.Prev != null) _state.Color *= _state.Prev.Color;
            }
        }

        public void PushColor()
        {
            if (_state.ColorReserved == null) _state.ColorReserved = new List<Color4>(1);
            _state.ColorReserved.Add(_state.Color);
        }

        private void PeekColor(bool pop)
        {
            if (_state.ColorReserved != null && _state.ColorReserved.Count != 0)
            {
                var last = _state.ColorReserved.Count - 1;
                _state.Color = _state.ColorReserved[last];
                if (pop) _state.ColorReserved.RemoveAt(last);
            }
            else if (_state.Prev != null) _state.Color = _state.Prev.Color;
            else _state.Color = Color4.White;
        }

        public void PopColor() => PeekColor(true);
        public void ResetColor() => PeekColor(false);

        public RenderState State => _state.Batch;
        public ref Material Material => ref _state.Batch.Material;

        public Color3 FogColor
        {
            set => _state.Batch.FogColor = value;
            get => _state.Batch.FogColor;
        }

        public float FogNear
        {
            set => _state.Batch.FogNear = value;
            get => _state.Batch.FogNear;
        }

        public float FogFar
        {
            set => _state.Batch.FogFar = value;
            get => _state.Batch.FogFar;
        }

        public bool UsingFog => _state.Batch.UsingFog;

        public void ClearFog()
        {
            FogNear = 0;
            FogFar = 0;
        }

        public float BloomThreshold
        {
            set => _state.Batch.BloomThreshold = value;
            get => _state.Batch.BloomThreshold;
        }

        public float Brightness
        {
            set => SetBrightness(value, true);
            get => _state.Batch.Brightness;
        }

        public void SetBrightness(float brightness, bool inherit)
        {
            if (_state.Prev != null && inherit)
            {
                brightness += _state.Prev.Batch.Brightness;
            }
            _state.Batch.Brightness = brightness;
        }

        public float Contrast
        {
            set => SetContrast(value, true);
            get => _state.Batch.Contrast;
        }

        public void SetContrast(float contrast, bool inherit)
        {
            if (_state.Prev != null && inherit)
            {
                contrast *= _state.Prev.Batch.Contrast;
            }
            _state.Batch.Contrast = contrast;
        }

        public float Saturation
        {
            set => SetSaturation(value, true);
            get => _state.Batch.Saturation;
        }

        public void SetSaturation(float saturation, bool inherit)
        {
            if (_state.Prev != null && inherit)
            {
                saturation *= _state.Prev.Batch.Saturation;
            }
            _state.Batch.Saturation = saturation;
        }

        public PolygonMode PolygonMode
        {
            set => _state.Batch.PolygonMode = value;
            get => _state.Batch.PolygonMode;
        }

        public DepthMode DepthMode
        {
            set => _state.Batch.DepthMode = value;
            get => _state.Batch.DepthMode;
        }

        public StencilMode StencilMode
        {
            set
            {
                _state.Batch.StencilMode = value;

                if (value == StencilMode.Inclusive && !_state.UsingStencil)
                {
                    Debug.Assert(_state.Batch.StencilDepth < 255);

                    _state.Batch.StencilDepth++;
                    _state.UsingStencil = true;
                }
            }
            get => _state.Batch.StencilMode;
        }

        public bool RenderOrder
        {
            set => _state.RenderOrder = value;
            get => _state.RenderOrder;
        }

        public byte Layer
        {
            set => _state.Batch.Layer = value;
            get => _state.Batch.Layer;
        }

        public byte StrokeWidth
        {
            set => _state.Batch.StrokeWidth = value;
            get => _state.Batch.StrokeWidth;
        }

        public StrokeMode StrokeMode
        {
            set => _state.Batch.StrokeMode = value;
            get => _state.Batch.StrokeMode;
        }

        public Color4 StrokeColor
        {
            set => SetStrokeColor(value, true);
            get => _state.Batch.StrokeColor;
        }

        public void SetStrokeColor(Color4 color, bool inherit)
        {
            if (_state.Prev != null && inherit)
            {
                color *= _state.Prev.Color;
            }
            _state.Batch.StrokeColor = color;
        }

        public Rectangle ConvertToTargetSpace(in Rectangle rect)
        {
            var viewport = _state.Batch.Target.Viewport;
            var xrate = viewport.Width / _state.Batch.Camera.Width;
            var yrate = viewport.Height / _state.Batch.Camera.Height;

            return new Rectangle(
                rect.X * xrate + viewport.X,
                rect.Y * yrate + viewport.Y,
                rect.Width * xrate,
                rect.Height * yrate);
        }

        public Rectangle Scissor
        {
            set
            {
                if (_state.Batch.Scissor != value)
                {
                    if (value != Rectangle.Zero)
                    {
                        value = ConvertToTargetSpace(value);

                        if (_state.Prev != null && _state.Prev.Batch.Scissor != Rectangle.Zero)
                        {
                            value.Intersect(_state.Prev.Batch.Scissor);
                        }
                    }
                    _state.Batch.Scissor = value;
                }
            }
            get => _state.Batch.Scissor;
        }

        public void ClearScissor() => Scissor = Rectangle.Zero;

        public float LineWidth
        {
            set => _state.Batch.LineWidth = value;
            get => _state.Batch.LineWidth;
        }

        public LightSpaceState LightSpaceState
        {
            set => _state.Batch.LightSpaceState = value;
            get => _state.Batch.LightSpaceState;
        }

        public object RendererParam
        {
            set => _state.Batch.RendererParam = value;
            get => _state.Batch.RendererParam;
        }

        public void DrawVertices(VertexArray vertices, PrimitiveMode mode, ABoundingBox? aabb = null, IEnumerable<VertexArrayInstance> instances = null)
        {
            DrawVertices(vertices, null, 0, mode, 0, vertices.IndexBuffer.Count, aabb, instances);
        }

        public void DrawVertices(VertexArray vertices, PrimitiveMode mode, int indexOffset, int indexCount, ABoundingBox? aabb = null, IEnumerable<VertexArrayInstance> instances = null)
        {
            DrawVertices(vertices, null, 0, mode, indexOffset, indexCount, aabb, instances);
        }

        public void DrawVertices(VertexArray vertices, Buffer boneBuffer, int boneOffset, PrimitiveMode mode, ABoundingBox? aabb = null, IEnumerable<VertexArrayInstance> instances = null)
        {
            DrawVertices(vertices, boneBuffer, boneOffset, mode, 0, vertices.IndexBuffer.Count, aabb, instances);
        }

        public void DrawVertices(VertexArray vertices, Buffer boneBuffer, int boneOffset, PrimitiveMode mode, int indexOffset, int indexCount, ABoundingBox? aabb = null, IEnumerable<VertexArrayInstance> instances = null)
        {
            var command = new StaticRenderCommand(this, vertices, boneBuffer, boneOffset, mode, indexOffset, indexCount, instances != null ? instances.Count() : 1, aabb);

            if (instances != null)
            {
                foreach (var i in instances) command.AddInstance(i);
            }
            else command.AddInstance(VertexArrayInstance.Origin);

            Command(command);
        }

        public void DrawSkybox(Texture skybox)
        {
            var target = Target;
            var camera = Camera;

            var command = Command((GraphicsApi api) =>
            {
                target.Focus(api);

                Shaders.Skybox.Draw(api, camera, skybox);
            });
            command.AddFence(skybox, BatchFlag.Read);
            command.AddFence(target, BatchFlag.Write | BatchFlag.Retrieve);
        }

        public void DrawSkybox()
        {
            var texture = _state.Batch.LightSpaceState?.EnvMap;

            if (texture != null) DrawSkybox(texture);
        }

        public int Remaining => Queue.Remaining;

        public void Command(ICommand command)
        {
            if (Queue.Frame.Command(command))
            {
                if (_state.Batch.StencilMode != StencilMode.None)
                {
                    var state = _state;
                    while (!state.UsingStencil)
                    {
                        state = state.Prev;
                        break;
                    }
                    if (command.Bounds != null)
                    {
                        foreach (var bounds in command.Bounds) state.StencilBounds.Append(bounds);
                    }
                    else state.StencilBounds = Rectangle.ScreenFull;
                }
            }
        }

        public DelegateCommand Command(Action<GraphicsApi> inv)
        {
            var command = new DelegateCommand(inv);
            Command(command);
            return command;

        }
        public void Focus() => GraphicsContext.Instance.AttachGraphics(this);
        public void UnFocus() => GraphicsContext.Instance.DetachGraphics(this);
        public void Render()
        {
            GraphicsContext.Instance.DetachGraphics(this);

            Queue.Render();
        }
    }
}
