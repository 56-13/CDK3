//!#version 430
//!#include "stroke.glsl"

#define PrimitivePoints		1
#define PrimitiveLines		2
#define PrimitiveTriangles	3

//!#define UsingPrimitive	PrimitiveTriangles

#if UsingPrimitive == PrimitivePoints
layout (points) in; 
layout (points, max_vertices = 9) out; 
#define PrimitiveVertexCount	1
#elif UsingPrimitive == PrimitiveLines
layout (lines) in; 
layout (line_strip, max_vertices = 18) out; 
#define PrimitiveVertexCount	2
#elif UsingPrimitive == PrimitiveTriangles
layout (triangles) in; 
layout (triangle_strip, max_vertices = 27) out; 
#define PrimitiveVertexCount	3
#else
# error "invalid primitive"
#endif

void main() { 
	for (float y = -1; y <= 1; y++) {
		for (float x = -1; x <= 1; x++) {
			for (int i = 0; i < PrimitiveVertexCount; i++) { 
				vec4 pos = gl_in[i].gl_Position; 
				pos.xy += vec2(x, y) * strokeScale * pos.w;
				EmitVertex(); 
			} 
			EndPrimitive(); 
		}
	}
}
