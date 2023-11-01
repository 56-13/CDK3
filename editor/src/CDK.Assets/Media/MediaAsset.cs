using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.Media
{
    public class MediaAsset : Asset
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

        private string OriginDataPath => $"{base.ContentPath}.content.{Extension}";

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
            private MediaAsset _asset;
            private string _prevDataPath;
            public Asset Asset => _asset;

            public ResetCommand(MediaAsset asset)
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
                string dataPath = _asset.DataPath;
                string nextDataPath = File.Exists(dataPath) ? ResourceManager.Instance.AddFile(dataPath) : null;

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

        public MediaAsset()
        {
            _Extension = string.Empty;
        }

        public MediaAsset(MediaAsset other, bool content) : base(other, content)
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

        public override AssetType Type => AssetType.Media;
        public override Asset Clone(bool content) => new MediaAsset(this, content);
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

            var dataPath = DataPath;

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
                            var length = fs.Read(buffer, 0, buffer.Length);
                            if (length == 0) break;
                            writer.Write(buffer, 0, length);
                        }
                    }
                }
                else writer.Write((byte)0);
            }
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);

            if (_DataPath != null)
            {
                File.Delete(OriginDataPath);
                if (_DataPath != null)
                {
                    File.Move(_DataPath, OriginDataPath);
                }
                _DataPath = null;
            }

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("mediaAsset");
                writer.WriteAttribute("version", Updater.AssetVersion); 
                writer.WriteAttribute("extension", _Extension);
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

                if (node.LocalName != "mediaAsset") throw new XmlException();

                Updater.ValidateMediaAsset(node);

                Extension = node.Attributes["extension"].Value;

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
