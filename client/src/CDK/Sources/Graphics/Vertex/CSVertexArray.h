#ifndef __CDK__CSVertexArray__
#define __CDK__CSVertexArray__

#include "CSGBuffer.h"

#include "CSVertexLayout.h"

class CSGraphicsApi;

class CSVertexArray : public CSGraphicsResource {
private:
    int _object = 0;
    byte _vertexBufferCount;
    bool _ownIndexBuffer;
    CSGBuffer* _vertexBuffers[16] = {};
    CSGBuffer* _indexBuffer = NULL;
    CSArray<CSVertexLayout> _layouts;
    bool _attribEnabled[16] = {};
public:
    CSVertexArray(int vertexBufferCount, bool indexBuffer, const CSVertexLayout* layouts = NULL, int layoutCount = 0);
    CSVertexArray(int vertexBufferCount, bool indexBuffer, const CSArray<CSVertexLayout>* layouts);
private:
    ~CSVertexArray();
public:
    static inline CSVertexArray* vertexArray(int vertexBufferCount, bool indexBuffer, const CSVertexLayout* layouts = NULL, int layoutCount = 0) {
        return autorelease(new CSVertexArray(vertexBufferCount, indexBuffer, layouts, layoutCount));
    }
    static inline CSVertexArray* vertexArray(int vertexBufferCount, bool indexBuffer, const CSArray<CSVertexLayout>* layouts) {
        return autorelease(new CSVertexArray(vertexBufferCount, indexBuffer, layouts));
    }

    inline CSResourceType resourceType() const override {
        return CSResourceTypeVertexArray;
    }
    int resourceCost() const override;

    inline int object() const {
        return _object;
    }
    inline CSGBuffer* vertexBuffer(int i) {
        CSAssert(i >= 0 && i < _vertexBufferCount);
        return _vertexBuffers[i];
    }
    inline const CSGBuffer* vertexBuffer(int i) const {
        CSAssert(i >= 0 && i < _vertexBufferCount);
        return _vertexBuffers[i];
    }
    inline int vertexBufferCount() const {
        return _vertexBufferCount;
    }
    inline CSGBuffer* indexBuffer() {
        return _indexBuffer;
    }
    inline const CSGBuffer* indexBuffer() const {
        return _indexBuffer;
    }
    inline bool ownIndexBuffer() const {
        return _ownIndexBuffer;
    }
    inline const CSArray<CSVertexLayout>* layouts() const {
        return &_layouts;
    }
    
    bool batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const override;

    void bind(CSGraphicsApi* api);
    void unbind(CSGraphicsApi* api);
    void attachLayout(CSGraphicsApi* api, const CSVertexLayout& layout);
    void detachLayout(CSGraphicsApi* api, int attrib);
    bool hasAttrib(CSGraphicsApi* api, int attrib) const;
    void attachIndex(CSGraphicsApi* api, CSGBuffer* indices);
    void setAttribEnabled(CSGraphicsApi* api, int attrib, bool enabled);
    bool attribEnabled(CSGraphicsApi* api, int attrib) const;
private:
    CSDrawElementsType elementType() const;
public:
    void drawArrays(CSGraphicsApi* api, CSPrimitiveMode mode, int vertexOffset, int vertexCount) const;
    void drawElements(CSGraphicsApi* api, CSPrimitiveMode mode, int indexOffset, int indexCount) const;
    void drawElements(CSGraphicsApi* api, CSPrimitiveMode mode) const;
    void drawElementsInstanced(CSGraphicsApi* api, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCount) const;
    void drawElementsInstanced(CSGraphicsApi* api, CSPrimitiveMode mode, int instanceCount) const;
};

#endif