#ifndef __CDK__CSAnimationColorChannel__
#define __CDK__CSAnimationColorChannel__

#include "CSAnimationColor.h"

#include "CSAnimationFloat.h"

class CSAnimationColorChannel : public CSAnimationColor {
public:
	CSPtr<CSAnimationFloat> red;
	CSPtr<CSAnimationFloat> green;
	CSPtr<CSAnimationFloat> blue;
	CSPtr<CSAnimationFloat> alpha;
	bool normalized = true;
	bool fixedChannel = false;

	CSAnimationColorChannel() = default;
	CSAnimationColorChannel(const CSAnimationColorChannel* other);
	explicit CSAnimationColorChannel(CSBuffer* buffer);

	static inline CSAnimationColorChannel* color() {
		return autorelease(new CSAnimationColorChannel());
	}

	inline Type type() const override {
		return TypeChannel;
	}
	int resourceCost() const override;
	CSColor value(float t, const CSColor& r, const CSColor& default) const override;
};

#endif