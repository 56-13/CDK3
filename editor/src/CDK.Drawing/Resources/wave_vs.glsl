//!#version 430
//!#include "wave.glsl"

layout (location = 0) in vec3 attrPosition;

out vec3 varOriginPos;
out vec4 varCenterPos;

void main() {
    gl_Position = worldViewProjection * vec4(attrPosition, 1.0);

    varOriginPos = attrPosition;
    varCenterPos = worldViewProjection * vec4(center, 1.0);
}
