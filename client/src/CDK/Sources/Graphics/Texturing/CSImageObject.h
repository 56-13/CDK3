#ifndef __CDK__CSImageObject__
#define __CDK__CSImageObject__

#include "CSSceneObject.h"

class CSImageBuilder;

class CSImageObject : public CSSceneObject {
private:
	const CSImageBuilder* _origin;
public:
	CSImageObject(const CSImageBuilder* origin);
private:
	~CSImageObject();
public:
	static inline CSImageObject* object(const CSImageBuilder* origin) {
		return autorelease(new CSImageObject(origin));
	}

	inline Type type() const override {
		return TypeImage;
	}
	inline const CSImageBuilder* origin() const {
		return _origin;
	}
	bool addAABB(CSABoundingBox& result) const override;
	bool getTransform(float progress, const string& name, CSMatrix& result) const override;
protected:
	UpdateState onUpdate(float delta, bool alive, uint& flags) override;
	uint onShow() override;
	void onDraw(CSGraphics* graphics, CSInstanceLayer layer) override;
};

class CSImageBuilder : public CSSceneObjectBuilder {
public:
	CSPtr<const CSArray<ushort>> indices;
	CSInstanceBlendLayer layer = CSInstanceBlendLayerMiddle;
	CSBlendMode blendMode = CSBlendAlpha;
	bool billboard = true;
	float depthBias = 0;

	CSImageBuilder() = default;
	CSImageBuilder(CSBuffer* buffer, bool withScene);
	CSImageBuilder(const CSImageBuilder* other);
private:
	~CSImageBuilder() = default;
public:
	static inline CSImageBuilder* builder() {
		return autorelease(new CSImageBuilder());
	}
	static inline CSImageBuilder* builderWithBuilder(const CSImageBuilder* other) {
		return autorelease(new CSImageBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeImage;
	}
	int resourceCost() const override;
	inline CSImageObject* createObject() const override {
		return new CSImageObject(this);
	}
	void preload() const override;
};

#endif