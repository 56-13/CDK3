#ifndef __CDK__CSSpriteTimeline__
#define __CDK__CSSpriteTimeline__

#include "CSSpriteElement.h"

class CSSpriteObject;
class CSBuffer;

class CSSpriteTimeline : public CSObject {
public:
	float startTime;
	float endTime;
	byte layer;
	bool reset;
	CSArray<CSSpriteElement> elements;

	CSSpriteTimeline(float startTIme, float endTime, int layer);
	CSSpriteTimeline(CSBuffer* buffer);
	CSSpriteTimeline(const CSSpriteTimeline* other);
private:
	~CSSpriteTimeline() = default;
public:
	static inline CSSpriteTimeline* timeline(float startTIme, float endTime, int layer) {
		return autorelease(new CSSpriteTimeline(startTIme, endTime, layer));
	}
	static inline CSSpriteTimeline* timelineWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSSpriteTimeline(buffer));
	}
	static inline CSSpriteTimeline* timelineWithTimeline(const CSSpriteTimeline* other) {
		return autorelease(new CSSpriteTimeline(other));
	}

	int resourceCost() const;
	void preload() const;

	inline float duration() const {
		return endTime - startTime;
	}
};

#endif
