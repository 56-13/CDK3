#define CDK_IMPL

#include "CSData.h"
#include "CSBytes.h"

#include "CSMath.h"
#include "CSLog.h"
#include "CSFile.h"
#include "CSCrypto.h"

CSData::CSData() : _length(0), _capacity(256) {
    _bytes = fmalloc(_capacity);
}

CSData::CSData(int capacity) : _length(0), _capacity(capacity) {
    CSAssert(_capacity > 0);
    _bytes = fmalloc(_capacity);
}

CSData::CSData(const void* bytes, int length, bool copy) {
    CSAssert(bytes && length > 0);
    if (copy) {
        _bytes = fmalloc(length);
        memcpy(_bytes, bytes, length);
    }
    else {
        _bytes = const_cast<void*>(bytes);
    }
    _capacity = _length = length;
}

CSData::CSData(const CSData* other) {
    _capacity = _length = other->_length;
    _bytes = fmalloc(_capacity);
    memcpy(_bytes, other->_bytes, _length);
}

CSData::~CSData() {
    free(_bytes);
}

CSData* CSData::createWithContentOfFile(const char* path, bool compression) {
    int length = 0;
    void* bytes = CSFile::createRawWithContentOfFile(path, 0, length, compression);
    return bytes ? new CSData(bytes, length, false) : NULL;
}

CSData* CSData::createWithContentOfFile(const char* path, int offset, int length, bool compression) {
    void* bytes = CSFile::createRawWithContentOfFile(path, offset, length, compression);
    return bytes ? new CSData(bytes, length, false) : NULL;
}

void CSData::setLength(int length) {
    CSAssert(length > 0);
    if (_length < length) expand(length - _length);
    _length = length;
}

void CSData::setBytes(const void* bytes, int length) {
    _length = 0;
    appendBytes(bytes, length);
}

void CSData::expand(int length) {
    CSAssert(length > 0);
    int capacity = _capacity;
    while (_length + length > capacity) capacity *= 2;
    if (capacity != _capacity) {
        _capacity = capacity;
        _bytes = frealloc(_bytes, capacity);
    }
}

void CSData::appendBytes(const void* bytes, int length) {
    expand(length);
    memcpy((byte*)_bytes + _length, bytes, length);
    _length += length;
}

void CSData::appendData(const CSData* data) {
    appendBytes(data->_bytes, data->_length);
}

CSData* CSData::subdataWithRange(int offset, int length) const {
    CSAssert(offset >= 0 && length > 0 && offset + length <= _length, "data offset overflow");
    
    return CSData::dataWithBytes((byte*)_bytes + offset, length);
}

void CSData::shrink(int position) {
    if (position > 0) {
        CSAssert(position <= _length, "data offset overflow");
        if (position < _length) {
            memmove(_bytes, (byte*)_bytes + position, _length - position);
            _length -= position;
        }
        else _length = 0;
    }
}

bool CSData::AESEncrypt(const char* key, bool usingCheckSum) {
    if (usingCheckSum) {
        int checkSumLength = CSMath::min(_length / 4, 256);
        
        uint checkSum = 0;
        const byte* ptr = (const byte*)_bytes;
        for (int i = 0; i < checkSumLength; i++) {
            checkSum ^= (uint)readInt(ptr);
        }
        
        appendBytes(&checkSum, 4);
    }
    int length = _length;
    const void* bytes = CSCrypto::encrypt(_bytes, length, key);
    if (bytes) {
        _length = 0;
        appendBytes(bytes, length);
        return true;
    }
    if (usingCheckSum) _length -= 4;
    return false;
}
bool CSData::AESDecrypt(const char* key, bool usingCheckSum) {
    int length = _length;
    const void* bytes = CSCrypto::decrypt(_bytes, length, key);
    if (bytes) {
        if (usingCheckSum) {
            if (length < 4) {
                return false;
            }
            int checkSumLength = CSMath::min((length - 4) / 4, 256);
            
            uint checkSum = 0;
            const byte* ptr = (const byte*)bytes;
            for (int i = 0; i < checkSumLength; i++) {
                checkSum ^= (uint)readInt(ptr);
            }
            
            uint dataCheckSum = bytesToInt((const byte*)bytes + length - 4);
            
            if (checkSum != dataCheckSum) return false;

            length -= 4;
        }
        _length = 0;
        appendBytes(bytes, length);
        return true;
    }
    return false;
}

string CSData::sha1() const {
    return CSCrypto::sha1(_bytes, _length);
}

bool CSData::compress() {
    int compLength = 0;
    void* comp = CSFile::createCompressedRawWithData(_bytes, _length, compLength);
    if (comp) {
        free(_bytes);
        _bytes = comp;
        _capacity = _length = compLength;
        return true;
    }
    return false;
}

bool CSData::uncompress() {
    int uncompLength = 0;
    void* uncomp = CSFile::createRawWithCompressedData(_bytes, _length, 0, uncompLength);
    if (uncomp) {
        free(_bytes);
        _bytes = uncomp;
        _capacity = _length = uncompLength;
        return true;
    }
    return false;
}

bool CSData::writeToFile(const char* path, bool compression) const {
    return CSFile::writeRawToFile(path, _bytes, _length, compression);
}

uint CSData::hash() const {
    int length = CSMath::min(_length, 10);
    
    const byte* ptr = (const byte*)_bytes;
    
    CSHash hash;
    for (int i = 0; i < length; i++) hash.combine(ptr[i]);
    return hash;
}

bool CSData::isEqual(const CSData* other) const {
    return _length == other->_length && memcmp(_bytes, other->_bytes, _length) == 0;
}

void CSData::hide() {
    memset(_bytes, 0, _capacity);
}

