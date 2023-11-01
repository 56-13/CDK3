using System;

namespace CDK.Drawing.Meshing
{
    internal struct BoneKey : IEquatable<BoneKey>
    {
        public Instance Instance { private set; get; }
        public Fragment Mesh { private set; get; }
        public Animation Animation { private set; get; }
        public float Progress { private set; get; }

        public BoneKey(Instance instance, Fragment mesh, Animation animation, float progress)
        {
            Instance = instance;
            Mesh = mesh;
            Animation = animation;
            Progress = progress;
        }

        public static bool operator ==(in BoneKey a, in BoneKey b) => a.Equals(b);
        public static bool operator !=(in BoneKey a, in BoneKey b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            if (Instance != null) hash.Combine(Instance.GetHashCode());
            hash.Combine(Mesh.GetHashCode());
            if (Animation != null) hash.Combine(Animation.GetHashCode());
            hash.Combine(Progress.GetHashCode());
            return hash;
        }

        public bool Equals(BoneKey other)
        {
            return Instance == other.Instance &&
                Mesh == other.Mesh &&
                Animation == other.Animation &&
                MathUtil.NearEqual(Progress, other.Progress);
        }

        public override bool Equals(object obj) => obj is BoneKey other && Equals(other);
    }
}
