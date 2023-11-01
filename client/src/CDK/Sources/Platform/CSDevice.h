#ifndef __CDK__CSDevice__
#define __CDK__CSDevice__

#include "CSString.h"

struct CSMemoryUsage {
    int64 totalMemory;
    int64 freeMemory;
    int64 threshold;
};

class CSDevice {
public:
    static const char* brand();
    static const char* model();
    static const char* systemVersion();
    static const char* locale();
    static const char* countryCode();
	static const char* uuid();
    static CSMemoryUsage memoryUsage();
    static int battery();
};

#endif
