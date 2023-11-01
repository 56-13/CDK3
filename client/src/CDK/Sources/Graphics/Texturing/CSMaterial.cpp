#define CDK_IMPL

#include "CSMaterial.h"

#include "CSEntry.h"

const CSMaterial CSMaterial::Default;

CSMaterial::CSMaterial() {
    memset(this, 0, sizeof(CSMaterial));

    shader = ShaderLight;
    blendMode = CSBlendNone;
    cullMode = CSCullBack;
    depthTest = true;
    color = CSColor::White;
    bloom = true;
    reflection = true;
    receiveShadow = true;
    metallic = 0.5f;
    roughness = 0.5f;
    ambientOcclusion = 1;
}

bool CSMaterial::batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes) const {
    bool result = true;
    if (colorMap) result &= colorMap->batch(reads, writes, CSGBatchFlagRead);
    if (normalMap) result &= normalMap->batch(reads, writes, CSGBatchFlagRead);
    if (materialMap) result &= materialMap->batch(reads, writes, CSGBatchFlagRead);
    if (emissiveMap) result &= emissiveMap->batch(reads, writes, CSGBatchFlagRead);
    return result;
}

uint CSMaterial::hash() const {
    CSHash hash;
    hash.combine(shader);
    hash.combine(blendMode);
    hash.combine(cullMode);
    hash.combine(materialMapComponents);
    hash.combine(depthTest);
    hash.combine(alphaTest);
    hash.combine(depthBias);
    hash.combine(alphaTestBias);
    hash.combine(displacementScale);
    hash.combine(distortionScale);
    hash.combine(colorMap);
    hash.combine(normalMap);
    hash.combine(materialMap);
    hash.combine(emissiveMap);
    hash.combine(color);
    hash.combine(bloom);
    hash.combine(reflection);
    hash.combine(receiveShadow);
    hash.combine(receiveShadow2D);
    hash.combine(metallic);
    hash.combine(roughness);
    hash.combine(rim);
    hash.combine(emission);
    hash.combine(uvOffset);
    return hash;
}
