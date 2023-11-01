using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Support;
using CDK.Assets.Updaters;

namespace CDK.Assets.Version
{
    public class VersionAsset : Asset
    {
        private string _EncryptionKey;
        public string EncryptionKey
        {
            set
            {
                Load();
                SetSharedProperty(ref _EncryptionKey, value);
            }
            get
            {
                Load();
                return _EncryptionKey;
            }
        }

        private int _BundleVersion;
        public int BundleVersion
        {
            set
            {
                Load();
                SetProperty(ref _BundleVersion, value);
            }
            get
            {
                Load();
                return _BundleVersion;
            }
        }

        private int _Revision;
        public int Revision
        {
            private set
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

        private AssetElementList<VersionEntry> _Entries;
        public AssetElementList<VersionEntry> Entries
        {
            get
            {
                Load();
                return _Entries;
            }
        }

        public VersionAsset()
        {
            _EncryptionKey = string.Empty;

            _BundleVersion = 1;

            _Entries = new AssetElementList<VersionEntry>(this);
            _Entries.BeforeListChanged += Entries_BeforeListChanged;
            _Entries.ListChanged += Entries_ListChanged;
        }

        public VersionAsset(VersionAsset other, bool content)
            : base(other, content)
        {
            other.Load();

            _EncryptionKey = other._EncryptionKey;

            if (content)
            {
                _BundleVersion = other._BundleVersion;

                _Revision = other._Revision;

                _Entries = new AssetElementList<VersionEntry>(this);
                foreach (var otherEntry in other._Entries)
                {
                    _Entries.Add(new VersionEntry(this, otherEntry));
                }
                _Entries.BeforeListChanged += Entries_BeforeListChanged;
                _Entries.ListChanged += Entries_ListChanged;
            }
            else
            {
                _BundleVersion = 1;
            }
        }
        
        private void Entry_Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Location"))
            {
                foreach (var entry in Entries)
                {
                    if (sender == entry.Asset)
                    {
                        entry.OnPropertyChanged("TargetLocation");
                    }
                }
            }
        }

        private void Entries_BeforeListChanged(object sender, BeforeListChangedEventArgs<VersionEntry> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (Linked)
                    {
                        _Entries[e.NewIndex].Asset.PropertyChanged -= Entry_Asset_PropertyChanged;
                    }
                    break;
                case ListChangedType.Reset:
                    if (Linked)
                    {
                        foreach (var entry in _Entries)
                        {
                            entry.Asset.PropertyChanged -= Entry_Asset_PropertyChanged;
                        }
                    }
                    break;
            }
        }

        private void Entries_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (_Entries[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    if (Linked) _Entries[e.NewIndex].Asset.PropertyChanged += Entry_Asset_PropertyChanged;
                    break;
                case ListChangedType.ItemChanged:

                    if (e.PropertyDescriptor == null)
                    {
                        if (_Entries[e.NewIndex].Parent != this) throw new InvalidOperationException();
                        if (Linked) _Entries[e.NewIndex].Asset.PropertyChanged += Entry_Asset_PropertyChanged;
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var entry in _Entries)
                    {
                        if (entry.Parent != this) throw new InvalidOperationException();
                        if (Linked) entry.Asset.PropertyChanged += Entry_Asset_PropertyChanged;
                    }
                    break;
            }
        }

        public override AssetType Type => AssetType.Version;
        public override Asset Clone(bool content) => new VersionAsset(this, content);

        protected override bool CompareContent(Asset asset)
        {
            Load();

            var other = (VersionAsset)asset;

            other.Load();

            return _EncryptionKey.Equals(other._EncryptionKey);
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            Load();

            foreach (var entry in _Entries)
            {
                retains.Add(entry.Asset.Key);
            }
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (element is Asset)
            {
                Load();

                foreach (var entry in _Entries)
                {
                    if (entry.Asset == element)
                    {
                        from = this;
                        return true;
                    }
                }
            }
            from = null;
            return false;
        }

        internal override void Link()
        {
            foreach (var entry in _Entries)
            {
                entry.Asset.PropertyChanged += Entry_Asset_PropertyChanged;
            }

            base.Link();
        }

        internal override void Unlink(bool dirty)
        {
            base.Unlink(dirty);

            foreach (var entry in _Entries)
            {
                entry.Asset.PropertyChanged -= Entry_Asset_PropertyChanged;
            }
        }

        protected override bool SaveContent()
        {
            Load();

            Directory.CreateDirectory(Parent.ContentPath);
            Directory.CreateDirectory(ContentPath);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                writer.WriteStartElement("versionAsset");
                
                writer.WriteAttribute("version", Updater.AssetVersion);
                writer.WriteAttribute("bundleVersion", _BundleVersion);
                writer.WriteAttribute("revision", _Revision);
                writer.WriteAttribute("encryptionKey", _EncryptionKey);

                writer.WriteStartElement("entries");
                foreach (var entry in _Entries)
                {
                    entry.Save(writer);
                }
                writer.WriteEndElement();

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

                if (node.LocalName != "versionAsset") throw new XmlException();

                Updater.ValidateVersionAsset(node);

                EncryptionKey = node.ReadAttributeString("encryptionKey");
                BundleVersion = node.ReadAttributeInt("bundleVersion");
                Revision = node.ReadAttributeInt("revision");
                
                _Entries.Clear();
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "entries":
                            foreach (XmlNode entryNode in subnode.ChildNodes)
                            {
                                _Entries.Add(new VersionEntry(this, entryNode));
                            }
                            break;
                    }
                }
            }
        }

        private string GetEncryptionHash(string path)
        {
            Load();

            if (_EncryptionKey == string.Empty) return string.Empty;

            var key = Encoding.UTF8.GetBytes(_EncryptionKey + path.Replace(Path.PathSeparator, '/'));

            return Crypto.GetHash(key, 32);
        }
        
        private void BuildRemote(string dirpath, int revisionFrom, AssetBuildPlatform platform)
        {
            Directory.CreateDirectory(dirpath);

            for (var i = 0; i < _Entries.Count; i++)
            {
                var entry = _Entries[i];

                for (var j = i + 1; j < _Entries.Count; j++)
                {
                    var otherEntry = _Entries[j];

                    if (entry.Path.Equals(otherEntry.Asset))
                    {
                        if (entry.Version == otherEntry.Version)
                        {
                            throw new AssetException(Parent, "버전이 중복된 파일이 있습니다.");
                        }
                        else
                        {
                            var appVersionHigher = false;
                            if (entry.AppVersion0 > otherEntry.AppVersion0) appVersionHigher = true;
                            else if (entry.AppVersion0 == otherEntry.AppVersion0)
                            {
                                if (entry.AppVersion1 > otherEntry.AppVersion1) appVersionHigher = true;
                                else if (entry.AppVersion1 == otherEntry.AppVersion1)
                                {
                                    if (entry.AppVersion2 > otherEntry.AppVersion2) appVersionHigher = true;
                                    else if (entry.AppVersion2 == otherEntry.AppVersion2) throw new AssetException(Parent, "앱버전이 중복된 파일이 있습니다.");
                                }
                            }
                            if (appVersionHigher != (entry.Version > otherEntry.Version))
                            {
                                throw new AssetException(Parent, "앱버전과 버전이 맞지 않는 파일이 있습니다.");
                            }
                        }
                    }
                }
            }

            var buildRemotePath = $"{Name}-{_Revision}.xmf";

            byte[] content;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.WriteLength(_Entries.Count);
                    foreach (var entry in _Entries)
                    {
                        entry.BuildRemote(writer, dirpath, revisionFrom, platform);
                    }
                    if (_EncryptionKey != string.Empty)
                    {
                        var checkSum = 0u;

                        ms.Position = 0;
                        using (var reader = new BinaryReader(ms, Encoding.Default, true))
                        {
                            var checkSumLength = Math.Min((int)ms.Length / 4, 256);
                            for (int i = 0; i < checkSumLength; i++)
                            {
                                checkSum ^= reader.ReadUInt32();
                            }
                        }
                        ms.Position = ms.Length;

                        writer.Write(checkSum);
                    }
                }
                content = ms.ToArray();
            }
            if (_EncryptionKey != string.Empty)
            {
                content = Crypto.Encrypt(content, GetEncryptionHash(buildRemotePath));
            }
            File.WriteAllBytes(dirpath + buildRemotePath, content);
        }

        private List<VersionEntry> GetNewestEntries(bool local, bool remote)
        {
            var entries = new List<VersionEntry>();
            foreach (var entry in _Entries)
            {
                if (entry.Local ? local : remote)
                {
                    VersionEntry duplicateEntry = null;
                    foreach (var otherEntry in entries)
                    {
                        if (entry.Path.Equals(otherEntry.Asset))
                        {
                            duplicateEntry = otherEntry;
                            break;
                        }
                    }
                    if (duplicateEntry == null) entries.Add(entry);
                    else if (entry.Version > duplicateEntry.Version)
                    {
                        entries.Remove(duplicateEntry);
                        entries.Add(entry);
                    }
                    else if (entry.Version == duplicateEntry.Version)
                    {
                        throw new AssetException(Parent, "중복된 버전의 파일이 있습니다.");
                    }
                }
            }
            return entries;
        }

        private void BuildLocal(string dirpath, string verdirpath, bool remote, int revisionFrom)
        {
            var entries = GetNewestEntries(true, remote);

            byte[] content;

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write((ushort)_BundleVersion);
                    writer.Write((ushort)(remote ? _Revision : 0));
                    writer.WriteLength(entries.Count);
                    foreach (var entry in entries)
                    {
                        entry.BuildLocal(writer, dirpath, revisionFrom);
                    }
                    if (_EncryptionKey != string.Empty)
                    {
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
                }
                content = ms.ToArray();
            }

            var versionSubPath = BuildPath;

            if (_EncryptionKey != string.Empty)
            {
                content = Crypto.Encrypt(content, GetEncryptionHash(versionSubPath));
            }

            var versionPath = Path.Combine(verdirpath, versionSubPath);

            Directory.CreateDirectory(Path.GetDirectoryName(versionPath));

            File.WriteAllBytes(versionPath, content);
        }

        private void BuildPlayAssetDelivery(string dirpath, int revisionFrom)
        {
            var buildPath = Path.Combine(dirpath, "play-asset-delivery");

            Directory.CreateDirectory(buildPath);

            var entries = GetNewestEntries(false, true);

            foreach (var entry in entries)
            {
                if (entry.Revision >= revisionFrom)
                {
                    entry.Export(buildPath, AssetBuildPlatform.Android, false);
                }
            }
            var bundleVersionPath = Path.Combine(dirpath, "local", "full", "common", $"{BuildDirPath}{Name}.xmf");
            var packVersionPath = Path.Combine(buildPath, $"{BuildDirPath}{Name}.xmf");

            File.Delete(packVersionPath);
            File.Copy(bundleVersionPath, packVersionPath);
        }

        public void Build(string dirpath, int revisionFrom, AssetBuildPlatform platform = AssetBuildPlatform.All)
        {
            Load();

            BuildLocal(Path.Combine(dirpath, "local", "full"), Path.Combine(dirpath, "local", "full", "common"), true, revisionFrom);
            BuildLocal(Path.Combine(dirpath, "local", "download"), Path.Combine(dirpath, "local", "download_version"), false, revisionFrom);

            switch (platform)
            {
                case AssetBuildPlatform.All:
                    BuildRemote(Path.Combine(dirpath, "remote", "android"), revisionFrom, AssetBuildPlatform.Android);
                    BuildRemote(Path.Combine(dirpath, "remote", "iOS"), revisionFrom, AssetBuildPlatform.iOS);
                    BuildRemote(Path.Combine(dirpath, "remote", "windows"), revisionFrom, AssetBuildPlatform.Windows);
                    BuildPlayAssetDelivery(dirpath, revisionFrom);
                    break;
                case AssetBuildPlatform.Android:
                    BuildRemote(Path.Combine(dirpath + "remote", "android"), revisionFrom, AssetBuildPlatform.Android);
                    BuildPlayAssetDelivery(dirpath, revisionFrom);
                    break;
                case AssetBuildPlatform.iOS:
                    BuildRemote(Path.Combine(dirpath + "remote", "iOS"), revisionFrom, AssetBuildPlatform.iOS);
                    break;
                case AssetBuildPlatform.Windows:
                    BuildRemote(Path.Combine(dirpath + "remote", "windows"), revisionFrom, AssetBuildPlatform.Windows);
                    break;
            }
        }
        
        internal override void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            
        }

        protected override void GetDataPaths(List<string> paths)
        {
            Load();

            var path = ContentPath;
            paths.Add($"{path}.xml");
            paths.Add(path);

            foreach (var subpath in Directory.GetFiles(path))
            {
                var flag = false;
                foreach (var entry in _Entries)
                {
                    if (entry.IsContentPath(subpath))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    paths.Add(subpath);
                }
            }
        }
    }
}
