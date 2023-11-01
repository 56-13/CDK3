#ifdef CDK_WINDOWS

#ifdef CDK_IMPL

#ifndef __CDK__CSTextFieldImpl__
#define __CDK__CSTextFieldImpl__

#include "CSGraphics.h"

#include "CSTextField.h"

class CSTextFieldHandle {
private:
	CSTextField* _textField;
	CSColor _textColor;
	string _placeholder;
	int _maxLine;
	int _maxLength;
	bool _secureText;
	CSTextAlignment _textAlignment;
	string _text;
	int _cursor;
	float _scroll;
	bool _attach;
	bool _cursorVisible;
public:
	CSTextFieldHandle(CSTextField* textField);
	~CSTextFieldHandle();

	CSTextFieldHandle(const CSTextFieldHandle&) = delete;
	CSTextFieldHandle& operator=(const CSTextFieldHandle&) = delete;

	inline CSTextField* textField() {
		return _textField;
	}
	inline const CSTextField* textField() const {
		return _textField;
	}

	inline bool isAttached() const {
		return _attach;
	}
	void attach();
	void detach();

	inline int maxLine() const {
		return _maxLine;
	}
	inline void setMaxLine(int maxLine) {
		_maxLine = maxLine;
	}

	inline const string& text() {
		return _text;
	}
	void setText(const string& text);
	void clearText();

	inline int maxLength() const {
		return _maxLength;
	}
	void setMaxLength(int maxLength);

	inline const CSColor& textColor() const {
		return _textColor;
	}
	inline void setTextColor(const CSColor& textColor) {
		_textColor = textColor;
	}
	inline const string& placeholder() const {
		return _placeholder;
	}
	void setPlaceholder(const string& placeholder);

	inline CSTextAlignment textAlignment() const {
		return _textAlignment;
	}
	inline void setTextAlignment(CSTextAlignment textAlignment) {
		_textAlignment = textAlignment;
	}
	inline bool isSecureText() const {
		return _secureText;
	}
	inline void setSecureText(bool secureText) {
		_secureText = secureText;
	}
	bool isFocused() const;
	void setFocus(bool focused);

	void inputCharacter(uint codepoint);
	void moveLeftCursor(bool deleting);
	void moveRightCursor(bool deleting);
	void moveStartCursor();
	void moveEndCursor();
	void moveCursor(CSVector2 p);
	void complete();
	void draw(CSGraphics* graphics);
	void timeout();
};

class CSTextFieldHandleManager {
private:
	CSArray<CSTextFieldHandle*> _handles;
	CSTextFieldHandle* _focusedHandle;

	static CSTextFieldHandleManager* _instance;

	CSTextFieldHandleManager();
	~CSTextFieldHandleManager();
public:
	static void initialize();
	static void finalize();

	static inline CSTextFieldHandleManager* sharedManager() {
		return _instance;
	}

	CSTextFieldHandle* focusedHandle() {
		return _focusedHandle;
	}

	void addHandle(CSTextFieldHandle* handle);
	void removeHandle(CSTextFieldHandle* handle);
	bool touchHandle(const CSVector2& p);
	void focusHandle(CSTextFieldHandle* handle);
	void unfocusHandle(CSTextFieldHandle* handle);
	void draw(CSGraphics* graphics);
};

#endif

#endif

#endif
