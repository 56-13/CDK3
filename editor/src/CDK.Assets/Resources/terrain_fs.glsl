//!#include "../../CDK.Drawing/Resources/base_fs.glsl"
//!#include "terrain.glsl"

in vec2 varTerrainCoord;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec3 bloomColor;
#endif

void main() {
#ifdef UsingSurface
# ifdef UsingColorMap
	vec4 color = sampleTexture(colorMap, varTexCoord) * baseColor;
# else
	vec4 color = baseColor;
# endif
    color.a *= texture(intensityMap, varTerrainCoord).r;
#else
	vec4 color = baseColor;
#endif
#ifdef UsingLightMode
	float ao = pow(texture(ambientOcclusionMap, varTerrainCoord).r, ambientOcclusionIntensity);

    processColor(color.rgb, ao);
#endif
	color.rgb *= color.a;

	fragColor = color;

#ifdef UsingBloomSupported
    bloomColor = processBloom(color.rgb);
#endif
}

