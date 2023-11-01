#define CDK_IMPL

#include "CSScroll.h"

#include "CSMath.h"
#include "CSTime.h"

#define ScrollBarDuration           0.5f
#define ScrollBounceBackBoundary    0.5f
#define ScrollDragDecreaseRate      0.75f
#define ScrollReleaseDecreaseRate   0.95f
#define ScrollBounceBackRate        0.8f

CSScroll::CSScroll(CSScrollParent* parent) : _parent(parent) {
    
}

CSScroll::~CSScroll() {
    if (_auto) {
        delete _auto;
        _auto = NULL;
    }
}

static bool adjust(float& current, float& delta, float content, float clip, float interval, bool dragging, CSScrollBounceBack bounceBack) {
    bool flag;
    if (bounceBack == CSScrollBounceBackNone) flag = false;
    else if (bounceBack == CSScrollBounceBackIfNeeded) flag = content > clip;
    else flag = true;
    
    float value = current;
    
    if (flag) {
        if (delta) {
            float boundary = content * ScrollBounceBackBoundary;
            
            float next = value + delta;
            
            if (next < 0) {
                value = delta < 0 ? (next < -boundary ? -boundary : value + delta * (1 + value / boundary)) : next;
                if (!dragging) delta = 0;
            }
            else {
                float max = CSMath::max(content - clip, 0.0f);
                
                if (next > max) {
                    value = delta > 0 ? (next > max + boundary ? max + boundary : value + delta * ((max + boundary) - value) / boundary) : next;
                    if (!dragging) delta = 0;
                }
                else value = next;
            }
        }
        else if (!dragging) {
            if (value < 0) {
                value *= CSMath::pow(ScrollBounceBackRate, interval * 60);
                if (value > -1) value = 0;
            }
            else {
                float max = CSMath::max(content - clip, 0.0f);
                
                if (value > max) {
                    value -= (value - max) * (1 - CSMath::pow(ScrollBounceBackRate, interval * 60));
                    if (value < max + 1) value = max;
                }
            }
        }
    }
    else {
        if (content <= clip) {
            value = 0;
            delta = 0;
        }
        else {
            float next = value + delta;
            
            if (next < 0) {
                value = 0;
                delta = 0;
            }
            else {
                float max = CSMath::max(content - clip, 0.0f);
                
                if (next > max) {
                    value = max;
                    delta = 0;
                }
                else value = next;
            }
        }
    }
    if (current != value) {
        current = value;
        return true;
    }
    return false;
}

static bool jump(float& current, float value, float content, float clip, CSScrollBounceBack bounceBack) {
    if (value < 0 || content <= clip) value = 0;
    else if (value + clip > content) value = content - clip;

    if (current != value) {
        current = value;
        return true;
    }
    return false;
}

bool CSScroll::adjust(CSVector2& delta, float interval, bool dragging) {
    CSVector2 content = _parent->scrollContentSize();
    CSVector2 clip = _parent->scrollClipSize();
    
    bool refresh = false;
    refresh |= ::adjust(_current.x, delta.x, content.x, clip.x, interval, dragging, _horizontalBounceBack);
    refresh |= ::adjust(_current.y, delta.y, content.y, clip.y, interval, dragging, _verticalBounceBack);
    return refresh;
}

bool CSScroll::jump(const CSVector2& position) {
    CSVector2 content = _parent->scrollContentSize();
    CSVector2 clip = _parent->scrollClipSize();
    
    bool refresh = false;
    refresh |= ::jump(_current.x, position.x, content.x, clip.x, _horizontalBounceBack);
    refresh |= ::jump(_current.y, position.y, content.y, clip.y, _verticalBounceBack);
    return refresh;
}

void CSScroll::stop() {
    _delta = CSVector2::Zero;
    if (_auto) {
        delete _auto;
        _auto = NULL;
    }
}

void CSScroll::set(const CSVector2& position) {
    stop();

    if (jump(position)) OnScroll(_parent);
}

void CSScroll::set(const CSVector2& position, float duration) {
    const CSVector2& current = _auto ? _auto->next : _current;
    
    _delta = CSVector2::Zero;
    
    if (current != position) {
        if (!_auto) _auto = new Auto();
        _auto->prev = _current;
        _auto->next = position;
        _auto->duration = duration;
        _auto->progress = 0;
    }
    else {
        if (_auto) {
            delete _auto;
            _auto = NULL;
        }
    }
}

void CSScroll::drag(const CSVector2& delta) {
    if (!_auto) {
        if (delta) {
            _delta += delta;
            _barRemaining = ScrollBarDuration;
        }
        CSVector2 d = delta;
        if (adjust(d, 0, true)) OnScroll(_parent);
    }
}

void CSScroll::timeout(float interval, bool dragging) {
    bool refresh = false;
    
    if (_auto) {
        _auto->progress += interval;
        
        float progress = _auto->progress / _auto->duration;
        
        if (progress < 1) {
            CSVector2 value = CSVector2::lerp(_auto->prev, _auto->next, progress);
            
            refresh |= jump(value);
        }
        else {
            refresh |= jump(_auto->next);
            delete _auto;
            _auto = NULL;
        }
    }
    else{
        float deltaDecreaseRate;
        if (dragging) {
            deltaDecreaseRate = CSMath::pow(ScrollDragDecreaseRate, interval * 60);
        }
        else {
            refresh |= adjust(_delta, interval, false);
            deltaDecreaseRate = CSMath::pow(ScrollReleaseDecreaseRate, interval * 60);
        }
        _delta.x = (int)_delta.x ? _delta.x * deltaDecreaseRate : 0;
        _delta.y = (int)_delta.y ? _delta.y * deltaDecreaseRate : 0;
    }
    if (_barRemaining) {
        _barRemaining -= interval;
        if (_barRemaining > 0) {
            if (_barHidden) refresh = true;
        }
        else _barRemaining = 0;
    }
    if (refresh) OnScroll(_parent);
}

void CSScroll::draw(CSGraphics* graphics) {
    if (_barThickness && (_barRemaining || !_barHidden)) {
        CSVector2 content = _parent->scrollContentSize();
        CSVector2 clip = _parent->scrollClipSize();
        
        graphics->pushColor();
        if (_barHidden) {
            graphics->setColor(CSColor(_barColor.r, _barColor.g, _barColor.b, _barColor.a * _barRemaining / ScrollBarDuration));
        }
        else {
            graphics->setColor(_barColor);
        }
        if (content.x > clip.x) {
            float barWidth = clip.x * clip.x / content.x;
            float barX = _current.x * (clip.x - barWidth) / (content.x - clip.x);
            
            if (barX < 0) {
                barWidth += barX;
                barX = 0;
            }
            else if (barX + barWidth > clip.x) {
                barWidth = clip.x - barX;
            }
                
            graphics->drawRoundRect(CSRect(barX, clip.y - _barThickness, barWidth, _barThickness), _barThickness * 0.3f, true);
        }
        
        if (content.y > clip.y) {
            float barHeight = clip.y * clip.y / content.y;
            float barY = _current.y * (clip.y - barHeight) / (content.y - clip.y);
            
            if (barY < 0) {
                barHeight += barY;
                barY = 0;
            }
            else if (barY + barHeight > clip.y) {
                barHeight = clip.y - barY;
            }
            
            graphics->drawRoundRect(CSRect(clip.x - _barThickness, barY, _barThickness, barHeight), _barThickness * 0.3f, true);
        }
        graphics->popColor();
    }
}
