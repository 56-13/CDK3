using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;

using CDK.Assets.Support;
using CDK.Assets.Updaters;

namespace CDK.Assets.Containers
{
    public class FileAsset : Asset
    {
        private string _EncryptionKey;
        public string EncryptionKey
        {
            set
            {
                Load();
                SetSharedProperty(ref _EncryptionKey, value ?? string.Empty);
            }
            get 
            {
                Load();
                return _EncryptionKey;
            }
        }

        private int _Revision;
        public int Revision
        {
            set
            {
                Load();
                SetProperty(ref _Revision, value);
            }
            get
            {
                Load();
                return _Revision;
            }
        }

        private bool _Compression;
        public bool Compression
        {
            set
            {
                Load();
                SetSharedProperty(ref _Compression, value);
            }
            get
            {
                Load();
                return _Compression;
            }
        }

        private bool _MultiPlatforms;
        public override bool MultiPlatforms
        {
            set
            {
                Load();
                SetSharedProperty(ref _MultiPlatforms, value);
            }
            get
            {
                Load();
                return _MultiPlatforms || base.MultiPlatforms;
            }
        }

        
        public FileAsset()
        {
            _EncryptionKey = string.Empty;
        }

        public FileAsset(FileAsset other, bool content) : base(other, content)
        {
            other.Load();
            _EncryptionKey = other._EncryptionKey;
            if (content) _Revision = other._Revision;
            _Compression = other._Compression;
            _MultiPlatforms = other._MultiPlatforms;
        }

        private string EncryptionHash
        {
            get
            {
                if (_EncryptionKey == string.Empty) return string.Empty;

                var key = Encoding.UTF8.GetBytes($"{_EncryptionKey}{BuildPath.Replace(Path.DirectorySeparatorChar, '/')}");

                return Crypto.GetHash(key, 32);
            }
        }

        public override AssetType Type => AssetType.File;
        public override Asset Clone(bool content) => new FileAsset(this, content);
        internal override string BuildPath
        {
            get
            {
                Load();

                StringBuilder strbuf = new StringBuilder();
                strbuf.Append(Name);
                if (_Revision != 0)
                {
                    strbuf.Append('[');
                    strbuf.Append(_Revision.ToString("D3"));
                    strbuf.Append(']');
                }
                strbuf.Append(".xmf");
                return Path.Combine(BuildDirPath, strbuf.ToString());
            }
        }

        protected override bool CompareContent(Asset asset)
        {
            Load();

            FileAsset other = (FileAsset)asset;

            other.Load();

            return _EncryptionKey == other._EncryptionKey && _Compression == other._Compression && _MultiPlatforms == other._MultiPlatforms;
        }

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.File && type != AssetType.Folder && type != AssetType.SubImage;
        }

        private void BuildImpl(string path, AssetBuildPlatform platform)
        {
            var filePath = Path.Combine(path, GetBuildPlatformDirPath(platform), BuildPath);

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    base.BuildContent(writer, path, platform);
                }
                bytes = ms.ToArray();
            }
            if (_Compression)
            {
                using (var ms = new MemoryStream())
                {
                    using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        gzip.Write(bytes, 0, bytes.Length);
                    }
                    bytes = ms.ToArray();
                }
            }
            if (_EncryptionKey != string.Empty)
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(ms))
                    {
                        writer.Write(bytes);

                        uint checkSum = 0;

                        ms.Position = 0;

                        using (var reader = new BinaryReader(ms, Encoding.Default, true))
                        {
                            var checkSumLength = Math.Min((int)ms.Length / 4, 256);
                            for (var i = 0; i < checkSumLength; i++)
                            {
                                checkSum ^= reader.ReadUInt32();
                            }
                        }
                        ms.Position = ms.Length;

                        writer.Write(checkSum);
                    }
                    bytes = ms.ToArray();
                }
                bytes = Crypto.Encrypt(bytes, EncryptionHash);
            }
            File.WriteAllBytes(filePath, bytes);
        }

        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            Load();

            if (writer != null)
            {
                base.BuildContent(writer, path, platform);
            }
            else
            {
                if (platform != AssetBuildPlatform.All)
                {
                    BuildImpl(path, platform);
                }
                else if (_MultiPlatforms)
                {
                    BuildImpl(path, AssetBuildPlatform.Android);
                    BuildImpl(path, AssetBuildPlatform.iOS);
                    BuildImpl(path, AssetBuildPlatform.Windows);
                }
                else
                {
                    BuildImpl(path, AssetBuildPlatform.All);
                }
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
                writer.WriteStartElement("fileAsset");
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("encryptionKey", _EncryptionKey, string.Empty);
                writer.WriteAttribute("revision", _Revision);
                writer.WriteAttribute("compression", _Compression);
                writer.WriteAttribute("multiPlatforms", _MultiPlatforms);
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

                if (node.LocalName != "fileAsset") throw new XmlException();

                Updater.ValidateFileAsset(node);

                EncryptionKey = node.ReadAttributeString("encryptionKey", string.Empty);

                Revision = node.ReadAttributeInt("revision");

                Compression = node.ReadAttributeBool("compression");

                MultiPlatforms = node.ReadAttributeBool("multiPlatforms");
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
            base.GetDataPaths(paths);
        }
    }
}

