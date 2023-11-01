#define CDK_IMPL

#include "CSUpdatePass.h"

#include "CSSceneObject.h"

bool CSUpdatePass::addPrecedence(const CSSceneObject* obj) {
	if (_pass < Max) {
		int pass = obj->getUpdatePass() + obj->located();
		if (pass > _pass) {
			_pass = pass;
			if (_pass < Max) return true;
			else _pass = Max;
		}
	}
	return false;
}
