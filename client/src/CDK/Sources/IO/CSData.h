#ifndef __CDK__CSData__
#define __CDK__CSData__

#include "CSString.h"

class CSData : public CSObject {
private:
    void* _bytes;
    int _length;
    int _capacity;
public:
    CSData();
    CSData(int capacity);
    CSData(const void* bytes, int length, bool copy = true);
    CSData(const CSData* other);
    ~CSData();

    static CSData* createWithContentOfFile(const char* path, bool compression = false);
    static CSData* createWithContentOfFile(const char* path, int offset, int length, bool compression = false);
    static CSData* createWithBase64EncodedString(const char* str);

    static inline CSData* data() {
        return autorelease(new CSData());
    }
    static inline CSData* dataWithCapacity(int capacity) {
        return autorelease(new CSData(capacity));
    }
    static inline CSData* dataWithBytes(const void* bytes, int length, bool copy = true) {
        return autorelease(new CSData(bytes, length, copy));
    }
    static inline CSData* dataWithContentOfFile(const char* path, bool compression = false) {
        return autorelease(createWithContentOfFile(path, compression));
    }
    static inline CSData* dataWithContentOfFile(const char* path, int offset, int length, bool compression = false) {
        return autorelease(createWithContentOfFile(path, offset, length, compression));
    }
    static inline CSData* dataWithBase64EncodedString(const char* str) {
        return autorelease(createWithBase64EncodedString(str));
    }
    static inline CSData* dataWithData(const CSData* other) {
        return autorelease(new CSData(other));
    }
    
    inline void* bytes() {
        return _bytes;
    }
    inline const void* bytes() const {
        return _bytes;
    }
    inline int length() const {
        return _length;
    }
    inline int capacity() const {
        return _capacity;
    }
    
    const CSStringContent* base64EncodedString() const;
    
    void setLength(int length);
    void setBytes(const void* bytes, int length);
    void appendBytes(const void* bytes, int length);
    void appendData(const CSData* data);
    
    CSData* subdataWithRange(int offset, int length) const;
    
    void shrink(int position);
    
    bool AESEncrypt(const char* key, bool usingCheckSum);
    bool AESDecrypt(const char* key, bool usingCheckSum);
    string sha1() const;
    
    bool compress();
    bool uncompress();
    
    bool writeToFile(const char* path, bool compression = false) const;
    
    virtual uint hash() const override;
    bool isEqual(const CSData* other) const;
    inline virtual bool isEqual(const CSObject* object) const override {
        const CSData* other = dynamic_cast<const CSData*>(object);
        return other && isEqual(other);
    }

    void hide();
private:
    void expand(int length);
};

#endif
