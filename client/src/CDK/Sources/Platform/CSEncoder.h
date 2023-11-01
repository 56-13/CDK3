#ifdef CDK_IMPL

#ifndef __CDK__CSEncoder__
#define __CDK__CSEncoder__

#include "CSString.h"

class CSEncoder {
public:
    static string encode(const void* bytes, int length, CSStringEncoding encoding);
    static void* decode(const char* str, int& length, CSStringEncoding encoding);
};

#endif

#endif
