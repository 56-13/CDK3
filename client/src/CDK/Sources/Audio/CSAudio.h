#ifndef __CDK__CSAudio__
#define __CDK__CSAudio__

#include "CSArray.h"

enum CSAudioControl : byte {
	CSAudioControlBgm,
	CSAudioControlEffect,
	CSAudioControlVoice,
	CSAudioControlCount
};

class CSAudio {
public:
#ifdef CDK_IMPL
	static void initialize();
	static void finalize();
	static void autoPause();
	static void autoResume();
#endif
#ifdef CDK_IOS
	static void restoreCategory();
#endif
	static int play(const char* path, float volume, CSAudioControl control, int loop, std::function<void(int)> stopcb = NULL);
	static bool isPlaying(int handle);
    static void stop(int handle);
    static void pause(int handle);
    static void resume(int handle);
	static void setVolume(int handle, float volume);
	static void setLoop(int handle, int loop);
	static void setStopDelegate(int handle, std::function<void(int)> stopcb);
	static void stopControl(CSAudioControl control);
	static void pauseControl(CSAudioControl control);
	static void resumeControl(CSAudioControl control);
    static void setVolumeControl(CSAudioControl control, float volume);
	static void vibrate(float time);
};

#endif
