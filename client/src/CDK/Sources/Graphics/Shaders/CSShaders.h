#ifndef __CDK__CSShaders__
#define __CDK__CSShaders__

#include "CSEmptyShader.h"
#include "CSBlitShader.h"
#include "CSBlurShader.h"
#include "CSLensShader.h"
#include "CSWaveShader.h"
#include "CSStrokeShader.h"
#include "CSSkyboxShader.h"
#include "CSPostProcessShader.h"
#include "CSTerrainShader.h"

class CSShaders {
public:
#ifdef CDK_IMPL
	static void initialize();
	static void finalize();
#endif
	static CSEmptyShader* empty();
	static CSBlitShader* blit();
	static CSBlurShader* blur();
	static CSLensShader* lens();
	static CSWaveShader* wave();
	static CSStrokeShader* stroke();
	static CSSkyboxShader* skybox();
	static CSPostProcessShader* postProcess();
	static CSTerrainShader* terrain();
};

#endif
