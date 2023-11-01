#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexCTN::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(4,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 34, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 34, 12, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 34, 20, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 34, 28, 0, true));

uint CSVertexCTN::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    hash.combine(texCoord);
    hash.combine(normal);
    return hash;
}
