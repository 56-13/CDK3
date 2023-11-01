#define CDK_IMPL

#include "CSRandom.h"

#include "CSMath.h"

int CSRandom::nextInt() {
    return nextLong();
}

int64 CSRandom::nextLong() {
    return _seed = (_seed * 214013L + 2531011L) >> 16;
}

int CSRandom::nextInt(int min, int max) {
    return min + (abs(nextInt()) % (max - min + 1));
}

int64 CSRandom::nextLong(int64 min, int64 max) {
    return min + (llabs(nextLong()) % (max - min + 1));
}

float CSRandom::nextFloat() {
    union {
        int iv;
        float fv;
    } out;
    out.iv = nextInt();
    return out.fv;
}

float CSRandom::nextFloat(float min, float max) {
    return min + (double)abs(nextInt()) / 0x7FFFFFFF * (max - min);
}

fixed CSRandom::nextFixed() {
    return fixed(fixed::Raw, nextLong() & 0xFFFFFFFFFFFF);
}

fixed CSRandom::nextFixed(fixed min, fixed max) {
    return fixed(fixed::Raw, nextLong(min.raw, max.raw));
}

float CSRandom::toFloat(int random) {
    return (float)((double)(uint)random / 0xFFFFFFFF);
}

float CSRandom::toFloat(int random, float min, float max) {
    return min + toFloat(random) * (max - min);
}

float CSRandom::toFloatSequenced(int random, int seq) {
    int s = CSMath::abs(seq) % 32;
    uint r = random;
    r = (r >> s) | (r << (32 - s));
    return (double)r / 0xFFFFFFFF;
}

float CSRandom::toFloatSequenced(int random, int seq0, int seq1, float rate) {
    return CSMath::lerp(toFloatSequenced(random, seq0), toFloatSequenced(random, seq1), rate);
}

int64 randLong() {
    return (int64)rand() << 32 | rand();
}

int randInt(int min, int max) {
    return min + (abs(rand()) % (max - min + 1));
}

int64 randLong(int64 min, int64 max) {
    return min + (llabs(randLong()) % (max - min + 1));
}

float randFloat() {
    union {
        int iv;
        float fv;
    } out;
    
    out.iv = rand();
    return out.fv;
}

float randFloat(float min, float max) {
    return min + (double)rand() / RAND_MAX * (max - min);
}

fixed randFixed() {
    return fixed(fixed::Raw, randLong() & 0xFFFFFFFFFFFF);
}

fixed randFixed(fixed min, fixed max) {
    return fixed(fixed::Raw, randLong(min.raw, max.raw));
}
