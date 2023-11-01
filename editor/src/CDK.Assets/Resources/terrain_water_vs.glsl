//!#include "../../CDK.Drawing/Resources/base.glsl"
//!#include "terrain_water.glsl"

layout(location = 0) in vec3 attrPosition;

out vec3 varPos;
out vec2 varTexCoord;
out vec4 varWavePos;
out vec2 varFoamTexCoord;
out vec3 varNormal;
out vec3 varTangent;
out vec4 varDirectionalLightShadowPos;
out vec4 varDirectionalLightShadow2DPos;

void main() {
    vec4 pos = vec4(attrPosition * positionScale, 1.0);

#ifdef UsingWave
# ifdef UsingTransparency
	varWavePos = viewProjection * world * pos;
# endif
	vec2 gridPos = wave.x * attrPosition.xy + progress;
	pos.z += sin(gridPos.x) * cos(gridPos.y) * wave.y;
	pos = world * pos;

	gl_Position = viewProjection * pos;

# ifdef UsingNormal
	vec3 normal;
	normal.x = cos(gridPos.x) * cos(gridPos.y) * wave.x * wave.y;
	normal.y = sin(gridPos.x) * sin(gridPos.y) * wave.x * wave.y;
	normal.z = positionScale;
	normal = normalize(vec3(world * vec4(normal, 0.0)));
	varNormal = normal;
#  ifdef UsingNormalMap
	varTangent = cross(vec3(0.0, 1.0, 0.0), normal);
#  endif
# endif
#else
	pos = world * pos;

	vec4 vpos = viewProjection * pos;
# ifdef UsingTransparency
	varWavePos = vpos;
# endif
	gl_Position = vpos;

# ifdef UsingNormal
	varNormal = normalize(vec3(world[2][0], world[2][1], world[2][2]));
#  ifdef UsingNormalMap
	varTangent = normalize(vec3(world[0][0], world[0][1], world[0][2]));
#  endif
# endif
#endif

#ifdef UsingLight
	varPos = pos.xyz / pos.w;
# ifdef UsingDirectionalLightShadow
    varDirectionalLightShadowPos = directionalLightShadowViewProjection * pos;
# endif
# ifdef UsingDirectionalLightShadow2D
    varDirectionalLightShadow2DPos = directionalLightShadow2DViewProjection * pos;
# endif
#endif

#ifdef UsingMap
	varTexCoord = attrPosition.xy * textureScale + textureFlowForward * progress;
#endif

#ifdef UsingFoam
	varFoamTexCoord = attrPosition.xy * foamScale + foamFlowForward * progress;
#endif
}