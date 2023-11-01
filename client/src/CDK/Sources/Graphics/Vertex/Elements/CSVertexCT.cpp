#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexCT::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(3,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 28, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 28, 12, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 28, 20, 0, true));

uint CSVertexCT::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    hash.combine(texCoord);
    return hash;
}
