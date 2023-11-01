#define CDK_IMPL

#include "CSTicker.h"

CSTicker::CSTicker() {
    _font = retain(CSGraphics::defaultFont());
}

CSTicker::~CSTicker() {
    _font->release();
}

void CSTicker::resetWidth() {
    _width = CSGraphics::stringSize(_texts.objectAtIndex(_position), _font).x;
}

void CSTicker::rewind() {
    _position = 0;
    _scroll = 0;
    if (_texts.count()) resetWidth();
    refresh();
}

void CSTicker::addText(const char* text) {
    _texts.addObject(string::replace(text, "\n", " "));
    
    _finished = false;
    if (_texts.count() == 1) {
        _scroll = 0;
        resetWidth();
    }
    
    refresh();
}

void CSTicker::clearText() {
    _texts.removeAllObjects();
    _position = 0;
    _scroll = 0;
    _width = 0;
    refresh();
}

void CSTicker::setFont(const CSFont* font) {
    if (retain(_font, font)) {
        if (_texts.count()) resetWidth();
        refresh();
    }
}

void CSTicker::setTextColor(const CSColor& textColor) {
    _textColor = textColor;
    refresh();
}


void CSTicker::setStrokeWidth(int width) {
    _strokeWidth = width;
    refresh();
}

void CSTicker::setStrokeColor(const CSColor& color) {
    _strokeColor = color;
    refresh();
}

bool CSTicker::timeout(float delta, CSLayerVisible visible) {
    if (!_paused && _texts.count()) {
        _scroll += _scrollSpeed * delta;
        if (_scroll > _width + width()) {
            _scroll = 0;
            if (++_position >= _texts.count()) {
                _finished = true;
                _position = 0;
            }
            resetWidth();
        }
        refresh();
    }
    return CSLayer::timeout(delta, visible);
}

void CSTicker::onDraw(CSGraphics* graphics) {
    CSLayer::onDraw(graphics);
    
    if (_texts.count()) {
        graphics->reset();
        
        const string& str = _texts.objectAtIndex(_position);

        clip(graphics);
        
        graphics->setColor(_textColor);
        graphics->setFont(_font);
        graphics->setStrokeWidth(_strokeWidth);
        graphics->setStrokeColor(_strokeColor);
        graphics->drawString(str, CSRect(width() - _scroll, 0, CSStringWidthUnlimited, height()), CSAlignMiddle);
    }
}
