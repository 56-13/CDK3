//?#version 430

layout(std140, binding = 11) uniform waterBlock {
	mat4 world;
	float positionScale;
    float perturbIntensity;
	vec2 textureScale;
	vec2 textureFlowForward;
    vec2 textureFlowCross;
    vec2 foamScale;
    vec2 foamFlowForward;
    vec2 foamFlowCross;
    float foamIntensity;
    float foamDepth;
    vec4 baseColor;
    vec4 shallowColor;
    vec2 wave;
    float depthMax;
};

layout(std140, binding = 12) uniform waterProgressBlock {
	float progress;
};

layout(binding = 19) uniform sampler2D destMap;
layout(binding = 20) uniform sampler2D depthMap;
layout(binding = 21) uniform sampler2D foamMap;

            