#ifndef __CDK__CSStringContent_h
#define __CDK__CSStringContent_h

#include "CSResource.h"

#include "CSCharSequence.h"

#include <string>

class CSStringDisplay;

class CSStringContent : public CSResource {
private:
    std::string _string;
	mutable const CSCharSequence* _characters;
    mutable const CSStringDisplay* _display;

    CSStringContent(std::string&& str);
    ~CSStringContent();
public:
    inline CSResourceType resourceType() const override {
        return CSResourceTypeString;
    }
    int resourceCost() const override;

    inline const std::string& string() const {
        return _string;
    }
    inline int clength() const {
        return _string.length();
    }
    inline const char* cstring() const {
        return _string.c_str();
    }
    inline operator const char* () const {
        return cstring();
    }

    const CSCharSequence* characters() const;
    const CSStringDisplay* display() const;

    friend class CSStringContentTable;
};

#endif
