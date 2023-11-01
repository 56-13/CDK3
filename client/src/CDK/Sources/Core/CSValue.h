#ifndef __CDK__CSValue__
#define __CDK__CSValue__

#include "CSObject.h"

template <typename T>
class CSValue : public CSObject {
private:
    T _value;
public:
    CSValue() = default;
    inline CSValue(const T& value) : _value(value) {}
private:
    ~CSValue() = default;
public:
    static inline CSValue<T>* value(const T& value) {
        return autorelease(new CSValue<T>(value));
    }
    
    inline operator T() const {
        return _value;
    }
    inline void setValue(const T& value) {
        _value = value;
    }
    inline const T& value() const {
        return _value;
    }
    inline uint hash() const override {
        return CSHash::value(_value);
    }
    inline bool isEqual(const CSValue<T>* other) const {
        return _value == other->_value;
    }
    inline bool isEqual(const CSObject* object) const override {
        const CSValue<T>* other = dynamic_cast<const CSValue<T>*>(object);
        return other && isEqual(other);
    }
};

typedef CSValue<sbyte> CSSByte;
typedef CSValue<byte> CSByte;
typedef CSValue<short> CSShort;
typedef CSValue<ushort> CSUShort;
typedef CSValue<int> CSInteger;
typedef CSValue<uint> CSUInteger;
typedef CSValue<int64> CSLong;
typedef CSValue<uint64> CSULong;
typedef CSValue<float> CSFloat;
typedef CSValue<double> CSDouble;
typedef CSValue<bool> CSBool;

#endif
