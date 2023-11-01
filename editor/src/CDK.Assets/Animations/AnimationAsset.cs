using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;
using CDK.Assets.Updaters;

namespace CDK.Assets.Animations
{
    public class AnimationAsset : Asset
    {
        private AnimationFragment _Animation;
        public AnimationFragment Animation
        {
            set
            {
                Load();

                if (_Animation != value)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new AssetPropertyCommand(this, _Animation, value, "Animation"));
                    }
                    if (_Animation != null) _Animation.Parent = null;

                    _Animation = value;

                    IsDirty = true;

                    if (_Animation != null)
                    {
                        if (_Animation.Parent is AnimationFragment parent) parent.Children.Remove(_Animation);
                        else if (_Animation.Parent is AnimationAsset asset) asset.Animation = null;
                        else if (_Animation.Parent is AnimationObject obj)
                        {
                            Debug.Assert(obj.Asset == null);
                            obj.Animation = null;
                        }
                        _Animation.Parent = this;
                    }
                    OnPropertyChanged("Animation");
                }
            }
            get
            {
                Load();
                return _Animation;
            }
        }

        public AnimationAsset()
        {
            _Animation = new AnimationFragment() { Parent = this };
        }
        
        public AnimationAsset(AnimationAsset other, bool content) : base(other, content)
        {
            if (content)
            {
                other.Load();

                if (other._Animation != null) _Animation = new AnimationFragment(other._Animation) { Parent = this };
            }
            else
            {
                _Animation = new AnimationFragment() { Parent = this };
            }
        }

        private Scene _Scene;
        private Scene Scene
        {
            get
            {
                if (_Scene == null)
                {
                    var obj = new AnimationObject(this);
                    _Scene = NewDefaultScene(obj);
                    _Scene.Seperated = false;
                }
                return _Scene;
            }
        }

        public override AssetType Type => AssetType.Animation;
        public override Asset Clone(bool content) => new AnimationAsset(this, content);
        public override bool Spawnable => true;
        public override SceneComponent NewSceneComponent() => new AnimationReferenceObject(this);
        public override Scene NewScene()
        {
            Load();

            var scene = new Scene(this, Scene, true)
            {
                Seperated = true
            };
            return scene;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            Load();

            _Animation?.AddRetains(retains);
            Scene.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            Load();

            if (_Animation != null && _Animation.IsRetaining(element, out from)) return true;

            return Scene.IsRetaining(element, out from);
        }

        private void BuildImpl(BinaryWriter writer)
        {
            if (_Animation == null) throw new AssetException(this, "빈 애니메이션입니다.");
            
            _Animation.Build(writer);
        }
        
        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer == null)
            {
                var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    using (writer = new BinaryWriter(fs))
                    {
                        BuildImpl(writer);
                    }
                }
            }
            else BuildImpl(writer);
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("animationAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);

                _Animation?.Save(writer);

                Scene.Save(writer);

                writer.WriteEndElement();
            }
            return true;
        }

        protected override void LoadContent()
        {
            string path = ContentXmlPath;

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "animationAsset") throw new XmlException();

                Updater.ValidateAnimationAsset(node);

                Animation = null;

                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "animation":
                            Animation = new AnimationFragment();
                            _Animation.Load(subnode);
                            break;
                        case "scene":
                            Scene.Load(subnode);
                            break;
                    }
                }
            }
        }

        internal override void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            Load();

            _Animation?.GetLocaleStrings(strings);
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }
    }
}
