#define CDK_IMPL

#include "CSTouch.h"

#include "CSTime.h"

#include "CSLayer.h"

CSTouch::CSTouch(uint64 key, CSTouchButton button, const CSVector2& point) :
    _key(key),
	_button(button),
    _firstPoint(point),
    _prevPoint(point),
    _point(point),
    _state(CSTouchStateStopped),
    _timestamp(CSTime::currentTime())
{

}

void CSTouch::setPoint(const CSVector2& point) {
    _prevPoint = _point;
    _point = point;
    _state = CSTouchStateMoved;
}

CSVector2 CSTouch::convertToLayerSpace(const CSLayer* layer, CSVector2 point) const {
    if (layer) layer->convertToLocalSpace(point);
    return point;
}

CSVector2 CSTouch::firstPoint(const CSLayer* layer) const {
    return layer ? convertToLayerSpace(layer, _firstPoint) : _firstPoint;
}
CSVector2 CSTouch::prevPoint(const CSLayer* layer) const {
    return layer ? convertToLayerSpace(layer, _prevPoint) : _prevPoint;
}
CSVector2 CSTouch::point(const CSLayer* layer) const {
    return layer ? convertToLayerSpace(layer, _point) : _point;
}

float CSTouch::time() const {
    return CSTime::currentTime() - _timestamp;
}
