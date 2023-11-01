#ifdef CDK_IMPL

#ifndef __CDK__CSFileRawImpl__
#define __CDK__CSFileRawImpl__

#include "CSTypes.h"

void* createRawWithCompressedContentOfFile(const char* path, int offset, int& length);
void* createRawWithContentOfFile(const char* path, int offset, int& length);
bool writeRawToFile(const char* path, const void* data, int length);
bool writeCompressedRawToFile(const char* path, const void* data, int length);

#endif

#endif
