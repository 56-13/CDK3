#ifndef __CDK__CSTextureScratch__
#define __CDK__CSTextureScratch__

#include "CSTexture.h"

class CSTextureScratch : public CSObject {
private:
    CSTexture* _texture;
    void* _raw;
    ushort _width;
    ushort _height;
    byte _bpp;
    byte _level;
    mutable ushort _updateLeft;
    mutable ushort _updateRight;
    mutable ushort _updateTop;
    mutable ushort _updateBottom;
private:
    CSTextureScratch() = default;
    ~CSTextureScratch();
public:
    static CSTextureScratch* create(const CSTextureDescription& desc);
    static CSTextureScratch* create(CSTexture* texture, int level);

    static inline CSTextureScratch* scratch(const CSTextureDescription& desc) {
        return autorelease(create(desc));
    }
    static inline CSTextureScratch* scratch(CSTexture* texture, int level) {
        return autorelease(create(texture, level));
    }

    inline int bpp() const {
        return _bpp;
    }
    void* get(int x, int y) const;
    void set(int x, int y, void* data);
    void upload() const;

    inline CSTexture* texture() {
        upload();
        return _texture;
    }
    inline const CSTexture* texture() const {
        upload();
        return _texture;
    }
};

#endif
