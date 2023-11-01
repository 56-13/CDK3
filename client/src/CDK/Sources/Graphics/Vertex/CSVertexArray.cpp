#define CDK_IMPL

#include "CSVertexArray.h"

#include "CSGraphicsContext.h"

CSVertexArray::CSVertexArray(int vertexBufferCount, bool indexBuffer, const CSVertexLayout* layouts, int layoutCount) : _vertexBufferCount(vertexBufferCount), _ownIndexBuffer(indexBuffer), _layouts(layoutCount) {
    CSAssert(_vertexBufferCount >= 0 && _vertexBufferCount < countof(_vertexBuffers));

	for (int i = 0; i < _vertexBufferCount; i++) _vertexBuffers[i] = new CSGBuffer(CSGBufferTargetArray);

    if (layoutCount) _layouts.addObjectsFromPointer(layouts, layoutCount);

    if (indexBuffer) _indexBuffer = new CSGBuffer(CSGBufferTargetElementArray);

    CSDelegateRenderCommand* command = CSGraphicsContext::sharedContext()->invoke(true, [this](CSGraphicsApi* api) {
        _object = api->genVertexArray();

        bind(api);

        if (_indexBuffer) api->bindBuffer(CSGBufferTargetElementArray, _indexBuffer->object());

        foreach (const CSVertexLayout&, layout, &_layouts) {
            CSAssert(layout.attrib >= 0 && layout.attrib <= countof(_attribEnabled));

            api->bindBuffer(CSGBufferTargetArray, layout.buffer ? layout.buffer->object() : _vertexBuffers[layout.bufferIndex]->object());
            api->vertexAttribPointer(layout.attrib, layout.size, layout.type, layout.normalized, layout.stride, layout.offset);
            api->vertexAttribDivisor(layout.attrib, layout.divisor);
            if (layout.enabledByDefault) {
                api->setVertexAttribEnabled(layout.attrib, true);
                _attribEnabled[layout.attrib] = true;
            }
        }

        unbind(api);
    }, this);

    if (command) {
        command->addFence(this, CSGBatchFlagReadWrite);
        foreach (const CSVertexLayout&, layout, &_layouts) {
            command->addFence(layout.buffer ? layout.buffer : _vertexBuffers[layout.bufferIndex], CSGBatchFlagRead);
        }
    }
}

CSVertexArray::CSVertexArray(int vertexCount, bool indices, const CSArray<CSVertexLayout>* layouts) : CSVertexArray(vertexCount, indices, layouts->pointer(), layouts->count()) {

}

CSVertexArray::~CSVertexArray() {
    if (_indexBuffer) _indexBuffer->release();
    for (int i = 0; i < _vertexBufferCount; i++) _vertexBuffers[i]->release();

    int object = _object;
    CSGraphicsContext::sharedContext()->invoke(false, [object](CSGraphicsApi* api) {
        api->deleteVertexArray(object);
    });
}

int CSVertexArray::resourceCost() const {
    int cost = sizeof(CSVertexArray);
    for (int i = 0; i < _vertexBufferCount; i++) cost += _vertexBuffers[i]->resourceCost();
    if (_ownIndexBuffer) cost += _indexBuffer->resourceCost();
    cost += _layouts.capacity() * sizeof(CSVertexLayout);
    return cost;
}

bool CSVertexArray::batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const {
    bool result = CSGraphicsResource::batch(reads, writes, flags);
    if (flags & CSGBatchFlagRetrieve) {
        for (int i = 0; i < _vertexBufferCount; i++) result &= _vertexBuffers[i]->batch(reads, writes, flags);
        if (_indexBuffer) result &= _indexBuffer->batch(reads, writes, flags);
        foreach (const CSVertexLayout&, e, &_layouts) {
            if (e.buffer) result &= e.buffer->batch(reads, writes, flags);
        }
    }
    return result;
}

void CSVertexArray::bind(CSGraphicsApi* api) {
    CSAssert(api->vertexArrayBinding() == 0);
    api->bindVertexArray(_object);
}

void CSVertexArray::unbind(CSGraphicsApi* api) {
    CSAssert(api->vertexArrayBinding() == _object);
    api->bindVertexArray(0);
    api->bindBuffer(CSGBufferTargetArray, 0);
    api->bindBuffer(CSGBufferTargetElementArray, 0);
}

void CSVertexArray::attachLayout(CSGraphicsApi* api, const CSVertexLayout& layout) {
    CSAssert(api->vertexArrayBinding() == _object && layout.attrib >= 0 && layout.attrib <= countof(_attribEnabled) && (layout.buffer ? (layout.bufferIndex >= 0 && layout.bufferIndex < _vertexBufferCount) : layout.buffer->target() == CSGBufferTargetArray));

    foreach (CSVertexLayout&, e, &_layouts) {
        if (e.attrib == layout.attrib) {
            if (e.buffer != layout.buffer ||
                e.bufferIndex != layout.bufferIndex ||
                e.attrib != layout.attrib ||
                e.size != layout.size ||
                e.type != layout.type ||
                e.normalized != layout.normalized ||
                e.stride != layout.stride ||
                e.offset != layout.offset)
            {
                api->bindBuffer(CSGBufferTargetArray, layout.buffer ? layout.buffer->object() : _vertexBuffers[layout.bufferIndex]->object());
                api->vertexAttribPointer(layout.attrib, layout.size, layout.type, layout.normalized, layout.stride, layout.offset);
            }
            if (e.divisor != layout.divisor) {
                api->vertexAttribDivisor(layout.attrib, layout.divisor);
            }
            if (_attribEnabled[layout.attrib] != layout.enabledByDefault) {
                api->setVertexAttribEnabled(layout.attrib, layout.enabledByDefault);
                _attribEnabled[layout.attrib] = layout.enabledByDefault;
            }
            e = layout;
            return;
        }
    }
    api->bindBuffer(CSGBufferTargetArray, layout.buffer ? layout.buffer->object() : _vertexBuffers[layout.bufferIndex]->object());
    api->vertexAttribPointer(layout.attrib, layout.size, layout.type, layout.normalized, layout.stride, layout.offset);
    api->vertexAttribDivisor(layout.attrib, layout.divisor);
    api->setVertexAttribEnabled(layout.attrib, layout.enabledByDefault);
    _attribEnabled[layout.attrib] = layout.enabledByDefault;

    _layouts.addObject(layout);
}

void CSVertexArray::detachLayout(CSGraphicsApi* api, int attrib) {
    CSAssert(api->vertexArrayBinding() == _object);

    for (int i = 0; i < _layouts.count(); i++) {
        const CSVertexLayout& e = _layouts.objectAtIndex(i);

        if (e.attrib == attrib) {
            api->setVertexAttribEnabled(attrib, false);
            _attribEnabled[attrib] = false;
            _layouts.removeObjectAtIndex(i);
            break;
        }
    }
}

bool CSVertexArray::hasAttrib(CSGraphicsApi* api, int attrib) const {
    foreach (const CSVertexLayout&, layout, &_layouts) {
        if (layout.attrib == attrib) return true;
    }
    return false;
}

void CSVertexArray::attachIndex(CSGraphicsApi* api, CSGBuffer* indices) {
    CSAssert(!_ownIndexBuffer);

    if (CSObject::retain(_indexBuffer, indices)) {
        CSAssert(api->vertexArrayBinding() == _object && indices->target() == CSGBufferTargetElementArray);
        api->bindBuffer(CSGBufferTargetElementArray, indices->object());
    }
}

void CSVertexArray::setAttribEnabled(CSGraphicsApi* api, int attrib, bool enabled) {
    CSAssert(api->vertexArrayBinding() == _object && attrib >= 0 && attrib < countof(_attribEnabled));

    if (hasAttrib(api, attrib) && _attribEnabled[attrib] != enabled) {
        api->setVertexAttribEnabled(attrib, enabled);
        _attribEnabled[attrib] = enabled;
    }
}

bool CSVertexArray::attribEnabled(CSGraphicsApi* api, int attrib) const {
    CSAssert(attrib >= 0 && attrib < countof(_attribEnabled));
    return _attribEnabled[attrib];
}

CSDrawElementsType CSVertexArray::elementType() const {
    if (_indexBuffer) {
        switch (_indexBuffer->size()) {
            case 1:
                return CSDrawElementsTypeUnsignedByte;
            case 2:
                return CSDrawElementsTypeUnsignedShort;
            case 4:
                return CSDrawElementsTypeUnsignedInt;
        }
    }
    CSAssert(false);
    return CSDrawElementsTypeUnsignedByte;
}

void CSVertexArray::drawArrays(CSGraphicsApi* api, CSPrimitiveMode mode, int vertexOffset, int vertexCount) const {
    int binding = api->vertexArrayBinding();
    CSAssert(binding == 0 || binding == _object);
    if (binding == 0) api->bindVertexArray(_object);
    api->drawArrays(mode, vertexOffset, vertexCount);
    if (binding == 0) api->bindVertexArray(0);
}

void CSVertexArray::drawElements(CSGraphicsApi* api, CSPrimitiveMode mode, int indexOffset, int indexCount) const {
    int binding = api->vertexArrayBinding();
    CSAssert(_indexBuffer && indexOffset >= 0 && indexCount > 0 && indexOffset + indexCount <= _indexBuffer->count() && (binding == 0 || binding == _object));
    if (binding == 0) api->bindVertexArray(_object);
    api->drawElements(mode, indexCount, elementType(), _indexBuffer->size() * indexOffset);
    if (binding == 0) api->bindVertexArray(0);
}

void CSVertexArray::drawElements(CSGraphicsApi* api, CSPrimitiveMode mode) const {
    int binding = api->vertexArrayBinding();
    CSAssert(_indexBuffer && _indexBuffer->count() > 0 && (binding == 0 || binding == _object));
    if (binding == 0) api->bindVertexArray(_object);
    api->drawElements(mode, _indexBuffer->count(), elementType(), 0);
    if (binding == 0) api->bindVertexArray(0);
}

void CSVertexArray::drawElementsInstanced(CSGraphicsApi* api, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCount)const {
    int binding = api->vertexArrayBinding();
    CSAssert(_indexBuffer && indexOffset >= 0 && indexCount > 0 && indexOffset + indexCount <= _indexBuffer->count() && (binding == 0 || binding == _object));
    if (binding == 0) api->bindVertexArray(_object);
    api->drawElementsInstanced(mode, indexCount, elementType(), _indexBuffer->size() * indexOffset, instanceCount);
    if (binding == 0) api->bindVertexArray(0);
}

void CSVertexArray::drawElementsInstanced(CSGraphicsApi* api, CSPrimitiveMode mode, int instanceCount) const {
    int binding = api->vertexArrayBinding();
    CSAssert(_indexBuffer && _indexBuffer->count() > 0 && (binding == 0 || binding == _object));
    if (binding == 0) api->bindVertexArray(_object);
    api->drawElementsInstanced(mode, _indexBuffer->count(), elementType(), 0, instanceCount);
    if (binding == 0) api->bindVertexArray(0);
}