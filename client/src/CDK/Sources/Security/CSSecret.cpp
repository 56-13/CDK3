#define CDK_IMPL

#include "CSSecret.h"

#include "CSLog.h"
#include "CSRandom.h"
#include "CSThread.h"

CSSecret CSSecret::_instance;

CSSecret::CSSecret() {
    for (int i = 0; i < CSSecretCapacity; i++) table[i] = randLong();
    valueMask = randLong();
    checkMask = randLong();
}

void CSSecret::addValue(CSSecretValueBase* value) {
#ifdef CS_SECRET_THREAD_SAFE
    _lock.assertOnActive();
#endif
    _values.addObject(value);
}

void CSSecret::removeValue(CSSecretValueBase* value) {
#ifdef CS_SECRET_THREAD_SAFE
    _lock.assertOnActive();
#endif
    _values.removeObjectIdenticalTo(value);
}

void CSSecret::reset(bool withTable) {
    lock();
    
    uint64 newValueMask = randLong();
    uint64 newCheckMask = randLong();
    
    if (withTable) {
        uint64 newTable[CSSecretCapacity];
        for (int i = 0; i < CSSecretCapacity; i++) {
            newTable[i] = randLong();
        }
        foreach (CSSecretValueBase*, value, &_values) {
            value->reset(newTable, newValueMask, newCheckMask);
        }
        memcpy(table, newTable, CSSecretCapacity * sizeof(uint64));
    }
    else {
        foreach (CSSecretValueBase*, value, &_values) {
            value->reset(NULL, newValueMask, newCheckMask);
        }
    }
    valueMask = newValueMask;
    checkMask = newCheckMask;
    
    unlock();
}
