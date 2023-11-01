#ifndef __CDK__CSRenderTargetDescription__
#define __CDK__CSRenderTargetDescription__

#include "CSGEnums.h"

#include "CSArray.h"
#include "CSColor.h"

struct CSRenderTargetDescription {
public:
    struct Attachment {
        CSFramebufferAttachment attachment;
        byte samples;
        CSRawFormat format;
        bool texture;
        byte textureLayer;
        CSTextureTarget textureTarget;
        CSTextureWrapMode textureWrapS;
        CSTextureWrapMode textureWrapT;
        CSTextureWrapMode textureWrapR;
        CSTextureMinFilter textureMinFilter;
        CSTextureMagFilter textureMagFilter;
        CSColor textureBorderColor;

        Attachment();

        uint hash() const;

        inline bool operator ==(const Attachment& other) const {
            return memcmp(this, &other, sizeof(Attachment)) == 0;
        }
        inline bool operator !=(const Attachment& other) const {
            return !(*this == other);
        }
    };
    ushort width = 0;
    ushort height = 0;
    CSArray<Attachment> attachments;

    CSRenderTargetDescription() = default;
    CSRenderTargetDescription(const CSRenderTargetDescription& other);

    CSRenderTargetDescription& operator =(const CSRenderTargetDescription& other);

    void validate() const;

    uint hash() const;

    bool operator ==(const CSRenderTargetDescription& other) const;
    inline bool operator !=(const CSRenderTargetDescription& other) const {
        return !(*this == other);
    }
};

#endif
