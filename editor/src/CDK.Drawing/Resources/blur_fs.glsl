//!#version 430

#define BlurModeNormal		0
#define BlurModeCube		1
#define BlurModeDepth		2
#define BlurModeDirection	3
#define BlurModeCenter		4

#define KernelRadius	2

#if (BlurMode == BlurModeNormal || BlurMode == BlurModeCube)
layout(std140, binding = 0) uniform block {
	vec4 kernel;
	vec2 direction;
};
#elif BlurMode == BlurModeDepth
layout(std140, binding = 0) uniform block {
	vec4 kernel;
	vec2 direction;
	float range;
	float distance;
	float near;
	float far;
};
#elif BlurMode == BlurModeDirection
layout(std140, binding = 0) uniform block {
	vec2 resolution;
	vec2 direction;
};
#elif BlurMode == BlurModeCenter
layout(std140, binding = 0) uniform block {
	vec2 resolution;
	vec2 centerCoord;
	float range;
};
#endif

#if BlurMode == BlurModeCube
layout(binding = 0) uniform samplerCube srcMap;
in vec3 varTexCoord;
in vec3 varDirection;
#else
layout(binding = 0) uniform sampler2D srcMap;
# if BlurMode == BlurModeDepth
layout(binding = 1) uniform sampler2D depthMap;
# endif
in vec2 varTexCoord;
#endif

out vec4 fragColor;

#if BlurMode == BlurModeDepth
float convertToLinearDepth(float depth) {
	return (near * far) / (far - depth * (far - near));
}
#endif

void main() {
    vec4 color;

#if BlurMode == BlurModeCenter || BlurMode == BlurModeDirection
# if BlurMode == BlurModeCenter
	vec2 direction = varTexCoord - centerCoord;
# endif
	float d = length(direction);
# if BlurMode == BlurModeCenter
	d *= range;
# endif
	int c = int(ceil(d)) + 1;
	vec2 off = direction / (resolution * c);
# if BlurMode == BlurModeCenter
	off *= range;
# endif
	float r = 1.0 / c;
	
	color = vec4(0.0);
	for (int i = 0; i < c; i++) {
		color += texture(srcMap, varTexCoord + i * off) * r;
	}
#else
	float k = kernel[0];

# if BlurMode == BlurModeCube
	//vec3 dir = varDirection * length(varTexCoord);		//check
	vec3 dir = varDirection;
# else
	vec2 dir = direction;
#  if BlurMode == BlurModeDepth
	float depth = convertToLinearDepth(texture(depthMap, varTexCoord).r);
	float rate = min(abs(distance - depth) / range, 1.0);
	k = mix(1.0, k, rate);
#  endif
# endif

	color = texture(srcMap, varTexCoord) * k;

	for (int i = 1; i <= KernelRadius; i++) {
		k = kernel[i];
# if BlurMode == BlurModeDepth
		k = mix(0.0, k, rate);
# endif
		color += texture(srcMap, varTexCoord + i * dir) * k;
		color += texture(srcMap, varTexCoord - i * dir) * k;
	}
#endif
	fragColor = color;
}
