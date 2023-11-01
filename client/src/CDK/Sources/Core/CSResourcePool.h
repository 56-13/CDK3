#ifndef __CDK__CSResourcePool__
#define __CDK__CSResourcePool__

#include "CSResource.h"
#include "CSDictionary.h"
#include "CSTask.h"

class CSResourcePool {
public:
    typedef std::function<bool(const CSResource* candidate)> Match;
    typedef std::function<CSResource* (CSResourceType, const CSArray<ushort>*, bool)> Load;
private:
    class Entry : public CSObject {
    public:
        const CSObject* key;
        CSResource* resource;
        int life;
        int elapsed;
        bool recycle;
        Entry* prev;
        Entry* next;

        Entry() = default;
    private:
        ~Entry();
    };
    CSDictionary<CSObject, Entry> _resources;
    CSArray<Entry> _recycles;
    Entry* _first;
    Entry* _last;
    int _elapsed;
    Load _load;
    mutable CSLock _lock;

    static CSResourcePool* _instance;

    CSResourcePool();
    ~CSResourcePool() = default;
public:
#ifdef CDK_IMPL
    static void initialize();
    static void finalize();
#endif
    static inline CSResourcePool* sharedPool() {
        return _instance;
    }

    struct Cost {
        int64 cost = 0;
        int count = 0;
    };
    Cost totalCost() const;
    Cost totalCost(CSResourceType type) const;
    Cost entryCost() const;
    Cost entryCost(CSResourceType type) const;
    Cost recycleCost() const;
    Cost recycleCost(CSResourceType type) const;

    CSResource* get(const CSObject* key);
    void add(const CSObject* key, CSResource* resource, int life, bool recycle);
    inline void add(CSResource* resource, int life, bool recycle) {
        add(NULL, resource, life, recycle);
    }
    void remove(const CSObject* key);
    bool recycle(const Match& match, const CSObject* key, int life, CSResource*& resource);
    inline bool recycle(const Match& match, int life, CSResource*& resource) {
        return _instance->recycle(match, NULL, life, resource);
    }
    void purge(int64 cost);
#ifdef CDK_IMPL
    void purgeCycle();
#endif
    inline void loadLink(const Load& load) {
        _load = load;
    }
    CSResource* load(CSResourceType type, const CSArray<ushort>* indices);
private:
    void useEntry(Entry* e);
    void detachEntry(Entry* e);
    void removeEntry(Entry* e, bool recycle);
};

#endif
