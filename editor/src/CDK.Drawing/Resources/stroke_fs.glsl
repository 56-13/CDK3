//!#include "base.glsl"
//!#include "stroke.glsl"

in vec4 varTexCoord;

out vec4 fragColor;

void main() {
    vec4 color = strokeColor;
#ifdef UsingColorMap
    color.a *= sampleTexture2d(colorMap, varTexCoord).a;
#endif
    color.rgb *= color.a;

    fragColor = color;
}

