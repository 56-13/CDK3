//!#version 430

//!#define MaxBoneCount  256

layout(binding = 0) uniform sampler2D colorMap;
layout(binding = 1) uniform sampler2D normalMap;
layout(binding = 2) uniform sampler2D materialMap;
layout(binding = 3) uniform sampler2D emissiveMap;
layout(binding = 4) uniform samplerCube envMap;
layout(binding = 5) uniform sampler2D brdfMap;
layout(binding = 6) uniform sampler2D screenMap;
layout(binding = 7) uniform sampler2D directionalShadowMaps[3];
layout(binding = 10) uniform sampler2D directionalShadow2DMaps[3];
layout(binding = 13) uniform samplerCube pointShadowMaps[3];
layout(binding = 16) uniform sampler2D spotShadowMaps[3];

layout(binding = 0, rgba8ui) readonly uniform uimage3D pointLightClusterMap;
layout(binding = 1, rgba8ui) readonly uniform uimage3D spotLightClusterMap;

layout(std140, binding = 0) uniform envBlock {
    vec2 resolution;
    float fogNear;
    float fogFar;
    vec3 fogColor;
};

layout(std140, binding = 1) uniform cameraBlock {
    mat4 viewProjection;
    vec3 eye;
    float near;
    float far;
};

layout(std140, binding = 2) uniform modelBlock {
    vec4 materialFactor;
    vec3 emissiveFactor;
    float displacementScale;
    vec2 uvOffset;
    float distortionScale;
    float bloomThreshold;
    float brightness;
    float contrast;
    float saturation;
    float alphaTestBias;
    float depthBias;
    float depthBiasPerspective;
};

layout(std140, binding = 3) uniform boneBlock
{
    mat4 bones[MaxBoneCount];
};

#define LightCluster    16

layout(std140, binding = 4) uniform lightBlock {
    vec3 ambientLight;
    float lightClusterMaxDepth;
    vec3 envColor;
    int envMapMaxLod;
    int directionalLightCount;
};

struct DirectionalLight
{
    vec3 direction;
    vec3 color;
    mat4 shadowViewProjection;
    mat4 shadow2DViewProjection;
    int shadowMapIndex;
    int shadow2DMapIndex;
    float shadowBias;
    float shadowBleeding;
};
layout(std140, binding = 5) uniform directionalLightBlock
{
    DirectionalLight directionalLights[3];
};

struct PointLight
{
    vec3 position;
    vec3 color;
    vec4 attenuation;
    int shadowMapIndex;
    float shadowBias;
    float shadowBleeding;
};
layout(std140, binding = 6) uniform pointLightBlock
{
    PointLight pointLights[256];
};

struct SpotLight
{
    vec3 position;
    float cutoff;
    vec3 direction;
    float epsilon;
    vec3 color;
    vec4 attenuation;
    int shadowMapIndex;
    float shadowBias;
    float shadowBleeding;
};
layout(std140, binding = 7) uniform spotLightBlock
{
    SpotLight spotLights[256];
};
layout(std140, binding = 8) uniform spotShadowBlock
{
    mat4 spotShadowViewProjections[3];
};

#ifdef UsingTriPlaner
vec4 sampleTextureTriPlaner(sampler2D map, vec3 texCoord, in vec3 normal) {
#ifdef UsingUVOffset
    texCoord.xy += uvOffset;
#endif
    vec3 blending = abs(normal);
    blending /= (blending.x + blending.y + blending.z);
    vec4 xaxis = texture(map, texCoord.yz); 
    vec4 yaxis = texture(map, texCoord.xz); 
    vec4 zaxis = texture(map, texCoord.xy); 
    vec4 result = xaxis * blending.x + xaxis * blending.y + zaxis * blending.z;
    return result;
}
#endif

vec4 sampleTexture2d(sampler2D map, vec2 texCoord) {
#ifdef UsingUVOffset
    texCoord += uvOffset;
#endif
    return texture(map, texCoord);
}
