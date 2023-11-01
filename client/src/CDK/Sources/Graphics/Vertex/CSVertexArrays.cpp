#define CDK_IMPL

#include "CSVertexArrays.h"

#include "CSResourcePool.h"

CSVertexArray* CSVertexArrays::get(const CSObject* key, int life, bool recycle, int vertexBufferCount, bool indexBuffer, const CSVertexLayout* layouts, int layoutCount) {
    auto match = [vertexBufferCount, indexBuffer, layouts, layoutCount](const CSResource* candidate) {
        if (candidate->resourceType() == CSResourceTypeVertexArray) {
            const CSVertexArray* vertices = static_cast<const CSVertexArray*>(candidate);

            if (vertices->vertexBufferCount() == vertexBufferCount && vertices->ownIndexBuffer() == indexBuffer && vertices->layouts()->count() == layoutCount) {
                for (int i = 0; i < layoutCount; i++) {
                    if (vertices->layouts()->objectAtIndex(i) != layouts[i]) return false;
                }
                return true;
            }
        }
        return false;
    };

    CSResource* resource;
    if (CSResourcePool::sharedPool()->recycle(match, key, life, resource)) {
        return static_cast<CSVertexArray*>(resource);
    }
    CSVertexArray* newVertices = new CSVertexArray(vertexBufferCount, indexBuffer, layouts, layoutCount);
    CSResourcePool::sharedPool()->add(key, newVertices, life, recycle);
    newVertices->release();
    return newVertices;
}
