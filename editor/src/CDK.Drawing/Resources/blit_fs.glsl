//!#version 430

#ifdef UsingCube
layout(binding = 0) uniform samplerCube srcMap
in vec3 varTexCoord;
#else
layout(binding = 0) uniform sampler2D srcMap;
in vec2 varTexCoord;
#endif

out vec4 fragColor;

void main() {
#ifdef UsingCube
	fragColor = textureCube(srcMap, varTexCoord);
#else
	fragColor = texture(srcMap, varTexCoord);
#endif
}
