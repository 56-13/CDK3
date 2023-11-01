using System.Collections.Generic;
using System.Xml;

using CDK.Assets.Containers;

namespace CDK.Assets.Configs
{
    public class SceneConfig : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public AssetElementList<PreferenceConfig> Preferences { private set; get; }
        public AssetElementList<EnvironmentConfig> Environments { private set; get; }
        public AssetElementList<GroundConfig> Grounds { private set; get; }

        public SceneConfig(AssetElement parent) : this(parent, AssetManager.Instance.Config.Scene)
        {
            
        }

        public SceneConfig(AssetElement parent, SceneConfig other)
        {
            Parent = parent;

            Preferences = new AssetElementList<PreferenceConfig>(this);
            Environments = new AssetElementList<EnvironmentConfig>(this);
            Grounds = new AssetElementList<GroundConfig>(this);

            using (new AssetCommandHolder())
            {
                foreach (var preference in other.Preferences)
                {
                    Preferences.Add(new PreferenceConfig(this, preference));
                }
                foreach (var light in other.Environments)
                {
                    Environments.Add(new EnvironmentConfig(this, light));
                }
                foreach (var ground in other.Grounds)
                {
                    Grounds.Add(new GroundConfig(this, ground));
                }
            }
        }

        public SceneConfig(ProjectAsset project, XmlNode node)
        {
            Parent = project;

            Preferences = new AssetElementList<PreferenceConfig>(this);
            Environments = new AssetElementList<EnvironmentConfig>(this);
            Grounds = new AssetElementList<GroundConfig>(this);

            using (new AssetCommandHolder())
            {
                foreach (XmlNode subnode in node.GetChildNode("preferences").ChildNodes)
                {
                    Preferences.Add(new PreferenceConfig(this, subnode));
                }
                foreach (XmlNode subnode in node.GetChildNode("environments").ChildNodes)
                {
                    Environments.Add(new EnvironmentConfig(this, subnode));
                }
                foreach (XmlNode subnode in node.GetChildNode("grounds").ChildNodes)
                {
                    Grounds.Add(new GroundConfig(this, subnode));
                }
            }
        }

        internal override void AddRetains(ICollection<string> keys)
        {
            foreach (var light in Environments) light.AddRetains(keys);
            foreach (var ground in Grounds) ground.AddRetains(keys);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var light in Environments) if (light.IsRetaining(element, out from)) return true;
            foreach (var ground in Grounds) if (ground.IsRetaining(element, out from)) return true;
            from = null;
            return false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("scene");

            writer.WriteStartElement("preferences");
            foreach (var preference in Preferences) preference.Save(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("environments");
            foreach (var light in Environments) light.Save(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("grounds");
            foreach (var ground in Grounds) ground.Save(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
