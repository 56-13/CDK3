#ifdef CDK_WINDOWS

#define CDK_IMPL

#include "CSAudioOgg.h"

#include "CSArray.h"

#include <stdio.h>
#include <vorbis/vorbisfile.h>
#include <AL/al.h>

static size_t ov_read_func(void *ptr, size_t size, size_t nmemb, void *datasource) {
	return fread(ptr, size, nmemb, (FILE*)datasource);
}

static int ov_seek_func(void *datasource, ogg_int64_t offset, int whence) {
	return fseek((FILE*)datasource, (long)offset, whence);
}

static int ov_close_func(void *datasource) {
	return fclose((FILE*)datasource);
}

static long ov_tell_func(void *datasource) {
	return ftell((FILE*)datasource);
}

static constexpr int StreamBufferCount = 4;

struct Stream {
	OggVorbis_File* fp;
	void* decodeBuffer;
	uint decodeBufferSize;
	int channels;
	int frequency;
	int bufferFormat;
	uint buffers[StreamBufferCount];
	uint source;
	int handle;
	float mainVolume;
	float subVolume;
	CSAudioControl control;
	byte loopOrigin;
	byte loop;
	std::function<void(int)> stopcb;
};

static uint oggDecode(Stream* stream) {
	int selection;

	unsigned int decodeLen = 0;

	for (;;) {
		int len = ov_read(stream->fp, (char*)stream->decodeBuffer + decodeLen, stream->decodeBufferSize - decodeLen, 0, 2, 1, &selection);
		if (len > 0) {
			decodeLen += len;

			if (decodeLen >= stream->decodeBufferSize) break;
		}
		else if (decodeLen == 0) {
			if (stream->loop == 1) break;
			else {
				ov_raw_seek(stream->fp, 0);
				if (stream->loop > 1) stream->loop--;
			}
		}
		else break;
	}

	if (stream->channels == 6) {
		short* samples = (short*)stream->decodeBuffer;
		for (int ulSamples = 0; ulSamples < (stream->decodeBufferSize >> 1); ulSamples += 6) {
			CSMath::swap(samples[ulSamples + 1], samples[ulSamples + 2]);
			CSMath::swap(samples[ulSamples + 3], samples[ulSamples + 5]);
			CSMath::swap(samples[ulSamples + 4], samples[ulSamples + 5]);
		}
	}
	return decodeLen;
}

static Stream* oggOpen(const char* path) {
	FILE* fp = fopen(path, "rb");

	if (!fp) return NULL;

	ov_callbacks callbacks;
	OggVorbis_File*	ovfp = new OggVorbis_File;

	callbacks.read_func = ov_read_func;
	callbacks.seek_func = ov_seek_func;
	callbacks.close_func = ov_close_func;
	callbacks.tell_func = ov_tell_func;

	if (ov_open_callbacks(fp, ovfp, NULL, 0, callbacks)) {
		fclose(fp);
		delete ovfp;
		return NULL;
	}

	vorbis_info* vorbisInfo = ov_info(ovfp, -1);

	if (!vorbisInfo) {
		ov_clear(ovfp);
		delete ovfp;
		return NULL;
	}

	int bufferFormat;
	int decodeBufferSize;
	int frequency = vorbisInfo->rate;

	if (vorbisInfo->channels == 1) {
		bufferFormat = AL_FORMAT_MONO16;
		decodeBufferSize = frequency >> 1;
		decodeBufferSize -= (decodeBufferSize % 2);
	}
	else if (vorbisInfo->channels == 2) {
		bufferFormat = AL_FORMAT_STEREO16;
		decodeBufferSize = frequency;
		decodeBufferSize -= (decodeBufferSize % 4);
	}
	else if (vorbisInfo->channels == 4) {
		bufferFormat = alGetEnumValue("AL_FORMAT_QUAD16");
		decodeBufferSize = frequency * 2;
		decodeBufferSize -= (decodeBufferSize % 8);
	}
	else if (vorbisInfo->channels == 6) {
		bufferFormat = alGetEnumValue("AL_FORMAT_51CHN16");
		decodeBufferSize = frequency * 3;
		decodeBufferSize -= (decodeBufferSize % 12);
	}
	else {
		ov_clear(ovfp);
		delete ovfp;
		return NULL;
	}

	void* decodeBuffer = malloc(decodeBufferSize);

	if (!decodeBuffer) {
		ov_clear(ovfp);
		delete ovfp;
		return NULL;
	}

	Stream* stream = new Stream;
	stream->fp = ovfp;
	stream->decodeBuffer = decodeBuffer;
	stream->decodeBufferSize = decodeBufferSize;
	stream->bufferFormat = bufferFormat;
	stream->frequency = frequency;
	alGenBuffers(StreamBufferCount, stream->buffers);
	alGenSources(1, &stream->source);

	for (int i = 0; i < StreamBufferCount; i++) {
		uint len = oggDecode(stream);
		if (len) {
			alBufferData(stream->buffers[i], stream->bufferFormat, stream->decodeBuffer, len, stream->frequency);
			alSourceQueueBuffers(stream->source, 1, &stream->buffers[i]);
		}
	}

	return stream;
}

static void oggClose(Stream* stream) {
	if (stream->stopcb) stream->stopcb(stream->handle);

	alSourceStop(stream->source);
	alDeleteSources(1, &stream->source);
	alDeleteBuffers(StreamBufferCount, stream->buffers);

	ov_clear(stream->fp);
	delete stream->fp;

	free(stream->decodeBuffer);

	delete stream;
}

static bool oggUpdate(Stream* stream) {
	int buffersProcessed = 0;
	alGetSourcei(stream->source, AL_BUFFERS_PROCESSED, &buffersProcessed);
	while (buffersProcessed) {
		uint buffer = 0;
		alSourceUnqueueBuffers(stream->source, 1, &buffer);
		uint len = oggDecode(stream);
		if (len) {
			alBufferData(buffer, stream->bufferFormat, stream->decodeBuffer, len, stream->frequency);
			alSourceQueueBuffers(stream->source, 1, &buffer);
		}
		buffersProcessed--;
	}
	int state;
	alGetSourcei(stream->source, AL_SOURCE_STATE, &state);
	if (state != AL_PLAYING && state != AL_PAUSED) {
		int queuedBuffer;
		alGetSourcei(stream->source, AL_BUFFERS_QUEUED, &queuedBuffer);
		if (queuedBuffer) alSourcePlay(stream->source);
		else return false;
	}
	return true;
}


//=============================================================================

static CSArray<Stream*> __streams;

void oggInitialize() {
	//nothing to do
}

void oggFinalize() {
	foreach (Stream*, stream, &__streams) oggClose(stream);
}

void oggPlay(int handle, const char* path, float mainVolume, float subVolume, CSAudioControl control, int loop, const std::function<void(int)>& stopcb) {
	Stream* stream = oggOpen(path);

	if (!stream) {
		CSErrorLog("unable to play:%s", path);
		if (stopcb) stopcb(handle);
		return;
	}

	alSourcef(stream->source, AL_GAIN, mainVolume * subVolume);
	alSourcePlay(stream->source);

	stream->handle = handle;
	stream->mainVolume = mainVolume;
	stream->subVolume = subVolume;
	stream->control = control;
	stream->loopOrigin = stream->loop = loop;
	stream->stopcb = stopcb;
	__streams.addObject(stream);
}

bool oggIsPlaying(int handle) {
	foreach (const Stream*, stream, &__streams) {
		if (stream->handle == handle) return true;
	}
	return false;
}

void oggStop(int handle) {
	int i = 0;
	while (i < __streams.count()) {
		Stream* stream = __streams.objectAtIndex(i);

		if (stream->handle == handle) {
			oggClose(stream);
			__streams.removeObjectAtIndex(i);
			break;
		}
		else i++;
	}
}

void oggPause(int handle) {
	foreach (Stream*, stream, &__streams) {
		if (stream->handle == handle) {
			alSourcePause(stream->source);
			break;
		}
	}
}

void oggResume(int handle) {
	foreach (Stream*, stream, &__streams) {
		if (stream->handle == handle) {
			alSourcePlay(stream->source);
			break;
		}
	}
}

void oggSetVolume(int handle, float volume) {
	foreach (Stream*, stream, &__streams) {
		if (stream->handle == handle) {
			alSourcef(stream->source, AL_GAIN, stream->mainVolume * volume);
			break;
		}
	}
}

void oggSetLoop(int handle, int loop) {
	foreach (Stream*, stream, &__streams) {
		if (stream->handle == handle) {
			if (loop > 0) {
				stream->loop = CSMath::max(1, loop - stream->loopOrigin + stream->loop);
				stream->loopOrigin = loop;
			}
			else {
				stream->loopOrigin = stream->loop = 0;
			}
			break;
		}
	}
}

void oggSetStopDelegate(int handle, std::function<void(int)> stopcb) {
	foreach (Stream*, stream, &__streams) {
		if (stream->handle == handle) {
			stream->stopcb = stopcb;
			break;
		}
	}
}

void oggStopControl(CSAudioControl control) {
	int i = 0;
	while (i < __streams.count()) {
		Stream* stream = __streams.objectAtIndex(i);

		if (stream->control == control) {
			oggClose(stream);
			__streams.removeObjectAtIndex(i);
		}
		else i++;
	}
}

void oggPauseControl(CSAudioControl control) {
	foreach (Stream*, stream, &__streams) {
		if (stream->control == control) alSourcePause(stream->source);
	}
}

void oggResumeControl(CSAudioControl control) {
	foreach (Stream*, stream, &__streams) {
		if (stream->control == control) alSourcePlay(stream->source);
	}
}

void oggSetVolumeControl(CSAudioControl control, float volume) {
	foreach (Stream*, stream, &__streams) {
		if (stream->control == control) alSourcef(stream->source, AL_GAIN, volume * stream->subVolume);
	}
}

void oggUpdate() {
	int i = 0;
	while (i < __streams.count()) {
		Stream* stream = __streams.objectAtIndex(i);

		if (oggUpdate(stream)) i++;
		else {
			oggClose(stream);
			__streams.removeObjectAtIndex(i);
		}
	}
}

#endif