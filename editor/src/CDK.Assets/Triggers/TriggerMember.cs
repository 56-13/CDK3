
namespace CDK.Assets.Triggers
{
    public class TriggerMember : AssetElement
    {
        public Trigger Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private string _Name;
        public string Name
        {
            set => SetProperty(ref _Name, value);
            get => _Name;
        }

        public TriggerMember(Trigger parent, string name)
        {
            Parent = parent;

            _Name = name;
        }

        public override string ToString() => _Name;
    }
}
