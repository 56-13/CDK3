#ifndef __CDK__CSFVertex__
#define __CDK__CSFVertex__

#include "CSColor.h"

struct CSFVertex {
    CSVector3 position;
    CSColor color;
    CSVector2 texCoord;
    CSVector3 normal;
    CSVector3 tangent;

    inline CSFVertex(const CSVector3& position, const CSColor& color, const CSVector2& texCoord, const CSVector3& normal, const CSVector3& tangent) :
        position(position),
        color(color),
        texCoord(texCoord),
        normal(normal),
        tangent(tangent) 
    {
    }
    inline CSFVertex(const CSVector3& position)
        : CSFVertex(position, CSColor::White, CSVector2::Zero, CSVector3::UnitZ, CSVector3::UnitX) {
    }
    inline CSFVertex(const CSVector3& position, const CSColor& color)
        : CSFVertex(position, color, CSVector2::Zero, CSVector3::UnitZ, CSVector3::UnitX) {
    }
    inline CSFVertex(const CSVector3& position, const CSVector2& texCoord)
        : CSFVertex(position, CSColor::White, texCoord, CSVector3::UnitZ, CSVector3::UnitX) {
    }
    inline CSFVertex(const CSVector3& position, const CSColor& color, const CSVector2& texCoord)
        : CSFVertex(position, color, texCoord, CSVector3::UnitZ, CSVector3::UnitX) {
    }
    inline CSFVertex(const CSVector3& position, const CSVector3& normal, const CSVector3& tangent)
        : CSFVertex(position, CSColor::White, CSVector2::Zero, normal, tangent) {
    }
    inline CSFVertex(const CSVector3& position, const CSColor& color, const CSVector3& normal, const CSVector3& tangent)
        : CSFVertex(position, color, CSVector2::Zero, normal, tangent) {
    }
    inline CSFVertex(const CSVector3& position, const CSVector2& texCoord, const CSVector3& normal, const CSVector3& tangent)
        : CSFVertex(position, CSColor::White, texCoord, normal, tangent) {
    }

    uint hash() const;

    inline bool operator ==(const CSFVertex& other) const {
        return memcmp(this, &other, sizeof(CSFVertex)) == 0;
    }
    inline bool operator !=(const CSFVertex& other) const {
        return !(*this == other);
    }
};

#endif