#ifndef __CDK__CSShaderCode__
#define __CDK__CSShaderCode__

#include "CSString.h"

class CSShaderCode {
public:
	static string Base;
	static string VSBase;
	static string FSBase;
#ifdef CDK_IMPL
	static void initialize();
	static void finalize();
#endif
};

#endif