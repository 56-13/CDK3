#define CDK_IMPL

#include "CSMeshAnimation.h"

#include "CSBuffer.h"

CSMeshAnimation::VectorKey::VectorKey(CSBuffer* buffer) : time(buffer->readFloat()), value(buffer) {

}

CSMeshAnimation::QuaternionKey::QuaternionKey(CSBuffer* buffer) : time(buffer->readFloat()), value(buffer) {

}

CSMeshAnimation::Element::Element(CSBuffer* buffer) :
	positionKeys(retain(buffer->readArray<VectorKey>())),
	rotationKeys(retain(buffer->readArray<QuaternionKey>())),
	scalingKeys(retain(buffer->readArray<VectorKey>()))
{

}

CSMeshAnimation::Element::~Element() {
	release(positionKeys);
	release(rotationKeys);
	release(scalingKeys);
}

CSMeshAnimation::CSMeshAnimation(CSBuffer* buffer) : _name(buffer->readString()), _duration(buffer->readFloat()), _elements(0) {
	int elementCount = buffer->readLength();
	_elements.setCapacity(elementCount);
	for (int i = 0; i < elementCount; i++) {
		string name = buffer->readString();
		new (&_elements.setObject(name)) Element(buffer);
	}
}

int CSMeshAnimation::resourceCost() const {
	int cost = sizeof(CSMeshAnimation);
	cost += _name.resourceCost();
	cost += _elements.capacity() * (8 + sizeof(Element));
	for (CSDictionary<string, Element>::ReadonlyIterator i = _elements.iterator(); i.remaining(); i.next()) {
		cost += i.key().resourceCost();
		const Element& e = i.object();
		if (e.positionKeys) cost += sizeof(CSArray<VectorKey>) + e.positionKeys->capacity() * sizeof(VectorKey);
		if (e.rotationKeys) cost += sizeof(CSArray<QuaternionKey>) + e.rotationKeys->capacity() * sizeof(QuaternionKey);
		if (e.scalingKeys) cost += sizeof(CSArray<VectorKey>) + e.scalingKeys->capacity() * sizeof(VectorKey);
	}
	return cost;
}

CSVector3 CSMeshAnimation::getValue(float time, const CSArray<VectorKey>* keys) const {
	if (time <= keys->firstObject().time) return keys->firstObject().value;

	for (int i = 1; i < keys->count(); i++) {
		const VectorKey& k1 = keys->objectAtIndex(i);

		if (time < k1.time) {
			const VectorKey& k0 = keys->objectAtIndex(i - 1);

			float weight = (time - k0.time) / (k1.time - k0.time);
			weight = CSMath::smoothStep(weight);
			CSVector3 result = CSVector3::lerp(k0.value, k1.value, weight);
			return result;
		}
	}
	return keys->lastObject().value;
}

CSQuaternion CSMeshAnimation::getValue(float time, const CSArray<QuaternionKey>* keys) const {
	if (time <= keys->firstObject().time) return keys->firstObject().value;

	for (int i = 1; i < keys->count(); i++) {
		const QuaternionKey& k1 = keys->objectAtIndex(i);

		if (time < k1.time) {
			const QuaternionKey& k0 = keys->objectAtIndex(i - 1);

			float weight = (time - k0.time) / (k1.time - k0.time);
			CSQuaternion result = CSQuaternion::slerp(k0.value, k1.value, weight);
			return result;
		}
	}
	return keys->lastObject().value;
}

bool CSMeshAnimation::getNodeTransform(const string& name, float time, CSMatrix& result) const {
	const Element* e = _elements.tryGetObjectForKey(name);

	if (!e) return false;

	result = CSMatrix::Identity;

	if (e->scalingKeys) {
		CSVector3 scaling = getValue(time, e->scalingKeys);
		result.setScaleVector(scaling);
	}
	if (e->rotationKeys) {
		CSQuaternion rotation = getValue(time, e->rotationKeys);
		result *= CSMatrix::rotationQuaternion(rotation);
	}
	if (e->positionKeys) {
		CSVector3 position = getValue(time, e->positionKeys);
		result.setTranslationVector(position);
	}
	return true;
}
