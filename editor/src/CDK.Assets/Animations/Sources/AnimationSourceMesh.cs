using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Meshing;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sources
{
    public class AnimationSourceMesh : AnimationSource
    {
        public MeshSelection Selection { private set; get; }
        public AssetElementList<string> Bones { private set; get; }

        public AnimationSourceMesh(AssetElement parent) : base(parent)
        {
            Selection = new MeshSelection(this);
            Selection.PropertyChanged += Selection_PropertyChanged;

            Bones = new AssetElementList<string>(this);
        }

        public AnimationSourceMesh(AssetElement parent, AnimationSourceMesh other) : base(parent)
        {
            Selection = new MeshSelection(this, other.Selection);
            Selection.PropertyChanged += Selection_PropertyChanged;

            Bones = new AssetElementList<string>(this, other.Bones, false, false);
        }

        private void Selection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Geometry") Bones.Clear();
        }

        public override AnimationSourceType Type => AnimationSourceType.Mesh;
        public override AnimationSource Clone(AssetElement parent) => new AnimationSourceMesh(parent, this);
        internal override void AddRetains(ICollection<string> retains) => Selection.AddRetains(retains);
        internal override bool IsRetaining(AssetElement obj, out AssetElement from) => Selection.IsRetaining(obj, out from);
        internal override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("mesh");
            writer.WriteAttributes("bones", Bones);
            Selection.Save(writer);
            writer.WriteEndElement();
        }

        internal override void Load(XmlNode node)
        {
            Selection.Load(node.GetChildNode("selection"));
            Bones.Clear();
            foreach (var bone in node.ReadAttributeStrings("bones")) Bones.Add(bone);
        }

        internal override void Build(BinaryWriter writer)
        {
            Selection.Build(writer);
            writer.WriteLength(Bones.Count);
            foreach (var bone in Bones) writer.WriteString(bone);
        }
    }
}
