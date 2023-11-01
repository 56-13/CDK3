#ifdef CDK_WINDOWS

#ifdef CDK_IMPL

#ifndef __CDK__CSAudioWave__
#define __CDK__CSAudioWave__

#include "CSAudio.h"

void waveInitialize();
void waveFinalize();
void wavePlay(int handle, const char* path, float mainVolume, float subVolume, CSAudioControl control, int loop, const std::function<void(int)>& stopcb);
bool waveIsPlaying(int handle);
void waveStop(int handle);
void wavePause(int handle);
void waveResume(int handle);
void waveSetVolume(int handle, float volume);
void waveSetLoop(int handle, int loop);
void waveSetStopDelegate(int handle, std::function<void(int)> stopcb);
void waveStopControl(CSAudioControl control);
void wavePauseControl(CSAudioControl control);
void waveResumeControl(CSAudioControl control);
void waveSetVolumeControl(CSAudioControl control, float volume);
void waveUpdate();

#endif

#endif

#endif