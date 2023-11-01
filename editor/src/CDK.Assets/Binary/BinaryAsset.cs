using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.Binary
{
    public class BinaryAsset : Asset
    {
        private string _Extension;
        public string Extension
        {
            set
            {
                Load();
                SetProperty(ref _Extension, value);
            }
            get
            {
                Load();
                return _Extension;
            }
        }

        private string OriginDataPath => $"{ContentPath}.content.{Extension}";

        private string _DataPath;
        public string DataPath
        {
            get
            {
                if (_DataPath == null && File.Exists(OriginDataPath))
                {
                    _DataPath = ResourceManager.Instance.AddFile(OriginDataPath);
                }

                return _DataPath;
            }
        }

        private class ResetCommand : IAssetCommand
        {
            private BinaryAsset _asset;

            private string _prevDataPath;

            public Asset Asset => _asset;

            public ResetCommand(BinaryAsset asset)
            {
                _asset = asset;

                var dataPath = asset.DataPath;

                if (dataPath != null && File.Exists(dataPath))
                {
                    _prevDataPath = ResourceManager.Instance.AddFile(dataPath);
                }
            }

            public void Undo()
            {
                var dataPath = _asset.DataPath;
                var nextDataPath = File.Exists(dataPath) ? ResourceManager.Instance.AddFile(dataPath) : null;

                _asset._DataPath = _prevDataPath;

                _prevDataPath = nextDataPath;

                _asset.OnPropertyChanged("DataPath");
            }

            public void Redo()
            {
                Undo();
            }

            public bool Merge(IAssetCommand other) => false;
        }

        public BinaryAsset()
        {
            _Extension = string.Empty;
        }

        public BinaryAsset(BinaryAsset other, bool content) : base(other, content)
        {
            if (content)
            {
                other.Load();

                _Extension = other._Extension;
                _DataPath = other.DataPath;
            }
            else
            {
                _Extension = string.Empty;
            }
        }

        public override AssetType Type => AssetType.Binary;
        public override Asset Clone(bool content) => new BinaryAsset(this, content);
        internal override string BuildPath => Path.Combine(BuildDirPath, $"{Name}.{Extension}");
        public override void Import(string path)
        {
            Load();

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new ResetCommand(this));
            }
            Extension = Path.GetExtension(path).Substring(1);
            _DataPath = ResourceManager.Instance.AddFile(path);
            IsDirty = true;
            OnPropertyChanged("DataPath");
        }

        public override void Export(string dirpath)
        {
            Load();

            var dataPath = DataPath;

            if (dataPath != null && File.Exists(dataPath))
            {
                dirpath = Path.Combine(dirpath, $"{Name}.{_Extension}");
                File.Delete(dirpath);
                File.Copy(dataPath, dirpath);
            }
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            string dataPath = DataPath;

            if (writer == null)
            {
                if (dataPath != null && File.Exists(dataPath))
                {
                    var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                    File.Delete(filePath);
                    File.Copy(dataPath, filePath);
                    File.SetLastWriteTime(filePath, DateTime.Now);
                }
            }
            else
            {
                if (dataPath != null && File.Exists(dataPath))
                {
                    using (var fs = new FileStream(dataPath, FileMode.Open))
                    {
                        writer.WriteLength((int)fs.Length);
                        var buffer = new byte[4096];
                        for (; ; )
                        {
                            int length = fs.Read(buffer, 0, buffer.Length);
                            if (length == 0) break;
                            writer.Write(buffer, 0, length);
                        }
                    }
                }
                else writer.WriteLength(0);
            }
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
                writer.WriteStartElement("binaryAsset");
                writer.WriteAttributeString("version", Updater.AssetVersion.ToString());
                writer.WriteAttributeString("extension", _Extension);
                writer.WriteEndElement();

                if (_DataPath != null)
                {
                    File.Delete(OriginDataPath);
                    File.Move(_DataPath, OriginDataPath);

                    _DataPath = null;
                }
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

                if (node.LocalName != "binaryAsset") throw new XmlException();

                Updater.ValidateBinaryAsset(node);

                Extension = node.ReadAttributeString("extension");
                if (AssetManager.Instance.CommandEnabled)
                {
                    AssetManager.Instance.Command(new ResetCommand(this));
                }
                _DataPath = null;
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
            paths.Add(OriginDataPath);
        }
    }
}
