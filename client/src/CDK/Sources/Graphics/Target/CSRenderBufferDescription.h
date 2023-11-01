#ifndef __CDK__CSRenderBufferDescription__
#define __CDK__CSRenderBufferDescription__

#include "CSGEnums.h"

struct CSRenderBufferDescription {
public:
    ushort width = 0;
    ushort height = 0;
    CSRawFormat format = CSRawFormat::None;
    byte samples = 1;

    void validate() const;

    uint hash() const;

    bool operator ==(const CSRenderBufferDescription& other) const;
    inline bool operator !=(const CSRenderBufferDescription& other) const {
        return !(*this == other);
    }
};

#endif
