#define CDK_IMPL

#include "CSGraphics.h"

#include "CSGraphicsContext.h"

#include "CSRenderers.h"
#include "CSShaders.h"

#include "CSDelegateRenderCommand.h"
#include "CSStaticRenderCommand.h"

static const CSFont* __defaultFont = NULL;          //TODO:finalize:setDefaultFont(NULL)

CSGraphics::State::State(CSRenderTarget* target, State* prev) :
	worldReserved(NULL),
	colorReserved(NULL),
    font(NULL),
    usingStencil(false),
    mark(false),
    prev(prev)
{
    reset(target);
}

CSGraphics::State::~State() {
    CSAssert(!usingStencil);
	release(worldReserved);
	release(colorReserved);
    release(font);
}

void CSGraphics::State::reset(CSRenderTarget* target) {
    CSAssert(!usingStencil);
	release(worldReserved);
	release(colorReserved);
	
	if (prev) {
        world = prev->world;
        color = prev->color;
        retain(font, prev->font);
        memcpy(fontColors, prev->fontColors, sizeof(fontColors));
        renderOrder = prev->renderOrder;
        batch = prev->batch;
    }
    else {
        world = CSMatrix::Identity;
        color = CSColor::White;
        retain(font, __defaultFont);
        fontColors[0] = fontColors[1] = fontColors[2] = fontColors[3] = CSColor::White;
        stencilBounds = CSRect::ScreenNone;
        renderOrder = true;
        batch.renderer = CSRenderers::standard();
        batch.target = target;
        batch.camera = CSCamera(0, target->width(), target->height(), 10, 10000);
        batch.material = CSMaterial::Default;
        batch.fogColor = CSColor3::Black;
        batch.fogNear = 0;
        batch.fogFar = 0;
        batch.bloomThreshold = 1;
        batch.brightness = 0;
        batch.contrast = 1;
        batch.saturation = 1;
        batch.polygonMode = CSPolygonFill;
        batch.depthMode = CSDepthNone;
        batch.stencilMode = CSStencilNone;
        batch.stencilDepth = 0;
        batch.layer = 0;
        batch.strokeWidth = 0;
        batch.strokeMode = CSStrokeNormal;
        batch.strokeColor = CSColor::Black;
        batch.lineWidth = 1;
        batch.scissor = CSRect::Zero;
        batch.lightSpaceState = NULL;
        batch.rendererParam = NULL;
    }
}

CSGraphics::CSGraphics(CSRenderQueue* queue, CSRenderTarget* target) :
    _queue(retain(queue)),
    _target(retain(target)),
    _state(new State(target, NULL))
{
	
}

CSGraphics::CSGraphics(CSRenderTarget* target) :
    _queue(new CSRenderQueue()),
    _target(retain(target)),
    _state(new State(target, NULL))
{

}

CSGraphics::~CSGraphics() {
    _queue->release();
    _target->release();
    
    State* current = _state;
    while (current) {
        State* prev = current->prev;
        delete current;
        current = prev;
    }
    CSGraphicsContext::sharedContext()->removeGraphics(this);
}

void CSGraphics::clear(const CSColor& color) {
#ifdef CS_ASSERT_DEBUG
    State* state = _state;
    do {
        CSAssert(!state->usingStencil);
        state = state->prev;
    } while (state);
#endif
    CSRenderTarget* target = _state->batch.target;
    CSRect scissor = _state->batch.scissor;
    CSDelegateRenderCommand* command = this->command([target, color = CSColor(color), scissor](CSGraphicsApi* api) {
        target->focus(api);
        target->setDrawBuffer(api, 0);
        api->applyScissor(scissor);
        api->clear(color);
    });
    command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::clearStencil() {
    if (_state->usingStencil) {
        if (_state->stencilBounds.onScreen()) {
            CSRenderTarget* target = _state->batch.target;
            CSRect bounds = _state->stencilBounds;
            int depth = _state->batch.stencilDepth;

            CSDelegateRenderCommand* command = this->command([target, bounds, depth](CSGraphicsApi* api) {
                target->focus(api);
                api->clearStencil(bounds, depth);
            });
            command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
        }
        _state->batch.stencilDepth--;
        _state->stencilBounds = CSRect::ScreenNone;
        _state->usingStencil = false;
    }
}

void CSGraphics::reset() {
	clearStencil();

	_state->reset(_target);
}

void CSGraphics::push(bool mark) {
	_state->mark = mark;
	_state = new State(_target, _state);
}

void CSGraphics::pop(bool mark) {
	if (mark) {
		while (_state->prev && !_state->prev->mark) {
			clearStencil();

			State* prev = _state->prev;
			delete _state;
			_state = prev;
		}
	}
	if (_state->prev) {
		if (_state->prev->mark == mark) {
			clearStencil();

			State* prev = _state->prev;
			delete _state;
			_state = prev;
		}
	}
	else {
		clearStencil();

		_state->reset(_target);
	}
}

int CSGraphics::pushCount() const {
	int count = 0;
	State* current = _state;
	while (current->prev) {
		count++;
		current = current->prev;
	}
	return count;
}

void CSGraphics::pushTransform() {
	if (!_state->worldReserved) _state->worldReserved = new CSArray<CSMatrix>(4);
	_state->worldReserved->addObject(_state->world);
}

void CSGraphics::peekTransform(bool pop) {
	if (_state->worldReserved && _state->worldReserved->count()) {
		_state->world = _state->worldReserved->lastObject();
		if (pop) _state->worldReserved->removeLastObject();
	}
	else if (_state->prev) _state->world = _state->prev->world;
	else _state->world = CSMatrix::Identity;
}

void CSGraphics::setColor(const CSColor& color, bool inherit) {
    _state->color = color;
    
    if (inherit) {
		if (_state->colorReserved && _state->colorReserved->count()) _state->color *= _state->colorReserved->lastObject();
		else if (_state->prev) _state->color *= _state->prev->color;
    }
}

void CSGraphics::pushColor() {
	if (!_state->colorReserved) _state->colorReserved = new CSArray<CSColor>(4);
	_state->colorReserved->addObject(_state->color);
}

void CSGraphics::peekColor(bool pop) {
	if (_state->colorReserved && _state->colorReserved->count()) {
		_state->color = _state->colorReserved->lastObject();
		if (pop) _state->colorReserved->removeLastObject();
	}
	else if (_state->prev) _state->color = _state->prev->color;
	else _state->color = CSColor::White;
}

void CSGraphics::setBrightness(float brightness, bool inherit) {
    if (_state->prev && inherit) {
        brightness *= _state->prev->batch.brightness;
    }
    _state->batch.brightness = brightness;
}

void CSGraphics::setContrast(float contrast, bool inherit) {
    if (_state->prev && inherit) {
        contrast *= _state->prev->batch.contrast;
    }
    _state->batch.contrast = contrast;
}

void CSGraphics::setSaturation(float saturation, bool inherit) {
    if (_state->prev && inherit) {
        saturation *= _state->prev->batch.saturation;
    }
    _state->batch.saturation = saturation;
}


void CSGraphics::setStrokeColor(const CSColor &strokeColor) {
    _state->batch.strokeColor = strokeColor;
}

void CSGraphics::setDefaultFont(const CSFont* font) {
    retain(__defaultFont, font);
}

const CSFont* CSGraphics::defaultFont() {
    return __defaultFont;
}

void CSGraphics::setStencilMode(CSStencilMode mode) {
    _state->batch.stencilMode = mode;
    if (mode == CSStencilInclusive && !_state->usingStencil) {
        CSAssert(_state->batch.stencilDepth < 255, "too many stencil steps");
        _state->batch.stencilDepth++;
        _state->usingStencil = true;
    }
}

CSRect CSGraphics::convertToTargetSpace(const CSRect& rect) const {
    const CSBounds2& viewport = _state->batch.target->viewport();
    float xrate = viewport.width / _state->batch.camera.width();
    float yrate = viewport.height / _state->batch.camera.height();
    
    return CSRect(
        rect.x * xrate + viewport.x,
        rect.y * yrate + viewport.y,
        rect.width * xrate,
        rect.height * yrate);
}

void CSGraphics::setScissor(const CSRect& scissor) {
    if (_state->batch.scissor != CSRect::Zero) {
        CSRect tscissor = convertToTargetSpace(scissor);
        if (_state->prev && _state->prev->batch.scissor != CSRect::Zero) {
            tscissor.intersect(_state->prev->batch.scissor);
        }
        _state->batch.scissor = tscissor;
    }
    else _state->batch.scissor = CSRect::Zero;
}

void CSGraphics::drawVertices(CSVertexArray* vertices, CSPrimitiveMode mode, const CSABoundingBox* aabb, const CSArray<CSVertexArrayInstance>* instances) {
    drawVertices(vertices, NULL, 0, mode, 0, vertices->indexBuffer()->count(), aabb, instances);
}

void CSGraphics::drawVertices(CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount, const CSABoundingBox* aabb, const CSArray<CSVertexArrayInstance>* instances) {
    drawVertices(vertices, NULL, 0, mode, indexOffset, indexCount, aabb, instances);
}

void CSGraphics::drawVertices(CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, const CSABoundingBox* aabb, const CSArray<CSVertexArrayInstance>* instances) {
    drawVertices(vertices, boneBuffer, boneOffset, mode, 0, vertices->indexBuffer()->count(), aabb, instances);
}

void CSGraphics::drawVertices(CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int indexOffset, int indexCount, const CSABoundingBox* aabb, const CSArray<CSVertexArrayInstance>* instances) {
    CSStaticRenderCommand* command = new CSStaticRenderCommand(this, vertices, boneBuffer, boneOffset, mode, indexOffset, indexCount, instances ? instances->count() : 1, aabb);

    if (instances) {
        foreach (const CSVertexArrayInstance&, i, instances) command->addInstance(i);
    }
    else command->addInstance(CSVertexArrayInstance::Origin);

    this->command(command);

    command->release();
}

void CSGraphics::drawSkybox(const CSTexture* skybox) {
    CSRenderTarget* target = _state->batch.target;
    CSCamera camera = _state->batch.camera;

    CSDelegateRenderCommand* command = this->command([target, camera, skybox](CSGraphicsApi* api) {
        target->focus(api);

        CSShaders::skybox()->draw(api, camera, skybox);
    });
    command->addFence(skybox, CSGBatchFlagRead);
    command->addFence(target, CSGBatchFlagWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::drawSkybox() {
    if (_state->batch.lightSpaceState) {
        const CSTexture* texture = _state->batch.lightSpaceState->envMap;

        if (texture) drawSkybox(texture);
    }
}

void CSGraphics::command(CSRenderCommand* command) {
    CSAssert(CSGraphicsContext::sharedContext()->currentGraphics() == this);

    if (_queue->frame()->command(command)) {
        if (_state->batch.stencilMode != CSStencilNone) {
            State* state = _state;
            while (!state->usingStencil) {
                state = state->prev;
                break;
            }
            if (command->bounds()) {
                foreach (const CSRect&, bounds, command->bounds()) state->stencilBounds.append(bounds);
            }
            else state->stencilBounds = CSRect::ScreenFull;
        }
    }
}

CSDelegateRenderCommand* CSGraphics::command(const std::function<void(CSGraphicsApi*)>& inv, const CSObject* retain0, const CSObject* retain1, const CSObject* retain2) {
    CSDelegateRenderCommand* command = new CSDelegateRenderCommand(inv, retain0, retain1, retain2);
    this->command(command);
    command->release();
    return command;
}

void CSGraphics::focus() {
    CSGraphicsContext::sharedContext()->attachGraphics(this);
}

void CSGraphics::unfocus() {
    CSGraphicsContext::sharedContext()->detachGraphics(this);
}

void CSGraphics::render() {
    CSGraphicsContext::sharedContext()->detachGraphics(this);

    _queue->render();
}

void CSGraphics::applyAlign(CSVector3& point, const CSVector2& size, CSAlign align) {
    if (align & CSAlignCenter) point.x -= size.x * 0.5f;
    else if (align & CSAlignRight) point.x -= size.x;
    if (align & CSAlignMiddle) point.y -= size.y * 0.5f;
    else if (align & CSAlignBottom) point.y -= size.y;
}

void CSGraphics::applyAlign(CSVector2& point, const CSVector2& size, CSAlign align) {
    if (align & CSAlignCenter) point.x -= size.x * 0.5f;
    else if (align & CSAlignRight) point.x -= size.x;
    if (align & CSAlignMiddle) point.y -= size.y * 0.5f;
    else if (align & CSAlignBottom) point.y -= size.y;
}

float CSGraphics::radianDistance(float radius) {
    return radius > 1 ? CSMath::acos((radius - 1) / radius) : FloatPiOverTwo;
}
