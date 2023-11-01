#define CDK_IMPL

#include "CSGraphics.h"

#include "CSGBuffers.h"
#include "CSShaderCode.h"
#include "CSShaders.h"
#include "CSRenderers.h"
#include "CSGlyphs.h"

void CSGraphics::initialize(CSGraphicsPlatform platform) {
	CSGraphicsContext::initialize(platform);
	CSResourcePool::initialize();
	CSGBuffers::initialize();
	CSShaderCode::initialize();
	CSShaders::initialize();
	CSRenderers::initialize();
	CSGlyphs::initialize();
}

void CSGraphics::finalize() {
	setDefaultFont(NULL);
	CSGlyphs::finalize();
	CSRenderers::finalize();
	CSShaders::finalize();
	CSShaderCode::finalize();
	CSGBuffers::finalize();
	CSResourcePool::finalize();
	CSGraphicsContext::finalize();
}