using System.Linq;

namespace CDK.Drawing
{
    public static partial class VertexArrays
    {
        public static VertexArray New(object key, int life, bool recycle, int vertexCount, bool indices, params VertexLayout[] layouts)
        {
            bool match(IResource candidate)
            {
                return candidate is VertexArray vertices &&
                    vertices.VertexBufferCount == vertexCount &&
                    vertices.OwnIndexBuffer == indices &&
                    vertices.Layouts.SequenceEqual(layouts);
            }
            if (ResourcePool.Instance.Recycle(match, key, life, out var resource))
            {
                return (VertexArray)resource;
            }
            var newVertices = new VertexArray(vertexCount, indices, layouts);
            ResourcePool.Instance.Add(key, newVertices, life, recycle);
            return newVertices;
        }
    }
}
