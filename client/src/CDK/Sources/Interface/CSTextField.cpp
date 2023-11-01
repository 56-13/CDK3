#define CDK_IMPL

#include "CSTextField.h"

#include "CSTextFieldBridge.h"

#include "CSApplication.h"

CSTextField::CSTextField() {
    _handle = CSTextFieldBridge::createHandle(this);
    _font = retain(CSGraphics::defaultFont());
}

CSTextField::~CSTextField() {
    _font->release();
    CSTextFieldBridge::destroyHandle(_handle);
}

void CSTextField::onStateChanged() {
    switch (state()) {
        case CSLayerStateFocus:
            applyFrame();
            applyFont();
            CSTextFieldBridge::addToView(_handle);
            setFocus(_autoFocus);
            break;
        case CSLayerStateHidden:
        case CSLayerStateDetach:
            CSTextFieldBridge::removeFromView(_handle);
            break;
        default:
            break;
    }
    CSLayer::onStateChanged();
}

void CSTextField::onFrameChanged() {
    applyFrame();
    
    CSLayer::onFrameChanged();
}

void CSTextField::onProjectionChanged() {
    applyFont();
    applyFrame();
    
    CSLayer::onProjectionChanged();
}

void CSTextField::onTouchesView(const CSArray<CSTouch>* touches, bool& interrupt) {
    if (_autoClose) setFocus(false);

    CSLayer::onTouchesView(touches, interrupt);
}

void CSTextField::applyFrame() {
    if (linked()) {
        CSRect frame = bounds();
        convertToViewSpace(frame.origin());
        CSApplication::sharedApplication()->convertToUIFrame(frame);
        CSTextFieldBridge::setFrame(_handle, frame);
    }
}

void CSTextField::applyFont() {
    if (linked()) {
        CSApplication* app = CSApplication::sharedApplication();
        CSVector2 screenSize = app->size();
        float scale = CSMath::min(screenSize.x / app->projectionWidth(), screenSize.y / app->projectionHeight());
        const CSFont* font = scale != 1.0f ? _font->derivedFontWithSize(_font->size() * scale) : _font;
        CSTextFieldBridge::setFont(_handle, font);
    }
}

void CSTextField::setAutoFocus(bool autoFocus) {
    _autoFocus = autoFocus;
    
    if (autoFocus) setFocus(true);
}

const char* CSTextField::text() const {
    return CSTextFieldBridge::text(_handle);
}

void CSTextField::setText(const char* text) {
    CSTextFieldBridge::setText(_handle, text);
}

void CSTextField::clearText() {
    CSTextFieldBridge::clearText(_handle);
}

void CSTextField::setTextColor(CSColor textColor) {
    CSTextFieldBridge::setTextColor(_handle, textColor);
}

CSColor CSTextField::textColor() const {
    return CSTextFieldBridge::textColor(_handle);
}

void CSTextField::setFont(const CSFont* font) {
    if (_font != font) {
        _font->release();
        _font = retain(font);
        applyFont();
    }
}

void CSTextField::setPlaceholder(const char* placeholder) {
    CSTextFieldBridge::setPlaceholder(_handle, placeholder);
}

const char* CSTextField::placeholder() const {
    return CSTextFieldBridge::placeholder(_handle);
}

void CSTextField::setTextAlignment(CSTextAlignment textAlignment) {
    CSTextFieldBridge::setTextAlignment(_handle, textAlignment);
}

CSTextAlignment CSTextField::textAlignment() const {
    return CSTextFieldBridge::textAlignment(_handle);
}

void CSTextField::setReturnKeyType(CSReturnKeyType keyType) {
    CSTextFieldBridge::setReturnKeyType(_handle, keyType);
}

CSReturnKeyType CSTextField::returnKeyType() const {
    return CSTextFieldBridge::returnKeyType(_handle);
}

void CSTextField::setKeyboardType(CSKeyboardType keyboardType) {
    CSTextFieldBridge::setKeyboardType(_handle, keyboardType);
}

CSKeyboardType CSTextField::keyboardType() const {
    return CSTextFieldBridge::keyboardType(_handle);
}

void CSTextField::setSecureText(bool secure) {
    CSTextFieldBridge::setSecureText(_handle, secure);
}

bool CSTextField::isSecureText() const {
    return CSTextFieldBridge::isSecureText(_handle);
}

void CSTextField::setEnabled(bool enabled) {
    if (enabled != this->enabled()) {
        if (!enabled) {
            CSTextFieldBridge::removeFromView(_handle);
        }
        else if (state() == CSLayerStateFocus) {
            CSTextFieldBridge::addToView(_handle);
        }
    }
    CSLayer::setEnabled(enabled);
}

int CSTextField::maxLength() const {
    return CSTextFieldBridge::maxLength(_handle);
}

void CSTextField::setMaxLength(int length) {
    CSTextFieldBridge::setMaxLength(_handle, length);
}

int CSTextField::maxLine() const {
	return CSTextFieldBridge::maxLine(_handle);
}

void CSTextField::setMaxLine(int maxLine) {
	CSTextFieldBridge::setMaxLine(_handle, maxLine);
}

bool CSTextField::isFocused() const {
    return CSTextFieldBridge::isFocused(_handle);
}

void CSTextField::setFocus(bool focused) {
    CSTextFieldBridge::setFocus(_handle, focused);
}

void CSTextField::onTextReturn() {
    OnTextReturn(this);
}

void CSTextField::onTextChanged() {
    OnTextChanged(this);
}

float CSTextField::keyboardScrollDegree(float bottom) const {
    float rtn = 0;
    if (isFocused()) {
        CSVector2 p(0, height() + _keyboardPadding);
        convertToViewSpace(p);
        if (p.y > bottom) rtn = p.y - bottom;
    }
    return rtn;
}
