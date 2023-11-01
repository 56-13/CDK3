#ifndef __CDK__CSButton__
#define __CDK__CSButton__

#include "CSLayer.h"

enum CSButtonScale {
    CSButtonScaleLeft = 1,
    CSButtonScaleRight = 2,
    CSButtonScaleTop = 4,
    CSButtonScaleBottom = 8,
    CSButtonScaleAll = 15
};

class CSButton : public CSLayer {
protected:
    uint _scaleMask = CSButtonScaleAll;
    float _scaleDegree = 0.04f;
    float _scaleDuration = 0.25f;
    float _scaleProgress = 0;
    float _scaleMaxProgress = 0;
public:
    CSButton() = default;
protected:
    virtual ~CSButton() = default;
public:
    static inline CSButton* button() {
        return autorelease(new CSButton());
    }
    
    inline uint scaleMask() const {
        return _scaleMask;
    }
    inline void setScaleMask(uint mask) {
        _scaleMask = mask;
    }
    inline float scaleDegree() const {
        return _scaleDegree;
    }
    inline void setScaleDegree(float degree) {
        _scaleDegree = degree;
    }
    inline float scaleDuration() const {
        return _scaleDuration;
    }
    inline void setScaleDuration(float duration) {
        _scaleDuration = duration;
    }

    virtual bool timeout(float delta, CSLayerVisible visible) override;
    virtual void draw(CSGraphics* graphics) override;
protected:
    virtual void onTouchesBegan() override;
    virtual void onTouchesEnded() override;
    virtual void onTouchesCancelled() override;
};

#endif
