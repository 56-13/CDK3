#define CDK_IMPL

#include "CSShaderCode.h"

#include "CSStringBuilder.h"

#include "CSFile.h"

#include "CSGraphicsContext.h"

string CSShaderCode::Base;
string CSShaderCode::VSBase;
string CSShaderCode::FSBase;

void CSShaderCode::initialize() {
	CSStringBuilder strbuf;

	strbuf.appendFormat("#define MaxBoneCount %d\n", CSGraphicsContext::sharedContext()->maxUniformBlockSize() / 64);
	strbuf.append(string::contentOfFile(CSFile::bundlePath("graphics/base.glsl")));
	Base = strbuf.toString();
	strbuf.clear();

	strbuf.append(Base);
	strbuf.append(string::contentOfFile(CSFile::bundlePath("graphics/base_vs.glsl")));
	VSBase = strbuf.toString();
	strbuf.clear();

	strbuf.append(Base);
	strbuf.append(string::contentOfFile(CSFile::bundlePath("graphics/base_fs.glsl")));
	FSBase = strbuf.toString();
	strbuf.clear();
}

void CSShaderCode::finalize() {
	Base.clear();
	VSBase.clear();
	FSBase.clear();
}
