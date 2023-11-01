#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertex::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(5,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 40, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 40, 12, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 40, 20, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 40, 28, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTangent, 3, CSVertexAttribTypeHalfFloat, false, 40, 34, 0, true));


uint CSVertex::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    hash.combine(texCoord);
    hash.combine(normal);
    hash.combine(tangent);
    return hash;
}
