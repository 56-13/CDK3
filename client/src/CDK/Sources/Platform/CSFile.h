#ifndef __CDK__CSFile__
#define __CDK__CSFile__

#include "CSString.h"
#include "CSArray.h"

class CSFile {
public:
#ifdef CDK_IMPL
#ifdef CDK_WINDOWS
	static int initialize();
	static void finalize();
#elif defined(CDK_ANDROID)
	static void initialize(void* assetManager, const char* storagePath);
	static void finalize();
#endif
    static void* createRawWithCompressedData(const void* src, int srcLength, int destOffset, int& destLength);
    static void* createCompressedRawWithData(const void* src, int srcLength, int& destLength);

    static void* createRawWithContentOfFile(const char* path, int offset, int& length, bool compression);
    static bool writeRawToFile(const char* path, const void* data, int length, bool compression);
#endif
    static const char* findPath(const char* subpath);
    static const char* storagePath(const char* subpath);
    static const char* bundlePath(const char* subpath);
    static bool fileExists(const char* path);
    static bool moveFile(const char* srcpath, const char* destpath);
    static bool deleteFile(const char* path);
    static bool makeDirectory(const char* path);
    static CSArray<string>* filePaths(const char* dirpath, bool full);
};

#endif
