#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexN::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(2,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 18, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 18, 12, 0, true));

uint CSVertexN::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(normal);
    return hash;
}
