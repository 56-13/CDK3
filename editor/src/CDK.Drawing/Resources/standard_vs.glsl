//!#include "base_vs.glsl"

void main() {
    vec4 pos = processPosition();

    vec4 vpos = processViewPosition(pos);

    gl_Position = vpos;
}
