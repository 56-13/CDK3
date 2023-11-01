#ifndef CSAnimationSourceImage_h
#define CSAnimationSourceImage_h

#include "CSAnimationSource.h"

#include "CSImage.h"
#include "CSMaterialSource.h"
#include "CSAnimationLoop.h"

class CSBuffer;

class CSAnimationSourceImage : public CSAnimationSource {
public:
    CSPtr<const CSArray<ushort>> rootIndices;
    CSPtr<const CSArray<ushort>> subIndices;
    CSPtr<CSMaterialSource> material;
    float duration = 0;
    CSAnimationLoop loop;

    CSAnimationSourceImage() = default;
    explicit CSAnimationSourceImage(CSBuffer* buffer);
    CSAnimationSourceImage(const CSAnimationSourceImage* other);

    static CSAnimationSourceImage* source() {
        return autorelease(new CSAnimationSourceImage());
    }
    static CSAnimationSourceImage* sourceWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSAnimationSourceImage(buffer));
    }
    static CSAnimationSourceImage* sourceWithSource(const CSAnimationSourceImage* other) {
        return autorelease(new CSAnimationSourceImage(other));
    }

    inline Type type() const override {
        return TypeImage;
    }
    int resourceCost() const override;
    void preload() const override;

    const CSImage* image(float progress) const;
};

#endif