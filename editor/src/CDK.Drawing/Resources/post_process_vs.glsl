//!#version 430

layout(location = 0) in vec2 attrPosition;

void main() {
	gl_Position = vec4(attrPosition, 0.0, 1.0);
}

