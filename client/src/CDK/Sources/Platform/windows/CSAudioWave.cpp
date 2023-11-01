#ifdef CDK_WINDOWS

#define CDK_IMPL

//TODO:purge가 필요하다면 추후 추가, 현재는 무제한

#include "CSAudioWave.h"

#include "CSArray.h"
#include "CSTime.h"

#include <windows.h>
#include <ks.h>
#include <ksmedia.h>
#include <stdio.h>

#include <AL/al.h>

enum WAVEFILETYPE {
	WF_EX = 1,
	WF_EXT = 2
};

#ifndef _WAVEFORMATEX_
#define _WAVEFORMATEX_
typedef struct tWAVEFORMATEX {
	WORD    wFormatTag;
	WORD    nChannels;
	DWORD   nSamplesPerSec;
	DWORD   nAvgBytesPerSec;
	WORD    nBlockAlign;
	WORD    wBitsPerSample;
	WORD    cbSize;
} WAVEFORMATEX;
#endif /* _WAVEFORMATEX_ */

#ifndef _WAVEFORMATEXTENSIBLE_
#define _WAVEFORMATEXTENSIBLE_
typedef struct {
	WAVEFORMATEX    Format;
	union {
		WORD wValidBitsPerSample;       /* bits of precision  */
		WORD wSamplesPerBlock;          /* valid if wBitsPerSample==0 */
		WORD wReserved;                 /* If neither applies, set to zero. */
	} Samples;
	DWORD           dwChannelMask;      /* which channels are */
										/* present in stream  */
	GUID            SubFormat;
} WAVEFORMATEXTENSIBLE, *PWAVEFORMATEXTENSIBLE;
#endif // !_WAVEFORMATEXTENSIBLE_

typedef struct {
	WAVEFILETYPE	wfType;
	WAVEFORMATEXTENSIBLE wfEXT;		// For non-WAVEFORMATEXTENSIBLE wavefiles, the header is stored in the Format member of wfEXT
	char			*pData;
	unsigned long	ulDataSize;
} WAVEFILEINFO, *LPWAVEFILEINFO;

#pragma pack(push, 4)

typedef struct
{
	char			szRIFF[4];
	unsigned long	ulRIFFSize;
	char			szWAVE[4];
} WAVEFILEHEADER;

typedef struct
{
	char			szChunkName[4];
	unsigned long	ulChunkSize;
} RIFFCHUNK;

typedef struct
{
	unsigned short	usFormatTag;
	unsigned short	usChannels;
	unsigned long	ulSamplesPerSec;
	unsigned long	ulAvgBytesPerSec;
	unsigned short	usBlockAlign;
	unsigned short	usBitsPerSample;
	unsigned short	usSize;
	unsigned short  usReserved;
	unsigned long	ulChannelMask;
	GUID            guidSubFormat;
} WAVEFMT;

#pragma pack(pop)

static LPWAVEFILEINFO loadInfo(const char* szFilename) {
	FILE* fp = fopen(szFilename, "rb");

	if (fp) {
		WAVEFILEHEADER header;

		fread(&header, 1, sizeof(WAVEFILEHEADER), fp);

		if (!_strnicmp(header.szRIFF, "RIFF", 4) && !_strnicmp(header.szWAVE, "WAVE", 4)) {
			RIFFCHUNK riffChunk;

			WAVEFMT fmt;

			unsigned int dataOffset = 0;

			LPWAVEFILEINFO wave = new WAVEFILEINFO;
			
			memset(wave, 0, sizeof(WAVEFILEINFO));

			while (fread(&riffChunk, 1, sizeof(RIFFCHUNK), fp) == sizeof(RIFFCHUNK)) {
				if (!_strnicmp(riffChunk.szChunkName, "fmt ", 4)) {
					if (riffChunk.ulChunkSize <= sizeof(WAVEFMT)) {
						fread(&fmt, 1, riffChunk.ulChunkSize, fp);

						if (fmt.usFormatTag == WAVE_FORMAT_PCM) { 
							wave->wfType = WF_EX;
							memcpy(&wave->wfEXT.Format, &fmt, sizeof(PCMWAVEFORMAT));
						}
						else if (fmt.usFormatTag == WAVE_FORMAT_EXTENSIBLE)	{
							wave->wfType = WF_EXT;
							memcpy(&wave->wfEXT, &fmt, sizeof(WAVEFORMATEXTENSIBLE));
						}
					}
					else fseek(fp, riffChunk.ulChunkSize, SEEK_CUR);
				}
				else if (!_strnicmp(riffChunk.szChunkName, "data", 4)) {
					wave->ulDataSize = riffChunk.ulChunkSize;
					dataOffset = ftell(fp);
					fseek(fp, riffChunk.ulChunkSize, SEEK_CUR);
				}
				else fseek(fp, riffChunk.ulChunkSize, SEEK_CUR);
				
				if (riffChunk.ulChunkSize & 1) fseek(fp, 1, SEEK_CUR);
			}

			if (wave->ulDataSize && dataOffset && ((wave->wfType == WF_EX) || (wave->wfType == WF_EXT))) {
				wave->pData = new char[wave->ulDataSize];
				if (wave->pData) {
					fseek(fp, dataOffset, SEEK_SET);

					if (fread(wave->pData, 1, wave->ulDataSize, fp) == wave->ulDataSize) {
						fclose(fp);
						return wave;
					}
					else free(wave->pData);
				}
			}

			delete wave;
		}

		fclose(fp);
	}
	
	return NULL;
}

static void releaseInfo(LPWAVEFILEINFO wave) {
	free(wave->pData);
	delete wave;
}

static int getALBufferFormat(LPWAVEFILEINFO wave) {
	int format = 0;
	
	if (wave->wfType == WF_EX) {
		if (wave->wfEXT.Format.nChannels == 1) {
			switch (wave->wfEXT.Format.wBitsPerSample) {
				case 4:
					return alGetEnumValue("AL_FORMAT_MONO_IMA4");
				case 8:
					return AL_FORMAT_MONO8;
				case 16:
					return AL_FORMAT_MONO16;
			}
		}
		else if (wave->wfEXT.Format.nChannels == 2)	{
			switch (wave->wfEXT.Format.wBitsPerSample)	{
				case 4:
					return alGetEnumValue("AL_FORMAT_STEREO_IMA4");
				case 8:
					return AL_FORMAT_STEREO8;
				case 16:
					return AL_FORMAT_STEREO16;
			}
		}
		else if ((wave->wfEXT.Format.nChannels == 4) && (wave->wfEXT.Format.wBitsPerSample == 16)) {
			return alGetEnumValue("AL_FORMAT_QUAD16");
		}
	}
	else if (wave->wfType == WF_EXT) {
		if ((wave->wfEXT.Format.nChannels == 1) &&
			((wave->wfEXT.dwChannelMask == SPEAKER_FRONT_CENTER) ||
			(wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT)) ||
				(wave->wfEXT.dwChannelMask == 0)))
		{
			switch (wave->wfEXT.Format.wBitsPerSample) {
				case 4:
					return alGetEnumValue("AL_FORMAT_MONO_IMA4");
					break;
				case 8:
					return AL_FORMAT_MONO8;
				case 16:
					return AL_FORMAT_MONO16;
			}
		}
		else if ((wave->wfEXT.Format.nChannels == 2) && (wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT))) {
			switch (wave->wfEXT.Format.wBitsPerSample) {
				case 4:
					return alGetEnumValue("AL_FORMAT_STEREO_IMA4");
				case 8:
					return AL_FORMAT_STEREO8;
				case 16:
					return AL_FORMAT_STEREO16;
			}
		}
		else if ((wave->wfEXT.Format.nChannels == 2) && (wave->wfEXT.Format.wBitsPerSample == 16) && (wave->wfEXT.dwChannelMask == (SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
			return alGetEnumValue("AL_FORMAT_REAR16");
		else if ((wave->wfEXT.Format.nChannels == 4) && (wave->wfEXT.Format.wBitsPerSample == 16) && (wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
			return alGetEnumValue("AL_FORMAT_QUAD16");
		else if ((wave->wfEXT.Format.nChannels == 6) && (wave->wfEXT.Format.wBitsPerSample == 16) && (wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT)))
			return alGetEnumValue("AL_FORMAT_51CHN16");
		else if ((wave->wfEXT.Format.nChannels == 7) && (wave->wfEXT.Format.wBitsPerSample == 16) && (wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT | SPEAKER_BACK_CENTER)))
			return alGetEnumValue("AL_FORMAT_61CHN16");
		else if ((wave->wfEXT.Format.nChannels == 8) && (wave->wfEXT.Format.wBitsPerSample == 16) && (wave->wfEXT.dwChannelMask == (SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT | SPEAKER_FRONT_CENTER | SPEAKER_LOW_FREQUENCY | SPEAKER_BACK_LEFT | SPEAKER_BACK_RIGHT | SPEAKER_SIDE_LEFT | SPEAKER_SIDE_RIGHT)))
			return alGetEnumValue("AL_FORMAT_71CHN16");
	}

	return 0;
}

static uint loadBuffer(const char* path) {
	LPWAVEFILEINFO wave = loadInfo(path);

	if (!wave) {
		CSErrorLog("unable to load wave:%s", path);
		return 0;
	}

	uint obj;
	alGetError();
	alGenBuffers(1, &obj);
	alBufferData(obj, getALBufferFormat(wave), wave->pData, wave->ulDataSize, wave->wfEXT.Format.nSamplesPerSec);

	releaseInfo(wave);

	return obj;
}

//==================================================================================
struct Buffer {
	char* path;
	uint object;
	int retainCount;
	double timestamp;
};

struct Stream {
	int handle;
	uint object;
	uint buffer;
	float mainVolume;
	float subVolume;
	CSAudioControl control;
	byte loopOrigin;
	byte loop;
	std::function<void(int)> stopcb;
};

static constexpr float BufferLifeTime = 120;

static CSArray<Buffer> __buffers;
static CSArray<Stream> __streams;

static void releaseStream(Stream& stream) {
	if (stream.stopcb) stream.stopcb(stream.handle);
	foreach (Buffer&, buffer, &__buffers) {
		if (buffer.object == stream.buffer) {
			if (--buffer.retainCount == 0) buffer.timestamp = CSTime::currentTime();
			break;
		}
	}
	alSourceStop(stream.object);
	alDeleteSources(1, &stream.object);
}

void waveInitialize() {
	//nothing to do
}

void waveFinalize() {
	foreach (Stream&, stream, &__streams) {
		alSourceStop(stream.object);
		alDeleteSources(1, &stream.object);
	}
	foreach (Buffer&, buffer, &__buffers) {
		alDeleteBuffers(1, &buffer.object);
		free(buffer.path);
	}
}

void wavePlay(int handle, const char* path, float mainVolume, float subVolume, CSAudioControl control, bool loop, const std::function<void(int)>& stopcb) {
	uint bufobj = 0;
	foreach (Buffer&, other, &__buffers) {
		if (stricmp(other.path, path) == 0) {
			bufobj = other.object;
			other.retainCount++;
			break;
		}
	}
	if (!bufobj) {
		bufobj = loadBuffer(path);

		Buffer& buffer = __buffers.addObject();
		buffer.path = strdup(path);
		buffer.object = bufobj;
		buffer.retainCount = 1;
	}

	uint srcobj;
	alGenSources(1, &srcobj);
	alSourcei(srcobj, AL_BUFFER, bufobj);
	alSourcef(srcobj, AL_GAIN, mainVolume * subVolume);
	alSourcePlay(srcobj);

	Stream& stream = __streams.addObject();
	stream.handle = handle;
	stream.buffer = bufobj;
	stream.object = srcobj;
	stream.mainVolume = mainVolume;
	stream.subVolume = subVolume;
	stream.control = control;
	stream.loopOrigin = stream.loop = loop;
	new (&stream.stopcb) std::function<void(int)>(stopcb);
}

bool waveIsPlaying(int handle) {
	foreach (const Stream&, stream, &__streams) {
		if (stream.handle == handle) return true;
	}
	return false;
}

void waveStop(int handle) {
	int i = 0;
	while (i < __streams.count()) {
		Stream& stream = __streams.objectAtIndex(i);

		if (stream.handle == handle) {
			releaseStream(stream);
			__streams.removeObjectAtIndex(i);
			break;
		}
		else i++;
	}
}

void wavePause(int handle) {
	foreach (Stream&, stream, &__streams) {
		if (stream.handle == handle) {
			alSourcePause(stream.object);
			break;
		}
	}
}

void waveResume(int handle) {
	foreach (Stream&, stream, &__streams) {
		if (stream.handle == handle) {
			alSourcePlay(stream.object);
			break;
		}
	}
}

void waveSetVolume(int handle, float volume) {
	foreach (Stream&, stream, &__streams) {
		if (stream.handle == handle) {
			alSourcef(stream.object, AL_GAIN, stream.mainVolume * volume);
			break;
		}
	}
}


void waveSetLoop(int handle, int loop) {
	foreach (Stream&, stream, &__streams) {
		if (stream.handle == handle) {
			if (loop > 0) {
				stream.loop = CSMath::max(1, loop - stream.loopOrigin + stream.loop);
				stream.loopOrigin = loop;
			}
			else {
				stream.loopOrigin = stream.loop = 0;
			}
			break;
		}
	}
}

void waveSetStopDelegate(int handle, std::function<void(int)> stopcb) {
	foreach (Stream&, stream, &__streams) {
		if (stream.handle == handle) {
			stream.stopcb = stopcb;
			break;
		}
	}
}

void waveStopControl(CSAudioControl control) {
	int i = 0;
	while (i < __streams.count()) {
		Stream& stream = __streams.objectAtIndex(i);

		if (stream.control == control) {
			releaseStream(stream);
			__streams.removeObjectAtIndex(i);
		}
		else i++;
	}
}

void wavePauseControl(CSAudioControl control) {
	foreach (Stream&, stream, &__streams) {
		if (stream.control == control) alSourcePause(stream.object);
	}
}

void waveResumeControl(CSAudioControl control) {
	foreach (Stream&, stream, &__streams) {
		if (stream.control == control) alSourcePlay(stream.object);
	}
}

void waveSetVolumeControl(CSAudioControl control, float volume) {
	foreach (Stream&, stream, &__streams) {
		if (stream.control == control) alSourcef(stream.object, AL_GAIN, volume * stream.subVolume);
	}
}

void waveUpdate() {
	int i = 0;
	while (i < __streams.count()) {
		Stream& stream = __streams.objectAtIndex(i);

		int state;
		alGetSourcei(stream.object, AL_SOURCE_STATE, &state);
		if (state != AL_PLAYING && state != AL_PAUSED) {
			if (stream.loop == 1) {
				releaseStream(stream);
				__streams.removeObjectAtIndex(i);
			}
			else {
				alSourcePlay(stream.object);
				if (stream.loop > 1) stream.loop--;
			}
		}
		else i++;
	}

	double currentTime = CSTime::currentTime();
	i = 0;
	while (i < __buffers.count()) {
		Buffer& buffer = __buffers.objectAtIndex(i);
		if (buffer.retainCount == 0 && currentTime - buffer.timestamp > BufferLifeTime) {
			alDeleteBuffers(1, &buffer.object);
			free(buffer.path);
			__buffers.removeObjectAtIndex(i);
		}
		else i++;
	}
}

#endif