#ifndef __CDK__CSGBufferData__
#define __CDK__CSGBufferData__

#include "CSArray.h"

template <typename T>
class CSGBufferData : public CSArray<T> {
public:
    CSGBufferData() = default;
    inline CSGBufferData(int capacity) : CSArray<T>(capacity) {}
protected:
    virtual ~CSGBufferData() = default;
public:
    static inline CSGBufferData* buffer() {
        return autorelease(new CSGBufferData());
    }
    static inline CSGBufferData* bufferWithCapacity(int capacity) {
        return autorelease(new CSGBufferData(capacity));
    }

    inline virtual int size() const {
        return sizeof(T);
    }
    inline virtual const void* pointer() const {
        return CSArray<T>::pointer();
    }
    inline uint hash() const override {
        return sequentialHash();
    }
    inline bool isEqual(const CSGBufferData* other) const {
        return count() == other->count() && memcmp(pointer(), other->pointer(), count() * sizeof(T)) == 0;
    }
    inline bool isEqual(const CSObject* object) const override {
        const CSGBufferData* other = dynamic_cast<const CSGBufferData*>(object);
        return other && isEqual(other);
    }
};

class CSVertexIndexData : public CSGBufferData<int> {
public:
    int vertexCapacity;

    inline CSVertexIndexData(int vertexCapacity) : vertexCapacity(vertexCapacity) {}
    inline CSVertexIndexData(int vertexCapacity, int indexCapacity) : CSGBufferData<int>(indexCapacity) {}
private:
    ~CSVertexIndexData() = default;
public:
    static inline CSVertexIndexData* buffer(int vertexCapacity) {
        return autorelease(new CSVertexIndexData(vertexCapacity));
    }
    static inline CSVertexIndexData* bufferWithCapacity(int vertexCapacity, int indexCapacity) {
        return autorelease(new CSVertexIndexData(vertexCapacity, indexCapacity));
    }

    int size() const override;
    const void* pointer() const override;
};

#endif
