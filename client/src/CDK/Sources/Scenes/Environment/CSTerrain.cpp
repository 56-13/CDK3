#define CDK_IMPL

#include "CSTerrain.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"
#include "CSVertex.h"
#include "CSTriangle.h"
#include "CSShaders.h"
#include "CSRenderTargets.h"

CSTerrain::CSTerrain(CSBuffer* buffer) {
    if (buffer->readBoolean()) _thumbnail = new CSTexture(buffer);

	_width = buffer->readShort();
	_height = buffer->readShort();
    _altitude = buffer->readShort();
    _grid = buffer->readShort();
    _vertexCell = buffer->readShort();

	loadAltitudes(buffer);
	loadSurfaces(buffer);
    
    _ambientOcclusionMap = new CSTexture(buffer);
    _ambientOcclusionIntensity = buffer->readFloat();
    _ambientOcclusionMap->flush();

    loadWaters(buffer);

    _props = buffer->readArrayFunc<CSSceneObjectBuilder>([](CSBuffer* buffer) -> CSSceneObjectBuilder* {
        return CSSceneObjectBuilder::builderWithBuffer(buffer, true);
    });
}

CSTerrain::~CSTerrain() {
    release(_thumbnail);
    free(_altitudes);
    _vertexBuffer->release();
    _vertices->release();
    _ambientOcclusionMap->release();
    _surfaces->release();
    _waters->release();
    release(_props);
}

void CSTerrain::loadAltitudes(CSBuffer* buffer) {
	int altitudeDataLength = (_width * _vertexCell + 1) * (_height * _vertexCell + 1) * sizeof(fixed);
	_altitudes = (fixed*)fmalloc(altitudeDataLength);
	buffer->read(_altitudes, altitudeDataLength);
}

void CSTerrain::loadSurfaces(CSBuffer* buffer) {
	_vertexBuffer = new CSGBuffer(CSGBufferTargetArray);

    CSVertexLayout vertexLayout(_vertexBuffer, 0, 3, CSVertexAttribTypeFloat, false, 18, 0, 0, true);
    _vertices = new CSVertexArray(0, true, &vertexLayout, 1);

    int vwidth = _width * _vertexCell;
    int vheight = _height * _vertexCell;
    int vertexCount = (vwidth + 1) * (vheight + 1);
    CSGBufferData<CSVertexN>* vertices = new CSGBufferData<CSVertexN>(vertexCount);
    for (int vy = 0; vy <= vheight; vy++) {
        for (int vx = 0; vx <= vwidth; vx++) {
            new (&vertices->addObject()) CSVertexN(CSVector3(vx, vy, altitude(vx, vy)), normal(vx, vy));
        }
    }
    _vertexBuffer->upload(vertices, CSGBufferUsageHintStaticDraw);
    vertices->release();

    bool* fills = (bool*)fcalloc(vwidth, vheight);
    CSVertexIndexData* indices = new CSVertexIndexData(vertexCount, 6 * vwidth * vheight);
    for (int vy = 0; vy < vheight; vy++) {
        for (int vx = 0; vx < vwidth; vx++) {
            addIndex(indices, vx, vy, fills);
        }
    }
    _vertices->indexBuffer()->upload(indices, CSGBufferUsageHintStaticDraw);
    indices->release();

    _vertices->flush();

    int surfaceCount = buffer->readLength();
    _surfaces = new CSArray<CSTerrainSurface>(surfaceCount);

    for (int i = 0; i < surfaceCount; i++) {
        CSTerrainSurface& surface = _surfaces->addObject();
        new (&surface) CSTerrainSurface();

        surface.material = CSMaterialSource::materialWithBuffer(buffer);
        surface.intensityMap = CSTexture::textureWithBuffer(buffer);
        CSVertexLayout vertexLayouts[] = {
            CSVertexLayout(_vertexBuffer, 0, 3, CSVertexAttribTypeFloat, false, 18, 0, 0, true),
            CSVertexLayout(_vertexBuffer, 1, 3, CSVertexAttribTypeHalfFloat, false, 18, 12, 0, true)
        };
        surface.vertices = CSVertexArray::vertexArray(0, true, vertexLayouts, 2);

        memset(fills, true, vwidth * vheight);

        int cellCount = buffer->readLength();
        int pos = buffer->position();
        for (int i = 0; i < cellCount; i++) {
            int vx = buffer->readShort();
            int vy = buffer->readShort();
            fills[vy * vwidth + vx] = false;
        }
        buffer->setPosition(pos);
        indices = new CSVertexIndexData(_vertexBuffer->count(), 6 * cellCount);
        for (int i = 0; i < cellCount; i++) {
            int vx = buffer->readShort();
            int vy = buffer->readShort();
            addIndex(indices, vx, vy, fills);
        }
        surface.vertices->indexBuffer()->upload(indices, CSGBufferUsageHintStaticDraw);
        indices->release();

        surface.rotation = buffer->readFloat();
        surface.scale = buffer->readFloat();
        surface.triPlaner = buffer->readBoolean();
        surface.offset = CSVector2(buffer);

        surface.material->flush();
        surface.vertices->flush();
    }
    free(fills);
}

void CSTerrain::loadWaters(CSBuffer* buffer) {
    int waterCount = buffer->readLength();
    _waters = new CSArray<CSTerrainWater>(waterCount);

    CSVertexLayout vertexLayout(0, 0, 3, CSVertexAttribTypeFloat, false, 12, 0, 0, true);

    for (int i = 0; i < waterCount; i++) {
        CSTerrainWater& water = _waters->addObject();
        new (&water) CSTerrainWater();

        water.material = CSMaterialSource::materialWithBuffer(buffer);
        water.textureScale = buffer->readFloat();
        water.perturbIntensity = buffer->readFloat();
        if (buffer->readBoolean()) water.foamTexture = CSTexture::textureWithBuffer(buffer);
        water.foamScale = buffer->readFloat();
        water.foamIntensity = buffer->readFloat();
        water.foamDepth = buffer->readFloat();
        water.angle = buffer->readFloat();
        water.forwardSpeed = buffer->readFloat();
        water.crossSpeed = buffer->readFloat();
        water.waveDistance = buffer->readFloat();
        water.waveAltitude = buffer->readFloat();
        water.depthMax = buffer->readFloat();
        water.shallowColor = CSColor3(buffer);
        water.hasTransparency = buffer->readBoolean();
        water.vertices = CSVertexArray::vertexArray(1, true, &vertexLayout, 1);

        int instanceCount = buffer->readLength();
        CSGBufferData<CSVector3>* vertices = new CSGBufferData<CSVector3>(instanceCount * 4);
        CSVertexIndexData* indices = new CSVertexIndexData(instanceCount * 4, instanceCount * 6);
        int vi = 0;
        for (int j = 0; j < instanceCount; j++) {
            int x = buffer->readShort();
            int y = buffer->readShort();
            float z = buffer->readFloat();

            vertices->addObject(CSVector3(x, y, z));
            vertices->addObject(CSVector3(x + 1, y, z));
            vertices->addObject(CSVector3(x, y + 1, z));
            vertices->addObject(CSVector3(x + 1, y + 1, z));

            indices->addObject(vi);
            indices->addObject(vi + 1);
            indices->addObject(vi + 2);
            indices->addObject(vi + 1);
            indices->addObject(vi + 3);
            indices->addObject(vi + 2);
            vi += 4;
        }
        water.vertices->vertexBuffer(0)->upload(vertices, CSGBufferUsageHintStaticDraw);
        water.vertices->indexBuffer()->upload(indices, CSGBufferUsageHintStaticDraw);

        vertices->release();
        indices->release();

        water.material->flush();
        if (water.foamTexture) water.foamTexture->flush();
        water.vertices->flush();
    }
}

void CSTerrain::addIndex(CSVertexIndexData* indices, int vx, int vy, bool* fills) {
    int vwidth = _width * _vertexCell;

    if (fills[vy * vwidth + vx]) return;

    int vheight = _height * _vertexCell;

    fixed a = altitude(vx, vy);

    int vw = 1;
    int vh = 1;

    if (a == altitude(vx + 1, vy) && a == altitude(vx, vy + 1) && a == altitude(vx + 1, vy + 1)) {
        while (vx + vw < vwidth) {
            bool flag = true;
            for (int vdy = 0; vdy <= vh; vdy++) {
                if (altitude(vx + vw + 1, vy + vdy) != a) {
                    flag = false;
                    break;
                }
            }
            if (flag) vw++;
            else break;
        }
        while (vy + vh < vheight) {
            bool flag = true;
            for (int vdx = 0; vdx <= vw; vdx++) {
                if (altitude(vx + vdx, vy + vh + 1) != a) {
                    flag = false;
                    break;
                }
            }
            if (flag) vh++;
            else break;
        }
    }

    int vi0 = vy * (vwidth + 1) + vx;
    int vi1 = vi0 + vw;
    int vi2 = vi0 + (vwidth + 1) * vh;
    int vi3 = vi0 + (vwidth + 1) * vh + vw;

    if (vw > 1 || vh > 1 || isZQuad(vx, vy)) {
        indices->addObject(vi0);
        indices->addObject(vi1);
        indices->addObject(vi2);
        indices->addObject(vi1);
        indices->addObject(vi3);
        indices->addObject(vi2);
    }
    else {
        indices->addObject(vi0);
        indices->addObject(vi1);
        indices->addObject(vi3);
        indices->addObject(vi0);
        indices->addObject(vi3);
        indices->addObject(vi2);
    }
    for (int vdx = 0; vdx < vw; vdx++) {
        for (int vdy = 0; vdy < vh; vdy++) {
            fills[(vy + vdy) * vwidth + (vx + vdx)] = true;
        }
    }
}

int CSTerrain::resourceCost() const {
    int cost = sizeof(CSTerrain);
    if (_thumbnail) cost += _thumbnail->resourceCost();
    int vwidth = _width * _vertexCell;
    int vheight = _height * _vertexCell;
    cost += vwidth * vheight * sizeof(fixed);
    cost += _vertexBuffer->resourceCost();
    cost += _vertices->resourceCost();
    cost += _ambientOcclusionMap->resourceCost();
    cost += _surfaces->capacity() * sizeof(CSTerrainSurface);
    foreach (const CSTerrainSurface&, surface, _surfaces) {
        cost += surface.intensityMap->resourceCost();
        cost += surface.vertices->resourceCost();
    }
    cost += _waters->capacity() * sizeof(CSTerrainWater);
    foreach (const CSTerrainWater&, water, _waters) {
        cost += water.material->resourceCost();
        if (water.foamTexture) cost += water.foamTexture->resourceCost();
        cost += water.vertices->resourceCost();
    }
    return cost;
}

void CSTerrain::preload() const {
    foreach (const CSTerrainSurface&, surface, _surfaces) surface.material->preload();
    foreach (const CSTerrainWater&, water, _waters) water.material->preload();
    foreach (const CSSceneObjectBuilder*, prop, _props) prop->preload();
}

fixed CSTerrain::altitude(int vx, int vy) const {
    int vwidth = _width * _vertexCell;
    int vheight = _height * _vertexCell;

    vx = CSMath::clamp(vx, 0, vwidth);
    vy = CSMath::clamp(vy, 0, vheight);

    return _altitudes[vy * (vwidth + 1) + vx];
}

fixed CSTerrain::altitude(const CSFixed2& p) const {
    fixed fvx = p.x * _vertexCell;
    fixed fvy = p.y * _vertexCell;

    int vx = fvx;
    int vy = fvy;

    if (p.x <= fixed::Zero || p.x >= _width || p.y <= fixed::Zero || p.y >= _height || (fvx == vx && fvy == vy)) return altitude(vx, vy);

    fixed xd = fvx - vx;
    fixed yd = fvy - vy;

    fixed h0 = altitude(vx, vy);
    fixed h1 = altitude(vx + 1, vy);
    fixed h2 = altitude(vx, vy + 1);
    fixed h3 = altitude(vx + 1, vy + 1);

    fixed a;

    if (CSMath::abs(h3 - h0) >= CSMath::abs(h2 - h1)) {
        if (xd <= fixed::One - yd) {
            a = h0 * (fixed::One - xd - yd) + h1 * xd + h2 * yd;
        }
        else {
            a = h1 * (fixed::One - yd) + h3 * (xd + yd - fixed::One) + h2 * (fixed::One - xd);
        }
    }
    else {
        if (xd <= yd) {
            a = h0 * (fixed::One - yd) + h3 * xd + h2 * (yd - xd);
        }
        else {
            a = h0 * (fixed::One - xd) + h1 * (xd - yd) + h3 * yd;
        }
    }
    return a;
}

CSVector3 CSTerrain::normal(int vx, int vy) const {
    float lh = altitude(vx - 1, vy);
    float rh = altitude(vx + 1, vy);
    float th = altitude(vx, vy - 1);
    float bh = altitude(vx, vy + 1);

    CSVector3 p0(2, 0, rh - lh);
    CSVector3 p1(0, 2, bh - th);

    CSVector3 normal = CSVector3::normalize(CSVector3::cross(p0, p1));

    return normal;
}

bool CSTerrain::isZQuad(int vx, int vy) const {
    fixed a0 = altitude(vx, vy);
    fixed a1 = altitude(vx + 1, vy);
    fixed a2 = altitude(vx, vy + 1);
    fixed a3 = altitude(vx + 1, vy + 1);

    return isZQuad(a0, a1, a2, a3);
}

bool CSTerrain::isZQuad(fixed a0, fixed a1, fixed a2, fixed a3) const {
    return CSMath::abs(a3 - a0) >= CSMath::abs(a2 - a1);
}

CSABoundingBox CSTerrain::space() const {
    return CSABoundingBox(CSVector3::Zero, CSVector3(_width, _height, _altitude) * _grid);
}

CSArray<CSSceneObject>* CSTerrain::createProps() const {
    if (!_props) return NULL;
    CSArray<CSSceneObject>* result = new CSArray<CSSceneObject>(_props->count());
    foreach (const CSSceneObjectBuilder*, pb, _props) {
        CSSceneObject* prop = pb->createObject();
        result->addObject(prop);
        prop->release();
    }
    return result;
}

void CSTerrain::getZ(const CSVector3& pos, float& z) const {
    float tz = (float)altitude(CSFixed2((fixed)(pos.x / _grid), (fixed)(pos.y / _grid))) * _grid;

    if (z < tz) z = tz;
}

bool CSTerrain::intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit) const {
    bool hitFlag = (flags & CSCollisionFlagHit) != 0 && hit != NULL;

    CSVector3 bottomPos = ray.position;
    if (ray.intersects(CSPlane(0, 0, 1, 0), distance)) bottomPos += ray.direction * distance;
    else return false;

    CSVector3 topPos = ray.position;
    float d;
    if (ray.intersects(CSPlane(CSVector3::UnitZ, -_altitude * _grid), d)) topPos += ray.direction * d;

    float minx = CSMath::min(topPos.x, bottomPos.x);
    float maxx = CSMath::max(topPos.y, bottomPos.y);
    CSVector3 diff = bottomPos - topPos;

    float wtov = (float)_vertexCell / _grid;
    float vtow = 1 / wtov;

    int vw = _width * _vertexCell - 1;
    int vh = _height * _vertexCell - 1;
    int vx0 = CSMath::clamp((int)(minx * wtov), 0, vw);
    int vx1 = CSMath::clamp((int)(maxx * wtov), 0, vw);

    for (int vx = vx0; vx <= vx1; vx++) {
        float y0 = topPos.y;
        float y1 = bottomPos.y;

        if (diff.x != 0) {
            float x0 = CSMath::max(vx * vtow, minx);
            float x1 = CSMath::min((vx + 1) * vtow, maxx);
            y0 = topPos.y + diff.y * (x0 - topPos.x) / diff.x;
            y1 = topPos.y + diff.y * (x1 - topPos.x) / diff.x;
        }
        if (y0 > y1) CSMath::swap(y0, y1);
        int vy0 = CSMath::clamp((int)(y0 * wtov), 0, vh);
        int vy1 = CSMath::clamp((int)(y1 * wtov), 0, vh);

        for (int vy = vy0; vy <= vy1; vy++) {
            fixed a0 = altitude(vx, vy) * _grid;
            fixed a1 = altitude(vx + 1, vy) * _grid;
            fixed a2 = altitude(vx, vy + 1) * _grid;
            fixed a3 = altitude(vx + 1, vy + 1) * _grid;

            CSVector3 p0(vx * vtow, vy * vtow, a0);
            CSVector3 p1((vx + 1) * vtow, vy * vtow, a1);
            CSVector3 p2(vx * vtow, (vy + 1) * vtow, a2);
            CSVector3 p3 ((vx + 1) * vtow, (vy + 1) * vtow, a3);

            CSTriangle tri0, tri1;

            if (isZQuad(a0, a1, a2, a3)) {
                tri0 = CSTriangle(p0, p1, p2);
                tri1 = CSTriangle(p1, p3, p2);
            }
            else {
                tri0 = CSTriangle(p0, p1, p3);
                tri1 = CSTriangle(p0, p3, p2);
            }
            if (ray.intersects(tri0, d) && d < distance) {
                distance = d;
                if (hitFlag) hit->direction = tri0.normal();
            }
            else if (ray.intersects(tri1, d) && d < distance) {
                distance = d;
                if (hitFlag) hit->direction = tri1.normal();
            }
        }
    }

    if (hitFlag) hit->position = ray.position + ray.direction * distance;

    return true;
}

void CSTerrain::drawShadow(CSGraphics* graphics) const {
    CSRenderTarget* target = graphics->target();
    CSRenderState state = graphics->state();
    CSMatrix world = graphics->world();

    CSDelegateRenderCommand* command = graphics->command([this, target, state, world](CSGraphicsApi* api) {
        target->focus(api);
        target->setDrawBuffer(api, 0);

        api->applyBlendMode(CSBlendNone);
        api->applyDepthMode(CSDepthReadWrite);
        api->applyCullMode(CSCullBack);

        CSTerrainShader* shader = CSShaders::terrain();

        shader->drawShadow(api, state, world, this, _vertices);
    }, this);
    
    command->addFence(target, CSGBatchFlagWrite | CSGBatchFlagRetrieve);
}

void CSTerrain::drawSurfaces(CSGraphics* graphics, float progress, int random) const {
    CSRenderTarget* target = graphics->target();
    CSRenderState state = graphics->state();
    CSMatrix world = graphics->world();
    CSColor color = graphics->color();

    CSDelegateRenderCommand* command = graphics->command([this, target, state, world, color, progress, random](CSGraphicsApi* api) mutable {
        target->focus(api);
        if (target->bloomSupported()) target->setDrawBuffers(api, 2, 0, 1);
        else target->setDrawBuffer(api, 0);

        api->applyBlendMode(CSBlendNone);
        api->applyDepthMode(CSDepthReadWrite);
        api->applyCullMode(CSCullBack);

        state.material.shader = CSMaterial::ShaderNoLight;
        state.material.color = CSColor::Black;

        CSTerrainShader* shader = CSShaders::terrain();

        shader->drawSurface(api, state, world, color, this, NULL, _vertices);

        api->applyBlendMode(CSBlendAdd);
        api->applyDepthMode(CSDepthRead);

        foreach (const CSTerrainSurface&, surface, _surfaces) {
            if (surface.material->getMaterial(progress, random, state.material)) {          //TODO:blendMode가 material에 따라가면 안되는데 체크
                shader->drawSurface(api, state, world, color, this, &surface, surface.vertices);
            }
        }
    }, this);

    command->addFence(target, CSGBatchFlagWrite | CSGBatchFlagRetrieve);
}

void CSTerrain::drawWaters(CSGraphics* graphics, float progress, int random) const {
    if (_waters->count() == 0) return;

    CSRenderTarget* target = graphics->target();
    CSRenderState state = graphics->state();
    CSMatrix world = graphics->world();
    CSColor color = graphics->color();

    CSDelegateRenderCommand* command = graphics->command([this, target, state, world, color, progress, random](CSGraphicsApi* api) mutable {
        CSRenderTargetDescription sampleTargetDesc;
        sampleTargetDesc.width = target->viewport().width;
        sampleTargetDesc.height = target->viewport().height;

        CSRenderTargetDescription::Attachment a;
        a.attachment = CSFramebufferAttachmentColor0;
        a.format = target->format(CSFramebufferAttachmentColor0);
        a.texture = true;
        sampleTargetDesc.attachments.addObject(a);

        a.attachment = CSFramebufferAttachmentDepthStencil;
        a.format = target->format(CSFramebufferAttachmentDepthStencil);
        a.texture = true;
        sampleTargetDesc.attachments.addObject(a);

        CSRenderTarget* sampleTarget = CSRenderTargets::getTemporary(sampleTargetDesc);
        target->blit(sampleTarget, CSClearBufferColor | CSClearBufferDepth, CSBlitFramebufferFilterNearest);
        CSTexture* destMap = sampleTarget->textureAttachment(CSFramebufferAttachmentColor0);
        CSTexture* depthMap = sampleTarget->textureAttachment(CSFramebufferAttachmentDepthStencil);

        target->focus(api);
        if (target->bloomSupported()) target->setDrawBuffers(api, 2, 0, 1);
        else target->setDrawBuffer(api, 0);

        api->applyBlendMode(CSBlendNone);
        api->applyDepthMode(CSDepthRead);
        api->applyCullMode(CSCullBack);

        CSTerrainShader* shader = CSShaders::terrain();

        foreach (const CSTerrainWater&, water, _waters) {
            if (water.material->getMaterial(progress, random, state.material)) {
                shader->drawWater(api, state, world, color, this, water, progress, destMap, depthMap);
            }
        }

        CSResourcePool::sharedPool()->remove(sampleTarget);
    }, this);

    command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSTerrain::draw(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random) const {
    switch (layer) {
        case CSInstanceLayerShadow:
            drawShadow(graphics);
            break;
        case CSInstanceLayerNone:
            drawSurfaces(graphics, progress, random);
            drawWaters(graphics, progress, random);
            break;
        case CSInstanceLayerBase:
            drawSurfaces(graphics, progress, random);
            break;
        case CSInstanceLayerBlendBottom:
            drawWaters(graphics, progress, random);
            break;
    }
    /*
    foreach (var wallInstance in Asset.WallInstances) {
        wallInstance.Draw(graphics, layer, progress, random);
    }
    */
    //TODO
}