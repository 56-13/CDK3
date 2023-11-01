#ifndef __CDK__CSJSONArray__
#define __CDK__CSJSONArray__

#include "CSArray.h"

#include "CSMacro.h"

class CSJSONObject;

class CSJSONArray : public CSArray<CSObject> {
public:
    CSJSONArray() = default;
    inline CSJSONArray(int capacity) : CSArray(capacity) {}
    inline CSJSONArray(const CSJSONArray* arr) : CSArray(arr) {}
private:
    ~CSJSONArray() = default;
public:
    static inline CSJSONArray* array() {
        return autorelease(new CSJSONArray());
    }
    static inline CSJSONArray* arrayWithCapacity(int capacity) {
        return autorelease(new CSJSONArray(capacity));
    }
    
    bool boolAtIndex(int index) const;
    int64 numberAtIndex(int index) const;
    double doubleAtIndex(int index) const;
    string stringAtIndex(int index) const;
    const CSJSONArray* jsonArrayAtIndex(int index) const;
    const CSJSONObject* jsonObjectAtIndex(int index) const;
    
    CSJSONArray* jsonArrayAtIndex(int index);
    CSJSONObject* jsonObjectAtIndex(int index);
    
    void addBool(bool value);
    void addNumber(int64 value);
    void addDouble(double value);
    void addString(const string& value);
    void addJSONArray(CSJSONArray* value);
    void addJSONObject(CSJSONObject* value);
    
    void insertBool(int index, bool value);
    void insertNumber(int index, int64 value);
    void insertDouble(int index, double value);
    void insertString(int index, const string& value);
    void insertJSONArray(int index, CSJSONArray* value);
    void insertJSONObject(int index, CSJSONObject* value);
    
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* readArray() const;
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* readNumericArray() const;
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* readPointArray() const;
    template <int dimension = 1>
    CSArray<string, dimension>* readStringArray() const;
    
    //===============================================================
    template <typename V, bool retain = derived<CSObject, V>::value>
    struct readArrayEntry {
        typedef V* (*func)(const CSJSONObject*);
    };
    template <typename V>
    struct readArrayEntry<V, false> {
        typedef void (*func)(const CSJSONObject*, V&);
    };
    template <typename V, typename P, bool retain = derived<CSObject, V>::value>
    struct readArrayEntryWithParam {
        typedef V* (*func)(const CSJSONObject*, P);
    };
    template <typename V, typename P>
    struct readArrayEntryWithParam<V, P, false> {
        typedef void (*func)(const CSJSONObject*, P, V&);
    };
    //===============================================================
    template <typename V, int dimension = 1>
    CSArray<V, dimension>* readArray(typename readArrayEntry<V>::func func) const;
    template <typename V, int dimension = 1, typename P>
    CSArray<V, dimension>* readArray(typename readArrayEntryWithParam<V, P>::func func, P param) const;
};

#include "CSJSONArray+array.h"

#endif
