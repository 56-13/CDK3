//!#include "base_fs.glsl"

in vec4 varColor;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec3 bloomColor;
#endif

void main() {
    vec4 color;
#ifdef UsingColorMap
    color = sampleTexture2d(colorMap, varTexCoord) * varColor;
#else
    color = varColor;
#endif

#ifdef UsingAlphaTest
    if (color.a <= alphaTestBias) discard;
#endif

    processColor(color.rgb, 1.0);

    color.rgb *= color.a;

    fragColor = color;

#ifdef UsingBloomSupported
    bloomColor = processBloom(color.rgb);
#endif
}

