#ifndef __CDK__CSCapsuleObject__
#define __CDK__CSCapsuleObject__

#include "CSSceneObject.h"
#include "CSMaterialSource.h"

class CSCapsuleBuilder;

class CSCapsuleObject : public CSSceneObject {
private:
	const CSCapsuleBuilder* _origin;
	float _progress;
	int _random;
public:
	CSCapsuleObject(const CSCapsuleBuilder* origin);
private:
	~CSCapsuleObject();
public:
	static inline CSCapsuleObject* object(const CSCapsuleBuilder* origin) {
		return autorelease(new CSCapsuleObject(origin));
	}

	inline Type type() const override {
		return TypeCapsule;
	}
	inline const CSCapsuleBuilder* origin() const {
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

class CSCapsuleBuilder : public CSSceneObjectBuilder {
public:
	float height = 100;
	float radius = 100;
	bool collision = false;
	CSPtr<CSMaterialSource> material;

	CSCapsuleBuilder() = default;
	CSCapsuleBuilder(CSBuffer* buffer, bool withScene);
	CSCapsuleBuilder(const CSCapsuleBuilder* other);
private:
	~CSCapsuleBuilder() = default;
public:
	static inline CSCapsuleBuilder* builder() {
		return autorelease(new CSCapsuleBuilder());
	}
	static inline CSCapsuleBuilder* builderWithBuilder(const CSCapsuleBuilder* other) {
		return autorelease(new CSCapsuleBuilder(other));
	}

	inline CSSceneObject::Type type() const override {
		return CSSceneObject::TypeCapsule;
	}
	int resourceCost() const override;
	inline CSCapsuleObject* createObject() const override {
		return new CSCapsuleObject(this);
	}
	void preload() const override;
};

#endif