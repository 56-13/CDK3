#ifndef CSAnimationLoop_h
#define CSAnimationLoop_h

#include "CSTypes.h"

class CSBuffer;

struct CSAnimationLoop {
public:
    ushort count = 0;
    bool finish = false;
    bool roundTrip = false;
public:
    CSAnimationLoop() = default;
    CSAnimationLoop(int count, bool finish, bool roundTrip);
    explicit CSAnimationLoop(CSBuffer* buffer);
    
    float getProgress(float progress, int* random0, int* random1) const;
    inline float getProgress(float progress) const {
        return getProgress(progress, NULL, NULL);
    }
    void getState(float progress, bool& remaining, bool& alive) const;
};

#endif
