#define CDK_IMPL

#include "CSPopupCoverLayer.h"

#include "CSApplication.h"

CSPopupCoverLayer::CSPopupCoverLayer(CSLayer* contentLayer, float darkness, float blur) {
    CSAssert(darkness >= 0 && darkness <= 1 && blur >= 0, "invalid operation");
    _contentLayer = retain(contentLayer);
    _darkness = darkness;
    _blur = blur;
    setOrder(contentLayer->order());
    
    _contentLayer->OnStateChanged.add(this, &CSPopupCoverLayer::onStateChangedContent);
}

CSPopupCoverLayer::~CSPopupCoverLayer() {
    _contentLayer->OnStateChanged.remove(this);
    _contentLayer->release();
}

void CSPopupCoverLayer::onStateChanged() {
    CSLayerState state = this->state();

    if (state == CSLayerStateAttach) {
        CSApplication* app = CSApplication::sharedApplication();
        setFrame(CSRect(0, 0, app->projectionWidth(), app->projectionHeight()));
        app->insertLayer(app->layers()->indexOfObjectIdenticalTo(this) + 1, _contentLayer);
    }
    else if (state == CSLayerStateDetach) {
        _contentLayer->removeFromParent(true);
    }
    else if (state == CSLayerStateRemoved) {
        _contentLayer->removeFromParent(false);
    }
}

void CSPopupCoverLayer::onProjectionChanged() {
    CSApplication* app = CSApplication::sharedApplication();
    setFrame(CSRect(0, 0, app->projectionWidth(), app->projectionHeight()));
}

void CSPopupCoverLayer::onStateChangedContent(CSLayer* layer) {
    if (layer->state() == CSLayerStateRemoved) removeFromParent(false);
}

void CSPopupCoverLayer::onDraw(CSGraphics* graphics) {
    if (_blur) {
        graphics->blur(bounds(), _blur * _contentLayer->transitionProgress());
    }
    if (_darkness) {
        graphics->setColor(CSColor(0.0f, 0.0f, 0.0f, _darkness * _contentLayer->transitionProgress()));
        graphics->drawRect(bounds(), true);
    }
}
