#ifndef __CDK__CSDiagnostics__
#define __CDK__CSDiagnostics__

#include "CSString.h"
#include "CSArray.h"

class CSStopWatch {
#ifdef CS_DIAGNOSTICS
private:
	double _currentTime;
public:
	CSStopWatch();
	CSStopWatch(double currentTime);

	float elapsed();
	void printElapsed(const char* name);
#else
public:
	CSStopWatch() = default;
	inline CSStopWatch(double currentTime) {}

	inline float elapsed() {
		return 0;
	}
	inline void printElapsed(const char* name) {}
#endif
};

class CSDiagnostics {
public:
	struct TimeRecord {
		string name;
		double timeTotal;
		int64 timeCount;
	};
	struct RenderRecord {
		int originCommandCount = 0;
		int batchCommandCount = 0;
		int invokeCount = 0;
		int drawCount = 0;
		int vertexCount = 0;
	};
private:
#ifdef CS_DIAGNOSTICS
	CSArray<TimeRecord> _timeRecords;
	int _objectCount;
	RenderRecord _renderRecord[2];
	CSLock _lock;

	static CSDiagnostics __instance;

	inline CSDiagnostics() : _lock(true) {}
	~CSDiagnostics() = default;
public:
	static void timeReset();
	static void timeAdd(const string& name, float time, bool accum);
# ifdef CDK_IMPL
	static void objectNew();
	static void objectDelete();
	static void renderCommand(int originCommandCount, int batchCommandCount);
	static void renderInvoke(int invokeCount);
	static void renderDraw(int vertexCount);
	static void renderCycle();
# endif
	class Capture {
	public:
		inline Capture() {
			__instance._lock.lock();
		}
		inline ~Capture() {
			__instance._lock.unlock();
		}
		Capture(const Capture&) = delete;
		Capture& operator=(const Capture&) = delete;

		inline const CSArray<TimeRecord>* timeRecords() const {
			return &__instance._timeRecords;
		}
		inline int objectCount() const {
			return __instance._objectCount;
		}
		inline const RenderRecord* renderRecord() const {
			return &__instance._renderRecord[1];
		}
	};
#else
public:
	static inline void timeReset() {}
	static inline void timeAdd(const string& name, float time, bool accum) {}
# ifdef CDK_IMPL
	static inline void objectNew() {}
	static inline void objectDelete() {}
	static inline void renderCommand(int originCommandCount, int batchCommandCount) {}
	static inline void renderInvoke(int invokeCount) {}
	static inline void renderDraw(int vertexCount) {}
	static inline void renderCycle() {}
# endif
	class Capture {
	public:
		Capture(const Capture&) = delete;
		Capture& operator=(const Capture&) = delete;

		inline const CSArray<TimeRecord>* timeRecords() const {
			return NULL;
		}
		inline int objectCount() const {
			return 0;
		}
		inline const RenderRecord* renderRecord() const {
			return NULL;
		}
	};
#endif
};

#endif
