//!#version 430

layout(std140, binding = 0) uniform block {
	vec2 delta;
};

layout(binding = 0) uniform sampler2D srcMap;

in vec2 varTexCoord;

out vec3 fragColor;

void main() {
    fragColor = 
		(texture(srcMap, varTexCoord - delta).rgb +
		texture(srcMap, varTexCoord + vec2(delta.x, -delta.y)).rgb +
		texture(srcMap, varTexCoord + vec2(-delta.x, delta.y)).rgb +
		texture(srcMap, varTexCoord + delta).rgb) * 0.25;
}
