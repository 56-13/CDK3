#define CDK_IMPL

#include "CSQuadTree.h"

#include "CSBoundingSphere.h"
#include "CSBoundingCapsule.h"
#include "CSOBoundingBox.h"
#include "CSBoundingMesh.h"

CSQuadTree::Node::Node(const CSABoundingBox& space) : _space(space), _objects(NULL), _nodes {}, _branch(false) {

}

CSQuadTree::Node::~Node() {
	CSObject::release(_objects);
	for (int i = 0; i < 8; i++) {
		if (_nodes[i]) delete _nodes[i];
	}
}

void CSQuadTree::Node::locate(CSSceneObject* obj, const CSABoundingBox& aabb, const CSVector3* aabbCorners, int depth) {
    if (depth == 0) {
        if (!_objects) _objects = new CSArray<CSSceneObject*>(4);
        _objects->addObject(obj);
    }
    else {
        depth--;

        CSVector3 center = _space.center();
        int flags = 0;
        for (int i = 0; i < 8; i++) {
            const CSVector3& corner = aabbCorners[i];
            int index = 0;
            if (corner.x > center.x) index |= 1;
            if (corner.y > center.y) index |= 2;
            if (corner.z > center.z) index |= 4;

            int s = 1 << index;
            if ((flags & s) == 0) {
                Node*& node = _nodes[index];

                if (!node) {
                    CSABoundingBox space;
                    space.minimum.x = (i & 1) == 0 ? _space.minimum.x : center.x;
                    space.maximum.x = (i & 1) == 0 ? center.x : _space.maximum.x;
                    space.minimum.y = (i & 2) == 0 ? _space.minimum.y : center.y;
                    space.maximum.y = (i & 2) == 0 ? center.y : _space.maximum.y;
                    space.minimum.z = (i & 4) == 0 ? _space.minimum.z : center.z;
                    space.maximum.z = (i & 4) == 0 ? center.z : _space.maximum.z;
                    node = new Node(space);
                }

                node->locate(obj, aabb, aabbCorners, depth);

                flags |= s;
            }
        }
        _branch = true;
    }
}

bool CSQuadTree::Node::unlocate(CSSceneObject* obj, const CSABoundingBox& aabb, const CSVector3* aabbCorners, int depth) {
    CSVector3 center = _space.center();

    if (depth == 0 || aabb.intersects(center, 0) != CSCollisionResultFront) {
        if (_objects && _objects->removeObjectIdenticalTo(obj) && _objects->count() == 0) {
            _objects->release();
            _objects = NULL;
        }
        if (_branch) {
            depth--;

            bool clear = true;
            for (int i = 0; i < 8; i++) {
                Node*& node = _nodes[i];
                if (node) {
                    if (node->unlocate(obj, aabb, aabbCorners, depth)) clear = false;
                    else {
                        delete node;
                        node = NULL;
                    }
                }
            }
            if (clear) _branch = false;
        }
    }
    else if (_branch) {
        depth--;

        int flags = 0;
        for (int i = 0; i < 8; i++) {
            const CSVector3& corner = aabbCorners[i];
            int index = 0;
            if (corner.x > center.x) index |= 1;
            if (corner.y > center.y) index |= 2;
            if (corner.z > center.z) index |= 4;

            int s = 1 << index;
            if ((flags & s) == 0) {
                Node*& node = _nodes[index];
                if (node && !node->unlocate(obj, aabb, aabbCorners, depth)) {
                    delete node;
                    node = NULL;
                }
                flags |= s;
            }
        }
        for (int i = 0; i < 8; i++) {
            if (_nodes[i]) return true;
        }
        _branch = false;
    }
    return _objects || _branch;
}

void CSQuadTree::Node::selectAll(CSSet<CSSceneObject*>* objs) const {
    if (_objects) {
        foreach (CSSceneObject*, obj, _objects) objs->addObject(obj);
    }
    if (_branch) {
        for (int i = 0; i < 8; i++) {
            if (_nodes[i]) _nodes[i]->selectAll(objs);
        }
    }
}

void CSQuadTree::Node::select(const std::function<CSCollisionResult(const CSABoundingBox&)>& func, CSSet<CSSceneObject*>* objs) const {
    switch (func(_space)) {
        case CSCollisionResultBack:
            selectAll(objs);
            break;
        case CSCollisionResultIntersects:
            if (_objects) {
                foreach (CSSceneObject*, obj, _objects) objs->addObject(obj);
            }
            if (_branch) {
                for (int i = 0; i < 8; i++) {
                    if (_nodes[i]) _nodes[i]->select(func, objs);
                }
            }
            break;
    }
}

//=============================================================================================================
static int depthFromGrid(const CSABoundingBox& space, int grid) {
    if (grid <= 0) return 0;
    CSVector3 d = space.maximum - space.minimum;
    float dd = CSMath::min(CSMath::min(d.x, d.y), d.z);
    int gd = (int)(dd / grid);

    int depth = 0;
    while (gd > 1) {
        gd >>= 1;
        depth++;
    }
    return depth;
}

CSQuadTree::CSQuadTree(const CSABoundingBox& space, int grid) : _top(new Node(space)), _grid(grid), _depth(depthFromGrid(space, grid)) {

}

CSQuadTree::~CSQuadTree() {
    delete _top;
}

void CSQuadTree::resize(const CSABoundingBox& space, int grid) {
    int depth;
    if (_top->space() != space) depth = depthFromGrid(space, grid);
    else if (_grid != grid) {
        depth = depthFromGrid(space, grid);
        if (_depth == depth) {
            _grid = grid;
            return;
        }
    }
    else return;

    CSSet<CSSceneObject*> objs;
    selectAll(&objs);
    delete _top;
    _top = new Node(space);
    _grid = grid;
    _depth = depth;

    CSABoundingBox aabb;
    CSVector3 aabbCorners[8];
    synchronized (_lock) {
        for (CSSet<CSSceneObject*>::Iterator i = objs.iterator(); i.remaining(); i.next()) {
            CSSceneObject* obj = i.object();
            if (obj->getAABB(aabb)) {
                aabb.getCorners(aabbCorners);
                _top->locate(obj, aabb, aabbCorners, depth);
            }
        }
    }
}

void CSQuadTree::locate(CSSceneObject* obj) {
    CSABoundingBox aabb;
    if (obj->getAABB(aabb)) {
        CSVector3 aabbCorners[8];
        aabb.getCorners(aabbCorners);
        synchronized (_lock) {
            _top->locate(obj, aabb, aabbCorners, _depth);
        }
    }
}

void CSQuadTree::unlocate(CSSceneObject* obj) {
    CSABoundingBox aabb;
    if (obj->getAABB(aabb)) {
        CSVector3 aabbCorners[8];
        aabb.getCorners(aabbCorners);
        synchronized (_lock) {
            _top->unlocate(obj, aabb, aabbCorners, _depth);
        }
    }
}

void CSQuadTree::relocate(CSSceneObject* obj, const CSABoundingBox* naabb) {
    CSABoundingBox paabb;
    if (obj->getAABB(paabb)) {
        if (naabb) {
            CSVector3 center = _top->space().center();
            int r = 1 << _depth;

            if ((int)((paabb.minimum.x - center.x) / r) != (int)((naabb->minimum.x - center.x) / r) ||
                (int)((paabb.minimum.y - center.y) / r) != (int)((naabb->minimum.y - center.y) / r) ||
                (int)((paabb.minimum.z - center.z) / r) != (int)((naabb->minimum.z - center.z) / r) ||
                (int)((paabb.maximum.x - center.x) / r) != (int)((naabb->maximum.x - center.x) / r) ||
                (int)((paabb.maximum.y - center.y) / r) != (int)((naabb->maximum.y - center.y) / r) ||
                (int)((paabb.maximum.z - center.z) / r) != (int)((naabb->maximum.z - center.z) / r)) {

                CSVector3 aabbCorners[8];
                paabb.getCorners(aabbCorners);
                synchronized (_lock) {
                    _top->unlocate(obj, paabb, aabbCorners, _depth);
                    naabb->getCorners(aabbCorners);
                    _top->locate(obj, *naabb, aabbCorners, _depth);
                }
            }
        }
        else {
            CSVector3 aabbCorners[8];
            paabb.getCorners(aabbCorners);
            synchronized (_lock) {
                _top->unlocate(obj, paabb, aabbCorners, _depth);
            }
        }
    }
}

void CSQuadTree::selectAll(CSSet<CSSceneObject*>* objs) const {
    synchronized (_lock) {
        _top->selectAll(objs);
    }
}

void CSQuadTree::select(const CSVector3& pos, CSSet<CSSceneObject*>* objs) const {
    auto func = [pos](const CSABoundingBox& aabb) -> CSCollisionResult {
        if (aabb.minimum.x <= pos.x && pos.x <= aabb.maximum.x &&
            aabb.minimum.y <= pos.y && pos.y <= aabb.maximum.y &&
            aabb.minimum.z <= pos.z) {
            return CSCollisionResultIntersects;
        }
        return CSCollisionResultFront;
    };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSBoundingFrustum& frustum, CSSet<CSSceneObject*>* objs) const {
    auto func = [frustum](const CSABoundingBox& aabb) -> CSCollisionResult { return frustum.intersects(aabb); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSRay& ray, CSSet<CSSceneObject*>* objs) const {
    auto func = [ray](const CSABoundingBox& aabb) -> CSCollisionResult { return ray.intersects(aabb, 0) ? CSCollisionResultIntersects : CSCollisionResultFront; };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSBoundingSphere& sphere, CSSet<CSSceneObject*>* objs) const {
    auto func = [sphere](const CSABoundingBox& aabb) -> CSCollisionResult { return sphere.intersects(aabb, CSCollisionFlagBack); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSBoundingCapsule& capsule, CSSet<CSSceneObject*>* objs) const {
    auto func = [capsule](const CSABoundingBox& aabb) -> CSCollisionResult { return capsule.intersects(aabb, CSCollisionFlagBack); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSABoundingBox& box, CSSet<CSSceneObject*>* objs) const {
    auto func = [box](const CSABoundingBox& aabb) -> CSCollisionResult { return box.intersects(aabb, CSCollisionFlagBack); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSOBoundingBox& box, CSSet<CSSceneObject*>* objs) const {
    auto func = [box](const CSABoundingBox& aabb) -> CSCollisionResult { return box.intersects(aabb, CSCollisionFlagBack); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSBoundingMesh& mesh, CSSet<CSSceneObject*>* objs) const {
    auto func = [mesh](const CSABoundingBox& aabb) -> CSCollisionResult { return mesh.intersects(aabb, CSCollisionFlagBack); };
    synchronized (_lock) {
        _top->select(func, objs);
    }
}

void CSQuadTree::select(const CSCollider* collider, CSSet<CSSceneObject*>* objs) const {
    foreach (const CSColliderFragment&, frag, collider) {
        switch (frag.type()) {
            case CSColliderFragment::TypeBox:
                select(frag.asBox(), objs);
                break;
            case CSColliderFragment::TypeSphere:
                select(frag.asSphere(), objs);
                break;
            case CSColliderFragment::TypeCapsule:
                select(frag.asCapsule(), objs);
                break;
            case CSColliderFragment::TypeMesh:
                select(frag.asMesh(), objs);
                break;
        }
    }
}

CSSet<CSSceneObject*>* CSQuadTree::selectAll() const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    selectAll(objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSVector3& pos) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(pos, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSBoundingFrustum& frustum) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(frustum, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSRay& ray) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(ray, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSBoundingSphere& sphere) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(sphere, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSBoundingCapsule& capsule) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(capsule, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSABoundingBox& box) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(box, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSOBoundingBox& box) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(box, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSBoundingMesh& mesh) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(mesh, objs);
    return objs;
}

CSSet<CSSceneObject*>* CSQuadTree::select(const CSCollider* collider) const {
    CSSet<CSSceneObject*>* objs = CSSet<CSSceneObject*>::set();
    select(collider, objs);
    return objs;
}