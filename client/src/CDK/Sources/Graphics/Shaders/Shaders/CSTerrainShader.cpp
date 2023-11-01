#define CDK_IMPL

#include "CSTerrainShader.h"

#include "CSFile.h"

#include "CSGBuffers.h"
#include "CSShaderCode.h"
#include "CSShadowRenderer.h"

#include "CSTerrain.h"

CSTerrainShader::CSTerrainShader() {
    string commonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain.glsl"));
    string vertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain_vs.glsl"));
    string fragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain_fs.glsl"));

    _programs.attach(CSShaderTypeVertex, CSShaderCode::Base, commonShaderCode, vertexShaderCode);
    _programs.attach(CSShaderTypeFragment, CSShaderCode::FSBase, commonShaderCode, fragmentShaderCode);

    string shadowCommonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow.glsl"));
    string shadowGeometryShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow_gs.glsl"));
    string shadowFragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/shadow_fs.glsl"));

    _shadowPrograms.attach(CSShaderTypeVertex, CSShaderCode::Base, shadowCommonShaderCode, commonShaderCode, vertexShaderCode);
    _shadowPrograms.attach(CSShaderTypeGeometry, shadowCommonShaderCode, shadowGeometryShaderCode);
    _shadowPrograms.attach(CSShaderTypeFragment, CSShaderCode::Base, shadowCommonShaderCode, shadowFragmentShaderCode);

    string waterCommonShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain_water.glsl"));
    string waterVertexShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain_water_vs.glsl"));
    string waterFragmentShaderCode = string::contentOfFile(CSFile::bundlePath("graphics/terrain_water_fs.glsl"));

    _waterPrograms.attach(CSShaderTypeVertex, CSShaderCode::Base, waterCommonShaderCode, waterVertexShaderCode);
    _waterPrograms.attach(CSShaderTypeFragment, CSShaderCode::FSBase, waterCommonShaderCode, waterFragmentShaderCode);
}

static constexpr int TextureBindingSurfaceAmbientOcclusionMap = CSRenderState::TextureBindingExtension;
static constexpr int TextureBindingSurfaceIntensityMap = CSRenderState::TextureBindingExtension + 1;

static constexpr int TextureBindingWaterDestMap = CSRenderState::TextureBindingExtension;
static constexpr int TextureBindingWaterDepthMap = CSRenderState::TextureBindingExtension + 1;
static constexpr int TextureBindingWaterFoamMap = CSRenderState::TextureBindingExtension + 2;

static constexpr int UniformBlockBindingData = CSRenderState::UniformBlockBindingExtension;
static constexpr int UniformBlockBindingSubData = CSRenderState::UniformBlockBindingExtension + 1;

struct SurfaceUniformData {
    CSMatrix world;
    CSVector2 positionScale;
    CSVector2 terrainScale;
    CSVector2 surfaceOffset;
    float surfaceScale;
    float surfaceRotation;
    CSColor baseColor;
    float ambientOcclusionIntensity;

    inline SurfaceUniformData() {
        memset(this, 0, sizeof(SurfaceUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(world);
        hash.combine(positionScale);
        hash.combine(terrainScale);
        hash.combine(surfaceOffset);
        hash.combine(surfaceScale);
        hash.combine(surfaceRotation);
        hash.combine(baseColor);
        return hash;
    }
    inline bool operator ==(const SurfaceUniformData& other) const {
        return memcmp(this, &other, sizeof(SurfaceUniformData)) == 0;
    }
    inline bool operator !=(const SurfaceUniformData& other) const {
        return !(*this == other);
    }
};

void CSTerrainShader::drawSurface(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSColor& color, const CSTerrain* terrain, const CSTerrainSurface* surface, const CSVertexArray* vertices) {
    state.applyBranch(&_programs, false, false, false);

    _programs.addBranch("UsingSurface", surface != NULL, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _programs.addBranch("UsingSurfaceRotation", surface != NULL && surface->rotation != 0, CSProgramBranch::MaskVertex);
    _programs.addBranch("UsingTriPlaner", surface != NULL && surface->triPlaner, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    CSProgram* program = _programs.endBranch();

    program->use(api);

    state.applyUniforms(api);

    if (state.usingLight()) {
        api->bindTextureBase(CSTextureTarget2D, TextureBindingSurfaceAmbientOcclusionMap, terrain->ambientOcclusionMap()->object());
    }

    int vertexCell = terrain->vertexCell();
    int grid = terrain->grid();

    SurfaceUniformData data;
    data.world = world;
    data.terrainScale = CSVector2(1.0f / (terrain->width() * vertexCell), 1.0f / (terrain->height() * vertexCell));
    data.positionScale = CSVector2((float)grid / vertexCell, grid);
    data.baseColor = color * state.material.color;
    data.ambientOcclusionIntensity = terrain->ambientOcclusionIntensity();

    if (surface) {
        const CSTexture* texture = state.material.colorMap;
        if (!texture) texture = state.material.normalMap;
        if (!texture) texture = state.material.materialMap;
        if (!texture) texture = state.material.emissiveMap;

        data.surfaceOffset = surface->offset;
        data.surfaceScale = texture ? surface->scale / texture->width() : 0;
        data.surfaceRotation = surface->rotation;

        api->bindTextureBase(CSTextureTarget2D, TextureBindingSurfaceIntensityMap, surface->intensityMap->object());
    }
    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingData, dataBuffer->object());

    vertices->drawElements(api, CSPrimitiveTriangles);
}

struct ShadowUniformData {
    CSMatrix world;
    CSVector2 positionScale;

    inline ShadowUniformData() {
        memset(this, 0, sizeof(ShadowUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(world);
        hash.combine(positionScale);
        return hash;
    }
    inline bool operator ==(const ShadowUniformData& other) const {
        return memcmp(this, &other, sizeof(ShadowUniformData)) == 0;
    }
    inline bool operator !=(const ShadowUniformData& other) const {
        return !(*this == other);
    }
};

void CSTerrainShader::drawShadow(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSTerrain* terrain, const CSVertexArray* vertices) {
    const CSShadowRenderer::Param* param = static_assert_cast<const CSShadowRenderer::Param*>(state.rendererParam.value());

    _shadowPrograms.addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    if (param->mode == CSShadowRenderer::Param::ModePoint) {
        _programs.addLink(CSProgramBranch::MaskGeometry);
        _programs.addBranch("UsingPrimitive", 3, 4, CSProgramBranch::MaskGeometry);
    }

    _shadowPrograms.addBranch("UsingShadowMode", param->mode, 4, CSProgramBranch::MaskVertex | CSProgramBranch::MaskGeometry | CSProgramBranch::MaskFragment);

    CSProgram* program = _shadowPrograms.endBranch();

    program->use(api);

    state.applyUniforms(api);

    api->bindBufferBase(CSGBufferTargetUniform, CSRenderState::UniformBlockBindingShadow, param->uniformBuffer->object());

    ShadowUniformData data;
    data.world = world;
    data.positionScale = CSVector2((float)terrain->grid() / terrain->vertexCell(), terrain->grid());

    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingData, dataBuffer->object());

    vertices->drawElements(api, CSPrimitiveTriangles);
}

struct WaterUniformData {
    CSMatrix world;
    float positionScale;
    float perturbIntensity;
    CSVector2 textureScale;
    CSVector2 textureFlowForward;
    CSVector2 textureFlowCross;
    CSVector2 foamScale;
    CSVector2 foamFlowForward;
    CSVector2 foamFlowCross;
    float foamIntensity;
    float foamDepth;
    CSColor baseColor;
    CSColor shallowColor;
    CSVector2 wave;
    float depthMax;

    inline WaterUniformData() {
        memset(this, 0, sizeof(WaterUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(world);
        hash.combine(positionScale);
        hash.combine(perturbIntensity);
        hash.combine(textureScale);
        hash.combine(textureFlowForward);
        hash.combine(textureFlowCross);
        hash.combine(foamScale);
        hash.combine(foamFlowForward);
        hash.combine(foamFlowCross);
        hash.combine(foamIntensity);
        hash.combine(foamDepth);
        hash.combine(baseColor);
        hash.combine(shallowColor);
        hash.combine(wave);
        hash.combine(depthMax);
        return hash;
    }
    inline bool operator ==(const WaterUniformData& other) const {
        return memcmp(this, &other, sizeof(WaterUniformData)) == 0;
    }
    inline bool operator !=(const WaterUniformData& other) const {
        return !(*this == other);
    }
};

void CSTerrainShader::drawWater(CSGraphicsApi* api, const CSRenderState& state, const CSMatrix& world, const CSColor& color, const CSTerrain* terrain, const CSTerrainWater& water, float progress, const CSTexture* destMap, const CSTexture* depthMap) {
    state.applyBranch(&_waterPrograms, false, false, false);

    _waterPrograms.addBranch("UsingWave", water.waveDistance != 0 && water.waveAltitude > 0, CSProgramBranch::MaskVertex);
    _waterPrograms.addBranch("UsingFoam", water.foamTexture, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _waterPrograms.addBranch("UsingCross", water.crossSpeed != 0, CSProgramBranch::MaskFragment);
    _waterPrograms.addBranch("UsingDepth", water.depthMax != 0, CSProgramBranch::MaskFragment);
    _waterPrograms.addBranch("UsingTransparency", water.hasTransparency, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    _waterPrograms.addBranch("UsingFoamDepth", water.foamTexture && water.foamDepth > 0, CSProgramBranch::MaskFragment);

    CSProgram* program = _waterPrograms.endBranch();

    program->use(api);

    state.applyUniforms(api);

    float sinq = CSMath::sin(water.angle);
    float cosq = CSMath::cos(water.angle);
    CSVector2 forward(water.forwardSpeed * sinq, water.forwardSpeed * cosq);
    CSVector2 cross(water.crossSpeed * -cosq, water.crossSpeed * sinq);

    api->bindTextureBase(CSTextureTarget2D, TextureBindingWaterDestMap, destMap->object());
    api->bindTextureBase(CSTextureTarget2D, TextureBindingWaterDepthMap, depthMap->object());

    int grid = terrain->grid();

    float scale = CSMath::sqrt((world.m31 * world.m31) + (world.m32 * world.m32) + (world.m33 * world.m33));

    WaterUniformData data;
    data.world = world;
    data.positionScale = grid;
    data.baseColor = color * state.material.color;
    data.shallowColor = water.shallowColor;
    data.wave = CSVector2(water.waveDistance, water.waveAltitude);          //TODO:waveDistance radian
    data.depthMax = water.depthMax * scale;

    const CSTexture* texture = state.material.colorMap;
    if (!texture) texture = state.material.normalMap;
    if (!texture) texture = state.material.materialMap;
    if (!texture) texture = state.material.emissiveMap;
    if (texture) {
        float width = texture->width();
        float height = texture->height();
        data.textureScale = CSVector2(grid / (water.textureScale * width), grid / (water.textureScale * height));
        data.textureFlowForward = CSVector2(forward.x / width, forward.y / height);
        data.textureFlowCross = CSVector2(cross.x / width, cross.y / height);
        data.perturbIntensity = water.perturbIntensity;
    }
    if (water.foamTexture) {
        api->bindTextureBase(CSTextureTarget2D, TextureBindingWaterFoamMap, water.foamTexture->object());

        float width = water.foamTexture->width();
        float height = water.foamTexture->height();
        data.foamScale = CSVector2(grid / (water.foamScale * width), grid / (water.foamScale * height));
        data.foamFlowForward = CSVector2(forward.x / width, forward.y / height);
        data.foamFlowCross = CSVector2(cross.x / width, cross.y / height);
        data.foamIntensity = water.foamIntensity;
        data.foamDepth = water.foamDepth * scale;
    }
    CSGBuffer* dataBuffer = CSGBuffers::fromData(data, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingData, dataBuffer->object());

    dataBuffer = CSGBuffers::fromData(progress, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingSubData, dataBuffer->object());

    water.vertices->drawElements(api, CSPrimitiveTriangles);
}