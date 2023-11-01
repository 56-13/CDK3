#define CDK_IMPL

#include "icu_compat.h"

#ifndef CDK_IOS

#include <unicode/ushape.h>

int32_t ucompat_shapeArabic(const UChar *source, int32_t sourceLength,
                         UChar *dest, int32_t destCapacity,
                         uint32_t options,
                         UErrorCode *pErrorCode) {
    
    return u_shapeArabic(source, sourceLength, dest, destCapacity, options, pErrorCode);
}

UBiDiDirection ucompat_getBaseDirection(const UChar* str, int32_t len) {
    return ubidi_getBaseDirection(str, len);
}

#endif
