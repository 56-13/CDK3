//!#include "base.glsl"

layout(location = 0) in vec3 attrPosition;
layout(location = 1) in vec4 attrColor;
layout(location = 2) in vec2 attrTexCoord;
layout(location = 3) in vec3 attrNormal;
layout(location = 4) in vec3 attrTangent;
layout(location = 5) in uvec4 attrBoneIndices;
layout(location = 6) in vec4 attrBoneWeights;
layout(location = 7) in mat4 attrInstanceModel;
layout(location = 11) in vec4 attrInstanceColor;
layout(location = 12) in uint attrInstanceBoneOffset;

out vec3 varPos;
out vec4 varColor;
out vec2 varTexCoord;
out vec3 varNormal;
out vec3 varTangent;

vec4 processPosition() {
    vec4 pos = vec4(attrPosition, 1.0);

#ifdef UsingDisplacementMap
    float displacement = (sampleTexture2d(normalMap, attrTexCoord).w * 2.0 - 1.0) * model.displacementScale;
    pos.xyz += attrNormal * displacement;
#endif

#ifdef UsingInstance
    mat4 model = attrInstanceModel;
# ifdef UsingBone
    mat4 bone = bones[attrInstanceBoneOffset + attrBoneIndices.x] * attrBoneWeights.x;
    bone += bones[attrInstanceBoneOffset + attrBoneIndices.y] * attrBoneWeights.y;
    bone += bones[attrInstanceBoneOffset + attrBoneIndices.z] * attrBoneWeights.z;
    bone += bones[attrInstanceBoneOffset + attrBoneIndices.w] * attrBoneWeights.w;
    model *= bone;
# endif
    pos = model * pos;
#endif

#ifdef UsingVertexColor
    varColor = attrColor;
# ifdef UsingInstance
    varColor *= attrInstanceColor;
# endif
#else
# ifdef UsingInstance
    varColor = attrInstanceColor;
# else
    varColor = vec4(1.0);
# endif
#endif

#ifdef UsingNormal
    vec3 n = attrNormal;
# ifdef UsingInstance
    n = normalize(vec3(model * vec4(n, 0.0)));
# endif
    varNormal = n;
# ifdef UsingNormalMap
    vec3 t = attrTangent;
#  ifdef UsingInstance
    t = normalize(vec3(model * vec4(t, 0.0)));
#  endif
    varTangent = t;
# endif

#endif

#ifdef UsingMap
    varTexCoord = attrTexCoord;
#endif

#ifdef UsingLight
    varPos = pos.xyz / pos.w;
#endif

    return pos;
}

vec4 processViewPosition(in vec4 pos) {
    vec4 vpos = viewProjection * pos;

#ifdef UsingDepthBias
# ifdef UsingPerspective
    float eyed = distance(pos.xyz / pos.w, eye);
    vpos.z = (vpos.z + depthBiasPerspective) * eyed / (eyed + depthBias);
# else
    vpos.z += depthBias * vpos.w;
# endif
#endif
    return vpos;
}
