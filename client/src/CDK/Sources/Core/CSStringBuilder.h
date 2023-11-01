#ifndef __CDK__CSStringBuilder_h
#define __CDK__CSStringBuilder_h

#include "CSString.h"

class CSStringBuilder {
private:
	std::string _string;
	bool _glyph;
#ifdef CS_ASSERT_DEBUG
	bool _usable = true;
#endif
public:
	inline explicit CSStringBuilder(bool glyph = false) : _glyph(glyph) {}
	inline explicit CSStringBuilder(const char* str, bool glyph = false) : _string(str), _glyph(glyph) {}
	explicit CSStringBuilder(const string& str, bool glyph = false);
private:
	int toCharacterIndex(int index);
public:
	void clear();
	void replace(const char* from, const char* to);
	void trim(bool left = true, bool right = true);
	void append(const char* str);
	void appendFormatAndArguments(const char* format, va_list args);
	void appendFormat(const char* format, ...);
	void insert(int index, const char* str);
	void insertFormatAndArguments(int index, const char* format, va_list args);
	void insertFormat(int index, const char* format, ...);
	void removeWithRange(int index, int length);
	void removeAtIndex(int index);
	void set(const char* str);
	void set(const string& str);

	const CSStringContent* toString();
};

#endif

