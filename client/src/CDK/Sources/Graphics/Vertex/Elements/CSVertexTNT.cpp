#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexTNT::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(4,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 32, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 32, 12, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 32, 20, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTangent, 3, CSVertexAttribTypeHalfFloat, false, 32, 26, 0, true));

uint CSVertexTNT::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(texCoord);
    hash.combine(normal);
    hash.combine(tangent);
    return hash;
}
