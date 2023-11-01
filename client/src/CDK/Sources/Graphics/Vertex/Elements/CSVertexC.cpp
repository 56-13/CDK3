#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexC::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(2,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 20, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 20, 12, 0, true));

uint CSVertexC::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    return hash;
}
