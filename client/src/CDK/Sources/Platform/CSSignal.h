#ifndef __CDK__CSSignal__
#define __CDK__CSSignal__

#include "CSString.h"

class CSSignal {
public:
#ifdef CDK_IMPL
    static void initialize();
    static void finalize();
#endif
    static const char* const FileName;
    static const char* filePath();
    static string readLog();
    static void deleteLog();
};

#endif
