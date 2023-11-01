#ifndef __CDK__CSVertexArrayDraw__
#define __CDK__CSVertexArrayDraw__

#include "CSVertexArray.h"

struct CSVertexArrayDraw {
	enum Type : byte {
		Arrays,
		Elements,
		ElementsInstanced
	};
	CSPtr<CSVertexArray> vertices;
	Type type;
	CSPrimitiveMode mode;
	int offset;
	int count;
	int instanceCount;
private:
	CSVertexArrayDraw(CSVertexArray* vertices, Type type, CSPrimitiveMode mode, int offset, int count, int instanceCount);
public:
	static inline CSVertexArrayDraw array(CSVertexArray* vertices, CSPrimitiveMode mode, int vertexOffset, int vertexCount) {
		return CSVertexArrayDraw(vertices, Arrays, mode, vertexOffset, vertexCount, 0);
	}
	static inline CSVertexArrayDraw elements(CSVertexArray* vertices, CSPrimitiveMode mode) {
		return CSVertexArrayDraw(vertices, Elements, mode, 0, vertices->indexBuffer()->count(), 0);
	}
	static inline CSVertexArrayDraw elements(CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount) {
		return CSVertexArrayDraw(vertices, Elements, mode, indexOffset, indexCount, 0);
	}
	static inline CSVertexArrayDraw elementsInstanced(CSVertexArray* vertices, CSPrimitiveMode mode, int instanceCount) {
		return CSVertexArrayDraw(vertices, ElementsInstanced, mode, 0, vertices->indexBuffer()->count(), instanceCount);
	}
	static inline CSVertexArrayDraw elementsInstanced(CSVertexArray* vertices, CSPrimitiveMode mode, int indexOffset, int indexCount, int instanceCount) {
		return CSVertexArrayDraw(vertices, ElementsInstanced, mode, indexOffset, indexCount, instanceCount);
	}

	void draw(CSGraphicsApi* api) const;
};

#endif