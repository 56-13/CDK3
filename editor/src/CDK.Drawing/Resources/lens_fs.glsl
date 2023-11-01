//!#include "lens.glsl"

layout(binding = 0) uniform sampler2D screenMap;

in vec3 varOriginPos;
in vec4 varCenterPos;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec4 bloomColor;
#endif

void main() {
    vec2 screenCoord = gl_FragCoord.xy / resolution;
    vec2 centerCoord = varCenterPos.xy / varCenterPos.w * 0.5 + 0.5;
    
	float d = distance(varOriginPos, center);

	if (d < radius) {
        float diff = 1.0 - (convex * (radius - d) / radius);
        vec2 diffuv = screenCoord - centerCoord;
        
        fragColor = texture(screenMap, centerCoord + diffuv * diff);

#ifdef UsingBloomSupported
        bloomColor = vec4(0.0);
#endif
	}
    else discard;
}
