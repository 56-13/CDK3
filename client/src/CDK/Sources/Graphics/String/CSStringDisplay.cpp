#define CDK_IMPL

#include "CSStringDisplay.h"
#include "CSStringContentTable.h"
#include "CSStringBuilder.h"
#include "CSGlyphs.h"

#include "icu_compat.h"

#include <functional>

#include <unicode/ushape.h>

#define FormatStringFuncPrefix          '#'
#define FormatStringFuncParamCount      4
#define FormatStringFuncNameLength      16
#define FormatStringFuncParamLength     16

CSStringParagraph::~CSStringParagraph() {
	if (type == CSStringParagraph::TypeFont) {
		if (attr.font.name) attr.font.name->release();
	}
}

CSStringParagraph::CSStringParagraph(const CSStringParagraph& other) {
	memcpy(this, &other, sizeof(CSStringParagraph));
	if (type == TypeFont) {
		if (attr.font.name) attr.font.name->retain();
	}
}

CSStringParagraph& CSStringParagraph::operator=(const CSStringParagraph& other) {
	if (type == TypeFont) {
		if (attr.font.name) attr.font.name->release();
	}
	memcpy(this, &other, sizeof(CSStringParagraph));
	if (type == TypeFont) {
		if (attr.font.name) attr.font.name->retain();
	}
	return *this;
}

//========================================================================================================================
//TODO:로직변경됨. 확인 필요 2023-04-10

CSStringDisplay::CSStringDisplay(const char* str) : CSStringDisplay(string::ucstring(str)) {

}

static uchar* createContent(const uchar* str) {
	int len = u_strlen(str);
	uchar* content = (uchar*)calloc((len + 1), sizeof(uchar));
	UErrorCode err = U_ZERO_ERROR;
	ucompat_shapeArabic(str, len, content, len + 1, U_SHAPE_LETTERS_SHAPE, &err);
	if (err > U_ZERO_ERROR) u_strcpy(content, str);
	return content;
}

CSStringDisplay::CSStringDisplay(const uchar* str) : _content(createContent(str)), _characters(UBRK_CHARACTER, _content) {
	_rtl = false;

	CSGlyphs* cs = CSGlyphs::sharedGlyphs();

	CSCharSequence words(UBRK_WORD, _content);

	int pci = 0;
	int ci = 0;
	int wi = 0;

	CSStringParagraph::Type texttype = CSStringParagraph::TypeNeutral;

	while (ci < _characters.count()) {
		uchar* cc = _content + _characters.from(ci);

		switch (cc[0]) {
			case '\r':
				if (_characters.length(ci) >= 2 && cc[1] == '\n') {
					CSStringParagraph& p = _paragraphs.addObject();
					p.type = CSStringParagraph::TypeLinebreak;
					p.attr.text.start = ci;
					p.attr.text.end = ci + 1;
				}
				pci = ++ci;
				while (wi < words.count() && _characters.from(ci) >= words.to(wi)) wi++;
				continue;
			case '\n':
				{
					CSStringParagraph& p = _paragraphs.addObject();
					p.type = CSStringParagraph::TypeLinebreak;
					p.attr.text.start = ci;
					p.attr.text.end = ci + 1;
				}
				pci = ++ci;
				while (wi < words.count() && _characters.from(ci) >= words.to(wi)) wi++;
				continue;
			case FormatStringFuncPrefix:
				if (readTag(ci)) {
					pci = ci;
					while (wi < words.count() && _characters.from(ci) >= words.to(wi)) wi++;
					continue;
				}
				break;
		}

		CSStringParagraph::Type ntexttype = CSStringParagraph::TypeNeutral;
		switch (ucompat_getBaseDirection(cc, _characters.length(ci))) {
			case UBIDI_LTR:
				ntexttype = CSStringParagraph::TypeLTR;
				break;
			case UBIDI_RTL:
				ntexttype = CSStringParagraph::TypeRTL;
				_rtl = true;
				break;
			case UBIDI_NEUTRAL:
				{
					int i = _characters.from(ci);
					if (u_isspace(string::toCodePoint(_content, i))) ntexttype = CSStringParagraph::TypeSpace;
				}
				break;
		}
		if (ntexttype != texttype && pci < ci) {
			CSStringParagraph& p = _paragraphs.addObject();
			p.type = texttype;
			p.attr.text.start = pci;
			p.attr.text.end = ci;
			pci = ci;
		}

		texttype = ntexttype;

		if (wi < words.count() && _characters.to(ci) >= words.to(wi)) {
			CSStringParagraph& p = _paragraphs.addObject();
			p.type = texttype;
			p.attr.text.start = pci;
			p.attr.text.end = ci + 1;
			pci = ci + 1;
			wi++;
		}
		ci++;
	}

	//==============================================================================
	/*
	foreach (const CSStringParagraph&, pa, _paragraphs) {
		int from = _characters.from(pa.attr.text.start);
		int to = _characters.from(pa.attr.text.end);
		CSLog("paragraph:%d~%d type:%d text:%s", from, to, pa.type, CSString::cstring(_content + from, to - from));
	}
	 */
	//============================================================================================
}

CSStringDisplay::~CSStringDisplay() {
	free(_content);
}

int CSStringDisplay::resourceCost() const {
	return sizeof(CSStringDisplay) + _characters.count() * sizeof(uchar) + _characters.resourceCost() + _paragraphs.capacity() * sizeof(CSStringParagraph);
}

static int parseTag(const uchar* str, int i, const std::function<bool(const char* func, const char* const* params, int paramCount)>& cb) {
	int prev = i;

	const char* func = NULL;
	const char* params[FormatStringFuncParamCount] = {};
	int paramCount = 0;

	bool flag = true;
	do {
		switch (str[i]) {
			case '\0':
				return 0;
			case '(':
				if (prev == i) return 0;
				func = string::cstring(str + prev, i - prev);
				i++;
				prev = i;
				flag = false;
				break;
			default:
				if (++i - prev > FormatStringFuncNameLength) return 0;
				break;
		}
	} while (flag);

	flag = true;

	do {
		switch (str[i]) {
			case '\0':
				return 0;
			case ')':
				flag = false;
				if (i == prev) {
					i++;
					prev = i;
					break;
				}
			case ',':
				if (paramCount == FormatStringFuncParamCount) return 0;
				while (str[prev] == ' ') prev++;
				if (i > prev) {
					int end = i;
					while (str[end - 1] == ' ') end--;
					params[paramCount] = string::cstring(str + prev, end - prev);
				}
				paramCount++;
				i++;
				prev = i;
				break;
			default:
				if (++i - prev > FormatStringFuncParamLength) return 0;
				break;
		}
	} while (flag);

	return cb(func, params, paramCount) ? i : 0;
}

bool CSStringDisplay::readTag(int& ci) {
	int next = parseTag(_content, _characters.from(ci) + 1, [this](const char* func, const char* const* params, int paramCount) {
		if (stricmp(func, "color") == 0) {
			CSStringParagraph& p = _paragraphs.addObject();
			p.type = CSStringParagraph::TypeColor;
			p.attr.color = params[0] ? CSColor(string::hexIntValue(params[0])) : CSColor::Transparent;
			return true;
		}
		else if (stricmp(func, "stroke") == 0) {
			CSStringParagraph& p = _paragraphs.addObject();
			p.type = CSStringParagraph::TypeStroke;
			p.attr.stroke.color = params[0] ? CSColor(string::hexIntValue(params[0])) : CSColor::Transparent;
			p.attr.stroke.width = params[1] ? string::intValue(params[1]) : 0;
			return true;
		}
		else if (stricmp(func, "gradient") == 0) {
			CSStringParagraph& p = _paragraphs.addObject();
			if (params[0] && params[1]) {
				p.type = CSStringParagraph::TypeGradient;
				p.attr.gradient.color[0] = CSColor(string::hexIntValue(params[0]));
				p.attr.gradient.color[1] = (string::hexIntValue(params[1]));
				p.attr.gradient.horizontal = params[2] && stricmp(params[2], "horizontal") == 0;
			}
			else {
				p.type = CSStringParagraph::TypeGradientReset;
			}
			return true;
		}
		else if (stricmp(func, "align") == 0) {
			if (params[0]) {
				CSStringParagraph& p = _paragraphs.addObject();
				p.type = CSStringParagraph::TypeAlign;
				if (stricmp(params[0], "center") == 0) p.attr.align = 1;
				else if (stricmp(params[0], "right") == 0) p.attr.align = 2;
				else p.attr.align = 0;

				return true;
			}
		}
		else if (stricmp(func, "font") == 0) {
			CSStringParagraph& p = _paragraphs.addObject();
			p.type = CSStringParagraph::TypeFont;
			p.attr.font.name = params[0] ? CSStringContentTable::sharedTable()->get(params[0]) : NULL;
			p.attr.font.size = params[1] ? string::floatValue(params[1]) : 0;
			p.attr.font.style = (CSFontStyle)-1;
			if (params[2]) {
				if (stricmp(params[2], "normal") == 0) {
					p.attr.font.style = CSFontStyleNormal;
				}
				else if (stricmp(params[2], "bold") == 0) {
					p.attr.font.style = CSFontStyleBold;
				}
				else if (stricmp(params[2], "italic") == 0) {
					p.attr.font.style = CSFontStyleItalic;
				}
				else if (stricmp(params[2], "medium") == 0) {
					p.attr.font.style = CSFontStyleMedium;
				}
				else if (stricmp(params[2], "semibold") == 0) {
					p.attr.font.style = CSFontStyleSemiBold;
				}
			}
			return true;
		}
		return false;
	});

	if (next) {
		while (_characters.from(ci) < next) ci++;
		return true;
	}
	return false;
}


string CSStringDisplay::customString(const string& str, char prefix, const std::function<bool(CSStringBuilder& dest, const char* func, const char* const* params, int paramCount)>& cb) {
	if (!str) return NULL;

	CSStringBuilder dest;

	int prev = 0;
	int i = 0;

	const CSStringDisplay* display = str.content()->display();

	const uchar* content = display->content();

	while (content[i]) {
		if (content[i] == prefix) {
			if (i > prev) {
				dest.append(string::cstring(content + prev, i - prev));
			}
			int next = parseTag(content, i + 1, [&](const char* func, const char* const* params, int paramCount) {
				return cb(dest, func, params, paramCount);
			});
			if (next) {
				prev = next;
				i = next;
			}
			else {
				prev = i;
				i++;
			}
		}
		else {
			i++;
		}
	}
	if (prev < i) {
		dest.append(string::cstring(content + prev, i - prev));
	}
	return dest.toString();
}
