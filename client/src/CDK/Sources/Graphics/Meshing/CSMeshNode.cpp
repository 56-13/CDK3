#define CDK_IMPL

#include "CSMeshNode.h"

#include "CSBuffer.h"

CSMeshNode::CSMeshNode(CSBuffer* buffer, CSMeshNode* parent) : 
	_name(buffer->readString()),
	_parent(parent),
	_children(retain(buffer->readArrayFunc<CSMeshNode>([this](CSBuffer* buffer) -> CSMeshNode* {
		return autorelease(new CSMeshNode(buffer, this));
	}))),
	_localTransform(buffer),
	_globalTransform(_parent ? _localTransform * _parent->_globalTransform : _localTransform),
	_fragmentIndices(retain(buffer->readArray<int>()))
{
	
}

CSMeshNode::~CSMeshNode() {
	release(_children);
	release(_fragmentIndices);
}

int CSMeshNode::resourceCost() const {
	int cost = sizeof(CSMeshNode);
	if (_name) cost += _name.resourceCost();
	if (_children) {
		cost += sizeof(CSArray<CSMeshNode>) + _children->capacity() * 8;
		foreach(const CSMeshNode*, child, _children) cost += child->resourceCost();
	}
	if (_fragmentIndices) {
		cost += sizeof(CSArray<int>) + _fragmentIndices->capacity() * sizeof(int);
	}
	return cost;
}
