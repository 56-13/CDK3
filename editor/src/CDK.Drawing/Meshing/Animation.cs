using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace CDK.Drawing.Meshing
{
    public class Animation
    {
        private Assimp.Animation _internal;
        public string Name => _internal.Name;
        public float Duration => (float)(_internal.DurationInTicks / _internal.TicksPerSecond);

        internal Animation(Assimp.Animation animation)
        {
            _internal = animation;
        }

        private Vector3 GetValue(double time, List<Assimp.VectorKey> keys)
        {
            if (time <= keys.First().Time) return keys.First().Value.ToVector3();

            for (int i = 1; i < keys.Count; i++)
            {
                var k1 = keys[i];

                if (time < k1.Time)
                {
                    var k0 = keys[i - 1];

                    var weight = (float)((time - k0.Time) / (k1.Time - k0.Time));

                    weight = MathUtil.SmoothStep(weight);

                    return Vector3.Lerp(k0.Value.ToVector3(), k1.Value.ToVector3(), weight);
                }
            }
            return keys.Last().Value.ToVector3();
        }

        private Quaternion GetValue(double time, List<Assimp.QuaternionKey> keys)
        {
            if (time <= keys.First().Time) return keys.First().Value.ToQuaternion();

            for (int i = 1; i < keys.Count; i++)
            {
                var k1 = keys[i];

                if (time < keys[i].Time)
                {
                    var k0 = keys[i - 1];

                    var weight = (float)((time - k0.Time) / (k1.Time - k0.Time));

                    return Quaternion.Slerp(k0.Value.ToQuaternion(), k1.Value.ToQuaternion(), weight);
                }
            }
            return keys.Last().Value.ToQuaternion();
        }

        internal bool GetNodeTransform(string nodeName, float time, out Matrix4x4 nodeTransform)
        {
            nodeTransform = Matrix4x4.Identity;

            var nodeAnimation = _internal.NodeAnimationChannels.FirstOrDefault(n => n.NodeName == nodeName);

            if (nodeAnimation == null) return false;

            var ticks = time * _internal.TicksPerSecond;

            if (nodeAnimation.HasScalingKeys)
            {
                var scaling = GetValue(ticks, nodeAnimation.ScalingKeys);
                nodeTransform.M11 = scaling.X;
                nodeTransform.M22 = scaling.Y;
                nodeTransform.M33 = scaling.Z;
            }
            if (nodeAnimation.HasRotationKeys)
            {
                var rotation = GetValue(ticks, nodeAnimation.RotationKeys);
                nodeTransform *= Matrix4x4.CreateFromQuaternion(rotation);
            }
            if (nodeAnimation.HasPositionKeys)
            {
                var position = GetValue(ticks, nodeAnimation.PositionKeys);
                nodeTransform.M41 = position.X;
                nodeTransform.M42 = position.Y;
                nodeTransform.M43 = position.Z;
            }
            return true;
        }
    }
}
