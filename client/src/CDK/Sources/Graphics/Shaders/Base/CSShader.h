#ifndef __CDK__CSShader__
#define __CDK__CSShader__

#include "CSGraphicsResource.h"

#include "CSGEnums.h"

class CSShader : public CSGraphicsResource {
private:
	CSShaderType _type;
	int _object = 0;
public:
	CSShader(CSShaderType type, const char* source);
	CSShader(CSShaderType type, const char* source0, const char* source1);
	CSShader(CSShaderType type, const char* source0, const char* source1, const char* source2);
	CSShader(CSShaderType type, const char* source0, const char* source1, const char* source2, const char* source3);
private:
	~CSShader();

	void init(const char* const* sources, int count);
public:
	static inline CSShader* shader(CSShaderType type, const char* source) {
		return autorelease(new CSShader(type, source));
	}
	static inline CSShader* shader(CSShaderType type, const char* source0, const char* source1) {
		return autorelease(new CSShader(type, source0, source1));
	}
	static inline CSShader* shader(CSShaderType type, const char* source0, const char* source1, const char* source2) {
		return autorelease(new CSShader(type, source0, source1, source2));
	}
	static inline CSShader* shader(CSShaderType type, const char* source0, const char* source1, const char* source2, const char* source3) {
		return autorelease(new CSShader(type, source0, source1, source2, source3));
	}

	inline CSResourceType resourceType() const override {
		return CSResourceTypeShader;
	}
	inline int resourceCost() const override {
		return sizeof(CSShader);
	}
	inline CSShaderType type() const {
		return _type;
	}
	inline int object() const {
		return _object;
	}
};

#endif
