//!#version 430

layout(location = 0) in vec2 attrPosition;

#ifndef UsingCube
out vec2 varTexCoord;
#endif

void main() {
    gl_Position = vec4(attrPosition, 0.0, 1.0);

#ifndef UsingCube
    varTexCoord = attrPosition * 0.5 + 0.5;
#endif
}

