#ifndef __CDK__CSLayer__
#define __CDK__CSLayer__

#include "CSValue.h"
#include "CSString.h"
#include "CSHandler.h"
#include "CSTouch.h"
#include "CSKeyCode.h"
#include "CSGraphics.h"

enum CSLayerState : byte  {
    CSLayerStateRemoved,
    CSLayerStateDetach,
    CSLayerStateAttach,
    CSLayerStateHidden,
    CSLayerStateFocus
};

enum CSLayerLayout : byte {
    CSLayerLayoutNone,
    CSLayerLayoutHorizontal,
    CSLayerLayoutVertical,
    CSLayerLayoutFlow
};

enum CSLayerKeyboardScroll : byte {
    CSLayerKeyboardScrollNone,
    CSLayerKeyboardScrollUp,
    CSLayerKeyboardScrollFit
};

enum CSLayerVisible {
    CSLayerVisibleCovered,
    CSLayerVisibleBackground,
    CSLayerVisibleForward
};

enum CSLayerTouchReturn {
    CSLayerTouchReturnNone,
    CSLayerTouchReturnCarry,
    CSLayerTouchReturnCatch
};

enum CSLayerTouchArea : byte {
	CSLayerTouchAreaFrame,
	CSLayerTouchAreaRange
};

enum CSLayerTransition : uint {
    CSLayerTransitionLeft = 0x00000001,
    CSLayerTransitionRight = 0x00000002,
    CSLayerTransitionUp = 0x00000004,
    CSLayerTransitionDown = 0x00000008,
    CSLayerTransitionLeftBounce = 0x00000010,
    CSLayerTransitionRightBounce = 0x00000020,
    CSLayerTransitionUpBounce = 0x00000040,
    CSLayerTransitionDownBounce = 0x00000080,
    CSLayerTransitionHorizontalFlip = 0x00000100,
    CSLayerTransitionVerticalFlip = 0x00000200,
    CSLayerTransitionFade = 0x00000400,
    CSLayerTransitionHorizontalZoom = 0x00000800,
    CSLayerTransitionVerticalZoom = 0x000001000,
    CSLayerTransitionZoom = (0x00000800 | 0x00001000),
    CSLayerTransitionHorizontalFall = 0x00002000,
    CSLayerTransitionVerticalFall = 0x00004000,
    CSLayerTransitionFall = (0x00002000 | 0x00004000),
    CSLayerTransitionHorizontalPop = 0x0008000,
    CSLayerTransitionVerticalPop = 0x00010000,
    CSLayerTransitionPop = (0x00008000 | 0x00010000),
	CSLayerTransitionCustom = 0x80000000
};

constexpr uint CSLayerTransitionMoveMask = 
    CSLayerTransitionLeft |
    CSLayerTransitionRight |
    CSLayerTransitionUp |
    CSLayerTransitionDown |
    CSLayerTransitionLeftBounce |
    CSLayerTransitionRightBounce |
    CSLayerTransitionUpBounce |
    CSLayerTransitionDownBounce |
    CSLayerTransitionHorizontalFlip |
    CSLayerTransitionVerticalFlip |
    CSLayerTransitionZoom |
    CSLayerTransitionFall |
    CSLayerTransitionPop;

class CSLayer : public CSObject {
public:
    CSHandler<CSLayer*, CSGraphics*> OnDraw;
    CSHandler<CSLayer*, CSGraphics*> OnDrawCover;
	CSHandler<CSLayer*, float> OnTimeout;
	CSHandler<CSLayer*, const CSArray<CSTouch>*, bool&> OnTouchesView;
	CSHandler<CSLayer*> OnTouchesBegan;
	CSHandler<CSLayer*> OnTouchesEnded;
	CSHandler<CSLayer*> OnTouchesMoved;
	CSHandler<CSLayer*> OnTouchesCancelled;
	CSHandler<CSLayer*, float> OnWheel;
	CSHandler<CSLayer*, int> OnKeyDown;
	CSHandler<CSLayer*, int> OnKeyUp;
	CSHandler<CSLayer*> OnStateChanged;
	CSHandler<CSLayer*> OnFrameChanged;
	CSHandler<CSLayer*> OnProjectionChanged;
	CSHandler<CSLayer*> OnLink;
	CSHandler<CSLayer*> OnUnlink;
	CSHandler<CSLayer*> OnPause;
	CSHandler<CSLayer*> OnResume;
	CSHandler<CSLayer*> OnBackKey;
	CSHandler<CSLayer*, int, void*, bool&> OnCustomEvent;
private:
    CSLayer* _parent = NULL;
    CSArray<CSLayer, 1, false>* _layers = NULL;
    CSArray<const CSTouch>* _touches = NULL;
    int _key = 0;
    uint _transition = 0;
    float _transitionDuration = 0.25f;
    float _transitionLatency = 0;
    float _transitionAccelation = 1;
    float _transitionProgress = 0;
    float _transitionWeight = 1;
    float _timeoutCounter = 0;
    float _timeoutInterval = 0;
    uint _timeoutSequence = 0;
    CSLayerState _state = CSLayerStateRemoved;
    bool _linked = false;
    bool _enabled = true;
    bool _covered = false;
    bool _touchInherit = false;
    bool _touchCarry = true;
    bool _touchIn = false;
    bool _touchOut = false;
    bool _touchMulti = false;
    bool _touchRefresh = false;
	CSLayerTouchArea _touchArea = CSLayerTouchAreaFrame;
    CSLayerLayout _layout = CSLayerLayoutNone;
    CSLayerKeyboardScroll _keyboardScroll = CSLayerKeyboardScrollNone;
    float _keyboardHeight = 0;
    CSRect _frame = CSRect::Zero;
    CSRect _prevFrame = CSRect::Zero;
    CSRect _nextFrame = CSRect::Zero;
    float _frameCounter = 0;
    float _frameInterval = 0;
    float _spacing = 0;
    float _contentWidth = 0;
    float _contentHeight = 0;
    int _order = 0;
    CSColor _color = CSColor::White;
    const CSObject* _tag = NULL;
	const CSObject* _subtag = NULL;
    CSArray<CSLayer>* _links = NULL;
public:
    CSLayer() = default;
protected:
    virtual ~CSLayer();
public:
    static inline CSLayer* layer() {
        return autorelease(new CSLayer());
    }
protected:
    friend class CSApplication;
    
    virtual void submitLayout();
    virtual void submitFrame(const CSRect& frame, bool fromParent);
    virtual bool attach(CSLayer* parent);
    virtual void detach(CSLayer* layer);
    
    virtual void onStateChanged();
    virtual void onTimeout(float delta);
    virtual void onDraw(CSGraphics* graphics);
    virtual void onDrawCover(CSGraphics* graphics);
    virtual void onTouchesView(const CSArray<CSTouch>* touches, bool& interrupt);
    virtual void onTouchesBegan();
    virtual void onTouchesMoved();
    virtual void onTouchesEnded();
    virtual void onTouchesCancelled();
	virtual void onWheel(float offset);
	virtual void onKeyDown(int keyCode);
	virtual void onKeyUp(int keyCode);
    virtual void onLink();
    virtual void onUnlink();
    virtual void onFrameChanged();
    virtual void onProjectionChanged();
    virtual void onPause();
    virtual void onResume();
    virtual void onBackKey();
    virtual void onCustomEvent(int op, void* param, bool& interrupt);
    virtual float keyboardScrollDegree(float bottom) const;
	virtual bool touchContains(const CSVector2& p) const;
private:
    void validateLinks();
    bool clearLinks();
    
    void notifyLink();
    void notifyUnlink();
    void notifyFrameChanged();
    void notifyProjectionChanged();
    void notifyTouchesView(const CSArray<CSTouch>* touches, bool& interrupt);
	void notifyPause();
    void notifyResume();
public:
    bool customEvent(int op, void* param);
    
    void commitTouchesBegan();
    bool commitTouchesMoved();
    void commitTouchesEnded();
    void commitTouchesCancelled();

    void beginTouch(const CSTouch* touch, bool event = true);
    void addTouch(const CSTouch* touch);
    void cancelTouches(bool event = true);
    void cancelChildrenTouches(bool event = true);
    
	virtual bool timeout(float delta, CSLayerVisible visible);
    
    inline uint timeoutSequence() const {
        return _timeoutSequence;
    }
    inline void resetTimeoutSequence() {
        _timeoutSequence = 0;
    }
    inline bool touchRefresh() const {
        return _touchRefresh;
    }
    inline void setTouchRefresh(bool touchRefresh) {
        _touchRefresh = touchRefresh;
    }
	void clip(CSGraphics* graphics);
protected:
    void commitDraw(CSGraphics* graphics);
	void commitDrawCover(CSGraphics* graphics);

    void drawHandler(CSGraphics* graphics, const CSHandler<CSLayer*, CSGraphics*>& handler);
public:
	virtual void draw(CSGraphics* graphics);
    
    virtual CSLayerTouchReturn layersOnTouch(const CSTouch* touch, CSArray<CSLayer>* targetLayers, bool began);
    
    bool transitionForward(CSGraphics* graphics);
    bool transitionBackward(CSGraphics* graphics);
    
    void removeFromParent(bool transition);
    void addToParent();
    
    inline const CSArray<CSLayer, 1, false>* layers() const {
        return _layers;
    }
    int layerCount() const;
    
    void refresh();

    void clearLayers(bool transition);
    virtual bool addLayer(CSLayer* layer);
    virtual bool insertLayer(int index, CSLayer* layer);
    virtual inline void focusLayer(CSLayer* layer, float duration = 0) {}
    void clearLayers(int key, bool transition, bool inclusive, bool masking = false);
    CSArray<CSLayer, 1, false>* findLayers(int key, bool masking = false);
    void findLayers(int key, CSArray<CSLayer, 1, false>* outArray, bool masking = false);
    CSLayer* findLayer(int key, bool mask = false);
    CSLayer* findParent(int key, bool mask = false);
    virtual void convertToLocalSpace(CSVector2& point) const;
    virtual void convertToParentSpace(CSVector2& point, const CSLayer* parent) const;
    virtual void convertToParentSpace(CSVector2& point, int key, bool masking = false) const;
    virtual void convertToViewSpace(CSVector2& point) const;
    
    inline bool enabled() const {
        return _enabled;
    }
    inline virtual void setEnabled(bool enabled) {
        if (!enabled) {
            cancelTouches();
            cancelChildrenTouches();
        }
        _enabled = enabled;
    }
    inline int key() const {
        return _key;
    }
    inline void setKey(int key) {
        _key = key;
    }
    inline CSLayer* parent() const {
        return _parent;
    }
    inline CSLayerState state() const {
        return _state;
    }
    inline bool linked() const {
        return _linked;
    }
    inline const CSArray<const CSTouch>* touches() const {
        return _touches;
    }
    inline const CSTouch* touch() const {
        return _touches->lastObject();
    }
    inline bool isTouching() const {
        return _touches->count() > 0;
    }
    inline void setTouchInherit(bool touchInherit) {
        _touchInherit = touchInherit;
    }
    inline bool touchInherit() const {
        return _touchInherit;
    }
    inline void setTouchCarry(bool touchCarry) {
        _touchCarry = touchCarry;
    }
    inline bool touchCarry() const {
        return _touchCarry;
    }
    inline void setTouchIn(bool touchIn) {
        _touchIn = touchIn;
    }
    inline bool touchIn() const {
        return _touchIn;
    }
    inline void setTouchOut(bool touchOut) {
        _touchOut = touchOut;
    }
    inline bool touchOut() const {
        return _touchOut;
    }
    inline void setTouchMulti(bool multiTouch) {
        _touchMulti = multiTouch;
    }
    inline bool touchMulti() const {
        return _touchMulti;
    }
	inline void setTouchArea(CSLayerTouchArea touchArea) {
		_touchArea = touchArea;
	}
	inline CSLayerTouchArea touchArea() const {
		return _touchArea;
	}
    inline void setTransition(uint transition) {
        _transition = transition;
    }
    inline uint transition() const {
        return _transition;
    }
    inline void setTransitionDuration(float transitionDuration) {
        _transitionDuration = transitionDuration;
    }
    inline float transitionDuration() const {
        return _transitionDuration;
    }
    inline void setTransitionAccelation(float transitionAccelation) {
        _transitionAccelation = transitionAccelation;
    }
    inline float transitionAccelation() const {
        return _transitionAccelation;
    }
    inline void setTransitionLatency(float transitionLatency) {
        _transitionLatency = transitionLatency;
    }
    inline float transitionLatency() const {
        return _transitionLatency;
    }
    inline float transitionProgress() const {
        return _transitionProgress;
    }
    inline float transitionWeight() const {
        return _transitionWeight;
    }
    inline void setTransitionWeight(float transitionWeight) {
        _transitionWeight = transitionWeight;
    }
    inline void setTimeoutInterval(float timeoutInterval) {
        _timeoutInterval = timeoutInterval;
    }
    inline float timeoutInterval() const {
        return _timeoutInterval;
    }
    inline void setCovered(bool covered) {
        _covered = covered;
    }
    inline bool covered() const {
        return _covered;
    }
    inline void setOrder(int order) {
        _order = order;
    }
    inline int order() const {
        return _order;
    }
    void setFrame(const CSRect& frame);
    void setFrame(const CSRect& frame, float interval);
    inline const CSRect& frame() const {
        return _frame;
    }
    inline const CSRect& nextFrame() const {
        return _nextFrame;
    }
    inline const CSRect& prevFrame() const {
        return _prevFrame;
    }
    inline CSRect bounds() const {
        return CSRect(0, 0, _frame.width, _frame.height);
    }
    inline const CSVector2& origin() const {
        return _frame.origin();
    }
    inline const CSVector2& size() const {
        return _frame.size();
    }
    inline float width() const {
        return _frame.width;
    }
    inline float height() const {
        return _frame.height;
    }
    inline float center() const {
        return _frame.center();
    }
    inline float middle() const {
        return _frame.middle();
    }
    inline CSVector2 centerMiddle() const {
        return _frame.centerMiddle();
    }

    float contentWidth() const;
    float contentHeight() const;
    
    inline void setContentWidth(float contentWidth) {
        _contentWidth = contentWidth;
    }
    inline void setContentHeight(float contentHeight) {
        _contentHeight = contentHeight;
    }
    inline CSVector2 contentSize() const {
        return CSVector2(contentWidth(), contentHeight());
    }
    
    inline void setSpacing(float spacing) {
        _spacing = spacing;
    }
    inline float spacing() const {
        return _spacing;
    }
    inline void setLayout(CSLayerLayout layout) {
        _layout = layout;
    }
    inline CSLayerLayout layout() const {
        return _layout;
    }
    inline void setKeyboardScroll(CSLayerKeyboardScroll keyboardScroll) {
        _keyboardScroll = keyboardScroll;
    }
    inline CSLayerKeyboardScroll keyboardScroll() const {
        return _keyboardScroll;
    }
	inline void clearTag() {
        release(_tag);
    }
    template <class T>
    inline void setTag(T* tag) {
        retain(_tag, tag);
    }
    template <class T>
    inline T* tag(bool assertType = true) const {
        return assertType ? static_assert_cast<T*>(const_cast<CSObject*>(_tag)) : dynamic_cast<T*>(const_cast<CSObject*>(_tag));
    }
    inline void setTagAsInt(int tag) {
        setTag(CSInteger::value(tag));
    }
    inline int tagAsInt() const {
        CSInteger* i = tag<CSInteger>();
        return i ? i->value() : 0;
    }
	inline void clearSubtag() {
		release(_subtag);
	}
	template <class T>
	inline void setSubtag(T* subtag) {
        retain(_subtag, subtag);
	}
	template <class T>
	inline T* subtag(bool assertType = true) const {
        return assertType ? static_assert_cast<T*>(const_cast<CSObject*>(_subtag)) : dynamic_cast<T*>(const_cast<CSObject*>(_subtag));
	}
	inline void setSubtagAsInt(int subtag) {
		setSubtag(CSInteger::value(subtag));
	}
	inline int subtagAsInt() const {
		CSInteger* i = subtag<CSInteger>();
		return i ? i->value() : 0;
	}
    inline void setColor(const CSColor& color) {
        if (_color != color) {
            _color = color;
            refresh();
        }
    }
    inline const CSColor& color() const {
        return _color;
    }
    
    void setOwner(CSLayer* owner);
};

#endif
