#ifndef __CDK__CSStreamRenderCommand__
#define __CDK__CSStreamRenderCommand__

#include "CSRenderCommand.h"

#include "CSRenderState.h"
#include "CSFVertex.h"
#include "CSVertexArray.h"

class CSGraphics;

class CSStreamRenderCommand : public CSRenderCommand {
public:
    CSRenderState state;
    CSMatrix world;
    CSColor color;
private:
    bool _renderOrder;
    CSPrimitiveMode _mode;
    CSGBufferData<CSFVertex>* _vertexData;
    CSVertexIndexData* _indexData;
    CSArray<CSRect>* _bounds;
    CSVertexArray* _vertices;
public:
    CSStreamRenderCommand(CSGraphics* graphics, CSPrimitiveMode mode);
    CSStreamRenderCommand(CSGraphics* graphics, CSPrimitiveMode mode, int vertexCapacity, int indexCapacity);
private:
    ~CSStreamRenderCommand();
public:
    inline int vertexCount() const {
        return _vertexData->count();
    }
    inline int indexCount() const {
        return _indexData->count();
    }
    void addVertex(CSFVertex vertex);
    void addIndex(int index);

    inline int layer() const override {
        return state.layer;
    }
    inline const CSRenderTarget* target() const override {
        return state.target;
    }
    inline const CSArray<CSRect>* bounds() const override {
        return _bounds;
    }
    bool submit() override;
    bool parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const override;
    bool findBatch(CSRenderCommand* command, CSRenderCommand*& candidate) const override;
    void batch(CSRenderCommand* command) override;
    void render(CSGraphicsApi* api, bool background, bool foreground) override;
};

#endif
