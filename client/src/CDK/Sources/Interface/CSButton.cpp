#define CDK_IMPL

#include "CSButton.h"

#include "CSApplication.h"

void CSButton::onTouchesBegan() {
    _scaleMaxProgress = 0.25f;
    while (_scaleProgress > _scaleMaxProgress) _scaleMaxProgress += 1;
    CSLayer::onTouchesBegan();
}

void CSButton::onTouchesEnded() {
    _scaleMaxProgress = 1;
    while (_scaleProgress + 0.5f > _scaleMaxProgress) _scaleMaxProgress += 1;
    CSLayer::onTouchesEnded();
}

void CSButton::onTouchesCancelled() {
    _scaleMaxProgress = 1;
    while (_scaleProgress + 0.5f > _scaleMaxProgress) _scaleMaxProgress += 1;
    CSLayer::onTouchesCancelled();
}

bool CSButton::timeout(float delta, CSLayerVisible visible) {
    if (_scaleProgress != _scaleMaxProgress) {
        _scaleProgress = CSMath::min(_scaleProgress + delta / _scaleDuration, _scaleMaxProgress);
        refresh();
    }
    else if (!isTouching()) {
        _scaleProgress = 0;
        _scaleMaxProgress = 0;
    }
    return CSLayer::timeout(delta, visible);
}

void CSButton::draw(CSGraphics* graphics) {
    graphics->push();
    graphics->translate(origin());
    
    if (transitionForward(graphics)) {
        if (_scaleProgress) {
            CSVector2 centerMiddle = this->centerMiddle();
            CSVector3 translation = centerMiddle;
            CSVector3 scale = CSVector3::One;
            
            float scaleDegree = -_scaleDegree * CSMath::sin(_scaleProgress * FloatTwoPi);
            
            if (_scaleMask & CSButtonScaleLeft) {
                translation.x += centerMiddle.x;
                scale.x += scaleDegree;
            }
            if (_scaleMask & CSButtonScaleRight) {
                translation.x -= centerMiddle.x;
                scale.x += scaleDegree;
            }
            if (_scaleMask & CSButtonScaleTop) {
                translation.y += centerMiddle.y;
                scale.y += scaleDegree;
            }
            if (_scaleMask & CSButtonScaleBottom) {
                translation.y -= centerMiddle.y;
                scale.y += scaleDegree;
            }
            graphics->translate(translation);
            graphics->scale(scale);
            graphics->translate(-translation);
        }
        
		graphics->push();
		
		commitDraw(graphics);
		graphics->reset();
        
        const CSArray<CSLayer, 1, false>* layers = this->layers();

        if (layers && layers->count()) {
            int i;
            int len = layers->count();
            
            for (i = len - 1; i > 0 && !layers->objectAtIndex(i)->covered(); i--);
            
            if (i > 0 && layers->objectAtIndex(i)->transitionBackward(graphics)) {
                int j;
                for (j = i - 1; j > 0 && !layers->objectAtIndex(j)->covered(); j--);
                for (; j < i; j++) {
                    layers->objectAtIndex(j)->draw(graphics);
                }
                graphics->reset();
            }
            for (; i < len; i++) {
                layers->objectAtIndex(i)->draw(graphics);
            }
            graphics->reset();
        }
        
		commitDrawCover(graphics);

		graphics->pop();
        
#ifdef CS_DISPLAY_LAYER_FRAME
        if (CSApplication::sharedApplication()->displayLayerFrame) {
            graphics->setColor(CSColor::Magenta);
            graphics->drawRect(bounds(), false);
        }
#endif
    }
    graphics->pop();
}

