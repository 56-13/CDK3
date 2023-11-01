using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;

using CDK.Assets.Configs;

namespace CDK.Assets.Containers
{
    public class ProjectAsset : Asset
    {
        internal AssetRetains Retains { private set; get; }
        internal AssetTimes Times { private set; get; }
        public string ExecutablePath { private set; get; }
        public string UploadPath { private set; get; }

        private Dictionary<string, IEnumerable<Constant>> _Constants = new Dictionary<string, IEnumerable<Constant>>();
        public IEnumerable<Constant> GetConstants(string group) => _Constants.TryGetValue(group, out var result) ? result : new Constant[0];
        public IEnumerable<Constant> GetAnimationKeyConstants() => GetConstants("AnimationKey");
        public IEnumerable<Constant> GetTerrainTileConstants() => GetConstants("TerrainTile");

        public SceneConfig SceneConfig { private set; get; }

        public ProjectAsset(string path)
        {
            _ContentPath = path.Substring(0, path.LastIndexOf("."));

            var doc = new XmlDocument();

            doc.Load(path);

            var node = doc.ChildNodes[1];

            if (node.LocalName != "projectAsset") throw new XmlException();

            var hierachyKeys = new HashSet<string>();

            AssetManager.Instance.HoldCommand();
            AssetManager.Instance.HoldRetrieving();

            try
            {

                LoadHierachy(doc.ChildNodes[1], hierachyKeys);

                Retains = new AssetRetains(this, hierachyKeys);
                Times = new AssetTimes(this, hierachyKeys);
            }
            finally
            {
                IsDirty = false;

                AssetManager.Instance.ReleaseCommand();
                AssetManager.Instance.ReleaseRetrieving();
            }

            LoadSettings();
        }

        public ProjectAsset(string name, string dirpath)
        {
            _ContentPath = Path.Combine(dirpath, name);

            Name = name;

            Retains = new AssetRetains(this, null);
            Times = new AssetTimes(this, null);

            LoadSettings();
        }

        private void LoadSettings()
        {
            var dirpath = Path.GetDirectoryName(ContentPath);

            var path = $"{ContentPath}.settings.xml";

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "settings") throw new XmlException();

                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.LocalName)
                    {
                        case "build":
                            ExecutablePath = Path.Combine(dirpath, subnode.ReadAttributeString("executablePath"));
                            UploadPath = Path.Combine(dirpath, subnode.ReadAttributeString("uploadPath"));
                            break;
                        case "constants":
                            {
                                var group = subnode.ReadAttributeString("group");
                                var constants = new Constant[subnode.ChildNodes.Count];
                                for (var i = 0; i < constants.Length; i++) constants[i] = new Constant(subnode.ChildNodes[i]);
                                _Constants.Add(group, constants);
                            }
                            break;
                    }
                }
            }
        }

        private void LoadSceneConfig()
        {
            var path = $"{ContentPath}.scene.xml";

            if (File.Exists(path))
            {
                var doc = new XmlDocument();

                doc.Load(path);

                var node = doc.ChildNodes[1];

                if (node.LocalName != "scene") throw new XmlException();

                SceneConfig = new SceneConfig(this, node);
            }
            else
            {
                SceneConfig = new SceneConfig(this);
            }
        }

        private void SaveSceneConfig()
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            var path = $"{ContentPath}.scene.xml";

            using (var writer = XmlWriter.Create(path, settings))
            {
                SceneConfig.Save(writer);
            }
        }

        private string _ContentPath;
        public override string ContentPath => _ContentPath;
        public override AssetType Type => AssetType.Project;

        internal override void Link()
        {
            base.Link();

            GitLock();

            if (SceneConfig == null) LoadSceneConfig();     //씬설정은 하위 애셋을 참조하므로 링크된 이후 나중에 로드
        }

        internal override void Unlink(bool dirty)
        {
            GitUnlock();

            base.Unlink(dirty);
        }

        protected override bool AddChildTypeEnabled(AssetType type)
        {
            return type != AssetType.Project && type != AssetType.SubImage;
        }

        internal override void AddRetains(ICollection<string> keys) => SceneConfig.AddRetains(keys);
        internal override bool IsRetaining(AssetElement element, out AssetElement from) => SceneConfig.IsRetaining(element, out from);

        protected override bool SaveContent()
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using (var writer = XmlWriter.Create(ContentXmlPath, settings))
            {
                SaveHierachy(writer);
            }

            Retains.Save();

            SaveSceneConfig();

            return true;
        }

        #region git

        private static void GitSubPaths(string dirpath, List<string> result)
        {
            if (dirpath.EndsWith(".git")) result.Add(dirpath);
            else if (Directory.Exists(dirpath))
            {
                foreach (var subdirpath in Directory.GetDirectories(dirpath, "*")) GitSubPaths(subdirpath, result);
            }
        }

        private static void GitParentPaths(string dirpath, List<string> result)
        {
            var index = dirpath.LastIndexOf(Path.DirectorySeparatorChar);

            if (index >= 0)
            {
                dirpath = dirpath.Substring(0, index);

                var path = $"{dirpath}{Path.DirectorySeparatorChar}.git";

                if (Directory.Exists(path)) result.Add(path);
                else GitParentPaths(dirpath, result);
            }
        }
        
        private static IEnumerable<string> GitPaths(string path)
        {
            var result = new List<string>();
            GitParentPaths(path, result);
            GitSubPaths(path, result);
            return result;
        }

        public static bool GitOccupy(string path)
        {
            if (Debugger.IsAttached) return true;

            path = path.Substring(0, path.LastIndexOf('.'));

            var dirpaths = GitPaths(path);

            var pname = Process.GetProcessesByName("git");

            if (pname.Length != 0)
            {
                foreach (var dirpath in dirpaths)
                {
                    path = $"{dirpath}{Path.DirectorySeparatorChar}index.lock";

                    if (File.Exists(path)) return false;
                }
            }
            else
            {
                foreach (var dirpath in dirpaths)
                {
                    path = $"{dirpath}{Path.DirectorySeparatorChar}index.lock";
                    try
                    {
                        File.Delete(path);

                        Console.WriteLine("git unlock : " + path);
                    }
                    catch
                    {
                        Console.WriteLine("unabled to git unlock : " + path);
                    }
                }
            }

            return true;
        }

        private void GitLock()
        {
            if (Debugger.IsAttached) return;

            foreach (var dirpath in GitPaths(_ContentPath))
            {
                var path = $"{dirpath}{Path.DirectorySeparatorChar}index.lock";
                try
                {
                    using (File.Create(path))
                    {
                    }
                    Console.WriteLine("git lock : " + path);
                }
                catch
                {
                    Console.WriteLine("unabled to git lock : " + path);
                }
            }
        }

        private void GitUnlock()
        {
            if (Debugger.IsAttached) return;

            foreach (var dirpath in GitPaths(_ContentPath))
            {
                var path = $"{dirpath}{Path.DirectorySeparatorChar}index.lock";
                try
                {
                    File.Delete(path);

                    Console.WriteLine("git unlock : " + path);
                }
                catch
                {
                    Console.WriteLine("unabled to git unlock : " + path);
                }
            }
        }

        #endregion
    }
}
