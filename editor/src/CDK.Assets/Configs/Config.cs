using System;
using System.Drawing;
using System.Xml;
using System.IO;

namespace CDK.Assets.Configs
{
    public class Config : IDisposable
    {
        public string[] Locales { private set; get; }
        public Font Font { private set; get; }
        public int Memory { private set; get; }
        public ResourceConfig Resource { private set; get; }
        public TextureConfig Texture { private set; get; }
        public SceneConfig Scene { private set; get; }

        private const string ProjectConfigPath = "project.path";

        private string _ProjectPath;
        public string ProjectPath
        {
            set
            {
                if (_ProjectPath != value)
                {
                    _ProjectPath = value;
                    File.WriteAllText(ProjectConfigPath, value);
                }
            }
            get
            {
                if (_ProjectPath == null && File.Exists(ProjectConfigPath))
                {
                    _ProjectPath = File.ReadAllText(ProjectConfigPath);
                }
                return _ProjectPath;
            }
        }

        public Config(string path)
        {
            var doc = new XmlDocument();

            doc.Load(path);

            var node = doc.ChildNodes[1];

            Memory = node.GetChildNode("memory").ReadAttributeInt("amount");
            Locales = node.GetChildNode("locale").ReadAttributeStrings("locales");
            {
                var subnode = node.GetChildNode("font");
                var name = subnode.ReadAttributeString("name");
                var size = subnode.ReadAttributeFloat("size");
                Font = new Font(name, size);
            }
            Resource = new ResourceConfig(node.GetChildNode("resource"));
            Texture = new TextureConfig(node.GetChildNode("texture"));
            Scene = new SceneConfig(null, node.GetChildNode("scene"));
        }

        public void Dispose()
        {
            Resource.Dispose();
        }
    }
}
    