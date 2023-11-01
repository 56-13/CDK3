#define CDK_IMPL

#include "CSTextureDescription.h"

#include "CSMath.h"

#include "CSEntry.h"

CSTextureDescription::CSTextureDescription() {
    memset(this, 0, sizeof(CSTextureDescription));

    samples = 1;
    mipmapCount = 1;
}

void CSTextureDescription::validate() const {
    CSAssert(width > 0 && height > 0 && samples >= 1 && mipmapCount >= 1);

#ifdef CS_ASSERT_DEBUG
    switch (target) {
        case CSTextureTarget2D:
        case CSTextureTargetCubeMap:
        case CSTextureTarget2DMultisample:
            CSAssert(depth == 0);
            break;
        case CSTextureTarget3D:
            CSAssert(depth > 0);
            break;
        default:
            CSAssert(false, "invalid format");
            break;
    }
#endif

    if (samples > 1) {
        CSAssert(target == CSTextureTarget2D || target == CSTextureTarget2DMultisample);
        CSAssert(wrapS == CSTextureWrapRepeat);
        CSAssert(wrapR == CSTextureWrapRepeat);
        CSAssert(wrapT == CSTextureWrapRepeat);
        CSAssert(minFilter == CSTextureMinFilterNearest);
        CSAssert(magFilter == CSTextureMagFilterNearest);
        CSAssert(mipmapCount == 1);
        if (target == CSTextureTarget2D) const_cast<CSTextureDescription*>(this)->target = CSTextureTarget2DMultisample;
    }
    else {
        CSAssert(target != CSTextureTarget2DMultisample);
    }
    
    if (mipmapCount > 1) const_cast<CSTextureDescription*>(this)->mipmapCount = CSMath::min((int)mipmapCount, maxMipmapCount());
}

int CSTextureDescription::maxMipmapCount() const {
    int d = CSMath::max(width, height);
    if (target == CSTextureTarget3D) d = CSMath::max(d, (int)depth);
    int c = 1;
    while (d > 0) {
        d >>= 1;
        c++;
    }
    return c;
}

uint CSTextureDescription::hash() const {
    CSHash hash;
    hash.combine(width);
    hash.combine(height);
    hash.combine(depth);
    hash.combine(samples);
    hash.combine(mipmapCount);
    hash.combine(format);
    hash.combine(target);
    hash.combine(wrapS);
    hash.combine(wrapT);
    hash.combine(wrapR);
    hash.combine(minFilter);
    hash.combine(magFilter);
    hash.combine(borderColor);
    return hash;
}
