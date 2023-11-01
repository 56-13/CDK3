#ifndef __CDK__CSVertexT__
#define __CDK__CSVertexT__

#include "CSVertexLayout.h"

#include "CSVector3.h"

#pragma pack(push, 2)
struct CSVertexT {
    CSVector3 position;
    CSVector2 texCoord;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexT(const CSVector3& position, const CSVector2& texCoord) :
        position(position),
        texCoord(texCoord)
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexT& other) const {
        return memcmp(this, &other, sizeof(CSVertexT)) == 0;
    }
    inline bool operator !=(const CSVertexT& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif