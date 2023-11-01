#define CDK_IMPL

#include "CSTextBox.h"

#include "CSStringBuilder.h"

CSTextBox::CSTextBox() : scroll(this) {
    scroll.OnScroll.add([this](CSScrollParent* parent) {
        refresh();
    });
    _font = retain(CSGraphics::defaultFont());
    setTouchCarry(false);
}

CSTextBox::~CSTextBox() {
    _font->release();
}

void CSTextBox::updateTextHeight() {
    _textHeight = CSGraphics::stringSize(_text, _font, width()).y;
}

void CSTextBox::setFont(const CSFont* font) {
    if (retain(_font, font)) {
        updateTextHeight();
        refresh();
    }
}

void CSTextBox::autoScroll() {
    if (_textHeight > height() && scroll.position().y < _textHeight - height()) {
        scroll.set(CSVector2(0, _textHeight - height()));
    }
}

void CSTextBox::setText(const string& str) {
    _text = str;
    updateTextHeight();
    refresh();
}

void CSTextBox::setTextColor(const CSColor& color) {
    _textColor = color;
    refresh();
}

void CSTextBox::setStrokeWidth(int width) {
	_strokeWidth = width;
	refresh();
}

void CSTextBox::setStrokeColor(const CSColor& color) {
	_strokeColor = color;
	refresh();
}

bool CSTextBox::shrinkText() {
    if (_maxHeight) {
        CSVector2 size = CSGraphics::stringSize(_text, _font, width());
        
        if (size.y > _maxHeight) {
            _text = CSGraphics::shrinkString(_text, _font, size.y - _maxHeight, width());
            return true;
        }
    }
    return false;
}

void CSTextBox::setMaxHeight(float maxHeight) {
    _maxHeight = maxHeight;
    if (shrinkText()) {
        updateTextHeight();
        
        refresh();
    }
}

void CSTextBox::clearText() {
    _text.clear();
    _textHeight = 0;
    refresh();
}

void CSTextBox::appendText(const char* str) {
    CSStringBuilder strbuf;
    strbuf.append(_text);
    strbuf.append(str);
    strbuf.append("\n");
    _text = strbuf.toString();

    shrinkText();
    updateTextHeight();
    
    if (!scroll.isScrolling() && scroll.position().y < _textHeight - height()) {
        scroll.set(CSVector2(0, _textHeight - height()));
    }
    refresh();
}

bool CSTextBox::timeout(float delta, CSLayerVisible visible) {
    scroll.timeout(delta, isTouching());
    
    return CSLayer::timeout(delta, visible);
}

void CSTextBox::onDraw(CSGraphics* graphics) {
    CSLayer::onDraw(graphics);

    graphics->reset();

    clip(graphics);

    graphics->setColor(_textColor);
    graphics->setFont(_font);
	graphics->setStrokeWidth(_strokeWidth);
	graphics->setStrokeColor(_strokeColor);
    graphics->drawStringScrolled(_text, bounds(), scroll.position().y);
    
    graphics->setColor(CSColor::White);
	graphics->setStrokeWidth(0);
    scroll.draw(graphics);
}

void CSTextBox::onTouchesMoved() {
    if (!_lockScroll) {
        const CSTouch* touch = touches()->objectAtIndex(0);
        CSVector2 p0 = touch->prevPoint(this);
        CSVector2 p1 = touch->point(this);
        
        scroll.drag(p0 - p1);
    }
    CSLayer::onTouchesMoved();
}

