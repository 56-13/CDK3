#define CDK_IMPL

#include "CSAnimationFloatCurve.h"

#include "CSBuffer.h"

CSAnimationFloatCurve::Point::Point(float time, float value, float valueVar, float leftAngle, float rightAngle) :
    time(time), 
    value(value), 
    valueVar(valueVar), 
    leftAngle(leftAngle), 
    rightAngle(rightAngle) 
{
    CSAssert(time >= 0 && time <= 1);
}

CSAnimationFloatCurve::Point::Point(CSBuffer* buffer) :
    time(buffer->readFloat()),
    value(buffer->readFloat()),
    valueVar(buffer->readFloat()),
    leftAngle(buffer->readFloat()),
    rightAngle(buffer->readFloat())
{

}

CSAnimationFloatCurve::CSAnimationFloatCurve(const CSAnimationFloatCurve* other) :
    _points(other->_points.capacity()),
    _sort(other->_sort) 
{
    _points.addObjectsFromArray(&other->_points);
}

CSAnimationFloatCurve::CSAnimationFloatCurve(CSBuffer* buffer) : 
    _points(buffer) 
{

}

void CSAnimationFloatCurve::sortPoints() const {
    if (_sort) {
        _points.sort([](const Point& a, const Point& b) -> int {
            if (a.time < b.time) return 1;
            else if (a.time > b.time) return -1;
            else return 0;
        });
        _sort = false;
    }
}

float CSAnimationFloatCurve::value(float t, float random) const {
    sortPoints();
    
    if (_points.count() < 2 || t <= _points.objectAtIndex(0).time) {
        return pointValue(_points.objectAtIndex(0), random);
    }
    if (t >= _points.lastObject().time) {
        return pointValue(_points.lastObject(), random);
    }
    int i;
    for (i = 1; i < _points.count() - 1; i++) {
        const Point& p = _points.objectAtIndex(i);
        
        if (p.time == t) return pointValue(p, random);
        else if (p.time > t) break;
    }
    const Point& p0 = _points.objectAtIndex(i - 1);
    const Point& p3 = _points.objectAtIndex(i);
    float d = p3.time - p0.time;
    
    float v0 = pointValue(p0, random);
    float v3 = pointValue(p3, random);
    if (p0.rightAngle >= FloatPiOverTwo || p3.leftAngle >= FloatPiOverTwo) {
        return CSMath::max(v0, v3);
    }
    if (p0.rightAngle <= -FloatPiOverTwo || p3.leftAngle <= -FloatPiOverTwo) {
        return CSMath::min(v0, v3);
    }
    float v1 = v0 + CSMath::tan(p0.rightAngle) * (d / 3.0f);
    float v2 = v3 - CSMath::tan(p3.leftAngle) * (d / 3.0f);
    
    t = (t - p0.time) / d;
    
    float rt = 1.0f - t;
    
    return (rt * rt * rt * v0) + (3.0f * rt * rt * t * v1) + (3.0f * rt * t * t * v2) + (t * t * t * v3);
}

float CSAnimationFloatCurve::value(float t) const {
    sortPoints();
    
    if (_points.count() < 2 || t <= _points.objectAtIndex(0).time) {
        return _points.objectAtIndex(0).value;
    }
    if (t >= _points.lastObject().time) {
        return _points.lastObject().value;
    }
    int i;
    for (i = 1; i < _points.count() - 1; i++) {
        const Point& p = _points.objectAtIndex(i);
        
        if (p.time == t) return p.value;
        else if (p.time > t) break;
    }
    const Point& p0 = _points.objectAtIndex(i - 1);
    const Point& p3 = _points.objectAtIndex(i);
    float d = p3.time - p0.time;
    
    float v1 = p0.value + CSMath::tan(p0.rightAngle) * (d / 3.0f);
    float v2 = p3.value - CSMath::tan(p3.leftAngle) * (d / 3.0f);
    
    t = (t - p0.time) / d;
    
    float rt = 1.0f - t;
    
    float v = (rt * rt * rt * p0.value) + (3.0f * rt * rt * t * v1) + (3.0f * rt * t * t * v2) + (t * t * t * p3.value);
    
    return v;
}

void CSAnimationFloatCurve::addPoint(const Point& point) {
    sortPoints();
    
    int i;
    for (i = 0; i < _points.count(); i++) {
        Point& p = _points.objectAtIndex(i);
        
        if (p.time == point.time) {
            p.value = point.value;
            p.valueVar = point.valueVar;
            p.leftAngle = point.leftAngle;
            p.rightAngle = point.rightAngle;
        }
        else if (p.time > point.time) break;
    }
    _points.insertObject(i, point);
}

void CSAnimationFloatCurve::linearRotatePointAtIndex(int index, bool left, bool right) {
    float a0 = 0;
    float a1 = 0;
    
    Point& p = _points.objectAtIndex(index);
    if (index > 0) {
        const Point& p0 = _points.objectAtIndex(index - 1);
        a0 = CSMath::atan2(p.value - p0.value, p.time - p0.time);
    }
    if (index < _points.count() - 1) {
        const Point& p1 = _points.objectAtIndex(index + 1);
        a1 = CSMath::atan2(p1.value - p.value, p1.time - p.time);
    }
    if (index == 0) a0 = a1;
    else if (index == _points.count() - 1) a1 = a0;
    
    if (left && right) p.leftAngle = p.rightAngle = (a0 + a1) * 0.5f;
    else if (left) p.leftAngle = a1;
    else if (right) p.rightAngle = a0;
}
