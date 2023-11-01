#define CDK_IMPL

#include "CSRenderers.h"

static CSLock __lock;
static CSStandardRenderer* __standard = NULL;
static CSShadowRenderer* __shadow = NULL;
static CSShadow2DRenderer* __shadow2D = NULL;
static CSDistortionRenderer* __distortion = NULL;

void CSRenderers::initialize() {

}

void CSRenderers::finalize() {
	CSObject::release(__standard);
	CSObject::release(__shadow);
	CSObject::release(__shadow2D);
	CSObject::release(__distortion);
}

CSStandardRenderer* CSRenderers::standard() {
	if (!__standard) {
		synchronized(__lock) {
			if (!__standard) __standard = new CSStandardRenderer();
		}
	}
	return __standard;
}

CSShadowRenderer* CSRenderers::shadow() {
	if (!__shadow) {
		synchronized(__lock) {
			if (!__shadow) __shadow = new CSShadowRenderer();
		}
	}
	return __shadow;
}

CSShadow2DRenderer* CSRenderers::shadow2D() {
	if (!__shadow2D) {
		synchronized(__lock) {
			if (!__shadow2D) __shadow2D = new CSShadow2DRenderer();
		}
	}
	return __shadow2D;
}

CSDistortionRenderer* CSRenderers::distortion() {
	if (!__distortion) {
		synchronized(__lock) {
			if (!__distortion) __distortion = new CSDistortionRenderer();
		}
	}
	return __distortion;
}
