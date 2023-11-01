#define CDK_IMPL

#include "CSShader.h"

#include "CSStringBuilder.h"

#include "CSGraphicsContext.h"

CSShader::CSShader(CSShaderType type, const char* source) : _type(type) {
    init(&source, 1);
}

CSShader::CSShader(CSShaderType type, const char* source0, const char* source1) : _type(type) {
    const char* sources[] = { source0, source1 };
    init(sources, 2);
}

CSShader::CSShader(CSShaderType type, const char* source0, const char* source1, const char* source2) : _type(type) {
    const char* sources[] = { source0, source1, source2 };
    init(sources, 3);
}

CSShader::CSShader(CSShaderType type, const char* source0, const char* source1, const char* source2, const char* source3) : _type(type) {
    const char* sources[] = { source0, source1, source2, source3 };
    init(sources, 4);
}

CSShader::~CSShader() {
    int object = _object;

    CSGraphicsContext::sharedContext()->invoke(false, [object](CSGraphicsApi* api) { api->deleteShader(object); });
}

void CSShader::init(const char* const* sources, int count) {
    CSGraphicsApi* api;
    if (CSGraphicsContext::sharedContext()->isRenderThread(&api)) {
        _object = api->createShader(_type, sources, count);
    }
    else {
        CSStringBuilder sourcebuf;
        for (int i = 0; i < count; i++) sourcebuf.append(sources[i]);
        string source = sourcebuf.toString();

        CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this, source](CSGraphicsApi* api) {
            const char* sources = source.cstring();
            _object = api->createShader(_type, &sources, 1);
        }, this);
        if (command) command->addFence(this, CSGBatchFlagReadWrite);
    }
}
