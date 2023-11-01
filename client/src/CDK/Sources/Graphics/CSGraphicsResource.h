#ifndef __CDK__CSGraphicsResource__
#define __CDK__CSGraphicsResource__

#include "CSResource.h"

#include "CSSet.h"

enum CSGBatchFlag {
	CSGBatchFlagRead = 1,
	CSGBatchFlagWrite = 2,
	CSGBatchFlagReadWrite = 3,
	CSGBatchFlagRetrieve = 4
};
typedef uint CSGBatchFlags;

class CSGraphicsResource : public CSResource {
protected:
	CSGraphicsResource() = default;
	virtual ~CSGraphicsResource() = default;
public:
	virtual bool batch(CSSet<const CSGraphicsResource*>* reads, CSSet<const CSGraphicsResource*>* writes, CSGBatchFlags flags) const;
	virtual void flush() const;
};

#endif