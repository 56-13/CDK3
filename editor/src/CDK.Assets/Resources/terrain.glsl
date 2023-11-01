//?#version 430

layout(std140, binding = 11) uniform surfaceBlock {
	mat4 world;
	vec2 positionScale;
	vec2 terrainScale;
	vec2 surfaceOffset;
	float surfaceScale;
	float surfaceRotation;
	vec4 baseColor;
	float ambientOcclusionIntensity;
};

layout(binding = 19) uniform sampler2D ambientOcclusionMap;
layout(binding = 20) uniform sampler2D intensityMap;
