#define CDK_IMPL

#include "CSLayer.h"

#include "CSTime.h"

#include "CSLayerTransition.h"
#include "CSApplication.h"

CSLayer::~CSLayer() {
    if (_layers) {
        foreach (CSLayer*, layer, _layers) layer->_parent = NULL;
        _layers->release();
    }
    release(_touches);
    release(_tag);
	release(_subtag);
    if (_links) {
        foreach (CSLayer*, link, _links) link->removeFromParent(false);
        _links->release();
    }
}

void CSLayer::setFrame(const CSRect& frame) {
    _nextFrame = frame;
    _prevFrame = frame;
    _frameInterval = 0;
	if (frame != _frame) submitFrame(frame, false);
}

void CSLayer::setFrame(const CSRect& frame, float interval) {
	if (interval > 0) {
		if (frame != _nextFrame) {
			_prevFrame = _frame;
			_nextFrame = frame;
			_frameCounter = 0;
			_frameInterval = interval;
		}
	}
	else setFrame(frame);
}

float CSLayer::contentWidth() const {
    float max = _contentWidth;
    
    foreach (CSLayer *, layer, _layers) {
        float right = layer->_frame.right();
        if (max < right) max = right;
    }
    return max;
}

float CSLayer::contentHeight() const {
    float max = _contentHeight;
    
    foreach (CSLayer *, layer, _layers) {
        float bottom = layer->_frame.bottom();
        if (max < bottom) max = bottom;
    }
    return max;
}

void CSLayer::submitFrame(const CSRect& frame, bool fromParent) {
    _frame = frame;
    notifyFrameChanged();
    refresh();
    
    submitLayout();

    if (!fromParent && _parent) _parent->submitLayout();
}

void CSLayer::onStateChanged() {
    OnStateChanged(this);
}

bool CSLayer::attach(CSLayer* parent) {
    if (_parent == parent) {
        if (_state == CSLayerStateDetach || _state == CSLayerStateRemoved) {
            _state = CSLayerStateAttach;
            onStateChanged();
        }
    }
    else if (!_parent && !_linked) {
        _timeoutCounter = 0;
        _transitionProgress = _transition ? 0 : 1;
        _state = CSLayerStateAttach;
        _parent = parent;
        return true;
    }
    return false;
}

void CSLayer::detach(CSLayer* layer) {

}

void CSLayer::onTimeout(float delta) {
    OnTimeout(this, delta);
}

void CSLayer::beginTouch(const CSTouch* touch, bool event) {
    if (_linked && CSApplication::sharedApplication()->beginTouch(this, touch)) {
        addTouch(touch);
        if (event) commitTouchesBegan();
    }
}

void CSLayer::addTouch(const CSTouch* touch) {
    if (!_touches) _touches = new CSArray<const CSTouch>(4);
    _touches->addObject(touch);
}

void CSLayer::cancelTouches(bool event) {
    if (_touches && _touches->count()) {
        if (_linked) CSApplication::sharedApplication()->cancelTouches(this);
        if (event) commitTouchesCancelled();
        else _touches->removeAllObjects();
    }
}

void CSLayer::cancelChildrenTouches(bool event) {
    foreach (CSLayer *, layer, _layers) {
        layer->cancelTouches(event);
        layer->cancelChildrenTouches(event);
    }
}

bool CSLayer::timeout(float delta, CSLayerVisible visible) {
    _timeoutCounter += delta;

    switch (_state) {
        case CSLayerStateHidden:
        case CSLayerStateFocus:
            if (visible == CSLayerVisibleForward) {
                if (_state == CSLayerStateHidden) {
                    _state = CSLayerStateFocus;
                    onStateChanged();
                }
            }
            else if (_state == CSLayerStateFocus) {
                cancelTouches();
                cancelChildrenTouches();
                _state = CSLayerStateHidden;
                onStateChanged();
            }
            
            if (visible) {
                bool loop = true;

                while (loop && _timeoutCounter >= _timeoutInterval) {
                    _timeoutSequence++;
                        
                    if (_timeoutInterval) {
                        _timeoutCounter -= _timeoutInterval;
                        onTimeout(_timeoutInterval);
                    }
                    else {
                        _timeoutCounter = 0;
                        onTimeout(delta);
                        loop = false;
                    }
                    switch (_state) {
                        case CSLayerStateDetach:
                            _timeoutCounter = 0;
                            visible = CSLayerVisibleBackground;
                            loop = false;
                            break;
                        case CSLayerStateRemoved:
                            _timeoutCounter = 0;
                            visible = CSLayerVisibleCovered;
                            loop = false;
                            break;
                    }
                }
            }
            break;
        case CSLayerStateDetach:
            if (visible) {
                if (_timeoutCounter >= _transitionLatency + _transitionDuration) {
                    _timeoutCounter = 0;
                    _transitionProgress = 0;
                    _state = CSLayerStateRemoved;
                    onStateChanged();
                    
                    visible = CSLayerVisibleCovered;
                }
                else {
                    _transitionProgress = CSMath::pow(CSMath::min(1.0f - (_timeoutCounter - _transitionLatency) / _transitionDuration, 1.0f), _transitionAccelation);
                    
                    visible = CSLayerVisibleBackground;
                }
                refresh();
            }
            else {
                _timeoutCounter = 0;
                _transitionProgress = 0;
                _state = CSLayerStateRemoved;
                onStateChanged();
            }
            break;
        case CSLayerStateAttach:
            if (visible) {
                if (_timeoutCounter >= _transitionLatency + _transitionDuration) {
                    _timeoutCounter = 0;
                    _transitionProgress = 1;
                    _state = visible == CSLayerVisibleForward ? CSLayerStateFocus : CSLayerStateHidden;
                    onStateChanged();
                }
                else {
                    _transitionProgress = CSMath::pow(CSMath::max((_timeoutCounter - _transitionLatency) / _transitionDuration, 0.0f), _transitionAccelation);

                    visible = CSLayerVisibleBackground;
                }
                refresh();
            }
            else {
                _timeoutCounter = 0;
                _transitionProgress = 1;
                _state = CSLayerStateHidden;
                onStateChanged();
            }
            break;
        case CSLayerStateRemoved:
            _timeoutCounter = 0;
            visible = CSLayerVisibleCovered;
            break;
    }
    
    bool performLayout = false;
    
    if (_layers) {
        int i = _layers->count() - 1;
        while (i >= 0) {
            CSLayer* layer = _layers->objectAtIndex(i);
            bool end = layer->timeout(delta, visible);
            i = _layers->indexOfObjectIdenticalTo(layer);

            if (end) {
                detach(layer);
                _layers->removeObjectAtIndex(i);
                performLayout = true;
            }
            else if (layer->covered()) {
                visible = CSLayerVisibleCovered;
            }
            i--;
        }
    }
    
    if (_frameInterval) {
        _frameCounter += delta;

        if (visible) {
            float progress = _frameCounter / _frameInterval;
            
            if (progress < 1) {
                CSRect frame;
                frame.x = CSMath::lerp(_prevFrame.x, _nextFrame.x, progress);
                frame.y = CSMath::lerp(_prevFrame.y, _nextFrame.y, progress);
                frame.width = CSMath::lerp(_prevFrame.width, _nextFrame.width, progress);
                frame.height = CSMath::lerp(_prevFrame.height, _nextFrame.height, progress);
                submitFrame(frame, false);
                performLayout = false;
            }
            else {
                submitFrame(_nextFrame, false);
                performLayout = false;
                _frameInterval = 0;
            }
        }
        else {
            submitFrame(_nextFrame, false);
            performLayout = false;
            _frameInterval = 0;
        }
    }
    else if (_keyboardScroll) {
        float keyboardHeight = CSApplication::sharedApplication()->keyboardHeight();
        
        if (keyboardHeight != _keyboardHeight) {
			CSRect frame;

			if (keyboardHeight) {
				float degree = keyboardScrollDegree(CSApplication::sharedApplication()->projectionHeight() - keyboardHeight);

				frame = _frame;
				frame.y -= degree;
			
				if (_keyboardScroll == CSLayerKeyboardScrollFit) {
					float miny = CSMath::min(_nextFrame.y, 0.0f);

					if (frame.y < miny) {
						frame.height -= miny - frame.y;
						frame.y = miny;
					}
				}
			}
			else frame = _nextFrame;

			if (frame != _frame) {
				submitFrame(frame, false);
				performLayout = false;
			}
			_keyboardHeight = keyboardHeight;
        }
    }

    if (performLayout) submitLayout();
    
    validateLinks();
    
    if (_state == CSLayerStateRemoved && clearLinks()) {
        notifyUnlink();
        _parent = NULL;
        return true;
    }
    return false;
}

bool CSLayer::transitionForward(CSGraphics* graphics) {
    switch (_state) {
        case CSLayerStateRemoved:
            return false;
        case CSLayerStateAttach:
        case CSLayerStateDetach:
            if (_transition) {
                for (int i = 0; i < LAYER_TRANSITION_COUNT; i++) {
                    if ((_transition >> i & 1) && !transitionMethods[i].forward(this, graphics)) return false;
                }
            }
            break;
    }
    return true;
}

bool CSLayer::transitionBackward(CSGraphics* graphics) {
    switch (_state) {
        case CSLayerStateAttach:
        case CSLayerStateDetach:
            if (_transition) {
                for (int i = 0; i < LAYER_TRANSITION_COUNT; i++) {
                    if ((_transition >> i & 1) && !transitionMethods[i].backward(this, graphics)) return false;
                }
                return true;
            }
            break;
    }
    return false;
}

void CSLayer::drawHandler(CSGraphics* graphics, const CSHandler<CSLayer*, CSGraphics*>& handler) {
    const CSArray<CSHandler<CSLayer*, CSGraphics*>::Callback>* callbacks = handler.callbacks();
    if (callbacks) {
        if (callbacks->count() == 1) {
            callbacks->objectAtIndex(0).func(this, graphics);
        }
        else if (callbacks->count() > 1) {
            for (int i = 0; i < callbacks->count(); i++) {
                callbacks->objectAtIndex(i).func(this, graphics);
                if (i + 1 < callbacks->count()) graphics->reset();
            }
        }
    }
}

void CSLayer::onDraw(CSGraphics* graphics) {
    drawHandler(graphics, OnDraw);
}

void CSLayer::onDrawCover(CSGraphics* graphics) {
    drawHandler(graphics, OnDrawCover);
}

void CSLayer::clip(CSGraphics* graphics) {
    CSLayer* current = this;
    bool transition = false;
    do {
        if ((current->transition() & CSLayerTransitionMoveMask) != 0) {
            float transitionProgress = current->transitionProgress();
            
            if (transitionProgress > 0 && transitionProgress < 1) {
                transition = true;
                break;
            }
        }
        current = current->parent();
    } while (current);
    
    if (transition) {
        graphics->setStencilMode(CSStencilInclusive);
        graphics->drawRect(bounds(), true);
        graphics->setStencilMode(CSStencilNone);
    }
    else {
        CSRect scissor = frame();
        if (_parent) _parent->convertToViewSpace(scissor.origin());
        graphics->setScissor(scissor);
    }
}

void CSLayer::commitDraw(CSGraphics* graphics) {
	if (_color != CSColor::White) {
		graphics->setColor(_color);
		graphics->pushColor();
		onDraw(graphics);
		graphics->popColor();
	}
	else {
		onDraw(graphics);
	}
}

void CSLayer::commitDrawCover(CSGraphics* graphics) {
	if (_color != CSColor::White) {
		graphics->setColor(_color);
		graphics->pushColor();
		onDrawCover(graphics);
		graphics->popColor();
	}
	else {
		onDrawCover(graphics);
	}
}

void CSLayer::draw(CSGraphics* graphics) {
    graphics->push();
    graphics->translate(_frame.origin());

	if (transitionForward(graphics)) {
		graphics->push();

		commitDraw(graphics);
		
		graphics->reset();

		if (_layers && _layers->count()) {
			int i;
			for (i = _layers->count() - 1; i > 0 && !_layers->objectAtIndex(i)->covered(); i--);
			if (i > 0) {
				if (_layers->objectAtIndex(i)->transitionBackward(graphics)) {
					int j;
					for (j = i - 1; j > 0 && !_layers->objectAtIndex(j)->covered(); j--);
					for (; j < i; j++) {
						_layers->objectAtIndex(j)->draw(graphics);
					}
				}
				graphics->reset();
			}
			for (; i < _layers->count(); i++) {
				_layers->objectAtIndex(i)->draw(graphics);
			}
		}

		commitDrawCover(graphics);

		graphics->pop();

#ifdef CS_DISPLAY_LAYER_FRAME
		if (CSApplication::sharedApplication()->displayLayerFrame) {
			graphics->setColor(CSColor::Magenta);
			switch (_touchArea) {
				case CSLayerTouchAreaFrame:
					graphics->drawRect(bounds(), false);
					break;
				case CSLayerTouchAreaRange:
					graphics->drawCircle(bounds(), false);
					break;
			}
        }
#endif
    }
    graphics->pop();
}

bool CSLayer::touchContains(const CSVector2& p) const {
	switch (_touchArea) {
		case CSLayerTouchAreaRange:
			{
				CSVector2 c = centerMiddle();
				CSVector2 d = _frame.origin() + c - p;
				return (d.x * d.x) / (c.x * c.x) + (d.y * d.y) / (c.y * c.y) <= 1;
			}
	}
	return _frame.contains(p);
}

CSLayerTouchReturn CSLayer::layersOnTouch(const CSTouch* touch, CSArray<CSLayer>* targetLayers, bool began) {
    if (_state == CSLayerStateFocus && _enabled) {
        bool touching = false;
        
        if (_layers) {
            for (int i = _layers->count() - 1; i >= 0; i--) {
                CSLayer* layer = _layers->objectAtIndex(i);

                CSLayerTouchReturn rtn = layer->layersOnTouch(touch, targetLayers, began);

                switch (rtn) {
                    case CSLayerTouchReturnCarry:
                        if (_touchInherit) {
                            touching = true;
                            goto check;
                        }
                        if (!_touchCarry) {
                            rtn = CSLayerTouchReturnCatch;
                        }
                    case CSLayerTouchReturnCatch:
                        return rtn;
                }
            }
        }
        
check:
        if (touchContains(touch->point(_parent))) {
            if (began || (_touchIn && (!_touches || !_touches->containsObject(touch)))) {
                addTouch(touch);
                targetLayers->addObject(this);
                touching = true;
            }
        }
        if (touching) return _touchCarry ? CSLayerTouchReturnCarry : CSLayerTouchReturnCatch;
    }
    return CSLayerTouchReturnNone;
}

void CSLayer::onTouchesBegan() {
    OnTouchesBegan(this);
}

bool CSLayer::customEvent(int op, void* param) {
    if (_layers) {
        for (int i = _layers->count() - 1; i >= 0; i--) {
            CSLayer* layer = _layers->objectAtIndex(i);

            if (layer->customEvent(op, param)) return true;
        }
    }
    bool interrupt = false;
    onCustomEvent(op, param, interrupt);
    return interrupt;
}

void CSLayer::commitTouchesBegan() {
    if (_touches && _touches->count()) {
        onTouchesBegan();
        
        if (_touchRefresh) refresh();
    }
}

void CSLayer::onTouchesMoved() {
    OnTouchesMoved(this);
}

bool CSLayer::commitTouchesMoved() {
    if (_touches && _touches->count()) {
        if (!_touchOut) {
            foreach (const CSTouch *, touch, _touches) {
                if (!touchContains(touch->point(_parent))) return false;
            }
        }
        onTouchesMoved();
    }
    
    return true;
}

void CSLayer::onTouchesEnded() {
    OnTouchesEnded(this);
}

void CSLayer::commitTouchesEnded() {
    if (_touches && _touches->count()) {
        onTouchesEnded();
        
        if (_touchRefresh) refresh();

        int i = 0;
        while (i < _touches->count()) {
            const CSTouch* touch = _touches->objectAtIndex(i);
            
            if (touch->state() == CSTouchStateEnded) _touches->removeObjectAtIndex(i);
            else i++;
        }
    }
}

void CSLayer::onTouchesCancelled() {
    OnTouchesCancelled(this);
}

void CSLayer::commitTouchesCancelled() {
    if (_touches) _touches->removeAllObjects();
    
    onTouchesCancelled();

    if (_touchRefresh) refresh();
}

void CSLayer::onWheel(float offset) {
	OnWheel(this, offset);
}

void CSLayer::onKeyDown(int keyCode) {
	OnKeyDown(this, keyCode);
}

void CSLayer::onKeyUp(int keyCode) {
	OnKeyUp(this, keyCode);
}

void CSLayer::onLink() {
    OnLink(this);
}

void CSLayer::onUnlink() {
    OnUnlink(this);
}

void CSLayer::onFrameChanged() {
    OnFrameChanged(this);
}

void CSLayer::onProjectionChanged() {
    OnProjectionChanged(this);
}

void CSLayer::onTouchesView(const CSArray<CSTouch>* touches, bool& interrupt) {
    OnTouchesView(this, touches, interrupt);
}

void CSLayer::onPause() {
    OnPause(this);
}

void CSLayer::onResume() {
    OnResume(this);
}

void CSLayer::onBackKey() {
    OnBackKey(this);
}

void CSLayer::onCustomEvent(int op, void* param, bool& interrupt) {
    OnCustomEvent(this, op, param, interrupt);
}

void CSLayer::validateLinks() {
    if (_links) {
        int i = 0;
        while (i < _links->count()) {
            CSLayer* link = _links->objectAtIndex(i);
            if (link->linked()) i++;
            else _links->removeObjectAtIndex(i);
        }
    }
}

bool CSLayer::clearLinks() {
    bool clear = true;
    
    if (_links && _links->count()) {
        foreach (CSLayer*, link, _links) link->removeFromParent(false);
        clear = false;
    }
    foreach (CSLayer*, layer, _layers) {
        if (!layer->clearLinks()) clear = false;
    }
    return clear;
}

void CSLayer::notifyLink() {
    _linked = true;
    onLink();
    foreach (CSLayer*, layer, _layers) layer->notifyLink();
}

void CSLayer::notifyUnlink() {
    //_color = CSColor::White;
    onUnlink();
    foreach (CSLayer*, layer, _layers) layer->notifyUnlink();
    _linked = false;
}

void CSLayer::notifyFrameChanged() {
    onFrameChanged();
    
    foreach (CSLayer*, layer, _layers) layer->notifyFrameChanged();
}

void CSLayer::notifyProjectionChanged() {
    onProjectionChanged();
    
    foreach (CSLayer*, layer, _layers) layer->notifyProjectionChanged();
}

void CSLayer::notifyTouchesView(const CSArray<CSTouch>* touches, bool& interrupt) {
    onTouchesView(touches, interrupt);
	if (interrupt) return;
    foreach (CSLayer*, layer, _layers) {
        layer->notifyTouchesView(touches, interrupt);
		if (interrupt) return;
    }
}

void CSLayer::notifyPause() {
    onPause();
    
    foreach (CSLayer*, layer, _layers) layer->notifyPause();
}

void CSLayer::notifyResume() {
    onResume();
    
    foreach (CSLayer*, layer, _layers) layer->notifyResume();
}

float CSLayer::keyboardScrollDegree(float bottom) const {
    float degree = 0;
    foreach (CSLayer*, layer, _layers) {
        float d = layer->keyboardScrollDegree(bottom);
        if (d > degree) {
            degree = d;
        }
    }
    return degree;
}

void CSLayer::submitLayout() {
    switch (_layout) {
        case CSLayerLayoutHorizontal:
            {
                float x = 0.0f;
                foreach (CSLayer *, layer, _layers) {
                    if (layer->state() != CSLayerStateRemoved) {
                        layer->submitFrame(CSRect(x, (_frame.height - layer->height()) * 0.5f, layer->width(), layer->height()), true);
                        x += layer->width() + _spacing;
                    }
                }
            }
            break;
        case CSLayerLayoutVertical:
            {
                float y = 0.0f;
                foreach (CSLayer *, layer, _layers) {
                    if (layer->state() != CSLayerStateRemoved) {
                        layer->submitFrame(CSRect((_frame.width - layer->width()) * 0.5f, y, layer->width(), layer->height()), true);
                        y += layer->height() + _spacing;
                    }
                }
            }
            break;
        case CSLayerLayoutFlow:
            {
                float x = 0.0f;
                float y = 0.0f;
                foreach (CSLayer *, layer, _layers) {
                    if (layer->state() != CSLayerStateRemoved) {
                        layer->submitFrame(CSRect(x, y, layer->width(), layer->height()), true);
                        x += layer->width() + _spacing;
                        if (x >= width()) {
                            y += layer->height() + _spacing;
                            x = 0.0f;
                        }
                    }
                }
            }
            break;
    }
}

void CSLayer::removeFromParent(bool transition) {
    if (_state == CSLayerStateDetach) {
        if (!transition) {
            _transitionProgress = 0;
            _state = CSLayerStateRemoved;
            onStateChanged();
        }
    }
    else if (_state != CSLayerStateRemoved) {
        if (_state != CSLayerStateAttach) _timeoutCounter = 0;
        _state = CSLayerStateDetach;
        onStateChanged();

        if (_state == CSLayerStateDetach && (!transition || !_transition)) {
            _transitionProgress = 0;
            _state = CSLayerStateRemoved;
            onStateChanged();
        }
        cancelTouches();
        cancelChildrenTouches();
    }
}

void CSLayer::addToParent() {
    if ((_parent || _linked) && (_state == CSLayerStateDetach || _state == CSLayerStateRemoved)) {
        _state = CSLayerStateAttach;
        onStateChanged();
    }
}

int CSLayer::layerCount() const {
    int layerCount = 1;
    foreach (CSLayer*, layer, _layers) layerCount += layer->layerCount();
    return layerCount;
}

void CSLayer::refresh() {
    if (_linked) CSApplication::sharedApplication()->refresh();
}

void CSLayer::clearLayers(bool transition) {
    foreach (CSLayer *, layer, _layers) layer->removeFromParent(transition);
}

bool CSLayer::addLayer(CSLayer* layer) {
    int i = _layers ? _layers->count() - 1 : -1;
    while (i >= 0) {
        if (layer->order() < _layers->objectAtIndex(i)->order()) i--;
        else break;
    }
    return insertLayer(i + 1,  layer);
}

bool CSLayer::insertLayer(int index, CSLayer* layer) {
    if (layer->attach(this)) {
        if (!_layers) _layers = new CSArray<CSLayer, 1, false>();
        _layers->insertObject(index, layer);
        layer->onStateChanged();
        
        submitLayout();
        
        if (_linked) {
            refresh();
            
            layer->notifyLink();
        }
        return true;
    }
    return false;
}

void CSLayer::clearLayers(int key, bool transition, bool inclusive, bool masking) {
    if (((masking ? (_key & key) : _key) == key) == inclusive) removeFromParent(transition);
    else {
        foreach (CSLayer*, layer, _layers) layer->clearLayers(key, transition, inclusive, masking);
    }
}

CSArray<CSLayer, 1, false>* CSLayer::findLayers(int key, bool masking) {
    CSArray<CSLayer, 1, false>* layers = CSArray<CSLayer, 1, false>::array();
    findLayers(key, layers, masking);
    return layers;
}

void CSLayer::findLayers(int key, CSArray<CSLayer, 1, false>* outArray, bool masking) {
    if ((masking ? (_key & key) : _key) == key) outArray->addObject(this);
    else {
        foreach (CSLayer *, layer, _layers) layer->findLayers(key, outArray, masking);
    }
}

CSLayer* CSLayer::findLayer(int key, bool masking) {
    if ((masking ? (_key & key) : _key) == key) return this;
    foreach (CSLayer *, layer, _layers) {
        CSLayer* result = layer->findLayer(key, masking);
        if (result) return result;
    }
    return NULL;
}

CSLayer* CSLayer::findParent(int key, bool masking) {
    if ((masking ? (_key & key) : _key) == key) return this;
    return _parent ? _parent->findParent(key) : NULL;
}

void CSLayer::convertToLocalSpace(CSVector2& point) const {
    if (_parent) _parent->convertToLocalSpace(point);
    else if (_linked) CSApplication::sharedApplication()->convertToLocalSpace(point);
    point.x -= _frame.x;
    point.y -= _frame.y;
}

void CSLayer::convertToParentSpace(CSVector2& point, const CSLayer* layer) const {
    if (this != layer) {
        point.x += _frame.x;
        point.y += _frame.y;
        if (_parent) _parent->convertToParentSpace(point, layer);
    }
}

void CSLayer::convertToParentSpace(CSVector2& point, int key, bool masking) const {
    if ((masking ? (_key & key) : _key) != key) {
        point.x += _frame.x;
        point.y += _frame.y;
        if (_parent) _parent->convertToParentSpace(point, key, masking);
    }
}

void CSLayer::convertToViewSpace(CSVector2& point) const {
    point.x += _frame.x;
    point.y += _frame.y;
    if (_parent) _parent->convertToViewSpace(point);
}

void CSLayer::setOwner(CSLayer* owner) {
    if (!owner->_links) owner->_links = new CSArray<CSLayer>(1);
    owner->_links->addObject(this);
}
