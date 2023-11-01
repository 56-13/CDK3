#define CDK_IMPL

#include "CSBytes.h"

#include "CSData.h"

int readLength(const byte*& bytes) {
    int v = 0;
    int current;
    
    do {
        current = *bytes;
        
        bytes++;
        
        v = (v << 7) | (current & 0x7f);
    } while (current & 0x80);
    
    return v;
}

string readString(const byte*& bytes) {
    int len = readLength(bytes);
    
    string str;
    
    if (len) {
        str = string::encode(bytes, len, CSStringEncodingUTF8);
        bytes += len;
    }
    
    return str;
}

CSLocaleString* readLocaleString(const byte*& bytes) {
    return CSLocaleString::localeStringWithBytes(bytes);
}

void writeLength(byte*& bytes, int length) {
    for (int i = 21; i > 0; i -= 7) {
        int current = (length >> i) & 0x7f;
        
        if (current) {
            writeByte(bytes, current | 0x80);
        }
    }
    writeByte(bytes, length & 0x7f);
}

void writeString(byte*& bytes, const char* str) {
    int len = strlen(str);
    writeLength(bytes, len);
    memcpy(bytes, str, len);
    bytes += len;
}

void writeString(byte*& bytes, const string& str) {
    if (!str) writeLength(bytes, 0);
    else writeString(bytes, str.cstring());
}

