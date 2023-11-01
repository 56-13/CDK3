#define CDK_IMPL

#include "CSFVertex.h"

#include "CSEntry.h"

uint CSFVertex::hash() const {
    CSHash hash;
    hash.combine(position);
    hash.combine(color);
    hash.combine(texCoord);
    hash.combine(normal);
    hash.combine(tangent);
    return hash;
}