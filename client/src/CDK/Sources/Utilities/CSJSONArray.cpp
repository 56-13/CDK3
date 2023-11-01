#define CDK_IMPL

#include "CSJSONArray.h"

#include "CSJSONObject.h"

#include "CSValue.h"

#include "CSString.h"

bool CSJSONArray::boolAtIndex(int index) const {
    const CSBool* obj = dynamic_cast<const CSBool*>(objectAtIndex(index));
    if (obj) return *obj;
    CSWarnLog("invalid boolAtIndex(%d)", index);
    return false;
}

int64 CSJSONArray::numberAtIndex(int index) const {
    const CSLong* obj = dynamic_cast<const CSLong*>(objectAtIndex(index));
    if (obj) return *obj;
    CSWarnLog("invalid numberAtIndex(%d)", index);
    return 0;
}

double CSJSONArray::doubleAtIndex(int index) const {
    const CSDouble* obj = dynamic_cast<const CSDouble*>(objectAtIndex(index));
    if (obj) return *obj;
    CSWarnLog("invalid doubleAtIndex(%d)", index);
    return 0.0;
}

string CSJSONArray::stringAtIndex(int index) const {
    const CSStringContent* obj = dynamic_cast<const CSStringContent*>(objectAtIndex(index));
#ifdef CS_CONSOLE_DEBUG
    if (!obj) CSWarnLog("invalid stringAtIndex(%d)", index);
#endif
    return obj;
}

const CSJSONArray* CSJSONArray::jsonArrayAtIndex(int index) const {
    const CSJSONArray* obj = dynamic_cast<const CSJSONArray*>(objectAtIndex(index));
#ifdef CS_CONSOLE_DEBUG
    if (!obj) CSWarnLog("invalid jsonArrayAtIndex(%d)", index);
#endif
    return obj;
}

const CSJSONObject* CSJSONArray::jsonObjectAtIndex(int index) const {
    const CSJSONObject* obj = dynamic_cast<const CSJSONObject*>(objectAtIndex(index));
    
#ifdef CS_CONSOLE_DEBUG
    if (!obj) CSWarnLog("invalid jsonObjectAtIndex(%d)", index);
#endif
    return obj;
}

CSJSONArray* CSJSONArray::jsonArrayAtIndex(int index) {
    CSJSONArray* obj = dynamic_cast<CSJSONArray*>(objectAtIndex(index));
#ifdef CS_CONSOLE_DEBUG
    if (!obj) CSWarnLog("invalid jsonArrayAtIndex(%d)", index);
#endif
    return obj;
}
CSJSONObject* CSJSONArray::jsonObjectAtIndex(int index) {
    CSJSONObject* obj = dynamic_cast<CSJSONObject*>(objectAtIndex(index));
    
#ifdef CS_CONSOLE_DEBUG
    if (!obj) CSWarnLog("invalid jsonObjectAtIndex(%d)", index);
#endif
    return obj;
}

void CSJSONArray::addBool(bool value) {
    CSArray<CSObject>::addObject(CSBool::value(value));
}
void CSJSONArray::addNumber(int64 value) {
    CSArray<CSObject>::addObject(CSLong::value(value));
}
void CSJSONArray::addDouble(double value) {
    CSArray<CSObject>::addObject(CSDouble::value(value));
}
void CSJSONArray::addString(const string& value) {
    CSArray<CSObject>::addObject(const_cast<CSStringContent*>(value.content()));
}
void CSJSONArray::addJSONArray(CSJSONArray* value) {
    CSArray<CSObject>::addObject(value);
}
void CSJSONArray::addJSONObject(CSJSONObject* value) {
    CSArray<CSObject>::addObject(value);
}

void CSJSONArray::insertBool(int index, bool value) {
    CSArray<CSObject>::insertObject(index, CSBool::value(value));
}
void CSJSONArray::insertNumber(int index, int64 value) {
    CSArray<CSObject>::insertObject(index, CSLong::value(value));
}
void CSJSONArray::insertDouble(int index, double value) {
    CSArray<CSObject>::insertObject(index, CSDouble::value(value));
}
void CSJSONArray::insertString(int index, const string& value) {
    CSArray<CSObject>::insertObject(index, const_cast<CSStringContent*>(value.content()));
}
void CSJSONArray::insertJSONArray(int index, CSJSONArray* value) {
    CSArray<CSObject>::insertObject(index, value);
}
void CSJSONArray::insertJSONObject(int index, CSJSONObject* value) {
    CSArray<CSObject>::insertObject(index, value);
}
