#ifndef __CDK__CSMeshAnimation__
#define __CDK__CSMeshAnimation__

#include "CSString.h"
#include "CSDictionary.h"
#include "CSMatrix.h"

class CSBuffer;

class CSMeshAnimation : public CSObject {
private:
	struct VectorKey {
		float time;
		CSVector3 value;

		VectorKey(CSBuffer* buffer);
	};
	struct QuaternionKey {
		float time;
		CSQuaternion value;

		QuaternionKey(CSBuffer* buffer);
	};
	struct Element {
		CSArray<VectorKey>* positionKeys;
		CSArray<QuaternionKey>* rotationKeys;
		CSArray<VectorKey>* scalingKeys;

		Element(CSBuffer* buffer);
		~Element();

		Element(const Element&) = delete;
		Element& operator=(const Element&) = delete;
	};
	string _name;
	float _duration;
	CSDictionary<string, Element> _elements;
public:
	CSMeshAnimation(CSBuffer* buffer);
private:
	~CSMeshAnimation() = default;
public:
	static inline CSMeshAnimation* animationWithBuffer(CSBuffer* buffer) {
		return autorelease(new CSMeshAnimation(buffer));
	}

	int resourceCost() const;

	inline const string& name() const {
		return _name;
	}
	inline float duration() const {
		return _duration;
	}
	bool getNodeTransform(const string& name, float time, CSMatrix& result) const;
private:
	CSVector3 getValue(float time, const CSArray<VectorKey>* keys) const;
	CSQuaternion getValue(float time, const CSArray<QuaternionKey>* keys) const;
};

#endif