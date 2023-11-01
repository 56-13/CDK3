#ifndef __CDK__CSVertexC__
#define __CDK__CSVertexC__

#include "CSVertexLayout.h"

#include "CSHalf4.h"

#pragma pack(push, 2)
struct CSVertexC {
    CSVector3 position;
    CSHalf4 color;

    static const CSArray<CSVertexLayout>* SingleBufferVertexLayouts;

    inline CSVertexC(const CSVector3& position, const CSHalf4& color) :
        position(position),
        color(color) 
    {
    }

    uint hash() const;

    inline bool operator ==(const CSVertexC& other) const {
        return memcmp(this, &other, sizeof(CSVertexC)) == 0;
    }
    inline bool operator !=(const CSVertexC& other) const {
        return !(*this == other);
    }
};
#pragma pack(pop)

#endif