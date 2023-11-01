#define CDK_IMPL

#include "CSLogger.h"

#include <stdarg.h>

CSLogger::CSLogger(FILE* fp) : _fp(fp) {

}
CSLogger::~CSLogger() {
    fflush(_fp);
    fclose(_fp);
}
CSLogger* CSLogger::create(const char* path, bool append) {
    FILE* fp = fopen(path, append ? "a" : "w");
    
    return fp ? new CSLogger(fp) : NULL;
}

void CSLogger::print(const char* format, ...) {
    va_list args;
    va_start(args, format);
    vfprintf(_fp, format, args);
    fprintf(_fp, "\n");
    CSLogv(format, args);
    va_end(args);
}
void CSLogger::flush() {
    fflush(_fp);
}
