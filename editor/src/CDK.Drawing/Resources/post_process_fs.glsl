//!#version 430

#if UsingMultisample > 1
layout(binding = 0) uniform sampler2DMS colorMap;
#else
layout(binding = 0) uniform sampler2D colorMap;
#endif

layout(binding = 1) uniform sampler2D bloomMap;

layout(std140, binding = 0) uniform block {
	vec2 resolution;
	float bloomIntensity;
	float gammaInv;
	float exposure;
};

out vec4 fragColor;

void main() {
	vec2 texCoord = gl_FragCoord.xy / resolution;

#if UsingMultisample > 1
	ivec2 itexCoord = ivec2(texCoord * resolution);
	vec4 color = vec4(0.0);
	for (int i = 0; i < UsingMultisample; i++) {
		color += texelFetch(colorMap, itexCoord, i);
	}
	color /= UsingMultisample;
#else
	vec4 color = texture(colorMap, texCoord);
#endif

#ifdef UsingBloom
	vec4 bloom = texture(bloomMap, texCoord);
	color.rgb += bloom.rgb * bloomIntensity;
#endif

	color.rgb = vec3(1.0) - exp(-color.rgb * exposure);

# ifdef UsingGamma
	color.rgb = pow(color.rgb, vec3(gammaInv));
# endif
	fragColor = color;
}

