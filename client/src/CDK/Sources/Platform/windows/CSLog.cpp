#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSLog.h"

#ifdef CS_CONSOLE_DEBUG

#include <stdio.h>
#include <windows.h>

#define CONSOLE_TEXT_ATTR_NORMAL	15
#define CONSOLE_TEXT_ATTR_WARN		6
#define CONSOLE_TEXT_ATTR_ERROR		4

void __CSLog(const char* tag, const char* format, ...) {
	va_list args;
	va_start(args, format);
	__CSLogv(tag, format, args);
	va_end(args);
}

void __CSWarnLog(const char* tag, const char* format, ...) {
	va_list args;
	va_start(args, format);
	__CSWarnLogv(tag, format, args);
	va_end(args);
}

void __CSErrorLog(const char* tag, const char* format, ...) {
	va_list args;
	va_start(args, format);
	__CSErrorLogv(tag, format, args);
	va_end(args);
}

void __CSLogv(const char* tag, const char* format, va_list args) {
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), CONSOLE_TEXT_ATTR_NORMAL);

	printf("%-28s ", tag);
	vprintf(format, args);
	printf("\n");
}

void __CSWarnLogv(const char* tag, const char* format, va_list args) {
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), CONSOLE_TEXT_ATTR_WARN);

	printf("%-28s ", tag);
	vprintf(format, args);
	printf("\n");
}

void __CSErrorLogv(const char* tag, const char* format, va_list args) {
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), CONSOLE_TEXT_ATTR_ERROR);

	printf("%-28s ", tag);
	vprintf(format, args);
	printf("\n");
}

#endif

#endif