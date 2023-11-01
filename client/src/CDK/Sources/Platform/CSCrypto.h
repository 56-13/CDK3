#ifdef CDK_IMPL

#ifndef __CDK__CSCrypto__
#define __CDK__CSCrypto__

#include "CSString.h"

class CSCrypto {
public:
    static const void* encrypt(const void* data, int& length, const char* key);
    static const void* decrypt(const void* data, int& length, const char* key);
    static string sha1(const void* data, int length);
};

#endif

#endif
