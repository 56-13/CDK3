#define CDK_IMPL

#include "CSVertexArrayDraw.h"

CSVertexArrayDraw::CSVertexArrayDraw(CSVertexArray* vertices, Type type, CSPrimitiveMode mode, int offset, int count, int instanceCount) : 
	vertices(vertices), 
	type(type), 
	mode(mode), 
	offset(offset), 
	count(count), 
	instanceCount(instanceCount) 
{

}

void CSVertexArrayDraw::draw(CSGraphicsApi* api) const {
	switch (type) {
		case Arrays:
			vertices->drawArrays(api, mode, offset, count);
			break;
		case Elements:
			vertices->drawElements(api, mode, offset, count);
			break;
		case ElementsInstanced:
			vertices->drawElementsInstanced(api, mode, offset, count, instanceCount);
			break;
	}
}
