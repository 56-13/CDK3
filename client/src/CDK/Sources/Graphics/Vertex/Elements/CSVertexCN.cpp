#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexCN::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(3,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 26, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 26, 12, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 26, 20, 0, true));

uint CSVertexCN::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    hash.combine(normal);
    return hash;
}
