//!#include "base_vs.glsl"

void main() {
    vec4 pos = processPosition();

    gl_Position = viewProjection * pos;
}
