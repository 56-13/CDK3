using System.Numerics;
using System.IO;

using CDK.Drawing;

namespace CDK.Assets.Scenes
{
    public abstract class World : SceneComponent
    {
        public enum BuildType
        {
            None,
            GroundRef,      //not used yet
            Ground,
            TerrainRef
        }

        protected World() { }
        protected World(World other, bool binding) : base(other, binding, false) { }
        public abstract ABoundingBox Space { get; }
        public abstract int Grid { get; }
        public abstract bool Intersects(in Ray ray, CollisionFlags flags, out float distance, out Hit hit);
        public abstract float GetZ(in Vector3 pos);
        internal abstract void Rewind();
        internal abstract void Update(float delta);
        internal virtual void Build(BinaryWriter writer) { }
    }
}
