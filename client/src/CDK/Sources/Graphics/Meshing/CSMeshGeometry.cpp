#define CDK_IMPL

#include "CSMeshGeometry.h"

#include "CSBuffer.h"

CSMeshGeometry::ColliderFragment::ColliderFragment(CSBuffer* buffer) : name(buffer->readString()), origin(buffer) {
	
}

//===========================================================================================================

CSMeshGeometry::CSMeshGeometry(CSBuffer* buffer) :
	_name(buffer->readString()),
	_rootNode(new CSMeshNode(buffer, NULL)),
	_fragments(buffer),
	_materialConfigs(buffer),
	_colliderFragments(buffer)
{
	retrieve(_rootNode);

	CSABoundingBox aabb;
	if (getNodeAABB(_rootNode, NULL, 0, true, aabb)) _aabb = aabb;
}

CSMeshGeometry::~CSMeshGeometry() {
	_rootNode->release();
}

int CSMeshGeometry::resourceCost() const {
	int cost = sizeof(CSMeshGeometry) + _name.resourceCost();
	cost += _rootNode->resourceCost();
	cost += _allNodes.capacity() * 8;
	cost += _renderNodes.capacity() * 8;
	foreach (const CSMeshFragment&, frag, &_fragments) cost += frag.resourceCost();
	cost += _materialConfigs.capacity() * 8;
	foreach (const string&, materialConfig, &_materialConfigs) cost += materialConfig.resourceCost();
	cost += _colliderFragments.capacity() * sizeof(ColliderFragment);
	foreach (const ColliderFragment&, frag, &_colliderFragments) cost += frag.name.resourceCost();
	return cost;
}

void CSMeshGeometry::retrieve(CSMeshNode* node) {
	if (node->name()) _allNodes.setObject(node->name(), node);
	if (node->fragmentIndices()) _renderNodes.addObject(node);
	foreach (CSMeshNode*, child, node->children()) retrieve(child);
}

int CSMeshGeometry::vertexCount() const {
	int count = 0;
	foreach (const CSMeshNode*, node, &_renderNodes) {
		foreach (int, fi, node->fragmentIndices()) {
			count += _fragments.objectAtIndex(fi).vertexCount();
		}
	}
	return count;
}

int CSMeshGeometry::faceCount() const {
	int count = 0;
	foreach (const CSMeshNode*, node, &_renderNodes) {
		foreach (int, fi, node->fragmentIndices()) {
			count += _fragments.objectAtIndex(fi).faceCount();
		}
	}
	return count;
}

int CSMeshGeometry::boneCount() const {
	int count = 0;
	foreach (const CSMeshNode*, node, &_renderNodes) {
		foreach (int, fi, node->fragmentIndices()) {
			count += _fragments.objectAtIndex(fi).boneCount();
		}
	}
	return count;
}

void CSMeshGeometry::getNodeTransform(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, CSMatrix& result) const {
	if (!animation) result = node->globalTransform();
	else {
		if (node->parent()) getNodeTransform(node->parent(), animation, progress, result);
		CSMatrix local;
		if (node->name() && !animation->getNodeTransform(node->name(), progress, local)) result = node->localTransform();
		result = local * result;
	}
}

bool CSMeshGeometry::getNodeTransform(const string& name, const CSMeshAnimation* animation, float progress, CSMatrix& result) const {
	const CSMeshNode* node = findNode(name);
	if (!node) return false;
	getNodeTransform(node, animation, progress, result);
	return true;
}

bool CSMeshGeometry::getNodeAABBInternal(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, const CSMatrix& parentTransform, bool leaf, CSABoundingBox& result) const {
	CSMatrix transform;

	if (leaf) {
		if (!animation || !node->name() || !animation->getNodeTransform(node->name(), progress, transform)) transform = node->localTransform();
		transform *= parentTransform;
	}
	else transform = parentTransform;

	bool flag = false;

	CSVector3 corners[8];

	foreach (const CSMeshFragment&, frag, &_fragments) {
		if (node->name()) {
			const CSMeshFragment::Bone* bone = frag.bone(node->name());
			if (bone) {
				CSMatrix boneTransform = bone->matrix * transform;
				if (boneTransform == CSMatrix::Identity) result.append(bone->aabb);			//TODO:거의 CASE가 없다.
				else {
					bone->aabb.getCorners(corners);
					for (int i = 0; i < 8; i++) result.append(CSVector3::transformCoordinate(corners[i], boneTransform));
				}
				flag = true;
			}
		}
	}

	foreach (int, fi, node->fragmentIndices()) {
		const CSMeshFragment& frag = _fragments.objectAtIndex(fi);

		if (!frag.hasBones()) {
			if (transform == CSMatrix::Identity) result.append(frag.aabb());			//TODO:거의 CASE가 없다.
			else {
				frag.aabb().getCorners(corners);
				for (int i = 0; i < 8; i++) result.append(CSVector3::transformCoordinate(corners[i], transform));
			}
			flag = true;
		}
	}

	if (inclusive) {
		foreach (const CSMeshNode*, child, node->children()) {
			if (getNodeAABBInternal(child, animation, progress, true, transform, true, result)) flag = true;
		}
	}
	return flag;
}

bool CSMeshGeometry::getNodeAABB(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const {
	getNodeTransform(node, animation, progress, transform);

	result = CSABoundingBox::None;

	return getNodeAABBInternal(node, animation, progress, inclusive, CSMatrix::Identity, false, result);
}

bool CSMeshGeometry::getNodeAABB(const CSMeshNode* node, const CSMeshAnimation* animation, float progress, bool inclusive, CSABoundingBox& result) const {
	CSMatrix transform;
	getNodeTransform(node, animation, progress, transform);

	result = CSABoundingBox::None;

	return getNodeAABBInternal(node, animation, progress, inclusive, transform, false, result);
}

bool CSMeshGeometry::getNodeAABB(const string& name, const CSMeshAnimation* animation, float progress, bool inclusive, CSMatrix& transform, CSABoundingBox& result) const {
	const CSMeshNode* node = findNode(name);
	if (!node) return false;
	return getNodeAABB(node, animation, progress, inclusive, transform, result);
}

bool CSMeshGeometry::getNodeAABB(const string& name, const CSMeshAnimation* animation, float progress, bool inclusive, CSABoundingBox& result) const {
	const CSMeshNode* node = findNode(name);
	if (!node) return false;
	return getNodeAABB(node, animation, progress, inclusive, result);
}

bool CSMeshGeometry::getAABB(const CSMeshAnimation* animation, float progress, CSABoundingBox& result) const {
	if (!animation) {
		result = _aabb;
		return result != CSABoundingBox::Zero;
	}
	return getNodeAABB(_rootNode, animation, progress, true, result);
}
