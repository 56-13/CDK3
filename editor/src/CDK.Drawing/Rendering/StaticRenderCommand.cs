using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    public class StaticRenderCommand : ICommand
    {
        public RenderState State;
        public Matrix4x4 World;
        public Color4 Color;
        private bool _renderOrder;
        private VertexArray _vertices;
        private Buffer _boneBuffer;
        private int _boneOffset;
        private PrimitiveMode _mode;
        private int _indexOffset;
        private int _indexCount;
        private ABoundingBox? _aabb;
        private BufferData<Instance> _instanceData;
        private List<Rectangle> _bounds;
        private Buffer _instanceBuffer;

        public StaticRenderCommand(Graphics graphics, VertexArray vertices, PrimitiveMode mode, int instanceCapacity = 1, ABoundingBox? aabb = null) :
            this(graphics, vertices, null, 0, mode, 0, vertices.IndexBuffer.Count, instanceCapacity, aabb)
        {

        }

        public StaticRenderCommand(Graphics graphics, VertexArray vertices, Buffer boneBuffer, int boneOffset, PrimitiveMode mode, int instanceCapacity = 1, ABoundingBox? aabb = null) :
            this(graphics, vertices, boneBuffer, boneOffset, mode, 0, vertices.IndexBuffer.Count, instanceCapacity, aabb)
        {

        }

        public StaticRenderCommand(Graphics graphics, VertexArray vertices, PrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity = 1, ABoundingBox? aabb = null) :
            this(graphics, vertices, null, 0, mode, indexOffset, indexCount, instanceCapacity, aabb)
        {

        }

        public StaticRenderCommand(Graphics graphics, VertexArray vertices, Buffer boneBuffer, int boneOffset, PrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity = 1, ABoundingBox? aabb = null)
        {
            State = graphics.State;
            World = graphics.World;
            Color = graphics.Color;
            _renderOrder = graphics.RenderOrder;
            _vertices = vertices;
            _indexOffset = indexOffset;
            _indexCount = indexCount;
            _boneBuffer = boneBuffer;
            _boneOffset = boneOffset;
            _aabb = aabb;
            _instanceData = new BufferData<Instance>(instanceCapacity);
            if (_aabb != null) _bounds = new List<Rectangle>(instanceCapacity);
            _mode = mode;
        }

        public void AddInstance(in VertexArrayInstance i)
        {
            if (_aabb != null)
            {
                var corners = _aabb.Value.GetCorners();
                var m = i.Model * World;

                if (State.Renderer.Clip(State, corners.Select(p => Vector3.Transform(p, m)), out var bounds))
                {
                    _instanceData.Add(new Instance(m, i.Color * State.Material.Color * Color, _boneOffset));
                    if (bounds.FullScreen) _bounds = null;
                    else if (_bounds != null) _bounds.Add(bounds);
                }
            }
            else
            {
                _instanceData.Add(new Instance(i.Model * World, i.Color * State.Material.Color * Color, _boneOffset));
            }
        }

        public int Layer => State.Layer;
        public RenderTarget Target => State.Target;
        public IEnumerable<Rectangle> Bounds => _bounds;

        public bool Submit()
        {
            State.Validate();
            State.Material.Color = Color4.White;       //pre appllied, reset for batching

            return State.Visible && _instanceData.Count != 0;
        }

        public bool Parallel(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes)
        {
            var result = _vertices.Batch(reads, writes, BatchFlag.ReadWrite | BatchFlag.Retrieve);
            if (_boneBuffer != null) result &= _boneBuffer.Batch(reads, writes, BatchFlag.Read);
            result &= State.Material.Batch(reads, writes);
            var flags = BatchFlag.Write | BatchFlag.Retrieve;
            if (_renderOrder && (State.DepthMode != DepthMode.ReadWrite || State.Material.BlendMode != BlendMode.None)) flags |= BatchFlag.Read;
            result &= State.Target.Batch(reads, writes, flags);
            return result;
        }

        public bool FindBatch(ICommand command, ref ICommand candidate)
        {
            if (command is StaticRenderCommand other && 
                other.State == State && 
                other._mode == _mode &&
                other._vertices == _vertices &&
                other._indexOffset == _indexOffset &&
                other._indexCount == _indexCount &&
                other._boneBuffer == _boneBuffer)
            {
                candidate = command;
                return true;
            }

            if (_bounds != null && command.Bounds != null && State.Target == command.Target)
            {
                foreach (Rectangle a in command.Bounds) {
                    foreach (Rectangle b in _bounds) {
                        if (a.Intersects(b)) return false;
                    }
                }
                return true;
            }
            return false;
        }

        public void Batch(ICommand command)
        {
            var other = (StaticRenderCommand)command;
            other._instanceData.AddRange(_instanceData);
            if (_bounds == null) other._bounds = null;
            else if (other._bounds != null) other._bounds.AddRange(_bounds);
        }

        private void AttachVertexInstances(GraphicsApi api)
        {
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceModel0, 4, VertexAttribType.Float, false, 76, 0, 1, true));
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceModel1, 4, VertexAttribType.Float, false, 76, 16, 1, true));
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceModel2, 4, VertexAttribType.Float, false, 76, 32, 1, true));
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceModel3, 4, VertexAttribType.Float, false, 76, 48, 1, true));
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceColor, 4, VertexAttribType.HalfFloat, false, 76, 64, 1, true));
            _vertices.AttachLayout(api, new VertexLayout(_instanceBuffer, RenderState.VertexAttribInstanceBoneOffset, 1, VertexAttribType.UnsignedInt, false, 76, 72, 1, _boneBuffer != null));
        }

        private void DetachVertexInstances(GraphicsApi api)
        {
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceBoneOffset);
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceColor);
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceModel3);
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceModel2);
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceModel1);
            _vertices.DetachLayout(api, RenderState.VertexAttribInstanceModel0);
        }

        public void Render(GraphicsApi api, bool background, bool foreground)
        {
            if (background)
            {
                _instanceBuffer = Buffers.FromData(_instanceData, BufferTarget.ArrayBuffer);
                _vertices.Bind(api);
                AttachVertexInstances(api);
                _vertices.Unbind(api);
            }
            if (foreground || State.UsingStroke)
            {
                State.Target.Focus(api);

                api.ApplyCullMode(State.Material.CullMode);
                api.ApplyDepthMode(State.Material.DepthTest ? State.DepthMode : DepthMode.None);
                api.ApplyStencilMode(State.StencilMode, State.StencilDepth);
                api.ApplyPolygonMode(State.PolygonMode);
                api.ApplyScissor(State.Scissor);
                api.ApplyLineWidth(State.LineWidth);

                if (background && State.UsingStroke)
                {
                    State.Target.SetDrawBuffers(api, 0);

                    Shaders.Stroke.Setup(api, State, _mode, _boneBuffer, true);

                    _vertices.Bind(api);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribPosition, true);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribColor, false);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTexCoord, State.Material.ColorMap != null);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribNormal, false);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTangent, false);
                    _vertices.DrawElementsInstanced(api, _mode, _indexOffset, _indexCount, _instanceBuffer.Count);
                    _vertices.Unbind(api);
                }
                if (foreground)
                {
                    if (State.Target.BloomSupported) State.Target.SetDrawBuffers(api, 0, 1);
                    else State.Target.SetDrawBuffers(api, 0);

                    api.ApplyBlendMode(State.Material.BlendMode);

                    var vertexColor = _vertices.HasAttrib(RenderState.VertexAttribColor);

                    State.Renderer.Setup(api, State, _mode, _boneBuffer, true, vertexColor);

                    _vertices.Bind(api);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribPosition, true);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribColor, vertexColor);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTexCoord, State.UsingMap);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribNormal, State.UsingVertexNormal);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTangent, State.UsingVertexTangent);
                    _vertices.DrawElementsInstanced(api, _mode, _indexOffset, _indexCount, _instanceBuffer.Count);
                    DetachVertexInstances(api);
                    _vertices.Unbind(api);

                    _instanceBuffer = null;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Instance : IEquatable<Instance>
        {
            public Matrix4x4 Model;
            public Half4 Color;
            public int BoneOffset;

            public Instance(in Matrix4x4 model, in Half4 color, int boneOffset)
            {
                Model = model;
                Color = color;
                BoneOffset = boneOffset;
            }

            public Instance(in Matrix4x4 model, in Color4 color, int boneOffset) : this(model, (Half4)color, boneOffset) { }

            public static bool operator ==(in Instance a, in Instance b) => a.Equals(b);
            public static bool operator !=(in Instance a, in Instance b) => !a.Equals(b);

            public override int GetHashCode()
            {
                var hash = HashCode.Initializer;
                hash.Combine(Model.GetHashCode());
                hash.Combine(Color.GetHashCode());
                hash.Combine(BoneOffset);
                return hash;
            }

            public bool Equals(Instance other)
            {
                return Model == other.Model &&
                    Color == other.Color &&
                    BoneOffset == other.BoneOffset;
            }

            public override bool Equals(object obj) => obj is Instance other && Equals(other);
        }
    }
}
