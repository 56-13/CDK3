#ifdef CDK_IMPL

#ifndef __CDK__CSRaster__
#define __CDK__CSRaster__

#include "CSVector2.h"

#include "CSFont.h"

class CSRaster {
public:
	static void loadFontHandle(const char* name, CSFontStyle style, const char* path);
    static void* createSystemFontHandle(float size, CSFontStyle style);
    static void* createFontHandle(const char* name, float size, CSFontStyle style);
    static void destroyFontHandle(void* handle);
    static CSVector2 characterSize(const uchar* cc, int cclen, const CSFont* font);
    static void* createRawWithCharacter(const uchar* cc, int cclen, const CSFont* font, int& width, int& height, float& offy);
    static void* createRawWithBitmap(const void* source, int len, int& width, int& height);
    static bool saveBitmapWithRaw(const char* path, const void* raw, int width, int height);
};

#endif

#endif
