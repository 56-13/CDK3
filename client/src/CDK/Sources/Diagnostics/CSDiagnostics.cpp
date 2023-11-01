#define CDK_IMPL

#include "CSDiagnostics.h"
#include "CSTime.h"
#include "CSThread.h"

#ifdef CS_DIAGNOSTICS

CSStopWatch::CSStopWatch() : _currentTime(CSTime::currentTime()) {

}

CSStopWatch::CSStopWatch(double currentTime) : _currentTime(currentTime) {

}

float CSStopWatch::elapsed() {
	double currentTime = CSTime::currentTime();
	float delay = currentTime - _currentTime;
	_currentTime = currentTime;
	return delay;
}

void CSStopWatch::printElapsed(const char* name) {
	CSLog("StopWatch:%s:%f", elapsed());
}

//=====================================================================================
void CSDiagnostics::timeReset() {
	synchronized(__instance._lock) {
		__instance._timeRecords.removeAllObjects();
	}
}

void CSDiagnostics::timeAdd(const string& name, float time, bool accum) {
	synchronized(__instance._lock) {
		foreach (TimeRecord&, record, &__instance._timeRecords) {
			if (record.name == name) {
				record.timeTotal += time;
				if (!accum) record.timeCount++;
				return;
			}
		}
		{
			TimeRecord& record = __instance._timeRecords.addObject();
			record.name = name;
			record.timeTotal = time;
			record.timeCount = 1;
		}
	}
}

void CSDiagnostics::objectNew() {
	synchronized(__instance._lock) {
		__instance._objectCount++;
	}
}

void CSDiagnostics::objectDelete() {
	synchronized(__instance._lock) {
		__instance._objectCount--;
	}
}

void CSDiagnostics::renderCommand(int originCommandCount, int batchCommandCount) {
	synchronized(__instance._lock) {
		__instance._renderRecord[0].originCommandCount += originCommandCount;
		__instance._renderRecord[0].batchCommandCount += batchCommandCount;
	}
}

void CSDiagnostics::renderInvoke(int invokeCount) {
	synchronized(__instance._lock) {
		__instance._renderRecord[0].invokeCount += invokeCount;
	}
}

void CSDiagnostics::renderDraw(int vertexCount) {
	synchronized(__instance._lock) {
		__instance._renderRecord[0].drawCount++;
		__instance._renderRecord[0].vertexCount += vertexCount;
	}
}

void CSDiagnostics::renderCycle() {
	synchronized(__instance._lock) {
		__instance._renderRecord[1] = __instance._renderRecord[0];
		__instance._renderRecord[0] = {};
	}
}

#endif
