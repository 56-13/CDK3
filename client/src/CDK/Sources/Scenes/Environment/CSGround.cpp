#define CDK_IMPL

#include "CSGround.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"
#include "CSRandom.h"
#include "CSStreamRenderCommand.h"

CSGround::CSGround(CSBuffer* buffer) : 
	width(buffer->readShort()),
	height(buffer->readShort()),
	altitude(buffer->readShort()),
	gridSize(buffer->readShort()),
	gridColor(buffer, true),
	gridVisible(buffer->readBoolean()),
	material(CSMaterialSource::materialWithBuffer(buffer))
{
	
}

CSABoundingBox CSGround::space() const {
	float w = width * gridSize * 0.5f;
	float h = height * gridSize * 0.5f;
	float a = altitude * gridSize;
	return CSABoundingBox(CSVector3(-w, -h, 0), CSVector3(w, h, a));
}

bool CSGround::intersects(const CSRay& ray, CSCollisionFlags flags, float& distance, CSHit* hit) const {
	float d;
	if (ray.intersects(CSPlane(0, 0, 1, 0), d) && d < distance) {
		distance = d;
		if ((flags & CSCollisionFlagHit) && hit) {
			hit->position = ray.position + ray.direction * distance;
			hit->direction = CSVector3::UnitZ;
		}
		return true;
	}

	return false;
}

void CSGround::draw(CSGraphics* graphics, CSInstanceLayer layer, float progress, int random) const {
	if (CSMaterialSource::apply(material, graphics, layer, progress, random, NULL, true)) {
		float w = width * gridSize * 0.5f;
		float h = height * gridSize * 0.5f;
		graphics->drawRect(CSRect(-w, -h, w * 2, h * 2), true);

		if (gridVisible && gridSize > 1) {
			graphics->material().blendMode = CSBlendAlpha;

			CSStreamRenderCommand* command = new CSStreamRenderCommand(graphics, CSPrimitiveLines);

			CSColor centerForeColor(gridColor, 1.0f);
			CSColor nearForeColor(gridColor, 0.75f);

			int vi = 0;
			int c = width / 2;
			for (int x = -c; x <= c; x++) {
				command->state.material.color = x == 0 ? centerForeColor : nearForeColor;
				command->addIndex(vi++);
				command->addIndex(vi++);
				command->addVertex(CSFVertex(CSVector3(x * gridSize, -w, 0.1f)));
				command->addVertex(CSFVertex(CSVector3(x * gridSize, w, 0.1f)));
			}
			c = height / 2;
			for (int y = -c; y <= c; y++) {
				command->state.material.color = y == 0 ? centerForeColor : nearForeColor;
				command->addIndex(vi++);
				command->addIndex(vi++);
				command->addVertex(CSFVertex(CSVector3(-w, y * gridSize, 0.1f)));
				command->addVertex(CSFVertex(CSVector3(w, y * gridSize, 0.1f)));
			}

			graphics->command(command);
		}

		graphics->pop();
	}
}


