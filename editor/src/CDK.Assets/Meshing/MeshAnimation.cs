using CDK.Drawing.Meshing;

namespace CDK.Assets.Meshing
{
    public class MeshAnimation : AssetElement
    {
        public MeshAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public override string GetLocation() => $"{Parent.GetLocation()}.{Name}";
        public Animation Origin { internal set; get; }
        public string Name => Origin.Name;
        public float Duration => Origin.Duration;

        public MeshAnimation(MeshAsset parent, Animation origin)
        {
            Parent = parent;

            Origin = origin;
        }

        public MeshAnimation(MeshAsset parent, MeshAnimation other)
        {
            AssetManager.Instance.AddRedirection(other, this);

            Parent = parent;

            Origin = other.Origin;
        }
    }
}