#ifndef CSString_h
#define CSString_h

#include "CSStringContent.h"

enum CSStringEncoding {
    CSStringEncodingUTF8,
    CSStringEncodingUTF16,
    CSStringEncodingUTF16LE,
    CSStringEncodingUTF16BE
};

#define CSMaxComposedCharLength   9

struct character {
    char str[CSMaxComposedCharLength + 1] = {};
};

template <typename V, int dimension, bool readonly>
class CSArray;
class CSData;
class CSBuffer;

class string {
private:
	const CSStringContent* _content;
public:
    inline string() : _content(NULL) {}
    inline string(const CSStringContent* content) : _content(CSObject::retain(_content)) {}
    inline string(const string& other) : _content(CSObject::retain(other._content)) {}
    explicit string(const char* str);           //explicit으로 비효율적인 형변환을 막음, literal의 경우 S() 매크로 사용
    string(const char* str, int length);
	explicit string(CSBuffer* buffer);
    explicit string(int value, bool comma = false);
    explicit string(uint value, bool comma = false);
    explicit string(int64 value, bool comma = false);
    explicit string(uint64 value, bool comma = false);
    ~string();

	static string encode(const void* data, int length, CSStringEncoding encoding);
	static string format(const char* format, ...);
	static string formatAndArguments(const char* format, va_list args);
	static string contentOfFile(const char* path, CSStringEncoding encoding = CSStringEncodingUTF8);

	static int intValue(const char* str);
	static uint hexIntValue(const char* str);
	static int64 longValue(const char* str);
	static float floatValue(const char* str);
	static double doubleValue(const char* str);
	static bool boolValue(const char* str);

    inline int intValue() const {
        return intValue(_content->cstring());
    }
    inline uint hexIntValue() const {
        return hexIntValue(_content->cstring());
    }
    inline int64 longValue() const {
        return longValue(_content->cstring());
    }
    inline float floatValue() const {
        return floatValue(_content->cstring());
    }
    inline double doubleValue() const {
        return doubleValue(_content->cstring());
    }
    inline bool boolValue() const {
        return boolValue(_content->cstring());
    }

    character characterAtIndex(int index) const;
    int length() const;
    bool startsWith(const char* str) const;
    bool endsWith(const char* str) const;
    int indexOf(const char* str) const;
    int indexOf(const char* str, int from) const;
    string substringWithRange(int index, int length) const;
    string substringFromIndex(int index) const;

    CSArray<string, 1, true>* split(const char* delimiter) const;
    CSData* data(CSStringEncoding encoding = CSStringEncodingUTF8) const;
    void replace(const char* from, const char* to);
    void trim(bool left = true, bool right = true);
    static string replace(const char* str, const char* from, const char* to);
    static string trim(const char* str, bool left = true, bool right = true);
    
    void clear();
    void append(const char* str);
    void appendFormatAndArguments(const char* format, va_list args);
    void appendFormat(const char* format, ...);
    void insert(int index, const char* str);
    void insertFormatAndArguments(int i, const char* format, va_list args);
    void insertFormat(int index, const char* format, ...);
    void removeWithRange(int index, int length);
    void removeAtIndex(int index);

    bool AESEncrypt(const char* key);
    bool AESDecrypt(const char* key);
    static string AESEncrypt(const char* str, const char* key);
    static string AESDecrypt(const char* str, const char* key);

    static char* cstring(const uchar* str, bool free = true);
    static char* cstring(const uchar* str, int ulength, bool free = true);
    static uchar* ucstring(const char* str, bool free = true);
    static uchar* ucstring(const char* str, int clength, bool free = true);

    static char* cstringWithFormatAndArguments(const char* format, va_list args);
    static char* cstringWithFormat(const char* format, ...);
    static char* cstringWithNumber(int value, bool comma = false);
    static char* cstringWithNumber(uint value, bool comma = false);
    static char* cstringWithNumber(int64 value, bool comma = false);
    static char* cstringWithNumber(uint64 value, bool comma = false);

    inline const CSStringContent* content() const {
        return _content;
    }
    inline int clength() const {
        return _content ? _content->clength() : 0;
    }
    inline const char* cstring() const {
        return _content ? _content->cstring() : NULL;
    }
    const uchar* ucstring() const;

    inline operator const CSStringContent* () const {
        return _content;
    }
    inline operator const char* () const {
        return cstring();
    }
    inline operator const uchar* () const {
        return ucstring();
    }

    static uint toCodePoint(const uchar* cc, int& i);
    static uint toCodePoint(const char* cc, int& i);
    static inline uint toCodePoint(const uchar* cc) {
        int i = 0;
        return toCodePoint(cc, i);
    }
    static inline uint toCodePoint(const char* cc) {
        int i = 0;
        return toCodePoint(cc, i);
    }

    string sha1() const;
    static string sha1(const char* str);

    static void writeTo(CSBuffer* buffer, const char* str);
    void writeTo(CSBuffer* buffer) const;

    static bool writeToFile(const char* path, const char* str, CSStringEncoding encoding = CSStringEncodingUTF8, bool compression = false);
    bool writeToFile(const char* path, CSStringEncoding encoding = CSStringEncodingUTF8, bool compression = false) const;

    inline operator bool() const {
        return _content != NULL;
    }
    inline bool operator !() const {
        return _content == NULL;
    }

    string& operator =(const char* str);

    inline string& operator =(const string& other) {
        CSObject::retain(_content, other._content);
        return *this;
    }
    inline string& operator =(const CSStringContent* other) {
        CSObject::retain(_content, other);
        return *this;
    }
    inline bool operator ==(const string& other) const {
        return _content == other._content;
    }
    inline bool operator !=(const string& other) const {
        return _content != other._content;
    }
    inline bool operator ==(const char* cstr) const {
        return _content ? (cstr && strcmp(_content->cstring(), cstr) == 0) : cstr == NULL;
    }
    inline bool operator !=(const char* cstr) const {
        return !(*this == cstr);
    }
    inline bool operator <(const string& other) const {
        return strcmp(_content->cstring(), other._content->cstring()) < 0;
    }
    inline bool operator >(const string& other) const {
        return strcmp(_content->cstring(), other._content->cstring()) > 0;
    }
    string& operator +=(const char* str);
    string operator +(const char* str) const;

    inline uint hash() const {
        return std::hash<const void*>()(_content);
    }
    inline int resourceCost() const {
        return _content ? _content->resourceCost() : 0;
    }
};

#endif
