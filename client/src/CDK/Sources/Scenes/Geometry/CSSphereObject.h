#ifndef __CDK__CSSphereObject__
#define __CDK__CSSphereObject__

#include "CSSceneObject.h"
#include "CSMaterialSource.h"

class CSSphereBuilder;

class CSSphereObject : public CSSceneObject {
private:
	const CSSphereBuilder* _origin;
	float _progress;
	int _random;
public:
	CSSphereObject(const CSSphereBuilder* origin);
private:
	~CSSphereObject();
public:
	static inline CSSphereObject* object(const CSSphereBuilder* origin) {
		return autorelease(new CSSphereObject(origin));
	}

	inline Type type() const override {
		return TypeSphere;
	}
	inline const CSSphereBuilder* origin() const {
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

class CSSphereBuilder : public CSSceneObjectBuilder {
public:
	float radius = 100;
	bool collision = false;
	CSPtr<CSMaterialSource> material;

	CSSphereBuilder() = default;
	CSSphereBuilder(CSBuffer* buffer, bool withScene);
	CSSphereBuilder(const CSSphereBuilder* other);
private:
	~CSSphereBuilder() = default;
public:
	static inline CSSphereBuilder* builder() {
		return autorelease(new CSSphereBuilder());
	}
	static inline CSSphereBuilder* builderWithBuilder(const CSSphereBuilder* other) {
		return autorelease(new CSSphereBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeSphere;
	}
	int resourceCost() const override;
	inline CSSphereObject* createObject() const override {
		return new CSSphereObject(this);
	}
	void preload() const override;
};

#endif