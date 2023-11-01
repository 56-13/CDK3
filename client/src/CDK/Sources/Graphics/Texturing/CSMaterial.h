#ifndef __CDK__CSMaterial__
#define __CDK__CSMaterial__

#include "CSColor.h"
#include "CSTexture.h"

struct CSMaterial {
    enum Shader : byte {
        ShaderLight,
        ShaderNoLight,
        ShaderDistortion
    };
    enum MaterialMapComponent : byte {
        ComponentMetallic = 1,
        ComponentRoughness = 2,
        ComponentOcclusion = 4
    };
    Shader shader;
    CSBlendMode blendMode;
    CSCullMode cullMode;
    byte materialMapComponents;
    bool depthTest;
    bool alphaTest;
    float depthBias;
    float alphaTestBias;
    float displacementScale;
    float distortionScale;
    CSPtr<const CSTexture> colorMap;
    CSPtr<const CSTexture> normalMap;
    CSPtr<const CSTexture> materialMap;
    CSPtr<const CSTexture> emissiveMap;
    CSColor color;
    bool bloom;
    bool reflection;
    bool receiveShadow;
    bool receiveShadow2D;
    float metallic;
    float roughness;
    float ambientOcclusion;
    float rim;
    CSColor3 emission;
    CSVector2 uvOffset;

    static const CSMaterial Default;

    CSMaterial();

    inline bool usingColor() const {
        switch (shader) {
            case ShaderLight:
            case ShaderNoLight:
                return true;
        }
        return false;
    }

    inline bool receiveLight() const {
        switch (shader) {
            case ShaderLight:
                return true;
        }
        return false;
    }

    bool batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const;

    uint hash() const;

    inline bool operator ==(const CSMaterial& other) const {
        return memcmp(this, &other, sizeof(CSMaterial)) == 0;
    }
    inline bool operator !=(const CSMaterial& other) const {
        return !(*this == other);
    }
};

#endif