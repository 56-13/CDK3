#define CDK_IMPL

#include "CSGBufferData.h"

int CSVertexIndexData::size() const {
	if (vertexCapacity <= 256) return 1;
	else if (vertexCapacity <= 65536) return 2;
	else return 4;
}

const void* CSVertexIndexData::pointer() const {
	const int* p = (const int*)CSGBufferData<int>::pointer();
	if (vertexCapacity <= 256) {
		byte* cp = (byte*)fmalloc(count());
		for (int i = 0; i < count(); i++) {
			CSAssert(p[i] < 256);
			cp[i] = p[i];
		}
		autofree(cp);
		return cp;
	}
	if (vertexCapacity <= 65536) {
		ushort* cp = (ushort*)fmalloc(count() * 2);
		for (int i = 0; i < count(); i++) {
			CSAssert(p[i] < 65536);
			cp[i] = p[i];
		}
		autofree(cp);
		return cp;
	}
	return p;
}