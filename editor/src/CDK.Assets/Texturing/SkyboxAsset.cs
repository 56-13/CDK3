using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Updaters;
using CDK.Assets.Configs;
using CDK.Assets.Scenes;

namespace CDK.Assets.Texturing
{
    public enum SkyboxAssetEncoding
    {
        HighSample,
        HighLinear,
        CompressedSample,
        CompressedLinear
    }

    public class SkyboxAsset : Asset
    {
        public TextureSlot _Content;
        public TextureSlot Content
        {
            get
            {
                Load();
                return _Content;
            }
        }

        public SkyboxAssetEncoding _Encoding;
        public SkyboxAssetEncoding Encoding
        {
            set
            {
                Load();
                if (SetProperty(ref _Encoding, value)) ResetEncoding();
            }
            get
            {
                Load();
                return _Encoding;
            }
        }

        private TextureSlotDescription _TextureDescription;
        public TextureSlotDescription TextureDescription
        {
            get
            {
                Load();
                return _TextureDescription;
            }
        }

        public SkyboxAsset()
        {
            using (new AssetCommandHolder())
            {
                _TextureDescription = new TextureSlotDescription(this, TextureTarget.TextureCubeMap)
                {
                    WrapS = TextureWrapMode.ClampToEdge,
                    WrapT = TextureWrapMode.ClampToEdge,
                    WrapR = TextureWrapMode.ClampToEdge,
                    MinFilter = TextureMinFilter.LinearMipmapLinear,
                    MipmapCount = TextureSlotDescription.MaxMipmapCount
                };
            }

            _Content = new TextureSlot(_TextureDescription, "content");

            ResetEncoding();
        }

        public SkyboxAsset(SkyboxAsset other, bool content) : base(other, content)
        {
            other.Load();

            if (content)
            {
                _TextureDescription = new TextureSlotDescription(this, other._TextureDescription);

                _Content = new TextureSlot(_TextureDescription, other._Content);

                _Encoding = other._Encoding;
            }
            else
            {
                _TextureDescription = new TextureSlotDescription(this, TextureTarget.TextureCubeMap);

                _Content = new TextureSlot(_TextureDescription, "content");

                ResetEncoding();
            }
        }

        private void ResetEncoding()
        {
            switch (_Encoding)
            {
                case SkyboxAssetEncoding.HighLinear:
                    _Content.Formats = new RawFormat[] { RawFormat.Rgb8 };
                    break;
                case SkyboxAssetEncoding.HighSample:
                    _Content.Formats = new RawFormat[] { RawFormat.Srgb8 };
                    break;
                case SkyboxAssetEncoding.CompressedLinear:
                    _Content.Formats = AssetManager.Instance.Config.Texture.CompressedRgb;
                    break;
                case SkyboxAssetEncoding.CompressedSample:
                    _Content.Formats = AssetManager.Instance.Config.Texture.CompressedSrgb;
                    break;
            }
        }

        public override AssetType Type => AssetType.Skybox;
        public override Asset Clone(bool content) => new SkyboxAsset(this, content);
        public override SceneComponent NewSceneComponent() => new SkyboxComponent(this);
        public override Scene NewScene()
        {
            var scene = new Scene(this) { Seperated = true };
            scene.Config = new SceneConfig(null, scene.Config);     //skybox 설정이 다른 곳에 적용되므로 복사본 사용

            var env = new Environment(scene.Config.Preferences[0], scene.Config.Environments[0]);
            env.EnvironmentConfig.Skybox = this;
            scene.Children.Add(env);

            var obj = new SkyboxComponent(this) { Fixed = true };
            scene.Children.Add(obj);
            scene.SelectedComponent = obj;

            return scene;
        }

        public override void Import(string path)
        {
            AssetManager.Instance.Purge();

            Load();

            _Content.Bitmap = BitmapTexture.Load(path);
        }

        public override void Export(string dirpath)
        {
            Load();

            var image = _Content.Bitmap;

            if (image != null)
            {
                dirpath = Path.Combine(dirpath, $"{Name}.png");

                image.Save(dirpath);
            }
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        _Content.Build(writer, platform);
                    }
                }
            }
            else
            {
                _Content.Build(writer, platform);
            }
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("skyboxAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("encoding", _Encoding);
                _TextureDescription.Save(writer, "format");
                writer.WriteEndElement();
            }
            _Content.Save();

            return true;
        }

        protected override void LoadContent()
        {
            var path = ContentXmlPath;

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "skyboxAsset") throw new XmlException();

                Updater.ValidateTextureAsset(node);

                Encoding = node.ReadAttributeEnum<SkyboxAssetEncoding>("encoding");

                _TextureDescription.Load(node, "format");

                _Content.Load();
            }
        }
         
        protected override void GetDataPaths(List<string> paths)
        {
            Load();

            paths.Add(ContentXmlPath);
            _Content.GetDataPath(paths);
        }
    }
}

