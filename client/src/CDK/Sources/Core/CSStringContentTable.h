#ifdef CDK_IMPL

#ifndef __CDK__CSStringContentTable_h
#define __CDK__CSStringContentTable_h

#include "CSStringContent.h"

#include "CSDictionary.h"

class CSStringContentTable {
private:
	struct ContentKey {
	private:
		const std::string& _pstr;
	public:
		inline ContentKey(const std::string& pstr) : _pstr(pstr) {}

		inline bool operator ==(const ContentKey& other) const {
			return _pstr == other._pstr;
		}
		inline bool operator !=(const ContentKey& other) const {
			return _pstr != other._pstr;
		}
		inline uint hash() const {
			return std::hash<std::string>()(_pstr);
		}
	};
	struct ContentDrain {
		CSStringContent* content;
		double elapsed;

		static constexpr float DrainCacheTime = 0.1f;

		ContentDrain(CSStringContent* content);
		~ContentDrain();
	};
	CSDictionary<ContentKey, CSStringContent*> _contents;
	CSArray<ContentDrain> _drains;
	CSLock _lock;

	static CSStringContentTable* _instance;

	CSStringContentTable();
	~CSStringContentTable() = default;
public:
	static void initialize();
	static void finalize();

	static inline CSStringContentTable* sharedTable() {
		return _instance;
	}

	const CSStringContent* get(std::string&& str);
	void release(const std::string& str);
	void drain();
};

#endif

#endif