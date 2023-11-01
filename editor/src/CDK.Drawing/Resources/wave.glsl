//?#version 430

layout(std140, binding = 0) uniform block {
    mat4 worldViewProjection;
    vec3 center;
    float radius;
    vec2 resolution;
    float thickness;
};
