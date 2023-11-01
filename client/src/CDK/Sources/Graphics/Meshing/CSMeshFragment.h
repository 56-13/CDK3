#ifndef __CDK__CSMeshFragment__
#define __CDK__CSMeshFragment__

#include "CSString.h"
#include "CSDictionary.h"
#include "CSHalf.h"
#include "CSGBuffers.h"
#include "CSGraphics.h"

class CSMesh;

class CSMeshFragment {
public:
	struct Bone {
		string name;
		CSMatrix matrix;
		CSABoundingBox aabb;

		explicit Bone(CSBuffer* buffer);
	};
private:
	string _name;
	byte _materialIndex;
	bool _hasTexCoords;
	bool _hasNormals;
	bool _hasTangents;
	CSABoundingBox _aabb;
	CSVertexArray* _vertices;
	CSArray<Bone>* _bones;
	CSDictionary<string, byte>* _boneIndices;			//본은 256개 이하
public:
	explicit CSMeshFragment(CSBuffer* buffer);
	~CSMeshFragment();

	CSMeshFragment(const CSMeshFragment&) = delete;
	CSMeshFragment& operator=(const CSMeshFragment&) = delete;

	int resourceCost() const;

	inline const string& name() const {
		return _name;
	}
	inline int materialIndex() const {
		return _materialIndex;
	}
	inline bool hasTexCoords() const {
		return _hasTexCoords;
	}
	inline bool hasNormals() const {
		return _hasNormals;
	}
	inline bool hasTangents() const {
		return _hasTexCoords;
	}
	inline bool hasBones() const {
		return _bones != NULL;
	}
	inline const CSABoundingBox& aabb() const {
		return _aabb;
	}
	inline int vertexCount() const {
		return _vertices->vertexBuffer(0)->count();
	}
	inline int faceCount() const {
		return _vertices->indexBuffer()->count() / 3;
	}
	inline int boneCount() const {
		return _bones ? _bones->count() : 0;
	}
	const Bone& bone(int index) const;
	const Bone* bone(const string& name) const;

	void draw(CSGraphics* graphics, const CSGBufferSlice* boneBufferSlice, const CSABoundingBox* aabb = NULL, const CSArray<CSVertexArrayInstance>* instances = NULL) const;
};

#endif
