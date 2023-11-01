//!#version 430
//!#include "shadow.glsl"

#define PrimitivePoints		1
#define PrimitiveLines		2
#define PrimitiveTriangles	3

//!#define UsingPrimitive	PrimitiveTriangles

#if UsingPrimitive == PrimitivePoints
layout (points) in; 
layout (points, max_vertices = 6) out; 
#define PrimitiveVertexCount	1
#elif UsingPrimitive == PrimitiveLines
layout (lines) in; 
layout (line_strip, max_vertices = 12) out; 
#define PrimitiveVertexCount	2
#elif UsingPrimitive == PrimitiveTriangles
layout (triangles) in; 
layout (triangle_strip, max_vertices = 18) out; 
#define PrimitiveVertexCount	3
#else
# error "invalid primitive"
#endif

out vec4 varShadowPos;

void main() { 
	for (int face = 0; face < 6; face++) { 
		gl_Layer = face;
		for (int i = 0; i < PrimitiveVertexCount; i++) { 
			vec4 pos = gl_in[i].gl_Position; 
			varShadowPos = pos;
			gl_Position = shadowViewProjections[face] * pos; 
			EmitVertex(); 
		} 
		EndPrimitive(); 
	} 
}
