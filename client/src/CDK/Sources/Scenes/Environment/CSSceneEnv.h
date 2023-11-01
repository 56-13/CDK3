#ifndef __CDK__CSSceneEnv__
#define __CDK__CSSceneEnv__

#include "CSAudio.h"

#include "CSSceneObject.h"

struct CSSceneEnv {
	struct Lighting {
		bool enabled = true;
		bool fog = false;
		CSColor3 fogColor = CSColor3::White;
		float fogNear = 100;
		float fogFar = 10000;
		CSColor3 ambientLight = 0x4C4C4CFF;
		CSPtr<const CSArray<ushort>> skyboxIndices;
		CSColor3 skyboxColor = CSColor3::Black;
		
		Lighting() = default;
		explicit Lighting(CSBuffer* buffer);

		const CSTexture* skyboxTexture() const;
	};
	struct PostProcess {
		bool enabled = true;
		byte bloomPass = 4;
		float bloomIntensity = 1;
		float bloomThreshold = 1;
		float exposure = 2;
		float gamma = 1.6f;

		PostProcess() = default;
		explicit PostProcess(CSBuffer* buffer);
	};
	struct Sound {
		float maxDistance = 16.0f;
		float refDistance = 2.0f;
		float rollOffFactor = 0.75f;
		float duplication = 0.2f;
		byte capacity[CSAudioControlCount] = { 0, 8, 1 };

		Sound() = default;
		explicit Sound(CSBuffer* buffer);
	};
	struct Camera {
		float fov = 60 * FloatToRadians;
		float znear = 10;
		float zfar = 10000;
		float angle = 45 * FloatToRadians;
		float distance = 1000;

		Camera() = default;
		explicit Camera(CSBuffer* buffer);
	};
	Lighting lighting;
	CSArray<CSSceneObjectBuilder> props;
	PostProcess postprocess;
	Sound sound;
	Camera camera;
	bool quadTree = false;
	
	CSSceneEnv() = default;
	CSSceneEnv(const CSSceneEnv& other);
	CSSceneEnv(CSBuffer* buffer);

	CSSceneEnv& operator =(const CSSceneEnv& other);
};

#endif