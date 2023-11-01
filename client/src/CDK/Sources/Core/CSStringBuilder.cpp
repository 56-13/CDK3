#define CDK_IMPL

#include "CSStringBuilder.h"
#include "CSStringContentTable.h"

CSStringBuilder::CSStringBuilder(const string& str, bool glyph) : _glyph(glyph) {
    if (str) _string = str.content()->string();
}

int CSStringBuilder::toCharacterIndex(int index) {
    if (_glyph && index > 0) {
        CSCharSequence characters(UBRK_CHARACTER, _string.c_str());
        index = characters.from(index);
    }
    return index;
}

void CSStringBuilder::clear() {
    _string.clear();
#ifdef CS_ASSERT_DEBUG
    _usable = true;
#endif
}

void CSStringBuilder::replace(const char* from, const char* to) {
    CSAssert(_usable);

    int pos = 0;
    int fromlen = strlen(from);
    int tolen = strlen(from);
    while ((pos = _string.find(from, pos)) != std::string::npos) {
        _string.replace(pos, fromlen, to);
        pos += tolen;
    }
}

void CSStringBuilder::trim(bool left, bool right) {
    CSAssert(_usable);

    static const char* whitespace = " \t\r\v\n";

    if (left) _string.erase(_string.find_last_not_of(whitespace) + 1);
    if (right) _string.erase(0, _string.find_first_not_of(whitespace));
}

void CSStringBuilder::append(const char* str) {
    CSAssert(_usable);

    if (str) _string.append(str);
}

void CSStringBuilder::appendFormatAndArguments(const char* format, va_list args) {
    CSAssert(_usable);

    va_list copy;
    va_copy(copy, args);
    int length = vsnprintf(NULL, 0, format, copy) + 1;
    va_end(copy);

    char* buf = (char*)fmalloc(length);
    vsprintf(buf, format, args);
    _string.append(buf);
    free(buf);
}

void CSStringBuilder::appendFormat(const char* format, ...) {
    va_list ap;
    va_start(ap, format);
    appendFormatAndArguments(format, ap);
    va_end(ap);
}

void CSStringBuilder::insert(int index, const char* str) {
    CSAssert(_usable);

    if (str) {
        index = toCharacterIndex(index);

        _string.insert(index, str);
    }
}

void CSStringBuilder::insertFormatAndArguments(int index, const char* format, va_list args) {
    CSAssert(_usable);

    index = toCharacterIndex(index);

    va_list copy;
    va_copy(copy, args);
    int length = vsnprintf(NULL, 0, format, copy) + 1;
    va_end(copy);

    char* buf = (char*)fmalloc(length);
    vsprintf(buf, format, args);
    _string.insert(index, buf);
    free(buf);
}

void CSStringBuilder::insertFormat(int index, const char* format, ...) {
    index = toCharacterIndex(index);

    va_list ap;
    va_start(ap, format);
    insertFormatAndArguments(index, format, ap);
    va_end(ap);
}

void CSStringBuilder::removeWithRange(int index, int length) {
    CSAssert(_usable);

    index = toCharacterIndex(index);

    _string.erase(index, length);
}

void CSStringBuilder::removeAtIndex(int i) {
    removeWithRange(i, 1);
}

void CSStringBuilder::set(const char* str) {
    _string = str;
#ifdef CS_ASSERT_DEBUG
    _usable = true;
#endif
}

void CSStringBuilder::set(const string& str) {
    if (str) _string = str.content()->string();
    else _string.clear();
#ifdef CS_ASSERT_DEBUG
    _usable = true;
#endif
}

const CSStringContent* CSStringBuilder::toString() {
    CSAssert(_usable);
    if (_string.length() == 0) return NULL;
    const CSStringContent* content = CSStringContentTable::sharedTable()->get(std::move(_string));
#ifdef CS_ASSERT_DEBUG
    _usable = false;
#endif
    return content;
}
