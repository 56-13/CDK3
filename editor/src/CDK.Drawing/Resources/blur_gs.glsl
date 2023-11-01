//!#version 430

layout(triangles) in; 
layout(triangle_strip, max_vertices = 18) out; 

layout(std140, binding = 0) uniform block {
	vec4 kernel;
	vec2 direction;
};

out vec3 varTexCoord;
out vec3 varDirection;

void main() { 
	vec2 d = direction * 2.0;		//check

	varDirection = vec3(0.0, d);
	gl_Layer = 0;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(1.0, -pos.yx);
		gl_Position = pos; 
		EmitVertex(); 
	} 
	EndPrimitive(); 
	
	gl_Layer = 1;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(-1.0, -pos.y, pos.x);
		gl_Position = pos; 
		EmitVertex(); 
	}  
	EndPrimitive(); 
	
	varDirection = vec3(d.x, 0.0, d.y);
	gl_Layer = 2;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(pos.x, 1.0, pos.y);
		gl_Position = pos; 
		EmitVertex(); 
	} 
	EndPrimitive(); 

	gl_Layer = 3;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(pos.x, -1.0, -pos.y);
		gl_Position = pos; 
		EmitVertex(); 
	} 
	EndPrimitive(); 

	varDirection = vec3(d, 0.0);
	gl_Layer = 4;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(pos.x, -pos.y, 1.0);
		gl_Position = pos; 
		EmitVertex(); 
	} 
	EndPrimitive(); 

	gl_Layer = 5;
	for (int i = 0; i < 3; ++i) {
		vec4 pos = gl_in[i].gl_Position; 
		varTexCoord = vec3(-pos.xy, -1.0);
		gl_Position = pos; 
		EmitVertex(); 
	} 
	EndPrimitive(); 
}
