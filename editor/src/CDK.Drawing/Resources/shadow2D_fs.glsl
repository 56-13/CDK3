//!#include "base.glsl"

in vec2 varTexCoord;
in vec4 varColor;

out float fragColor;

void main() {
	float alpha = varColor.a;
#ifdef UsingColorMap
    alpha *= sampleTexture2d(colorMap, varTexCoord);
#endif
	fragColor = max(1.0 - alpha, 0.0);
}
