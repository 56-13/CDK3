#ifndef __CDK__CSBoxObject__
#define __CDK__CSBoxObject__

#include "CSSceneObject.h"
#include "CSMaterialSource.h"

class CSBoxBuilder;

class CSBoxObject : public CSSceneObject {
private:
	const CSBoxBuilder* _origin;
	float _progress;
	int _random;
public:
	CSBoxObject(const CSBoxBuilder* origin);
private:
	~CSBoxObject();
public:
	static inline CSBoxObject* object(const CSBoxBuilder* origin) {
		return autorelease(new CSBoxObject(origin));
	}

	inline Type type() const override {
		return TypeBox;
	}
	inline const CSBoxBuilder* origin() const {
		return _origin;
	}
	bool addAABB(CSABoundingBox& result) const override;
	void addCollider(CSCollider*& result) const override;
protected:
	void onRewind() override;
	UpdateState onUpdate(float delta, bool alive, uint& flags) override;
	uint onShow() override;
	void onDraw(CSGraphics* graphics, CSInstanceLayer layer) override;
};

class CSBoxBuilder : public CSSceneObjectBuilder {
public:
	CSVector3 extent = CSVector3(100);
	bool collision = false;
	CSPtr<CSMaterialSource> material;

	CSBoxBuilder() = default;
	CSBoxBuilder(CSBuffer* buffer, bool withScene);
	CSBoxBuilder(const CSBoxBuilder* other);
private:
	~CSBoxBuilder() = default;
public:
	static inline CSBoxBuilder* builder() {
		return autorelease(new CSBoxBuilder());
	}
	static inline CSBoxBuilder* builderWithBuilder(const CSBoxBuilder* other) {
		return autorelease(new CSBoxBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeBox;
	}
	int resourceCost() const override;
	inline CSBoxObject* createObject() const override {
		return new CSBoxObject(this);
	}
	void preload() const override;
};

#endif