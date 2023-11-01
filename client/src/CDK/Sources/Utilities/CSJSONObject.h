#ifndef __CDK__CSJSONObject__
#define __CDK__CSJSONObject__

#include "CSDictionary.h"

#include "CSJSONArray.h"

class CSJSONObject : public CSDictionary<string, CSObject> {
public:
    CSJSONObject() = default;
    inline CSJSONObject(int capacity) : CSDictionary(capacity) {}
    inline CSJSONObject(const CSJSONObject* obj) : CSDictionary(obj) {}
private:
    ~CSJSONObject() = default;
public:
    static inline CSJSONObject* object() {
        return autorelease(new CSJSONObject());
    }
    static inline CSJSONObject* objectWithCapacity(int capacity) {
        return autorelease(new CSJSONObject(capacity));
    }
    
    bool containsKey(const string& key) const;
    bool boolForKey(const string& key, bool essential = true) const;
    int64 numberForKey(const string& key, bool essential = true) const;
    double doubleForKey(const string& key, bool essential = true) const;
    string stringForKey(const string& key, bool essential = true) const;
    const CSJSONArray* jsonArrayForKey(const string& key, bool essential = true) const;
    const CSJSONObject* jsonObjectForKey(const string& key, bool essential = true) const;

    template <typename V, int dimension = 1>
    CSArray<V, dimension>* arrayForKey(const string& key, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readArray<V, dimension>() : NULL;
    }
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* numericArrayForKey(const string& key, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readNumericArray<V, dimension>() : NULL;
    }
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* pointArrayForKey(const string& key, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readPointArray<V, dimension>() : NULL;
    }
    template <int dimension = 1>
    CSArray<string, dimension>* stringArrayForKey(const string& key, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readStringArray<dimension>() : NULL;
    }
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* arrayForKey(const string& key, typename CSJSONArray::readArrayEntry<V>::func func, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readArray<V, dimension>(func) : NULL;
    }
    template <typename V, int dimension = 1, typename P>
    CSArray<V, dimension>* arrayForKey(const string& key, typename CSJSONArray::readArrayEntryWithParam<V, P>::func func, P param, bool essential = true) const {
        const CSJSONArray* jarr = jsonArrayForKey(key, essential);

        return jarr ? jarr->readArray<V, dimension, P>(func, param) : NULL;
    }
    
    CSJSONArray* jsonArrayForKey(const string& key, bool essential = true);
    CSJSONObject* jsonObjectForKey(const string& key, bool essential = true);
    
    void setBool(const string& key, bool value);
    void setNumber(const string& key, int64 value);
    void setDouble(const string& key, double value);
    void setString(const string& key, const string& value);
    void setJSONArray(const string& key, CSJSONArray* value);
    void setJSONObject(const string& key, CSJSONObject* value);
};

#endif
