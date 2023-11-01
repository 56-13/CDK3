#ifndef __CDK__CSTouch__
#define __CDK__CSTouch__

#include "CSObject.h"
#include "CSVector2.h"

enum CSTouchButton : byte {
	CSTouchButtonMouseLeft,		//same as GLFW_MOUSE_BUTTON
	CSTouchButtonMouseRight,
	CSTouchButtonMouseMiddle,
	CSTouchButtonMouse4,
	CSTouchButtonMouse5,
	CSTouchButtonMouse6,
	CSTouchButtonMouse7,
	CSTouchButtonMouse8,
	CSTouchButtonFinger
};

enum CSTouchState : byte {
    CSTouchStateStopped,
    CSTouchStateMoved,
    CSTouchStateEnded
};

class CSLayer;

class CSTouch : public CSObject {
private:
    uint64 _key;
	CSTouchButton _button;
    CSVector2 _firstPoint;
    CSVector2 _prevPoint;
    CSVector2 _point;
    CSTouchState _state;
    double _timestamp;
public:
    CSTouch(uint64 key, CSTouchButton button, const CSVector2& point);
private:
    ~CSTouch() = default;
public:
    static inline CSTouch* touch(uint64 key, CSTouchButton button, const CSVector2& point) {
        return autorelease(new CSTouch(key, button, point));
    }
    
    void setPoint(const CSVector2& point);
    
    inline uint64 key() const {
        return _key;
    }
	inline CSTouchButton button() const {
		return _button;
	}
    inline CSTouchState state() const {
        return _state;
    }
    inline void end() {
        _state = CSTouchStateEnded;
    }
    inline bool isMoved() const {
        return _prevPoint != _point;
    }

    inline CSVector2 convertToLayerSpace(const CSLayer* layer, CSVector2 point) const;

    inline const CSVector2& firstPoint() const {
        return _firstPoint;
    }
    inline const CSVector2& prevPoint() const {
        return _prevPoint;
    }
    inline const CSVector2& point() const {
        return _point;
    }
    CSVector2 firstPoint(const CSLayer* layer) const;
    CSVector2 prevPoint(const CSLayer* layer) const;
    CSVector2 point(const CSLayer* layer) const;
    
    float time() const;

    inline uint hash() const override {
        return _key;
    }
    inline bool isEqual(const CSTouch* other) const {
		return _key == other->_key;
	}
	inline bool isEqual(const CSObject* object) const override {
		const CSTouch* other = dynamic_cast<const CSTouch*>(object);

		return other && isEqual(other);
	}
};

#endif
