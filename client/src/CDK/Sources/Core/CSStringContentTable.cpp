#define CDK_IMPL

#include "CSStringContentTable.h"

#include "CSThread.h"
#include "CSTime.h"

CSStringContentTable::ContentDrain::ContentDrain(CSStringContent* content) : content(content), elapsed(CSTime::currentTime() + DrainCacheTime) {
	content->retain();
}

CSStringContentTable::ContentDrain::~ContentDrain() {
	content->release();
}

//=============================================================================================================

CSStringContentTable* CSStringContentTable::_instance = NULL;

CSStringContentTable::CSStringContentTable() :
	_contents(16384),
	_drains(16384)
{

}

void CSStringContentTable::initialize() {
	if (!_instance) _instance = new CSStringContentTable();
}

void CSStringContentTable::finalize() {
	if (_instance) {
		delete _instance;
		_instance = NULL;
	}
}

const CSStringContent* CSStringContentTable::get(std::string&& str) {
	synchronized(_lock) {
		CSStringContent* content = _contents.objectForKey(str);
		if (content) content->retain();
		else {
			content = new CSStringContent(std::move(str));
			_contents.setObject(content->string(), content);
		}
		_drains.addObject(content);
		return content;
	}
	return NULL;
}

void CSStringContentTable::release(const std::string& str) {
	synchronized(_lock) {
		_contents.removeObject(str);
	}
}

void CSStringContentTable::drain() {
	double currentTime = CSTime::currentTime();
	int count = 0;
	synchronized(_lock) {
		while (count < _drains.count() && _drains.objectAtIndex(count).elapsed <= currentTime) count++;
		if (count) _drains.removeObjectsWithRange(0, count);
	}
}
