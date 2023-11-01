using System;
using System.Linq;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public class VertexArray : GraphicsResource
    {
        public int Object { private set; get; }

        private Buffer[] _vertexBuffers;
        private Buffer _indexBuffer;
        private bool _ownIndexBuffer;
        private List<VertexLayout> _layouts;
        private bool[] _attribEnabled;

        public VertexArray(int vertexCount, bool indices, params VertexLayout[] layouts)
        {
            _vertexBuffers = new Buffer[vertexCount];

            for (var i = 0; i < vertexCount; i++) _vertexBuffers[i] = new Buffer(BufferTarget.ArrayBuffer);

            if (indices)
            {
                _indexBuffer = new Buffer(BufferTarget.ElementArrayBuffer);
                _ownIndexBuffer = true;
            }

            _attribEnabled = new bool[16];

            _layouts = new List<VertexLayout>(layouts);

            var command = GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
            {
                Object = api.GenVertexArray();

                Bind(api);

                if (_indexBuffer != null) api.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer.Object);

                foreach (var layout in layouts)
                {
                    api.BindBuffer(BufferTarget.ArrayBuffer, layout.Buffer != null ? layout.Buffer.Object : _vertexBuffers[layout.BufferIndex].Object);
                    api.VertexAttribPointer(layout.Attrib, layout.Size, layout.Type, layout.Normalized, layout.Stride, layout.Offset);
                    api.VertexAttribDivisor(layout.Attrib, layout.Divisor);
                    if (layout.EnabledByDefault)
                    {
                        api.SetVertexAttribEnabled(layout.Attrib, true);
                        _attribEnabled[layout.Attrib] = true;
                    }
                }

                Unbind(api);
            });

            if (command != null)
            {
                command.AddFence(this, BatchFlag.ReadWrite);
                foreach (var layout in _layouts)
                {
                    command.AddFence(layout.Buffer != null ? layout.Buffer : _vertexBuffers[layout.BufferIndex], BatchFlag.Read);
                }
            }
        }

        ~VertexArray()
        {
            if (GraphicsContext.IsCreated) GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteVertexArray(Object));
        }

        public override void Dispose()
        {
            GraphicsContext.Instance.Invoke(false, (GraphicsApi api) => api.DeleteVertexArray(Object));
            foreach (var buffer in _vertexBuffers) buffer.Dispose();
            if (_ownIndexBuffer) _indexBuffer?.Dispose();

            GC.SuppressFinalize(this);
        }

        public override int Cost
        {
            get
            {
                var cost = 0;
                foreach (var buffer in _vertexBuffers) cost += buffer.Cost;
                if (_ownIndexBuffer) cost += _indexBuffer.Cost;
                return cost;
            }
        }

        public Buffer GetVertexBuffer(int i) => _vertexBuffers[i];
        public int VertexBufferCount => _vertexBuffers.Length;
        public Buffer IndexBuffer => _indexBuffer;
        public bool OwnIndexBuffer => _ownIndexBuffer;
        public VertexLayout GetLayout(int i) => _layouts[i];
        public int LayoutCount => _layouts.Count;
        public IEnumerable<VertexLayout> Layouts => _layouts;

        protected internal override bool Batch(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes, BatchFlag flags) 
        {
            var result = base.Batch(reads, writes, flags);
            if ((flags & BatchFlag.Retrieve) != 0)
            {
                foreach (var vertexBuffer in _vertexBuffers) result &= vertexBuffer.Batch(reads, writes, flags);
                if (_indexBuffer != null) result &= _indexBuffer.Batch(reads, writes, flags);
                foreach (var e in _layouts)
                {
                    if (e.Buffer != null) result &= e.Buffer.Batch(reads, writes, flags);
                }
            }
            return result;
        }

        public void Bind(GraphicsApi api)
        {
            Debug.Assert(api.GetVertexArrayBinding() == 0);

            api.BindVertexArray(Object);
        }

        public void Unbind(GraphicsApi api)
        {
            Debug.Assert(api.GetVertexArrayBinding() == Object);

            api.BindVertexArray(0);
            api.BindBuffer(BufferTarget.ArrayBuffer, 0);
            api.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void AttachLayout(GraphicsApi api, in VertexLayout layout)
        {
            Debug.Assert(api.GetVertexArrayBinding() == Object && (layout.Buffer == null || layout.Buffer.Target == BufferTarget.ArrayBuffer));

            for (int i = 0; i < _layouts.Count; i++) { 
                var other = _layouts[i];

                if (other.Attrib == layout.Attrib)
                {
                    if (other.Buffer != layout.Buffer ||
                        other.BufferIndex != layout.BufferIndex ||
                        other.Attrib != layout.Attrib ||
                        other.Size != layout.Size ||
                        other.Type != layout.Type ||
                        other.Normalized != layout.Normalized ||
                        other.Stride != layout.Stride ||
                        other.Offset != layout.Offset)
                    {
                        api.BindBuffer(BufferTarget.ArrayBuffer, layout.Buffer != null ? layout.Buffer.Object : _vertexBuffers[layout.BufferIndex].Object);
                        api.VertexAttribPointer(layout.Attrib, layout.Size, layout.Type, layout.Normalized, layout.Stride, layout.Offset);
                    }
                    if (other.Divisor != layout.Divisor)
                    {
                        api.VertexAttribDivisor(layout.Attrib, layout.Divisor);
                    }
                    if (_attribEnabled[layout.Attrib] != layout.EnabledByDefault)
                    {
                        api.SetVertexAttribEnabled(layout.Attrib, layout.EnabledByDefault);
                        _attribEnabled[layout.Attrib] = layout.EnabledByDefault;
                    }
                    _layouts[i] = layout;
                    return;
                }
            }
            api.BindBuffer(BufferTarget.ArrayBuffer, layout.Buffer != null ? layout.Buffer.Object : _vertexBuffers[layout.BufferIndex].Object);
            api.VertexAttribPointer(layout.Attrib, layout.Size, layout.Type, layout.Normalized, layout.Stride, layout.Offset);
            api.VertexAttribDivisor(layout.Attrib, layout.Divisor);
            api.SetVertexAttribEnabled(layout.Attrib, layout.EnabledByDefault);
            _attribEnabled[layout.Attrib] = layout.EnabledByDefault;

            _layouts.Add(layout);
        }

        public void DetachLayout(GraphicsApi api, int attrib)
        {
            Debug.Assert(api.GetVertexArrayBinding() == Object);

            for (var i = 0; i < _layouts.Count; i++)
            {
                if (_layouts[i].Attrib == attrib)
                {
                    api.SetVertexAttribEnabled(attrib, false);
                    _attribEnabled[attrib] = false;
                    _layouts.RemoveAt(i);
                    break;
                }
            }
        }

        public bool HasAttrib(int attrib) => _layouts.Any(l => l.Attrib == attrib);

        public void AttachIndex(GraphicsApi api, Buffer indices)
        {
            Debug.Assert(!_ownIndexBuffer);

            if (_indexBuffer != indices)
            {
                Debug.Assert(api.GetVertexArrayBinding() == Object && indices.Target == BufferTarget.ElementArrayBuffer);
                api.BindBuffer(BufferTarget.ElementArrayBuffer, indices.Object);
                _indexBuffer = indices;
            }
        }

        public void SetAttribEnabled(GraphicsApi api, int attrib, bool enabled)
        {
            Debug.Assert(api.GetVertexArrayBinding() == Object);

            if (HasAttrib(attrib) && _attribEnabled[attrib] != enabled)
            {
                api.SetVertexAttribEnabled(attrib, enabled);
                _attribEnabled[attrib] = enabled;
            }
        }

        public bool IsAttribEnabled(int attrib) => _attribEnabled[attrib];

        private DrawElementsType ElementType
        {
            get
            {
                switch(_indexBuffer.Size)
                {
                    case 1:
                        return DrawElementsType.UnsignedByte;
                    case 2:
                        return DrawElementsType.UnsignedShort;
                    case 4:
                        return DrawElementsType.UnsignedInt;
                }
                throw new NotSupportedException();
            }
        }

        public void DrawArrays(GraphicsApi api, PrimitiveMode mode, int vertexOffset, int vertexCount)
        {
            var binding = api.GetVertexArrayBinding();
            Debug.Assert(binding == 0 || binding == Object);
            if (binding == 0) api.BindVertexArray(Object);
            api.DrawArrays(mode, vertexOffset, vertexCount);
            if (binding == 0) api.BindVertexArray(0);
        }

        public void DrawElements(GraphicsApi api, PrimitiveMode mode, int indexOffset, int indexCount)
        {
            var binding = api.GetVertexArrayBinding();
            Debug.Assert(indexOffset >= 0 && indexCount > 0 && indexOffset + indexCount <= _indexBuffer.Count && (binding == 0 || binding == Object));
            if (binding == 0) api.BindVertexArray(Object);
            api.DrawElements(mode, indexCount, ElementType, _indexBuffer.Size * indexOffset);
            if (binding == 0) api.BindVertexArray(0);
        }

        public void DrawElements(GraphicsApi api, PrimitiveMode mode)
        {
            var binding = api.GetVertexArrayBinding();
            Debug.Assert(_indexBuffer.Count > 0 && (binding == 0 || binding == Object));
            if (binding == 0) api.BindVertexArray(Object);
            api.DrawElements(mode, _indexBuffer.Count, ElementType, 0);
            if (binding == 0) api.BindVertexArray(0);
        }

        public void DrawElementsInstanced(GraphicsApi api, PrimitiveMode mode, int indexOffset, int indexCount, int instanceCount)
        {
            var binding = api.GetVertexArrayBinding();
            Debug.Assert(indexOffset >= 0 && indexCount > 0 && indexOffset + indexCount <= _indexBuffer.Count && (binding == 0 || binding == Object));
            if (binding == 0) api.BindVertexArray(Object);
            api.DrawElementsInstanced(mode, indexCount, ElementType, _indexBuffer.Size * indexOffset, instanceCount);
            if (binding == 0) api.BindVertexArray(0);
        }

        public void DrawElementsInstanced(GraphicsApi api, PrimitiveMode mode, int instanceCount)
        {
            var binding = api.GetVertexArrayBinding();
            Debug.Assert(_indexBuffer.Count > 0 && (binding == 0 || binding == Object));
            if (binding == 0) api.BindVertexArray(Object);
            api.DrawElementsInstanced(mode, _indexBuffer.Count, ElementType, 0, instanceCount);
            if (binding == 0) api.BindVertexArray(0);
        }
    }
}
