//?#version 430

#define ShadowModeDirection	1
#define ShadowModePoint		2
#define ShadowModeSpot		3

//!#define UsingShadowMode	ShadowModePoint

#if UsingShadowMode == ShadowModeDirection
layout (std140, binding = 9) uniform shadowBlock {
	mat4 shadowViewProjection; 
};
#elif UsingShadowMode == ShadowModePoint
layout (std140, binding = 9) uniform shadowBlock {
	mat4 shadowViewProjections[6]; 
	vec3 lightPos;
	float shadowRange;
};
#elif UsingShadowMode == ShadowModeSpot
layout (std140, binding = 9) uniform shadowBlock {
	mat4 shadowViewProjection; 
	vec3 lightPos;
	float shadowRange;
};
#endif
