#define CDK_IMPL

#include "CSStreamRenderCommand.h"

#include "CSGraphicsContext.h"
#include "CSGraphics.h"
#include "CSResourcePool.h"
#include "CSVertex.h"
#include "CSVertexArrays.h"
#include "CSShaders.h"

CSStreamRenderCommand::CSStreamRenderCommand(CSGraphics* graphics, CSPrimitiveMode mode) : CSStreamRenderCommand(graphics, mode, 4, 6) {

}

CSStreamRenderCommand::CSStreamRenderCommand(CSGraphics* graphics, CSPrimitiveMode mode, int vertexCapacity, int indexCapacity) :
	state(graphics->state()),
	world(graphics->world()),
	color(graphics->color()),
    _renderOrder(graphics->renderOrder()),
    _mode(mode),
	_vertexData(new CSGBufferData<CSFVertex>(vertexCapacity)),
	_indexData(new CSVertexIndexData(vertexCapacity, indexCapacity)),
    _bounds(NULL),
    _vertices(NULL)
{
	
}

CSStreamRenderCommand::~CSStreamRenderCommand() {
	_vertexData->release();
	_indexData->release();
    release(_bounds);
}

void CSStreamRenderCommand::addVertex(CSFVertex vertex) {
    CSVector3::transform(vertex.position, world, vertex.position);

    vertex.color = (CSColor)vertex.color * state.material.color * color;

    if (state.usingLight()) {
        vertex.normal = CSVector3::normalize(CSVector3::transformNormal(vertex.normal, world));

        if (state.usingVertexTangent()) {
            vertex.tangent = CSVector3::normalize(CSVector3::transformNormal(vertex.tangent, world));
        }
    }

    _vertexData->addObject(vertex);
}

void CSStreamRenderCommand::addIndex(int index) {
    _indexData->addObject(index);
}

bool CSStreamRenderCommand::submit() {
    state.validate();
    state.material.color = CSColor::White;       //pre appllied, reset for batching
    
    if (state.visible()) {
        CSVector3* points = (CSVector3*)alloca(_vertexData->count() * sizeof(CSVector3));
        for (int i = 0; i < _vertexData->count(); i++) points[i] = _vertexData->objectAtIndex(i).position;

        CSRect bounds;
        if (state.renderer->clip(state, points, _vertexData->count(), bounds)) {
            if (!bounds.fullScreen()) _bounds = new CSArray<CSRect>(&bounds, 1);
            return true;
        }
    }
    return false;
}

bool CSStreamRenderCommand::parallel(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const {
    bool result = state.material.batch(reads, writes);
    CSGBatchFlags flags = CSGBatchFlagWrite | CSGBatchFlagRetrieve;
    if (_renderOrder && (state.depthMode != CSDepthReadWrite || state.material.blendMode != CSBlendNone)) flags |= CSGBatchFlagRead;
    result &= state.target->batch(reads, writes, flags);
    return result;
}

bool CSStreamRenderCommand::findBatch(CSRenderCommand* command, CSRenderCommand*& candidate) const {
    CSStreamRenderCommand* other = dynamic_cast<CSStreamRenderCommand*>(command);

    if (other && other->state == state && other->_mode == _mode) {
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

void CSStreamRenderCommand::batch(CSRenderCommand* command) {
    CSStreamRenderCommand* other = static_assert_cast<CSStreamRenderCommand*>(command);
    foreach (int, i, _indexData) other->_indexData->addObject(i + other->_vertexData->count());
    other->_vertexData->addObjectsFromArray(_vertexData);
    if (!_bounds) release(other->_bounds);
    else if (other->_bounds) other->_bounds->addObjectsFromArray(_bounds);
}

class CSStreamRenderCommandKey : public CSObject {
private:
    bool _texCoord;
    bool _normal;
    bool _tangent;
    CSGBufferData<CSFVertex>* _vertexData;
    CSVertexIndexData* _indexData;
public:
    CSStreamRenderCommandKey(bool texCoord, bool normal, bool tangent, CSGBufferData<CSFVertex>* vertexData, CSVertexIndexData* indexData) :
        _texCoord(texCoord),
        _normal(normal),
        _tangent(tangent),
        _vertexData(retain(vertexData)),
        _indexData(retain(indexData))
    {
    }
private:
    ~CSStreamRenderCommandKey() {
        _vertexData->release();
        _indexData->release();
    }

    uint hash() const override {
        CSHash hash;
        hash.combine(_texCoord);
        hash.combine(_normal);
        hash.combine(_tangent);
        hash.combine(_vertexData);
        hash.combine(_indexData);
        return hash;
    }

    bool isEqual(const CSStreamRenderCommandKey* other) const {
        return _texCoord == other->_texCoord &&
            _normal == other->_normal &&
            _tangent == other->_tangent &&
            _vertexData->isEqual(other->_vertexData) &&
            _indexData->isEqual(other->_indexData);
    }

    bool isEqual(const CSObject* obj) const override {
        const CSStreamRenderCommandKey* other = dynamic_cast<const CSStreamRenderCommandKey*>(obj);

        return other && isEqual(other);
    }
};

static constexpr int BufferPositionAndColor = 0;
static constexpr int BufferTexCoord = 1;
static constexpr int BufferNormal = 2;
static constexpr int BufferTangent = 3;
static constexpr int BufferCount = 4;

static const CSVertexLayout __vertexLayouts[] = {
    CSVertexLayout(BufferPositionAndColor, CSRenderState::VertexAttribPosition, 3, CSVertexAttribTypeFloat, false, 20, 0, 0, true),
    CSVertexLayout(BufferPositionAndColor, CSRenderState::VertexAttribColor, 4, CSVertexAttribTypeHalfFloat, false, 20, 12, 0, true),
    CSVertexLayout(BufferTexCoord, CSRenderState::VertexAttribTexCoord, 2, CSVertexAttribTypeFloat, false, 8, 0, 0, false),
    CSVertexLayout(BufferNormal, CSRenderState::VertexAttribNormal, 3, CSVertexAttribTypeHalfFloat, false, 6, 0, 0, false),
    CSVertexLayout(BufferTangent, CSRenderState::VertexAttribTangent, 3, CSVertexAttribTypeHalfFloat, false, 6, 0, 0, false)
};

void CSStreamRenderCommand::render(CSGraphicsApi* api, bool background, bool foreground) {
    if (background) {
        _indexData->vertexCapacity = _vertexData->count();

        CSStreamRenderCommandKey* key = new CSStreamRenderCommandKey(state.usingMap(), state.usingVertexNormal(), state.usingVertexTangent(), _vertexData, _indexData);

        _vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

        if (!_vertices) {
            _vertices = CSVertexArrays::get(key, 1, true, BufferCount, true, __vertexLayouts, countof(__vertexLayouts));

            CSGBufferData<CSVertexC>* positionAndColors = new CSGBufferData<CSVertexC>(_vertexData->count());
            CSGBufferData<CSVector2>* texCoords = state.usingMap() ? new CSGBufferData<CSVector2>(_vertexData->count()) : NULL;
            CSGBufferData<CSHalf3>* normals = state.usingVertexNormal() ? new CSGBufferData<CSHalf3>(_vertexData->count()) : NULL;
            CSGBufferData<CSHalf3>* tangents = state.usingVertexTangent() ? new CSGBufferData<CSHalf3>(_vertexData->count()) : NULL;

            foreach (const CSFVertex&, vertex, _vertexData) {
                positionAndColors->addObject(CSVertexC(vertex.position, vertex.color));
                if (texCoords) texCoords->addObject(vertex.texCoord);
                if (normals) normals->addObject(vertex.normal);
                if (tangents) tangents->addObject(vertex.tangent);
            }
            _vertices->vertexBuffer(BufferPositionAndColor)->upload(positionAndColors, CSGBufferUsageHintDynamicDraw);
            if (texCoords) {
                _vertices->vertexBuffer(BufferTexCoord)->upload(texCoords, CSGBufferUsageHintDynamicDraw);
                texCoords->release();
            }
            if (normals) {
                _vertices->vertexBuffer(BufferNormal)->upload(normals, CSGBufferUsageHintDynamicDraw);
                normals->release();
            }
            if (tangents) {
                _vertices->vertexBuffer(BufferTangent)->upload(tangents, CSGBufferUsageHintDynamicDraw);
                tangents->release();
            }
            _vertices->indexBuffer()->upload(_indexData, CSGBufferUsageHintDynamicDraw);
        }

        key->release();
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

            CSShaders::stroke()->setup(api, state, _mode, NULL, false);

            _vertices->bind(api);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribPosition, true);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribColor, false);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTexCoord, state.material.colorMap);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribNormal, false);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTangent, false);
            _vertices->drawElements(api, _mode);
            _vertices->unbind(api);
        }
        if (foreground) {
            if (state.target->bloomSupported()) state.target->setDrawBuffers(api, 2, 0, 1);
            else state.target->setDrawBuffer(api, 0);

            api->applyBlendMode(state.material.blendMode);

            state.renderer->setup(api, state, _mode, NULL, false, true);

            _vertices->bind(api);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribPosition, true);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribColor, true);
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTexCoord, state.usingMap());
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribNormal, state.usingVertexNormal());
            _vertices->setAttribEnabled(api, CSRenderState::VertexAttribTangent, state.usingVertexTangent());
            _vertices->drawElements(api, _mode);
            _vertices->unbind(api);

            _vertices = NULL;
        }
    }
}
