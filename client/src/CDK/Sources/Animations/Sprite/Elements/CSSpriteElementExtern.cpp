#define CDK_IMPL

#include "CSSpriteElementExtern.h"
#include "CSSprite.h"
#include "CSAnimation.h"
#include "CSBuffer.h"

CSSpriteElementExtern::CSSpriteElementExtern(CSBuffer* buffer) : data(buffer->readData(buffer->readLength())) {

}

CSSpriteElementExtern::CSSpriteElementExtern(const CSSpriteElementExtern* other) : data(other->data) {

}

int CSSpriteElementExtern::resourceCost() const {
	int cost = sizeof(CSSpriteElementExtern);
	if (data) cost += sizeof(CSData) + data->capacity();
	return cost;
}

void CSSpriteElementExtern::draw(DrawParam& param) const {
	const byte* bytes = data ? static_cast<const byte*>(data->bytes()) : NULL;

	if (param.parent->externDelegate) param.parent->externDelegate(param.graphics, bytes);
	else {
		const CSAnimationObjectFragment* anifrag = param.parent->animation();
		if (anifrag) {
			const CSAnimationObject* ani = anifrag->root();
			if (ani && ani->spriteExternDelegate)  ani->spriteExternDelegate(param.graphics, bytes);
		}
	}
}
