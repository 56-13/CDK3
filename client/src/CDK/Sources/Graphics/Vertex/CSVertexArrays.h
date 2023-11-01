#ifndef __CDK__CSVertexArrays__
#define __CDK__CSVertexArrays__

#include "CSVertexArray.h"

#include "CSColor.h"
#include "CSZRect.h"
#include "CSABoundingBox.h"

class CSVertexArrays {
public:
    static CSVertexArray* get(const CSObject* key, int life, bool recycle, int vertexBufferCount, bool indexBuffer, const CSVertexLayout* layouts, int layoutCount);
    static inline CSVertexArray* get(const CSObject* key, int life, bool recycle, int vertexBufferCount, bool indexBuffer, const CSArray<CSVertexLayout>* layouts) {
        return get(key, life, recycle, vertexBufferCount, indexBuffer, layouts->pointer(), layouts->count());
    }

    static CSVertexArray* get2D(const CSVector2* positions, int count);
    static CSVertexArray* get2D(int count, CSVector2 position, ...);
    static CSVertexArray* get2D(const CSArray<CSVector2>* positions);
    static CSVertexArray* get2D(const CSRect& bounds);
    static CSVertexArray* getScreen2D();
    static CSVertexArray* get3D(const CSVector3* positions, int count);
    static CSVertexArray* get3D(int count, CSVector3 position, ...);
    static CSVertexArray* get3D(const CSArray<CSVector3>* positions);
    static CSVertexArray* getRect(int life, const CSZRect& rect, bool fill, const CSRect& uv, CSABoundingBox* aabb);
private:
    static CSVertexArray* getGradientRect(int life, const CSZRect& rect, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb);
public:
    static CSVertexArray* getGradientRectH(int life, const CSZRect& rect, const CSColor& leftColor, const CSColor& rightColor, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientRectV(int life, const CSZRect& rect, const CSColor& topColor, const CSColor& bottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getRoundRect(int life, const CSZRect& rect, float radius, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getRoundRect(int life, const CSZRect& rect, float radius, bool fill, const CSRect& uv, CSABoundingBox* aabb);
private:
    static CSVertexArray* getGradientRoundRect(int life, const CSZRect& rect, float radius, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb);
public:
    static CSVertexArray* getGradientRoundRectH(int life, const CSZRect& rect, float radius, const CSColor& leftColor, const CSColor& rightColor, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientRoundRectV(int life, const CSZRect& rect, float radius, const CSColor& topColor, const CSColor& bottomColor, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientRoundRectH(int life, const CSZRect& rect, float radius, const CSColor& leftColor, const CSColor& rightColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientRoundRectV(int life, const CSZRect& rect, float radius, const CSColor& topColor, const CSColor& bottomColor, bool fill, CSCorner corner, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getArc(int life, const CSZRect& rect, float angle1, float angle2, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getArc(int life, const CSVector3& pos, float radius, float angle1, float angle2, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getCircle(int life, const CSZRect& rect, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getCircle(int life, const CSVector3& pos, float radius, bool fill, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientArc(int life, const CSZRect& rect, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientArc(int life, const CSVector3& pos, float radius, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getGradientCircle(int life, const CSZRect& rect, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getSphere(int life, const CSVector3& pos, float radius, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getCapsule(int life, const CSVector3& pos, float height, float radius, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getCylinder(int life, const CSVector3& pos, float topRadius, float bottomRadius, float height, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getPyramid(int life, const CSVector3& pos, float size, float height, bool reverse, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getBox(int life, const CSVector3& min, const CSVector3& max, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getCube(int life, const CSVector3& pos, float radius, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getHexahedron(int life, const CSVector3& pos, const CSVector2& topSize, const CSVector2& bottomSize, float height, const CSRect& uv, CSABoundingBox* aabb);
    static CSVertexArray* getHexahedron(int life, const CSZRect& topRect, const CSZRect& bottomRect, const CSRect& uv, CSABoundingBox* aabb);
};

#endif
