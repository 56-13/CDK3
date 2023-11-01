//!#version 430

layout(location = 0) in vec2 attrPosition;

out vec2 varTexCoord;

void main() {
    gl_Position = vec4(attrPosition, 0.0, 1.0);

    varTexCoord = attrPosition * 0.5 + 0.5;
}

