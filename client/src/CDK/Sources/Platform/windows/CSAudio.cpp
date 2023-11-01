#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSAudio.h"
#include "CSAudioWave.h"
#include "CSAudioOgg.h"

#include "CSDictionary.h"
#include "CSThread.h"

#include <AL/alc.h>

static constexpr float UpdateInterval = 0.1f;

struct AudioContext {
	ALCcontext* context;
	CSDictionary<CSAudioControl, float> volumes;
	CSThread* thread;
	CSTaskBase* updateTask;
	int handleSeed;
};

static AudioContext* __context = NULL;

void CSAudio::initialize() {
	if (!__context) {
		const ALCchar* defaultDeviceName = alcGetString(NULL, ALC_DEFAULT_DEVICE_SPECIFIER);

		ALCdevice* device = alcOpenDevice(defaultDeviceName);

		if (device) {
			ALCcontext* context = alcCreateContext(device, NULL);

			if (context) {
				alcMakeContextCurrent(context);

				__context = new AudioContext();
				__context->context = context;
				__context->thread = new CSThread();
				__context->thread->start();

				waveInitialize();
				oggInitialize();

				auto inv = []() {
					waveUpdate();
					oggUpdate();
				};
				__context->updateTask = __context->thread->run<void>(inv, UpdateInterval, true);
				__context->handleSeed = 0;
			}
			else {
				alcCloseDevice(device);

				CSErrorLog("unable to create context:%s", defaultDeviceName);
			}
		}
		else CSErrorLog("unable to open device:%s", defaultDeviceName);
	}
}

void CSAudio::finalize() {
	if (__context) {
		__context->updateTask->stop();

		__context->thread->stop();
		__context->thread->release();

		waveFinalize();
		oggFinalize();

		ALCdevice* device = alcGetContextsDevice(__context->context);
		alcMakeContextCurrent(NULL);
		alcDestroyContext(__context->context);
		alcCloseDevice(device);

		delete __context;
		__context = NULL;
	}
}

void CSAudio::autoPause() {
	//nothing to do
}

void CSAudio::autoResume() {
	//nothing to do
}

int CSAudio::play(const char* path, float volume, CSAudioControl control, int loop, std::function<void(int)> stopcb) {
	int handle = ++__context->handleSeed;

	char* cpath = strdup(path);

	auto inv = [handle, cpath, volume, control, loop, stopcb]() {
		float mainVolume;
		if (!__context->volumes.tryGetObjectForKey(control, mainVolume)) mainVolume = 1;

		int pathlen = strlen(cpath);
		if (pathlen <= 3) {
			CSErrorLog("invalid path:%s", cpath);
			if (stopcb) stopcb(handle);
			return;
		}
		else if (stricmp(cpath + pathlen - 3, "wav") == 0) wavePlay(handle, cpath, mainVolume, volume, control, loop, stopcb);
		else if (stricmp(cpath + pathlen - 3, "ogg") == 0) oggPlay(handle, cpath, mainVolume, volume, control, loop, stopcb);

		free(cpath);
	};
	__context->thread->run<void>(inv);
	
	return handle;
}

bool CSAudio::isPlaying(int handle) {
	auto inv = [handle]() -> bool {
		return waveIsPlaying(handle) || oggIsPlaying(handle);
	};
	CSTask<bool>* task = __context->thread->run<bool>(inv);
	task->finish();
	return task->result();
}

void CSAudio::stop(int handle) {
	auto inv = [handle]() {
		waveStop(handle);
		oggStop(handle);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::pause(int handle) {
	auto inv = [handle]() {
		wavePause(handle);
		oggPause(handle);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::resume(int handle) {
	auto inv = [handle]() {
		waveResume(handle);
		oggResume(handle);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::setVolume(int handle, float volume) {
	auto inv = [handle, volume]() {
		waveSetVolume(handle, volume);
		oggSetVolume(handle, volume);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::setLoop(int handle, int loop) {
	auto inv = [handle, loop]() {
		waveSetLoop(handle, loop);
		oggSetLoop(handle, loop);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::setStopDelegate(int handle, std::function<void(int)> stopcb) {
	auto inv = [handle, stopcb]() {
		waveSetStopDelegate(handle, stopcb);
		oggSetStopDelegate(handle, stopcb);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::stopControl(CSAudioControl control) {
	if (!__context) return;

	auto inv = [control]() {
		waveStopControl(control);
		oggStopControl(control);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::pauseControl(CSAudioControl control) {
	if (!__context) return;

	auto inv = [control]() {
		wavePauseControl(control);
		oggPauseControl(control);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::resumeControl(CSAudioControl control) {
	if (!__context) return;

	auto inv = [control]() {
		waveResumeControl(control);
		oggResumeControl(control);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::setVolumeControl(CSAudioControl control, float volume) {
	auto inv = [control, volume]() {
		if (volume >= 1) __context->volumes.removeObject(control);
		else __context->volumes.setObject(control, volume);
		waveSetVolumeControl(control, volume);
		oggSetVolumeControl(control, volume);
	};
	__context->thread->run<void>(inv);
}

void CSAudio::vibrate(float time) {
	//nothing to do
}

#endif