#ifdef CDK_IMPL

#ifndef __CDK__CSTextFieldBridge__
#define __CDK__CSTextFieldBridge__

#include "CSTextField.h"

class CSTextFieldBridge {
public:
    static void* createHandle(CSTextField* textField);
    static void destroyHandle(void* handle);
    static void addToView(void* handle);
    static void removeFromView(void* handle);
    static void setFrame(void* handle, const CSRect& frame);
    static const char* text(void* handle);
    static void setText(void* handle, const char* text);
    static void clearText(void* handle);
    static CSColor textColor(void* handle);
    static void setTextColor(void* handle, const CSColor& textColor);
    static void setFont(void* handle, const CSFont* font);
    static const char* placeholder(void* handle);
    static void setPlaceholder(void* handle, const char* placeholder);
    static CSTextAlignment textAlignment(void* handle);
    static void setTextAlignment(void* handle, CSTextAlignment textAlignment);
    static CSReturnKeyType returnKeyType(void* handle);
    static void setReturnKeyType(void* handle, CSReturnKeyType returnKeyType);
    static CSKeyboardType keyboardType(void* handle);
    static void setKeyboardType(void* handle, CSKeyboardType keyboardType);
    static bool isSecureText(void* handle);
    static void setSecureText(void* handle, bool secureText);
    static int maxLength(void* handle);
    static void setMaxLength(void* handle, int maxLength);
	static int maxLine(void* handle);
	static void setMaxLine(void* handle, int maxLine);
	static bool isFocused(void* handle);
    static void setFocus(void* handle, bool focused);
};

#endif

#endif
