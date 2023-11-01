#ifndef __CDK__CSMeshNode__
#define __CDK__CSMeshNode__

#include "CSString.h"
#include "CSArray.h"
#include "CSMatrix.h"

class CSBuffer;

class CSMeshNode : public CSObject {
private:
	string _name;
	CSMeshNode* _parent;
	CSArray<CSMeshNode>* _children;
	CSMatrix _localTransform;
	CSMatrix _globalTransform;
	CSArray<int>* _fragmentIndices;
public:
	CSMeshNode(CSBuffer* buffer, CSMeshNode* parent);
private:
	~CSMeshNode();
public:
	int resourceCost() const;

	inline const string& name() const {
		return _name;
	}
	inline const CSMeshNode* parent() const {
		return _parent;
	}
	inline CSArray<CSMeshNode>* children() {
		return _children;
	}
	inline const CSArray<CSMeshNode>* children() const {
		return _children;
	}
	inline const CSMatrix& localTransform() const {
		return _localTransform;
	}
	inline const CSMatrix& globalTransform() const {
		return _globalTransform;
	}
	inline const CSArray<int>* fragmentIndices() const {
		return _fragmentIndices;
	}
};

#endif
