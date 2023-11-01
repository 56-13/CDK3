#define CDK_IMPL

#include "CSSpriteElement.h"

#include "CSBuffer.h"

#include "CSSpriteElementImage.h"
#include "CSSpriteElementMesh.h"
#include "CSSpriteElementString.h"
#include "CSSpriteElementLine.h"
#include "CSSpriteElementGradientLine.h"
#include "CSSpriteElementRect.h"
#include "CSSpriteElementGradientRect.h"
#include "CSSpriteElementRoundRect.h"
#include "CSSpriteElementGradientRoundRect.h"
#include "CSSpriteElementArc.h"
#include "CSSpriteElementGradientArc.h"
#include "CSSpriteElementSphere.h"
#include "CSSpriteElementBox.h"
#include "CSSpriteElementExtern.h"
#include "CSSpriteElementTranslate.h"
#include "CSSpriteElementRotate.h"
#include "CSSpriteElementScale.h"
#include "CSSpriteElementInvert.h"
#include "CSSpriteElementColor.h"
#include "CSSpriteElementStroke.h"
#include "CSSpriteElementBrightness.h"
#include "CSSpriteElementContrast.h"
#include "CSSpriteElementSaturation.h"
#include "CSSpriteElementBlur.h"
#include "CSSpriteElementLens.h"
#include "CSSpriteElementWave.h"

CSSpriteElement* CSSpriteElement::createWithBuffer(CSBuffer* buffer) {
	switch (buffer->readByte()) {
        case TypeImage:
            return new CSSpriteElementImage(buffer);
        case TypeMesh:
            return new CSSpriteElementMesh(buffer);
        case TypeString:
            return new CSSpriteElementString(buffer);
        case TypeLine:
            return new CSSpriteElementLine(buffer);
        case TypeGradientLine:
            return new CSSpriteElementGradientLine(buffer);
        case TypeRect:
            return new CSSpriteElementRect(buffer);
        case TypeGradientRect:
            return new CSSpriteElementGradientRect(buffer);
        case TypeRoundRect:
            return new CSSpriteElementRoundRect(buffer);
        case TypeGradientRoundRect:
            return new CSSpriteElementGradientRoundRect(buffer);
        case TypeArc:
            return new CSSpriteElementArc(buffer);
        case TypeGradientArc:
            return new CSSpriteElementGradientArc(buffer);
        case TypeSphere:
            return new CSSpriteElementSphere(buffer);
        case TypeBox:
            return new CSSpriteElementBox(buffer);
        case TypeExtern:
            return new CSSpriteElementExtern(buffer);
        case TypeTranslate:
            return new CSSpriteElementTranslate(buffer);
        case TypeRotate:
            return new CSSpriteElementRotate(buffer);
        case TypeScale:
            return new CSSpriteElementScale(buffer);
        case TypeInvert:
            return new CSSpriteElementInvert(buffer);
        case TypeColor:
            return new CSSpriteElementColor(buffer);
        case TypeStroke:
            return new CSSpriteElementStroke(buffer);
        case TypeBrightness:
            return new CSSpriteElementBrightness(buffer);
        case TypeContrast:
            return new CSSpriteElementContrast(buffer);
        case TypeSaturation:
            return new CSSpriteElementSaturation(buffer);
        case TypeBlur:
            return new CSSpriteElementBlur(buffer);
        case TypeLens:
            return new CSSpriteElementLens(buffer);
        case TypeWave:
            return new CSSpriteElementWave(buffer);
	}
    CSAssert(false);
    return NULL;
}


CSSpriteElement* CSSpriteElement::createWithElement(const CSSpriteElement* other) {
    switch (other->type()) {
        case TypeImage:
            return new CSSpriteElementImage(static_cast<const CSSpriteElementImage*>(other));
        case TypeMesh:
            return new CSSpriteElementMesh(static_cast<const CSSpriteElementMesh*>(other));
        case TypeString:
            return new CSSpriteElementString(static_cast<const CSSpriteElementString*>(other));
        case TypeLine:
            return new CSSpriteElementLine(static_cast<const CSSpriteElementLine*>(other));
        case TypeGradientLine:
            return new CSSpriteElementGradientLine(static_cast<const CSSpriteElementGradientLine*>(other));
        case TypeRect:
            return new CSSpriteElementRect(static_cast<const CSSpriteElementRect*>(other));
        case TypeGradientRect:
            return new CSSpriteElementGradientRect(static_cast<const CSSpriteElementGradientRect*>(other));
        case TypeRoundRect:
            return new CSSpriteElementRoundRect(static_cast<const CSSpriteElementRoundRect*>(other));
        case TypeGradientRoundRect:
            return new CSSpriteElementGradientRoundRect(static_cast<const CSSpriteElementGradientRoundRect*>(other));
        case TypeArc:
            return new CSSpriteElementArc(static_cast<const CSSpriteElementArc*>(other));
        case TypeGradientArc:
            return new CSSpriteElementGradientArc(static_cast<const CSSpriteElementGradientArc*>(other));
        case TypeSphere:
            return new CSSpriteElementSphere(static_cast<const CSSpriteElementSphere*>(other));
        case TypeBox:
            return new CSSpriteElementBox(static_cast<const CSSpriteElementBox*>(other));
        case TypeExtern:
            return new CSSpriteElementExtern(static_cast<const CSSpriteElementExtern*>(other));
        case TypeTranslate:
            return new CSSpriteElementTranslate(static_cast<const CSSpriteElementTranslate*>(other));
        case TypeRotate:
            return new CSSpriteElementRotate(static_cast<const CSSpriteElementRotate*>(other));
        case TypeScale:
            return new CSSpriteElementScale(static_cast<const CSSpriteElementScale*>(other));
        case TypeInvert:
            return new CSSpriteElementInvert(static_cast<const CSSpriteElementInvert*>(other));
        case TypeColor:
            return new CSSpriteElementColor(static_cast<const CSSpriteElementColor*>(other));
        case TypeStroke:
            return new CSSpriteElementStroke(static_cast<const CSSpriteElementStroke*>(other));
        case TypeBrightness:
            return new CSSpriteElementBrightness(static_cast<const CSSpriteElementBrightness*>(other));
        case TypeContrast:
            return new CSSpriteElementContrast(static_cast<const CSSpriteElementContrast*>(other));
        case TypeSaturation:
            return new CSSpriteElementSaturation(static_cast<const CSSpriteElementSaturation*>(other));
        case TypeBlur:
            return new CSSpriteElementBlur(static_cast<const CSSpriteElementBlur*>(other));
        case TypeLens:
            return new CSSpriteElementLens(static_cast<const CSSpriteElementLens*>(other));
        case TypeWave:
            return new CSSpriteElementWave(static_cast<const CSSpriteElementWave*>(other));
    }
    CSAssert(false);
    return NULL;
}