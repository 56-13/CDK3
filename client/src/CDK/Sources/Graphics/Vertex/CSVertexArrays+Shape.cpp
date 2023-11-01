#define CDK_IMPL

#include "CSVertexArrays.h"

#include "CSVertex.h"
#include "CSGBufferData.h"
#include "CSResourcePool.h"
#include "CSGraphics.h"

CSVertexArray* CSVertexArrays::get2D(const CSVector2* positions, int count) {
    CSGBufferData<CSVector2>* data = new CSGBufferData<CSVector2>(count);
    
    data->addObjectsFromPointer(positions, count);
    
    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(data));

    if (!vertices) {
        static const CSVertexLayout layout[] = {
            CSVertexLayout(0, 0, 2, CSVertexAttribTypeFloat, false, 8, 0, 0, true)
        };

        vertices = get(data, 1, true, 1, true, layout, 1);

        vertices->vertexBuffer(0)->upload(data, CSGBufferUsageHintDynamicDraw);
    }

    data->release();

    return vertices;
}

CSVertexArray* CSVertexArrays::get2D(int count, CSVector2 position, ...) {
    CSVector2* positions = (CSVector2*)alloca(count * sizeof(CSVector2));
    positions[0] = position;
    
    va_list ap;
    va_start(ap, position);
    for (int i = 1; i < count; i++) positions[i] = va_arg(ap, CSVector2);
    va_end(ap);

    return get2D(positions, count);
}

CSVertexArray* CSVertexArrays::get2D(const CSArray<CSVector2>* positions) {
    return get2D(positions->pointer(), positions->count());
}

CSVertexArray* CSVertexArrays::get2D(const CSRect& bounds) {
    CSVector2 quad[] = {
        bounds.leftTop(),
        bounds.rightTop(),
        bounds.leftBottom(),
        bounds.rightBottom()
    };
    return get2D(quad, 4);
}

CSVertexArray* CSVertexArrays::getScreen2D() {
    static const CSVector2 quad[] = {
        CSVector2(-1, -1),
        CSVector2(1, -1),
        CSVector2(-1, 1),
        CSVector2(1, 1)
    };
    return get2D(quad, 4);
}

CSVertexArray* CSVertexArrays::get3D(const CSVector3* positions, int count) {
    CSGBufferData<CSVector3>* data = new CSGBufferData<CSVector3>(count);

    data->addObjectsFromPointer(positions, count);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(data));

    if (!vertices) {
        static const CSVertexLayout layout[] = {
            CSVertexLayout(0, 0, 3, CSVertexAttribTypeFloat, false, 12, 0, 0, true)
        };

        vertices = get(data, 1, true, 1, true, layout, 1);

        vertices->vertexBuffer(0)->upload(data, CSGBufferUsageHintDynamicDraw);
    }

    data->release();

    return vertices;
}

CSVertexArray* CSVertexArrays::get3D(int count, CSVector3 position, ...) {
    CSVector3* positions = (CSVector3*)alloca(count * sizeof(CSVector3));
    positions[0] = position;

    va_list ap;
    va_start(ap, position);
    for (int i = 1; i < count; i++) positions[i] = va_arg(ap, CSVector3);
    va_end(ap);

    return get3D(positions, count);
}

CSVertexArray* CSVertexArrays::get3D(const CSArray<CSVector3>* positions) {
    return get3D(positions->pointer(), positions->count());
}

//==============================================================================================================================
class ShapeKey : public CSObject {
private:
    enum {
        Rect,
        GradientRect,
        RoundRect,
        GradientRoundRect,
        Arc,
        GradientArc,
        Sphere,
        Capsule,
        Cylinder,
        Hexahedron
    } _type;
    union var {
        struct {
            CSZRect rect;
            bool fill;
            CSRect uv;
        } rect;
        struct {
            CSZRect rect;
            CSColor color[4];
            bool fill;
            CSRect uv;
        } gradientRect;
        struct {
            CSZRect rect;
            float radius;
            bool fill;
            CSCorner corner;
            CSRect uv;
        } roundRect;
        struct {
            CSZRect rect;
            float radius;
            CSColor color[4];
            bool fill;
            CSCorner corner;
            CSRect uv;
        } gradientRoundRect;
        struct {
            CSZRect rect;
            float angle1;
            float angle2;
            bool fill;
            CSRect uv;
        } arc;
        struct {
            CSZRect rect;
            float angle1;
            float angle2;
            CSColor color[2];
            CSRect uv;
        } gradientArc;
        struct {
            CSVector3 pos;
            float radius;
            CSRect uv;
        } sphere;
        struct {
            CSVector3 pos;
            float height;
            float radius;
            CSRect uv;
        } capsule;
        struct {
            CSVector3 pos;
            float topRadius;
            float bottomRadius;
            float height;
            CSRect uv;
        } cylinder;
        struct {
            CSZRect topRect;
            CSZRect bottomRect;
            CSRect uv;
        } hexahedron;
    } _var;
    
    ShapeKey() {
        memset(&_var, 0, sizeof(var));
    }
    ~ShapeKey() = default;
public:
    static ShapeKey* rect(const CSZRect& rect, bool fill, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Rect;
        key->_var.rect.rect = rect;
        key->_var.rect.fill = fill;
        key->_var.rect.uv = uv;
        return key;
    }
    static ShapeKey* gradientRect(const CSZRect& rect, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = GradientRect;
        key->_var.gradientRect.rect = rect;
        key->_var.gradientRect.color[0] = leftTopColor;
        key->_var.gradientRect.color[1] = rightTopColor;
        key->_var.gradientRect.color[2] = leftBottomColor;
        key->_var.gradientRect.color[3] = rightBottomColor;
        key->_var.gradientRect.fill = fill;
        key->_var.gradientRect.uv = uv;
        return key;
    }
    static ShapeKey* roundRect(const CSZRect& rect, float radius, bool fill, CSCorner corner, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = RoundRect;
        key->_var.roundRect.rect = rect;
        key->_var.roundRect.radius = radius;
        key->_var.roundRect.fill = fill;
        key->_var.roundRect.corner = corner;
        key->_var.roundRect.uv = uv;
        return key;
    }
    static ShapeKey* gradientRoundRect(const CSZRect& rect, float radius, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, CSCorner corner, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = GradientRoundRect;
        key->_var.gradientRoundRect.rect = rect;
        key->_var.gradientRoundRect.radius = radius;
        key->_var.gradientRoundRect.fill = fill;
        key->_var.gradientRoundRect.corner = corner;
        key->_var.gradientRoundRect.uv = uv;
        return key;
    }
    static ShapeKey* arc(const CSZRect& rect, float angle1, float angle2, bool fill, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Arc;
        key->_var.arc.rect = rect;
        key->_var.arc.angle1 = angle1;
        key->_var.arc.angle2 = angle2;
        key->_var.arc.fill = fill;
        key->_var.arc.uv = uv;
        return key;
    }
    static ShapeKey* gradientArc(const CSZRect& rect, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = GradientArc;
        key->_var.gradientArc.rect = rect;
        key->_var.gradientArc.angle1 = angle1;
        key->_var.gradientArc.angle2 = angle2;
        key->_var.gradientArc.color[0] = centerColor;
        key->_var.gradientArc.color[1] = surroundColor;
        key->_var.gradientArc.uv = uv;
        return key;
    }
    static ShapeKey* sphere(const CSVector3& pos, float radius, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Sphere;
        key->_var.sphere.pos = pos;
        key->_var.sphere.radius = radius;
        key->_var.sphere.uv = uv;
        return key;
    }
    static ShapeKey* capsule(const CSVector3& pos, float height, float radius, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Capsule;
        key->_var.capsule.pos = pos;
        key->_var.capsule.height = height;
        key->_var.capsule.radius = radius;
        key->_var.capsule.uv = uv;
        return key;
    }
    static ShapeKey* cylinder(const CSVector3& pos, float topRadius, float bottomRadius, float height, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Cylinder;
        key->_var.cylinder.pos = pos;
        key->_var.cylinder.topRadius = topRadius;
        key->_var.cylinder.bottomRadius = bottomRadius;
        key->_var.cylinder.height = height;
        key->_var.cylinder.uv = uv;
        return key;
    }
    static ShapeKey* hexahedron(const CSZRect& topRect, const CSZRect& bottomRect, const CSRect& uv) {
        ShapeKey* key = autorelease(new ShapeKey());
        key->_type = Hexahedron;
        key->_var.hexahedron.topRect = topRect;
        key->_var.hexahedron.bottomRect = bottomRect;
        key->_var.hexahedron.uv = uv;
        return key;
    }

    uint hash() const override {
        CSHash hash;
        hash.combine(_type);
        const float* p = (const float*)&_var;
        constexpr int count = sizeof(_var) / sizeof(float);
        for (int i = 0; i < count; i++) hash.combine(p[i]);
        return hash;
    }

    bool isEqual(const ShapeKey* other) const {
        return _type == other->_type && memcmp(&_var, &other->_var, sizeof(_var)) == 0;
    }

    bool isEqual(const CSObject* obj) const override {
        const ShapeKey* other = dynamic_cast<const ShapeKey*>(obj);

        return isEqual(other);
    }
};

//==============================================================================================================================

CSVertexArray* CSVertexArrays::getRect(int life, const CSZRect& rect, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::rect(rect, fill, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>(4);
        CSVertexIndexData* indexData = new CSVertexIndexData(4, fill ? 6 : 8);

        vertexData->addObject(CSVertexTNT(rect.leftTop(), uv.leftTop()));
        vertexData->addObject(CSVertexTNT(rect.rightTop(), uv.rightTop()));
        vertexData->addObject(CSVertexTNT(rect.leftBottom(), uv.leftBottom()));
        vertexData->addObject(CSVertexTNT(rect.rightBottom(), uv.rightBottom()));

        if (fill) {
            indexData->addObject(0);
            indexData->addObject(1);
            indexData->addObject(2);
            indexData->addObject(1);
            indexData->addObject(3);
            indexData->addObject(2);
        }
        else {
            indexData->addObject(0);
            indexData->addObject(1);
            indexData->addObject(1);
            indexData->addObject(3);
            indexData->addObject(3);
            indexData->addObject(2);
            indexData->addObject(2);
            indexData->addObject(0);
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getGradientRect(int life, const CSZRect& rect, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::gradientRect(rect, leftTopColor, rightTopColor, leftBottomColor, rightBottomColor, fill, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertex::SingleBufferVertexLayouts);

        CSGBufferData<CSVertex>* vertexData = new CSGBufferData<CSVertex>(4);
        CSVertexIndexData* indexData = new CSVertexIndexData(4, fill ? 6 : 8);

        vertexData->addObject(CSVertex(rect.leftTop(), leftTopColor, uv.leftTop()));
        vertexData->addObject(CSVertex(rect.rightTop(), rightTopColor, uv.rightTop()));
        vertexData->addObject(CSVertex(rect.leftBottom(), leftBottomColor, uv.leftBottom()));
        vertexData->addObject(CSVertex(rect.rightBottom(), rightBottomColor, uv.rightBottom()));

        if (fill) {
            indexData->addObject(0);
            indexData->addObject(1);
            indexData->addObject(2);
            indexData->addObject(1);
            indexData->addObject(3);
            indexData->addObject(2);
        }
        else {
            indexData->addObject(0);
            indexData->addObject(1);
            indexData->addObject(1);
            indexData->addObject(3);
            indexData->addObject(3);
            indexData->addObject(2);
            indexData->addObject(2);
            indexData->addObject(0);
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }
    
    return vertices;
}

CSVertexArray* CSVertexArrays::getGradientRectH(int life, const CSZRect& rect, const CSColor& leftColor, const CSColor& rightColor, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRect(life, rect, leftColor, rightColor, leftColor, rightColor, fill, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientRectV(int life, const CSZRect& rect, const CSColor& topColor, const CSColor& bottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRect(life, rect, topColor, topColor, bottomColor, bottomColor, fill, uv, aabb);
}

CSVertexArray* CSVertexArrays::getRoundRect(int life, const CSZRect& rect, float radius, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb) {
    float minRadius = CSMath::min(rect.width, rect.height) * 0.5f;

    if (radius > minRadius) radius = minRadius;

    if (radius <= 0) return getRect(life, rect, fill, uv, aabb);
    
    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    ShapeKey* key = ShapeKey::roundRect(rect, radius, fill, corner, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(radius);

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>((int)(FloatPi / d) + 1 + 4);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), fill ? (vertexData->capacity() - 2) * 3 : vertexData->capacity() * 2);

        float tw = radius * uv.width / rect.width;
        float th = radius * uv.height / rect.height;

        float s, e;

        if (corner & CSCornerLeftTop) {
            s = FloatPi;
            e = FloatPi * 1.5f;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.left() + radius + cos * radius;
                float sy = rect.top() + radius + sin * radius;
                float u = uv.left() + tw + cos * tw;
                float v = uv.top() + th + sin * th;

                vertexData->addObject(CSVertexTNT(CSVector3(sx, sy, rect.z), CSVector2(u, v)));
            }

            vertexData->addObject(CSVertexTNT(CSVector3(rect.left() + radius, rect.top(), rect.z), CSVector2(uv.left() + tw, uv.top())));
        }
        else {
            vertexData->addObject(CSVertexTNT(rect.leftTop(), uv.leftTop()));
        }

        if (corner & CSCornerRightTop) {
            s = FloatPi * 1.5f;
            e = FloatTwoPi;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.right() - radius + cos * radius;
                float sy = rect.top() + radius + sin * radius;
                float u = uv.right() - tw + cos * tw;
                float v = uv.top() + th + sin * th;

                vertexData->addObject(CSVertexTNT(CSVector3(sx, sy, rect.z), CSVector2(u, v)));
            }

            vertexData->addObject(CSVertexTNT(CSVector3(rect.right(), rect.top() + radius, rect.z), CSVector2(uv.right(), uv.top() + th)));
        }
        else {
            vertexData->addObject(CSVertexTNT(rect.rightTop(), uv.rightTop()));
        }

        if (corner & CSCornerRightBottom) {
            s = 0;
            e = FloatPiOverTwo;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.right() - radius + cos * radius;
                float sy = rect.bottom() - radius + sin * radius;
                float u = uv.right() - tw + cos * tw;
                float v = uv.bottom() - th + sin * th;
                vertexData->addObject(CSVertexTNT(CSVector3(sx, sy, rect.z), CSVector2(u, v)));
            }

            vertexData->addObject(CSVertexTNT(CSVector3(rect.right() - radius, rect.bottom(), rect.z), CSVector2(uv.right() - tw, uv.bottom())));
        }
        else {
            vertexData->addObject(CSVertexTNT(rect.rightBottom(), uv.rightBottom()));
        }

        if (corner & CSCornerLeftBottom) {
            s = FloatPiOverTwo;
            e = FloatPi;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.left() + radius + cos * radius;
                float sy = rect.bottom() - radius + sin * radius;
                float u = uv.left() + tw + cos * tw;
                float v = uv.bottom() - th + sin * th;
                vertexData->addObject(CSVertexTNT(CSVector3(sx, sy, rect.z), CSVector2(u, v)));
            }

            vertexData->addObject(CSVertexTNT(CSVector3(rect.left(), rect.bottom() - radius, rect.z), CSVector2(uv.left(), uv.bottom() - th)));
        }
        else {
            vertexData->addObject(CSVertexTNT(rect.leftBottom(), uv.leftBottom()));
        }

        if (fill) {
            for (int i = 0; i < vertexData->count() - 2; i++) {
                indexData->addObject(0);
                indexData->addObject(i + 1);
                indexData->addObject(i + 2);
            }
        }
        else {
            for (int i = 0; i < vertexData->count() - 1; i++) {
                indexData->addObject(i);
                indexData->addObject(i + 1);
            }
            indexData->addObject(vertexData->count() - 1);
            indexData->addObject(0);
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getRoundRect(int life, const CSZRect& rect, float radius, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getRoundRect(life, rect, radius, fill, CSCornerAll, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientRoundRect(int life, const CSZRect& rect, float radius, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb) {
    float minRadius = CSMath::min(rect.width, rect.height) * 0.5f;

    if (radius > minRadius) radius = minRadius;

    if (radius <= 0) return getGradientRect(life, rect, leftTopColor, rightTopColor, leftBottomColor, rightBottomColor, fill, uv, aabb);
    
    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    ShapeKey* key = ShapeKey::gradientRoundRect(rect, radius, leftTopColor, rightTopColor, leftBottomColor, rightBottomColor, fill, corner, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (!vertices) {

        vertices = get(key, life, false, 1, true, CSVertex::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(radius);

        CSGBufferData<CSVertex>* vertexData = new CSGBufferData<CSVertex>((int)(FloatPi / d) + 1 + 4 + 1);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), fill ? (vertexData->capacity() - 1) * 3 : vertexData->capacity() * 2);

        float tw = radius * uv.width / rect.width;
        float th = radius * uv.height / rect.height;

        if (fill) {
            vertexData->addObject(CSVertex(rect.centerMiddle(),
                (leftTopColor + rightTopColor + leftBottomColor + rightBottomColor) * 0.25f,
                uv.centerMiddle()));
        }

        float s, e;

        if (corner & CSCornerLeftTop) {
            s = FloatPi;
            e = FloatPi * 1.5f;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.left() + radius + cos * radius;
                float sy = rect.top() + radius + sin * radius;
                float xr = (sx - rect.left()) / rect.width;
                float yr = (sy - rect.top()) / rect.height;
                float u = uv.left() + tw + cos * tw;
                float v = uv.top() + th + sin * th;

                vertexData->addObject(CSVertex(CSVector3(sx, sy, rect.z),
                    CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr),
                    CSVector2(u, v)));
            }
            {
                float r = radius / rect.width;
                vertexData->addObject(CSVertex(CSVector3(rect.left() + radius, rect.top(), rect.z),
                    CSColor::lerp(leftTopColor, rightTopColor, r),
                    CSVector2(uv.left() + tw, uv.top())));
            }
        }
        else {
            vertexData->addObject(CSVertex(rect.leftTop(), leftTopColor, uv.leftTop()));
        }

        if (corner & CSCornerRightTop) {
            s = FloatPi * 1.5f;
            e = FloatTwoPi;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.right() - radius + cos * radius;
                float sy = rect.top() + radius + sin * radius;
                float xr = (sx - rect.left()) / rect.width;
                float yr = (sy - rect.top()) / rect.height;
                float u = uv.right() - tw + cos * tw;
                float v = uv.top() + th + sin * th;

                vertexData->addObject(CSVertex(CSVector3(sx, sy, rect.z),
                    CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr),
                    CSVector2(u, v)));
            }
            {
                float r = radius / rect.height;
                vertexData->addObject(CSVertex(CSVector3(rect.right(), rect.top() + radius, rect.z),
                    CSColor::lerp(rightTopColor, rightBottomColor, r),
                    CSVector2(uv.right(), uv.top() + th)));
            }
        }
        else {
            vertexData->addObject(CSVertex(rect.rightTop(), rightTopColor, uv.rightTop()));
        }

        if (corner & CSCornerRightBottom) {
            s = 0;
            e = FloatPiOverTwo;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.right() - radius + cos * radius;
                float sy = rect.bottom() - radius + sin * radius;
                float xr = (sx - rect.left()) / rect.width;
                float yr = (sy - rect.top()) / rect.height;
                float u = uv.right() - tw + cos * tw;
                float v = uv.bottom() - th + sin * th;

                vertexData->addObject(CSVertex(CSVector3(sx, sy, rect.z),
                    CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr),
                    CSVector2(u, v)));
            }
            {
                float r = (rect.width - radius) / rect.width;
                vertexData->addObject(CSVertex(CSVector3(rect.right() - radius, rect.bottom(), rect.z),
                    CSColor::lerp(leftBottomColor, rightBottomColor, r),
                    CSVector2(uv.right() - tw, uv.bottom())));
            }
        }
        else {
            vertexData->addObject(CSVertex(rect.rightBottom(), rightBottomColor, uv.rightBottom()));
        }

        if (corner & CSCornerLeftBottom) {
            s = FloatPiOverTwo;
            e = FloatPi;
            for (float a = s; a < e; a += d) {
                float cos = CSMath::cos(a);
                float sin = CSMath::sin(a);
                float sx = rect.left() + radius + cos * radius;
                float sy = rect.bottom() - radius + sin * radius;
                float xr = (sx - rect.left()) / rect.width;
                float yr = (sy - rect.top()) / rect.height;
                float u = uv.left() + tw + cos * tw;
                float v = uv.bottom() - th + sin * th;

                vertexData->addObject(CSVertex(CSVector3(sx, sy, rect.z),
                    CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr),
                    CSVector2(u, v)));
            }
            {
                float r = (rect.height - radius) / rect.height;
                vertexData->addObject(CSVertex(CSVector3(rect.left(), rect.bottom() - radius, rect.z),
                    CSColor::lerp(leftTopColor, leftBottomColor, r),
                    CSVector2(uv.left(), uv.bottom() - th)));
            }
        }
        else {
            vertexData->addObject(CSVertex(rect.leftBottom(), leftBottomColor, uv.leftBottom()));
        }

        if (fill) {
            for (int i = 0; i < vertexData->count() - 2; i++) {
                indexData->addObject(0);
                indexData->addObject(i + 1);
                indexData->addObject(i + 2);
            }
            indexData->addObject(0);
            indexData->addObject(vertexData->count() - 1);
            indexData->addObject(1);
        }
        else {
            for (int i = 0; i < vertexData->count() - 1; i++) {
                indexData->addObject(i);
                indexData->addObject(i + 1);
            }
            indexData->addObject(vertexData->count() - 1);
            indexData->addObject(0);
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getGradientRoundRectH(int life, const CSZRect& rect, float radius, const CSColor& leftColor, const CSColor& rightColor, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRoundRect(life, rect, radius, leftColor, rightColor, leftColor, rightColor, fill, CSCornerAll, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientRoundRectV(int life, const CSZRect& rect, float radius, const CSColor& topColor, const CSColor& bottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRoundRect(life, rect, radius, topColor, topColor, bottomColor, bottomColor, fill, CSCornerAll, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientRoundRectH(int life, const CSZRect& rect, float radius, const CSColor& leftColor, const CSColor& rightColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRoundRect(life, rect, radius, leftColor, rightColor, leftColor, rightColor, fill, corner, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientRoundRectV(int life, const CSZRect& rect, float radius, const CSColor& topColor, const CSColor& bottomColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientRoundRect(life, rect, radius, topColor, topColor, bottomColor, bottomColor, fill, corner, uv, aabb);
}

CSVertexArray* CSVertexArrays::getArc(int life, const CSZRect& rect, float angle1, float angle2, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    if (angle1 >= angle2) return NULL;

    ShapeKey* key = ShapeKey::arc(rect, angle1, angle2, fill, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (!vertices) {

        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(CSMath::max(rect.width, rect.height) * 0.5f);

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>((int)((angle2 - angle1) / d) + 3);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), fill ? (vertexData->capacity() - 2) * 3 : (vertexData->capacity() - 1) * 2);

        CSVector3 c = rect.centerMiddle();
        CSVector2 hs = rect.halfSize();
        CSVector2 uvc = uv.centerMiddle();
        CSVector2 uvhs = uv.halfSize();

        if (fill) {
            vertexData->addObject(CSVertexTNT(c, uvc));
        }
        float x, y, u, v, cosq, sinq;

        for (float a = angle1; a < angle2; a += d) {
            cosq = CSMath::cos(a);
            sinq = CSMath::sin(a);
            x = c.x + hs.x * cosq;
            y = c.y + hs.y * sinq;
            u = uvc.x + uvhs.x * cosq;
            v = uvc.y + uvhs.y * sinq;
            vertexData->addObject(CSVertexTNT(CSVector3(x, y, rect.z), CSVector2(u, v)));
        }

        cosq = CSMath::cos(angle2);
        sinq = CSMath::sin(angle2);
        x = c.x + hs.x * cosq;
        y = c.y + hs.y * sinq;
        u = uvc.x + uvhs.x * cosq;
        v = uvc.y + uvhs.y * sinq;
        vertexData->addObject(CSVertexTNT(CSVector3(x, y, rect.z), CSVector2(u, v)));

        if (fill) {
            for (int i = 0; i < vertexData->count() - 2; i++) {
                indexData->addObject(0);
                indexData->addObject((ushort)(i + 1));
                indexData->addObject((ushort)(i + 2));
            }
        }
        else {
            for (int i = 0; i < vertexData->count() - 1; i++) {
                indexData->addObject((ushort)i);
                indexData->addObject((ushort)(i + 1));
            }
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getArc(int life, const CSVector3& pos, float radius, float angle1, float angle2, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getArc(life, CSZRect(pos, radius * 2, radius * 2), angle1, angle2, fill, uv, aabb);
}

CSVertexArray* CSVertexArrays::getCircle(int life, const CSZRect& rect, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getArc(life, rect, 0, FloatTwoPi, fill, uv, aabb);
}

CSVertexArray* CSVertexArrays::getCircle(int life, const CSVector3& pos, float radius, bool fill, const CSRect& uv, CSABoundingBox* aabb) {
    return getArc(life, CSZRect(pos, radius * 2, radius * 2), 0, FloatTwoPi, fill, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientArc(int life, const CSZRect& rect, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb) {
    if (aabb) *aabb = CSABoundingBox(rect.leftTop(), rect.rightBottom());

    if (angle1 >= angle2) return NULL;

    ShapeKey* key = ShapeKey::gradientArc(rect, angle1, angle2, centerColor, surroundColor, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertex::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(CSMath::max(rect.width, rect.height) * 0.5f);

        CSGBufferData<CSVertex>* vertexData = new CSGBufferData<CSVertex>((int)((angle2 - angle1) / d) + 3);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), (vertexData->capacity() - 2) * 3);

        CSVector3 c = rect.centerMiddle();
        CSVector2 hs = rect.halfSize();
        CSVector2 uvc = uv.centerMiddle();
        CSVector2 uvhs = uv.halfSize();

        vertexData->addObject(CSVertex(c, centerColor, uvc));

        float x, y, u, v, cosq, sinq;

        for (float a = angle1; a < angle2; a += d) {
            cosq = CSMath::cos(a);
            sinq = CSMath::sin(a);
            x = c.x + hs.x * cosq;
            y = c.y + hs.y * sinq;
            u = uvc.x + uvhs.x * cosq;
            v = uvc.y + uvhs.y * sinq;
            vertexData->addObject(CSVertex(CSVector3(x, y, rect.z), surroundColor, CSVector2(u, v)));
        }
        
        cosq = CSMath::cos(angle2);
        sinq = CSMath::sin(angle2);
        x = c.x + hs.x * cosq;
        y = c.y + hs.y * sinq;
        u = uvc.x + uvhs.x * cosq;
        v = uvc.y + uvhs.y * sinq;
        vertexData->addObject(CSVertex(CSVector3(x, y, rect.z), surroundColor, CSVector2(u, v)));

        for (int i = 0; i < vertexData->count() - 2; i++) {
            indexData->addObject(0);
            indexData->addObject(i + 1);
            indexData->addObject(i + 2);
        }

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getGradientArc(int life, const CSVector3& pos, float radius, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientArc(life, CSZRect(pos, radius * 2, radius * 2), angle1, angle2, centerColor, surroundColor, uv, aabb);
}

CSVertexArray* CSVertexArrays::getGradientCircle(int life, const CSZRect& rect, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb) {
    return getGradientArc(life, rect, 0, FloatTwoPi, centerColor, surroundColor, uv, aabb);
}

CSVertexArray* CSVertexArrays::getSphere(int life, const CSVector3& pos, float radius, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::sphere(pos, radius, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) *aabb = CSABoundingBox(pos - CSVector3(radius), pos + CSVector3(radius));

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(radius);

        int div = (int)(FloatPi / d);
        int div2 = div * 2;

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>((div + 1) * (div2 + 1));
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), div * div2 * 6);

        float v = uv.top();
        float vd = uv.height / div;
        float ud = uv.width / div2;

        for (int i = 0; i <= div; i++) {
            float ir = FloatPi * i / div;
            float z = CSMath::cos(ir);
            float hd = CSMath::sin(ir);
            float u = uv.left();
            for (int j = 0; j <= div2; j++) {
                float jr = FloatTwoPi * (j % div2) / div2;
                float cosq = CSMath::cos(jr);
                float sinq = CSMath::sin(jr);
                float x = hd * sinq;
                float y = hd * cosq;
                CSVector3 normal(x, y, z);
                CSVector3 tangent(cosq, -sinq, 0);

                vertexData->addObject(CSVertexTNT(pos + normal * radius, CSVector2(u, v), normal, tangent));
                u += ud;
            }
            v += vd;
        }

        int vi = 0;

        for (int i = 0; i < div; i++) {
            for (int j = 0; j < div2; j++) {
                indexData->addObject(vi);
                indexData->addObject(vi + 1);
                indexData->addObject(vi + div2 + 1);
                indexData->addObject(vi + 1);
                indexData->addObject(vi + div2 + 2);
                indexData->addObject(vi + div2 + 1);

                vi++;
            }
            vi++;
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getCapsule(int life, const CSVector3& pos, float height, float radius, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::capsule(pos, height, radius, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) {
        CSVector3 abbr(radius, radius, height + radius);
        *aabb = CSABoundingBox(pos - abbr, pos + abbr);
    }

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(radius);

        int div = (int)(FloatPiOverTwo / d);
        int div2 = div * 2;
        int div4 = div * 4;

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>(((div + 1) * (div4 + 1)) * 2);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), (div2 + 1) * div4 * 6);

        float v = uv.top();
        float vd = uv.height / (div2 + 1);
        float ud = uv.width / div4;

        CSVector3 tp(pos.x, pos.y, pos.z + height);
        for (int i = 0; i <= div; i++) {
            float ir = FloatPiOverTwo * i / div;
            float z = CSMath::cos(ir);
            float hd = CSMath::sin(ir);
            float u = uv.left();
            for (int j = 0; j <= div4; j++) {
                float jr = FloatTwoPi * (j % div4) / div4;
                float cosq = CSMath::cos(jr);
                float sinq = CSMath::sin(jr);
                float x = hd * sinq;
                float y = hd * cosq;
                CSVector3 normal(x, y, z);
                CSVector3 tangent(cosq, -sinq, 0);

                vertexData->addObject(CSVertexTNT(tp + normal * radius, CSVector2(u, v), normal, tangent));
                u += ud;
            }
            v += vd;
        }

        CSVector3 bp(pos.x, pos.y, pos.z - height);
        for (int i = 0; i <= div; i++) {
            float ir = FloatPiOverTwo * (i + div) / div;
            float z = CSMath::cos(ir);
            float hd = CSMath::sin(ir);
            float u = uv.left();
            for (int j = 0; j <= div4; j++) {
                float jr = FloatTwoPi * (j % div4) / div4;
                float cosq = CSMath::cos(jr);
                float sinq = CSMath::sin(jr);
                float x = hd * sinq;
                float y = hd * cosq;
                CSVector3 normal(x, y, z);
                CSVector3 tangent(cosq, -sinq, 0);

                vertexData->addObject(CSVertexTNT(bp + normal * radius, CSVector2(u, v), normal, tangent));
                u += ud;
            }
            v += vd;
        }


        int vi = 0;

        for (int i = 0; i <= div2; i++) {
            for (int j = 0; j < div4; j++) {
                indexData->addObject(vi);
                indexData->addObject(vi + 1);
                indexData->addObject(vi + div4 + 1);
                indexData->addObject(vi + 1);
                indexData->addObject(vi + div4 + 2);
                indexData->addObject(vi + div4 + 1);

                vi++;
            }
            vi++;
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getCylinder(int life, const CSVector3& pos, float topRadius, float bottomRadius, float height, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::cylinder(pos, topRadius, bottomRadius, height, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) {
        float maxr = CSMath::max(topRadius, bottomRadius);
        CSVector3 aabbr(maxr, maxr, height);
        *aabb = CSABoundingBox(pos - aabbr, pos + aabbr);
    }

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        float d = CSGraphics::radianDistance(CSMath::max(topRadius, bottomRadius));

        int div2 = (int)(FloatTwoPi / d);

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>((div2 + 1) * 6);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), div2 * 18);

        float tv = uv.top();
        float wtv = uv.top() + uv.height * topRadius / (topRadius + bottomRadius + height);
        float wbv = uv.top() + uv.height * (topRadius + height) / (topRadius + bottomRadius + height);
        float bv = uv.bottom();
        float u = uv.left();
        float ud = uv.width / (div2 + 1);

        float top = pos.z + height;
        float bottom = pos.z - height;

        for (int i = 0; i <= div2; i++) {
            float ir = FloatTwoPi * (i % div2) / div2;
            float x = CSMath::sin(ir);
            float y = CSMath::cos(ir);

            CSVector3 normal(x, y, 0);
            CSVector3 tangent(y, -x, 0);

            vertexData->addObject(CSVertexTNT(CSVector3(pos.x, pos.y, top), CSVector2(u, tv), CSVector3::UnitZ, CSVector3::UnitX));
            vertexData->addObject(CSVertexTNT(CSVector3(x * topRadius, y * topRadius, top), CSVector2(u, wtv), CSVector3::UnitZ, CSVector3::UnitX));
            vertexData->addObject(CSVertexTNT(CSVector3(x * topRadius, y * topRadius, top), CSVector2(u, wtv), normal, tangent));
            vertexData->addObject(CSVertexTNT(CSVector3(x * bottomRadius, y * bottomRadius, bottom), CSVector2(u, wbv), normal, tangent));
            vertexData->addObject(CSVertexTNT(CSVector3(x * bottomRadius, y * bottomRadius, bottom), CSVector2(u, wbv), -CSVector3::UnitZ, -CSVector3::UnitX));
            vertexData->addObject(CSVertexTNT(CSVector3(pos.x, pos.y, bottom), CSVector2(u, bv), -CSVector3::UnitZ, -CSVector3::UnitX));

            u += ud;
        }

        int vi = 0;

        for (int i = 0; i < div2; i++) {
            indexData->addObject(vi);
            indexData->addObject(vi + 6);
            indexData->addObject(vi + 1);
            indexData->addObject(vi + 1);
            indexData->addObject(vi + 6);
            indexData->addObject(vi + 7);

            indexData->addObject(vi + 2);
            indexData->addObject(vi + 2 + 6);
            indexData->addObject(vi + 2 + 1);
            indexData->addObject(vi + 2 + 1);
            indexData->addObject(vi + 2 + 6);
            indexData->addObject(vi + 2 + 7);

            indexData->addObject(vi + 4);
            indexData->addObject(vi + 4 + 6);
            indexData->addObject(vi + 4 + 1);
            indexData->addObject(vi + 4 + 1);
            indexData->addObject(vi + 4 + 6);
            indexData->addObject(vi + 4 + 7);

            vi += 6;
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);

        vertexData->release();
        indexData->release();
    }

    return vertices;
}

CSVertexArray* CSVertexArrays::getPyramid(int life, const CSVector3& pos, float size, float height, bool reverse, const CSRect& uv, CSABoundingBox* aabb) {
    return getHexahedron(life, pos, reverse ? CSVector2(size) : CSVector2::Zero, reverse ? CSVector2::Zero : CSVector2(size), height, uv, aabb);
}

CSVertexArray* CSVertexArrays::getBox(int life, const CSVector3& min, const CSVector3& max, const CSRect& uv, CSABoundingBox* aabb) {
    CSZRect topRect(min.x, min.y, max.z, max.x - min.x, max.y - min.y);
    CSZRect bottomRect(min.x, min.y, min.z, max.x - min.x, max.y - min.y);

    return getHexahedron(life, topRect, bottomRect, uv, aabb);
}

CSVertexArray* CSVertexArrays::getCube(int life, const CSVector3& pos, float radius, const CSRect& uv, CSABoundingBox* aabb) {
    CSZRect topRect(pos.x - radius, pos.y - radius, pos.z + radius, radius * 2, radius * 2);
    CSZRect bottomRect(pos.x - radius, pos.y - radius, pos.z - radius, radius * 2, radius * 2);

    return getHexahedron(life, topRect, bottomRect, uv, aabb);
}

CSVertexArray* CSVertexArrays::getHexahedron(int life, const CSVector3& pos, const CSVector2& topSize, const CSVector2& bottomSize, float height, const CSRect& uv, CSABoundingBox* aabb) {
    CSZRect topRect(pos.x - topSize.x, pos.y - topSize.y, pos.z + height, topSize.x * 2, topSize.x * 2);
    CSZRect bottomRect(pos.x - bottomSize.x, pos.y - bottomSize.y, pos.z - height, bottomSize.y * 2, bottomSize.y * 2);

    return getHexahedron(life, topRect, bottomRect, uv, aabb);
}

CSVertexArray* CSVertexArrays::getHexahedron(int life, const CSZRect& topRect, const CSZRect& bottomRect, const CSRect& uv, CSABoundingBox* aabb) {
    ShapeKey* key = ShapeKey::hexahedron(topRect, bottomRect, uv);

    CSVertexArray* vertices = static_assert_cast<CSVertexArray*>(CSResourcePool::sharedPool()->get(key));

    if (aabb) *aabb = CSABoundingBox(CSVector3::min(topRect.leftTop(), bottomRect.leftTop()), CSVector3::max(topRect.rightBottom(), bottomRect.rightBottom()));

    if (!vertices) {
        vertices = get(key, life, false, 1, true, CSVertexTNT::SingleBufferVertexLayouts);

        CSVector3 positions[8] = {
            topRect.leftTop(),
            topRect.rightTop(),
            topRect.leftBottom(),
            topRect.rightBottom(),
            bottomRect.leftTop(),
            bottomRect.rightTop(),
            bottomRect.leftBottom(),
            bottomRect.rightBottom()
        };
        static const int quads[6][4] = {
            { 0, 1, 2, 3 },
            { 2, 3, 6, 7 },
            { 0, 2, 4, 6 },
            { 1, 0, 5, 4 },
            { 3, 1, 7, 5 },
            { 5, 4, 7, 6 }
        };

        CSVector2 uv0 = uv.leftTop();
        CSVector2 uv1 = uv.rightTop();
        CSVector2 uv2 = uv.leftBottom();
        CSVector2 uv3 = uv.rightBottom();

        CSGBufferData<CSVertexTNT>* vertexData = new CSGBufferData<CSVertexTNT>(24);
        CSVertexIndexData* indexData = new CSVertexIndexData(vertexData->capacity(), 36);

        int vi = 0;
        for (int i = 0; i < 6; i++) {
            const CSVector3& pos0 = positions[quads[i][0]];
            const CSVector3& pos1 = positions[quads[i][1]];
            const CSVector3& pos2 = positions[quads[i][2]];
            const CSVector3& pos3 = positions[quads[i][3]];

            CSVector3 normal = CSVector3::normalize(CSVector3::cross(pos1 - pos0, pos2 - pos0));
            CSVector3 tangent = CSVector3::normalize(pos1 - pos0);

            vertexData->addObject(CSVertexTNT(pos0, uv0, normal, tangent));
            vertexData->addObject(CSVertexTNT(pos1, uv1, normal, tangent));
            vertexData->addObject(CSVertexTNT(pos2, uv2, normal, tangent));
            vertexData->addObject(CSVertexTNT(pos3, uv3, normal, tangent));

            indexData->addObject(vi);
            indexData->addObject(vi + 1);
            indexData->addObject(vi + 2);
            indexData->addObject(vi + 1);
            indexData->addObject(vi + 3);
            indexData->addObject(vi + 2);

            vi += 4;
        }

        vertices->vertexBuffer(0)->upload(vertexData, CSGBufferUsageHintStaticDraw);
        vertices->indexBuffer()->upload(indexData, CSGBufferUsageHintStaticDraw);
    
        vertexData->release();
        indexData->release();
    }

    return vertices;
}