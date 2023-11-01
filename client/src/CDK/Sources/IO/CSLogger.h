#ifndef __CDK__CSLogger__
#define __CDK__CSLogger__

#include "CSObject.h"

#include <stdio.h>

class CSLogger : public CSObject {
private:
    FILE* _fp;
private:
    CSLogger(FILE* fp);
    ~CSLogger();
public:
    static CSLogger* create(const char* path, bool append = false);
    static inline CSLogger* logger(const char* path, bool append) {
        return autorelease(create(path, append));
    }
    
    void print(const char* format, ...);
    void flush();
};

#endif
