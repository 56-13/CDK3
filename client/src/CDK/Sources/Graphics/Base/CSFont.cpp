#define CDK_IMPL

#include "CSFont.h"

#include "CSRaster.h"

#include "CSEntry.h"

CSFont::CSFont(float size, CSFontStyle style) :
    _handle(CSRaster::createSystemFontHandle(size, style)),
    _size(size),
    _style(style)
{
}

CSFont::CSFont(const string& name, float size, CSFontStyle style) :
    _handle(CSRaster::createFontHandle(name, size, style)),
    _name(name),
    _size(size),
    _style(style)
{
}

CSFont::~CSFont() {
    CSRaster::destroyFontHandle(_handle);
}

void CSFont::load(const char* name, CSFontStyle style, const char* path) {
	CSRaster::loadFontHandle(name, style, path);
}

uint CSFont::hash() const {
    CSHash hash;
    hash.combine(_name);
    hash.combine(_size);
    hash.combine(_style);
    return hash;
}

bool CSFont::isEqual(const CSFont* font) const {
    return _name == font->_name && _size == font->_size && _style == font->_style;
}
