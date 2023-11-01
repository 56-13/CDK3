//!#include "../../CDK.Drawing/Resources/base_fs.glsl"
//!#include "terrain_water.glsl"

in vec4 varWavePos;
in vec2 varFoamTexCoord;

layout(location = 0) out vec4 fragColor;

#ifdef UsingBloomSupported
layout(location = 1) out vec3 bloomColor;
#endif


float convertToLinearDepth(float depth) {
	return (near * far) / (far - depth * (far - near));
}

vec4 sampleWaterTexture(sampler2D map) {
# ifdef UsingCross
	vec2 uvoff = textureFlowCross * progress;
	return (texture(map, varTexCoord + uvoff) + texture(map, varTexCoord - uvoff)) * 0.5;
# else
	return texture(map, varTexCoord);
# endif
}

vec3 sampleFoamTexture() {
#ifdef UsingCross
	vec2 uvoff = foamFlowCross * progress;
	return (texture(foamMap, varFoamTexCoord + uvoff).rgb + texture(foamMap, varFoamTexCoord - uvoff).rgb) * 0.5;
#else
	return texture(foamMap, varFoamTexCoord).rgb;
#endif
}

void main() {
	vec4 color;
#ifdef UsingColorMap
	color = sampleWaterTexture(colorMap) * baseColor;
#else
	color = baseColor;
#endif

#ifdef UsingNormalMap
	vec3 nn = sampleWaterTexture(normalMap).rgb * 2.0 - 1.0;
#endif

#ifdef UsingLightMode
	vec3 n = normalize(varNormal);
# ifdef UsingNormalMap
	vec3 t = normalize(varTangent);
    mat3 tbn = mat3(t, cross(n, t), n);
    n = normalize(tbn * nn);
# endif
	vec4 material = materialFactor;
# ifdef UsingMaterialMap
    sampleMaterial(material, sampleWaterTexture(materialMap));
# endif
	processLight(color.rgb, n, material);
#endif
	vec2 originCoord = gl_FragCoord.xy / resolution;

#ifdef UsingTransparency	
	vec2 destCoord = varWavePos.xy / varWavePos.w * 0.5 + 0.5;
# ifdef UsingNormalMap
	destCoord += nn.xy * perturbIntensity;
# endif
#endif

#ifdef UsingDepth
	float d = convertToLinearDepth(texture(depthMap, originCoord).r) - (1.0 / gl_FragCoord.w);

	float a = smoothstep(0.0, depthMax, d);

	color = mix(shallowColor, color, a);
# ifdef UsingTransparency
	destCoord = mix(originCoord, destCoord, a);
# endif
#endif

#ifdef UsingFoam
	vec3 foamColor = sampleFoamTexture();

	color.rgb *= (foamColor * 2.0 - 1.0) * foamIntensity + 1.0;
#endif
	
#ifdef UsingTransparency
	vec3 destColor = texture(destMap, destCoord).rgb;
	color.rgb = mix(destColor, color.rgb, color.a);
#endif
	color.a = 1.0;

#ifdef UsingFoamDepth
	if (d <= foamDepth) {
		color.rgb += max(foamColor - 0.5, 0.0) * (1.0 - d / foamDepth);
	}
#endif

#ifdef UsingEmission
# ifdef UsingEmissiveMap
    color.rgb += sampleWaterTexture(emissiveMap, varTexCoord).rgb * emissiveFactor;
# else
    color.rgb += emissiveFactor;
# endif
#endif
	
	processFilter(color.rgb);

	fragColor = color;

#ifdef UsingBloomSupported
    bloomColor = processBloom(color.rgb);
#endif
}
