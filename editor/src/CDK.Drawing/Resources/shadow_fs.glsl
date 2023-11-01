//!#include "base.glsl"
//!#include "shadow.glsl"

in vec2 varTexCoord;
in vec4 varColor;

#if UsingShadowMode != ShadowModeDirection
in vec4 varShadowPos;
#endif

out vec2 fragDepth;

void main() {
#ifdef UsingAlphaTest
	float alpha = varColor.a;
# ifdef UsingColorMap
    alpha *= sampleTexture2d(colorMap, varTexCoord).a;
# endif
	if (alpha <= alphaTestBias) discard;
#endif

#if UsingShadowMode == ShadowModeDirection
	float depth = gl_FragCoord.z;
#else
	float depth = length(varShadowPos.xyz - lightPos) / shadowRange;
#endif
	fragDepth.x = depth * 2.0 - 1.0;
	fragDepth.y = depth * depth * 2.0 - 1.0;
}

