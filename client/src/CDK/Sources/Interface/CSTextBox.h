#ifndef __CDK__CSTextBox__
#define __CDK__CSTextBox__

#include "CSLayer.h"

#include "CSScroll.h"

class CSTextBox : public CSLayer, public CSScrollParent {
public:
    CSScroll scroll;
private:
    string _text;
    const CSFont* _font;
	CSColor _textColor = CSColor::White;
	float _maxHeight = 0;
    float _textHeight = 0;
	CSColor _strokeColor = CSColor::Black;
	byte _strokeWidth = 0;
	bool _lockScroll = false;
public:
    CSTextBox();
protected:
    virtual ~CSTextBox();
public:
    static inline CSTextBox* textBox() {
        return autorelease(new CSTextBox());
    }
    
    inline CSVector2 scrollContentSize() const override {
        return CSVector2(width(), _textHeight);
    }
    inline CSVector2 scrollClipSize() const override {
        return size();
    }
    void setText(const string& str);
    inline const string& text() const {
        return _text;
    }
    void setFont(const CSFont* font);
    inline const CSFont* font() const {
        return _font;
    }
    void setTextColor(const CSColor& color);
    inline const CSColor& textColor() const {
        return _textColor;
    }
	void setStrokeWidth(int width);
	inline int strokeWidth() const {
		return _strokeWidth;
	}
	void setStrokeColor(const CSColor& color);
	inline const CSColor& strokeColor() const {
		return _strokeColor;
	}
    void setMaxHeight(float maxHeight);
    inline float maxHeight() const {
        return _maxHeight;
    }
    inline float textHeight() const {
        return _textHeight;
    }
    inline void setLockScroll(bool lockScroll) {
        _lockScroll = lockScroll;
    }
    inline bool lockScroll() const {
        return _lockScroll;
    }
    
    void autoScroll();

    void clearText();
    void appendText(const char* str);
private:
    bool shrinkText();
    void updateTextHeight();
public:
    virtual bool timeout(float delta, CSLayerVisible visible) override;
protected:
    virtual void onDraw(CSGraphics* graphics) override;
    virtual void onTouchesMoved() override;
};
#endif
