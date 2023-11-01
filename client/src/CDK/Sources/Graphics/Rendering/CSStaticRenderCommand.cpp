#define CDK_IMPL

#include "CSStaticRenderCommand.h"

#include "CSGraphicsContext.h"
#include "CSGraphics.h"
#include "CSResourcePool.h"
#include "CSGBuffers.h"
#include "CSVertexArrays.h"
#include "CSShaders.h"

CSStaticRenderCommand::Instance::Instance(const CSMatrix& model, const CSHalf4& color, int boneOffset) : model(model), color(color), boneOffset(boneOffset) {

}

uint CSStaticRenderCommand::Instance::hash() const {
	CSHash hash;
	hash.combine(model);
	hash.combine(color);
	hash.combine(boneOffset);
	return hash;
}

CSStaticRenderCommand::CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, CSPrimitiveMode mode, int instanceCapacity, const CSABoundingBox* aabb) :
	CSStaticRenderCommand(graphics, vertices, NULL, 0, mode, 0, vertices->indexBuffer()->count(), instanceCapacity, aabb)
{

}

CSStaticRenderCommand::CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int instanceCapacity, const CSABoundingBox* aabb) :
	CSStaticRenderCommand(graphics, vertices, boneBuffer, boneOffset, mode, 0, vertices->indexBuffer()->count(), instanceCapacity, aabb)
{

}

CSStaticRenderCommand::CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity, const CSABoundingBox* aabb) :
	CSStaticRenderCommand(graphics, vertices, NULL, 0, mode, indexOffset, indexCount, instanceCapacity, aabb)
{

}

CSStaticRenderCommand::CSStaticRenderCommand(CSGraphics* graphics, CSVertexArray* vertices, const CSGBuffer* boneBuffer, int boneOffset, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCapacity, const CSABoundingBox* aabb) :
    state(graphics->state()),
	world(graphics->world()),
	color(graphics->color()),
	_renderOrder(graphics->renderOrder()),
	_vertices(retain(vertices)),
	_boneBuffer(retain(boneBuffer)),
	_boneOffset(boneOffset),
	_mode(mode),
	_indexOffset(indexOffset),
	_indexCount(indexCount),
	_aabb(aabb ? new CSABoundingBox(*aabb) : NULL),
	_instanceData(new CSGBufferData<Instance>(instanceCapacity)),
	_bounds(aabb ? new CSArray<CSRect>(instanceCapacity) : NULL),
    _instanceBuffer(NULL)
{

}

CSStaticRenderCommand::~CSStaticRenderCommand() {
    release(_vertices);
    release(_boneBuffer);
    if (_aabb) delete _aabb;
    _instanceData->release();
    release(_bounds);
}

void CSStaticRenderCommand::addInstance(const CSVertexArrayInstance& i) {
	if (_aabb) {
		CSVector3 points[8];
		_aabb->getCorners(points);
		CSMatrix m = i.model * world;
		for (int i = 0; i < 8; i++) CSVector3::transform(points[i], m, points[i]);

		CSRect bounds;
		if (state.renderer->clip(state, points, 8, bounds)) {
			_instanceData->addObject(Instance(m, i.color * state.material.color * color, _boneOffset));
			if (bounds.fullScreen()) release(_bounds);
			else if (_bounds) _bounds->addObject(bounds);
		}
	}
	else {
		_instanceData->addObject(Instance(i.model * world, i.color * state.material.color * color, _boneOffset));
	}
}

bool CSStaticRenderCommand::submit() {
    state.validate();
    state.material.color = CSColor::White;       //pre appllied, reset for batching

    return state.visible() && _instanceData->count() != 0;
}

bool CSStaticRenderCommand::parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const {
    bool result = _vertices->batch(reads, writes, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);        //instanceBuffer를 attach/detach 하므로 write도 필요하다.
    if (_boneBuffer) result &= _boneBuffer->batch(reads, writes, CSGBatchFlagRead);
    result &= state.material.batch(reads, writes);
    CSGBatchFlags flags = CSGBatchFlagWrite | CSGBatchFlagRetrieve;
    if (_renderOrder && (state.depthMode != CSDepthReadWrite || state.material.blendMode != CSBlendNone)) flags |= CSGBatchFlagRead;
    result &= state.target->batch(reads, writes, flags);
    return result;
}

bool CSStaticRenderCommand::findBatch(CSRenderCommand* command, CSRenderCommand*& candidate) const {
    CSStaticRenderCommand* other = dynamic_cast<CSStaticRenderCommand*>(command);

    if (other &&
        other->state == state &&
        other->_mode == _mode &&
        other->_vertices == _vertices &&
        other->_indexOffset == _indexOffset &&
        other->_indexCount == _indexCount &&
        other->_boneBuffer == _boneBuffer) 
    {
        candidate = command;
        return true;
    }

    if (_bounds && command->bounds() && state.target == command->target()) {
        foreach (const CSRect&, a, command->bounds()) {
            foreach (const CSRect&, b, _bounds) {
                if (a.intersects(b)) return false;
            }
        }
        return true;
    }
    return false;
}

void CSStaticRenderCommand::batch(CSRenderCommand* command) {
    CSStaticRenderCommand* other = static_assert_cast<CSStaticRenderCommand*>(command);
    other->_instanceData->addObjectsFromArray(_instanceData);
    if (!_bounds) release(other->_bounds);
    else if (other->_bounds) other->_bounds->addObjectsFromArray(_bounds);
}

void CSStaticRenderCommand::render(CSGraphicsApi* api, bool background, bool foreground) {
    if (background) {
        _instanceBuffer = CSGBuffers::fromData(_instanceData, CSGBufferTargetArray);
        _vertices->bind(api);
        attachVertexInstances(api);
        _vertices->unbind(api);
    }

    if (foreground || state.usingStroke()) {
        state.target->focus(api);

        api->applyCullMode(state.material.cullMode);
        api->applyDepthMode(state.material.depthTest ? state.depthMode : CSDepthNone);
        api->applyStencilMode(state.stencilMode, state.stencilDepth);
        api->applyPolygonMode(state.polygonMode);
        api->applyScissor(state.scissor);
        api->applyLineWidth(state.lineWidth);

        if (background && state.usingStroke()) {
            state.target->setDrawBuffer(api, 0);

            CSShaders::stroke()->setup(api, state, _mode, _boneBuffer, true);

            _vertices->bind(api);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribPosition, true);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribColor, false);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTexCoord, state.material.colorMap);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribNormal, false);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTangent, false);
            _vertices->drawElementsInstanced(api, _mode, _indexOffset, _indexCount, _instanceBuffer->count());
            _vertices->unbind(api);
        }
        if (foreground) {
            if (state.target->bloomSupported()) state.target->setDrawBuffers(api, 2, 0, 1);
            else state.target->setDrawBuffer(api, 0);

            api->applyBlendMode(state.material.blendMode);

            bool vertexColor = _vertices->hasAttrib(api, CSRenderState::VertexAttribColor);

            state.renderer->setup(api, state, _mode, _boneBuffer, true, vertexColor);

            _vertices->bind(api);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribPosition, true);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribColor, vertexColor);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTexCoord, state.usingMap());
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribNormal, state.usingVertexNormal());
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTangent, state.usingVertexTangent());
            _vertices->drawElementsInstanced(api, _mode, _indexOffset, _indexCount, _instanceBuffer->count());
            detachVertexInstances(api);
            _vertices->unbind(api);

            _instanceBuffer = NULL;
        }
    }
}

void CSStaticRenderCommand::attachVertexInstances(CSGraphicsApi* api) {
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceModel0, 4, CSVertexAttribTypeFloat, false, 76, 0, 1, true));
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceModel1, 4, CSVertexAttribTypeFloat, false, 76, 16, 1, true));
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceModel2, 4, CSVertexAttribTypeFloat, false, 76, 32, 1, true));
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceModel3, 4, CSVertexAttribTypeFloat, false, 76, 48, 1, true));
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceColor, 4, CSVertexAttribTypeHalfFloat, false, 76, 64, 1, true));
    _vertices->attachLayout(api, CSVertexLayout(_instanceBuffer, CSRenderState::VertexAttribInstanceBoneOffset, 1, CSVertexAttribTypeUnsignedInt, false, 76, 72, 1, _boneBuffer != NULL));
}

void CSStaticRenderCommand::detachVertexInstances(CSGraphicsApi* api) {
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceBoneOffset);
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceColor);
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceModel3);
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceModel2);
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceModel1);
    _vertices->detachLayout(api, CSRenderState::VertexAttribInstanceModel0);
}
