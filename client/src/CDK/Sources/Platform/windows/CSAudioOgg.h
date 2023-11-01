#ifdef CDK_WINDOWS

#ifdef CDK_IMPL

#ifndef __CDK__CSAudioOgg__
#define __CDK__CSAudioOgg__

#include "CSAudio.h"

void oggInitialize();
void oggFinalize();
void oggPlay(int handle, const char* path, float mainVolume, float subVolume, CSAudioControl control, int loop, const std::function<void(int)>& stopcb);
bool oggIsPlaying(int handle);
void oggStop(int handle);
void oggPause(int handle);
void oggResume(int handle);
void oggSetVolume(int handle, float volume);
void oggSetLoop(int handle, int loop);
void oggSetStopDelegate(int handle, std::function<void(int)> stopcb);
void oggStopControl(CSAudioControl control);
void oggPauseControl(CSAudioControl control);
void oggResumeControl(CSAudioControl control);
void oggSetVolumeControl(CSAudioControl control, float volume);
void oggUpdate();

#endif

#endif

#endif
