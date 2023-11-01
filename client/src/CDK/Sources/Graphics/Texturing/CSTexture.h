#ifndef __CDK__CSTexture__
#define __CDK__CSTexture__

#include "CSGraphicsResource.h"

#include "CSTextureDescription.h"

class CSBuffer;
class CSGraphicsApi;

class CSTexture : public CSGraphicsResource {
private:
    CSTextureDescription _description;
    int _object = 0;
    bool _allocated = false;
public:
    CSTexture(const CSTextureDescription& desc, bool allocate = true);
    CSTexture(const CSTextureDescription& desc, const void* raw, int rawLength);
    CSTexture(CSBuffer* buffer);

    static CSTexture* createWithBitmap(CSTextureDescription desc, const void* data, int dataLength);
    static CSTexture* createWithContentOfFile(const CSTextureDescription& desc, const char* path);
private:
    ~CSTexture();
private:
    void load(CSGraphicsApi* api, CSBuffer* buffer);
public:
    static inline CSTexture* texture(const CSTextureDescription& desc, bool allocate = true) {
        return autorelease(new CSTexture(desc, allocate));
    }
    static inline CSTexture* texture(const CSTextureDescription& desc, const void* raw, int rawLength) {
        return autorelease(new CSTexture(desc, raw, rawLength));
    }
    static inline CSTexture* textureWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSTexture(buffer));
    }
    static inline CSTexture* textureWithBitmap(const CSTextureDescription& desc, const void* data, int dataLength) {
        return autorelease(createWithBitmap(desc, data, dataLength));
    }
    static inline CSTexture* textureWithContentOfFile(const CSTextureDescription& desc, const char* path) {
        return autorelease(createWithContentOfFile(desc, path));
    }

    inline CSResourceType resourceType() const override {
        return CSResourceTypeTexture;
    }
    int resourceCost() const override;

    inline int object() const {
        return _object;
    }
    inline const CSTextureDescription& description() const {
        return _description;
    }
    inline CSTextureTarget target() const {
        return _description.target;
    }
    inline int width() const {
        return _description.width;
    }
    inline int height() const {
        return _description.height;
    }
    inline int depth() const {
        return _description.depth;
    }
    inline CSRawFormat format() const {
        return _description.format;
    }
    inline int samples() const {
        return _description.samples;
    }
    inline bool allocated() const {
        return _allocated;
    }
private:
    int getCost2D() const;
    int getCost3D() const;
    void uploadFail();
    void upload(CSGraphicsApi* api, CSBuffer* buffer, CSTextureTarget target, int width, int height, int depth, CSRawFormat format, int level, int samples);
public:
    void upload(const void* raw, int rawLength, int level, int width, int height, int depth);
    bool uploadSub(const void* raw, int rawLength, int level, int x, int y, int z, int width, int height, int depth);
    void allocate();
    void resize(int width, int height);
    bool reload(CSTextureDescription desc);
};

#endif
