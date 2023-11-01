#define CDK_IMPL

#include "CSProgram.h"

#include "CSGraphicsContext.h"

CSProgram::CSProgram() {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) { _object = api->createProgram(); }, this);
	if (command) command->addFence(this, CSGBatchFlagReadWrite);
}

CSProgram::~CSProgram() {
	int object = _object;

	CSGraphicsContext::sharedContext()->invoke(false, [object](CSGraphicsApi* api) { api->deleteProgram(object); });
}

void CSProgram::attach(const CSShader* shader) {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, shader](CSGraphicsApi* api) {
		api->attachShader(_object, shader->object()); 
	}, this, shader);
	if (command) {
		command->addFence(shader, CSGBatchFlagRead);
		command->addFence(this, CSGBatchFlagWrite);
	}
}

void CSProgram::detach(const CSShader* shader) {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, shader](CSGraphicsApi* api) {
		api->detachShader(_object, shader->object());
	}, this, shader);
	if (command) {
		command->addFence(shader, CSGBatchFlagRead);
		command->addFence(this, CSGBatchFlagWrite);
	}
}

void CSProgram::link() {
	CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) { api->linkProgram(_object); }, this);
	if (command) {
		command->addFence(this, CSGBatchFlagReadWrite);
		
		flush();
	}
}

void CSProgram::use(CSGraphicsApi* api) {
	api->useProgram(_object);
}
