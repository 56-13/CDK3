#ifndef __CDK__CSLocaleString__
#define __CDK__CSLocaleString__

#include "CSString.h"
#include "CSArray.h"

class CSBuffer;

class CSLocaleString : public CSResource {
private:
    mutable string _currentLocaleValue;
    mutable uint _currentLocaleMark = 0;
    
    struct LocaleValue {
        string locale;
        string value;

        LocaleValue(const string& locale, const string& value);
        LocaleValue(CSBuffer* buffer);
        LocaleValue(const byte*& data);
    };
    CSArray<LocaleValue> _localeValues;
    
    static uint _localeMark;
    static string _locale;
	static char _digitGroupSeperator;

    static const string DefaultLocale;
public:
    CSLocaleString() = default;
    explicit CSLocaleString(CSBuffer* buffer);
    explicit CSLocaleString(const byte*& data);

    static inline CSLocaleString* localeString() {
        return autorelease(new CSLocaleString());
    }
    static inline CSLocaleString* localeStringWithBuffer(CSBuffer* buffer) {
        return autorelease(new CSLocaleString(buffer));
    }
    static inline CSLocaleString* localeStringWithBytes(const byte*& data) {
        return autorelease(new CSLocaleString(data));
    }

    const string& value() const;

    inline operator const string& () const {
        return value();
    }
    inline operator const char* () const {
        return value();
    }
    inline operator const uchar* () const {
        return value();
    }
    
    string localeValue(const string& locale, bool useDefault = true) const;
    
    void setLocaleValue(const string& locale, const string& value);
    
    inline CSResourceType resourceType() const override {
        return CSResourceTypeLocaleString;
    }
    int resourceCost() const override;

    static inline uint localeMark() {
        return _localeMark;
    }
    static inline const string& locale() {
        return _locale;
    }
    static void setLocale(const string& locale);

	static inline char digitGroupSeperator() {
		return _digitGroupSeperator;
	}
	static inline void setDigitGroupSeperator(char seperator) {
		_digitGroupSeperator = seperator;
	}
};

#endif
