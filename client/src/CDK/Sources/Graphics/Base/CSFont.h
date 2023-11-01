#ifndef __CDK__CSFont__
#define __CDK__CSFont__

#include "CSString.h"

enum CSFontStyle {
    CSFontStyleNormal,
    CSFontStyleBold,
    CSFontStyleItalic,
    CSFontStyleMedium = 4,         //extension from here
    CSFontStyleSemiBold
};

class CSFont : public CSObject {
private:
    void* _handle;
    string _name;
    float _size;
    CSFontStyle _style;
public:
    CSFont(float size, CSFontStyle style);
    CSFont(const string& name, float size, CSFontStyle style);
private:
    ~CSFont();
public:
	static void load(const char* name, CSFontStyle style, const char* path);

    static inline CSFont* systemFont(float size, CSFontStyle style) {
        return autorelease(new CSFont(size, style));
    }
    static inline CSFont* font(const string& name, float size, CSFontStyle style) {
        return autorelease(new CSFont(name, size, style));
    }

    inline void* handle() const {
        return _handle;
    }
    inline const string& name() const {
        return _name;
    }
    inline float size() const {
        return _size;
    }
    inline CSFontStyle style() const {
        return _style;
    }
    
    inline CSFont* CSFont::derivedFont(float size, CSFontStyle style) const {
        return _name ? font(_name, size, style) : systemFont(size, style);
    }
    inline CSFont* derivedFontWithSize(float size) const {
        return derivedFont(size, _style);
    }
    inline CSFont* derivedFontWithStyle(CSFontStyle style) const {
        return derivedFont(_size, style);
    }
    uint hash() const override;
    bool isEqual(const CSFont* font) const;
    inline bool isEqual(const CSObject* object) const override {
        const CSFont* font = dynamic_cast<const CSFont*>(object);
        return font && isEqual(font);
    }
};

#endif
