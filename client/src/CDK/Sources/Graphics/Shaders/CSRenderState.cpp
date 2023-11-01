#define CDK_IMPL

#include "CSRenderState.h"

#include "CSGBuffers.h"
#include "CSTextures.h"

void CSRenderState::applyBranch(CSProgramBranch* programs, bool usingBone, bool usingInstance, bool usingVertexColor) const {
    programs->addLink(CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);

    programs->addBranch("UsingLight", usingLight(), CSProgramBranch::MaskVertex);
    programs->addBranch("UsingNormal", usingVertexNormal(), CSProgramBranch::MaskVertex);
    programs->addBranch("UsingMap", usingMap(), CSProgramBranch::MaskVertex);
    programs->addBranch("UsingColorMap", material.colorMap, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingNormalMap", material.normalMap, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    programs->addBranch("UsingDisplacementMap", material.displacementScale, CSProgramBranch::MaskVertex);
    programs->addBranch("Usingmaterial.ap", (int)material.materialMapComponents, 8, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingEmissiveMap", material.emissiveMap, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingBone", usingBone, CSProgramBranch::MaskVertex);
    programs->addBranch("UsingInstance", usingInstance, CSProgramBranch::MaskVertex);
    programs->addBranch("UsingVertexColor", usingVertexColor, CSProgramBranch::MaskVertex);
    programs->addBranch("UsingPerspective", camera.fov() != 0, CSProgramBranch::MaskVertex);
    programs->addBranch("UsingAlphaTest", material.alphaTest, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingDepthBias", material.depthBias != 0, CSProgramBranch::MaskVertex);
    programs->addBranch("UsingEmission", material.emission != CSColor3::Black, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingUVOffset", material.uvOffset != CSVector2::Zero, CSProgramBranch::MaskVertex | CSProgramBranch::MaskFragment);
    programs->addBranch("UsingFog", usingFog(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingBrightness", brightness != 0, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingContrast", contrast != 1, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingSaturation", saturation != 1, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingRim", material.rim != 0, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingReflection", material.reflection, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingLightMode", (int)lightMode(), 8, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingDirectionalLight", usingDirectionalLight(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingPointLight", usingPointLight(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingSpotLight", usingSpotLight(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingShadow", usingShadow(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingShadow2D", usingShadow2D(), CSProgramBranch::MaskFragment);
    programs->addBranch("UsingBloom", material.bloom, CSProgramBranch::MaskFragment);
    programs->addBranch("UsingBloomSupported", target->bloomSupported(), CSProgramBranch::MaskFragment);
}

struct RenderStateEnvUniformData {
    CSVector2 resolution;
    float fogNear;
    float fogFar;
    CSColor3 fogColor;

    inline RenderStateEnvUniformData() {
        memset(this, 0, sizeof(RenderStateEnvUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(resolution);
        hash.combine(fogNear);
        hash.combine(fogFar);
        hash.combine(fogColor);
        return hash;
    }
    inline bool operator ==(const RenderStateEnvUniformData& other) const {
        return memcmp(this, &other, sizeof(RenderStateEnvUniformData)) == 0;
    }
    inline bool operator !=(const RenderStateEnvUniformData& other) const {
        return !(*this == other);
    }
};

struct RenderStateCameraUniformData {
    CSMatrix viewProjection;
    CSVector3 eye;
    float znear;
    float zfar;

    inline RenderStateCameraUniformData() {
        memset(this, 0, sizeof(RenderStateCameraUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(viewProjection);
        hash.combine(eye);
        hash.combine(znear);
        hash.combine(zfar);
        return hash;
    }
    inline bool operator ==(const RenderStateCameraUniformData& other) const {
        return memcmp(this, &other, sizeof(RenderStateCameraUniformData)) == 0;
    }
    inline bool operator !=(const RenderStateCameraUniformData& other) const {
        return !(*this == other);
    }
};

struct RenderStateModelUniformData {
    CSVector4 materialFactor;
    CSColor3 emissiveFactor;
    float displacementScale;
    CSVector2 uvOffset;
    float distortionScale;
    float bloomThreshold;
    float brightness;
    float contrast;
    float saturation;
    float alphaTestBias;
    float depthBias;
    float depthBiasPerspective;

    inline RenderStateModelUniformData() {
        memset(this, 0, sizeof(RenderStateModelUniformData));
    }
    uint hash() const {
        CSHash hash;
        hash.combine(materialFactor);
        hash.combine(emissiveFactor);
        hash.combine(displacementScale);
        hash.combine(uvOffset);
        hash.combine(distortionScale);
        hash.combine(bloomThreshold);
        hash.combine(brightness);
        hash.combine(contrast);
        hash.combine(saturation);
        hash.combine(alphaTestBias);
        hash.combine(depthBias);
        hash.combine(depthBiasPerspective);
        return hash;
    }
    inline bool operator ==(const RenderStateModelUniformData& other) const {
        return memcmp(this, &other, sizeof(RenderStateModelUniformData)) == 0;
    }
    inline bool operator !=(const RenderStateModelUniformData& other) const {
        return !(*this == other);
    }
};

void CSRenderState::applyUniforms(CSGraphicsApi* api, const CSGBuffer* boneBuffer) const {
    RenderStateEnvUniformData envData;
    envData.resolution = CSVector2(target->width(), target->height());
    envData.fogNear = fogNear;
    envData.fogFar = fogFar;
    envData.fogColor = fogColor;

    RenderStateCameraUniformData cameraData;
    cameraData.viewProjection = camera.viewProjection();
    cameraData.eye = camera.position();
    cameraData.znear = camera.znear();
    cameraData.zfar = camera.zfar();

    RenderStateModelUniformData modelData;
    modelData.materialFactor = CSVector4(material.metallic, material.roughness, material.ambientOcclusion, material.rim);
    modelData.emissiveFactor = material.emission;
    modelData.uvOffset = material.uvOffset;
    modelData.displacementScale = material.displacementScale;
    modelData.distortionScale = material.distortionScale;
    modelData.bloomThreshold = bloomThreshold;
    modelData.brightness = brightness;
    modelData.contrast = contrast;
    modelData.saturation = saturation;
    modelData.alphaTestBias = material.alphaTestBias;
    modelData.depthBias = material.depthBias;
    
    if (material.depthBias != 0 && camera.fov() != 0) modelData.depthBiasPerspective = material.depthBias * (1.0f + camera.znear() / camera.zfar());

    CSGBuffer* envDataBuffer = CSGBuffers::fromData(envData, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingEnv, envDataBuffer->object());

    CSGBuffer* cameraDataBuffer = CSGBuffers::fromData(cameraData, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingCamera, cameraDataBuffer->object());

    CSGBuffer* modelDataBuffer = CSGBuffers::fromData(modelData, CSGBufferTargetUniform);
    api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingModel, modelDataBuffer->object());

    if (boneBuffer) api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingBone, boneBuffer->object());

    if (material.colorMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingColorMap, material.colorMap->object());
    if (material.normalMap ) api->bindTextureBase(CSTextureTarget2D, TextureBindingNormalMap, material.normalMap->object());
    if (material.materialMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingMaterialMap, material.materialMap->object());
    if (material.emissiveMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingEmissiveMap, material.emissiveMap->object());

    if (usingLight()) {
        api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingLight, lightSpaceState->lightBuffer->object());

        if (lightSpaceState->usingDirectionalLight) {
            api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingDirectionalLight, lightSpaceState->directionalLightBuffer->object());

            if (lightSpaceState->usingShadow) {
                if (material.receiveShadow) {
                    for (int i = 0; i < CSLightSpace::MaxDirectionalLightCount; i++) {
                        const CSTexture* shadowMap = lightSpaceState->directionalShadowMaps[i][false];
                        if (shadowMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingDirectionalShadowMap + i, shadowMap->object());
                    }
                }
                if (material.receiveShadow2D) {
                    for (int i = 0; i < CSLightSpace::MaxDirectionalLightCount; i++) {
                        const CSTexture* shadowMap = lightSpaceState->directionalShadowMaps[i][true];
                        if (shadowMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingDirectionalShadow2DMap + i, shadowMap->object());
                    }
                }
            }
        }
        if (lightSpaceState->usingPointLight) {
            api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingPointLight, lightSpaceState->pointLightBuffer->object());

            api->bindImageTexture(ImageBindingPointLightClusterMap, lightSpaceState->pointLightClusterMap->object(), 0, false, 0, CSTextureAccessReadOnly, lightSpaceState->pointLightClusterMap->format());

            if (lightSpaceState->usingShadow && material.receiveShadow) {
                for (int i = 0; i < CSLightSpace::MaxPointShadowCount; i++) {
                    const CSTexture* shadowMap = lightSpaceState->pointShadowMaps[i];
                    if (shadowMap) api->bindTextureBase(CSTextureTargetCubeMap, TextureBindingPointShadowMap + i, shadowMap->object());
                }
            }
        }
        if (lightSpaceState->usingSpotLight) {
            api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingSpotLight, lightSpaceState->spotLightBuffer->object());

            api->bindImageTexture(ImageBindingSpotLightClusterMap, lightSpaceState->spotLightClusterMap->object(), 0, false, 0, CSTextureAccessReadOnly, lightSpaceState->spotLightClusterMap->format());

            if (lightSpaceState->usingShadow && material.receiveShadow) {
                if (lightSpaceState->spotShadowBuffer) api->bindBufferBase(CSGBufferTargetUniform, UniformBlockBindingSpotShadow, lightSpaceState->spotShadowBuffer->object());

                for (int i = 0; i < CSLightSpace::MaxSpotShadowCount; i++) {
                    const CSTexture* shadowMap = lightSpaceState->spotShadowMaps[i];
                    if (shadowMap) api->bindTextureBase(CSTextureTarget2D, TextureBindingSpotShadowMap + i, shadowMap->object());
                }
            }
        }

        if (material.reflection && lightSpaceState->envMap) api->bindTextureBase(CSTextureTargetCubeMap, TextureBindingEnvMap, lightSpaceState->envMap->object());
        api->bindTextureBase(CSTextureTarget2D, TextureBindingBrdfMap, (lightSpaceState->brdfMap ? lightSpaceState->brdfMap : CSTextures::brdf())->object());
    }
}

uint CSRenderState::hash() const {
    CSHash hash;
    hash.combine(target);
    hash.combine(renderer);
    hash.combine(camera);
    hash.combine(material);
    hash.combine(fogColor);
    hash.combine(fogNear);
    hash.combine(fogFar);
    hash.combine(bloomThreshold);
    hash.combine(brightness);
    hash.combine(contrast);
    hash.combine(saturation);
    hash.combine(polygonMode);
    hash.combine(stencilMode);
    hash.combine(stencilDepth);
    hash.combine(layer);
    hash.combine(strokeWidth);
    hash.combine(strokeMode);
    hash.combine(strokeColor);
    hash.combine(lineWidth);
    hash.combine(scissor);
    hash.combine(lightSpaceState);
    hash.combine(rendererParam);
    return hash;
}
