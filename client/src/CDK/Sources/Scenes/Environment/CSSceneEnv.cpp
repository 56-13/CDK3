#define CDK_IMPL

#include "CSSceneEnv.h"
#include "CSResourcePool.h"
#include "CSBuffer.h"

CSSceneEnv::Lighting::Lighting(CSBuffer* buffer) {
	enabled = buffer->readBoolean();
	fog = buffer->readBoolean();

	if (fog) {
		fogColor = CSColor3(buffer);
		fogNear = buffer->readFloat();
		fogFar = buffer->readFloat();
	}

	if (enabled) ambientLight = CSColor3(buffer);
	skyboxIndices = buffer->readArray<ushort>();
	skyboxColor = CSColor3(buffer);
}

CSSceneEnv::PostProcess::PostProcess(CSBuffer* buffer) {
	enabled = buffer->readBoolean();

	if (enabled) {
		bloomPass = buffer->readByte();
		bloomIntensity = buffer->readFloat();
		bloomThreshold = buffer->readFloat();
		exposure = buffer->readFloat();
	}
}

const CSTexture* CSSceneEnv::Lighting::skyboxTexture() const {
	return static_assert_cast<CSTexture*>(CSResourcePool::sharedPool()->load(CSResourceTypeTexture, skyboxIndices));
}

CSSceneEnv::Sound::Sound(CSBuffer* buffer) :
	maxDistance(buffer->readFloat()),
	refDistance(buffer->readFloat()),
	rollOffFactor(buffer->readFloat()),
	duplication(buffer->readFloat()) 
{
	for (int i = 0; i < CSAudioControlCount; i++) capacity[i] = buffer->readByte();
}

CSSceneEnv::Camera::Camera(CSBuffer* buffer) :
	fov(buffer->readFloat()),
	znear(buffer->readFloat()),
	zfar(buffer->readFloat()),
	angle(buffer->readFloat()),
	distance(buffer->readFloat())
{
}

CSSceneEnv::CSSceneEnv(CSBuffer* buffer) :
	lighting(buffer),
	props(buffer, [](CSBuffer* buffer) -> CSSceneObjectBuilder* { return CSSceneObjectBuilder::builderWithBuffer(buffer, true);  }),
	postprocess(buffer), 
	sound(buffer),
	camera(buffer),
	quadTree(buffer->readBoolean())
{

}

CSSceneEnv::CSSceneEnv(const CSSceneEnv& other) :
	lighting(other.lighting),
	props(other.props.capacity()),
	postprocess(other.postprocess),
	sound(other.sound),
	camera(other.camera),
	quadTree(other.quadTree)
{
	foreach (const CSSceneObjectBuilder*, prop, &other.props) {
		CSSceneObjectBuilder* copy = prop->createWithBuilder(prop);
		props.addObject(copy);
		copy->release();
	}
}

CSSceneEnv& CSSceneEnv::operator=(const CSSceneEnv& other) {
	lighting = other.lighting;
	props.removeAllObjects();
	foreach (const CSSceneObjectBuilder*, prop, &other.props) {
		CSSceneObjectBuilder* copy = prop->createWithBuilder(prop);
		props.addObject(copy);
		copy->release();
	}
	sound = other.sound;
	camera = other.camera;
	quadTree = other.quadTree;
	return *this;
}
