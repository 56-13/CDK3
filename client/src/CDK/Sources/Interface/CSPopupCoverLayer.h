#ifdef CDK_IMPL

#ifndef __CDK__CSPopupCoverLayer__
#define __CDK__CSPopupCoverLayer__

#include "CSLayer.h"

class CSPopupCoverLayer : public CSLayer {
private:
    CSLayer* _contentLayer;
    float _darkness;
    float _blur;
public:
    CSPopupCoverLayer(CSLayer* contentLayer, float darkness, float blur);
private:
    ~CSPopupCoverLayer();
public:
    static inline CSPopupCoverLayer* layer(CSLayer* contentLayer, float darkness, float blur) {
        return autorelease(new CSPopupCoverLayer(contentLayer, darkness, blur));
    }
private:
    void onProjectionChanged() override;
    void onStateChanged() override;
    void onDraw(CSGraphics* graphics) override;

	void onStateChangedContent(CSLayer* layer);
};

#endif

#endif
