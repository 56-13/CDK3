using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using Assimp;

using Matrix4x4 = System.Numerics.Matrix4x4;

namespace CDK.Drawing.Meshing
{
    public class Geometry
    {
        public string Name { private set; get; }
        public Node RootNode { private set; get; }
        public MaterialConfig[] MaterialConfigs { private set; get; }
        public ABoundingBox AABB { private set; get; }
        internal Fragment[] Fragments { private set; get; }
        internal Node[] RenderNodes { private set; get; }

        private Dictionary<string, Node> _allNodes;

        internal Geometry(string name, Scene scene)
        {
            Name = name;

            scene.RootNode.Name = string.Empty;     //"RootNode" has duplicate name by OptimizeGraph flag

            RootNode = new Node(scene.RootNode, null);

            MaterialConfigs = scene.Materials.Select(m => new MaterialConfig(m)).ToArray();

            Fragments = scene.Meshes.Select(m => new Fragment(m)).ToArray();

            var renderNodes = new List<Node>();
            _allNodes = new Dictionary<string, Node>();
            Retrieve(RootNode, renderNodes);
            RenderNodes = renderNodes.ToArray();

            if (GetNodeAABB(RootNode, null, 0, true, out var aabb)) AABB = aabb;
        }

        private void Retrieve(Node node, List<Node> renderNodes)
        {
            _allNodes.Add(node.Name, node);
            if (node.HasFragments) renderNodes.Add(node);
            foreach (var child in node.Children) Retrieve(child, renderNodes);
        }

        public int VertexCount
        {
            get
            {
                var count = 0;
                foreach (var node in RenderNodes)
                {
                    foreach (var fi in node.FragmentIndices) count += Fragments[fi].VertexCount;
                }
                return count;
            }
        }

        public int FaceCount
        {
            get
            {
                var count = 0;
                foreach (var node in RenderNodes)
                {
                    foreach (var fi in node.FragmentIndices) count += Fragments[fi].FaceCount;
                }
                return count;
            }
        }

        public int BoneCount
        {
            get
            {
                var count = 0;
                foreach (var node in RenderNodes)
                {
                    foreach (var fi in node.FragmentIndices) count += Fragments[fi].BoneCount;
                }
                return count;
            }
        }

        public void Purge()
        {
            foreach (var frag in Fragments) frag.Purge();
        }

        public bool HasBone(string nodeName)
        {
            foreach (var mesh in Fragments)
            {
                if (mesh.GetBone(nodeName, out _, out _)) return true;
            }
            return false;
        }

        private void GetNodeTransformInternal(Node node, Animation animation, float progress, ref Matrix4x4 transform)
        {
            if (animation == null) transform = node.GlobalTransform;
            else
            {
                if (node.Parent != null) GetNodeTransformInternal(node.Parent, animation, progress, ref transform);
                if (!animation.GetNodeTransform(node.Name, progress, out var nodeTransform)) nodeTransform = node.LocalTransform;
                transform = nodeTransform * transform;
            }
        }

        public void GetNodeTransform(Node node, Animation animation, float progress, out Matrix4x4 transform)
        {
            transform = Matrix4x4.Identity;
            GetNodeTransformInternal(node, animation, progress, ref transform);
        }

        public Matrix4x4 GetNodeTransform(Node node, Animation animation, float progress)
        {
            GetNodeTransform(node, animation, progress, out var transform);
            return transform;
        }

        public Node FindNode(string nodeName) => _allNodes.TryGetValue(nodeName, out var node) ? node : null;

        public bool GetNodeTransform(string nodeName, Animation animation, float progress, out Matrix4x4 transform)
        {
            var node = FindNode(nodeName);
            if (node == null)
            {
                transform = Matrix4x4.Identity;
                return false;
            }
            GetNodeTransform(node, animation, progress, out transform);
            return true;
        }

        internal bool GetNodeAABBInternal(Node node, Animation animation, float progress, bool inclusive, in Matrix4x4 parentTransform, bool leaf, ref ABoundingBox result)
        {
            Matrix4x4 transform;

            if (leaf)
            {
                if (animation == null || !animation.GetNodeTransform(node.Name, progress, out transform)) transform = node.LocalTransform;
                transform *= parentTransform;
            }
            else transform = parentTransform;

            var flag = false;

            var corners = new Vector3[8];

            foreach (var mesh in Fragments)
            {
                if (mesh.GetBone(node.Name, out var bone, out var boneAABB))
                {
                    var boneTransform = bone.OffsetMatrix.ToMatrix() * transform;
                    if (boneTransform.IsIdentity) result.Append(boneAABB);			//TODO:거의 CASE가 없다.
                    else
                    {
                        boneAABB.GetCorners(corners);
                        foreach (var c in corners) result.Append(Vector3.Transform(c, boneTransform));
                    }
                    flag = true;
                }
            }

            foreach (var fi in node.FragmentIndices)
            {
                var frag = Fragments[fi];

                if (!frag.HasBones)
                {
                    if (transform.IsIdentity) result.Append(frag.AABB);			//TODO:거의 CASE가 없다.
                    else {
                        frag.AABB.GetCorners(corners);
                        foreach (var c in corners) result.Append(Vector3.Transform(c, transform));
                    }
                    flag = true;
                }
            }

            if (inclusive)
            {
                foreach (var child in node.Children)
                {
                    if (GetNodeAABBInternal(child, animation, progress, true, transform, true, ref result)) flag = true;
                }
            }
            return flag;
        }

        public bool GetNodeAABB(Node node, Animation animation, float progress, bool inclusive, out Matrix4x4 transform, out ABoundingBox result)
        {
            GetNodeTransform(node, animation, progress, out transform);

            result = ABoundingBox.None;

            return GetNodeAABBInternal(node, animation, progress, inclusive, Matrix4x4.Identity, false, ref result);
        }

        public bool GetNodeAABB(Node node, Animation animation, float progress, bool inclusive, out ABoundingBox result)
        {
            GetNodeTransform(node, animation, progress, out var transform);

            result = ABoundingBox.None;

            return GetNodeAABBInternal(node, animation, progress, inclusive, transform, false, ref result);
        }

        public bool GetNodeAABB(string nodeName, Animation animation, float progress, bool inclusive, out Matrix4x4 transform, out ABoundingBox result)
        {
            var node = FindNode(nodeName);
            if (node == null)
            {
                transform = Matrix4x4.Identity;
                result = ABoundingBox.None;
                return false;
            }
            return GetNodeAABB(node, animation, progress, inclusive, out transform, out result);
        }

        public bool GetNodeAABB(string nodeName, Animation animation, float progress, bool inclusive, out ABoundingBox result)
        {
            var node = FindNode(nodeName);
            if (node == null)
            {
                result = ABoundingBox.None;
                return false;
            }
            return GetNodeAABB(node, animation, progress, inclusive, out result);
        }
        public bool GetAABB(Animation animation, float progress, out ABoundingBox result)
        {
            if (animation == null)
            {
                result = AABB;
                return result != ABoundingBox.Zero;
            }
            return GetNodeAABB(RootNode, animation, progress, true, out result);
        }
        
        public void GetBoneNames(ICollection<string> names)
        {
            foreach (var frag in Fragments)
            {
                for (var i = 0; i < frag.BoneCount; i++) names.Add(frag.GetBone(i).Name);
            }
        }

        public string[] GetBoneNames()
        {
            var names = new List<string>();
            GetBoneNames(names);
            return names.ToArray();
        }
    }
}
