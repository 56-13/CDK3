using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.IO;

using CDK.Assets.Containers;
using CDK.Assets.Support;
using CDK.Assets.Scenes;

namespace CDK.Assets
{
    public enum AssetType
    {
        Project,
        Package,
        List,
        Block,
        BlockList,
        File,
        Folder,
        Binary,
        Media,
        String,
        Attribute,
        RootImage,
        SubImage,
        Material,
        Skybox,
        Mesh,
        Animation,
        Spawn,
        Scene,
        Terrain,
        Trigger,
        TriggerFormat,
        Reference,
        Independent,
        Version,
        Unused
    }

    public enum AssetLoadingState
    {
        NotLoaded,
        Loading,
        Complete
    }

    public enum AssetBuildPlatform
    {
        All,
        Android,
        iOS,
        Windows
    }

    public abstract class Asset : AssetElement
    {
        private Asset _Parent;
        public Asset Parent
        {
            private set
            {
                if (_Parent != value)
                {
                    var nameChanged = false;

                    if (value == null)
                    {
                        var project = Project;

                        if (project != null)
                        {
                            Unlink(true);

                            project.IsDirty = true;

                            OnPropertyChanged("Project");
                        }

                        _Parent = null;
                    }
                    else
                    {
                        var oldProject = Project;
                        var newProject = value.Project;

                        if (oldProject != null)
                        {
                            if (oldProject != newProject) oldProject.IsDirty = true;
                            if (newProject == null) Unlink(true);
                        }

                        var oldpath = ContentPath;

                        var originName = _Name;

                        if (value.IsListed) originName = $"{value.Name}[{value.Children.IndexOf(this):D3}]";

                        var name = originName;

                        var idx = 0;
                        bool loop;

                        do
                        {
                            loop = false;

                            foreach (var other in value.Children)
                            {
                                if (other != this && name == other.Name)
                                {
                                    name = originName + '_' + idx;
                                    idx++;
                                    loop = true;
                                    break;
                                }
                            }
                        } while (loop);

                        if (_Name != name)
                        {
                            nameChanged = true;

                            _Name = name;
                        }

                        _Parent = value;

                        if (oldpath != null)
                        {
                            var newpath = ContentPath;

                            if (newpath != null) MovePath(oldpath, newpath);
                        }
                        if (newProject != null)
                        {
                            newProject.IsDirty = true;

                            if (oldProject == null) Link();
                            else if (oldProject != newProject) MoveProject(oldProject, newProject);
                        }
                        if (oldProject != newProject) OnPropertyChanged("Project");
                    }
                    OnPropertyChanged("Parent");
                    OnLocationChanged();
                    OnTagsChanged();

                    if (nameChanged)
                    {
                        OnPropertyChanged("Name");
                        OnPropertyChanged("Title");
                        if (_Tag == null) OnPropertyChanged("TagName");
                        RenameChildren();
                    }
                }
            }
            get => _Parent;
        }

        public override AssetElement GetParent() => _Parent;

        public AssetElementList<Asset> Children { private set; get; }

        private void AddAllChildren(List<Asset> allChildren)
        {
            allChildren.AddRange(Children);
            foreach (var child in Children) child.AddAllChildren(allChildren);
        }

        public IEnumerable<Asset> AllChildren
        {
            get
            {
                var allChildren = new List<Asset>();
                AddAllChildren(allChildren);
                return allChildren;
            }
        }

        public int AllChildrenCount
        {
            get
            {
                var count = Children.Count;
                foreach (var child in Children)
                {
                    count += child.AllChildrenCount;
                }
                return count;
            }
        }

        public override IEnumerable<AssetElement> GetChildren() => Children.Select(a => (AssetElement)a);
        private bool GetSiblings(Asset origin, Asset prev, int depth, List<AssetElement> siblings, Stack<int> indices)
        {
            if (IsIndependent) return false;

            var collision = false;

            if (depth == 0)
            {
                if (Type == origin.Type) siblings.Add(this);
                else collision = true;
            }
            else if (depth > 0)
            {
                var idx = indices.Pop();
                if (IsListed)
                {
                    foreach (var child in Children)
                    {
                        if (child != prev && child.GetSiblings(origin, this, depth - 1, siblings, indices)) collision = true;
                    }
                }
                else if (idx >= Children.Count) collision = true;
                else if (Children[idx] != prev && Children[idx].GetSiblings(origin, this, depth - 1, siblings, indices)) collision = true;

                indices.Push(idx);

                if (_Parent != null && _Parent != prev)
                {
                    indices.Push(Index);
                    if (_Parent.GetSiblings(origin, this, depth + 1, siblings, indices)) collision = true;
                }
            }
            return collision;
        }

        public override IEnumerable<AssetElement> GetSiblings()
        {
            var siblings = new List<AssetElement>();

            if (_Parent != null)
            {
                var indices = new Stack<int>();
                indices.Push(Index);
                if (_Parent.GetSiblings(this, this, 1, siblings, indices))
                {
                    AssetManager.Instance.Message("리스트에 포함된 에셋의 형태가 다릅니다.");
                }
            }
            return siblings;
        }

        private void MoveProject(ProjectAsset oldProject, ProjectAsset newProject)
        {
            if (Linked)
            {
                var targets = oldProject.Retains.GetTargets(Key);
                newProject.Retains.Update(Key, targets);
                oldProject.Retains.Update(Key, null);

                var time = oldProject.Times.GetTime(Key);
                oldProject.Times.Update(Key, DateTime.MinValue);
                newProject.Times.Update(Key, time);
            }

            foreach (Asset child in Children) child.MoveProject(oldProject, newProject);
        }
        
        private static readonly Regex FileNameRegEx = new Regex("^[A-Za-z0-9_\\-\\[\\]\\(\\)]*$");

        private string _Name;
        public string Name
        {
            set
            {
                if (value == null || FileNameRegEx.IsMatch(value))
                {
                    var originName = value;

                    if (string.IsNullOrEmpty(value))
                    {
                        originName = _Parent != null && _Parent.IsListed ? $"{_Parent.Name}[{Index:D3}]" : Type.ToString();
                    }

                    var name = originName;

                    if (_Parent != null)
                    {
                        var idx = 0;

                        bool flag;

                        do
                        {
                            flag = false;

                            foreach (var child in _Parent.Children)
                            {
                                if (child != this && name == child.Name)
                                {
                                    name = $"{originName}_{idx}";
                                    idx++;
                                    flag = true;
                                    break;
                                }
                            }
                        } while (flag);
                    }

                    if (_Name != name)
                    {
                        var oldpath = ContentPath;

                        _Name = name;

                        if (oldpath != null)
                        {
                            var newpath = ContentPath;

                            MovePath(oldpath, newpath);
                        }

                        OnLocationChanged();
                        OnPropertyChanged("Name");
                        OnPropertyChanged("Title");
                        if (_Tag == null) OnPropertyChanged("TagName");

                        var project = Project;
                        if (project != null) project.IsDirty = true;

                        RenameChildren();

                        if (_Parent != null && !_Parent.IsListed && AssetManager.Instance.RetrieveEnabled)
                        {
                            using (new AssetRetrieveHolder())
                            {
                                foreach (Asset a in GetSiblings()) a.Name = value;
                            }
                        }
                    }
                }
            }
            get => _Name;
        }

        private string _Tag;
        public string Tag
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;

                if (_Tag != value)
                {
                    _Tag = value;

                    var project = Project;
                    if (project != null) project.IsDirty = true;

                    OnPropertyChanged("Tag");
                    OnPropertyChanged("TagName");
                    OnPropertyChanged("Title");
                    OnLocationChanged();
                    OnTagsChanged();

                    if (_Parent != null && !_Parent.IsListed && AssetManager.Instance.RetrieveEnabled)
                    {
                        using (new AssetRetrieveHolder())
                        {
                            foreach (Asset a in GetSiblings()) a.Tag = value;
                        }
                    }
                }
            }
            get => _Tag;
        }

        public string TagName => _Tag != null ? $"{_Name} - {_Tag}" : _Name;
        public string Title => IsDirty ? $"{TagName} *" : TagName;

        public int Index => _Parent != null ? _Parent.Children.IndexOf(this) : -1;

        public IEnumerable<int> Indices
        {
            get
            {
                var current = this;
                var indices = new List<int>();
                while (current.Parent != null)
                {
                    if (current.Parent.IsListed) indices.Insert(0, current.Index);
                    current = current.Parent;
                }
                return indices;
            }
        }

        private bool _IsDirty;
        public override bool IsDirty
        {
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;

                    if (Linked)
                    {
                        if (_IsDirty) AssetManager.Instance.Dirties.Add(this);
                        else AssetManager.Instance.Dirties.Remove(this);
                    }

                    OnPropertyChanged("IsDirty");
                    OnPropertyChanged("Title");
                }
            }
            get => _IsDirty;
        }

        public string Location
        {
            get
            {
                var strbuf = new StringBuilder();

                var current = this;

                for (; ; )
                {
                    strbuf.Insert(0, current.TagName);
                    current = current._Parent;
                    if (current == null) break;
                    strbuf.Insert(0, '.');
                }
                return strbuf.ToString();
            }
        }

        private void OnLocationChanged()
        {
            OnPropertyChanged("Location");
            foreach (var child in Children) child.OnLocationChanged();
        }

        public override string GetLocation() => Location;

        public string Tags
        {
            get
            {
                var strbuf = new StringBuilder();

                var current = this;

                for (; ; )
                {
                    if (current.Tag != null)
                    {
                        if (strbuf.Length != 0) strbuf.Insert(0, '.');
                        strbuf.Insert(0, current.Tag);
                    }
                    current = current._Parent;
                    if (current == null) break;
                }
                return strbuf.ToString();
            }
        }

        private void OnTagsChanged()
        {
            OnPropertyChanged("Tags");
            foreach (var child in Children) child.OnTagsChanged();
        }

        public virtual string ContentPath
        {
            get
            {
                var path = _Parent?.ContentPath;
                if (path == null) return null;
                return Path.Combine(path, _Name);
            }
        }
            
        internal virtual string ContentXmlPath => $"{ContentPath}.xml";
        internal virtual string BuildDirPath => _Parent?.BuildDirPath;
        internal virtual string BuildPath => Path.Combine(BuildDirPath, $"{_Name}.xmf");

        internal static string GetBuildPlatformDirPath(AssetBuildPlatform platform, bool multiPlatforms)
        {
            if (multiPlatforms)
            {
                switch (platform)
                {
                    case AssetBuildPlatform.Android:
                        return "android";
                    case AssetBuildPlatform.iOS:
                        return "iOS";
                    case AssetBuildPlatform.Windows:
                        return "windows";
                }
            }
            return "common";
        }

        internal string GetBuildPlatformDirPath(AssetBuildPlatform platform)
        {
            return GetBuildPlatformDirPath(platform, MultiPlatforms);
        }

        public virtual bool MultiPlatforms { 
            set => throw new InvalidOperationException(); 
            get => false; 
        }
        
        public string Key { private set; get; }

        public virtual bool IsUnused => _Parent != null && _Parent.IsUnused;
        public virtual bool IsIndependent => false;
        public AssetLoadingState LoadingState { private set; get; }

        protected Asset()
        {
            Key = AssetManager.NewKey();

            _Name = Type.ToString();

            IsDirty = true;

            Children = new AssetElementList<Asset>(this);
            Children.BeforeListChanged += Children_BeforeListChanged;
            Children.ListChanged += Children_ListChanged;

            LoadingState = AssetLoadingState.Complete;
        }

        protected Asset(Asset other, bool content)
        {
            AssetManager.Instance.AddRedirection(other, this);

            Key = AssetManager.NewKey();

            _Name = other._Name;

            if (content) _Tag = other._Tag;

            IsDirty = true;

            Children = new AssetElementList<Asset>(this);
            Children.BeforeListChanged += Children_BeforeListChanged;
            Children.ListChanged += Children_ListChanged;

            if (!IsListed || content)
            {
                using (new AssetCommandHolder())
                using (new AssetRetrieveHolder())
                {
                    for (int i = 0; i < other.Children.Count; i++)
                    {
                        Children.Add(other.Children[i].Clone(content));
                    }
                }
            }

            LoadingState = AssetLoadingState.Complete;
        }

        private void Children_BeforeListChanged(object sender, BeforeListChangedEventArgs<Asset> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (!BeforeChildrenAdded(e.Object)) e.Cancel = true;
                    break;
                case ListChangedType.ItemChanged:
                    if (!BeforeChildrenAdded(e.Object)) e.Cancel = true;
                    if (!BeforeChildrenRemoved(e.NewIndex)) e.Cancel = true;
                    break;
                case ListChangedType.ItemDeleted:
                    if (!BeforeChildrenRemoved(e.NewIndex)) e.Cancel = true;
                    break;
                case ListChangedType.Reset:
                    if (!BeforeChildrenReset()) e.Cancel = true;
                    break;
                
            }
        }

        private bool BeforeChildrenAdded(Asset child)
        {
            if (child._Parent != null)
            {
                if (child._Parent == this) return false;

                if (!child._Parent.IsListed && AssetManager.Instance.RetrieveEnabled)
                {
                    foreach (var a in child.GetSiblings())
                    {
                        if (a.IsRetained(false, true)) return false;
                    }
                }
            }
            var current = this;
            do
            {
                if (current == child) return false;
                current = current.Parent;
            }
            while (current != null);

            if (!AddChildEnabled(child, false)) return false;

            if (!IsListed && AssetManager.Instance.RetrieveEnabled)
            {
                foreach (Asset a in GetSiblings())
                {
                    if (!a.AddChildEnabled(child, false)) return false;
                }
            }
            return true;
        }

        private bool BeforeChildrenReset()
        {
            if (AssetManager.Instance.RetrieveEnabled)
            {
                foreach (var child in Children)
                {
                    if (child.IsRetained(false, true)) return false;
                }
                if (!IsListed)
                {
                    var siblings = GetSiblings();
                    foreach (Asset a in siblings)
                    {
                        foreach (var ac in a.Children)
                        {
                            if (ac.IsRetained(false, true)) return false;
                        }
                    }
                    using (new AssetRetrieveHolder())
                    {
                        foreach (Asset a in siblings) a.Children.Clear();
                    }
                }
            }
            foreach (var child in Children)
            {
                child.LoadAll();
                child.Parent = null;
            }
            return true;
        }

        private bool BeforeChildrenRemoved(int index)
        {
            if (AssetManager.Instance.RetrieveEnabled)
            {
                if (Children[index].IsRetained(false, true)) return false;

                if (!IsListed)
                {
                    var siblings = GetSiblings();
                    foreach (Asset sibling in siblings)
                    {
                        if (index < sibling.Children.Count && sibling.Children[index].IsRetained(false, true)) return false;
                    }
                    foreach (Asset sibling in siblings)
                    {
                        if (index < sibling.Children.Count) sibling.Children.RemoveAt(index);
                    }
                }
            }
            Children[index].LoadAll();
            Children[index].Parent = null;

            return true;
        }

        private void ChildrenAdded(int index)
        {
            var child = Children[index];

            var oldParent = child._Parent;

            if (oldParent != null && !oldParent.IsListed && AssetManager.Instance.RetrieveEnabled)
            {
                using (new AssetRetrieveHolder())
                {
                    foreach (Asset sibling in child.GetSiblings()) sibling.Parent.Children.Remove(sibling);
                }
            }
            child.Parent = this;

            if (oldParent != null)
            {
                oldParent.Children.BeforeListChanged -= oldParent.Children_BeforeListChanged;
                oldParent.Children.Remove(child);
                oldParent.Children.BeforeListChanged += oldParent.Children_BeforeListChanged;
            }

            if (!IsListed && AssetManager.Instance.RetrieveEnabled)
            {
                using (new AssetRetrieveHolder())
                {
                    foreach (Asset a in GetSiblings())
                    {
                        if (index <= a.Children.Count) a.Children.Insert(index, child.Clone(false));
                    }
                }
            }
        }

        private void ChildrenMoved(int oldIndex, int newIndex)
        {
            var project = Project;

            if (project != null) project.IsDirty = true;

            if (!IsListed && AssetManager.Instance.RetrieveEnabled)
            {
                using (new AssetRetrieveHolder())
                {
                    foreach (Asset a in GetSiblings())
                    {
                        if (oldIndex < a.Children.Count && newIndex < a.Children.Count)
                        {
                            a.Children.Move(oldIndex, newIndex);
                        }
                    }
                }
            }

            RenameChildren(Math.Min(oldIndex, newIndex), Math.Max(oldIndex, newIndex));
        }

        private void ChildrenReset()
        {
            for (var i = 0; i < Children.Count; i++) ChildrenAdded(i);
        }

        private bool renamingChildrenReserved;
        private HashSet<Asset> renamingChildren = new HashSet<Asset>();

        private void RenameChildren(int min, int max)
        {
            if (IsListed)
            {
                for (var i = min; i <= max; i++) renamingChildren.Add(Children[i]);

                if (!renamingChildrenReserved && renamingChildren.Count != 0)
                {
                    renamingChildrenReserved = true;
                    AssetManager.Instance.Invoke(() =>
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            foreach (Asset child in renamingChildren)
                            {
                                if (child.Parent == this) child.Name = null;
                            }
                        }
                        renamingChildren.Clear();
                        renamingChildrenReserved = false;
                    });
                }
            }
        }

        private void RenameChildren() => RenameChildren(0, Children.Count - 1);

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ChildrenAdded(e.NewIndex);

                    RenameChildren(e.NewIndex, Children.Count - 1);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null) ChildrenAdded(e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    RenameChildren(e.NewIndex, Children.Count - 1);
                    break;
                case ListChangedType.ItemMoved:
                    ChildrenMoved(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    ChildrenReset();

                    RenameChildren();
                    break;
            }
        }

        private static void FileFunc(Action action)
        {
            var retry = 0;
            for (;;)
            {
                try
                {
                    action();
                    break;
                }
                catch(IOException e)
                {
                    if (++retry >= 50) throw e;
                }
                Thread.Sleep(100);
            }
        }
        
        private void TouchDirectory(string path)
        {
            try
            {
                Directory.SetLastWriteTime(path, DateTime.Now);
            }
            catch { }
            foreach (var filepath in Directory.GetFiles(path))
            {
                try
                {
                    File.SetLastWriteTime(filepath, DateTime.Now);
                }
                catch { }
            }
            foreach (var dirpath in Directory.GetDirectories(path))
            {
                TouchDirectory(dirpath);
            }
        }

        private void CopyDirectory(DirectoryInfo olddir, DirectoryInfo newdir)
        {
            if (!Directory.Exists(newdir.FullName))
            {
                FileFunc(() => Directory.CreateDirectory(newdir.FullName));
            }
            foreach (var fi in olddir.GetFiles())
            {
                FileFunc(() => fi.CopyTo(Path.Combine(newdir.ToString(), fi.Name), true));
            }
            foreach (DirectoryInfo oldsubdir in olddir.GetDirectories())
            {
                DirectoryInfo newsubdir = null;

                FileFunc(() => newsubdir = newdir.CreateSubdirectory(oldsubdir.Name));
                    
                CopyDirectory(oldsubdir, newsubdir);
            }
        }

        private void MoveDirectory(string oldpath, string newpath)
        {
            if (Directory.GetDirectoryRoot(oldpath).Equals(Directory.GetDirectoryRoot(newpath)))
            {
                FileFunc(() => Directory.Move(oldpath, newpath));
            }
            else
            {
                CopyDirectory(new DirectoryInfo(oldpath), new DirectoryInfo(newpath));

                FileFunc(() => Directory.Delete(oldpath, true));
            }
        }

        private void MoveFile(string oldpath, string newpath)
        {
            if (Directory.GetDirectoryRoot(oldpath).Equals(Directory.GetDirectoryRoot(newpath)))
            {
                FileFunc(() => File.Move(oldpath, newpath));
            }
            else
            {
                FileFunc(() => File.Copy(oldpath, newpath));
                FileFunc(() => File.Delete(oldpath));
            }
        }

        private void MovePath(string oldpath, string newpath)
        {
            if (!oldpath.Equals(newpath))
            {
                try
                {
                    if (Directory.Exists(oldpath))
                    {
                        if (!oldpath.ToLower().Equals(newpath.ToLower()))
                        {
                            if (Directory.Exists(newpath))
                            {
                                FileFunc(() => Directory.Delete(newpath, true));
                            }
                            FileFunc(() => Directory.CreateDirectory(Path.GetDirectoryName(newpath)));
                        }
                        TouchDirectory(oldpath);

                        MoveDirectory(oldpath, newpath);
                    }

                    var olddirpath = Path.GetDirectoryName(oldpath);

                    if (Directory.Exists(olddirpath))
                    {
                        var oldfilepaths = Directory.GetFiles(olddirpath, $"{Path.GetFileName(oldpath)}.*");

                        if (oldfilepaths.Length != 0)
                        {
                            var newdirpath = Path.GetDirectoryName(newpath);

                            if (!olddirpath.ToLower().Equals(newdirpath.ToLower()))
                            {
                                FileFunc(() => Directory.CreateDirectory(newdirpath));
                            }

                            foreach (string oldfilepath in oldfilepaths)
                            {
                                var oldfilename = Path.GetFileName(oldfilepath);
                                var oldfilesuffix = oldfilename.Substring(oldfilename.IndexOf('.') + 1);
                                var newfilepath = $"{newpath}.{oldfilesuffix}";

                                if (!oldfilepath.ToLower().Equals(newfilepath.ToLower()))
                                {
                                    FileFunc(() => File.Delete(newfilepath));
                                }
                                try
                                {
                                    File.SetLastWriteTime(oldfilepath, DateTime.Now);
                                }
                                catch { }

                                MoveFile(oldfilepath, newfilepath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.Record(e);

                    AssetManager.Instance.Message($"파일을 이동할 수 없습니다. \n{oldpath} => {newpath}");
                }
            }
        }

        private Asset GetFirstChild()
        {
            if (Children.Count > 0) return Children[0];

            foreach (Asset a in GetSiblings())
            {
                if (a.Children.Count > 0) return a.Children[0];
            }
            return null;
        }

        protected virtual bool AddChildTypeEnabled(AssetType type) => false;

        public bool AddChildEnabled(AssetType type)
        {
            if (IsListed)
            {
                var firstChild = GetFirstChild();

                if (firstChild != null)
                {
                    return type == firstChild.Type;
                }
            }
            return AddChildTypeEnabled(type);
        }

        public bool AddChildEnabled(Asset asset, bool ui)
        {
            if (IsListed)
            {
                var firstChild = GetFirstChild();

                if (firstChild != null)
                {
                    return firstChild.Compare(asset, true);
                }
            }
            if (ui && !AssetManager.Instance.IsDeveloper)
            {
                return false;
            }
            return AddChildTypeEnabled(asset.Type);
        }

        public Asset NewChild(AssetType type)
        {
            if (IsListed)
            {
                var firstChild = GetFirstChild();

                if (firstChild != null)
                {
                    AssetManager.Instance.BeginRedirection();
                    var child = firstChild.Clone(false);
                    AssetManager.Instance.EndRedirection();
                    return child;
                }
            }

            return Create(type);
        }

        protected void SaveRetains()
        {
            var targets = new SortedSet<string>();
            AddRetains(targets);
            targets.Remove(Key);
            Project.Retains.Update(Key, targets);
        }

        public IEnumerable<Asset> GetRetainOrigins()
        {
            var originAssets = new List<Asset>();

            foreach (var sourceAsset in AssetManager.Instance.Dirties)
            {
                if (sourceAsset != this && sourceAsset.IsRetaining(this, out _))
                {
                    originAssets.Add(sourceAsset);
                }
            }
            if (Linked)
            {
                foreach (var projectAsset in AssetManager.Instance.Projects)
                {
                    var sources = projectAsset.Retains.GetOrigins(Key);

                    if (sources != null)
                    {
                        foreach (var source in sources)
                        {
                            var sourceAsset = AssetManager.Instance.GetAsset(source);

                            if (sourceAsset != null && !sourceAsset.IsDirty && !originAssets.Contains(sourceAsset))
                            {
                                originAssets.Add(sourceAsset);
                            }
                        }
                    }
                }
            }
            return originAssets;
        }

        public IEnumerable<Asset> GetRetainTargets()
        {
            var targetAssets = new List<Asset>();

            SortedSet<string> targets = null;

            if (IsDirty)
            {
                targets = new SortedSet<string>();
                AddRetains(targets);
            }
            else if (Linked)
            {
                targets = Project.Retains.GetTargets(Key);
            }

            if (targets != null)
            {
                foreach (var target in targets)
                {
                    var targetAsset = AssetManager.Instance.GetAsset(target);

                    if (targetAsset != null && !targetAssets.Contains(targetAsset))
                    {
                        targetAssets.Add(targetAsset);
                    }
                }
            }
            return targetAssets;
        }

        public void ClipChildren(int[] indices, bool cut)
        {
            var children = new List<Asset>();
            foreach (int idx in indices) children.Add(Children[idx]);
            AssetManager.Instance.Clip(children.ToArray(), cut);
        }

        public void PasteChildren()
        {
            if (AssetManager.Instance.ClipObject is Asset[] origins)
            {
                var cut = AssetManager.Instance.ClipCut;
                foreach (var child in origins)
                {
                    if (!AddChildEnabled(child, true)) return;
                }

                if (cut)
                {
                    foreach (var asset in origins) Children.Add(asset);
                    AssetManager.Instance.ClearClip();
                }
                else
                {
                    var clones = new List<Asset>();
                    AssetManager.Instance.BeginRedirection();
                    foreach (var asset in origins) clones.Add(asset.Clone(true));
                    AssetManager.Instance.EndRedirection();
                    foreach (var asset in clones) Children.Add(asset);
                }
            }
        }
        
        public void LevelUp()
        {
            if (_Parent != null && _Parent.Parent != null && !_Parent.IsListed)
            {
                var assets = GetSiblings().Select(a => (Asset)a).ToList();
                var parents = new Asset[assets.Count + 1];

                var flag = false;
                for (int i = 0; i < assets.Count; i++)
                {
                    var a = this;
                    var b = assets[i];
                    for (;;)
                    {
                        if (a.Index < b.Index)
                        {
                            flag = true;
                            break;
                        }
                        a = a.Parent;
                        b = b.Parent;
                    }
                    if (flag)
                    {
                        assets.Insert(i, this);
                        break;
                    }
                }
                if (!flag) assets.Add(this);

                using (new AssetRetrieveHolder())
                {
                    for (var i = 0; i < assets.Count; i++)
                    {
                        var asset = assets[i];
                        parents[i] = asset.Parent.Parent;
                        parents[i].Children.Remove(asset.Parent);
                    }
                    for (var i = 0; i < assets.Count; i++)
                    {
                        parents[i].Children.Add(assets[i]);
                    }
                }
            }
        }

        public static void LevelDown(AssetType type, Asset[] assets)
        {
            var parent = assets[0].Parent;
            
            if (parent == null) return;

            switch (type)
            {
                case AssetType.List:
                case AssetType.BlockList:
                    if (assets.Length > 1) return;
                    break;
            }
            for (var i = 1; i < assets.Length; i++)
            {
                if (assets[i].Parent != parent) return;
            }
            var parents = parent.GetSiblings().Select(a => (Asset)a).ToList();
            parents.Add(parent);

            var newParents = new Asset[parents.Count];
            for (var i = 0; i < newParents.Length; i++) newParents[i] = Create(type);

            var allAssets = new List<Asset>();
            foreach (var asset in assets)
            {
                var localAssets = asset.GetSiblings().Select(a => (Asset)a).ToList();
                localAssets.Add(asset);
                foreach (var localAsset in localAssets)
                {
                    if (!allAssets.Contains(localAsset))
                    {
                        var current = localAsset.Index;

                        int i;
                        for (i = 0; i < allAssets.Count; i++)
                        {
                            if (current >= allAssets[i].Index) break;
                        }
                        allAssets.Insert(i, localAsset);
                    }
                }
            }

            using (new AssetRetrieveHolder())
            {
                foreach (var asset in allAssets)
                {
                    var index = parents.IndexOf(asset.Parent);
                    parents[index].Children.Remove(asset);
                    newParents[index].Children.Add(asset);
                }
                for (var i = 0; i < parents.Count; i++)
                {
                    parents[i].Children.Add(newParents[i]);
                }
            }
        }
        
        public virtual Asset Clone(bool content)
        {
            var asset = Create(Type);

            AssetManager.Instance.AddRedirection(this, asset);

            asset._Name = _Name;
            if (content) asset._Tag = _Tag;

            if (!IsListed || content)
            {
                using (new AssetCommandHolder())
                using (new AssetRetrieveHolder())
                {
                    foreach (var child in Children)
                    {
                        asset.Children.Add(child.Clone(content));
                    }
                }
            }
            return asset;
        }

        protected virtual bool CompareContent(Asset asset) => true;

        public bool Compare(Asset asset, bool all, bool content = true)
        {
            if (Type == asset.Type && (!content || CompareContent(asset)))
            {
                if (IsListed)
                {
                    return asset.IsListed && (asset.Children.Count == 0 || Children.Count == 0 || asset.Children[0].Compare(Children[0], all, content));
                }
                else if (Children.Count == asset.Children.Count)
                {
                    for (var i = 0; i < Children.Count; i++)
                    {
                        if (!Children[i].Compare(asset.Children[i], all, content)) return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool Spawnable => false;
        public virtual SceneComponent NewSceneComponent() => null;
        public virtual Scene NewScene() => null;

        public Scene NewDefaultScene(SceneComponent obj)
        {
            var scene = new Scene(this)
            {
                Seperated = true
            };
            scene.Children.Add(new Scenes.Environment(scene.Config.Preferences[0], scene.Config.Environments[0]));
            if (obj is World)
            {
                obj.Fixed = true;
                scene.Children.Add(obj);
            }
            else
            {
                scene.Children.Add(new Ground(scene.Config.Grounds[0]));
                if (obj != null)
                {
                    obj.Fixed = true;
                    scene.Children.Add(obj);
                }
            }
            scene.SelectedComponent = obj;
            return scene;
        }

        public abstract AssetType Type { get; }
        public virtual string Description => string.Empty;

        public virtual void Import(string path)
        {
            
        }

        public virtual void Export(string dirpath)
        {
            dirpath = Path.Combine(dirpath, Name);

            Directory.CreateDirectory(dirpath);

            foreach (var child in Children) child.Export(dirpath);
        }

        internal virtual int BuildProgress => 1;

        public int GetBuildProgress(bool multiPlatforms = true)
        {
            var extendPlatforms = multiPlatforms && MultiPlatforms;
            if (extendPlatforms) multiPlatforms = false;
            var progress = BuildProgress;
            foreach (var child in Children)
            {
                progress += child.GetBuildProgress(multiPlatforms);
            }
            if (extendPlatforms) progress *= 3;       //iOS, Android, Windows
            return progress;
        }

        public void Build(string path, AssetBuildPlatform platform = AssetBuildPlatform.All)
        {
            Build(new Asset[] { this }, path, platform);
        }

        public static void Build(Asset[] assets, string path, AssetBuildPlatform platform = AssetBuildPlatform.All)
        {
            var progress = 0;
            foreach (var asset in assets)
            {
                progress += asset.GetBuildProgress(platform == AssetBuildPlatform.All);
            }
            AssetManager.Instance.BeginProgress(progress);
            try
            {
                foreach (var asset in assets) asset.BuildContent(null, path, platform);
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }

        public void Upload() => Upload(new Asset[] { this });

        public static void Upload(Asset[] assets)
        {
            var progress = 0;
            foreach (var asset in assets)
            {
                var parent = asset.Parent;
                while (parent != null)
                {
                    if (parent.Type == AssetType.File) throw new AssetException(asset, "파일에 속해 있는 애셋을 업로드할 수 없습니다.");

                    parent = parent.Parent;
                }

                progress += asset.GetBuildProgress(true);
            }
            AssetManager.Instance.BeginProgress(progress);
            try
            {
                foreach (var asset in assets)
                {
                    var path = asset.Project.UploadPath;

                    if (path == null) throw new AssetException(asset, "프로젝트 세팅에 업로드 경로가 설정되지 않았습니다.");

                    asset.BuildContent(null, path, AssetBuildPlatform.All);
                }
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }

        internal virtual void BuildContent(BinaryWriter writer, string path, AssetBuildPlatform platform)
        {
            if (IsListed)
            {
                for (var i = 1; i < Children.Count; i++)
                {
                    if (!Children[i].IsIndependent && !Children[0].Compare(Children[i], true))
                    {
                        throw new AssetException(Children[i], "리스트에 포함된 하위 에셋의 포맷이 다릅니다.");
                    }
                }
            }
            foreach (var child in Children)
            {
                if (!AssetManager.Instance.Progress(child.Location, 1))
                {
                    throw new AssetException(null, "작업이 중단되었습니다.");
                }

                child.BuildContent(writer, path, platform);
            }
        }

        internal void SaveHierachy(XmlWriter writer)
        {
            var nodename = Type.ToString();

            nodename = $"{nodename.Substring(0, 1).ToLower()}{nodename.Substring(1)}Asset";

            writer.WriteStartElement(nodename);

            writer.WriteAttribute("key", Key);
            writer.WriteAttribute("name", _Name);
            writer.WriteAttribute("tag", _Tag);

            foreach (Asset child in Children) child.SaveHierachy(writer);

            writer.WriteEndElement();
        }

        internal void LoadHierachy(XmlNode node, ICollection<string> keys)
        {
            Key = node.ReadAttributeString("key");
            _Name = node.ReadAttributeString("name");
            _Tag = node.ReadAttributeString("tag");

            OnPropertyChanged("Name");
            OnPropertyChanged("Tag");
            OnPropertyChanged("TagName");
            OnPropertyChanged("Title");

            keys.Add(Key);

            Children.BeforeListChanged -= Children_BeforeListChanged;
            Children.ListChanged -= Children_ListChanged;

            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var subnode = node.ChildNodes[i];

                var typename = subnode.LocalName;

                typename = typename.Substring(0, 1).ToUpper() + typename.Substring(1, typename.Length - 5 - 1);

                var type = (AssetType)Enum.Parse(typeof(AssetType), typename);

                var subasset = Create(type);

                Children.Add(subasset);
                subasset._Parent = this;
                subasset.OnPropertyChanged("Parent");
                subasset.LoadHierachy(subnode, keys);
            }

            Children.BeforeListChanged += Children_BeforeListChanged;
            Children.ListChanged += Children_ListChanged;

            LoadingState = AssetLoadingState.NotLoaded;
            IsDirty = false;
        }

        protected virtual bool SaveContent() => false;
        protected virtual void LoadContent() { }

        public void Save(bool force)          
        {
            if (Linked && (IsDirty || force))
            {
                SaveRetains();
                
                if (SaveContent())
                {
                    Project.Times.Update(Key, DateTime.Now);
                }
                IsDirty = false;
            }
        }

        public void SaveAll(bool force) => SaveAll(new Asset[] { this }, force);

        public static void SaveAll(Asset[] assets, bool force)
        {
            if (force)
            {
                var progress = 0;
                foreach (var asset in assets) progress += asset.AllChildrenCount + 1;
                AssetManager.Instance.BeginProgress(progress);
            }
            try
            {
                foreach (var asset in assets) asset.SaveAllImpl(force);
            }
            finally
            {
                if (force) AssetManager.Instance.EndProgress();
            }
        }

        private void SaveAllImpl(bool force)
        {
            if (force)
            {
                if (!AssetManager.Instance.Progress(Location, 1))
                {
                    throw new AssetException(null, "작업이 중단되었습니다.");
                }
            }
            Save(force);

            foreach (var child in Children) child.SaveAllImpl(force);
        }

        private void LoadImpl()
        {
            AssetManager.Instance.HoldCommand();
            AssetManager.Instance.HoldRetrieving();

            try
            {
                AssetManager.Instance.BeginLoad(this);

                LoadContent();
            }
            catch (Exception e)
            {
                ErrorHandler.Record(e);

                AssetManager.Instance.Message(ToString() + " 파일을 열 수 없습니다.");
            }
            finally
            {
                AssetManager.Instance.EndLoad();

                AssetManager.Instance.ReleaseCommand();
                AssetManager.Instance.ReleaseRetrieving();

                AssetManager.Instance.ClearCommands(this);

                LoadingState = AssetLoadingState.Complete;

                IsDirty = false;
            }
        }
        
        public void Load(bool force = false)
        {
            if (LoadingState == AssetLoadingState.NotLoaded || force)
            {
                LoadingState = AssetLoadingState.Loading;

                if (AssetManager.IsMainThread) LoadImpl();
                else
                {
                    lock (_reservedLoadingAssets)
                    {
                        if (!_reservedLoadingAssets.Contains(this)) _reservedLoadingAssets.Add(this);

                        lock (this)
                        {
                            Monitor.Wait(this);
                        }
                    }
                }
            }
            else if (LoadingState == AssetLoadingState.Loading && !AssetManager.IsMainThread)
            {
                lock (this)
                {
                    if (LoadingState == AssetLoadingState.Loading) Monitor.Wait(this);
                }
            }
        }

        private static List<Asset> _reservedLoadingAssets = new List<Asset>();

        public static void LoadReserved()
        {
            Debug.Assert(AssetManager.IsMainThread);

            lock (_reservedLoadingAssets)
            {
                foreach(var asset in _reservedLoadingAssets)
                {
                    asset.LoadImpl();

                    lock (asset)
                    {
                        Monitor.PulseAll(asset);
                    }
                }
                _reservedLoadingAssets.Clear();
            }
        }

        public void LoadAll(bool force = false)
        {
            Load(force);

            foreach (var child in Children) child.LoadAll(force);
        }

        internal virtual void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var child in Children)
            {
                if (!AssetManager.Instance.Progress(child.Location, 1))
                {
                    throw new AssetException(null, "작업이 중단되었습니다.");
                }
                child.GetLocaleStrings(strings);
            }
        }

        public void LocaleClean() => LocaleClean(new Asset[] { this });

        public static void LocaleClean(Asset[] assets)
        {
            var strings = new List<LocaleString>();

            var allChildrenCount = 0;
            foreach (var asset in assets) allChildrenCount += asset.AllChildrenCount;
            AssetManager.Instance.BeginProgress(allChildrenCount);

            try
            {
                foreach (var asset in assets) asset.GetLocaleStrings(strings);

                foreach (var str in strings) str.Clean();
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }

        public string[][] LocaleExport(string keyLocale, string[] targetLocales, DateTime? timestamp)
        {
            return LocaleExport(new Asset[] { this }, keyLocale, targetLocales, timestamp);
        }

        public static string[][] LocaleExport(Asset[] assets, string keyLocale, string[] targetLocales, DateTime? timestamp)
        {
            var strings = new List<LocaleString>();

            var progress = 0;
            foreach (var asset in assets) progress += asset.AllChildrenCount;
            AssetManager.Instance.BeginProgress(progress);

            try {
                foreach (var asset in assets) asset.GetLocaleStrings(strings);

                var i = 0;
                while (i < strings.Count)
                {
                    var str = strings[i];

                    if (!str.Locale || (timestamp != null && str.Timestamp.CompareTo(timestamp.Value) < 0)) strings.RemoveAt(i);
                    else i++;
                }
                if (keyLocale != null)
                {
                    var dic = new Dictionary<string, LocaleString>();

                    using (new AssetCommandHolder())
                    {
                        foreach (var str in strings)
                        {
                            var key = str.GetLocaleValue(keyLocale);

                            if (!string.IsNullOrWhiteSpace(key))
                            {
                                if (dic.TryGetValue(key, out var value))
                                {
                                    var isNewer = str.Timestamp.CompareTo(value.Timestamp) > 0;
                                    foreach (var l in AssetManager.Instance.Config.Locales)
                                    {
                                        var v = str.GetLocaleValue(l);

                                        if (!string.IsNullOrWhiteSpace(v) && (isNewer || string.IsNullOrWhiteSpace(value.GetLocaleValue(l))))
                                        {
                                            value.SetLocaleValue(l, v);
                                        }
                                    }
                                    if (isNewer)
                                    {
                                        value.Timestamp = str.Timestamp;
                                    }
                                }
                                else
                                {
                                    value = new LocaleString(str.Owner, str, true, false)
                                    {
                                        Timestamp = str.Timestamp
                                    };
                                    dic.Add(key, value);
                                }
                            }
                        }
                    }

                    strings = new List<LocaleString>(dic.Values);
                }

                var cells = new string[strings.Count + 1][];
                cells[0] = new string[2 + targetLocales.Length];
                cells[0][0] = keyLocale ?? "key";
                cells[0][1] = "location";
                Array.Copy(targetLocales, 0, cells[0], 2, targetLocales.Length);

                var row = 1;
                foreach (var str in strings)
                {
                    var key = keyLocale != null ? CSV.Make(new string[] { str.GetLocaleValue(keyLocale) }) : str.Key;

                    cells[row] = new string[3 + targetLocales.Length];
                    cells[row][0] = keyLocale != null ? CSV.Make(new string[] { str.GetLocaleValue(keyLocale) }) : str.Key;
                    cells[row][1] = str.Owner.Location;

                    i = 2;
                    foreach (var locale in targetLocales)
                    {
                        cells[row][i++] = str.GetLocaleValue(locale);
                    }
                    cells[row][i++] = str.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

                    row++;
                }
                return cells;
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }

        public void LocaleImport(string[][] cells, List<LocaleString.ImportCollision> collisions)
        {
            LocaleImport(new Asset[] { this }, cells, collisions);
        }

        public static void LocaleImport(Asset[] assets, string[][] cells, List<LocaleString.ImportCollision> collisions)
        {
            var strings = new List<LocaleString>();

            var progress = 0;
            foreach (var asset in assets) progress += asset.AllChildrenCount;
            AssetManager.Instance.BeginProgress(progress);

            try
            {
                foreach (var asset in assets) asset.GetLocaleStrings(strings);

                {
                    var i = 0;
                    while (i < strings.Count)
                    {
                        var str = strings[i];

                        if (!str.Locale) strings.RemoveAt(i);
                        else i++;
                    }
                }

                var dic = new Dictionary<string, string[]>();

                var keyBase = cells[0][0];
                var byLocale = !keyBase.Equals("key");
                var locales = new string[cells[0].Length - 2];
                Array.Copy(cells[0], 2, locales, 0, locales.Length);

                for (var i = 1; i < cells.GetLength(0); i++)
                {
                    var row = cells[i];

                    var key = row[0];
                    var values = new string[locales.Length];
                    Array.Copy(row, 2, values, 0, locales.Length);
                    for (var j = 0; j < locales.Length; j++)
                    {
                        if (values[j] != null)
                        {
                            values[j] = values[j].Replace("\n", "\r\n").Replace("\r\r", "\r");
                        }
                    }
                    if (dic.ContainsKey(key)) dic[key] = values;
                    else dic.Add(key, values);
                }

                foreach (var str in strings)
                {
                    var key = byLocale ? str.GetLocaleValue(keyBase) : str.Key;

                    if (dic.TryGetValue(key, out var values))
                    {
                        var checkCollision = !byLocale && collisions != null;

                        var collision = str.Import(values, locales, checkCollision);

                        if (collision != null) collisions.Add(collision);
                    }
                }
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }
        
        public bool Linked { private set; get; }

        private string backupPath;
        private SortedSet<string> backupRetains;

        internal virtual void Link()
        {
            if (IsDirty)
            {
                AssetManager.Instance.Dirties.Add(this);
            }
            if (AssetManager.Instance.LinkAsset(this)) Linked = true;

            if (backupPath != null)
            {
                MovePath(backupPath, ContentPath);
                backupPath = null;
            }
            if (backupRetains != null)
            {
                if (Linked) Project.Retains.Update(Key, backupRetains);
                backupRetains = null;
            }

            foreach (var child in Children) child.Link();
        }

        internal virtual void Unlink(bool dirty)
        {
            if (IsDirty)
            {
                AssetManager.Instance.Dirties.Remove(this);
            }
            if (dirty)
            {
                if (Linked)
                {
                    var project = Project;
                    backupRetains = project.Retains.GetTargets(Key);
                    project.Retains.Update(Key, null);
                }

                backupPath = Path.Combine(ResourceManager.Instance.DirectoryPath, Key);

                MovePath(ContentPath, backupPath);
            }
            AssetManager.Instance.UnlinkAsset(this);

            Linked = false;

            foreach (var child in Children) child.Unlink(dirty);
        }
        
        protected virtual void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentPath);
        }

        private void GetAllDataPaths(List<string> paths)
        {
            GetDataPaths(paths);

            foreach (var child in Children) child.GetAllDataPaths(paths);
        }

        private void GetAllPaths(string dirpath, List<string> paths)
        {
            foreach (var path in Directory.GetFiles(dirpath)) paths.Add(path.ToLower());

            foreach (var path in Directory.GetDirectories(dirpath))
            {
                paths.Add(path.ToLower());

                GetAllPaths(path, paths);
            }
        }

        public void CleanFiles()
        {
            var path = ContentPath;

            if (Directory.Exists(path))
            {
                var remainingPaths = new List<string>();
                GetAllPaths(path, remainingPaths);

                var dataPaths = new List<string>();
                GetAllDataPaths(dataPaths);

                foreach (var datapath in dataPaths)
                {
                    remainingPaths.Remove(datapath.ToLower());
                }
                foreach (var remainingPath in remainingPaths)
                {
                    if (File.Exists(remainingPath)) File.Delete(remainingPath);
                    else if (Directory.Exists(remainingPath)) Directory.Delete(remainingPath, true);
                }
            }
        }

        public DateTime DataTime
        {
            get
            {
                var project = Project;
                
                if (project == null) return DateTime.MinValue;
                
                var time = project.Times.GetTime(Key);
                foreach (var child in AllChildren)
                {
                    var childTime = project.Times.GetTime(child.Key);
                    if (childTime.CompareTo(time) > 0) time = childTime;
                }
                return time;
            }
        }
        
        public void ReissueKey()
        {
            var assets = GetRetainOrigins();

            if (Linked)
            {
                foreach (var projectAsset in AssetManager.Instance.Projects)
                {
                    projectAsset.Retains.Update(Key, null);
                }
            }
            Key = AssetManager.NewKey();
            Save(true);
            foreach (var asset in assets) asset.Save(true);
        }

        public void ReissueKeyAll() => ReissueKeyAll(new Asset[] { this });

        public static void ReissueKeyAll(Asset[] assets)
        {
            var sources = new List<Asset>();
            foreach (var asset in assets)
            {
                foreach (var child in asset.Children)
                {
                    if (sources.Contains(child)) sources.Add(child);
                }
                if (sources.Contains(asset)) sources.Add(asset);
            }
            
            AssetManager.Instance.BeginProgress(sources.Count);

            try
            {
                foreach (var source in sources)
                {
                    if (!AssetManager.Instance.Progress(source.Location, 1))
                    {
                        throw new AssetException(null, "작업이 중단되었습니다.");
                    }
                    source.ReissueKey();
                }
            }
            finally
            {
                AssetManager.Instance.EndProgress();
            }
        }

        internal Asset GetRelativeAsset(Asset sibling, Asset siblingRef)
        {
            var indices = new List<int>();

            var siblingContainer = siblingRef;

            for (; ; )
            {
                if (siblingContainer.Parent == null) return siblingRef;
                if (siblingContainer.Parent.IsListed) break;
                indices.Add(siblingContainer.Index);
                siblingContainer = siblingContainer.Parent;
            }
            var current = sibling;
            var depth = 0;
            for (; ; )
            {
                if (current == null) return siblingRef;
                if (current == siblingContainer) break;
                depth++;
                current = current.Parent;
            }
            var rtn = this;
            for (var i = 0; i < depth; i++)
            {
                rtn = rtn.Parent;
                if (rtn == null) return siblingRef;
            }
            foreach (var index in indices)
            {
                if (index >= rtn.Children.Count) return siblingRef;
                rtn = rtn.Children[index];
            }
            return rtn.Compare(siblingRef, false) ? rtn : siblingRef;
        }
        
        public static Asset Create(AssetType type)
        {
            switch (type)
            {
                case AssetType.Package:
                    return new Containers.PackageAsset();
                case AssetType.List:
                    return new Containers.ListAsset();
                case AssetType.File:
                    return new Containers.FileAsset();
                case AssetType.Folder:
                    return new Containers.FolderAsset();
                case AssetType.Block:
                    return new Containers.BlockAsset();
                case AssetType.BlockList:
                    return new Containers.BlockListAsset();
                case AssetType.Binary:
                    return new Binary.BinaryAsset();
                case AssetType.Media:
                    return new Media.MediaAsset();
                case AssetType.String:
                    return new String.StringAsset();
                case AssetType.Attribute:
                    return new Attributes.AttributeAsset();
                case AssetType.RootImage:
                    return new Texturing.RootImageAsset();
                case AssetType.SubImage:
                    return new Texturing.SubImageAsset();
                case AssetType.Material:
                    return new Texturing.MaterialAsset();
                case AssetType.Skybox:
                    return new Texturing.SkyboxAsset();
                case AssetType.Mesh:
                    return new Meshing.MeshAsset();
                case AssetType.Animation:
                    return new Animations.AnimationAsset();
                case AssetType.Spawn:
                    return new Spawn.SpawnAsset();
                case AssetType.Terrain:
                    return new Terrain.TerrainAsset();
                case AssetType.Trigger:
                    return new Triggers.TriggerAsset();
                case AssetType.TriggerFormat:
                    return new Triggers.TriggerFormatAsset();
                case AssetType.Reference:
                    return new Reference.ReferenceAsset();
                case AssetType.Independent:
                    return new Containers.IndependentAsset();
                case AssetType.Version:
                    return new Version.VersionAsset();
                case AssetType.Unused:
                    return new Containers.UnusedAsset();
            }
            throw new NotImplementedException();
        }
    }
}
