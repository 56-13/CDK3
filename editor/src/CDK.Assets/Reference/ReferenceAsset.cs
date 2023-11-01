using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using CDK.Assets.Updaters;
using CDK.Assets.Scenes;

namespace CDK.Assets.Reference
{
    public class ReferenceAsset : Asset
    {
        private Asset _Root;
        public Asset Root
        {
            set
            {
                Load();
                SetSharedProperty(ref _Root, value);
            }
            get
            {
                Load();
                return _Root;
            }
        }

        private Asset _Selection;
        public Asset Selection
        {
            set
            {
                Load();
                SetProperty(ref _Selection, value);
            }
            get
            {
                Load();
                return _Selection;
            }
        }

        private int _Depth;
        public int Depth
        {
            set
            {
                Load();
                SetSharedProperty(ref _Depth, value);
            }
            get
            {
                Load();
                return _Depth;
            }
        }

        private bool _AllowEmpty;
        public bool AllowEmpty
        {
            set
            {
                Load();
                SetSharedProperty(ref _AllowEmpty, value);
            }
            get
            {
                Load();
                return _AllowEmpty;
            }
        }

        public ReferenceAsset()
        {
            _Depth = 1;
        }

        public ReferenceAsset(ReferenceAsset other, bool content) : base(other, content)
        {
            other.Load();

            AssetManager.Instance.InvokeRedirection(() =>
            {
                _Root = AssetManager.Instance.GetRedirection(other._Root);
                if (content) _Selection = AssetManager.Instance.GetRedirection(other._Selection);
            });
            _Depth = other._Depth;
            _AllowEmpty = other._AllowEmpty;
        }

        public override AssetType Type => AssetType.Reference;
        public override Asset Clone(bool content) => new ReferenceAsset(this, content);
        internal override void AddRetains(ICollection<string> retains)
        {
            Load();

            if (_Root != null)
            {
                retains.Add(_Root.Key);
            }
            if (_Selection != null)
            {
                retains.Add(_Selection.Key);
            }
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            Load();

            if (_Root == element || _Selection == element)
            {
                from = this;
                return true;
            }
            else
            {
                from = null;
                return false;
            }
        }

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (ReferenceAsset)asset;

            other.Load();

            return other._Depth == _Depth && other._AllowEmpty == _AllowEmpty;
        }

        public override SceneComponent NewSceneComponent() => Selection?.NewSceneComponent();
        public override Scene NewScene() => Selection?.NewScene();

        private void BuildImpl(BinaryWriter writer)
        {
            if (_Root == null) throw new AssetException(this, "값이 입력되지 않았습니다.");

            BuildReference(writer, _Root, _Selection, _Depth, _AllowEmpty);
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
                writer.WriteStartElement("referenceAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);

                var project = Project;

                if (_Root != null)
                {
                    if (_Root.Project != project)
                    {
                        throw new AssetException(this, "잘못된 참조입니다.");
                    }
                    writer.WriteAttribute("root", _Root.Key);
                }
                if (_Selection != null)
                {
                    if (_Selection.Project != project)
                    {
                        throw new AssetException(this, "잘못된 참조입니다.");
                    }
                    writer.WriteAttribute("selection", _Selection.Key);
                }
                writer.WriteAttribute("depth", _Depth);
                writer.WriteAttribute("allowEmpty", _AllowEmpty);
                writer.WriteEndElement();
            }
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

                if (node.LocalName != "referenceAsset") throw new XmlException();

                Updater.ValidateReferenceAsset(node);

                Root = node.ReadAttributeAsset("root");
                Selection = node.ReadAttributeAsset("selection");
                Depth = node.ReadAttributeInt("depth");
                AllowEmpty = node.ReadAttributeBool("allowEmpty");
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }
    }
}
