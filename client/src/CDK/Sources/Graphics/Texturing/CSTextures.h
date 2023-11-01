#ifndef __CDK__CSTextures__
#define __CDK__CSTextures__

#include "CSTexture.h"

class CSTextures {
public:
    static CSTexture* brdf();
    static CSTexture* get(const CSObject* key, int life, bool recycle, const CSTextureDescription& desc, bool allocate = true);
    static inline CSTexture* getTemporary(const CSTextureDescription& desc, bool allocate = true) {
        return get(NULL, 1, true, desc, allocate);
    }
};

#endif
