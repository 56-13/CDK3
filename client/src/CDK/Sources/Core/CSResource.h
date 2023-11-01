#ifndef __CDK__CSResource__
#define __CDK__CSResource__

#include "CSObject.h"

enum CSResourceType : byte {
    CSResourceTypeTexture,
    CSResourceTypeImage,
    CSResourceTypeMesh,
    CSResourceTypeMaterial,
    CSResourceTypeGBuffer,
    CSResourceTypeGBufferSlice,
    CSResourceTypeVertexArray,
    CSResourceTypeRenderTarget,
    CSResourceTypeRenderBuffer,
    CSResourceTypeShader,
    CSResourceTypeProgram,
    CSResourceTypeString,
    CSResourceTypeLocaleString,
    CSResourceTypeAnimation,
    CSResourceTypeGround,
    CSResourceTypeTerrain,
    CSResourceTypeCount
};

class CSResource : public CSObject {
protected:
    CSResource() = default;
public:
    virtual ~CSResource() = default;

    virtual CSResourceType resourceType() const = 0;
    virtual int resourceCost() const = 0;
};

#endif
