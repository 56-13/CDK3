#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSTextFieldImpl.h"

#include "CSTime.h"

#include "CSStringBuilder.h"

#include "CSApplication.h"

CSTextFieldHandle::CSTextFieldHandle(CSTextField* textField) : _textField(textField),
	_textColor(CSColor::White),
	_maxLine(1),
	_maxLength(0),
	_secureText(false),
	_textAlignment(CSTextAlignmentLeft),
	_cursor(0),
	_scroll(0),
	_attach(false),
	_cursorVisible(false)
{
	CSTextFieldHandleManager::sharedManager()->addHandle(this);
}

CSTextFieldHandle::~CSTextFieldHandle() {
	CSTextFieldHandleManager::sharedManager()->removeHandle(this);
}

void CSTextFieldHandle::attach() {
	if (!_attach) {
		_attach = true;

		_textField->refresh();
	}
}

void CSTextFieldHandle::detach() {
	if (_attach) {
		_attach = false;
		CSTextFieldHandleManager::sharedManager()->unfocusHandle(this);

		_textField->refresh();
	}
}

void CSTextFieldHandle::setText(const string& text) {
	_text = text;
	_cursor = _text.length();
	_textField->onTextChanged();
	_textField->refresh();
}

void CSTextFieldHandle::clearText() {
	_text.clear();
	_cursor = 0;
	_textField->onTextChanged();
	_textField->refresh();
}

void CSTextFieldHandle::setMaxLength(int maxLength) {
	_maxLength = maxLength;
	if (_maxLength && _text.length() > _maxLength) {
		_text = _text.substringWithRange(0, _maxLength);
		if (_cursor > _text.length()) _cursor = _text.length();
		_textField->onTextChanged();

		_textField->refresh();
	}
}

void CSTextFieldHandle::setPlaceholder(const string& placeholder) {
	_placeholder = placeholder;
}

bool CSTextFieldHandle::isFocused() const {
	return CSTextFieldHandleManager::sharedManager()->focusedHandle() == this;
}

void CSTextFieldHandle::setFocus(bool focused) {
	if (focused) {
		CSTextFieldHandleManager::sharedManager()->focusHandle(this);
	}
	else {
		CSTextFieldHandleManager::sharedManager()->unfocusHandle(this);
	}
	_textField->refresh();
}

static bool convertToUTF8(uint codepoint, char* target, int& length) {
	static const uint byteMask = 0xBF;
	static const uint byteMark = 0x80;
	static const byte firstByteMark[7] = { 0x00, 0x00, 0xC0, 0xE0, 0xF0, 0xF8, 0xFC };

	if (codepoint < 0x80) length = 1;
	else if (codepoint < 0x800) length = 2;
	else if (codepoint < 0x10000) length = 3;
	else if (codepoint <= 0x0010FFFF) length = 4;
	else {
		length = 0;
		return false;
	}

	switch (length) {
		case 4: target[3] = (char)((codepoint | byteMark) & byteMask); codepoint >>= 6;
		case 3: target[2] = (char)((codepoint | byteMark) & byteMask); codepoint >>= 6;
		case 2: target[1] = (char)((codepoint | byteMark) & byteMask); codepoint >>= 6;
		case 1: target[0] = (char)(codepoint | firstByteMark[length]);
	}
	target[length] = 0;
	return true;
}

void CSTextFieldHandle::inputCharacter(uint codepoint) {
	int len;
	char str[8];
	if (convertToUTF8(codepoint, str, len) && (!_maxLength || _text.length() + len <= _maxLength)) {
		_text.insert(_cursor++, str);
		_textField->onTextChanged();
		_textField->refresh();
	}
}

void CSTextFieldHandle::moveLeftCursor(bool deleting) {
	if (_cursor) {
		if (deleting) _text.removeAtIndex(_cursor - 1);
		_cursor--;

		_textField->refresh();
	}
}

void CSTextFieldHandle::moveRightCursor(bool deleting) {
	if (_cursor < _text.length()) {
		if (deleting) _text.removeAtIndex(_cursor);
		else _cursor++;

		_textField->refresh();
	}
}

void CSTextFieldHandle::moveStartCursor() {
	if (_cursor) {
		_cursor = 0;
		_textField->refresh();
	}
}

void CSTextFieldHandle::moveEndCursor() {
	if (_cursor < _text.length()) {
		_cursor = _text.length();
		_textField->refresh();
	}
}

void CSTextFieldHandle::moveCursor(CSVector2 p) {
	if (_maxLine > 1) {
		p.y += _scroll;
		_cursor = CSGraphics::stringCursor(_text, _textField->font(), p, _textField->width());
	}
	else {
		p.x += _scroll;
		_cursor = CSGraphics::stringCursor(_text, _textField->font(), CSVector2(p.x, 0));
	}
	_textField->refresh();
}

void CSTextFieldHandle::complete() {
	_textField->onTextReturn();
}

void CSTextFieldHandle::draw(CSGraphics* graphics) {
	CSRect frame = _textField->bounds();
	_textField->convertToViewSpace(frame.origin());

	graphics->push();
	graphics->setScissor(frame);
	graphics->setFont(_textField->font());

	string str;
	if (_secureText) {
		CSStringBuilder strbuf;
		for (int i = 0; i < _text.length(); i++) strbuf.append("*");
		str = strbuf.toString();
	}
	else {
		str = _text;
	}

	if (str.length()) {
		graphics->setColor(_textColor);
	}
	else {
		str = _placeholder;

		graphics->setColor(_textColor * CSColor::Gray);
	}

	if (_maxLine > 1) {
		CSVector2 pos = graphics->stringPosition(CSGraphics::StringParam(str, 0, _cursor), frame.width);
		_scroll = 0;
		if (pos.y > frame.height) {
			_scroll = pos.y - frame.height;
		}
		graphics->drawStringScrolled(str, frame, _scroll);
		
		if (isFocused() && _cursorVisible) {
			pos += frame.origin();
			pos.y -= _scroll;
			graphics->setLineWidth(3);
			graphics->drawLine(CSVector2(pos.x, pos.y - graphics->font()->size()), pos);
		}
	}
	else {
		CSVector2 size;
		float x;
		switch (_textAlignment) {
			case CSTextAlignmentRight:
				size = graphics->stringSize(CSGraphics::StringParam(str, _cursor, str.length() - _cursor));
				_scroll = CSMath::max(size.x - frame.width, 0.0f);
				graphics->drawString(str, CSVector2(frame.right() + _scroll, frame.middle()), CSAlignRightMiddle);
				x = frame.right() - size.x + _scroll;
				break;
			case CSTextAlignmentCenter:
				size = graphics->stringSize(CSGraphics::StringParam(str, 0, _cursor));
				_scroll = CSMath::max(size.x - frame.width, 0.0f);
				graphics->drawString(str, CSVector2(frame.center() - _scroll * 0.5f, frame.middle()), CSAlignCenterMiddle);
				x = frame.center() + (size.x - _scroll) * 0.5f;
				break;
			default:
				size = graphics->stringSize(CSGraphics::StringParam(str, 0, _cursor));
				_scroll = CSMath::max(size.x - frame.width, 0.0f);
				graphics->drawString(str, CSVector2(frame.left() - _scroll, frame.middle()), CSAlignMiddle);
				x = frame.left() + size.x - _scroll;
				break;
		}

		if (isFocused() && _cursorVisible) {
			float y = frame.middle() - graphics->font()->size() * 0.5f;
			graphics->setLineWidth(3);
			graphics->drawLine(CSVector2(x, y), CSVector2(x, y + graphics->font()->size()));
		}
	}
	graphics->pop();
}

void CSTextFieldHandle::timeout() {
	double currentTime = CSTime::currentTime();

	bool cursorVisible = (currentTime - CSMath::floor(currentTime) < 0.5);

	if (cursorVisible != _cursorVisible) {
		_cursorVisible = cursorVisible;
		_textField->refresh();
	}
}

CSTextFieldHandleManager* CSTextFieldHandleManager::_instance = NULL;

CSTextFieldHandleManager::CSTextFieldHandleManager() : _focusedHandle(NULL) {

}

void CSTextFieldHandleManager::initialize() {
	if (!_instance) _instance = new CSTextFieldHandleManager();
}

void CSTextFieldHandleManager::finalize() {
	if (_instance) {
		delete _instance;
		_instance = NULL;
	}
}

void CSTextFieldHandleManager::addHandle(CSTextFieldHandle* handle) {
	_handles.addObject(handle);
}

void CSTextFieldHandleManager::removeHandle(CSTextFieldHandle* handle) {
	_handles.removeObjectIdenticalTo(handle);
}

bool CSTextFieldHandleManager::touchHandle(const CSVector2& p) {
	_focusedHandle = NULL;

	foreach (CSTextFieldHandle*, handle, &_handles) {
		if (handle->isAttached()) {
			CSVector2 lp = p;
			CSApplication::sharedApplication()->convertToLocalSpace(lp);
			CSRect frame = handle->textField()->bounds();
			handle->textField()->convertToViewSpace(frame.origin());

			if (frame.contains(lp)) {
				_focusedHandle = handle;
				handle->moveCursor(lp - frame.origin());
				break;
			}
		}
	}
	return _focusedHandle != NULL;
}

void CSTextFieldHandleManager::focusHandle(CSTextFieldHandle* handle) {
	_focusedHandle = handle;
}

void CSTextFieldHandleManager::unfocusHandle(CSTextFieldHandle* handle) {
	if (_focusedHandle == handle) _focusedHandle = NULL;
}

void CSTextFieldHandleManager::draw(CSGraphics* graphics) {
	foreach (CSTextFieldHandle*, handle, &_handles) {
		if (handle->isAttached()) handle->draw(graphics);
	}
}

#endif