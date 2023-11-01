#ifndef __CDK__CSProgram__
#define __CDK__CSProgram__

#include "CSShader.h"

class CSGraphicsApi;

class CSProgram : public CSGraphicsResource {
private:
	int _object = 0;
public:
	CSProgram();
private:
	~CSProgram();
public:
	static inline CSProgram* program() {
		return autorelease(new CSProgram());
	}

	inline CSResourceType resourceType() const override {
		return CSResourceTypeProgram;
	}
	inline int resourceCost() const override {
		return sizeof(CSProgram);
	}
	inline int object() const {
		return _object;
	}
	void attach(const CSShader* shader);
	void detach(const CSShader* shader);
	void link();
	void use(CSGraphicsApi* api);
};

#endif
