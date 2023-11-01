#define CDK_IMPL

#include "CSGraphics.h"

#include "CSGlyphs.h"

#include "CSStreamRenderCommand.h"

#define LineBreakEpsilon    0.001f

void CSGraphics::paragraphDisplays(const StringParam& str, const CSFont* font, CSArray<CSGraphics::ParagraphDisplay>* result) {
    CSGlyphs* cs = CSGlyphs::sharedGlyphs();

    result->removeAllObjects();
    result->setCapacity(str.display->paragraphs()->count());

    foreach (const CSStringParagraph&, pa, str.display->paragraphs()) {
        switch (pa.type) {
            case CSStringParagraph::TypeLinebreak:
                {
                    ParagraphDisplay& d = result->addObject();
                    d.font = font;
                    d.size = CSVector2::Zero;
                    if (pa.attr.text.end <= str.start) d.visible = ParagraphDisplay::Forward;
                    else if (pa.attr.text.start >= str.end) d.visible = ParagraphDisplay::Backward;
                    else {
                        d.size.y = font->size();
                        d.visible = ParagraphDisplay::Visible;
                    }
                }
                break;
            case CSStringParagraph::TypeNeutral:
            case CSStringParagraph::TypeLTR:
            case CSStringParagraph::TypeRTL:
            case CSStringParagraph::TypeSpace:
                {
                    ParagraphDisplay& d = result->addObject();
                    d.font = font;
                    d.size = CSVector2::Zero;
                    if (pa.attr.text.end <= str.start) d.visible = ParagraphDisplay::Forward;
                    else if (pa.attr.text.start >= str.end) d.visible = ParagraphDisplay::Backward;
                    else {
                        const CSCharSequence* characters = str.display->characters();
                        const uchar* content = str.display->content();

                        for (int ci = pa.attr.text.start; ci < pa.attr.text.end; ci++) {
                            const uchar* cc = content + characters->from(ci);
                            int cclen = characters->length(ci);

                            const CSVector2& ccs = cs->size(cc, cclen, font);

                            d.size.x += ccs.x;
                            if (d.size.y < ccs.y) d.size.y = ccs.y;
                        }
                        d.visible = ParagraphDisplay::Visible;
                    }
                }
                break;
            case CSStringParagraph::TypeFont:
                {
                    CSFontStyle style = pa.attr.font.style >= 0 ? pa.attr.font.style : font->style();
                    font = pa.attr.font.name ? CSFont::font(pa.attr.font.name, pa.attr.font.size, style) : font->derivedFont(pa.attr.font.size, style);
                }
            default:
                {
                    ParagraphDisplay& d = result->addObject();
                    d.font = font;
                    d.size = CSVector2::Zero;
                    d.visible = ParagraphDisplay::Visible;
                }
                break;
        }
    }
}

int CSGraphics::stringIndex(const StringParam& str, int ci) {
    foreach (const CSStringParagraph&, pa, str.display->paragraphs()) {
        switch (pa.type) {
            case CSStringParagraph::TypeNeutral:
            case CSStringParagraph::TypeLTR:
            case CSStringParagraph::TypeRTL:
            case CSStringParagraph::TypeSpace:
            case CSStringParagraph::TypeLinebreak:
                if (ci >= pa.attr.text.start && ci < pa.attr.text.end) return ci;
                else if (ci < pa.attr.text.start) return pa.attr.text.start;
                break;
        }
    }
    return ci;
}

CSVector2 CSGraphics::stringSizeImpl(const StringParam& str, float width, const CSArray<ParagraphDisplay>* paraDisplays) {
    CSVector2 size = CSVector2::Zero;
    CSVector2 lineSize = CSVector2::Zero;
    float space = 0;

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays->objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeLinebreak:
                    {
                        if (size.x < lineSize.x) size.x = lineSize.x;
                        size.y += CSMath::max(lineSize.y, pd.size.y);
                        lineSize = CSVector2::Zero;
                        space = 0;
                    }
                    break;
                case CSStringParagraph::TypeSpace:
                    if (lineSize.x) {
                        if (lineSize.y < pd.size.y) lineSize.y = pd.size.y;
                        space += pd.size.x;
                    }
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    {
                        if (lineSize.x && lineSize.x + space + pd.size.x > width) {
                            if (size.x < lineSize.x) size.x = lineSize.x;
                            size.y += CSMath::max(lineSize.y, pd.size.y);
                            lineSize = pd.size;
                        }
                        else {
                            if (lineSize.y < pd.size.y) lineSize.y = pd.size.y;
                            lineSize.x += space + pd.size.x;
                        }
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;
    }

    if (size.x < lineSize.x) size.x = lineSize.x;
    if (lineSize.x) size.y += lineSize.y;

    return size;
}

CSVector2 CSGraphics::stringSize(const StringParam& str, const CSFont* font, float width) {
    if (!str) return CSVector2::Zero;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, font, &paraDisplays);

    return stringSizeImpl(str, width, &paraDisplays);
}

CSVector2 CSGraphics::stringSize(const StringParam& str, float width) const {
	return stringSize(str, _state->font, width);
}

CSVector2 CSGraphics::stringLineSizeImpl(const StringParam& str, int pi, float width, const CSArray<ParagraphDisplay>* paraDisplays) {
    CSVector2 size = CSVector2::Zero;
    float space = 0;

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    while (pi < paras->count()) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays->objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeLinebreak:
                    if (size.y < pd.size.y) size.y = pd.size.y;
                    goto exit;
                case CSStringParagraph::TypeSpace:
                    if (size.x) {
                        if (size.y < pd.size.y) size.y = pd.size.y;
                        space += pd.size.x;
                    }
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    {
                        if (size.x && size.x + space + pd.size.x > width) goto exit;
                        if (size.y < pd.size.y) size.y = pd.size.y;
                        size.x += space + pd.size.x;
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;

        pi++;
    }
exit:
    return size;
}

string CSGraphics::shrinkString(const StringParam& str, const CSFont* font, float width, float scroll) {
    if (!str) return NULL;
        
    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, font, &paraDisplays);

    CSVector2 lineSize = CSVector2::Zero;
    float space = 0;
    int fromIndex = 0;

    const CSCharSequence* characters = str.display->characters();

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays.objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeLinebreak:
                    scroll -= CSMath::max(lineSize.y, pd.size.y);
                    fromIndex = characters->to(pa.attr.text.end);
                    if (scroll <= 0) goto exit;
                    lineSize = CSVector2::Zero;
                    space = 0;
                    break;
                case CSStringParagraph::TypeSpace:
                    if (lineSize.x) {
                        if (lineSize.y < pd.size.y) lineSize.y = pd.size.y;
                        space += pd.size.x;
                    }
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    {
                        if (lineSize.x && lineSize.x + space + pd.size.x > width) {
                            scroll -= CSMath::max(lineSize.y, pd.size.y);
                            fromIndex = characters->from(pa.attr.text.start);
                            if (scroll <= 0) goto exit;
                            lineSize = pd.size;
                        }
                        else {
                            if (lineSize.y < pd.size.y) lineSize.y = pd.size.y;
                            lineSize.x += space + pd.size.x;
                        }
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;
    }
exit:
    if (scroll > lineSize.y) return string();

    const uchar* content = str.display->content();

    return string(string::cstring(content + fromIndex, u_strlen(content) - fromIndex));
}

static float stringX(int align, float width, float lineWidth, bool rtl) {
    switch (align) {
        case 0:
            return rtl ? width - lineWidth : 0;
        case 1:
            return (rtl ? width + lineWidth : width - lineWidth) * 0.5f;
        case 2:
            return rtl ? width : width - lineWidth;
    }
    CSAssert(false, "invalid paramter");
    return 0;
}

static int paragraphCursor(const CSGraphics::StringParam& str, const CSStringParagraph& pa, const CSFont* font, float cx) {
    CSGlyphs* cs = CSGlyphs::sharedGlyphs();

    const CSCharSequence* characters = str.display->characters();
    const uchar* content = str.display->content();

    if (str.display->isRTL() && pa.type != CSStringParagraph::TypeLTR) {
        if (cx <= 0) return pa.attr.text.end;
        for (int ci = (int)pa.attr.text.end - 1; ci >= (int)pa.attr.text.start; ci--) {
            const uchar* cc = content + characters->from(ci);
            int cclen = characters->length(ci);
            cx -= cs->size(cc, cclen, font).x;
            if (cx <= 0) return ci;
        }
        return pa.attr.text.start;
    }
    else {
        if (cx <= 0) return pa.attr.text.start;
        for (int ci = pa.attr.text.start; ci < pa.attr.text.end; ci++) {
            const uchar* cc = content + characters->from(ci);
            int cclen = characters->length(ci);
            cx -= cs->size(cc, cclen, font).x;
            if (cx <= 0) return ci;
        }
        return pa.attr.text.end;
    }
}

int CSGraphics::stringCursor(const StringParam& str, const CSFont* font, const CSVector2& target, float width) {
	if (!str) return 0;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, font, &paraDisplays);

    bool rtl = str.display->isRTL();
    int align = rtl ? 2 : 0;
    float x = rtl ? width : 0, y = 0, space = 0, lineWidth = 0;
    CSVector2 lineSize = stringLineSizeImpl(str, 0, width, &paraDisplays);

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays.objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeAlign:
                    if (align != pa.attr.align) {
                        align = pa.attr.align;
                        x = stringX(align, width, lineSize.x, rtl);
                    }
                    break;
                case CSStringParagraph::TypeLinebreak:
                    {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi + 1, width, &paraDisplays);
                        x = stringX(align, width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    break;
                case CSStringParagraph::TypeSpace:
                    if (lineWidth) {
                        if (y + lineSize.y >= target.y) {
                            if (rtl) {
                                if (x - space - pd.size.x <= target.x) return paragraphCursor(str, pa, pd.font, x - space - target.x);
                            }
                            else {
                                if (x + space + pd.size.x >= target.x) return paragraphCursor(str, pa, pd.font, target.x - x - space);
                            }
                        }
                        space += pd.size.x;
                    }
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    if (lineWidth && lineWidth + space + pd.size.x > width + LineBreakEpsilon) {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi, width, &paraDisplays);
                        x = stringX(align, width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    if (y + lineSize.y >= target.y) {
                        if (rtl) {
                            if (x - space - pd.size.x <= target.x) return paragraphCursor(str, pa, pd.font, x - space - target.x);
                        }
                        else {
                            if (x + space + pd.size.x >= target.x) return paragraphCursor(str, pa, pd.font, target.x - x - space);
                        }
                    }
                    {
                        float pw = space + pd.size.x;
                        lineWidth += pw;
                        x += rtl ? -pw : pw;
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) return pa.attr.text.start;
    }
    return str.display->characters()->count();
}

int CSGraphics::stringCursor(const StringParam& str, const CSVector2& target, float width) const {
	return stringCursor(str, _state->font, target, width);
}

CSVector2 CSGraphics::stringPosition(const StringParam& str, const CSFont* font, float width) {
	if (!str) return CSVector2::Zero;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, font, &paraDisplays);

    bool rtl = str.display->isRTL();
    int align = rtl ? 2 : 0;
    float x = rtl ? width : 0, y = 0, space = 0, lineWidth = 0;
    CSVector2 lineSize = stringLineSizeImpl(str, 0, width, &paraDisplays);

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays.objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeAlign:
                    if (align != pa.attr.align) {
                        align = pa.attr.align;
                        x = stringX(align, width, lineSize.x, rtl);
                    }
                    break;
                case CSStringParagraph::TypeLinebreak:
                    {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi + 1, width, &paraDisplays);
                        x = stringX(align, width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    break;
                case CSStringParagraph::TypeSpace:
                    if (lineWidth) space += pd.size.x;
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    if (lineWidth && lineWidth + space + pd.size.x > width + LineBreakEpsilon) {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi, width, &paraDisplays);
                        x = stringX(align, width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    {
                        float pw = space + pd.size.x;
                        lineWidth += pw;
                        x += rtl ? -pw : pw;
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;
    }
    return CSVector2(x, y);
}

CSVector2 CSGraphics::stringPosition(const StringParam& str, float width) const {
	return stringPosition(str, _state->font, width);
}

void CSGraphics::drawStringCharacter(const CSImage* image, const CSVector3& pos, int capacity, CSStreamRenderCommand*& command) {
    const CSTexture* texture = image->texture();
    if (!command || command->state.material.colorMap != texture) {
        if (command) {
            this->command(command);
            command->release();
        }
        command = new CSStreamRenderCommand(this, CSPrimitiveTriangles, 4 * capacity, 6 * capacity);
        command->state.material.colorMap = texture;
    }

    const CSRect& frame = image->frame();

    float lu = frame.left() / texture->width();
    float ru = frame.right() / texture->width();
    float tv = frame.top() / texture->height();
    float bv = frame.bottom() / texture->height();

    int vi = command->vertexCount();

    command->addVertex(CSFVertex(pos, _state->fontColors[0], CSVector2(lu, tv)));
    command->addVertex(CSFVertex(CSVector3(pos.x + frame.width, pos.y, pos.z), _state->fontColors[1], CSVector2(ru, tv)));
    command->addVertex(CSFVertex(CSVector3(pos.x, pos.y + frame.height, pos.z), _state->fontColors[2], CSVector2(lu, bv)));
    command->addVertex(CSFVertex(CSVector3(pos.x + frame.width, pos.y + frame.height, pos.z), _state->fontColors[3], CSVector2(ru, bv)));

    command->addIndex(vi + 0);
    command->addIndex(vi + 1);
    command->addIndex(vi + 2);
    command->addIndex(vi + 1);
    command->addIndex(vi + 3);
    command->addIndex(vi + 2);
}

void CSGraphics::drawStringCharacterEnd(CSStreamRenderCommand*& command) {
    if (command) {
        this->command(command);
        command->release();
        command = NULL;
    }
}

void CSGraphics::drawStringParagraphs(const StringParam& str, const CSVector3& point) {
    push();

    _state->batch.material.colorMap = NULL;
    _state->batch.material.normalMap = NULL;
    _state->batch.material.materialMap = NULL;
    _state->batch.material.emissiveMap = NULL;
    _state->batch.layer = 1;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);
    
    CSGlyphs* cs = CSGlyphs::sharedGlyphs();

    CSColor originColor = _state->color;
    CSColor currentColor = originColor;

    const uchar* content = str.display->content();
    const CSCharSequence* characters = str.display->characters();

    float y = point.y;

    CSStreamRenderCommand* command = NULL;

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays.objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeColor:
                    if (pa.attr.color != CSColor::Transparent) {
                        _state->color = currentColor = pa.attr.color * originColor;
                    }
                    else {
                        _state->color = currentColor = originColor;
                    }
                    if (command) command->color = currentColor;
                    break;
                case CSStringParagraph::TypeStroke:
                    drawStringCharacterEnd(command);
                    if (pa.attr.stroke.color != CSColor::Transparent) _state->batch.strokeColor = pa.attr.stroke.color;
                    _state->batch.strokeWidth = pa.attr.stroke.width;
                    break;
                case CSStringParagraph::TypeGradient:
                    _state->color = originColor;
                    if (pa.attr.gradient.horizontal) setFontColorH(pa.attr.gradient.color[0], pa.attr.gradient.color[1]);
                    else setFontColorV(pa.attr.gradient.color[0], pa.attr.gradient.color[1]);
                    if (command) command->color = originColor;
                    break;
                case CSStringParagraph::TypeGradientReset:
                    _state->color = originColor;
                    resetFontColor();
                    if (command) command->color = currentColor;
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    {
                        float x = point.x;
                        bool crtl = pa.type == CSStringParagraph::TypeRTL;
                        if (crtl) x += pd.size.x;

                        for (int ci = pa.attr.text.start; ci < pa.attr.text.end; ci++) {
                            const uchar* cc = content + characters->from(ci);
                            int cclen = characters->length(ci);

                            float ccw = cs->size(cc, cclen, pd.font).x;

                            float offy = 0;
                            const CSImage* image = cs->image(cc, cclen, pd.font, offy);

                            if (image) {
                                drawStringCharacter(image, CSVector3(crtl ? x - ccw : x, y + offy, point.z), characters->count(), command);
                            }
                            x += crtl ? -ccw : ccw;
                        }
                    }
                    y += pd.font->size();
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;
    }

    drawStringCharacterEnd(command);

    pop();
}

void CSGraphics::drawStringImpl(const StringParam& str, const CSZRect& destRect, float scroll, const CSArray<ParagraphDisplay>* paraDisplays) {
    push();

    _state->batch.material.colorMap = NULL;
    _state->batch.material.normalMap = NULL;
    _state->batch.material.materialMap = NULL;
    _state->batch.material.emissiveMap = NULL;
    _state->batch.layer = 1;

    CSGlyphs* cs = CSGlyphs::sharedGlyphs();
    
    CSColor originColor = _state->color;
    CSColor currentColor = originColor;
    
    bool rtl = str.display->isRTL();
    int align = rtl ? 2 : 0;
    float x = rtl ? destRect.width : 0, y = -scroll, space = 0, lineWidth = 0;
    CSVector2 lineSize = stringLineSizeImpl(str, 0, destRect.width, paraDisplays);

    const uchar* content = str.display->content();
    const CSCharSequence* characters = str.display->characters();
    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();

    CSStreamRenderCommand* command = NULL;

    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays->objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeColor:
                    if (pa.attr.color != CSColor::Transparent) {
                        _state->color = currentColor = pa.attr.color * originColor;
                    }
                    else {
                        _state->color = currentColor = originColor;
                    }
                    if (command) command->color = currentColor;
                    break;
                case CSStringParagraph::TypeStroke:
                    drawStringCharacterEnd(command);
                    if (pa.attr.stroke.color != CSColor::Transparent) _state->batch.strokeColor = pa.attr.stroke.color;
                    _state->batch.strokeWidth = pa.attr.stroke.width;
                    break;
                case CSStringParagraph::TypeAlign:
                    if (align != pa.attr.align) {
                        align = pa.attr.align;
                        x = stringX(align, destRect.width, lineSize.x, rtl);
                    }
                    break;
                case CSStringParagraph::TypeGradient:
                    _state->color = originColor;
                    if (pa.attr.gradient.horizontal) setFontColorH(pa.attr.gradient.color[0], pa.attr.gradient.color[1]);
                    else setFontColorV(pa.attr.gradient.color[0], pa.attr.gradient.color[1]);
                    if (command) command->color = originColor;
                    break;
                case CSStringParagraph::TypeGradientReset:
                    _state->color = originColor;
                    resetFontColor();
                    if (command) command->color = currentColor;
                    break;
                case CSStringParagraph::TypeLinebreak:
                    {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi + 1, destRect.width, paraDisplays);
                        if (y + lineSize.y > destRect.height) goto exit;
                        x = stringX(align, destRect.width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    break;
                case CSStringParagraph::TypeSpace:
                    if (lineWidth) space += pd.size.x;
                    break;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                    if (lineWidth && lineWidth + space + pd.size.x > destRect.width + LineBreakEpsilon) {
                        y += lineSize.y;
                        lineSize = stringLineSizeImpl(str, pi, destRect.width, paraDisplays);
                        if (y + lineSize.y > destRect.height) goto exit;
                        x = stringX(align, destRect.width, lineSize.x, rtl);
                        lineWidth = 0;
                        space = 0;
                    }
                    if (y + pd.size.y > 0) {
                        float cx = destRect.x + x + (rtl ? -space : space);
                        float cy = destRect.y + y;
                        bool crtl = pa.type == CSStringParagraph::TypeRTL;
                        if (rtl && !crtl) cx -= pd.size.x;

                        int start = CSMath::max(str.start, pa.attr.text.start);
                        int end = CSMath::min(str.end, pa.attr.text.end);

                        for (int ci = start; ci < end; ci++) {
                            const uchar* cc = content + characters->from(ci);
                            int cclen = characters->length(ci);

                            float ccw = cs->size(cc, cclen, pd.font).x;

                            float offy = 0;
                            const CSImage* image = cs->image(cc, cclen, pd.font, offy);

                            if (image) {
                                drawStringCharacter(image, CSVector3(crtl ? cx - ccw : cx, cy + offy, destRect.z), characters->count(), command);
                            }
                            cx += crtl ? -ccw : ccw;
                        }
                    }
                    {
                        float pw = space + pd.size.x;
                        lineWidth += pw;
                        x += rtl ? -pw : pw;
                        space = 0;
                    }
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) goto exit;
    }
exit:
    drawStringCharacterEnd(command);

    pop();
}

void CSGraphics::drawStringScrolled(const StringParam& str, const CSZRect& destRect, float scroll) {
    if (!str) return;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);
        
    drawStringImpl(str, destRect, scroll, &paraDisplays);
}

void CSGraphics::drawString(const StringParam& str, CSZRect destRect, CSAlign align) {
	if (!str) return;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    if (align & (CSAlignCenter | CSAlignRight | CSAlignMiddle | CSAlignBottom)) {
        CSVector2 size = stringSizeImpl(str, destRect.width, &paraDisplays);
        
        float scroll = 0;
        
        if (align & CSAlignCenter) {
            destRect.x += (destRect.width - size.x) * 0.5f;
            destRect.width = size.x;
        }
        else if (align & CSAlignRight) {
            destRect.x += destRect.width - size.x;
            destRect.width = size.x;
        }
        if (align & CSAlignMiddle) {
            if (size.y > destRect.height) {
                scroll = (size.y - destRect.height) * 0.5f;
            }
            else {
                destRect.y += (destRect.height - size.y) * 0.5f;
                destRect.height = size.y;
            }
        }
        else if (align & CSAlignBottom) {
            if (size.y > destRect.height) {
                scroll = size.y - destRect.height;
            }
            else {
                destRect.y += destRect.height - size.y;
                destRect.height = size.y;
            }
        }
        drawStringImpl(str, destRect, scroll, &paraDisplays);
    }
    else {
        drawStringImpl(str, destRect, 0, &paraDisplays);
    }
}

void CSGraphics::drawStringScaled(const StringParam& str, const CSZRect& destRect) {
    if (!str) return;
        
    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    CSVector2 size = stringSizeImpl(str, destRect.width, &paraDisplays);
    
    if (size.y > destRect.height) {
        float s = 0;
        
        for (; ; ) {
            float ns = CSMath::min(destRect.width / size.x, destRect.height / size.y);
            if (ns > s) {
                s = ns;
                size = stringSizeImpl(str, destRect.width / s, &paraDisplays);
            }
            else {
                break;
            }
        }
        pushTransform();
        translate(destRect.leftTop());
        scale(s);
        drawStringImpl(str, CSZRect(CSVector3::Zero, size), 0, &paraDisplays);
        popTransform();
    }
    else {
        drawStringImpl(str, destRect, 0, &paraDisplays);
    }
}

void CSGraphics::drawStringScaled(const StringParam& str, CSZRect destRect, CSAlign align) {
    if (!str) return;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

	CSVector2 size = stringSizeImpl(str, destRect.width, &paraDisplays);
    
    if (size.y > destRect.height) {
        float s = 0;
        
        for (; ; ) {
            float ns = CSMath::min(destRect.width / size.x, destRect.height / size.y);
            if (ns > s) {
                s = ns;
                size = stringSizeImpl(str, destRect.width / s, &paraDisplays);
            }
            else {
                break;
            }
        }
        
        if (align & CSAlignCenter) {
            destRect.x += (destRect.width - size.x * s) * 0.5f;
        }
        else if (align & CSAlignRight) {
            destRect.x += destRect.width - size.x * s;
        }
        if (align & CSAlignMiddle) {
            destRect.y += (destRect.height - size.y * s) * 0.5f;
        }
        else if (align & CSAlignBottom) {
            destRect.y += destRect.height - size.y * s;
        }
        destRect.width = size.x;
        destRect.height = size.y;

        pushTransform();
        translate(destRect.leftTop());
        scale(s);
        
        destRect.x = 0;
        destRect.y = 0;
        destRect.z = 0;
        
        drawStringImpl(str, destRect, 0, &paraDisplays);
        popTransform();
    }
    else {
        if (align & CSAlignCenter) {
            destRect.x += (destRect.width - size.x) * 0.5f;
        }
        else if (align & CSAlignRight) {
            destRect.x += destRect.width - size.x;
        }
        if (align & CSAlignMiddle) {
            destRect.y += (destRect.height - size.y) * 0.5f;
        }
        else if (align & CSAlignBottom) {
            destRect.y += destRect.height - size.y;
        }
        destRect.width = size.x;
        destRect.height = size.y;
        
        drawStringImpl(str, destRect, 0, &paraDisplays);
    }
}

float CSGraphics::drawString(const StringParam& str, const CSVector3& point, float width) {
	if (!str) return 0;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    if (width >= CSStringWidthUnlimited) width = stringSizeImpl(str, CSStringWidthUnlimited, &paraDisplays).x;
        
    drawStringImpl(str, CSZRect(point, CSVector2(width, CSStringWidthUnlimited)), 0, &paraDisplays);

    return width;
}

float CSGraphics::drawString(const StringParam& str, CSVector3 point, CSAlign align, float width) {
	if (!str) return 0;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    CSVector2 size = stringSizeImpl(str, width, &paraDisplays);

    applyAlign(point, size, align);
    drawStringImpl(str, CSZRect(point, size), 0, &paraDisplays);

    return width >= CSStringWidthUnlimited ? size.x : width;
}

void CSGraphics::drawStringScaled(const StringParam& str, const CSVector3& point, float width) {
	if (!str) return;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    CSVector2 size = stringSizeImpl(str, CSStringWidthUnlimited, &paraDisplays);
    
    if (size.x > width) {
        float s = width / size.x;
        
        pushTransform();
        translate(point);
        scale(s);
        drawStringImpl(str, CSZRect(CSVector3::Zero, CSVector2(width / s, size.y)), 0, &paraDisplays);
        popTransform();
    }
    else {
        size.x = width;
        drawStringImpl(str, CSZRect(point, size), 0, &paraDisplays);
    }
}

void CSGraphics::drawStringScaled(const StringParam& str, CSVector3 point, CSAlign align, float width) {
	if (!str) return;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    CSVector2 size = stringSizeImpl(str, CSStringWidthUnlimited, &paraDisplays);
    
    if (size.x > width) {
        float s = width / size.x;
        
        applyAlign(point, size * s, align);

        pushTransform();
        translate(point);
        scale(s);
        drawStringImpl(str, CSZRect(CSVector3::Zero, size), 0, &paraDisplays);
        popTransform();
    }
    else {
        applyAlign(point, size, align);
        drawStringImpl(str, CSZRect(point, size), 0, &paraDisplays);
    }
}

void CSGraphics::drawStringTruncated(const StringParam& str, const CSVector3& point, float width) {
    drawStringTruncated(str, point, CSAlignNone, width);
}

void CSGraphics::drawStringTruncated(StringParam str, CSVector3 point, CSAlign align, float width) {
    if (!str) return;

    CSGlyphs* cs = CSGlyphs::sharedGlyphs();

    static const string trstr("...");
    CSArray<CSGraphics::ParagraphDisplay> trpd(0);
    paragraphDisplays(trstr, _state->font, &trpd);
    float trw = stringSizeImpl(trstr, CSStringWidthUnlimited, &trpd).x;

    CSArray<CSGraphics::ParagraphDisplay> paraDisplays(0);
    paragraphDisplays(str, _state->font, &paraDisplays);

    bool rtl = str.display->isRTL();

    CSVector2 lineSize = CSVector2::Zero;

    const CSArray<CSStringParagraph>* paras = str.display->paragraphs();
    for (int pi = 0; pi < paras->count(); pi++) {
        const CSStringParagraph& pa = paras->objectAtIndex(pi);
        const ParagraphDisplay& pd = paraDisplays.objectAtIndex(pi);

        if (pd.visible == ParagraphDisplay::Visible) {
            switch (pa.type) {
                case CSStringParagraph::TypeLinebreak:
                    return;
                case CSStringParagraph::TypeNeutral:
                case CSStringParagraph::TypeLTR:
                case CSStringParagraph::TypeRTL:
                case CSStringParagraph::TypeSpace:
                    //if (lineSize.x + pd.size.x + trw > width) {
                    if (lineSize.x + pd.size.x > width) {
                        applyAlign(point, lineSize, align);
                        str.end = pa.attr.text.start;
                        if (str.start < str.end) {
                            drawStringImpl(str, CSZRect(point, CSVector2(width, CSStringWidthUnlimited)), 0, &paraDisplays);
                        }
                        drawStringImpl(trstr, CSZRect(rtl ? point.x - trw : point.x + lineSize.x, point.y, point.z, trw, CSStringWidthUnlimited), 0, &trpd);
                        return;
                    }
                    if (lineSize.y < pd.size.y) lineSize.y = pd.size.y;
                    lineSize.x += pd.size.x;
                    break;
            }
        }
        else if (pd.visible == ParagraphDisplay::Backward) break;
    }
    applyAlign(point, lineSize, align);
    drawStringImpl(str, CSZRect(point, CSVector2(width, CSStringWidthUnlimited)), 0, &paraDisplays);
}
