#define CDK_IMPL

#include "CSStringContent.h"
#include "CSStringContentTable.h"
#include "CSStringDisplay.h"

static CSLock __lock;

CSStringContent::CSStringContent(std::string&& str) : _string(std::move(str)), _characters(NULL), _display(NULL) {
    
}

CSStringContent::~CSStringContent() {
    if (_characters) delete _characters;
    if (_display) delete _display;
    CSStringContentTable* table = CSStringContentTable::sharedTable();
    if (table) table->release(_string);
}

int CSStringContent::resourceCost() const {
    int cost = sizeof(CSStringContent);
    cost += _string.length();
    if (_characters) cost += _characters->resourceCost();
    if (_display) cost += _display->resourceCost();
    return cost;
}

const CSCharSequence* CSStringContent::characters() const {
    if (!_characters) {
        synchronized(__lock) {
            if (!_characters) _characters = new CSCharSequence(UBRK_CHARACTER, _string.c_str());
        }
    }
	return _characters;
}

const CSStringDisplay* CSStringContent::display() const {
    if (!_display) {
        synchronized(__lock) {
            if (!_display) _display = new CSStringDisplay(_string.c_str());
        }
    }
    return _display;
}

