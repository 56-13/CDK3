#ifdef CDK_IMPL

#ifndef icu_compat_h
#define icu_compat_h

#include <unicode/utypes.h>
#include <unicode/ubidi.h>

int32_t ucompat_shapeArabic(const UChar *source, int32_t sourceLength,
                            UChar *dest, int32_t destCapacity,
                            uint32_t options,
                            UErrorCode *pErrorCode);

UBiDiDirection ucompat_getBaseDirection(const UChar* str, int32_t len);

#endif

#endif
