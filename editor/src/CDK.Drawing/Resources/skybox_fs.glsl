//!#version 430

layout(binding = 0) uniform samplerCube skyboxMap;

in vec3 varTexCoords;

out vec4 fragColor;

void main()
{    
    vec4 color = texture(skyboxMap, varTexCoords);

    fragColor = color;
}
