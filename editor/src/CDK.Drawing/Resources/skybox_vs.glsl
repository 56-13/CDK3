//!#version 430

layout(std140, binding = 0) uniform block {
    mat4 viewProjection;
};

layout(location = 0) in vec3 attrPosision;

out vec3 varTexCoords;

void main()
{
    varTexCoords = attrPosision;
    vec4 pos = viewProjection * vec4(attrPosision, 1.0);
    gl_Position = pos.xyww;
}
