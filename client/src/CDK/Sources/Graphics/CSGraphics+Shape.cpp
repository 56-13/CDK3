#define CDK_IMPL

#include "CSGraphics.h"

#include "CSStreamRenderCommand.h"

void CSGraphics::drawPoint(const CSVector3& point) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitivePoints, 1, 1);
    
    command->addVertex(CSFVertex(point));
    
    command->addIndex(0);
    
    this->command(command);

    command->release();
}

void CSGraphics::drawLine(const CSVector3& point1, const CSVector3& point2) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveLines, 2, 2);
    
    command->addVertex(CSFVertex(point1));
    command->addVertex(CSFVertex(point2));
    
    command->addIndex(0);
    command->addIndex(1);
    
    this->command(command);

    command->release();
}

void CSGraphics::drawGradientLine(const CSVector3& point1, const CSColor& color1, const CSVector3& point2, const CSColor& color2) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveLines, 2, 2);
    
    command->addVertex(CSFVertex(point1, color1));
    command->addVertex(CSFVertex(point2, color2));
    
    command->addIndex(0);
    command->addIndex(1);
    
    this->command(command);

    command->release();
}

void CSGraphics::drawRect(const CSZRect& rect, bool fill, const CSRect& uv) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, fill ? CSPrimitiveTriangles : CSPrimitiveLines, 4, fill ? 6 : 8);
    
    command->addVertex(CSFVertex(rect.leftTop(), uv.leftTop()));
    command->addVertex(CSFVertex(rect.rightTop(), uv.rightTop()));
    command->addVertex(CSFVertex(rect.leftBottom(), uv.leftBottom()));
    command->addVertex(CSFVertex(rect.rightBottom(), uv.rightBottom()));
    
    if (fill) {
        command->addIndex(0);
        command->addIndex(1);
        command->addIndex(2);
        command->addIndex(1);
        command->addIndex(3);
        command->addIndex(2);
    }
    else {
        command->addIndex(0);
        command->addIndex(1);
        command->addIndex(1);
        command->addIndex(3);
        command->addIndex(3);
        command->addIndex(2);
        command->addIndex(2);
        command->addIndex(0);
    }
    this->command(command);

    command->release();
}

void CSGraphics::drawGradientRect(const CSZRect& rect, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, const CSRect& uv) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, fill ? CSPrimitiveTriangles : CSPrimitiveLines, 4, fill ? 6 : 8);
    
    command->addVertex(CSFVertex(rect.leftTop(), leftTopColor, uv.leftTop()));
    command->addVertex(CSFVertex(rect.rightTop(), rightTopColor, uv.rightTop()));
    command->addVertex(CSFVertex(rect.leftBottom(), leftBottomColor, uv.leftBottom()));
    command->addVertex(CSFVertex(rect.rightBottom(), rightBottomColor, uv.rightBottom()));
        
    if (fill) {
        command->addIndex(0);
        command->addIndex(1);
        command->addIndex(2);
        command->addIndex(1);
        command->addIndex(3);
        command->addIndex(2);
    }
    else {
        command->addIndex(0);
        command->addIndex(1);
        command->addIndex(1);
        command->addIndex(3);
        command->addIndex(3);
        command->addIndex(2);
        command->addIndex(2);
        command->addIndex(0);
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawRoundRect(const CSZRect& rect, float radius, bool fill, CSCorner corner, const CSRect& uv) {
    float minRadius = CSMath::min(rect.width, rect.height) * 0.5f;

    if (radius > minRadius) radius = minRadius;

    if (radius <= 0) {
        drawRect(rect, fill, uv);
        return;
    }

    float d = radianDistance(radius);
    
    int vertexCapacity = (int)CSMath::ceil(FloatTwoPi / d);
    int indexCapacity = fill ? (vertexCapacity - 2) * 3 : vertexCapacity * 2;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, fill ? CSPrimitiveTriangles : CSPrimitiveLines, vertexCapacity, indexCapacity);

    float s, e;
    
    if (corner & CSCornerLeftTop) {
        s = FloatPi;
        e = FloatPi * 1.5f;
        for (float a = s; a < e; a += d) {
            float sx = rect.left() + radius + CSMath::cos(a) * radius;
            float sy = rect.top() + radius + CSMath::sin(a) * radius;
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z)));
        }
        command->addVertex(CSFVertex(CSVector3(rect.left() + radius, rect.top(), rect.z)));
    }
    else {
        command->addVertex(CSFVertex(CSVector3(rect.left(), rect.top(), rect.z)));
    }
    
    if (corner & CSCornerRightTop) {
        s = FloatPi * 1.5f;
        e = FloatTwoPi;
        for (float a = s; a < e; a += d) {
            float sx = rect.right() - radius + CSMath::cos(a) * radius;
            float sy = rect.top() + radius + CSMath::sin(a) * radius;
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z)));
        }
        command->addVertex(CSFVertex(CSVector3(rect.right(), rect.top() + radius, rect.z)));
    }
    else {
        command->addVertex(CSFVertex(CSVector3(rect.right(), rect.top(), rect.z)));
    }
    
    if (corner & CSCornerRightBottom) {
        s = 0.0f;
        e = FloatPiOverTwo;
        for (float a = s; a < e; a += d) {
            float sx = rect.right() - radius + CSMath::cos(a) * radius;
            float sy = rect.bottom() - radius + CSMath::sin(a) * radius;
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z)));
        }
        command->addVertex(CSFVertex(CSVector3(rect.right() - radius, rect.bottom(), rect.z)));
    }
    else {
        command->addVertex(CSFVertex(CSVector3(rect.right(), rect.bottom(), rect.z)));
    }
    
    if (corner & CSCornerLeftBottom) {
        s = FloatPiOverTwo;
        e = FloatPi;
        for (float a = s; a < e; a += d) {
            float sx = rect.left() + radius + CSMath::cos(a) * radius;
            float sy = rect.bottom() - radius + CSMath::sin(a) * radius;
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z)));
        }
        command->addVertex(CSFVertex(CSVector3(rect.left(), rect.bottom() - radius, rect.z)));
    }
    else {
        command->addVertex(CSFVertex(CSVector3(rect.left(), rect.bottom(), rect.z)));
    }
    
    if (fill) {
        for (int i = 0; i < command->vertexCount() - 2; i++) {
            command->addIndex(0);
            command->addIndex(i + 1);
            command->addIndex(i + 2);
        }
    }
    else {
        for (int i = 0; i < command->vertexCount() - 1; i++) {
            command->addIndex(i);
            command->addIndex(i + 1);
        }
        command->addIndex(command->vertexCount() - 1);
        command->addIndex(0);
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawGradientRoundRect(const CSZRect& rect, float radius, const CSColor& leftTopColor, const CSColor& rightTopColor, const CSColor& leftBottomColor, const CSColor& rightBottomColor, bool fill, CSCorner corner, const CSRect& uv) {
    float minRadius = CSMath::min(rect.width, rect.height) * 0.5f;

    if (radius > minRadius) radius = minRadius;

    if (radius <= 0) {
        drawGradientRect(rect, leftTopColor, rightTopColor, leftBottomColor, rightBottomColor, fill, uv);
        return;
    }

    float d = radianDistance(radius);

    int vertexCapacity = (int)CSMath::ceil(FloatTwoPi / d) + 4;
    int indexCapacity = fill ? (vertexCapacity - 1) * 3 : vertexCapacity * 2;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, fill ? CSPrimitiveTriangles : CSPrimitiveLines, vertexCapacity, indexCapacity);
    
    if (fill) {
        command->addVertex(CSFVertex(CSVector3(rect.center(), rect.middle(), rect.z), (leftTopColor + rightTopColor + leftBottomColor + rightBottomColor) * 0.25f));
    }

    float s, e;

    if (corner & CSCornerLeftTop) {
        s = FloatPi;
        e = FloatPi * 1.5f;
        for (float a = s; a < e; a += d) {
            float sx = rect.left() + radius + CSMath::cos(a) * radius;
            float sy = rect.top() + radius + CSMath::sin(a) * radius;
            float xr = (sx - rect.left()) / rect.width;
            float yr = (sy - rect.top()) / rect.height;
            CSColor color = CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr);
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z), color, uv.leftTop()));
        }
        {
            float r = radius / rect.width;
            CSColor color = CSColor::lerp(leftTopColor, rightTopColor, r);
            command->addVertex(CSFVertex(CSVector3(rect.left() + radius, rect.top(), rect.z), color, uv.leftTop()));
        }
    }
    else {
        command->addVertex(CSFVertex(rect.leftTop(), leftTopColor, uv.leftTop()));
    }

    if (corner & CSCornerRightTop) {
        s = FloatPi * 1.5f;
        e = FloatTwoPi;
        for (float a = s; a < e; a += d) {
            float sx = rect.right() - radius + CSMath::cos(a) * radius;
            float sy = rect.top() + radius + CSMath::sin(a) * radius;
            float xr = (sx - rect.left()) / rect.width;
            float yr = (sy - rect.top()) / rect.height;
            CSColor color = CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr);
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z), color, uv.rightTop()));
        }
        {
            float r = radius / rect.height;
            CSColor color = CSColor::lerp(rightTopColor, rightBottomColor, r);
            command->addVertex(CSFVertex(CSVector3(rect.right(), rect.top() + radius, rect.z), color, uv.rightTop()));
        }
    }
    else {
        command->addVertex(CSFVertex(rect.rightTop(), rightTopColor, uv.rightTop()));
    }

    if (corner & CSCornerRightBottom) {
        s = 0;
        e = FloatPiOverTwo;
        for (float a = s; a < e; a += d) {
            float sx = rect.right() - radius + CSMath::cos(a) * radius;
            float sy = rect.bottom() - radius + CSMath::sin(a) * radius;
            float xr = (sx - rect.left()) / rect.width;
            float yr = (sy - rect.top()) / rect.height;
            CSColor color = CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr);
            command->addVertex(CSFVertex(CSVector3(sx, sy, rect.z), color, uv.rightBottom()));
        }
        {
            float r = (rect.width - radius) / rect.width;
            CSColor color = CSColor::lerp(leftBottomColor, rightBottomColor, r);
            command->addVertex(CSFVertex(CSVector3(rect.right() - radius, rect.bottom(), rect.z), color, uv.rightBottom()));
        }
    }
    else {
        command->addVertex(CSFVertex(rect.rightBottom(), rightBottomColor, uv.rightBottom()));
    }

    if (corner & CSCornerLeftBottom) {
        s = FloatPiOverTwo;
        e = FloatPi;
        for (float a = s; a < e; a += d) {
            float sx = rect.left() + radius + CSMath::cos(a) * radius;
            float sy = rect.bottom() - radius + CSMath::sin(a) * radius;
            float xr = (sx - rect.left()) / rect.width;
            float yr = (sy - rect.top()) / rect.height;
            CSColor color = CSColor::lerp(CSColor::lerp(leftTopColor, rightTopColor, xr), CSColor::lerp(leftBottomColor, rightBottomColor, xr), yr);
            command->addVertex(CSFVertex(CSVector3(sx, sy, 0), color, uv.leftBottom()));
        }
        {
            float r = (rect.height - radius) / rect.height;
            CSColor color = CSColor::lerp(leftTopColor, leftBottomColor, r);
            command->addVertex(CSFVertex(CSVector3(rect.left(), rect.bottom() - radius, rect.z), color, uv.leftBottom()));
        }
    }
    else {
        command->addVertex(CSFVertex(rect.leftBottom(), leftBottomColor, uv.leftBottom()));
    }

    if (fill) {
        for (int i = 0; i < command->vertexCount() - 2; i++) {
            command->addIndex(0);
            command->addIndex(i + 1);
            command->addIndex(i + 2);
        }
        command->addIndex(0);
        command->addIndex(command->vertexCount() - 1);
        command->addIndex(1);
    }
    else {
        for (int i = 0; i < command->vertexCount() - 1; i++) {
            command->addIndex(i);
            command->addIndex(i + 1);
        }
        command->addIndex(command->vertexCount() - 1);
        command->addIndex(0);
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawArc(const CSZRect& rect, float angle1, float angle2, bool fill, const CSRect& uv) {
    if (angle1 >= angle2) return;

    float d = radianDistance(CSMath::max(rect.width, rect.height) * 0.5f);

    int vertexCapacity = CSMath::ceil((angle2 - angle1) / d) + 2;
    int indexCapacity = fill ? (vertexCapacity - 1) * 3 : (vertexCapacity - 1) * 2;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, fill ? CSPrimitiveTriangles : CSPrimitiveLines, vertexCapacity, indexCapacity);
        
    CSVector3 c = rect.centerMiddle();
    CSVector2 hs = rect.halfSize();
    CSVector2 uvc = uv.centerMiddle();
    CSVector2 uvhs = uv.halfSize();

    if (fill) {
        command->addVertex(CSFVertex(c, uvc));
    }
    float x, y, u, v, cosq, sinq;

    for (float a = angle1; a < angle2; a += d) {
        cosq = CSMath::cos(a);
        sinq = CSMath::sin(a);
        x = c.x + hs.x * cosq;
        y = c.y + hs.y * sinq;
        u = uvc.x + uvhs.x * cosq;
        v = uvc.y + uvhs.y * sinq;
        command->addVertex(CSFVertex(CSVector3(x, y, c.z), CSVector2(u, v)));
    }

    cosq = CSMath::cos(angle2);
    sinq = CSMath::sin(angle2);
    x = c.x + hs.x * cosq;
    y = c.y + hs.y * sinq;
    u = uvc.x + uvhs.x * cosq;
    v = uvc.y + uvhs.y * sinq;
    command->addVertex(CSFVertex(CSVector3(x, y, c.z), CSVector2(u, v)));

    if (fill) {
        for (int i = 0; i < command->vertexCount() - 2; i++) {
            command->addIndex(0);
            command->addIndex(i + 1);
            command->addIndex(i + 2);
        }
    }
    else {
        for (int i = 0; i < command->vertexCount() - 1; i++) {
            command->addIndex(i);
            command->addIndex(i + 1);
        }
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawGradientArc(const CSZRect& rect, float angle1, float angle2, const CSColor& centerColor, const CSColor& surroundColor, const CSRect& uv) {
    if (angle1 >= angle2) return;

    float d = radianDistance(CSMath::max(rect.width, rect.height) * 0.5f);

    int vertexCapacity = CSMath::ceil((angle2 - angle1) / d) + 2;
    int indexCapacity = (vertexCapacity - 1) * 3;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, vertexCapacity, indexCapacity);

    CSVector3 c = rect.centerMiddle();
    CSVector2 hs = rect.halfSize();
    CSVector2 uvc = uv.centerMiddle();
    CSVector2 uvhs = uv.halfSize();

    command->addVertex(CSFVertex(c, centerColor, uvc));

    float x, y, u, v, cosq, sinq;

    for (float a = angle1; a < angle2; a += d) {
        cosq = CSMath::cos(a);
        sinq = CSMath::sin(a);
        x = c.x + hs.x * cosq;
        y = c.y + hs.y * sinq;
        u = uvc.x + uvhs.x * cosq;
        v = uvc.y + uvhs.y * sinq;
        command->addVertex(CSFVertex(CSVector3(x, y, c.z), CSVector2(u, v)));
    }

    cosq = CSMath::cos(angle2);
    sinq = CSMath::sin(angle2);
    x = c.x + hs.x * cosq;
    y = c.y + hs.y * sinq;
    u = uvc.x + uvhs.x * cosq;
    v = uvc.y + uvhs.y * sinq;
    command->addVertex(CSFVertex(CSVector3(x, y, c.z), CSVector2(u, v)));

    for (int i = 0; i < command->vertexCount() - 2; i++) {
        command->addIndex(0);
        command->addIndex(i + 1);
        command->addIndex(i + 2);
    }

    this->command(command);
    
    command->release();
}

void CSGraphics::drawSphere(const CSVector3& pos, float radius, const CSRect& uv) {
    float d = radianDistance(radius);

    int div = (int)(FloatPi / d);
    int div2 = div * 2;

    int vertexCapacity = (div + 1) * (div2 + 1);
    int indexCapacity = div * div2 * 6;
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, vertexCapacity, indexCapacity);

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
            command->addVertex(CSFVertex(pos + normal * radius, CSVector2(u, v), normal, tangent));
            u += ud;
        }
        v += vd;
    }
    
    int vi = 0;

    for (int i = 0; i < div; i++) {
        for (int j = 0; j < div2; j++) {
            command->addIndex(vi);
            command->addIndex(vi + 1);
            command->addIndex(vi + div2 + 1);
            command->addIndex(vi + 1);
            command->addIndex(vi + div2 + 2);
            command->addIndex(vi + div2 + 1);

            vi++;
        }
        vi++;
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawCapsule(const CSVector3& pos, float height, float radius, const CSRect& uv) {
    float d = radianDistance(radius);

    int div = (int)(FloatPiOverTwo / d);
    int div2 = div * 2;
    int div4 = div * 4;

    int vertexCapacity = (div + 1) * (div4 + 1);
    int indexCapacity = (div2 + 1) * div4 * 6;
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, vertexCapacity, indexCapacity);

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

            command->addVertex(CSFVertex(tp + normal * radius, CSVector2(u, v), normal, tangent));
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

            command->addVertex(CSFVertex(bp + normal * radius, CSVector2(u, v), normal, tangent));
            u += ud;
        }
        v += vd;
    }


    int vi = 0;

    for (int i = 0; i <= div2; i++) {
        for (int j = 0; j < div4; j++) {
            command->addIndex(vi);
            command->addIndex(vi + 1);
            command->addIndex(vi + div4 + 1);
            command->addIndex(vi + 1);
            command->addIndex(vi + div4 + 2);
            command->addIndex(vi + div4 + 1);

            vi++;
        }
        vi++;
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawCylinder(const CSVector3& pos, float topRadius, float bottomRadius, float height, const CSRect& uv) {
    float d = radianDistance(CSMath::max(topRadius, bottomRadius));

    int div2 = (int)(FloatTwoPi / d);

    int vertexCapacity = (div2 + 1) * 6;
    int indexCapacity = div2 * 18;
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, vertexCapacity, indexCapacity);

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

        command->addVertex(CSFVertex(CSVector3(pos.x, pos.y, top), CSVector2(u, tv), CSVector3::UnitZ, CSVector3::UnitX));
        command->addVertex(CSFVertex(CSVector3(x * topRadius, y * topRadius, top), CSVector2(u, wtv), CSVector3::UnitZ, CSVector3::UnitX));
        command->addVertex(CSFVertex(CSVector3(x * topRadius, y * topRadius, top), CSVector2(u, wtv), normal, tangent));
        command->addVertex(CSFVertex(CSVector3(x * bottomRadius, y * bottomRadius, bottom), CSVector2(u, wbv), normal, tangent));
        command->addVertex(CSFVertex(CSVector3(x * bottomRadius, y * bottomRadius, bottom), CSVector2(u, wbv), -CSVector3::UnitZ, -CSVector3::UnitX));
        command->addVertex(CSFVertex(CSVector3(pos.x, pos.y, bottom), CSVector2(u, bv), -CSVector3::UnitZ, -CSVector3::UnitX));

        u += ud;
    }

    int vi = 0;

    for (int i = 0; i < div2; i++) {
        command->addIndex(vi);
        command->addIndex(vi + 6);
        command->addIndex(vi + 1);
        command->addIndex(vi + 1);
        command->addIndex(vi + 6);
        command->addIndex(vi + 7);

        command->addIndex(vi + 2);
        command->addIndex(vi + 2 + 6);
        command->addIndex(vi + 2 + 1);
        command->addIndex(vi + 2 + 1);
        command->addIndex(vi + 2 + 6);
        command->addIndex(vi + 2 + 7);

        command->addIndex(vi + 4);
        command->addIndex(vi + 4 + 6);
        command->addIndex(vi + 4 + 1);
        command->addIndex(vi + 4 + 1);
        command->addIndex(vi + 4 + 6);
        command->addIndex(vi + 4 + 7);

        vi += 6;
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawPyramid(const CSVector3& pos, float size, float height, bool reverse, const CSRect& uv) {
    drawHexahedron(pos, reverse ? CSVector2(size, size) : CSVector2::Zero, reverse ? CSVector2::Zero : CSVector2(size, size), height, uv);
}

void CSGraphics::drawBox(const CSVector3& min, const CSVector3& max, const CSRect& uv) {
    CSZRect topRect(min.x, min.y, max.z, max.x - min.x, max.y - min.y);
    CSZRect bottomRect(min.x, min.y, min.z, max.x - min.x, max.y - min.y);

    drawHexahedron(topRect, bottomRect, uv);
}

void CSGraphics::drawCube(const CSVector3& pos, float size, const CSRect& uv) {
    CSZRect topRect(pos.x - size, pos.y - size, pos.z + size, size * 2, size * 2);
    CSZRect bottomRect(pos.x - size, pos.y - size, pos.z - size, size * 2, size * 2);

    drawHexahedron(topRect, bottomRect, uv);
}

void CSGraphics::drawHexahedron(const CSVector3& pos, const CSVector2& topSize, const CSVector2& bottomSize, float height, const CSRect& uv) {
    CSZRect topRect(pos.x - topSize.x, pos.y - topSize.y, pos.z + height, topSize.x * 2, topSize.y * 2);
    CSZRect bottomRect(pos.x - bottomSize.x, pos.y - bottomSize.y, pos.z - height, bottomSize.x * 2, bottomSize.y * 2);

    drawHexahedron(topRect, bottomRect, uv);
}

void CSGraphics::drawHexahedron(const CSZRect& topRect, const CSZRect& bottomRect, const CSRect& uv) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, 24, 36);

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

    int vi = 0;
    for (int i = 0; i < 6; i++) {
        const CSVector3& pos0 = positions[quads[i][0]];
        const CSVector3& pos1 = positions[quads[i][1]];
        const CSVector3& pos2 = positions[quads[i][2]];
        const CSVector3& pos3 = positions[quads[i][3]];

        CSVector3 normal = CSVector3::normalize(CSVector3::cross(pos1 - pos0, pos2 - pos0));
        CSVector3 tangent = CSVector3::normalize(pos1 - pos0);

        command->addVertex(CSFVertex(pos0, uv0, normal, tangent));
        command->addVertex(CSFVertex(pos1, uv1, normal, tangent));
        command->addVertex(CSFVertex(pos2, uv2, normal, tangent));
        command->addVertex(CSFVertex(pos3, uv3, normal, tangent));

        command->addIndex(vi);
        command->addIndex(vi + 1);
        command->addIndex(vi + 2);
        command->addIndex(vi + 1);
        command->addIndex(vi + 3);
        command->addIndex(vi + 2);

        vi += 4;
    }

    this->command(command);

    command->release();
}