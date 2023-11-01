#ifndef __CDK__CSBytes__
#define __CDK__CSBytes__

#include "CSHalf.h"
#include "CSFixed.h"

#include "CSLocaleString.h"

#pragma pack(push, 1)
typedef struct {
	short data;
} PackedShort;

typedef struct {
	int data;
} PackedInt;

typedef struct {
	int64 data;
} PackedLong;

typedef struct {
	float data;
} PackedFloat;

typedef struct {
	double data;
} PackedDouble;
#pragma pack(pop)

static inline int bytesToShort(const byte* bytes) {
    return ((const PackedShort*)bytes)->data;
}

static inline int bytesToInt(const byte* bytes) {
    return ((const PackedInt*)bytes)->data;
}

static inline int64 bytesToLong(const byte* bytes) {
    return ((const PackedLong*)bytes)->data;
}

static inline half bytesToHalf(const byte* bytes) {
    return half(half::Raw, bytesToShort(bytes));
}

static inline float bytesToFloat(const byte* bytes) {
    return ((const PackedFloat*)bytes)->data;
}

static inline double bytesToDouble(const byte* bytes) {
    return ((const PackedDouble*)bytes)->data;
}

static inline fixed bytesToFixed(const byte* bytes) {
    return fixed(fixed::Raw, bytesToLong(bytes));
}

static inline int readByte(const byte*& bytes) {
    int v = *(const sbyte*)bytes;
    bytes++;
    return v;
}

static inline bool readBoolean(const byte*& bytes) {
    bool v = *bytes != 0;
    bytes++;
    return v;
}

static inline int readShort(const byte*& bytes) {
    int v = bytesToShort(bytes);
    bytes += 2;
    return v;
}

static inline int readInt(const byte*& bytes) {
    int v = bytesToInt(bytes);
    bytes += 4;
    return v;
}

static inline half readHalf(const byte*& bytes) {
    half v = bytesToHalf(bytes);
    bytes += 2;
    return v;
}

static inline float readFloat(const byte*& bytes) {
    float v = bytesToFloat(bytes);
    bytes += 4;
    return v;
}

static inline double readDouble(const byte*& bytes) {
    double v = bytesToDouble(bytes);
    bytes += 8;
    return v;
}

static inline fixed readFixed(const byte*& bytes) {
    fixed v = bytesToFixed(bytes);
    bytes += 8;
    return v;
}

int readLength(const byte*& bytes);
string readString(const byte*& bytes);
CSLocaleString* readLocaleString(const byte*& bytes);

static inline void shortToBytes(byte* bytes, int value) {
    ((PackedShort*)bytes)->data = value;
}

static inline void intToBytes(byte* bytes, int value) {
    ((PackedInt*)bytes)->data = value;
}

static inline void longToBytes(byte* bytes, int64 value) {
    ((PackedLong*)bytes)->data = value;
}

static inline void halfToBytes(byte* bytes, half value) {
    shortToBytes(bytes, value.raw);
}

static inline void floatToBytes(byte* bytes, float value) {
    ((PackedFloat*)bytes)->data = value;
}

static inline void doubleToBytes(byte* bytes, double value) {
    ((PackedDouble*)bytes)->data = value;
}

static inline void fixedToBytes(byte* bytes, fixed value) {
    longToBytes(bytes, value.raw);
}

static inline void writeByte(byte*& bytes, int value) {
    *(bytes++) = value;
}

static inline void writeBoolean(byte*& bytes, bool value) {
    *(bytes++) = value;
}

static inline void writeShort(byte*& bytes, int value) {
    shortToBytes(bytes, value);
    bytes += 2;
}

static inline void writeInt(byte*& bytes, int value) {
    intToBytes(bytes, value);
    bytes += 4;
}

static inline void writeLong(byte*& bytes, int64 value) {
    longToBytes(bytes, value);
    bytes += 8;
}

static inline void writeHalf(byte*& bytes, half value) {
    halfToBytes(bytes, value);
    bytes += 2;
}

static inline void writeFloat(byte*& bytes, float value) {
    floatToBytes(bytes, value);
    bytes += 4;
}

static inline void writeDouble(byte*& bytes, double value) {
    doubleToBytes(bytes, value);
    bytes += 8;
}

static inline void writeFixed(byte*& bytes, fixed value) {
    fixedToBytes(bytes, value);
    bytes += 8;
}

void writeLength(byte*& bytes, int length);
void writeString(byte*& bytes, const char* str);
void writeString(byte*& bytes, const string& str);

#endif
