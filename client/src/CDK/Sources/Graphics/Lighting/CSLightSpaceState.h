#ifndef __CDK__CSLightSpaceState__
#define __CDK__CSLightSpaceState__

#include "CSLightSpace.h"

class CSLightSpaceState : public CSObject {
public:
    CSLightMode mode;
    bool usingDirectionalLight;
    bool usingPointLight;
    bool usingSpotLight;
    bool usingShadow;
    CSPtr<const CSGBuffer> lightBuffer;
    CSPtr<const CSGBuffer> directionalLightBuffer;
    CSPtr<const CSTexture> directionalShadowMaps[CSLightSpace::MaxDirectionalLightCount][2];
    CSPtr<const CSGBuffer> pointLightBuffer;
    CSPtr<const CSTexture> pointLightClusterMap;
    CSPtr<const CSTexture> pointShadowMaps[CSLightSpace::MaxPointShadowCount];
    CSPtr<const CSGBuffer> spotLightBuffer;
    CSPtr<const CSTexture> spotLightClusterMap;
    CSPtr<const CSGBuffer> spotShadowBuffer;
    CSPtr<const CSTexture> spotShadowMaps[CSLightSpace::MaxSpotShadowCount];
    CSPtr<const CSTexture> envMap;
    CSPtr<const CSTexture> brdfMap;
#ifdef CDK_IMPL
public:
#else
private:
#endif
    CSLightSpaceState() = default;
private:
    ~CSLightSpaceState() = default;
public:
#ifdef CDK_IMPL
    void flush(CSGraphics* graphics) const;
#endif
};

#endif
