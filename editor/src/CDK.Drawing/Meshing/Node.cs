using System;
using System.Collections.Generic;
using System.Numerics;

namespace CDK.Drawing.Meshing
{
    public class Node
    {
        private Assimp.Node _internal;
        public string Name => _internal.Name;
        public Node Parent { private set; get; }
        public Node[] Children { private set; get; }
        public Matrix4x4 LocalTransform => _internal.Transform.ToMatrix();
        public Matrix4x4 GlobalTransform { private set; get; }
        public bool HasFragments => _internal.HasMeshes;
        public IEnumerable<int> FragmentIndices => _internal.MeshIndices;

        internal Node(Assimp.Node node, Node parent)
        {
            _internal = node;

            Parent = parent;

            GlobalTransform = LocalTransform;
            if (parent != null) GlobalTransform *= parent.GlobalTransform;

            Children = new Node[node.Children.Count];
            for (int i = 0; i < node.Children.Count; i++) Children[i] = new Node(node.Children[i], this);
        }

        public override string ToString() => Name;
    }
}
