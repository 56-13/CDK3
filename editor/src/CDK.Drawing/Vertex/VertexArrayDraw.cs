
namespace CDK.Drawing
{
    public enum VertexArrayDrawType
    {
        Arrays,
        Elements,
        ElementsInstanced
    }

    public struct VertexArrayDraw
    {
        public VertexArray Vertices;
        public VertexArrayDrawType Type;
        public PrimitiveMode Mode;
        public int Offset;
        public int Count;
        public int InstanceCount;

        public static VertexArrayDraw Array(VertexArray vertices, PrimitiveMode mode, int vertexOffset, int vertexCount)
        {
            return new VertexArrayDraw()
            {
                Vertices = vertices,
                Type = VertexArrayDrawType.Arrays,
                Mode = mode,
                Offset = vertexOffset,
                Count = vertexCount
            };
        }

        public static VertexArrayDraw Elements(VertexArray vertices, PrimitiveMode mode)
        {
            return new VertexArrayDraw()
            {
                Vertices = vertices,
                Type = VertexArrayDrawType.Elements,
                Mode = mode,
                Count = vertices.IndexBuffer.Count
            };
        }

        public static VertexArrayDraw Elements(VertexArray vertices, PrimitiveMode mode, int indexOffset, int indexCount)
        {
            return new VertexArrayDraw()
            {
                Vertices = vertices,
                Type = VertexArrayDrawType.Elements,
                Mode = mode,
                Offset = indexOffset,
                Count = indexCount
            };
        }

        public static VertexArrayDraw ElementsInstanced(VertexArray vertices, PrimitiveMode mode, int instanceCount)
        {
            return new VertexArrayDraw()
            {
                Vertices = vertices,
                Type = VertexArrayDrawType.ElementsInstanced,
                Mode = mode,
                Count = vertices.IndexBuffer.Count,
                InstanceCount = instanceCount
            };
        }

        public static VertexArrayDraw ElementsInstanced(VertexArray vertices, PrimitiveMode mode, int indexOffset, int indexCount, int instanceCount)
        {
            return new VertexArrayDraw()
            {
                Vertices = vertices,
                Type = VertexArrayDrawType.ElementsInstanced,
                Mode = mode,
                Offset = indexOffset,
                Count = indexCount,
                InstanceCount = instanceCount
            };
        }

        public void Draw(GraphicsApi api)
        {
            switch (Type)
            {
                case VertexArrayDrawType.Arrays:
                    Vertices.DrawArrays(api, Mode, Offset, Count);
                    break;
                case VertexArrayDrawType.Elements:
                    Vertices.DrawElements(api, Mode, Offset, Count);
                    break;
                case VertexArrayDrawType.ElementsInstanced:
                    Vertices.DrawElementsInstanced(api, Mode, Offset, Count, InstanceCount);
                    break;
            }
        }
    }
}
