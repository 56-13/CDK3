#ifndef __CDK__CSSpriteElement__
#define __CDK__CSSpriteElement__

#include "CSRandom.h"

#include "CSGraphics.h"

#include "CSSceneObject.h"

class CSSpriteObject;
class CSBuffer;

class CSSpriteElement : public CSObject {
public:
	enum Type {
        TypeImage,
        TypeMesh,
        TypeString,
        TypeLine,
        TypeGradientLine,
        TypeRect,
        TypeGradientRect,
        TypeRoundRect,
        TypeGradientRoundRect,
        TypeArc,
        TypeGradientArc,
        TypeSphere,
        TypeBox,
        TypeExtern,
        TypeTranslate,
        TypeRotate,
        TypeScale,
        TypeInvert,
        TypeColor,
        TypeStroke,
        TypeBrightness,
        TypeContrast,
        TypeSaturation,
        TypeBlur,
        TypeLens,
        TypeWave
	};
    struct TransformParam {
        const CSSpriteObject* parent;
        CSMatrix transform;
        float progress;
        float duration;
        int random;
    };
    struct TransformUpdatedParam {
        const CSSpriteObject* parent;
        float progress0;
        float progress1;
        float duration;
        uint inflags;
    };
    struct DrawParam {
        CSGraphics* graphics;
        CSInstanceLayer layer;
        const CSSpriteObject* parent;
        float progress;
        float duration;
        int random;
    };

protected:	
    CSSpriteElement() = default;
    virtual ~CSSpriteElement() = default;
public:
    static CSSpriteElement* createWithBuffer(CSBuffer* buffer);
    static inline CSSpriteElement* elementWithBufer(CSBuffer* buffer) {
        return autorelease(createWithBuffer(buffer));
    }
    static CSSpriteElement* createWithElement(const CSSpriteElement* other);
    static inline CSSpriteElement* elementWithElement(const CSSpriteElement* other) {
        return autorelease(createWithElement(other));
    }

    virtual Type type() const = 0;
    virtual int resourceCost() const = 0;

    inline virtual void preload() const {
    
    }
    inline virtual bool addAABB(TransformParam& param, CSABoundingBox& result) const {
        return false;
    }
    inline virtual void addCollider(TransformParam& param, CSCollider*& result) const {
        
    }
    inline virtual bool getTransform(TransformParam& param, const string& name, CSMatrix& result) const {
        return false;
    }
    inline virtual void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const {
    
    }
    inline virtual uint showFlags() const {
        return 0;
    }
    virtual void draw(DrawParam& param) const = 0;
};

#endif
