//!#include "base.glsl"

in vec2 varTexCoord;
in vec4 varColor;
in vec3 varNormal;
in vec3 varTangent;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec4 bloomColor;
#endif

void main() {
#ifdef UsingAlphaTest
	float alpha = varColor.a;
# ifdef UsingColorMap
    alpha *= sampleTexture2d(colorMap, varTexCoord).a;
# endif
	if (alpha <= alphaTestBias) discard;
#endif
    vec3 normal = normalize(varNormal);
# ifdef UsingNormalMap
    vec3 t = normalize(varTangent);
    mat3 tbn = mat3(t, cross(normal, t), normal);
    normal = normalize(tbn * (sampleTexture2d(normalMap, varTexCoord).xyz * 2.0 - 1.0));
# endif
    vec2 screenCoord = (gl_FragCoord.xy - normal.xy * distortionScale) / resolution;

    fragColor = texture(screenMap, screenCoord);
    
#ifdef UsingBloomSupported
    bloomColor = vec4(0.0);
#endif
}
