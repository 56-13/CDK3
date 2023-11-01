#ifndef __CDK__CSDirectionalLight__
#define __CDK__CSDirectionalLight__

#include "CSColor3.h"

#include "CSShadow.h"

struct CSDirectionalLight {
	void* key;
	CSVector3 direction;
	CSColor3 color;
	bool castShadow;
	bool castShadow2D;
	CSShadow shadow;

	CSDirectionalLight() = default;
	CSDirectionalLight(void* key, const CSVector3& direction, const CSColor3& color, bool castShadow, bool castShadow2D, const CSShadow& shadow);
	CSDirectionalLight(void* key, CSBuffer* buffer);
	CSDirectionalLight(void* key, const byte*& bytes);

	uint hash() const;

	bool operator ==(const CSDirectionalLight& other) const;
	inline bool operator !=(const CSDirectionalLight& other) const {
		return !(*this == other);
	}
};

#endif