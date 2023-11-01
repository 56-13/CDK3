using System.Numerics;
using System.Collections.Generic;

namespace CDK.Drawing
{
    public interface IRenderer
    {
        bool Visible(in RenderState state);
        void Validate(ref RenderState state);
        bool Clip(in RenderState state, IEnumerable<Vector3> wps, out Rectangle bounds);
        void Setup(GraphicsApi api, in RenderState state, PrimitiveMode mode, Buffer boneBuffer, bool usingInstance, bool usingVertexColor);
    }
}
