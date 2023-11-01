#define CDK_IMPL

#include "CSSpriteTimeline.h"

#include "CSBuffer.h"

#include "CSSprite.h"

CSSpriteTimeline::CSSpriteTimeline(float startTIme, float endTime, int layer) : 
	startTime(startTime), 
	endTime(endTime), 
	layer(layer),
	reset(false)
{

}

CSSpriteTimeline::CSSpriteTimeline(CSBuffer* buffer) :
	startTime(buffer->readFloat()),
	endTime(buffer->readFloat()),
	layer(buffer->readByte()),
	reset(buffer->readBoolean()),
	elements(buffer, CSSpriteElement::elementWithBufer)
{

}


CSSpriteTimeline::CSSpriteTimeline(const CSSpriteTimeline* other) :
	startTime(other->startTime),
	endTime(other->endTime),
	layer(other->layer),
	reset(other->reset),
	elements(other->elements.count()) 
{
	foreach (const CSSpriteElement*, element, &other->elements) elements.addObject(CSSpriteElement::elementWithElement(element));
}

int CSSpriteTimeline::resourceCost() const {
	int cost = sizeof(CSSpriteTimeline) + elements.capacity() * 8;
	foreach (const CSSpriteElement*, element, &elements) cost += element->resourceCost();
	return cost;
}

void CSSpriteTimeline::preload() const {
	foreach (const CSSpriteElement*, element, &elements) element->preload();
}
