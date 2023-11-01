using System;
using System.Xml;
using System.IO;

using CDK.Assets.Support;

namespace CDK.Assets.Version
{
    public class VersionEntry : AssetElement
    {
        public VersionAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        private Asset _Asset;
        public Asset Asset
        {
            private set
            {
                if (SetProperty(ref _Asset, value)) OnPropertyChanged("TargetLocation");
            }
            get => _Asset;
        }

        public string TargetLocation => Asset.Location;

        public string Key { private set; get; }
        
        private int _Version;
        public int Version
        {
            private set => SetProperty(ref _Version, value);
            get => _Version;
        }

        private string GetOriginContentPath(AssetBuildPlatform platform)
        {
            switch (platform)
            {
                case AssetBuildPlatform.Android:
                    return System.IO.Path.Combine(Parent.ContentPath, $"{Key}-android-{_Version}");
                case AssetBuildPlatform.iOS:
                    return System.IO.Path.Combine(Parent.ContentPath, $"{Key}-iOS-{_Version}");
                case AssetBuildPlatform.Windows:
                    return System.IO.Path.Combine(Parent.ContentPath, $"{Key}-windows-{_Version}");
            }
            return System.IO.Path.Combine(Parent.ContentPath, $"{Key}-{_Version}");
        }

        private struct Record
        {
            public string ContentPath { set; get; }
            public string CheckSum { set; get; }

            public void Clear()
            {
                ContentPath = null;
                CheckSum = null;
            }
        }

        private Record[] records = new Record[4];
        
        private bool _MultiPlatforms;
        public bool MultiPlatforms
        {
            private set => SetProperty(ref _MultiPlatforms, value);
            get => _MultiPlatforms;
        }


        private int _AppVersion0;
        public int AppVersion0
        {
            set => SetProperty(ref _AppVersion0, value);
            get => _AppVersion0;
        }

        private int _AppVersion1;
        public int AppVersion1
        {
            set => SetProperty(ref _AppVersion1, value);
            get => _AppVersion1;
        }

        private int _AppVersion2;
        public int AppVersion2
        {
            set => SetProperty(ref _AppVersion2, value);
            get => _AppVersion2;
        }

        private bool _Local;
        public bool Local
        {
            set => SetProperty(ref _Local, value);
            get => _Local;
        }

        private string _Path;
        public string Path
        {
            set => SetProperty(ref _Path, value);
            get => _Path;
        }

        public string FileSize
        {
            get
            {
                return _MultiPlatforms ? 
                    $"{new FileInfo(records[1].ContentPath).Length},{ new FileInfo(records[2].ContentPath).Length},{new FileInfo(records[3].ContentPath).Length}" : 
                    new FileInfo(records[0].ContentPath).Length.ToString();
            }
        }

        public string CheckSum
        {
            get
            {
                return _MultiPlatforms ? 
                    $"{records[1].CheckSum},{records[2].CheckSum},{records[3].CheckSum}" : 
                    records[0].CheckSum;
            }
        }

        internal string GetContentPath(AssetBuildPlatform platform)
        {
            return records[_MultiPlatforms ? (int)platform : 0].ContentPath;
        }

        internal bool IsContentPath(string path)
        {
            return _MultiPlatforms ? 
                path.Equals(records[1].ContentPath, StringComparison.OrdinalIgnoreCase) || path.Equals(records[2].ContentPath, StringComparison.OrdinalIgnoreCase) || path.Equals(records[3].ContentPath, StringComparison.OrdinalIgnoreCase) : 
                path.Equals(records[0].ContentPath, StringComparison.OrdinalIgnoreCase);
        }

        private DateTime _ContentTime;
        public DateTime ContentTime
        {
            set => SetProperty(ref _ContentTime, value);
            get => _ContentTime;
        }

        public bool GetDirty()
        {
            foreach (var otherEntry in Parent.Entries)
            {
                if (otherEntry != this && otherEntry._Asset == _Asset && otherEntry._Version > _Version)
                {
                    return false;
                }
            }
            return _ContentTime.CompareTo(_Asset.DataTime) < 0;
        }

        public int Revision { private set; get; }
        
        public VersionEntry(VersionAsset parent, Asset asset, int version)
        {
            Parent = parent;

            Key = AssetManager.NewKey();

            Revision = parent.Revision;

            _Version = version;
            foreach (var otherEntry in parent.Entries)
            {
                if (otherEntry._Asset == asset)
                {
                    if (_Version <= otherEntry._Version)
                    {
                        _Version = otherEntry._Version + 1;
                    }
                }
            }
            _AppVersion0 = 1;

            Update(asset, false);
        }

        public VersionEntry(VersionAsset parent, VersionEntry other)
        {
            Parent = parent;
            _Asset = other._Asset;

            Key = AssetManager.NewKey();

            Revision = other.Revision;

            _Version = other._Version;
            _AppVersion0 = other._AppVersion0;
            _AppVersion1 = other._AppVersion1;
            _AppVersion2 = other._AppVersion2;
            _Local = other._Local;

            _MultiPlatforms = other._MultiPlatforms;

            records[0] = other.records[0];
            records[1] = other.records[1];
            records[2] = other.records[2];
            records[3] = other.records[3];

            _ContentTime = other._ContentTime;
        }

        internal VersionEntry(VersionAsset parent, XmlNode node)
        {
            Parent = parent;

            Key = node.ReadAttributeString("key");
            
            Revision = node.ReadAttributeInt("revision");

            _Asset = node.ReadAttributeAsset("asset");

            if (_Asset == null) throw new XmlException("존재하지 않는 애셋이 있습니다.");

            _Path = node.ReadAttributeString("path");
            _Version = node.ReadAttributeInt("version");
            var appVersion = node.ReadAttributeString("appVersion").Split('.');
            _AppVersion0 = int.Parse(appVersion[0]);
            _AppVersion1 = int.Parse(appVersion[1]);
            _AppVersion2 = int.Parse(appVersion[2]);
            _Local = node.ReadAttributeBool("local");
            _ContentTime = new DateTime(node.ReadAttributeLong("contentTime"));

            _MultiPlatforms = node.ReadAttributeBool("multiPlatforms");

            if (_MultiPlatforms)
            {
                var checkSums = node.ReadAttributeStrings("checkSum");

                records[1].ContentPath = GetOriginContentPath(AssetBuildPlatform.Android);
                records[1].CheckSum = checkSums[0];

                records[2].ContentPath = GetOriginContentPath(AssetBuildPlatform.iOS);
                records[2].CheckSum = checkSums[1];

                records[3].ContentPath = GetOriginContentPath(AssetBuildPlatform.Windows);
                records[3].CheckSum = checkSums[2];

                if (!File.Exists(records[1].ContentPath) || !File.Exists(records[2].ContentPath) || !File.Exists(records[3].ContentPath))
                {
                    throw new IOException("내용 파일을 찾을 수 없습니다.");
                }
            }
            else
            {
                records[0].ContentPath = GetOriginContentPath(AssetBuildPlatform.All);
                records[0].CheckSum = node.ReadAttributeString("checkSum");

                if (!File.Exists(records[0].ContentPath))
                {
                    throw new IOException("내용 파일을 찾을 수 없습니다.");
                }
            }
        }

        public bool Update(bool versionUp)
        {
            return Update(_Asset, versionUp);
        }

        public bool Update(Asset asset, bool versionUp)
        {
            if (versionUp)
            {
                foreach (var entry in Parent.Entries)
                {
                    if (entry._Asset == asset && entry.Version > _Version)
                    {
                        return false;
                    }
                }
            }
            var buildDirPath = $"{Parent.ContentPath}.build";

            var subpath = asset.BuildPath;

            //if (subpath == null) throw new InvalidOperationException();

            try
            {
                if (versionUp) Version++;

                asset.BuildContent(null, buildDirPath, AssetBuildPlatform.All);

                var path0 = System.IO.Path.Combine(buildDirPath, asset.GetBuildPlatformDirPath(AssetBuildPlatform.All), subpath);
                var path1 = System.IO.Path.Combine(buildDirPath, asset.GetBuildPlatformDirPath(AssetBuildPlatform.Android), subpath);
                var path2 = System.IO.Path.Combine(buildDirPath, asset.GetBuildPlatformDirPath(AssetBuildPlatform.iOS), subpath);
                var path3 = System.IO.Path.Combine(buildDirPath, asset.GetBuildPlatformDirPath(AssetBuildPlatform.Windows), subpath);
                var orgpath0 = GetOriginContentPath(AssetBuildPlatform.All);
                var orgpath1 = GetOriginContentPath(AssetBuildPlatform.Android);
                var orgpath2 = GetOriginContentPath(AssetBuildPlatform.iOS);
                var orgpath3 = GetOriginContentPath(AssetBuildPlatform.Windows);

                File.Delete(orgpath0);
                File.Delete(orgpath1);
                File.Delete(orgpath2);
                File.Delete(orgpath3);

                if (asset.MultiPlatforms)
                {
                    if (!File.Exists(path1) || !File.Exists(path2) || !File.Exists(path3)) throw new InvalidOperationException();

                    File.Move(path1, orgpath1);
                    File.Move(path2, orgpath2);
                    File.Move(path3, orgpath3);

                    records[0].Clear();

                    records[1].ContentPath = orgpath1;
                    records[1].CheckSum = Crypto.GetHash(File.ReadAllBytes(orgpath1));

                    records[2].ContentPath = orgpath2;
                    records[2].CheckSum = Crypto.GetHash(File.ReadAllBytes(orgpath2));

                    records[3].ContentPath = orgpath3;
                    records[3].CheckSum = Crypto.GetHash(File.ReadAllBytes(orgpath3));
                }
                else 
                {
                    if (!File.Exists(path0)) throw new InvalidOperationException();

                    File.Move(path0, orgpath0);

                    records[0].ContentPath = orgpath0;
                    records[0].CheckSum = Crypto.GetHash(File.ReadAllBytes(orgpath0));

                    records[1].Clear();
                    records[2].Clear();
                    records[3].Clear();
                }

                Path = subpath.Replace(System.IO.Path.PathSeparator, '/');

                Asset = asset;

                MultiPlatforms = asset.MultiPlatforms;

                ContentTime = DateTime.Now;

                Revision = Parent.Revision;
            }
            catch (Exception e)
            {
                if (versionUp) Version--;

                throw e;
            }
            finally
            {
                Directory.Delete(buildDirPath, true);
            }
            OnPropertyChanged("FileSize");
            OnPropertyChanged("CheckSum");

            return true;
        }
        
        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("entry");
            writer.WriteAttribute("key", Key);
            writer.WriteAttribute("revision", Revision);
            writer.WriteAttribute("asset", _Asset);
            writer.WriteAttribute("path", _Path);
            writer.WriteAttribute("multiPlatforms", _MultiPlatforms);
            writer.WriteAttribute("checkSum", CheckSum);
            writer.WriteAttribute("version", _Version);
            writer.WriteAttribute("appVersion", $"{_AppVersion0}.{_AppVersion1}.{_AppVersion2}");
            writer.WriteAttribute("local", _Local);
            writer.WriteAttribute("contentTime", _ContentTime.Ticks);
            writer.WriteEndElement();

            if (_MultiPlatforms)
            {
                var orgpath1 = GetOriginContentPath(AssetBuildPlatform.Android);
                var orgpath2 = GetOriginContentPath(AssetBuildPlatform.iOS);
                var orgpath3 = GetOriginContentPath(AssetBuildPlatform.Windows);
                if (!records[1].ContentPath.Equals(orgpath1))
                {
                    File.Delete(orgpath1);
                    File.Copy(records[1].ContentPath, orgpath1);
                    records[1].ContentPath = orgpath1;
                }
                if (!records[2].ContentPath.Equals(orgpath2))
                {
                    File.Delete(orgpath2);
                    File.Copy(records[2].ContentPath, orgpath2);
                    records[2].ContentPath = orgpath2;
                }
                if (!records[3].ContentPath.Equals(orgpath3))
                {
                    File.Delete(orgpath3);
                    File.Copy(records[3].ContentPath, orgpath3);
                    records[3].ContentPath = orgpath3;
                }
            }
            else
            {
                var orgpath0 = GetOriginContentPath(AssetBuildPlatform.All);
                if (!records[0].ContentPath.Equals(orgpath0))
                {
                    File.Delete(orgpath0);
                    File.Copy(records[0].ContentPath, orgpath0);
                    records[0].ContentPath = orgpath0;
                }
            }
        }

        internal void BuildLocal(BinaryWriter writer, string dirpath, int revisionFrom)
        {
            writer.WriteString(_Path);
            writer.Write((short)_Version);

            if (Revision >= revisionFrom)
            {
                Export(dirpath);
            }
        }
         
        internal void BuildRemote(BinaryWriter writer, string dirpath, int revisionFrom, AssetBuildPlatform platform)
        {
            var record = records[MultiPlatforms ? (int)platform : 0];

            writer.WriteString(_Path);
            writer.WriteString(Key + '-' + _Version);
            writer.Write((short)_Version);
            writer.Write((byte)_AppVersion0);
            writer.Write((byte)_AppVersion1);
            writer.Write((byte)_AppVersion2);
            writer.Write((int)(new FileInfo(record.ContentPath).Length));
            writer.WriteString(record.CheckSum);

            if (Revision >= revisionFrom)
            {
                var path = $"{dirpath}{Key}-{_Version}";
                File.Delete(path);
                File.Copy(record.ContentPath, path);
            }
        }

        internal void Export(string dirpath, AssetBuildPlatform platform, bool subdir)
        {
            var record = records[MultiPlatforms ? (int)platform : 0];

            var path = dirpath;
            
            if (subdir) path = System.IO.Path.Combine(path, Asset.GetBuildPlatformDirPath(platform, MultiPlatforms));

            path = System.IO.Path.Combine(path, _Path.Replace('/', System.IO.Path.DirectorySeparatorChar));

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

            File.Delete(path);
            File.Copy(record.ContentPath, path);
        }

        public void Export(string dirpath)
        {
            if (MultiPlatforms)
            {
                Export(dirpath, AssetBuildPlatform.Android, true);
                Export(dirpath, AssetBuildPlatform.iOS, true);
                Export(dirpath, AssetBuildPlatform.Windows, true);
            }
            else
            {
                Export(dirpath, AssetBuildPlatform.All, true);
            }
        }
    }
}
