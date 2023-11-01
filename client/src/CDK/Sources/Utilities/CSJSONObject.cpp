#define CDK_IMPL

#include "CSJSONObject.h"
#include "CSJSONArray.h"
#include "CSString.h"
#include "CSValue.h"

#ifdef CS_CONSOLE_DEBUG
#define NotFoundLog(...)    if (essential) CSErrorLog(__VA_ARGS__)
#else
#define NotFoundLog(...);
#endif

bool CSJSONObject::containsKey(const string& key) const {
    return CSDictionary<string, CSObject>::containsKey(key);
}

bool CSJSONObject::boolForKey(const string& key, bool essential) const {
    const CSBool* obj = dynamic_cast<const CSBool*>(objectForKey(key));
    if (obj) return *obj;
    NotFoundLog("invalid boolForKey(%s)", key);
    return false;
}

int64 CSJSONObject::numberForKey(const string& key, bool essential) const {
    const CSLong* obj = dynamic_cast<const CSLong*>(objectForKey(key));
    if (obj) return *obj;
    NotFoundLog("invalid numberForKey(%s)", key);
    return 0;
}

double CSJSONObject::doubleForKey(const string& key, bool essential) const {
    const CSDouble* obj = dynamic_cast<const CSDouble*>(objectForKey(key));
    if (obj) return *obj;
    NotFoundLog("invalid doubleForKey(%s)", key);
    return 0.0;
}

string CSJSONObject::stringForKey(const string& key, bool essential) const {
    const CSStringContent* obj = dynamic_cast<const CSStringContent*>(objectForKey(key));
    if (obj) return obj;
    NotFoundLog("invalid stringForKey(%s)", key);
    return NULL;
}

const CSJSONArray* CSJSONObject::jsonArrayForKey(const string& key, bool essential) const {
    const CSJSONArray* obj = dynamic_cast<const CSJSONArray*>(objectForKey(key));
    if (obj) return obj;
    NotFoundLog("invalid jsonArrayForKey(%s)", key);
    return NULL;
}

const CSJSONObject* CSJSONObject::jsonObjectForKey(const string& key, bool essential) const {
    const CSJSONObject* obj = dynamic_cast<const CSJSONObject*>(objectForKey(key));
    if (obj) return obj;
    NotFoundLog("invalid jsonObjectForKey(%s)", key);
    return NULL;
}

CSJSONArray* CSJSONObject::jsonArrayForKey(const string& key, bool essential) {
    CSJSONArray* obj = dynamic_cast<CSJSONArray*>(objectForKey(key));
    if (obj) return obj;
    NotFoundLog("invalid jsonArrayForKey(%s)", key);
    return NULL;
}
CSJSONObject* CSJSONObject::jsonObjectForKey(const string& key, bool essential) {
    CSJSONObject* obj = dynamic_cast<CSJSONObject*>(objectForKey(key));
    if (obj) return obj;
    NotFoundLog("invalid jsonObjectForKey(%s)", key);
    return NULL;
}

void CSJSONObject::setBool(const string& key, bool value) {
    CSDictionary<string, CSObject>::setObject(key, CSBool::value(value));
}
void CSJSONObject::setNumber(const string& key, int64 value) {
    CSDictionary<string, CSObject>::setObject(key, CSLong::value(value));
}
void CSJSONObject::setDouble(const string& key, double value) {
    CSDictionary<string, CSObject>::setObject(key, CSDouble::value(value));
}
void CSJSONObject::setString(const string& key, const string& value) {
    CSDictionary<string, CSObject>::setObject(key, const_cast<CSStringContent*>(value.content()));
}
void CSJSONObject::setJSONArray(const string& key, CSJSONArray* value) {
    CSDictionary<string, CSObject>::setObject(key, value);
}
void CSJSONObject::setJSONObject(const string& key, CSJSONObject* value) {
    CSDictionary<string, CSObject>::setObject(key, value);
}

