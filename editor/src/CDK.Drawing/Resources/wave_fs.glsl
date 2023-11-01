//!#version 430
//!#include "wave.glsl"

layout(binding = 0) uniform sampler2D screenMap;

in vec3 varOriginPos;
in vec4 varCenterPos;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec4 bloomColor;
#endif

void main() {
    vec2 screenCoord = gl_FragCoord.xy / resolution;
    vec2 centerCoord = vec2(varCenterPos.x / varCenterPos.w, varCenterPos.y / varCenterPos.w) * 0.5 + 0.5;
    
	float d = distance(varOriginPos, center);

	if (d <= radius + thickness && d >= radius - thickness) {
		float diff = (d - radius) / thickness * 0.1;

		float powDiff = 1.0 - pow(abs(diff * 10.0), 0.8);

		vec2 diffuv = normalize(screenCoord - centerCoord);

        fragColor = texture(screenMap, screenCoord + (diffuv * (diff * powDiff)));
#ifdef UsingBloomSupported
        bloomColor = vec4(0.0);
#endif
    }
    else discard;
}
