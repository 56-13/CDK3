#define CDK_IMPL

#include "CSShaders.h"

static CSLock __lock;
static CSEmptyShader* __empty = NULL;
static CSBlitShader* __blit = NULL;
static CSBlurShader* __blur = NULL;
static CSLensShader* __lens = NULL;
static CSWaveShader* __wave = NULL;
static CSStrokeShader* __stroke = NULL;
static CSSkyboxShader* __skybox = NULL;
static CSPostProcessShader* __postProcess = NULL;
static CSTerrainShader* __terrain = NULL;

void CSShaders::initialize() {

}

void CSShaders::finalize() {
	if (__empty) {
		delete __empty;
		__empty = NULL;
	}
	if (__blit) {
		delete __blit;
		__blit = NULL;
	}
	if (__blur) {
		delete __blur;
		__blur = NULL;
	}
	if (__lens) {
		delete __lens;
		__lens = NULL;
	}
	if (__wave) {
		delete __wave;
		__wave = NULL;
	}
	if (__stroke) {
		delete __stroke;
		__stroke = NULL;
	}
	if (__skybox) {
		delete __skybox;
		__skybox = NULL;
	}
	if (__postProcess) {
		delete __postProcess;
		__postProcess = NULL;
	}
	if (__terrain) {
		delete __terrain;
		__terrain = NULL;
	}
}

CSEmptyShader* CSShaders::empty() {
	if (!__empty) {
		synchronized(__lock) {
			if (!__empty) __empty = new CSEmptyShader();
		}
	}
	return __empty;
}

CSBlitShader* CSShaders::blit() {
	if (!__blit) {
		synchronized(__lock) {
			if (!__blit) __blit = new CSBlitShader();
		}
	}
	return __blit;
}

CSBlurShader* CSShaders::blur() {
	if (!__blur) {
		synchronized(__lock) {
			if (!__blur) __blur = new CSBlurShader();
		}
	}
	return __blur;
}

CSLensShader* CSShaders::lens() {
	if (!__lens) {
		synchronized(__lock) {
			if (!__lens) __lens = new CSLensShader();
		}
	}
	return __lens;
}

CSWaveShader* CSShaders::wave() {
	if (!__wave) {
		synchronized(__lock) {
			if (!__wave) __wave = new CSWaveShader();
		}
	}
	return __wave;
}

CSStrokeShader* CSShaders::stroke() {
	if (!__stroke) {
		synchronized(__lock) {
			if (!__stroke) __stroke = new CSStrokeShader();
		}
	}
	return __stroke;
}

CSSkyboxShader* CSShaders::skybox() {
	if (!__skybox) {
		synchronized(__lock) {
			if (!__skybox) __skybox = new CSSkyboxShader();
		}
	}
	return __skybox;
}

CSPostProcessShader* CSShaders::postProcess() {
	if (!__postProcess) {
		synchronized(__lock) {
			if (!__postProcess) __postProcess = new CSPostProcessShader();
		}
	}
	return __postProcess;
}


CSTerrainShader* CSShaders::terrain() {
	if (!__terrain) {
		synchronized(__lock) {
			if (!__terrain) __terrain = new CSTerrainShader();
		}
	}
	return __terrain;
}
