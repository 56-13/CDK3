#define CDK_IMPL

#include "CSGraphics.h"

#include "CSRenderers.h"

#include "CSStreamRenderCommand.h"

static void setImageCommand(CSStreamRenderCommand* command, const CSImage* image) {
    command->state.material.colorMap = image->texture();
    command->state.material.normalMap = NULL;
    command->state.material.materialMap = NULL;
    command->state.material.emissiveMap = NULL;
}

void CSGraphics::drawImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame) {
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles);
    setImageCommand(command, image);

    float lu = frame.left() / image->texture()->width();
    float ru = frame.right() / image->texture()->width();
    float tv = frame.top() / image->texture()->height();
    float bv = frame.bottom() / image->texture()->height();

    command->addVertex(CSFVertex(destRect.leftTop(), CSVector2(lu, tv)));
    command->addVertex(CSFVertex(destRect.rightTop(), CSVector2(ru, tv)));
    command->addVertex(CSFVertex(destRect.leftBottom(), CSVector2(lu, bv)));
    command->addVertex(CSFVertex(destRect.rightBottom(), CSVector2(ru, bv)));

    command->addIndex(0);
    command->addIndex(1);
    command->addIndex(2);
    command->addIndex(1);
    command->addIndex(3);
    command->addIndex(2);

    this->command(command);

    command->release();
}

void CSGraphics::drawImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect) {
    if (!image) return;

    CSRect subframe = srcRect;
    const CSRect& frame = image->frame();
    subframe.x += frame.x;
    subframe.y += frame.y;
    subframe.intersect(frame);
    
    drawImageImpl(image, destRect, subframe);
}

void CSGraphics::drawImage(const CSImage* image, const CSZRect& destRect, bool stretch) {
    if (!image) return;

    if (stretch) drawImageImpl(image, destRect, image->frame());
    else {
        const CSRect& frame = image->frame();
        float rate = CSMath::min(destRect.width / frame.width, destRect.height / frame.height);
        float x = destRect.center();
        float y = destRect.middle();
        float w = frame.width * rate;
        float h = frame.height * rate;
        drawImageImpl(image, CSZRect(x - w * 0.5f, y - h * 0.5f, destRect.z, w, h), frame);
    }
}

void CSGraphics::drawImage(const CSImage* image, const CSVector3& point, CSAlign align) {
    if (!image) return;

    drawImageImpl(image, image->displayRect(point, align), image->frame());
}

void CSGraphics::drawImageScaled(const CSImage* image, const CSVector3& point, float scale, CSAlign align) {
    if (!image) return;

    drawImageImpl(image, image->displayRect(point, align, scale), image->frame());
}

void CSGraphics::drawLineImage(const CSImage* image, float scroll, const CSVector3& src, const CSVector3& dest) {
    if (!image || src == dest) return;
    
    CSVector3 diff = dest - src;
    float len = diff.length();
    float angle = CSMath::atan2(diff.y, diff.x);

    const CSRect& frame = image->frame();

    int tw = image->texture()->width();
    int th = image->texture()->height();
    float cs = image->contentScale();

    float tv = frame.top() / th;
    float bv = frame.bottom() / th;
    
    float w = frame.width * cs;
    float hh = frame.height * cs * 0.5f;
    
    scroll -= (int)(scroll / w) * w;
    
    if (scroll > 0) scroll -= w;

    int capacity = len / w + 2;
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, capacity * 4, capacity * 6);
    setImageCommand(command, image);

    command->world.m41 += src.x;
    command->world.m42 += src.y;
    command->world = CSMatrix::rotationZ(angle) * command->world;

    int i = 0;
    do {
        float lu = 0;
        float ru = frame.width;
        float lx = scroll;
        float rx = scroll + w;
        if (scroll < 0) {
            lu = -scroll / cs;
            lx = 0;
        }
        if (scroll + w > len) {
            ru = (len - scroll) / cs;
            rx = len;
        }
        lu += frame.x;
        ru += frame.x;
        lu /= tw;
        ru /= tw;
        
        float lz = CSMath::lerp(src.z, dest.z, lx / len);
        float rz = CSMath::lerp(src.z, dest.z, rx / len);
        command->addVertex(CSFVertex(CSVector3(lx, -hh, lz), CSVector2(lu, tv)));
        command->addVertex(CSFVertex(CSVector3(rx, -hh, rz), CSVector2(ru, tv)));
        command->addVertex(CSFVertex(CSVector3(lx, hh, lz), CSVector2(lu, bv)));
        command->addVertex(CSFVertex(CSVector3(rx, hh, rz), CSVector2(ru, bv)));
        
        command->addIndex(i + 0);
        command->addIndex(i + 1);
        command->addIndex(i + 2);
        command->addIndex(i + 1);
        command->addIndex(i + 3);
        command->addIndex(i + 2);
        
        scroll += w;
        i += 4;
    } while (scroll < len);

    this->command(command);

    command->release();
}

void CSGraphics::drawClockImage(const CSImage* image, float progress, const CSVector3& point, CSAlign align) {
    if (!image) return;

    drawClockImage(image, progress, image->displayRect(point, align));
}

void CSGraphics::drawClockImageScaled(const CSImage* image, float progress, const CSVector3& point, float scale, CSAlign align) {
    if (!image) return;

    drawClockImage(image, progress, image->displayRect(point, align, scale));
}

void CSGraphics::drawClockImage(const CSImage* image, float progress, const CSZRect& destRect) {
    if (!image) return;

    if (progress >= 1.0f) {
        drawImage(image, destRect);
        return;
    }
    float a = progress * FloatTwoPi;
    
    static const float Angle0 = FloatPiOverFour;
    static const float Angle1 = FloatPiOverFour * 3;
    static const float Angle2 = FloatPiOverFour * 5;
    static const float Angle3 = FloatPiOverFour * 7;
    
    const CSRect& frame = image->frame();
    int tw = image->texture()->width();
    int th = image->texture()->height();

    float lu = frame.left() / tw;
    float ru = frame.right() / tw;
    float cu = frame.center() / tw;
    float tv = frame.top() / th;
    float bv = frame.bottom() / th;
    float mv = frame.middle() / th;

    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, 7, 18);
    setImageCommand(command, image);

    command->addVertex(CSFVertex(CSVector3(destRect.center(), destRect.middle(), destRect.z), CSVector2(cu, mv)));
    command->addVertex(CSFVertex(CSVector3(destRect.center(), destRect.top(), destRect.z), CSVector2(cu, tv)));
    
    if (a <= Angle0) {
        float t = CSMath::tan(a);
        
        command->addVertex(CSFVertex(CSVector3(destRect.center() + destRect.width * 0.5f * t, destRect.top(), destRect.z), CSVector2((frame.center() + frame.width * 0.5f * t) / tw, tv)));
    }
    else {
        command->addVertex(CSFVertex(CSVector3(destRect.right(), destRect.top(), destRect.z), CSVector2(ru, tv)));
        
        if (a <= Angle1) {
            float t = CSMath::tan(FloatPiOverTwo - a);
            
            command->addVertex(CSFVertex(CSVector3(destRect.right(), destRect.middle() - destRect.height * 0.5f * t, destRect.z), CSVector2(ru, (frame.middle() - frame.height * 0.5f * t) / th)));
        }
        else {
            command->addVertex(CSFVertex(CSVector3(destRect.right(), destRect.bottom(), destRect.z), CSVector2(ru, bv)));
                               
            if (a <= Angle2) {
                float t = CSMath::tan(a - FloatPi);
                
                command->addVertex(CSFVertex(CSVector3(destRect.center() - destRect.width * 0.5f * t, destRect.bottom(), destRect.z), CSVector2((frame.center() - frame.width * 0.5f * t) / tw, bv)));
            }
            else {
                command->addVertex(CSFVertex(CSVector3(destRect.left(), destRect.bottom(), destRect.z), CSVector2(lu, bv)));
                
                if (a <= Angle3) {
                    float t = CSMath::tan(FloatPi * 1.5f - a);
                    
                    command->addVertex(CSFVertex(CSVector3(destRect.left(), destRect.middle() + destRect.height * 0.5f * t, destRect.z), CSVector2(lu, (frame.middle() + frame.height * 0.5f * t) / th)));
                }
                else {
                    command->addVertex(CSFVertex(CSVector3(destRect.left(), destRect.top(), destRect.z), CSVector2(lu, tv)));
                    
                    float t = CSMath::tan(a - FloatTwoPi);
                    
                    command->addVertex(CSFVertex(CSVector3(destRect.center() + destRect.width * 0.5f * t, destRect.top(), destRect.z), CSVector2((frame.center() + frame.width * 0.5f * t) / tw, tv)));
                }
            }
        }
    }
    for (int i = 0; i < command->vertexCount() - 1; i++) {
        command->addIndex(0);
        command->addIndex(i);
        command->addIndex(i + 1);
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawStretchImage(const CSImage *image, const CSZRect &destRect, float horizontal, float vertical) {
    if (!image) {
        return;
    }
    const CSRect& frame = image->frame();
    int tw = image->texture()->width();
    int th = image->texture()->height();
    
    float u0 = frame.left() / tw;
    float u1 = (frame.left() + horizontal) / tw;
    float u2 = (frame.right() - horizontal) / tw;
    float u3 = frame.right() / tw;
    float v0 = frame.top() / th;
    float v1 = (frame.top() + vertical) / th;
    float v2 = (frame.bottom() - vertical) / th;
    float v3 = frame.bottom() / th;
    
    float hw = CSMath::min(horizontal, destRect.width * 0.5f);
    float hh = CSMath::min(vertical, destRect.height * 0.5f);
    
    float x0 = destRect.left();
    float x1 = x0 + hw;
    float x3 = destRect.right();
    float x2 = x3 - hw;
    
    float y0 = destRect.top();
    float y1 = y0 + hh;
    float y3 = destRect.bottom();
    float y2 = y3 - hh;
    
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, 16, 54);
    setImageCommand(command, image);

    command->addVertex(CSFVertex(CSVector3(x0, y0, destRect.z), CSVector2(u0, v0)));
    command->addVertex(CSFVertex(CSVector3(x1, y0, destRect.z), CSVector2(u1, v0)));
    command->addVertex(CSFVertex(CSVector3(x2, y0, destRect.z), CSVector2(u2, v0)));
    command->addVertex(CSFVertex(CSVector3(x3, y0, destRect.z), CSVector2(u3, v0)));
    command->addVertex(CSFVertex(CSVector3(x0, y1, destRect.z), CSVector2(u0, v1)));
    command->addVertex(CSFVertex(CSVector3(x1, y1, destRect.z), CSVector2(u1, v1)));
    command->addVertex(CSFVertex(CSVector3(x2, y1, destRect.z), CSVector2(u2, v1)));
    command->addVertex(CSFVertex(CSVector3(x3, y1, destRect.z), CSVector2(u3, v1)));
    command->addVertex(CSFVertex(CSVector3(x0, y2, destRect.z), CSVector2(u0, v2)));
    command->addVertex(CSFVertex(CSVector3(x1, y2, destRect.z), CSVector2(u1, v2)));
    command->addVertex(CSFVertex(CSVector3(x2, y2, destRect.z), CSVector2(u2, v2)));
    command->addVertex(CSFVertex(CSVector3(x3, y2, destRect.z), CSVector2(u3, v2)));
    command->addVertex(CSFVertex(CSVector3(x0, y3, destRect.z), CSVector2(u0, v3)));
    command->addVertex(CSFVertex(CSVector3(x1, y3, destRect.z), CSVector2(u1, v3)));
    command->addVertex(CSFVertex(CSVector3(x2, y3, destRect.z), CSVector2(u2, v3)));
    command->addVertex(CSFVertex(CSVector3(x3, y3, destRect.z), CSVector2(u3, v3)));
    
    for (int i = 0; i < 12; i += 4) {
        for (int j = 0; j < 3; j++) {
            int k = i + j;
            command->addIndex(k);
            command->addIndex(k + 1);
            command->addIndex(k + 4);
            command->addIndex(k + 1);
            command->addIndex(k + 5);
            command->addIndex(k + 4);
        }
    }

    this->command(command);

    command->release();
}

void CSGraphics::drawStretchImage(const CSImage* image, const CSZRect& destRect) {
    if (!image) return;

    drawStretchImage(image, destRect, image->width() * 0.5f, image->height() * 0.5f);
}

void CSGraphics::drawShadowFlatImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame, const CSVector2& offset, bool xflip, bool yflip) {
    const CSShadow2DRenderer::Param* param = static_assert_cast<const CSShadow2DRenderer::Param*>(_state->batch.rendererParam.value());

    float lz = CSMath::abs(param->lightDirection.z);
    if (lz == 0 || lz == 1) return;
    
    CSVector2 ld = -(CSVector2)param->lightDirection;
    float ll = ld.length();
    if (ll == 0) return;
    ld /= ll;
    
    float length = ll / lz;
    
    CSVector2 cameraRight = CSVector2::normalize((CSVector2)_state->batch.camera.right());
    CSVector2 cameraDown(-cameraRight.y, cameraRight.x);
    CSVector2 bottomLeft = cameraRight * (xflip ? -destRect.right() : destRect.left()) + cameraDown * offset.x * destRect.width;
    CSVector2 bottomRight = cameraRight * (xflip ? -destRect.left() : destRect.right()) + cameraDown * offset.y * destRect.height;
    float bottom = yflip ? destRect.top() : -destRect.bottom();
    if (bottom > 0) bottom *= length;
    bottom += destRect.z * length;
    CSVector2 dir = ld * bottom;
    bottomLeft += dir;
    bottomRight += dir;
    
    CSVector2 lr(ld.y, -ld.x);
    float sin = CSVector2::dot(lr, CSVector2::normalize(bottomRight - bottomLeft));
    if (sin < 0) {
        lr = -lr;
        sin = -sin;
        yflip = false;
    }
    
    static const float ThicknessMin = 0.75f;
    static const float ThicknessSinMax = CSMath::sin(CSMath::atan(ThicknessMin));
    
    if (sin < ThicknessSinMax) {
        float tan = sin / CSMath::sqrt(1 - sin * sin);
        CSVector2 ar = lr * ((ThicknessMin - tan) * destRect.width * 0.5f);
        bottomLeft -= ar;
        bottomRight += ar;
    }
    CSVector2 topLeft = bottomLeft;
    CSVector2 topRight = bottomRight;
    dir = ld * (-bottom + ((yflip ? destRect.bottom() : -destRect.top()) + destRect.z) * length);
    topLeft += dir;
    topRight += dir;
    
    float lu, ru, tv, lbv, rbv;
    if (xflip) {
        lu = frame.right();
        ru = frame.left();
    }
    else {
        lu = frame.left();
        ru = frame.right();
    }
    if (yflip) {
        tv = frame.bottom();
        lbv = frame.top() - CSMath::min(offset.x, 0.0f);
        rbv = frame.top() - CSMath::min(offset.y, 0.0f);
    }
    else {
        tv = frame.top();
        lbv = frame.bottom() + CSMath::min(offset.x, 0.0f);
        rbv = frame.bottom() + CSMath::min(offset.y, 0.0f);
    }
    int tw = image->texture()->width();
    int th = image->texture()->height();
    lu /= tw;
    ru /= tw;
    tv /= th;
    lbv /= th;
    rbv /= th;
    
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles);
    setImageCommand(command, image);

    command->addVertex(CSFVertex(topLeft, CSColor::Black, CSVector2(lu, tv)));
    command->addVertex(CSFVertex(topRight, CSColor::Black, CSVector2(ru, tv)));
    command->addVertex(CSFVertex(bottomLeft, CSColor::Black, CSVector2(lu, lbv)));
    command->addVertex(CSFVertex(bottomRight, CSColor::Black, CSVector2(ru, rbv)));

    command->addIndex(0);
    command->addIndex(1);
    command->addIndex(2);
    command->addIndex(1);
    command->addIndex(3);
    command->addIndex(2);

    this->command(command);

    command->release();
}

void CSGraphics::drawShadowFlatImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect, const CSVector2& offset, bool xflip, bool yflip) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    CSRect subframe = srcRect;
    const CSRect& frame = image->frame();
    subframe.x += frame.x;
    subframe.y += frame.y;
    subframe.intersect(frame);
    
    drawShadowFlatImageImpl(image, destRect, subframe, offset, xflip, yflip);
}

void CSGraphics::drawShadowFlatImage(const CSImage* image, const CSZRect& destRect, const CSVector2& offset, bool xflip, bool yflip) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    drawShadowFlatImageImpl(image, destRect, image->frame(), offset, xflip, yflip);
}

void CSGraphics::drawShadowFlatImage(const CSImage* image, const CSVector3& point, CSAlign align, const CSVector2& offset, bool xflip, bool yflip) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    drawShadowFlatImageImpl(image, image->displayRect(point, align), image->frame(), offset, xflip, yflip);
}

void CSGraphics::drawShadowRotateImageImpl(const CSImage* image, const CSZRect& destRect, const CSRect& frame, const CSVector2& offset, float flatness) {
    const CSShadow2DRenderer::Param* param = static_assert_cast<const CSShadow2DRenderer::Param*>(_state->batch.rendererParam.value());
    
    float lz = CSMath::abs(param->lightDirection.z);
    if (lz == 0 || lz == 1) return;
    
    CSVector2 ld = -(CSVector2)param->lightDirection;
    float ll = ld.length();
    if (ll == 0) return;
    ld /= ll;
    
    float length = ll / lz;
    
    CSVector2 cameraRight = CSVector2::normalize((CSVector2)_state->batch.camera.right());
    CSVector2 cameraDown(-cameraRight.y, cameraRight.x);
    CSVector2 lr(ld.y, -ld.x);
    float sin = CSVector2::dot(lr, cameraRight);
    if (sin < 0) lr = -lr;
    
    CSVector2 bottomLeft = lr * destRect.left();
    CSVector2 bottomRight = lr * destRect.right();
    CSVector2::lerp(bottomLeft, cameraRight * destRect.left(), flatness, bottomLeft);
    CSVector2::lerp(bottomRight, cameraRight * destRect.right(), flatness, bottomRight);
    float bottom = -destRect.bottom();
    if (bottom > 0) bottom *= length;
    bottom += destRect.z * length;
    CSVector2 dir = ld * bottom + cameraRight * offset.x + cameraDown * offset.y;
    bottomLeft += dir;
    bottomRight += dir;
    
    CSVector2 topLeft = bottomLeft;
    CSVector2 topRight = bottomRight;
    dir = ld * (-bottom + (-destRect.top() + destRect.z) * length);
    topLeft += dir;
    topRight += dir;
    
    int tw = image->texture()->width();
    int th = image->texture()->height();
    float lu = frame.left() / tw;
    float ru = frame.right() / tw;
    float tv = frame.top() / th;
    float bv = frame.bottom() / th;
    
    CSStreamRenderCommand* command = new CSStreamRenderCommand(this, CSPrimitiveTriangles);
    setImageCommand(command, image);
    
    command->addVertex(CSFVertex(topLeft, CSColor::Black, CSVector2(lu, tv)));
    command->addVertex(CSFVertex(topRight, CSColor::Black, CSVector2(ru, tv)));
    command->addVertex(CSFVertex(bottomLeft, CSColor::Black, CSVector2(lu, bv)));
    command->addVertex(CSFVertex(bottomRight, CSColor::Black, CSVector2(ru, bv)));
    
    command->addIndex(0);
    command->addIndex(1);
    command->addIndex(2);
    command->addIndex(1);
    command->addIndex(3);
    command->addIndex(2);
    
    this->command(command);

    command->release();
}

void CSGraphics::drawShadowRotateImage(const CSImage* image, const CSZRect& destRect, const CSRect& srcRect, const CSVector2& offset, float flatness) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    CSRect subframe = srcRect;
    const CSRect& frame = image->frame();
    subframe.x += frame.x;
    subframe.y += frame.y;
    subframe.intersect(frame);
    
    drawShadowRotateImageImpl(image, destRect, subframe, offset, flatness);
}

void CSGraphics::drawShadowRotateImage(const CSImage* image, const CSZRect& destRect, const CSVector2& offset, float flatness) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    drawShadowRotateImageImpl(image, destRect, image->frame(), offset, flatness);
}

void CSGraphics::drawShadowRotateImage(const CSImage* image, const CSVector3& point, CSAlign align, const CSVector2& offset, float flatness) {
    if (!image || _state->batch.renderer != CSRenderers::shadow2D()) return;

    drawShadowRotateImageImpl(image, image->displayRect(point, align), image->frame(), offset, flatness);
}
