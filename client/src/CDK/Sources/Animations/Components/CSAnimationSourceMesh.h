#ifndef CSAnimationSourceMesh_h
#define CSAnimationSourceMesh_h

#include "CSAnimationSource.h"

#include "CSMeshBuilder.h"

class CSBuffer;

class CSAnimationSourceMesh : public CSAnimationSource {
public:
    CSPtr<CSMeshBuilder> builder;
    CSPtr<const CSArray<string>> bones;

    CSAnimationSourceMesh() = default;
    explicit CSAnimationSourceMesh(CSBuffer* buffer);
    CSAnimationSourceMesh(const CSAnimationSourceMesh* other);

    static CSAnimationSourceMesh* source() {
        return autorelease(new CSAnimationSourceMesh());
    }
    static CSAnimationSourceMesh* sourceWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSAnimationSourceMesh(buffer));
    }
    static CSAnimationSourceMesh* sourceWithSource(const CSAnimationSourceMesh* other) {
        return autorelease(new CSAnimationSourceMesh(other));
    }

    inline Type type() const override {
        return TypeMesh;
    }
    int resourceCost() const override;
    void preload() const override;
};

#endif