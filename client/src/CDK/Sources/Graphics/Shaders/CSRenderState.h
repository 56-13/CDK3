#ifndef __CDK__CSRenderState__
#define __CDK__CSRenderState__

#include "CSRenderTarget.h"
#include "CSRenderer.h"
#include "CSCamera.h"
#include "CSMaterial.h"
#include "CSLightSpaceState.h"
#include "CSProgramBranch.h"

struct CSRenderState {
    CSPtr<CSRenderTarget> target;
    CSPtr<CSRenderer> renderer;
    CSCamera camera;
    CSMaterial material;
    CSColor3 fogColor;
    float fogNear;
    float fogFar;
    float bloomThreshold;
    float brightness;
    float contrast;
    float saturation;
    CSDepthMode depthMode;
    CSPolygonMode polygonMode;
    CSStencilMode stencilMode;
    byte stencilDepth;
    byte layer;
    byte strokeWidth;
    CSStrokeMode strokeMode;
    CSColor strokeColor;
    float lineWidth;
    CSRect scissor;
    CSPtr<const CSLightSpaceState> lightSpaceState;
    CSPtr<const CSObject> rendererParam;

    static constexpr int VertexAttribPosition = 0;
    static constexpr int VertexAttribColor = 1;
    static constexpr int VertexAttribTexCoord = 2;
    static constexpr int VertexAttribNormal = 3;
    static constexpr int VertexAttribTangent = 4;
    static constexpr int VertexAttribBoneIndices = 5;
    static constexpr int VertexAttribBoneWeights = 6;
    static constexpr int VertexAttribInstanceModel0 = 7;
    static constexpr int VertexAttribInstanceModel1 = 8;
    static constexpr int VertexAttribInstanceModel2 = 9;
    static constexpr int VertexAttribInstanceModel3 = 10;
    static constexpr int VertexAttribInstanceColor = 11;
    static constexpr int VertexAttribInstanceBoneOffset = 12;

    static constexpr int TextureBindingColorMap = 0;
    static constexpr int TextureBindingNormalMap = 1;
    static constexpr int TextureBindingMaterialMap = 2;
    static constexpr int TextureBindingEmissiveMap = 3;
    static constexpr int TextureBindingEnvMap = 4;
    static constexpr int TextureBindingBrdfMap = 5;
    static constexpr int TextureBindingScreenMap = 6;
    static constexpr int TextureBindingDirectionalShadowMap = 7;
    static constexpr int TextureBindingDirectionalShadow2DMap = 10;
    static constexpr int TextureBindingPointShadowMap = 13;
    static constexpr int TextureBindingSpotShadowMap = 16;
    static constexpr int TextureBindingExtension = 19;

    static constexpr int ImageBindingPointLightClusterMap = 0;
    static constexpr int ImageBindingSpotLightClusterMap = 1;
    static constexpr int ImageBindingExtension = 2;

    static constexpr int UniformBlockBindingEnv = 0;
    static constexpr int UniformBlockBindingCamera = 1;
    static constexpr int UniformBlockBindingModel = 2;
    static constexpr int UniformBlockBindingBone = 3;
    static constexpr int UniformBlockBindingLight = 4;
    static constexpr int UniformBlockBindingDirectionalLight = 5;
    static constexpr int UniformBlockBindingPointLight = 6;
    static constexpr int UniformBlockBindingSpotLight = 7;
    static constexpr int UniformBlockBindingSpotShadow = 8;
    static constexpr int UniformBlockBindingShadow = 9;
    static constexpr int UniformBlockBindingStroke = 10;
    static constexpr int UniformBlockBindingExtension = 11;

    inline CSRenderState() {
        memset(this, 0, sizeof(CSRenderState));
    }

    inline void validate() {
        renderer->validate(*this);
    }
    inline bool visible() const {
        return renderer->visible(*this);
    }
    inline bool usingMap() const {
        return material.colorMap || material.normalMap || material.materialMap || material.emissiveMap;
    }
    inline bool usingVertexNormal() const {
        return usingLight() || material.shader == CSMaterial::ShaderDistortion;
    }
    inline bool usingVertexTangent() const {
        return material.normalMap;
    }
    inline bool usingFog() const {
        return fogFar > fogNear;
    }
    inline bool usingStroke() const {
        return (material.blendMode == CSBlendAlpha || material.blendMode == CSBlendNone) && strokeWidth != 0 && strokeColor.a != 0;
    }
    inline bool usingLight() const {
        return lightSpaceState && material.receiveLight();
    }
    inline bool usingShadow() const {
        return usingLight() && lightSpaceState->usingShadow && material.receiveShadow;
    }
    inline bool usingShadow2D() const {
        return usingLight() && lightSpaceState->usingShadow && material.receiveShadow2D;
    }
    inline CSLightMode lightMode() const {
        return usingLight() ? lightSpaceState->mode : CSLightNone;
    }
    inline bool usingDirectionalLight() const {
        return usingLight() && lightSpaceState->usingDirectionalLight;
    }
    inline bool usingPointLight() const {
        return usingLight() && lightSpaceState->usingPointLight;
    }
    inline bool usingSpotLight() const {
        return usingLight() && lightSpaceState->usingSpotLight;
    }

    void applyBranch(CSProgramBranch* programs, bool usingBone, bool usingInstance, bool usingVertexColor) const;
    void applyUniforms(CSGraphicsApi* api, const CSGBuffer* boneBuffer = NULL) const;

    uint hash() const;

    inline bool operator ==(const CSRenderState& other) const {
        return memcmp(this, &other, sizeof(CSRenderState)) == 0;
    }
    inline bool operator !=(const CSRenderState& other) const {
        return !(*this == other);
    }
};

#endif
