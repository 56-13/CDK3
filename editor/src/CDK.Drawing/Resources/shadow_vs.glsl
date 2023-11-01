//!#include "base_vs.glsl"
//!#include "shadow.glsl"

#if UsingShadowMode == ShadowModeSpot
out vec4 varShadowPos;
#endif

void main() {
    vec4 pos = processPosition();

#if UsingShadowMode == ShadowModePoint
    gl_Position = pos;
#else
    gl_Position = shadowViewProjection * pos;
# if UsingShadowMode == ShadowModeSpot
    varShadowPos = pos;
# endif
#endif
}
