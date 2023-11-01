#define CDK_IMPL

#include "CSGraphics.h"

CSGraphics::StringParam::StringParam(const string& str) {
	if (str) {
		display = str.content()->display();
		end = display->characters()->count();
	}
	else {
		display = NULL;
		start = 0;
		end = 0;
	}
}
CSGraphics::StringParam::StringParam(const string& str, int offset, int length) {
	if (str) {
		display = str.content()->display();
		if (length <= 0 || offset >= display->characters()->count()) {
			display = NULL;
			return;
		}
		start = offset;
		end = CSMath::min(offset + length, display->characters()->count());
	}
	else {
		display = NULL;
		start = 0;
		end = 0;
	}
}

CSGraphics::StringParam::StringParam(const CSLocaleString* str) : StringParam(str->value()) {
	
}
