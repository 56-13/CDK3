#ifndef __CDK__CSRenderers__
#define __CDK__CSRenderers__

#include "CSStandardRenderer.h"
#include "CSShadowRenderer.h"
#include "CSShadow2DRenderer.h"
#include "CSDistortionRenderer.h"

class CSRenderers {
public:
#ifdef CDK_IMPL
	static void initialize();
	static void finalize();
#endif
	static CSStandardRenderer* standard();
	static CSShadowRenderer* shadow();
	static CSShadow2DRenderer* shadow2D();
	static CSDistortionRenderer* distortion();
};

#endif
