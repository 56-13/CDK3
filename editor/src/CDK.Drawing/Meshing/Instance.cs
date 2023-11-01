using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using Matrix4x4 = System.Numerics.Matrix4x4;

namespace CDK.Drawing.Meshing
{
    public class Instance
    {
        public Geometry Geometry { private set; get; }
        public ISkinContainer Skin { set; get; }

        private int _FrameDivision;
        public int FrameDivision
        {
            set
            {
                if (_FrameDivision != value)
                {
                    _FrameDivision = value;

                    if (_Animation != null)
                    {
                        var duration = _Animation.Duration;

                        if (_internalProgress > 0 && _internalProgress < duration)
                        {
                            var p = GetFrameDividedProgress(_internalProgress, _FrameDivision);
                            if (_Progress != p)
                            {
                                _Progress = p;
                                _dirtyTransforms = true;
                            }
                        }
                    }
                }
            }
            get => _FrameDivision;
        }

        public static float GetFrameDividedProgress(float progress, int frameDivision)
        {
            return frameDivision > 0 ? (float)Math.Round(progress * frameDivision) / frameDivision : progress;
        }

        private Animation _Animation;
        public Animation Animation
        {
            set
            {
                if (_Animation != value)
                {
                    _Animation = value;
                    _internalProgress = _Progress = 0;
                    _dirtyTransforms = true;
                }
            }
            get => _Animation;
        }

        private float _internalProgress;

        private float _Progress;
        public float Progress
        {
            set
            {
                if (_Animation != null)
                {
                    var duration = _Animation.Duration;

                    if (value <= 0) value = _internalProgress = 0;
                    else if (value >= duration) value = _internalProgress = duration;
                    else
                    {
                        _internalProgress = value;
                        value = GetFrameDividedProgress(_internalProgress, _FrameDivision);
                    }
                    if (_Progress != value)
                    {
                        _Progress = value;
                        _dirtyTransforms = true;
                    }
                }
            }
            get => _Progress;
        }
        public float Duration => _Animation != null ? _Animation.Duration : 0;
        public float Remaining => Math.Max(Duration - _Progress, 0);

        private Dictionary<string, Matrix4x4> _customTransforms;
        private Dictionary<string, Matrix4x4> _transforms;
        private bool _dirtyTransforms;

        private const int BoneBufferSliceLife = 300;

        public Instance(Geometry geometry)
        {
            Geometry = geometry;
        }

        public Instance(Instance other)
        {
            Geometry = other.Geometry;

            Skin = other.Skin;

            _Animation = other._Animation;
            _internalProgress = other._internalProgress;
            _Progress = other._Progress;
            _FrameDivision = other._FrameDivision;

            other.UpdateTransforms();

            if (other._customTransforms != null) _customTransforms = new Dictionary<string, Matrix4x4>(other._customTransforms);
            if (other._transforms != null) _transforms = new Dictionary<string, Matrix4x4>(other._transforms);
        }

        public void SetCustomTransform(string nodeName, in Matrix4x4 transform)
        {
            if (_customTransforms == null) _customTransforms = new Dictionary<string, Matrix4x4>();
            else if (_customTransforms.TryGetValue(nodeName, out var pt) && pt == transform) return;
            _customTransforms[nodeName] = transform;
            _dirtyTransforms = true;
        }

        public void RemoveCustomTransform(string nodeName)
        {
            if (_customTransforms != null && _customTransforms.Remove(nodeName))
            {
                if (_customTransforms.Count == 0) _customTransforms = null;
                _dirtyTransforms = true;
            }
        }

        public void ClearCustomTransforms()
        {
            if (_customTransforms != null)
            {
                _customTransforms = null;
                _dirtyTransforms = true;
            }
        }

        public void Rewind()
        {
            Progress = 0;
        }

        public bool Update(float delta)
        {
            if (_Animation == null) return false;
            var duration = _Animation.Duration;
            if (_internalProgress >= duration) return true;
            Progress = _internalProgress + delta;
            return false;
        }

        public void GetNodeTransform(Node node, out Matrix4x4 result)
        {
            UpdateTransforms();
            if (_transforms != null && _transforms.TryGetValue(node.Name, out result)) return;
            Geometry.GetNodeTransform(node, null, 0, out result);
        }

        public Matrix4x4 GetNodeTransform(Node node)
        {
            GetNodeTransform(node, out var result);
            return result;
        }

        public bool GetNodeTransform(string nodeName, out Matrix4x4 transform)
        {
            UpdateTransforms();
            if (_transforms != null && _transforms.TryGetValue(nodeName, out transform)) return true;
            return Geometry.GetNodeTransform(nodeName, null, 0, out transform);
        }

        public bool GetNodeAABB(Node node, bool inclusive, out Matrix4x4 transform, out ABoundingBox result)
        {
            GetNodeTransform(node, out transform);
            result = ABoundingBox.None;
            return Geometry.GetNodeAABBInternal(node, _Animation, _Progress, inclusive, Matrix4x4.Identity, false, ref result);
        }

        public bool GetNodeAABB(Node node, bool inclusive, out ABoundingBox result)
        {
            GetNodeTransform(node, out var transform);
            result = ABoundingBox.None;
            return Geometry.GetNodeAABBInternal(node, _Animation, _Progress, inclusive, transform, false, ref result);
        }

        public bool GetAABB(out ABoundingBox result)
        {
            if (_Animation == null && _customTransforms == null)
            {
                result = Geometry.AABB;
                return true;
            }
            return GetNodeAABB(Geometry.RootNode, true, out result);
        }

        public bool GetNodeAABB(string nodeName, bool inclusive, out Matrix4x4 transform, out ABoundingBox result)
        {
            var node = Geometry.FindNode(nodeName);
            result = ABoundingBox.None;
            if (node == null)
            {
                transform = Matrix4x4.Identity;
                return false;
            }
            GetNodeTransform(node, out transform);
            return Geometry.GetNodeAABBInternal(node, _Animation, _Progress, inclusive, Matrix4x4.Identity, false, ref result);
        }

        public bool GetNodeAABB(string nodeName, bool inclusive, out ABoundingBox result)
        {
            var node = Geometry.FindNode(nodeName);
            result = ABoundingBox.None;
            if (node == null) return false;
            GetNodeTransform(node, out var transform);
            return Geometry.GetNodeAABBInternal(node, _Animation, _Progress, inclusive, transform, false, ref result);
        }

        private void UpdateTransforms(Node node, in Matrix4x4 parentTransform)
        {
            if (_customTransforms != null && _customTransforms.TryGetValue(node.Name, out var transform))
            {
                if (_Animation != null && _Animation.GetNodeTransform(node.Name, _Progress, out var animationTransform))
                {
                    if (Matrix4x4.Invert(node.LocalTransform, out var nodeTransformInv)) throw new InvalidOperationException();
                    transform = nodeTransformInv * animationTransform * transform;
                }
            }
            else
            {
                if (_Animation == null || !_Animation.GetNodeTransform(node.Name, _Progress, out transform)) transform = node.LocalTransform;
                transform *= parentTransform;
            }

            _transforms[node.Name] = transform;

            foreach (var child in node.Children) UpdateTransforms(child, transform);
        }

        private void UpdateTransforms()
        {
            if (_dirtyTransforms)
            {
                if (_Animation != null || _customTransforms != null)
                {
                    if (_transforms == null) _transforms = new Dictionary<string, Matrix4x4>();
                    UpdateTransforms(Geometry.RootNode, Matrix4x4.Identity);
                }
                else _transforms = null;

                _dirtyTransforms = false;
            }
        }

        public void Draw(Graphics graphics, InstanceLayer layer, float progress, int random, IEnumerable<VertexArrayInstance> instances = null)
        {
            var flag = false;

            foreach (var node in Geometry.RenderNodes)
            {
                foreach (var fi in node.FragmentIndices)
                {
                    var frag = Geometry.Fragments[fi];

                    var currInstances = instances;

                    var skin = Skin[frag.MaterialIndex];

                    if (SkinUtil.Apply(skin, graphics, layer, progress, random, ref currInstances, !flag))
                    {
                        flag = true;

                        if (frag.HasBones)
                        {
                            BoneKey boneKey;

                            BufferSlice boneBufferSlice = null;

                            var upload = false;

                            if (_customTransforms != null)
                            {
                                boneKey = new BoneKey(this, frag, _Animation, _Progress);
                                if (_dirtyTransforms) upload = true;
                            }
                            else boneKey = new BoneKey(null, frag, _Animation, _Progress);

                            boneBufferSlice = (BufferSlice)ResourcePool.Instance.Get(boneKey);

                            if (boneBufferSlice != null && boneBufferSlice.Count != frag.BoneCount)
                            {
                                ResourcePool.Instance.Remove(boneKey);
                                boneBufferSlice = null;
                            }
                            if (boneBufferSlice == null)
                            {
                                boneBufferSlice = Buffers.NewSlice(BufferTarget.UniformBuffer, 64, frag.BoneCount, GraphicsContext.Instance.MaxUniformBlockSize / 64, BufferUsageHint.DynamicDraw);
                                ResourcePool.Instance.Add(boneKey, boneBufferSlice, BoneBufferSliceLife, false);
                                upload = true;
                            }
                            if (upload)
                            {
                                UpdateTransforms();

                                var boneTransforms = new Matrix4x4[frag.BoneCount];
                                for (var i = 0; i < frag.BoneCount; i++)
                                {
                                    var bone = frag.GetBone(i);
                                    if (GetNodeTransform(bone.Name, out var bt)) boneTransforms[i] = bone.OffsetMatrix.ToMatrix() * bt;
                                }
                                boneBufferSlice.Buffer.UploadSub(boneTransforms, boneBufferSlice.Offset);
                            }

                            ABoundingBox? aabb = null;
                            if (_customTransforms == null) aabb = Geometry.AABB;
                            frag.Draw(graphics, boneBufferSlice, aabb, currInstances);
                            //계산 편의를 위해 랜더영역을 결정하는 AABB는 포즈를 사용, 애니메이션이 포즈에서 크게 벗어나고 블랜딩 드로우가 섞일 경우 랜더순서가 달라보이는 경우가 드물게 있을 수 있음.
                        }
                        else
                        {
                            UpdateTransforms();
                            graphics.Transform(GetNodeTransform(node));
                            frag.Draw(graphics, null, frag.AABB, currInstances);
                        }
                        graphics.Reset();
                    }
                }
            }
            if (flag) graphics.Pop();
        }
    }
}
