#ifndef __CDK__CSStaticRenderCommand__
#define __CDK__CSStaticRenderCommand__

#include "CSRenderCommand.h"

#include "CSRenderState.h"
#include "CSVertex.h"
#include "CSVertexArray.h"
#include "CSVertexArrayInstance.h"

class CSGraphics;

class CSStaticRenderCommand : public CSRenderCommand {
public:
    CSRenderState state;
    CSMatrix world;
    CSColor color;
private:
    struct Instance {
        CSMatrix model;
        CSHalf4 color;
        int boneOffset;

        Instance(const CSMatrix& model, const CSHalf4& color, int boneOffset);

        uint hash() const;
        inline bool operator ==(const Instance& other) const {
            return memcmp(this, &other, sizeof(Instance)) == 0;
        }
        inline bool operator !=(const Instance& other) const {
            return !(*this == other);
        }
    };
    bool _renderOrder;
    CSVertexArray* _vertices;
    const CSGBuffer* _boneBuffer;
    int _boneOffset;
    CSPrimitiveMode _mode;
    int _indexOffset;
    int _indexCount;
    const CSABoundingBox* _aabb;
    CSGBufferData<Instance>* _instanceData;
    CSArray<CSRect>* _bounds;
    CSGBuffer* _instanceBuffer;
public:
    CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, CSPrimitiveMode mode, int instanceCapacity = 1, const CSABoundingBox* aabb = NULL);
    CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int instanceCapacity = 1, const CSABoundingBox* aabb = NULL);
    CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity = 1, const CSABoundingBox* aabb = NULL);
    CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity = 1, const CSABoundingBox* aabb = NULL);
private:
    ~CSStaticRenderCommand();
public:
    void addInstance(const CSVertexArrayInstance& i);

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
private:
    void attachVertexInstances(CSGraphicsApi* api);
    void detachVertexInstances(CSGraphicsApi* api);
};

#endif
