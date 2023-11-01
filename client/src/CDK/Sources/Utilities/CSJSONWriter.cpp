#define CDK_IMPL

#include "CSJSONWriter.h"

#include "CSString.h"
#include "CSValue.h"

using namespace rapidjson;

CSJSONWriter::CSJSONWriter() {
    _document.SetObject();
    _stack.addObject(&_document);
}

void CSJSONWriter::writeBool(const char* name, bool value) {
    _stack.lastObject()->AddMember(name, value, _document.GetAllocator());
}

void CSJSONWriter::writeNumber(const char* name, int64 value) {
    _stack.lastObject()->AddMember(name, value, _document.GetAllocator());
}

void CSJSONWriter::writeDouble(const char* name, double value) {
    _stack.lastObject()->AddMember(name, value, _document.GetAllocator());
}

void CSJSONWriter::writeString(const char* name, const char* value) {
    if (value) {
        _stack.lastObject()->AddMember(name, value, _document.GetAllocator());
    }
}

void CSJSONWriter::writeObject(const char* name, const CSJSONObject* object) {
    Value* last = _stack.lastObject();
    Value value(kObjectType);
    last->AddMember(name, value, _document.GetAllocator());
    Value& v = (*last)[name];
    
    for (CSJSONObject::ReadonlyIterator i = object->iterator(); i.remaining(); i.next()) {
        addElement(v, i.key(), i.object());
    }
}

void CSJSONWriter::writeArray(const char* name, const CSJSONArray* array) {
    Value* last = _stack.lastObject();
    Value value(kArrayType);
    last->AddMember(name, value, _document.GetAllocator());
    Value& v = (*last)[name];
    
    foreach (const CSObject *, obj, array) {
        addElement(v, obj);
    }
}

void CSJSONWriter::writeBool(bool value) {
    _stack.lastObject()->PushBack(value, _document.GetAllocator());
}

void CSJSONWriter::writeNumber(int64 value) {
    _stack.lastObject()->PushBack(value, _document.GetAllocator());
}

void CSJSONWriter::writeDouble(double value) {
    _stack.lastObject()->PushBack(value, _document.GetAllocator());
}

void CSJSONWriter::writeString(const char* value) {
    if (value) _stack.lastObject()->PushBack(value, _document.GetAllocator());
}

void CSJSONWriter::writeObject(const CSJSONObject* object) {
    Value* last = _stack.lastObject();
    Value value(kObjectType);
    last->PushBack(value, _document.GetAllocator());
    Value& v = (*last)[last->Size() - 1];
    
    for (CSJSONObject::ReadonlyIterator i = object->iterator(); i.remaining(); i.next()) {
        addElement(v, i.key(), i.object());
    }
}

void CSJSONWriter::writeArray(const CSJSONArray* array) {
    Value* last = _stack.lastObject();
    Value value(kArrayType);
    last->PushBack(value, _document.GetAllocator());
    Value& v = (*last)[last->Size() - 1];
    
    foreach (const CSObject*, obj, array) {
        addElement(v, obj);
    }
}

void CSJSONWriter::addElement(Value& v, const CSObject* obj) {
    const CSBool* b = dynamic_cast<const CSBool*>(obj);
    if (b) {
        v.PushBack((bool)*b, _document.GetAllocator());
        return;
    }
    const CSLong* n = dynamic_cast<const CSLong*>(obj);
    if (n) {
        v.PushBack((int64) * n, _document.GetAllocator());
        return;
    }
    const CSDouble* d = dynamic_cast<const CSDouble*>(obj);
    if (d) {
        v.PushBack((double)*d, _document.GetAllocator());
        return;
    }
    const CSStringContent* s = dynamic_cast<const CSStringContent*>(obj);
    if (s) {
        v.PushBack(s->cstring(), _document.GetAllocator());
        return;
    }
    const CSJSONArray* a = dynamic_cast<const CSJSONArray*>(obj);
    if (a) {
        Value value(kArrayType);
        v.PushBack(value, _document.GetAllocator());
        Value& nv = v[v.Size() - 1];
        
        foreach (const CSObject *, obj, a) {
            addElement(nv, obj);
        }
        return;
    }
    const CSJSONObject* c = dynamic_cast<const CSJSONObject*>(obj);
    if (c) {
        Value value(kObjectType);
        v.PushBack(value, _document.GetAllocator());
        Value& nv = v[v.Size() - 1];
        
        for (CSJSONObject::ReadonlyIterator i = c->iterator(); i.remaining(); i.next()) {
            addElement(nv, i.key(), i.object());
        }
        return;
    }
}

void CSJSONWriter::addElement(Value& v, const char* name, const CSObject* obj) {
    const CSBool* b = dynamic_cast<const CSBool*>(obj);
    if (b) {
        v.AddMember(name, (bool)*b, _document.GetAllocator());
        return;
    }
    const CSLong* n = dynamic_cast<const CSLong*>(obj);
    if (n) {
        v.AddMember(name, (int64) * n, _document.GetAllocator());
        return;
    }
    const CSDouble* d = dynamic_cast<const CSDouble*>(obj);
    if (d) {
        v.AddMember(name, (double)*d, _document.GetAllocator());
        return;
    }
    const CSStringContent* s = dynamic_cast<const CSStringContent*>(obj);
    if (s) {
        v.AddMember(name, s->cstring(), _document.GetAllocator());
        return;
    }
    const CSJSONArray* a = dynamic_cast<const CSJSONArray*>(obj);
    if (a) {
        Value value(kArrayType);
        v.AddMember(name, value, _document.GetAllocator());
        Value& nv = v[name];
        
        foreach (const CSObject *, obj, a) {
            addElement(nv, obj);
        }
        return;
    }
    const CSJSONObject* c = dynamic_cast<const CSJSONObject*>(obj);
    if (c) {
        Value value(kObjectType);
        v.AddMember(name, value, _document.GetAllocator());
        Value& nv = v[name];
        
        for (CSJSONObject::ReadonlyIterator i = c->iterator(); i.remaining(); i.next()) {
            addElement(nv, i.key(), i.object());
        }
    }
}

void CSJSONWriter::startArray(const char* name) {
    Value value(kArrayType);
    
    Value* last = _stack.lastObject();
    last->AddMember(name, value, _document.GetAllocator());
    _stack.addObject(&(*last)[name]);
}

void CSJSONWriter::endArray() {
    CSAssert(_stack.lastObject()->GetType() == kArrayType, "invalid stack");
    
    _stack.removeLastObject();
}

void CSJSONWriter::startObject(const char* name) {
    Value value(kObjectType);
    
    Value* last = _stack.lastObject();
    last->AddMember(name, value, _document.GetAllocator());
    _stack.addObject(&(*last)[name]);
}

void CSJSONWriter::endObject() {
    CSAssert(_stack.count() > 1 && _stack.lastObject()->GetType() == kObjectType, "invalid stack");
    
    _stack.removeLastObject();
}

void CSJSONWriter::startArray() {
    Value value(kArrayType);
    
    Value* last = _stack.lastObject();
    last->PushBack(value, _document.GetAllocator());
    _stack.addObject(&(*last)[last->Size() - 1]);
}

void CSJSONWriter::startObject() {
    Value value(kObjectType);
    
    Value* last = _stack.lastObject();
    last->PushBack(value, _document.GetAllocator());
    _stack.addObject(&(*last)[last->Size() - 1]);
}

string CSJSONWriter::toString() const {
    GenericStringBuffer<UTF8<> > strbuf;
    Writer<StringBuffer> writer(strbuf);
    _document.Accept(writer);
    
    return string(strbuf.GetString());
}

