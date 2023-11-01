using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CDK.Drawing.Meshing
{
    internal class Fragment
    {
        public string Name => _internal.Name;
        public int VertexCount => _internal.VertexCount;
        public int FaceCount => _internal.FaceCount;
        public int BoneCount => _internal.BoneCount;
        public bool HasBones => _internal.HasBones;
        public ABoundingBox AABB { private set; get; }
        public int MaterialIndex => _internal.MaterialIndex;

        private Assimp.Mesh _internal;
        private Dictionary<string, (Assimp.Bone bone, ABoundingBox aabb)> _bones;
        private BoneIndex4[] _boneIndices;
        private Vector4[] _boneWeights;
        private VertexArray _vertices;

        private struct BoneComponent
        {
            public byte Index;
            public float Weight;
        }

        public Fragment(Assimp.Mesh mesh)
        {
            _internal = mesh;

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i].ToVector3();
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
            AABB = new ABoundingBox(min, max);

            if (mesh.HasBones)
            {
                var boneComponentListByVertex = new List<BoneComponent>[mesh.VertexCount];

                _bones = new Dictionary<string, (Assimp.Bone bone, ABoundingBox aabb)>(mesh.Bones.Count);

                for (var i = 0; i < mesh.Bones.Count; i++)
                {
                    var bone = mesh.Bones[i];

                    min = new Vector3(float.MaxValue);
                    max = new Vector3(float.MinValue);

                    foreach (var vw in bone.VertexWeights)
                    {
                        if (boneComponentListByVertex[vw.VertexID] == null) boneComponentListByVertex[vw.VertexID] = new List<BoneComponent>(4);
                        else if (boneComponentListByVertex[vw.VertexID].Count >= 4) throw new NotSupportedException("bone components should be less than 4");

                        boneComponentListByVertex[vw.VertexID].Add(new BoneComponent() { Index = (byte)i, Weight = vw.Weight });

                        var v = mesh.Vertices[vw.VertexID].ToVector3();
                        min = Vector3.Min(min, v);
                        max = Vector3.Max(max, v);
                    }

                    _bones.Add(bone.Name, (bone, new ABoundingBox(min, max)));
                }

                _boneIndices = new BoneIndex4[mesh.VertexCount];
                _boneWeights = new Vector4[mesh.VertexCount];

                for (var i = 0; i < mesh.VertexCount; i++)
                {
                    var list = boneComponentListByVertex[i];

                    if (list != null)
                    {
                        var indexes = BoneIndex4.Zero;
                        var weights = Vector4.Zero;

                        indexes.X = list[0].Index;
                        weights.X = list[0].Weight;

                        if (list.Count >= 2)
                        {
                            indexes.Y = list[1].Index;
                            weights.Y = list[1].Weight;
                        }
                        if (list.Count >= 3)
                        {
                            indexes.Z = list[2].Index;
                            weights.Z = list[2].Weight;
                        }
                        if (list.Count >= 4)
                        {
                            indexes.W = list[3].Index;
                            weights.W = list[3].Weight;
                        }

                        _boneIndices[i] = indexes;
                        _boneWeights[i] = weights;
                    }
                }
            }
        }

        public Assimp.Bone GetBone(int i) => _internal.Bones[i];
        public bool GetBone(string name, out Assimp.Bone bone, out ABoundingBox aabb)
        {
            if (_bones != null && _bones.TryGetValue(name, out var e))
            {
                bone = e.bone;
                aabb = e.aabb;
                return true;
            }
            bone = null;
            aabb = ABoundingBox.Zero;
            return false;
        }

        private void Upload<V>(IntPtr dest, IEnumerable<V> src, int stride, int offset)
        {
            dest += offset;

            foreach (var v in src)
            {
                Marshal.StructureToPtr(v, dest, false);

                dest += stride;
            }
        }

        public void Draw(Graphics graphics, BufferSlice boneBufferSlice, ABoundingBox? aabb = null, IEnumerable<VertexArrayInstance> instances = null)
        {
            if (_vertices == null)
            {
                var layouts = new List<VertexLayout>(6);

                var stride = 12;
                if (_internal.HasTextureCoords(0)) stride += 8;
                if (_internal.HasNormals) stride += 6;
                if (_internal.HasTangentBasis) stride += 6;
                if (_internal.HasBones) stride += 12;

                layouts.Add(new VertexLayout(0, RenderState.VertexAttribPosition, 3, VertexAttribType.Float, false, stride, 0, 0, true));

                var offset = 12;
                if (_internal.HasTextureCoords(0))
                {
                    layouts.Add(new VertexLayout(0, RenderState.VertexAttribTexCoord, 2, VertexAttribType.Float, false, stride, offset, 0, true));
                    offset += 8;
                }
                if (_internal.HasNormals)
                {
                    layouts.Add(new VertexLayout(0, RenderState.VertexAttribNormal, 3, VertexAttribType.HalfFloat, false, stride, offset, 0, true));
                    offset += 6;
                }
                if (_internal.HasTangentBasis)
                {
                    layouts.Add(new VertexLayout(0, RenderState.VertexAttribTangent, 3, VertexAttribType.HalfFloat, false, stride, offset, 0, true));
                    offset += 6;
                }
                if (_internal.HasBones)
                {
                    layouts.Add(new VertexLayout(0, RenderState.VertexAttribBoneIndices, 4, VertexAttribType.UnsignedByte, false, stride, offset, 0, true));
                    offset += 4;
                    layouts.Add(new VertexLayout(0, RenderState.VertexAttribBoneWeights, 4, VertexAttribType.HalfFloat, false, stride, offset, 0, true));
                }

                _vertices = new VertexArray(1, true, layouts.ToArray());

                var dest = Marshal.AllocHGlobal(stride * _internal.VertexCount);

                Upload(dest, _internal.Vertices, stride, 0);

                offset = 12;

                if (_internal.HasTextureCoords(0))
                {
                    Upload(dest, _internal.TextureCoordinateChannels[0], stride, offset);
                    offset += 8;
                }
                if (_internal.HasNormals)
                {
                    Upload(dest, _internal.Normals.Select(v => v.ToHalf3()), stride, offset);
                    offset += 6;
                }
                if (_internal.HasTangentBasis)
                {
                    Upload(dest, _internal.BiTangents.Select(v => v.ToHalf3()), stride, offset);
                    offset += 6;
                }
                if (_internal.HasBones)
                {
                    Upload(dest, _boneIndices, stride, offset);
                    offset += 4;
                    Upload(dest, _boneWeights.Select(v => (Half4)v), stride, offset);
                }
                GraphicsContext.Instance.Invoke(true, (GraphicsApi api) =>
                {
                    _vertices.GetVertexBuffer(0).Upload(api, dest, stride, _internal.VertexCount, BufferUsageHint.StaticDraw);
                    Marshal.FreeHGlobal(dest);
                });
                var indexData = new VertexIndexData(_internal.VertexCount, _internal.FaceCount * 3);
                foreach (var face in _internal.Faces) indexData.AddRange(face.Indices);
                _vertices.IndexBuffer.Upload(indexData, BufferUsageHint.StaticDraw);
            }

            if (boneBufferSlice != null) graphics.DrawVertices(_vertices, boneBufferSlice.Buffer, boneBufferSlice.Offset, PrimitiveMode.Triangles, aabb, instances);
            else graphics.DrawVertices(_vertices, PrimitiveMode.Triangles, aabb, instances);
        }

        public void Purge()
        {
            if (_vertices != null)
            {
                _vertices.Dispose();
                _vertices = null;
            }
        }
    }
}
