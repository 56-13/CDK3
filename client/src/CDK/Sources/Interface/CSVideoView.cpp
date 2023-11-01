#define CDK_IMPL

#include "CSVideoView.h"

#include "CSVideoViewBridge.h"

#include "CSApplication.h"

CSVideoView::CSVideoView() {
    _handle = CSVideoViewBridge::createHandle(this);
}

CSVideoView::~CSVideoView() {
    CSVideoViewBridge::destroyHandle(_handle);
}

void CSVideoView::onStateChanged() {
    switch (state()) {
        case CSLayerStateFocus:
            applyFrame();
            CSVideoViewBridge::addToView(_handle);
            break;
        case CSLayerStateHidden:
        case CSLayerStateDetach:
            CSVideoViewBridge::removeFromView(_handle);
            break;
        default:
            break;
    }
    CSLayer::onStateChanged();
}

void CSVideoView::onFrameChanged() {
    applyFrame();
    
    CSLayer::onFrameChanged();
}

void CSVideoView::onProjectionChanged() {
    applyFrame();
    
    CSLayer::onProjectionChanged();
}

void CSVideoView::playStart(const char* path) {
    CSVideoViewBridge::playStart(_handle, path);
}


void CSVideoView::onPlayFinish() {
    OnPlayFinish(this);
}

void CSVideoView::applyFrame() {
    if (linked()) {
        CSRect frame = bounds();
        convertToViewSpace(frame.origin());
        CSApplication::sharedApplication()->convertToUIFrame(frame);
        CSVideoViewBridge::setFrame(_handle, frame);
    }
}
