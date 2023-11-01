#ifndef __CDK__CSJSONWriter__
#define __CDK__CSJSONWriter__

#include "CSArray.h"

#include "CSJSONObject.h"
#include "CSJSONArray.h"

#include "rapidjson/document.h"
#include "rapidjson/writer.h"
#include "rapidjson/stringbuffer.h"

class CSJSONWriter : public CSObject {
private:
    rapidjson::Document _document;
    CSArray<rapidjson::Value*> _stack;
public:
    CSJSONWriter();

    static inline CSJSONWriter* writer() {
        return autorelease(new CSJSONWriter());
    }
private:
    void addElement(rapidjson::Value& v, const CSObject* obj);
    void addElement(rapidjson::Value& v, const char* name, const CSObject* obj);
public:
    void writeBool(const char* name, bool value);
    void writeNumber(const char* name, int64 value);
    void writeDouble(const char* name, double value);
    void writeString(const char* name, const char* value);
    void writeObject(const char* name, const CSJSONObject* object);
    void writeArray(const char* name, const CSJSONArray* array);
    
    void writeBool(bool value);
    void writeNumber(int64 value);
    void writeDouble(double value);
    void writeString(const char* value);
    void writeObject(const CSJSONObject* value);
    void writeArray(const CSJSONArray* value);
    
    void startArray(const char* name);
    void endArray();
    void startObject(const char* name);
    void endObject();
    void startArray();
    void startObject();
    
    template <typename V, int dimension>
    void writeArray(const char* name, const CSArray<V, dimension>* array, void (V:: *func)(CSJSONWriter*) const);
    template <typename V, typename P, int dimension>
    void writeArray(const char* name, const CSArray<V, dimension>* array, void (*func)(CSJSONWriter*, P, typename CSEntryType<V, true>::ConstValueParamType), P param);
    
    string toString() const override;
};

#include "CSJSONWriter+array.h"

#endif
