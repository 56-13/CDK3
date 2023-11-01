//!#include "base.glsl"

#extension GL_ARB_texture_query_lod : enable

#define LightModePhong          1
#define LightModeBlinn          2
#define LightModeCookBlinn      3
#define LightModeCookBeckmann   4
#define LightModeCookGGX        5

in vec3 varPos;
#ifdef UsingTriPlaner
in vec3 varTexCoord;
#else
in vec2 varTexCoord;
#endif
in vec3 varNormal;
in vec3 varTangent;

const float PI = 3.14159265359;

const vec3 LumCoeff = vec3(0.2126, 0.7152, 0.0722);

#ifdef UsingTriPlaner
vec4 sampleTexture(sampler2D map, in vec3 texCoord) {
    return sampleTextureTriPlaner(map, texCoord, varNormal);
}
#else
vec4 sampleTexture(sampler2D map, in vec2 texCoord) {
    return sampleTexture2d(map, texCoord);
}
#endif

vec3 fresnelFactor(vec3 f0, float product) {
    return mix(f0, vec3(1.0), pow(1.01 - product, 5.0));
}

vec3 phongSpecular(vec3 v, vec3 l, vec3 n, vec3 specular, float roughness) {
    vec3 r = reflect(-l, n);
    float spec = max(0.0, dot(v, r));

    float k = 1.999 / (roughness * roughness);

    return min(1.0, 3.0 * 0.0398 * k) * pow(spec, min(10000.0, k)) * specular;
}

vec3 blinnSpecular(float ndh, vec3 specular, float roughness) {
    float k = 1.999 / (roughness * roughness);
    
    return min(1.0, 3.0 * 0.0398 * k) * pow(ndh, min(10000.0, k)) * specular;
}

float blinn(float roughness, float ndh) {
    float m = roughness * roughness;
    float m2 = m * m;
    float n = 2.0 / m2 - 2.0;
    return (n + 2.0) / (2.0 * PI) * pow(ndh, n);
}

float beckmann(float roughness, float ndh) {
    float m = roughness * roughness;
    float m2 = m * m;
    float ndh2 = ndh * ndh;
    return exp((ndh2 - 1.0) / (m2 * ndh2)) / (PI * m2 * ndh2 * ndh2);
}

float ggx(float roughness, float ndh) {
    float m = roughness * roughness;
    float m2 = m * m;
    float d = (ndh * m2 - ndh) * ndh + 1.0;
    return m2 / (PI * d * d);
}

float schlick(float roughness, float ndv, float ndl) {
    float k = roughness * roughness * 0.5;
    float v = ndv * (1.0 - k) + k;
    float l = ndl * (1.0 - k) + k;
    return 0.25 / (v * l);
}

vec3 cooktorranceSpecular(float ndl, float ndv, float ndh, vec3 specular, float roughness, float rim) {
    float d;
#if UsingLightMode == LightModeCookBlinn
    d = blinn(roughness, ndh);
#elif UsingLightMode == LightModeCookBeckmann
    d = beckmann(roughness, ndh);
#elif UsingLightMode == LightModeCookGGX
    d = ggx(roughness, ndh);
#endif
    float g = schlick(roughness, ndv, ndl);

    vec3 result = specular * g * d;
#ifdef UsingRim
    result *= 1.0 / mix(1.0 - roughness * rim * 0.9, 1.0, ndv);
#endif
    return result;
}

void processPerLight(inout vec3 diffuseLight, inout vec3 reflectedLight, vec3 l, vec3 v, vec3 n, vec3 lightColor, vec3 specular, float roughness, float rim) {
    vec3 h = normalize(l + v);

    float ndl = max(0.0, dot(n, l));
    float ndv = max(0.001, dot(n, v));
    float ndh = max(0.001, dot(n, h));
    float hdv = max(0.001, dot(h, v));

#if UsingLightMode == LightModePhong
    vec3 specFresnel = fresnelFactor(specular, ndv);
    vec3 specRef = phongSpecular(v, l, n, specFresnel, roughness);
#elif UsingLightMode == LightModeBlinn
    vec3 specFresnel = fresnelFactor(specular, hdv);
    vec3 specRef = blinnSpecular(ndh, specFresnel, roughness);
#else
    vec3 specFresnel = fresnelFactor(specular, hdv);
    vec3 specRef = cooktorranceSpecular(ndl, ndv, ndh, specFresnel, roughness, rim);
#endif
    specRef *= ndl;

    vec3 diffRef = (vec3(1.0) - specFresnel) * ndl / PI;

    diffuseLight += diffRef * lightColor;
    reflectedLight += specRef * lightColor;
}

float chebyshevUpperBound(float dist, vec2 moments, float shadowBias, float shadowBleeding) {
    if (dist <= moments.x) return 1.0;
    float variance = max(abs(moments.y - (moments.x * moments.x)), shadowBias);
    float d = dist - moments.x;
    float p = variance / (variance + d * d);
    p = clamp((p - shadowBleeding) / (1.0 - shadowBleeding), 0, 1);
    return p;
}

void processLight(inout vec3 color, vec3 normal, vec4 material) {
#ifdef UsingLightMode
    vec3 view = normalize(eye - varPos);

    vec3 diffuseLight = ambientLight * material.z;
    vec3 reflectedLight = vec3(0.0);
    
    vec3 specular = mix(vec3(0.04), color, material.x);

# ifdef UsingDirectionalLight
    for (int i = 0; i < directionalLightCount; i++) {
        const DirectionalLight light = directionalLights[i];

        vec3 lightColor = light.color;
#  ifdef UsingShadow
        if (light.shadowMapIndex >= 0) {
            vec4 shadowPos = light.shadowViewProjection * vec4(varPos, 1.0);
            vec3 shadowCoord = shadowPos.xyz / shadowPos.w * 0.5 + 0.5;
            vec2 moments = texture(directionalShadowMaps[light.shadowMapIndex], shadowCoord.xy).xy * 0.5 + 0.5;

            lightColor *= chebyshevUpperBound(shadowCoord.z, moments, light.shadowBias, light.shadowBleeding);
        }
#  endif
#  ifdef UsingShadow2D
        if (light.shadow2DMapIndex >= 0) {
            vec4 shadowPos = light.shadow2DViewProjection * vec4(varPos, 1.0);
            vec2 shadowCoord = shadowPos.xy / shadowPos.w * 0.5 + 0.5;
            float shadow = texture(directionalShadow2DMaps[light.shadow2DMapIndex], shadowCoord).x;

            lightColor *= shadow;
        }
#  endif
        processPerLight(diffuseLight, reflectedLight, -light.direction, view, normal, lightColor, specular, material.y, material.w);
    }
# endif

# if defined(UsingPointLight) || defined(UsingSpotLight)
    ivec3 lightClusterCoord = min(ivec3(gl_FragCoord.xy / resolution * LightCluster, LightCluster / (lightClusterMaxDepth * gl_FragCoord.w)), LightCluster - 1);

    uvec4 lightIndices;

#  ifdef UsingPointLight
    lightIndices = imageLoad(pointLightClusterMap, lightClusterCoord);

    for (int i = 1; i <= lightIndices[0]; i++) {
        uint lightIndex = lightIndices[i];

        const PointLight light = pointLights[lightIndex];

        vec3 lightDiff = varPos - light.position;
        float lightDist = length(lightDiff);

        if (lightDist > 0 && lightDist < light.attenuation.x) {
            lightDiff /= lightDist;

            float lightRate = 1.0 / (light.attenuation.y + light.attenuation.z * lightDist + light.attenuation.w * lightDist * lightDist);

            vec3 lightColor = light.color * lightRate;

#   ifdef UsingShadow
            if (light.shadowMapIndex >= 0) {
                vec2 moments = texture(pointShadowMaps[light.shadowMapIndex], lightDiff).xy * 0.5 + 0.5;

                lightColor *= chebyshevUpperBound(lightDist / light.attenuation.x, moments, light.shadowBias, light.shadowBleeding);
            }
#   endif
            processPerLight(diffuseLight, reflectedLight, -lightDiff, view, normal, lightColor, specular, material.y, material.w);
        }
    }
#  endif

#  ifdef UsingSpotLight
    lightIndices = imageLoad(spotLightClusterMap, lightClusterCoord);

    for (int i = 1; i <= lightIndices[0]; i++) {
        uint lightIndex = lightIndices[i];

        const SpotLight light = spotLights[lightIndex];

        vec3 lightDiff = varPos - light.position;
        float lightDist = length(lightDiff);

        if (lightDist > 0 && lightDist < light.attenuation.x) {
            lightDiff /= lightDist;

            float theta = dot(lightDiff, light.direction);

            if (theta < light.cutoff) continue;

            float lightRate = 1.0 / (light.attenuation.y + light.attenuation.z * lightDist + light.attenuation.w * lightDist * lightDist);
            
            lightRate *= min((theta - light.cutoff) / light.epsilon, 1.0);

            vec3 lightColor = light.color * lightRate;

#   ifdef UsingShadow
            if (light.shadowMapIndex >= 0) {
                vec4 shadowPos = spotShadowViewProjections[light.shadowMapIndex] * vec4(varPos, 1.0);
                vec2 shadowCoord = shadowPos.xy / shadowPos.w * 0.5 + 0.5;

                vec2 moments = texture(spotShadowMaps[light.shadowMapIndex], shadowCoord).xy * 0.5 + 0.5;

                lightColor *= chebyshevUpperBound(lightDist / light.attenuation.x, moments, light.shadowBias, light.shadowBleeding);
            }
#   endif
            processPerLight(diffuseLight, reflectedLight, -lightDiff, view, normal, lightColor, specular, material.y, material.w);
        }
    }
#  endif
# endif

# ifdef UsingReflection
    vec3 envDiff = textureLod(envMap, normal, envMapMaxLod).rgb;
    vec3 refl = reflect(-view, normal);
#  ifdef GL_ARB_texture_query_lod
    float envLod = max(material.y * envMapMaxLod, textureQueryLOD(envMap, refl).y);
#  else
    float envLod = material.y * envMapMaxLod;
#  endif
    vec3 envSpec = textureLod(envMap, refl, envLod).rgb;
# else
    vec3 envDiff = envColor;
    vec3 envSpec = envColor;
# endif
    diffuseLight += envDiff / PI;

    float ndv = max(0.001, dot(normal, view));

    vec2 brdf = texture(brdfMap, vec2(ndv, material.y)).xy;

    reflectedLight += envSpec * (fresnelFactor(specular, ndv) * brdf.x + brdf.y);

    color = (diffuseLight * mix(color, vec3(0.0), material.x)) + reflectedLight;
#endif
}

void sampleMaterial(inout vec4 material, vec4 texColor) {
#ifdef UsingMaterialMap    
    int i = 0;
# if (UsingMaterialMap & 1) != 0
    material.x = clamp(texColor[i++] + material.x - 0.5, 0.0, 1.0);
# endif
# if (UsingMaterialMap & 2) != 0
    material.y = clamp(texColor[i++] + material.y - 0.5, 0.001, 1.0);
# endif
# if (UsingMaterialMap & 4) != 0
    material.z = pow(texColor[i], material.z);
# endif
#endif
}

void processEmission(inout vec3 color) {
#ifdef UsingEmission
# ifdef UsingEmissiveMap
    color += sampleTexture(emissiveMap, varTexCoord).rgb * emissiveFactor;
# else
    color += emissiveFactor;
# endif
#endif
}

void processFilter(inout vec3 color) {
#ifdef UsingFog
    float d = 1.0 / gl_FragCoord.w;
    float a = clamp(1 - (fogFar - d) / (fogFar - fogNear), 0.0, 1.0);
    color = mix(color, fogColor, a);
#endif

#ifdef UsingBrightness
    color += brightness;
#endif

#ifdef UsingContrast
	const vec3 LumAvg = vec3(0.5, 0.5, 0.5);

	color = mix(LumAvg, color, contrast);
#endif

#ifdef UsingSaturation
	vec3 intensity = vec3(dot(color.rgb, LumCoeff));
	color = mix(intensity, color, saturation);
#endif
}

vec3 processBloom(in vec3 color) {
#ifdef UsingBloom
    float brightness = dot(color, LumCoeff);
    float contribution = max(brightness - bloomThreshold, 0.0) / max(brightness, 0.00001);
    return color * contribution;
#else
    return vec3(0.0);
#endif    
}

void processColor(inout vec3 color, float ao) {
#ifdef UsingLightMode
    vec3 normal = normalize(varNormal);
# ifdef UsingNormalMap
    vec3 t = normalize(varTangent);
    mat3 tbn = mat3(t, cross(normal, t), normal);
    normal = normalize(tbn * (sampleTexture(normalMap, varTexCoord).xyz * 2.0 - 1.0));
# endif
    vec4 material = materialFactor;
# ifdef UsingMaterialMap
    sampleMaterial(material, sampleTexture(materialMap, varTexCoord));
# endif
    material.z *= ao;

    processLight(color, normal, material);
#endif
    processEmission(color);
    processFilter(color);
}
