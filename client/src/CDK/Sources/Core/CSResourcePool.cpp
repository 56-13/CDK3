#define CDK_IMPL

#include "CSResourcePool.h"

#include "CSThread.h"

CSResourcePool::Entry::~Entry() {
    release(key);
	resource->release();
}

CSResourcePool* CSResourcePool::_instance = NULL;

CSResourcePool::CSResourcePool() :
    _first(NULL),
    _last(NULL),
    _elapsed(0)
{
}

void CSResourcePool::initialize() {
	if (!_instance) _instance = new CSResourcePool();
}

void CSResourcePool::finalize() {
    if (_instance) {
        delete _instance;
        _instance = NULL;
    }
}

void CSResourcePool::useEntry(Entry* e) {
    e->elapsed = _elapsed + CSMath::max(e->life, 1);

    if (e != _first) {
        detachEntry(e);

        if (!_first) _first = _last = e;
        else {
            Entry* current = _first;

            while (current && e->elapsed < current->elapsed) current = current->next;

            if (current) {
                if (current->prev) current->prev->next = e;
                else _first = e;
                e->prev = current->prev;
                e->next = current;
                current->prev = e;
            }
            else {
                _last->next = e;
                e->prev = _last;
                _last = e;
            }
        }
    }
}

void CSResourcePool::detachEntry(Entry* e) {
    if (e->prev) e->prev->next = e->next;
    if (e->next) e->next->prev = e->prev;
    if (_first == e) _first = e->next;
    if (_last == e) _last = e->prev;
    e->next = NULL;
    e->prev = NULL;
}

void CSResourcePool::removeEntry(Entry* e, bool recycle) {
    e->retain();

    if (e->key && _resources.removeObject(e->key) && recycle && e->recycle) {
        CSObject::release(e->key);
        e->life = 1;
        _recycles.addObject(e);
        useEntry(e);
    }
    else
    {
        detachEntry(e);
        CSLog("resource remove:%.5fmb", (double)e->resource->resourceCost() / 1024768);
        _recycles.removeObjectIdenticalTo(e);
    }
    
    e->release();
}

CSResourcePool::Cost CSResourcePool::totalCost() const {
    Cost cost;
    synchronized(_lock) {
        Entry* e = _first;
        while (e) {
            cost.cost += e->resource->resourceCost();
            cost.count++;
            e = e->next;
        }
    }
    return cost;
}

CSResourcePool::Cost CSResourcePool::totalCost(CSResourceType type) const {
    Cost cost;
    synchronized(_lock) {
        Entry* e = _first;
        while (e) {
            if (e->resource->resourceType() == type) {
                cost.cost += e->resource->resourceCost();
                cost.count++;
            }
            e = e->next;
        }
    }
    return cost;
}

CSResourcePool::Cost CSResourcePool::entryCost() const {
    Cost cost;
    synchronized(_lock) {
        for (CSDictionary<CSObject, Entry>::ReadonlyIterator i = _resources.iterator(); i.remaining(); i.next()) {
            cost.cost += i.object()->resource->resourceCost();
            cost.count++;
        }
    }
    return cost;
}

CSResourcePool::Cost CSResourcePool::entryCost(CSResourceType type) const {
    Cost cost;
    synchronized(_lock) {
        for (CSDictionary<CSObject, Entry>::ReadonlyIterator i = _resources.iterator(); i.remaining(); i.next()) {
            if (i.object()->resource->resourceType() == type) {
                cost.cost += i.object()->resource->resourceCost();
                cost.count++;
            }
        }
    }
    return cost;
}

CSResourcePool::Cost CSResourcePool::recycleCost() const {
    Cost cost;
    synchronized(_lock) {
        foreach (const Entry*, e, &_recycles) {
            cost.cost += e->resource->resourceCost();
            cost.count++;
        }
    }
    return cost;
}

CSResourcePool::Cost CSResourcePool::recycleCost(CSResourceType type) const {
    Cost cost;
    synchronized(_lock) {
        foreach (const Entry*, e, &_recycles) {
            if (e->resource->resourceType() == type) {
                cost.cost += e->resource->resourceCost();
                cost.count++;
            }
        }
    }
    return cost;
}

CSResource* CSResourcePool::get(const CSObject* key) {
    synchronized(_lock) {
        Entry* e = _resources.objectForKey(key);
        if (e) {
            useEntry(e);
            return e->resource;
        }
    }
    return NULL;
}

void CSResourcePool::add(const CSObject* key, CSResource* resource, int life, bool recycle) {
    synchronized(_lock) {
        if (!key) key = resource;

        Entry* e = _resources.objectForKey(key);
        if (!e) {
            e = new Entry();
            e->key = CSObject::retain(key);
            _resources.setObject(key, e);
            e->release();
        }
        CSObject::retain(e->resource, resource);
        e->life = life;
        e->recycle = recycle;
        useEntry(e);

        CSLog("resource add:%s", key->toString().cstring());
    }
}

void CSResourcePool::remove(const CSObject* key) {
    synchronized(_lock) {
        Entry* e = _resources.objectForKey(key);
        if (e) removeEntry(e, true);
    }
}

bool CSResourcePool::recycle(const Match& match, const CSObject* key, int life, CSResource*& resource) {
    synchronized(_lock) {
        for (int i = 0; i < _recycles.count(); i++) {
            Entry* e = _recycles.objectAtIndex(i);

            if (match(e->resource)) {
                e->key = CSObject::retain(key ? key : e->resource);
                e->life = life;
                _resources.setObject(e->key, e);
                _recycles.removeObjectAtIndex(i);
                useEntry(e);
                resource = e->resource;
                return true;
            }
        }
    }
    resource = NULL;
    return false;
}

void CSResourcePool::purge(int64 cost) {
#ifdef CS_CONSOLE_DEBUG
    if (cost > 0) {
        Cost current = totalCost();
        CSLog("resource purge manual:%dkb:%d / %dkb", cost / 1024, current.count, current.cost / 1024);
    }
#endif
    bool purge = false;

    synchronized(_lock) {
        Entry* e = _last;
        while (e && e->elapsed <= _elapsed) {
            Entry* prev = e->prev;

            bool disposing = cost > 0;
            if (disposing || e->life > 0) {
                int retainCountUnlinked = 1;
                if (e->key == e->resource) retainCountUnlinked += 2;                  //resources, key
                if (e->resource->retainCount() <= retainCountUnlinked) {              //다른 모든 곳에서 retain이 없어진 경우
                    cost -= e->resource->resourceCost();
                    removeEntry(e, !disposing);
                    purge = true;
                }
            }

            e = prev;
        }
    }
#ifdef CS_CONSOLE_DEBUG
    if (purge) {
        Cost current = totalCost();
        CSLog("resource purge:%d / %dkb", current.count, current.cost / 1024);
    }
#endif
}

void CSResourcePool::purgeCycle() {
    purge(0);
    _elapsed++;
}

class LoadKey : public CSObject {
private:
    CSResourceType _type;
    const CSArray<ushort>* _indices;
public:
    LoadKey(CSResourceType type, const CSArray<ushort>* indices) : _type(type), _indices(retain(indices)) {

    }
private:
    ~LoadKey() {
        release(_indices);
    }
public:
    uint hash() const override {
        CSHash hash;
        hash.combine(_type);
        if (_indices) hash.combine(_indices->sequentialHash());
        return hash;
    }
    bool isEqual(const CSObject* obj) const override {
        const LoadKey* other = dynamic_cast<const LoadKey*>(obj);

        return other && _type == other->_type && (_indices ? other->_indices && _indices->sequentialEqual(other->_indices) : other->_indices == NULL);
    }
};

CSResource* CSResourcePool::load(CSResourceType type, const CSArray<ushort>* indices) {
    CSResource* resource = _load(type, indices, false);
    if (!resource) {
        LoadKey* key = new LoadKey(type, indices);
        resource = get(key);
        if (!resource) {
            resource = _load(type, indices, true);
            if (resource) add(key, resource, 0, false);
        }
        key->release();
    }
    return resource;
}
