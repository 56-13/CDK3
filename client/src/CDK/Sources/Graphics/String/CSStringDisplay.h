#ifndef __CDK__CSStringDisplay__
#define __CDK__CSStringDisplay__

#include "CSArray.h"
#include "CSColor.h"
#include "CSFont.h"

class CSStringBuilder;

struct CSStringParagraph {
public:
	enum Type {
		TypeNeutral,
		TypeLTR,
		TypeRTL,
		TypeSpace,
		TypeLinebreak,
		TypeColor,
		TypeStroke,
		TypeGradient,
		TypeGradientReset,
		TypeAlign,
		TypeFont
	};
	Type type;

	union _attr {
		struct {
			int start;
			int end;
		} text;
		CSColor color;
		struct {
			CSColor color;
			byte width;
		} stroke;
		struct {
			CSColor color[2];
			bool horizontal;
		} gradient;
		byte align;
		struct {
			const CSStringContent* name;
			float size;
			CSFontStyle style;
		} font;

		inline _attr() {}
	} attr;

	CSStringParagraph() = default;
	~CSStringParagraph();

	CSStringParagraph(const CSStringParagraph& other);
	CSStringParagraph& operator=(const CSStringParagraph& other);
};

class CSStringDisplay {
private:
	uchar* _content;
	CSCharSequence _characters;
	CSArray<CSStringParagraph> _paragraphs;
	bool _rtl;
public:
	CSStringDisplay(const char* str);
	CSStringDisplay(const uchar* str);
	~CSStringDisplay();

	CSStringDisplay(const CSStringDisplay&) = delete;
	CSStringDisplay& operator=(const CSStringDisplay&) = delete;

	int resourceCost() const;

	inline const uchar* content() const {
		return _content;
	}
	inline const CSCharSequence* characters() const {
		return &_characters;
	}
	inline const CSArray<CSStringParagraph>* paragraphs() const {
		return &_paragraphs;
	}
	inline bool isRTL() const {
		return _rtl;
	}

	static string customString(const string& str, char prefix, const std::function<bool(CSStringBuilder& dest, const char* func, const char* const* params, int paramCount)>& cb);

private:
	bool readTag(int& ci);
};

#endif
