#ifndef __CDK__CSRandom__
#define __CDK__CSRandom__

#include "CSFixed.h"

class CSRandom {
private:
    int64 _seed;
public:
    inline CSRandom() : _seed(rand()) {}
    inline CSRandom(int64 seed) : _seed(seed) {}
    
    inline void setSeed(int64 seed) {
        _seed = seed;
    }
    inline int64 seed() const {
        return _seed;
    }
    int nextInt();
    int64 nextLong();
    int nextInt(int min, int max);
    int64 nextLong(int64 min, int64 max);
    float nextFloat();
    float nextFloat(float min, float max);
    fixed nextFixed();
    fixed nextFixed(fixed min, fixed max);

    static float toFloat(int random);
    static float toFloat(int random, float min, float max);
    static float toFloatSequenced(int random, int seq);
    static float toFloatSequenced(int random, int seq0, int seq1, float rate);
};

static inline void randSeed(int seed) {
    srand(seed);
}
static inline int randInt() {
    return rand();
}
int64 randLong();
int randInt(int min, int max);
int64 randLong(int64 min, int64 max);
float randFloat();
float randFloat(float min, float max);
fixed randFixed();
fixed randFixed(fixed min, fixed max);

#endif
