#define CDK_IMPL

#include "CSApplicationBridge.h"

#include "CSStringBuilder.h"

#include "CSThread.h"
#include "CSDiagnostics.h"

#include "CSPopupCoverLayer.h"

CSApplication* CSApplication::_instance = NULL;

CSApplication::CSApplication(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc) :
    _graphics(new CSGraphics(CSRenderTarget::target(width, height, framebuffer, desc))),
    _timeStamp(CSTime::currentTime())
{
#ifdef CS_DISPLAY_COUNTER
    _lastFrameStamp = _timeStamp;
#endif
}

void CSApplication::initialize(int width, int height, int framebuffer, const CSSystemRenderTargetDescription& desc) {
    if (!_instance) _instance = new CSApplication(width, height, framebuffer, desc);
}

void CSApplication::finalize() {
    if (_instance) {
        delete _instance;
        _instance = NULL;
    }
}

void CSApplication::setVersion(const string& version) {
    if (_version != version) {
        _version = version;
        CSApplicationBridge::setVersion(version);
    }
}

CSRect CSApplication::frame() const {
    return CSApplicationBridge::frame();
}

CSEdgeInsets CSApplication::edgeInsets() const {
    return CSApplicationBridge::edgeInsets();
}

void CSApplication::setResolution(CSResolution resolution) {
    CSApplicationBridge::setResolution(resolution);
}

CSResolution CSApplication::resolution() const {
    return CSApplicationBridge::resolution();
}

void CSApplication::setProjection(const Projection& projection) {
    _projection = projection;
    foreach (CSLayer*, layer, &_layers) layer->notifyProjectionChanged();
    refresh();
}

CSApplication::Projection CSApplication::projection() const {
    Projection result = _projection;
    if (result.width <= 0) result.width = _graphics->target()->width();
    if (result.height <= 0) result.height = _graphics->target()->height();
    return result;
}

float CSApplication::projectionWidth() const {
    return _projection.width <= 0 ? _graphics->target()->width() : _projection.width;
}

float CSApplication::projectionHeight() const {
    return _projection.height <= 0 ? _graphics->target()->height() : _projection.height;
}

int CSApplication::layerCount() const {
    int layerCount = 0;
    foreach (CSLayer*, layer, &_layers) layerCount += layer->layerCount();
    return layerCount;
}

void CSApplication::setEnabled(bool enabled) {
    _enabled = enabled;
    if (!enabled) {
        foreach (CSArray<CSLayer>*, layers, _touchLayers.allObjects()) {
            foreach (CSLayer*, layer, layers) layer->cancelTouches();
        }
    }
}

void CSApplication::touchesBegan(const CSPlatformTouch* pts, int count) {
    if (!_enabled) {
        return;
    }
    for (int i = 0; i < count; i++) {
        const CSPlatformTouch& pt = pts[i];
        CSTouch* t = CSTouch::touch(pt.key, pt.button, CSVector2(pt.x, pt.y));
        _touches.setObject(pt.key, t);
        if (!_touchLayers.containsKey(pt.key)) {
            CSArray<CSLayer>* touchLayers = new CSArray<CSLayer>(1);
            _touchLayers.setObject(pt.key, touchLayers);
            touchLayers->release();
        }
    }
    bool interrupt = false;
    {
        const CSArray<CSTouch>* touches = _touches.allObjects();
        foreach (CSLayer*, layer, &_layers) {
            layer->notifyTouchesView(touches, interrupt);
            if (interrupt) return;
        }
    }
    for (CSDictionary<uint64, CSArray<CSLayer>>::Iterator i = _touchLayers.iterator(); i.remaining(); i.next()) {
        foreach (CSLayer*, layer, i.object()) {
            if (!layer->touchMulti()) return;
        }
    }
    CSLayer* topLayer = NULL;
    for (int i = _layers.count() - 1; i >= 0; i--) {
        CSLayer* layer = _layers.objectAtIndex(i);
        if (layer->enabled()) {
            topLayer = layer;
            break;
        }
    }
    if (topLayer) {
        CSArray<CSLayer> layers;

        for (int i = 0; i < count && !interrupt; i++) {
            uint64 key = pts[i].key;

            CSTouch* t = _touches.objectForKey(key);

            CSArray<CSLayer>* touchLayers = _touchLayers.objectForKey(key);

            if (topLayer->layersOnTouch(t, touchLayers, true)) {
                foreach (CSLayer*, layer, touchLayers) {
                    if (!layers.containsObject(layer)) {
                        layers.addObject(layer);
                        if (!layer->touchMulti()) interrupt = true;
                    }
                }
            }
        }
        if (layers.count()) {
            foreach (CSLayer*, layer, &layers) layer->commitTouchesBegan();
        }
        else if (topLayer->state() == CSLayerStateFocus && topLayer->enabled()) {
            cancelTouches(topLayer);
            topLayer->commitTouchesCancelled();
        }
    }
}

void CSApplication::touchesMoved(const CSPlatformTouch* pts, int count) {
    if (!_enabled) return;

    CSArray<CSLayer> layers;

    for (int i = 0; i < count; i++) {
        const CSPlatformTouch& pt = pts[i];

        CSVector2 p(pt.x, pt.y);

        CSTouch* t = _touches.objectForKey(pt.key);

        if (t) {
            if (t->state() != CSTouchStateMoved) {
                CSVector2 fcp = t->firstPoint();
                convertToLocalSpace(fcp);
                CSVector2 ccp = p;
                convertToLocalSpace(ccp);

                if (fcp.distanceSquared(ccp) <= _touchSensitivity * _touchSensitivity) {
                    continue;
                }
            }
            t->setPoint(p);

            foreach (CSLayer*, layer, _touchLayers.objectForKey(pt.key)) {
                if (!layers.containsObject(layer)) layers.addObject(layer);
            }
        }
    }

    foreach (CSLayer*, layer, &layers) {
        if (!layer->commitTouchesMoved()) {
            cancelTouches(layer);

            layer->commitTouchesCancelled();
        }
    }

    layers.removeAllObjects();

    CSLayer* topLayer = _layers.lastObject();

    if (topLayer) {
        CSArray<CSLayer>* layersOnTouch = new CSArray<CSLayer>();

        for (int i = 0; i < count; i++) {
            const CSPlatformTouch& pt = pts[i];

            CSTouch* t = _touches.objectForKey(pt.key);

            if (t) {
                layersOnTouch->removeAllObjects();

                if (topLayer->layersOnTouch(t, layersOnTouch, false)) {
                    foreach (CSLayer*, layer, layersOnTouch) {
                        if (!layers.containsObject(layer)) layers.addObject(layer);
                    }

                    CSArray<CSLayer>* touchLayers = _touchLayers.objectForKey(pt.key);

                    for (int j = 0; j < layersOnTouch->count(); j++) {
                        touchLayers->insertObject(j, layersOnTouch->objectAtIndex(j));
                        j++;
                    }
                }
            }
        }

        layersOnTouch->release();
    }

    foreach (CSLayer*, layer, &layers) layer->commitTouchesBegan();
}

void CSApplication::touchesEnded(const CSPlatformTouch* pts, int count) {
    if (!_enabled) return;

    CSArray<CSLayer> layers;

    for (int i = 0; i < count; i++) {
        const CSPlatformTouch& pt = pts[i];

        CSTouch* t = _touches.objectForKey(pt.key);

        if (t) {
            t->end();

            foreach (CSLayer*, layer, _touchLayers.objectForKey(pt.key)) {
                if (!layers.containsObject(layer)) layers.addObject(layer);
            }
            _touchLayers.removeObject(pt.key);
            _touches.removeObject(pt.key);
        }
    }

    foreach (CSLayer*, layer, &layers) layer->commitTouchesEnded();
}

void CSApplication::touchesCancelled(const CSPlatformTouch* touches, int count) {
    if (!_enabled) {
        return;
    }
    CSArray<CSLayer> layers;

    for (CSDictionary<uint64, CSArray<CSLayer>>::Iterator i = _touchLayers.iterator(); i.remaining(); i.next()) {
        foreach (CSLayer*, layer, i.object()) {
            if (!layers.containsObject(layer)) layers.addObject(layer);
        }
    }

    foreach (CSLayer*, layer, &layers) layer->commitTouchesCancelled();

    _touches.removeAllObjects();
    _touchLayers.removeAllObjects();
}

bool CSApplication::beginTouch(CSLayer* layer, const CSTouch* touch) {
    CSArray<CSLayer>* touchLayers = _touchLayers.objectForKey(touch->key());

    if (!touchLayers->containsObject(layer)) {
        touchLayers->addObject(layer);
        return true;
    }
    return false;
}

void CSApplication::cancelTouches(CSLayer* layer) {
    foreach (const CSTouch*, touch, layer->touches()) {
        CSArray<CSLayer>* layers = _touchLayers.objectForKey(touch->key());
        if (layers) layers->removeObjectIdenticalTo(layer);
    }
}

void CSApplication::wheel(float offset) {
    CSLayer* layer = _layers.lastObject();

    if (layer && layer->state() == CSLayerStateFocus) layer->onWheel(offset);
}

void CSApplication::keyDown(int keyCode) {
    CSLayer* layer = _layers.lastObject();

    if (layer && layer->state() == CSLayerStateFocus) layer->onKeyDown(keyCode);

#ifdef CS_DISPLAY_LAYER_FRAME
    if (keyCode == CSKeyCodeSpace) {
        displayLayerFrame = !displayLayerFrame;
        refresh();
    }
#endif
#if defined(CS_DISPLAY_COUNTER) && defined(CS_DIAGNOSTICS)
    if (keyCode == CSKeyCodeTab) {
        displayDiagnostics = !displayDiagnostics;
        refresh();
    }
#endif
}

void CSApplication::keyUp(int keyCode) {
    CSLayer* layer = _layers.lastObject();

    if (layer && layer->state() == CSLayerStateFocus) layer->onKeyUp(keyCode);
}

void CSApplication::backKey() {
    CSLayer* layer = _layers.lastObject();

    if (layer && layer->state() == CSLayerStateFocus) layer->onBackKey();
}

void CSApplication::timeout() {
    if (_graphics->remaining() > _vsyncFrame) return;

    double current = CSTime::currentTime();
    float delta = CSTime::currentTime() - _timeStamp;
    _timeStamp = current;
#ifdef CS_DISPLAY_COUNTER
    if (current - _lastFrameStamp >= 1) {
        _lastFrameStamp = current;
        _lastFrameCount = _frameCount;
        _frameCount = 0;
# ifdef CS_DIAGNOSTICS
        CSDiagnostics::timeReset();
# endif
        _refresh = true;
    }
    else _frameCount++;
#endif
    CSLayerVisible visible = CSLayerVisibleForward;

    int i = _layers.count() - 1;
    while (i >= 0) {
        CSLayer* layer = _layers.objectAtIndex(i);

        bool end = layer->timeout(delta, visible);

        i = _layers.indexOfObjectIdenticalTo(layer);

        if (end) {
            _layers.removeObjectAtIndex(i);
        }
        else if (layer->covered()) {
            visible = CSLayerVisibleCovered;
        }
        else if (visible == CSLayerVisibleForward && layer->enabled()) {
            visible = CSLayerVisibleBackground;
        }
        i--;
    }

    if (_refresh) {
        draw();
        _refresh = false;
    }
}

void CSApplication::resize(int width, int height) {
    _graphics->target()->resize(width, height);
}

void CSApplication::draw() {
    _graphics->focus();
    _graphics->reset();
    _graphics->clear(CSColor::Black);
    _graphics->camera().setProjection(_projection.fov, projectionWidth(), projectionHeight(), _projection.znear, _projection.zfar);
    _graphics->camera().setPosition(CSVector3(0, 0, _graphics->camera().defaultDistance()));

    _graphics->translate(CSVector3(-_graphics->camera().width() * 0.5f, -_graphics->camera().height() * 0.5f, 0));

    _graphics->push();
    int i;
    for (i = _layers.count() - 1; i > 0 && !_layers.objectAtIndex(i)->covered(); i--);
    if (i > 0) {
        if (_layers.objectAtIndex(i)->transitionBackward(_graphics)) {
            int j;
            for (j = i - 1; j > 0 && !_layers.objectAtIndex(j)->covered(); j--);
            for (; j < i; j++) {
                _layers.objectAtIndex(j)->draw(_graphics);
            }
        }
        _graphics->reset();
    }
    for (; i < _layers.count(); i++) {
        _layers.objectAtIndex(i)->draw(_graphics);
    }
    _graphics->pop();

#ifdef CS_DISPLAY_COUNTER
    {
        CSStringBuilder strbuf;
# ifdef DEBUG
        strbuf.append("DBG.");
# else
        strbuf.append("REL.");
# endif
        strbuf.append(CSApplication::sharedApplication()->version());
        strbuf.appendFormat(" FPS.%d Layer.%d / %d", _lastFrameCount, _layers.count(), layerCount());
# ifdef CS_DIAGNOSTICS
        CSDiagnostics::Capture capture;
        const CSDiagnostics::RenderRecord* renderRecord = capture.renderRecord();
        strbuf.appendFormat(" Draw.%d / %d", renderRecord->drawCount, renderRecord->vertexCount);
        if (displayDiagnostics) {
            strbuf.append("\n");
            foreach (const CSDiagnostics::TimeRecord&, timeRecord, capture.timeRecords()) {
                strbuf.appendFormat("%s:%.4f\n", timeRecord.name, timeRecord.timeTotal / timeRecord.timeCount);
            }
            strbuf.appendFormat("command count:%d / %d / %d\n", renderRecord->originCommandCount, renderRecord->batchCommandCount, renderRecord->invokeCount);
            strbuf.appendFormat("object alloc count:%d", capture.objectCount());
        }
# endif
        string str = strbuf.toString();

        _graphics->setColor(CSColor::Black);
        _graphics->drawString(str, CSVector2(6, 6));
        _graphics->setColor(CSColor::White);
        _graphics->drawString(str, CSVector2(5, 5));
    }
#endif

    CSAssert(_graphics->pushCount() == 0, "push count mismatch");

    CSApplicationBridge::draw(_graphics);

    _graphics->render();

    CSApplicationBridge::swapBuffer();
}


void CSApplication::pause() {
    foreach (CSLayer*, layer, &_layers) layer->notifyPause();
}

void CSApplication::resume() {
    foreach (CSLayer*, layer, &_layers) layer->notifyResume();
}

void CSApplication::setKeyboardHeight(float height) {
    _keyboardHeight = height * _projection.height / this->height();
}

void CSApplication::clearLayers(bool transition) {
    foreach (CSLayer*, layer, &_layers) layer->removeFromParent(transition);
}

bool CSApplication::addLayer(CSLayer* layer) {
    int i = _layers.count() - 1;
    while (i >= 0) {
        if (layer->order() < _layers.objectAtIndex(i)->order()) i--;
        else break;
    }
    return insertLayer(i + 1, layer);
}

bool CSApplication::insertLayer(int index, CSLayer* layer) {
    if (layer->attach(NULL)) {
        _layers.insertObject(index, layer);
        _refresh = true;
        layer->notifyLink();
       return true;
    }
    return false;
}

void CSApplication::clearLayers(int key, bool transition, bool inclusive, bool masking) {
    foreach (CSLayer*, layer, &_layers) {
        if (((masking ? (layer->key() & key) : layer->key()) == key) == inclusive) {
            layer->removeFromParent(transition);
        }
        else {
            layer->clearLayers(key, transition, inclusive, masking);
        }
    }
}

const CSArray<CSLayer, 1, false>* CSApplication::findLayers(int key, bool masking) {
    CSArray<CSLayer, 1, false>* layers = CSArray<CSLayer, 1, false>::array();
    findLayers(key, layers, masking);
    return layers;
}

void CSApplication::findLayers(int key, CSArray<CSLayer, 1, false>* outArray, bool masking) {
    foreach (CSLayer*, layer, &_layers) {
        if ((masking ? (layer->key() & key) : layer->key()) == key) {
            outArray->addObject(layer);
        }
        layer->findLayers(key, outArray, masking);
    }
}

CSLayer* CSApplication::findLayer(int key, bool masking) {
    foreach (CSLayer*, layer, &_layers) {
        if ((masking ? (layer->key() & key) : layer->key()) == key) {
            return layer;
        }
        else {
            CSLayer* rtnLayer = layer->findLayer(key, masking);
            if (rtnLayer) return rtnLayer;
        }
    }
    return NULL;
}

void CSApplication::insertLayerAsPopup(int index, CSLayer* layer, float darkness, float blur) {
    if (!layer->parent() && !layer->linked()) {
        insertLayer(index, CSPopupCoverLayer::layer(layer, darkness, blur));
    }
}

void CSApplication::addLayerAsPopup(CSLayer* layer, float darkness, float blur) {
    if (!layer->parent() && !layer->linked()) {
        addLayer(CSPopupCoverLayer::layer(layer, darkness, blur));
    }
}
void CSApplication::convertToUIFrame(CSRect& frame) const {
    CSVector2 size = this->size();

    float xrate = size.x / projectionWidth();
    float yrate = size.y / projectionHeight();

    frame.x *= xrate;
    frame.y *= yrate;
    frame.width *= xrate;
    frame.height *= yrate;
}

void CSApplication::convertToLocalSpace(CSVector2& point) const {
    CSVector2 size = this->size();

    point.x *= projectionWidth() / size.x;
    point.y *= projectionHeight() / size.y;
}

bool CSApplication::customEvent(int op, void* param) {
    for (int i = _layers.count() - 1; i >= 0; i--) {
        CSLayer* layer = _layers.objectAtIndex(i);
        if (layer->customEvent(op, param)) return true;
    }
    return false;
}

bool CSApplication::screenshot(const char* path) const {
    return _graphics->target()->screenshot(path);
}

void CSApplication::openURL(const char* url) {
    CSApplicationBridge::openURL(url);
}

const char* CSApplication::clipboard() {
    return CSApplicationBridge::clipboard();
}

void CSApplication::setClipboard(const char* text) {
    CSApplicationBridge::setClipboard(text);
}

void CSApplication::shareUrl(const char* title, const char* message, const char* url) {
    CSApplicationBridge::shareUrl(title, message, url);
}

void CSApplication::finish() {
    CSApplicationBridge::finish();
}
