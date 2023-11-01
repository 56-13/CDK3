#ifdef CDK_IMPL

#ifndef __CDK__CSApplicationBridge__
#define __CDK__CSApplicationBridge__

#include "CSApplication.h"

class CSApplicationBridge {
public:
	static void setVersion(const char* version);
	static CSRect frame();
	static CSEdgeInsets edgeInsets();
	static void setResolution(CSResolution resolution);
	static CSResolution resolution();
	static void draw(CSGraphics* graphics);
	static void swapBuffer();
	static void openURL(const char* url);
	static void setClipboard(const char* text);
	static const char* clipboard();
	static void shareUrl(const char* title, const char* message, const char* url);
	static void finish();
};

#endif

#endif