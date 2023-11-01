using System.Linq;
using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public class StreamRenderCommand : ICommand
    {
        public RenderState State;
        public Matrix4x4 World;
        public Color4 Color;
        private bool _renderOrder;
        private BufferData<FVertex> _vertexData;
        private VertexIndexData _indexData;
        private List<Rectangle> _bounds;
        private VertexArray _vertices;
        private PrimitiveMode _mode;

        private const int BufferPositionAndColor = 0;
        private const int BufferTexCoord = 1;
        private const int BufferNormal = 2;
        private const int BufferTangent = 3;
        private const int BufferCount = 4;

        private static readonly VertexLayout[] VertexLayouts = new VertexLayout[] {
            new VertexLayout(BufferPositionAndColor, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, 20, 0, 0, true),
            new VertexLayout(BufferPositionAndColor, RenderState.VertexAttribColor, 4, VertexAttribType.HalfFloat, false, 20, 12, 0, true),
            new VertexLayout(BufferTexCoord, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, 8, 0, 0, false),
            new VertexLayout(BufferNormal, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, 6, 0, 0, false),
            new VertexLayout(BufferTangent, RenderState.VertexAttribTangent, 3, VertexAttribType.HalfFloat, false, 6, 0, 0, false)
        };

        public StreamRenderCommand(Graphics graphics, PrimitiveMode mode) : this(graphics, mode, 4, 6)
        {
            
        }

        public StreamRenderCommand(Graphics graphics, PrimitiveMode mode, int vertexCapacity, int indexCapacity)
        {
            State = graphics.State;
            World = graphics.World;
            Color = graphics.Color;
            _renderOrder = graphics.RenderOrder;
            _vertexData = new BufferData<FVertex>(vertexCapacity);
            _indexData = new VertexIndexData(vertexCapacity, indexCapacity);
            _mode = mode;
        }

        public int VertexCount => _vertexData.Count;
        public int IndexCount => _indexData.Count;

        public void AddVertex(FVertex vertex)
        {
            vertex.Position = Vector3.Transform(vertex.Position, World);

            vertex.Color *= State.Material.Color * Color;

            if (State.UsingLight)
            {
                vertex.Normal = Vector3.Normalize(Vector3.TransformNormal(vertex.Normal, World));

                if (State.UsingVertexTangent)
                {
                    vertex.Tangent = Vector3.Normalize(Vector3.TransformNormal(vertex.Tangent, World));
                }
            }

            _vertexData.Add(vertex);
        }

        public void AddIndex(int index)
        {
            _indexData.Add(index);
        }

        public int Layer => State.Layer;
        public RenderTarget Target => State.Target;
        public IEnumerable<Rectangle> Bounds => _bounds;
        public bool Submit()
        {
            State.Validate();
            State.Material.Color = Color4.White;       //pre appllied, reset for batching

            if (State.Visible)
            {
                if (State.Renderer.Clip(State, _vertexData.Select(v => v.Position), out var bounds))
                {
                    if (!bounds.FullScreen) _bounds = new List<Rectangle> { bounds };
                    return true;
                }
            }
            return false;
        }
        public bool Parallel(HashSet<GraphicsResource> reads, HashSet<GraphicsResource> writes)
        {
            var result = State.Material.Batch(reads, writes);
            var flags = BatchFlag.Write | BatchFlag.Retrieve;
            if (_renderOrder && (State.DepthMode != DepthMode.ReadWrite || State.Material.BlendMode != BlendMode.None)) flags |= BatchFlag.Read;
            result &= State.Target.Batch(reads, writes, flags);
            return result;
        }

        public bool FindBatch(ICommand command, ref ICommand candidate)
        {
            if (command is StreamRenderCommand other && other.State == State && other._mode == _mode)
            {
                candidate = command;
                return true;
            }
            if (_bounds != null && command.Bounds != null && State.Target == command.Target)
            {
                foreach (Rectangle a in command.Bounds)
                {
                    foreach (Rectangle b in _bounds)
                    {
                        if (a.Intersects(b)) return false;
                    }
                }
                return true;
            }
            return false;
        }

        public void Batch(ICommand command)
        {
            var other = (StreamRenderCommand)command;
            foreach (var i in _indexData) other._indexData.Add(i + other._vertexData.Count);
            other._vertexData.AddRange(_vertexData);
            if (_bounds == null) other._bounds = null;
            else if (other._bounds != null) other._bounds.AddRange(_bounds);
        }

        public void Render(GraphicsApi api, bool background, bool foreground)
        {
            if (background)
            {
                _indexData.VertexCapacity = _vertexData.Count;

                var key = (State.UsingMap, State.UsingVertexNormal, State.UsingVertexTangent, _vertexData, _indexData);

                _vertices = (VertexArray)ResourcePool.Instance.Get(key);

                if (_vertices == null)
                {
                    _vertices = VertexArrays.New(key, 1, true, BufferCount, true, VertexLayouts);

                    var positionAndColors = new BufferData<VertexC>(_vertexData.Count);
                    var texCoords = State.UsingMap ? new BufferData<Vector2>(_vertexData.Count) : null;
                    var normals = State.UsingVertexNormal ? new BufferData<Half3>(_vertexData.Count) : null;
                    var tangents = State.UsingVertexTangent ? new BufferData<Half3>(_vertexData.Count) : null;

                    foreach (var vertex in _vertexData)
                    {
                        positionAndColors.Add(new VertexC(vertex.Position, vertex.Color));
                        if (texCoords != null) texCoords.Add(vertex.TexCoord);
                        if (normals != null) normals.Add((Half3)vertex.Normal);
                        if (tangents != null) tangents.Add((Half3)vertex.Tangent);
                    }
                    _vertices.GetVertexBuffer(BufferPositionAndColor).Upload(positionAndColors, BufferUsageHint.DynamicDraw);
                    if (texCoords != null) _vertices.GetVertexBuffer(BufferTexCoord).Upload(texCoords, BufferUsageHint.DynamicDraw);
                    if (normals != null) _vertices.GetVertexBuffer(BufferNormal).Upload(normals, BufferUsageHint.DynamicDraw);
                    if (tangents != null) _vertices.GetVertexBuffer(BufferTangent).Upload(tangents, BufferUsageHint.DynamicDraw);
                    _vertices.IndexBuffer.Upload(_indexData, BufferUsageHint.DynamicDraw);
                }
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

                    Shaders.Stroke.Setup(api, State, _mode, null, false);

                    _vertices.Bind(api);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribPosition, true);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribColor, false);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTexCoord, State.Material.ColorMap != null);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribNormal, false);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTangent, false);
                    _vertices.DrawElements(api, _mode);
                    _vertices.Unbind(api);
                }
                if (foreground)
                {
                    if (State.Target.BloomSupported) State.Target.SetDrawBuffers(api, 0, 1);
                    else State.Target.SetDrawBuffers(api, 0);

                    api.ApplyBlendMode(State.Material.BlendMode);

                    State.Renderer.Setup(api, State, _mode, null, false, true);

                    _vertices.Bind(api);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribPosition, true);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribColor, true);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTexCoord, State.UsingMap);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribNormal, State.UsingVertexNormal);
                    _vertices.SetAttribEnabled(api, RenderState.VertexAttribTangent, State.UsingVertexTangent);
                    _vertices.DrawElements(api, _mode);
                    _vertices.Unbind(api);

                    _vertices = null;
                }
            }
        }
    }
}
