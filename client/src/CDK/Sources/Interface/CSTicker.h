#ifndef CSTicker_h
#define CSTicker_h

#include "CSLayer.h"

class CSTicker : public CSLayer {
private:
    CSArray<string> _texts;
    const CSFont* _font;
    CSColor _textColor = CSColor::White;
    float _scroll = 0;
    float _scrollSpeed = 120;
    float _width = 0;
    int _position = 0;
    CSColor _strokeColor = CSColor::Black;
    byte _strokeWidth = 0;
    bool _finished = true;
    bool _paused = false;
public:
    CSTicker();
protected:
    virtual ~CSTicker();
public:
    inline const CSArray<string>* texts() const {
        return &_texts;
    }
private:
    void resetWidth();
public:
    void rewind();
    void addText(const char* text);
    void clearText();
    
    inline bool isFinished() const {
        return _finished;
    }
    
    void setFont(const CSFont* font);
    inline const CSFont* font() const {
        return _font;
    }
    void setTextColor(const CSColor& textColor);
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
    inline void setScroll(float scroll) {
        _scroll = scroll;
    }
    inline float scroll() const {
        return _scroll;
    }
    inline void setScrollSpeed(float scrollSpeed) {
        _scrollSpeed = scrollSpeed;
    }
    inline float scrollSpeed() const {
        return _scrollSpeed;
    }
    inline void pause() {
        _paused = true;
    }
    inline void resume() {
        _paused = false;
    }

    virtual bool timeout(float delta, CSLayerVisible visible) override;
protected:
    virtual void onDraw(CSGraphics* graphics) override;
};

#endif
