//!#include "../../CDK.Drawing/Resources/base.glsl"
//!#include "../../CDK.Drawing/Resources/shadow.glsl"
//!#include "terrain.glsl"

layout(location = 0) in vec3 attrPosition;
layout(location = 1) in vec3 attrNormal;

out vec3 varPos;
#ifdef UsingTriPlaner
out vec3 varTexCoord;
#else
out vec2 varTexCoord;
#endif
out vec2 varTerrainCoord;
out vec3 varNormal;
out vec3 varTangent;
out vec4 varDirectionalLightShadowPos;
out vec4 varDirectionalLightShadow2DPos;

#if UsingShadowMode == ShadowModeSpot
out vec4 varShadowPos;
#endif

void main() {
    vec4 pos = vec4(attrPosition.xy * positionScale.x, attrPosition.z * positionScale.y, 1.0);

#if defined(UsingLight) || defined(UsingSurface)
    varTerrainCoord = attrPosition.xy * terrainScale;
#endif

#ifdef UsingSurface
# ifdef UsingTriPlaner
    vec3 texCoord = vec3(attrPosition.xy + surfaceOffset, attrPosition.z) * positionScale.xxy * surfaceScale;
# else
    vec2 texCoord = (attrPosition.xy + surfaceOffset) * positionScale * surfaceScale;
# endif
# ifdef UsingSurfaceRotation
    float sinq = sin(surfaceRotation);
    float cosq = sin(surfaceRotation);
    texCoord.xy = texCoord.x * vec2(cosq, -sinq) + texCoord.y * vec2(sinq, cosq);
# endif

#ifdef UsingMap
    varTexCoord = texCoord;
#endif

# ifdef UsingDisplacementMap
    float displacement;
#  ifdef UsingTriPlaner
    displacement = sampleTextureTriPlaner(normalMap, texCoord, attrNormal).a;
#  else
    displacement = sampleTexture2d(normalMap, texCoord).a;
#  endif
    displacement *= 2.0 * displacementScale;
    pos.xyz += attrNormal * displacement;
# endif
#endif
    pos = world * pos;

#ifdef UsingNormal
    varNormal = normalize(vec3(world * vec4(attrNormal, 0.0)));
# ifdef UsingNormalMap
    vec3 t = cross(vec3(0.0, 1.0, 0.0), attrNormal);
    varTangent = normalize(vec3(world * vec4(t, 0.0)));
# endif
#endif

#ifdef UsingLight
    varPos = pos.xyz / pos.w;
# ifdef UsingDirectionalLightShadow
    varDirectionalLightShadowPos = directionalLightShadowViewProjection * pos;
# endif
# ifdef UsingDirectionalLightShadow2D
    varDirectionalLightShadow2DPos = directionalLightShadow2DViewProjection * pos;
# endif
#endif

#ifdef UsingShadowMode
# if UsingShadowMode == ShadowModePoint
    gl_Position = pos;
# else
    gl_Position = shadowViewProjection * pos;
#  if UsingShadowMode == ShadowModeSpot
    varShadowPos = pos;
#  endif
# endif
#else
    gl_Position = viewProjection * pos;
#endif
}