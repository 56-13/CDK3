#ifndef __CDK__CSAnimationFloatCurve__
#define __CDK__CSAnimationFloatCurve__

#include "CSAnimationFloat.h"

#include "CSMath.h"

#include "CSArray.h"

class CSBuffer;

struct CSAnimationFloatCurve : public CSAnimationFloat {
public:
    struct Point {
        float time;
        float value;
        float valueVar;
        float leftAngle;
        float rightAngle;

        Point(float time, float value, float valueVar, float leftAngle, float rightAngle);
        explicit Point(CSBuffer* buffer);
    };
private:
    mutable CSArray<Point> _points;
    mutable bool _sort = false;
public:
    CSAnimationFloatCurve() = default;
    explicit CSAnimationFloatCurve(CSBuffer* buffer);
    CSAnimationFloatCurve(const CSAnimationFloatCurve* other);

    static inline CSAnimationFloatCurve* factor() {
        return autorelease(new CSAnimationFloatCurve());
    }

    inline Type type() const override {
        return TypeCurve;
    }
    inline int resourceCost() const override {
        return sizeof(CSAnimationFloatCurve) + _points.capacity() * sizeof(Point);
    }
    float value(float t, float r) const override;
    float value(float t) const override;
    
    inline int pointCount() const {
        return _points.count();
    }
    inline Point& pointAtIndex(int index) {
        _sort = true;
        return _points.objectAtIndex(index);
    }
    inline const Point& pointAtIndex(int index) const {
        return _points.objectAtIndex(index);
    }
    inline Point& firstPoint() {
        _sort = true;
        return _points.objectAtIndex(0);
    }
    inline const Point& firstPoint() const {
        return _points.objectAtIndex(0);
    }
    inline Point& lastPoint() {
        _sort = true;
        return _points.lastObject();
    }
    inline const Point& lastPoint() const {
        return _points.lastObject();
    }
    void addPoint(const Point& point);
    inline void removePointAtIndex(int index) {
        _points.removeObjectAtIndex(index);
    }
    void linearRotatePointAtIndex(int index, bool left, bool right);
    inline void linearRotatePointAtIndex(int index) {
        linearRotatePointAtIndex(index, true, true);
    }
private:
    void sortPoints() const;

    inline float pointValue(const Point& point, float random) const {
        return point.valueVar ? CSMath::lerp(point.value - point.valueVar, point.value + point.valueVar, random) : point.value;
    }
};

#endif
