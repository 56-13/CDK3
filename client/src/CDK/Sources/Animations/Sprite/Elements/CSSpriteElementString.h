#ifndef __CDK__CSSpriteElementString__
#define __CDK__CSSpriteElementString__

#include "CSSpriteElement.h"

#include "CSLocaleString.h"

#include "CSAnimationFloat.h"
#include "CSMaterialSource.h"

#include "CSFont.h"

class CSSpriteElementString : public CSSpriteElement {
public:
	class Content {
	private:
		enum Type : byte {
			TypeNone,
			TypeString,
			TypeLocaleString,
			TypeStringRef,
			TypeLocaleStringRef
		};
		Type _type;
		union {
			const CSStringContent* content;
			const CSLocaleString* localeContent;
			const CSArray<ushort>* indices;
		} _var = {};
	public:
		Content();
		Content(const string& content);
		Content(const CSLocaleString* content);
		Content(const CSArray<ushort>* indices, bool locale);
		Content(CSBuffer* buffer);
		~Content();

		Content(const Content& other);
		Content& operator=(const Content& other);

		operator string() const;

		int resourceCost() const;
		void preload() const;
	};
	enum Scaling : byte {
		ScalingNone,
		ScalingHorizontal,
		ScalingBoth
	};

	Content content;
	int startLength = 10000;
	int endLength = 10000;
	CSVector3 position = CSVector3::Zero;
	CSPtr<CSAnimationFloat> x;
	CSPtr<CSAnimationFloat> y;
	CSPtr<CSAnimationFloat> z;
	CSAlign align = CSAlignCenterMiddle;
	Scaling scaling = ScalingNone;
	float scalingWidth = 0;
	float scalingHeight = 0;
	CSPtr<const CSFont> font;
	CSPtr<CSMaterialSource> material;

	CSSpriteElementString();
	CSSpriteElementString(CSBuffer* buffer);
	CSSpriteElementString(const CSSpriteElementString* other);
private:
	~CSSpriteElementString() = default;
public:
	static inline CSSpriteElementString* element() {
		return autorelease(new CSSpriteElementString());
	}

	inline Type type() const override {
		return TypeString;
	}
	int resourceCost() const override;
	void preload() const override;
	bool addAABB(TransformParam& param, CSABoundingBox& result) const override;
	void getTransformUpdated(TransformUpdatedParam& param, uint& outflags) const;
	void draw(DrawParam& param) const override;
private:
	CSVector3 getPosition(float progress, int random) const;
};

#endif