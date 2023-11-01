#define CDK_IMPL

#include "CSScrollPane.h"

#include "CSTime.h"

#include "CSApplication.h"

CSScrollPane::CSScrollPane() : scroll(this), _focusScroll(true) {
    scroll.OnScroll.add([this](CSScrollParent* parent) {
        refresh();
    });
    setTouchInherit(true);
    setTouchCarry(false);
    setTouchOut(true);
}

void CSScrollPane::focusLayer(CSLayer* layer, float duration) {
    if (layer->parent() == this) {
        const CSRect& frame = layer->nextFrame();
        
        bool flag = false;
        
        CSVector2 position = scroll.position();
        
        if (position.x > frame.x) {
            position.x = frame.x;
            flag = true;
        }
        else if (position.x < frame.x && position.x + width() < frame.x + frame.width) {
            position.x = CSMath::min(frame.x, frame.x + frame.width - width());
            flag = true;
        }
        if (position.y > frame.y) {
            position.y = frame.y;
            flag = true;
        }
        else if (position.y < frame.y && position.y + height() < frame.y + frame.height) {
            position.y = CSMath::min(frame.y, frame.y + frame.height - height());
            flag = true;
        }
        if (flag) {
            if (duration) scroll.set(position, duration);
            else scroll.set(position);
        }
    }
}

bool CSScrollPane::timeout(float delta, CSLayerVisible visible) {
    scroll.timeout(delta, isTouching());
    
    return CSLayer::timeout(delta, visible);
}

void CSScrollPane::onDrawContent(CSGraphics* graphics) {
	drawHandler(graphics, OnDrawContent);
}

void CSScrollPane::draw(CSGraphics* graphics) {
    graphics->push();
    graphics->translate(origin());
    
    if (transitionForward(graphics)) {
		graphics->push();

		commitDraw(graphics);
		graphics->reset();
		
		clip(graphics);
		const CSVector2& position = scroll.position();
		graphics->translate(-position);

		graphics->push();

		onDrawContent(graphics);
		graphics->reset();

        const CSArray<CSLayer, 1, false>* layers = this->layers();

		if (layers && layers->count()) {
			int i;

			for (i = layers->count() - 1; i > 0 && !layers->objectAtIndex(i)->covered(); i--);

			if (i > 0 && layers->objectAtIndex(i)->transitionBackward(graphics)) {
				int j;
				for (j = i; j > 0 && !layers->objectAtIndex(j)->covered(); j--);

				for (; j < i; j++) {
					CSLayer* layer = layers->objectAtIndex(j);
					CSRect frame = layer->frame();
					frame.origin() -= position;

					if (frame.right() > 0 && frame.left() < width() && frame.bottom() > 0 && frame.top() < height()) {
						layer->draw(graphics);
					}
				}
				graphics->reset();
			}

			for (; i < layers->count(); i++) {
				CSLayer* layer = layers->objectAtIndex(i);
				CSRect frame = layer->frame();
				frame.origin() -= position;

				if (frame.right() > 0 && frame.left() < width() && frame.bottom() > 0 && frame.top() < height()) {
					layer->draw(graphics);
				}
			}
		}

        graphics->pop();

		graphics->reset();
            
        scroll.draw(graphics);
        
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

CSLayerTouchReturn CSScrollPane::layersOnTouch(const CSTouch* touch, CSArray<CSLayer>* targetLayers, bool began) {
    if (state() == CSLayerStateFocus && enabled()) {
        CSVector2 point = touch->point(parent());
        
        if (frame().contains(point)) {
            bool touching = false;
            
            const CSArray<CSLayer, 1, false>* layers = this->layers();

            if (layers) {
                for (int i = layers->count() - 1; i >= 0; i--) {
                    CSLayer* layer = layers->objectAtIndex(i);

                    CSLayerTouchReturn rtn = layer->layersOnTouch(touch, targetLayers, began);

                    switch (rtn) {
                        case CSLayerTouchReturnCarry:
                            if (touchInherit()) {
                                touching = true;
                                goto occur;
                            }
                        case CSLayerTouchReturnCatch:
                            return rtn;
                    }
                }
            }
		occur:
            if (began || (touchIn() && (!touches() || !touches()->containsObject(touch)))) {
                addTouch(touch);
                targetLayers->addObject(this);
                touching = true;
            }
            if (touching) return touchCarry() ? CSLayerTouchReturnCarry : CSLayerTouchReturnCatch;
        }
    }
    return CSLayerTouchReturnNone;
}

bool CSScrollPane::touchContains(const CSVector2& p) const {
	return frame().contains(p);
}

void CSScrollPane::convertToLocalSpace(CSVector2& point) const {
    CSLayer::convertToLocalSpace(point);
    point += scroll.position();
}

void CSScrollPane::convertToParentSpace(CSVector2& point, const CSLayer* layer) const {
    if (this != layer) {
        point += origin() - scroll.position();
        const CSLayer* parent = this->parent();
        if (parent) parent->convertToParentSpace(point, layer);
    }
}

void CSScrollPane::convertToParentSpace(CSVector2& point, int key, bool masking) const {
    int mykey = this->key();
    if ((masking ? (mykey & key) : mykey) != key) {
        point += origin() - scroll.position();
        const CSLayer* parent = this->parent();
        if (parent) parent->convertToParentSpace(point, key);
    }
}

void CSScrollPane::convertToViewSpace(CSVector2& point) const {
    point += origin() - scroll.position();
    const CSLayer* parent = this->parent();
    if (parent) parent->convertToViewSpace(point);
}

void CSScrollPane::onTouchesMoved() {
    if (!_lockScroll) {
        const CSTouch* touch = touches()->objectAtIndex(0);
        CSVector2 p0 = touch->prevPoint(this);
        CSVector2 p1 = touch->point(this);
        
        scroll.drag(p0 - p1);
        
        if (_focusScroll) cancelChildrenTouches();
    }
    CSLayer::onTouchesMoved();
}

