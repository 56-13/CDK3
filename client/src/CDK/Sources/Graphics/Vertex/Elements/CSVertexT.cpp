#define CDK_IMPL

#include "CSVertex.h"

#include "CSRenderState.h"

const CSArray<CSVertexLayout>* CSVertexT::SingleBufferVertexLayouts = new CSArray<CSVertexLayout>(2,
    CSVertexLayout(0, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 20, 0, 0, true),
    CSVertexLayout(0, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 20, 12, 0, true));

uint CSVertexT::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(texCoord);
    return hash;
}
