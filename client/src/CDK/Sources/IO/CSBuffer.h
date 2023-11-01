#ifndef __CDK__CSBuffer__
#define __CDK__CSBuffer__

#include "CSData.h"
#include "CSBytes.h"
#include "CSLocaleString.h"

class CSBuffer : public CSObject {
private:
    CSData* _data;
    int _position = 0;
    int _markedPosition = 0;
    int _markedLength = 0;
    bool _marked = false;
public:
    CSBuffer();
    CSBuffer(int capacity);
    CSBuffer(CSData* data);
    CSBuffer(void* data, int length, bool copy = true);
    ~CSBuffer();

    static CSBuffer* createWithContentOfFile(const char* path, bool compression = false);
    static CSBuffer* createWithContentOfFile(const char* path, int offset, int length, bool compression = false);
    
    static inline CSBuffer* buffer() {
        return autorelease(new CSBuffer());
    }
    static inline CSBuffer* bufferWithCapacity(int capacity) {
        return autorelease(new CSBuffer(capacity));
    }
    static inline CSBuffer* bufferWithData(CSData* data) {
        return autorelease(new CSBuffer(data));
    }
    static inline CSBuffer* bufferWithBytes(void* data, int length, bool copy = true) {
        return autorelease(new CSBuffer(data, length, copy));
    }
    static inline CSBuffer* bufferWithContentOfFile(const char* path, bool compression = false) {
        return autorelease(createWithContentOfFile(path, compression));
    }
    static inline CSBuffer* bufferWithContentOfFile(const char* path, int offset, int length, bool compression = false) {
        return autorelease(createWithContentOfFile(path, offset, length, compression));
    }
    
    CSData* remainedData() const;
    CSBuffer* remainedBuffer() const;
    
    void clear();
    void mark();
    void rewind();
    void shrink();
    void read(void* data, int size);
    CSData* readData(int length);
    int readByte();
    bool readBoolean();
    int readShort();
    int readInt();
    int64 readLong();
    half readHalf();
    float readFloat();
    fixed readFixed();
    double readDouble();
    int readLength();
    string readString();
    CSLocaleString* readLocaleString();
    
    template <typename V, int dimension = 1, bool readonly = true>
    CSArray<V, dimension, readonly>* readArray(bool nullIfEmpty = true);
    template <typename V, int dimension = 1, bool readonly = true>
    CSArray<V, dimension, readonly>* readArrayFunc(const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty = true);
    
    void write(const void* data, int size);
    void writeByte(int v);
    void writeBoolean(bool v);
    void writeShort(int v);
    void writeInt(int v);
    void writeLong(int64 v);
    void writeHalf(half v);
    void writeFloat(float v);
    void writeDouble(double v);
    void writeFixed(fixed v);
    void writeLength(int len);
    void writeString(const char* str);
    void writeString(const string& str);
    
    template <typename V, int dimension, bool readonly>
    void writeArray(const CSArray<V, dimension, readonly>* array);
    template <typename V, int dimension, bool readonly>
    void writeArray(const CSArray<V, dimension, readonly>* array, const typename CSEntryBufferType<V>::WriteFunc& func);
    template <typename V, int dimension, bool readonly>
    void writeArray(const CSArray<V, dimension, readonly>* array, void (V::*func)(CSBuffer* buffer) const);
    
    void writeBuffer(CSBuffer* buffer);
    void writeData(const CSData* data);
    
    bool AESEncrypt(const char* key, bool usingCheckSum);
    bool AESDecrypt(const char* key, bool usingCheckSum);
    
    bool compress();
    bool uncompress();
    
    bool writeToFile(const char* path, bool compression = false);
    
    inline const CSData* data() const {
        return _data;
    }
    inline int position() const {
        return _position;
    }
    inline void setPosition(int position) {
        _position = position;
        CSAssert(_position >= 0 && _position <= _data->length(), "position overflow");
    }
    inline void skip(int length) {
        _position += length;
        CSAssert(_position >= 0 && _position <= _data->length(), "position overflow");
    }
    inline int length() const {
        return _data->length();
    }
    inline void setLength(int length) {
        _data->setLength(length);
    }
    inline int remaining() const {
        return _data->length() - _position;
    }
    inline void* bytes() {
        return (byte*)_data->bytes() + _position;
    }
    inline const void* bytes() const {
        return (const byte*)_data->bytes() + _position;
    }
};

//=============================================================================================================
//buffer + read array

template <typename V, bool readonly, bool constructor = std::is_constructible<V, CSBuffer*>::value>
class bufferReadNArray {
public:
    static CSArray<V, 1, readonly>* read(CSBuffer* buffer, bool nullIfEmpty) {
        int count = buffer->readLength();
        CSArray<V, 1, readonly>* array = count || !nullIfEmpty ? CSArray<V, 1, readonly>::arrayWithCapacity(count) : NULL;
        if (count) {
            array->addObjectsFromPointer(buffer->bytes(), count);
            buffer->skip(count * sizeof(V));
        }
        return array;
    }
    static void read(CSArray<V, 1, readonly>* array, CSBuffer* buffer) {
        int count = buffer->readLength();
        array->setCapacity(count);
        if (count) {
            array->addObjectsFromPointer(buffer->bytes(), count);
            buffer->skip(count * sizeof(V));
        }
    }
};

template <typename V, bool readonly>
class bufferReadNArray<V, readonly, true> {
public:
    static CSArray<V, 1, readonly>* read(CSBuffer* buffer, bool nullIfEmpty) {
        int count = buffer->readLength();
        CSArray<V, 1, readonly>* array = count || !nullIfEmpty ? CSArray<V, 1, readonly>::arrayWithCapacity(count) : NULL;
        for (int i = 0; i < count; i++) new (&array->addObject()) V(buffer);
        return array;
    }
    static void read(CSArray<V, 1, readonly>* array, CSBuffer* buffer) {
        int count = buffer->readLength();
        array->setCapacity(count);
        for (int i = 0; i < count; i++) new (&array->addObject()) V(buffer);
    }
};

template <typename V, int dimension, bool readonly, bool retain = derived<CSObject, V>::value>
class bufferReadArray {
public:
    static CSArray<V, dimension, readonly>* read(CSBuffer* buffer, bool nullIfEmpty) {
        int count = buffer->readLength();
        CSArray<V, dimension, readonly>* array = count || !nullIfEmpty ? CSArray<V, dimension, readonly>::arrayWithCapacity(count) : NULL;
        for (int i = 0; i < count; i++) array->addObject(bufferReadArray<V, dimension - 1, readonly, retain>::read(buffer, nullIfEmpty));
        return array;
    }
    static CSArray<V, dimension, readonly>* read(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
        int count = buffer->readLength();
        CSArray<V, dimension, readonly>* array = count || !nullIfEmpty ? CSArray<V, dimension, readonly>::arrayWithCapacity(count) : NULL;
        for (int i = 0; i < count; i++) array->addObject(bufferReadArray<V, dimension - 1, readonly, retain>::read(buffer, func, nullIfEmpty));
        return array;
    }
    static void read(CSArray<V, dimension, readonly>* array, CSBuffer* buffer, bool nullIfEmpty) {
        int count = buffer->readLength();
        array->setCapacity(count);
        for (int i = 0; i < count; i++) array->addObject(bufferReadArray<V, dimension - 1, readonly, retain>::read(buffer, nullIfEmpty));
    }
    static void read(CSArray<V, dimension, readonly>* array, CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
        int count = buffer->readLength();
        array->setCapacity(count);
        for (int i = 0; i < count; i++) array->addObject(bufferReadArray<V, dimension - 1, readonly, retain>::read(buffer, func, nullIfEmpty));
    }
};

template <typename V, bool readonly>
class bufferReadArray<V, 0, readonly, true> {
public:
    static inline V* read(CSBuffer* buffer, bool nullIfEmpty) {
        return CSObject::autorelease(new V(buffer));
    }
    static inline V* read(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
        return func(buffer);
    }
};

template <typename V, bool readonly>
class bufferReadArray<V, 1, readonly, false> {
public:
    static CSArray<V, 1, readonly>* read(CSBuffer* buffer, bool nullIfEmpty) {
        return bufferReadNArray<V, readonly>::read(buffer, nullIfEmpty);
    }
    static CSArray<V, 1, readonly>* read(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
        int count = buffer->readLength();
        CSArray<V, 1, readonly>* array = count || !nullIfEmpty ? CSArray<V, 1, readonly>::arrayWithCapacity(count) : NULL;
        for (int i = 0; i < count; i++) func(buffer, array->addObject());
        return array;
    }
    static void read(CSArray<V, 1, readonly>* array, CSBuffer* buffer, bool nullIfEmpty) {
        bufferReadNArray<V, readonly>::read(array, buffer);
    }
    static void read(CSArray<V, 1, readonly>* array, CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
        int count = buffer->readLength();
        array->setCapacity(count);
        for (int i = 0; i < count; i++) func(buffer, array->addObject());
    }
};

template <typename V, int dimension, bool readonly>
CSArray<V, dimension, readonly>* CSBuffer::readArray(bool nullIfEmpty) {
    return bufferReadArray<V, dimension, readonly>::read(this, nullIfEmpty);
}

template <typename V, int dimension, bool readonly>
CSArray<V, dimension, readonly>* CSBuffer::readArrayFunc(const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) {
    return bufferReadArray<V, dimension, readonly>::read(this, func, nullIfEmpty);
}

template <typename V, int dimension, bool readonly>
CSArray<V, dimension, readonly>::CSArray(CSBuffer* buffer, bool nullIfEmpty) : CSArray(0) {
    bufferReadArray<V, dimension, readonly>::read(this, buffer, nullIfEmpty);
}

template <typename V, int dimension, bool readonly>
CSArray<V, dimension, readonly>::CSArray(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func, bool nullIfEmpty) : CSArray(0) {
    bufferReadArray<V, dimension, readonly>::read(this, buffer, func, nullIfEmpty);
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray(CSBuffer* buffer) : CSArray(0) {
    bufferReadArray<V, 1, readonly>::read(this, buffer, false);
}

template <typename V, bool readonly>
CSArray<V, 1, readonly>::CSArray(CSBuffer* buffer, const typename CSEntryBufferType<V>::ReadFunc& func) : CSArray(0) {
    bufferReadArray<V, 1, readonly>::read(this, buffer, func, false);
}

//=============================================================================================================
//buffer + write array

template <typename V, int dimension, bool readonly, bool retain = derived<CSObject, V>::value>
class bufferWriteArray {
public:
    static void write(CSBuffer* buffer, const CSArray<V, dimension, readonly>* array) {
        if (array) {
            buffer->writeLength(array->count());
            for (int i = 0; i < array->count(); i++) {
                bufferWriteArray<V, dimension - 1, retain>::write(buffer, array->objectAtIndex(i));
            }
        }
        else buffer->writeLength(0);
    }
    static void write(CSBuffer* buffer, const CSArray<V, dimension, readonly>* array, const typename CSEntryBufferType<V>::WriteFunc& func) {
        if (array) {
            buffer->writeLength(array->count());
            for (int i = 0; i < array->count(); i++) {
                bufferWriteArray<V, dimension - 1, retain>::write(buffer, array->objectAtIndex(i), func);
            }
        }
        else buffer->writeLength(0);
    }
    static void write(CSBuffer* buffer, const CSArray<V, dimension, readonly>* array, void (V::* func)(CSBuffer*) const) {
        if (array) {
            buffer->writeLength(array->count());
            for (int i = 0; i < array->count(); i++) {
                bufferWriteArray<V, dimension - 1, retain>::write(buffer, array->objectAtIndex(i), func);
            }
        }
        else buffer->writeLength(0);
    }
};

template <typename V, bool readonly>
class bufferWriteArray<V, 0, readonly, true> {
public:
    static inline void write(CSBuffer* buffer, const V* v) {
        v->writeTo(buffer);
    }
    static inline void write(CSBuffer* buffer, const V* v, const typename CSEntryBufferType<V>::WriteFunc& func) {
        func(buffer, v);
    }
    static inline void write(CSBuffer* buffer, const V* v, void (V::* func)(CSBuffer*) const) {
        (v->*func)(buffer);
    }
};

template <typename V, bool readonly>
class bufferWriteArray<V, 1, readonly, false> {
public:
    static void write(CSBuffer* buffer, const CSArray<V, 1, readonly>* array) {
        if (array) {
            buffer->writeLength(array->count());
            buffer->write(array->pointer(), array->count() * sizeof(V));
        }
        else buffer->writeLength(0);
    }

    static void write(CSBuffer* buffer, const CSArray<V, 1, readonly>* array, const typename CSEntryBufferType<V>::WriteFunc& func) {
        if (array) {
            buffer->writeLength(array->count());
            for (int i = 0; i < array->count(); i++) {
                func(buffer, array->objectAtIndex(i));
            }
        }
        else buffer->writeLength(0);
    }

    static void write(CSBuffer* buffer, const CSArray<V, 1, readonly>* array, void (V::* func)(CSBuffer*) const) {
        if (array) {
            buffer->writeLength(array->count());
            for (int i = 0; i < array->count(); i++) {
                const V& v = array->objectAtIndex(i);
                (v.*func)(buffer);
            }
        }
        else buffer->writeLength(0);
    }
};

template <typename V, int dimension, bool readonly>
void CSBuffer::writeArray(const CSArray<V, dimension, readonly>* array) {
    bufferWriteArray<V, dimension, readonly>::write(this, array);
}

template <typename V, int dimension, bool readonly>
void CSBuffer::writeArray(const CSArray<V, dimension, readonly>* array, const typename CSEntryBufferType<V>::WriteFunc& func) {
    bufferWriteArray<V, dimension, readonly>::write(this, array, func);
}

template <typename V, int dimension, bool readonly>
void CSBuffer::writeArray(const CSArray<V, dimension, readonly>* array, void (V::* func)(CSBuffer* buffer) const) {
    bufferWriteArray<V, dimension, readonly>::write(this, array, func);
}

#endif
