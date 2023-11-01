#ifndef __CDK__CSScrollPane__
#define __CDK__CSScrollPane__

#include "CSLayer.h"

#include "CSScroll.h"

class CSScrollPane : public CSLayer, public CSScrollParent {
public:
    CSHandler<CSLayer*, CSGraphics*> OnDrawContent;
    CSScroll scroll;
private:
    bool _focusScroll = true;
    bool _lockScroll = false;
public:
    CSScrollPane();
protected:
    virtual ~CSScrollPane() = default;
public:
    static inline CSScrollPane* scrollPane() {
        return autorelease(new CSScrollPane());
    }
    inline CSVector2 scrollContentSize() const override {
        return contentSize();
    }
    inline CSVector2 scrollClipSize() const override {
        return frame().size();
    }
    inline void setFocusScroll(bool focusScroll) {
        _focusScroll = focusScroll;
    }
    inline bool focusScroll() const {
        return _focusScroll;
    }
    inline void setLockScroll(bool lockScroll) {
        _lockScroll = lockScroll;
    }
    inline bool lockScroll() const {
        return _lockScroll;
    }
    void focusLayer(CSLayer* layer, float duration = 0) override;

    virtual bool touchContains(const CSVector2& p) const override;
    virtual CSLayerTouchReturn layersOnTouch(const CSTouch* touch, CSArray<CSLayer>* targetLayers, bool began) override;

    virtual bool timeout(float delta, CSLayerVisible visible) override;
    virtual void draw(CSGraphics* graphics) override;
protected:
    virtual void onDrawContent(CSGraphics* graphics);
    virtual void onTouchesMoved() override;
public:
    virtual void convertToLocalSpace(CSVector2& point) const override;
    virtual void convertToParentSpace(CSVector2& point, const CSLayer* layer) const override;
    virtual void convertToParentSpace(CSVector2& point, int key, bool masking = false) const override;
    virtual void convertToViewSpace(CSVector2& point) const override;
};

#endif
