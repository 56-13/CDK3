#ifndef __CDK__CSTextureDescription__
#define __CDK__CSTextureDescription__

#include "CSGEnums.h"

#include "CSColor.h"

struct CSTextureDescription {
    ushort width;
    ushort height;
    ushort depth;
    byte samples;
    byte mipmapCount;
    CSRawFormat format;
    CSTextureTarget target;
    CSTextureWrapMode wrapS;
    CSTextureWrapMode wrapT;
    CSTextureWrapMode wrapR;
    CSTextureMinFilter minFilter;
    CSTextureMagFilter magFilter;
    CSColor borderColor;

    CSTextureDescription();

    void validate() const;
    
    int maxMipmapCount() const;

    uint hash() const;

    inline bool operator ==(const CSTextureDescription& other) const {
        return memcmp(this, &other, sizeof(CSTextureDescription)) == 0;
    }
    inline bool operator !=(const CSTextureDescription& other) const {
        return !(*this == other);
    }
};

#endif
