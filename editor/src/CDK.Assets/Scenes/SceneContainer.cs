using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace CDK.Assets.Scenes
{
    public abstract class SceneContainer : AssetElement
    {
        public override bool IsListed => true;
        public AssetElementList<SceneComponent> Children { private set; get; }
        public override IEnumerable<AssetElement> GetChildren() => Children.Select(e => (AssetElement)e);
        public bool Fixed { internal set; get; }
        
        private bool _listChanging;
        
        public SceneContainer()
        {
            Children = new AssetElementList<SceneComponent>(this);
            Children.BeforeListChanged += Objects_BeforeListChanged;
            Children.ListChanged += Objects_ListChanged;
        }

        public SceneContainer(SceneContainer other, bool binding, bool children) : base(other, binding)
        {
            AssetManager.Instance.AddRedirection(other, this);

            Children = new AssetElementList<SceneComponent>(this);
            Children.BeforeListChanged += Objects_BeforeListChanged;
            Children.ListChanged += Objects_ListChanged;

            if (children)
            {
                using (new AssetCommandHolder()) Children.Bind(other.Children, binding, true);
            }

            Fixed = other.Fixed;
        }

        private void Objects_BeforeListChanged(object sender, BeforeListChangedEventArgs<SceneComponent> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    if (!AddSubEnabled(e.Object)) e.Cancel = true;
                    break;
                case ListChangedType.ItemChanged:
                    if (!AddSubEnabled(e.Object)) e.Cancel = true;
                    goto case ListChangedType.ItemDeleted;
                case ListChangedType.ItemDeleted:
                    {
                        var obj = Children[e.NewIndex];
                        if (!_listChanging && (obj.Fixed || obj.IsRetained())) e.Cancel = true;
                        else obj.Parent = null;
                    }
                    break;
                case ListChangedType.Reset:
                    if (!_listChanging)
                    {
                        foreach (var obj in Children)
                        {
                            if (obj.Fixed || obj.IsRetained())
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                    foreach (var obj in Children) obj.Parent = null;
                    break;
            }
        }

        private void Objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        var obj = Children[e.NewIndex];
                        if (obj.Parent != null)
                        {
                            if (obj.Parent is SceneContainer parent) parent.Children.Remove(obj);
                            else throw new InvalidOperationException();
                        }
                        obj.Parent = this;
                    }
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        var obj = Children[e.NewIndex];
                        if (obj.Parent != null)
                        {
                            if (obj.Parent is SceneContainer parent) parent.Children.Remove(obj);
                            else throw new InvalidOperationException();
                        }
                        obj.Parent = this;
                    }
                    break;
            }
        }

        public abstract SceneComponentType[] SubTypes { get; }
        public virtual bool AddSubEnabled(SceneComponent obj) => SubTypes.Contains(obj.Type) && obj.Contains(this);
        public virtual void AddSub(SceneComponentType type) => Children.Add(SceneComponent.Create(GetAncestor<Scene>(true).Config, type));
        public virtual string ImportFilter => null;
        public virtual void Import(string path) { }

        public bool Attach(SceneComponent obj, bool move)
        {
            if (move)
            {
                if (!AddSubEnabled(obj)) return false;

                if (obj.Parent != null)
                {
                    if (!obj.Fixed && obj.Parent is SceneContainer parent)
                    {
                        _listChanging = true;
                        try
                        {
                            parent.Children.Remove(obj);
                        }
                        finally
                        {
                            _listChanging = false;
                        }
                    }
                    else return false;
                }
            }
            else
            {
                obj = obj.Clone(false);
                if (obj == null || !AddSubEnabled(obj)) return false;
            }
            Children.Add(obj);
            return true;
        }

        public bool Paste()
        {
            var clip = AssetManager.Instance.ClipObject;

            if (clip is SceneComponent clipobj)
            {
                var cut = AssetManager.Instance.ClipCut;

                if (Attach(clipobj, cut)) return true;

                if (GetParent() is SceneContainer parent && parent.Attach(clipobj, cut)) return true;
            }
            return false;
        }

        protected virtual void SaveChildren(XmlWriter writer) 
        {
            foreach (var obj in Children) obj.Save(writer);
        }

        protected virtual void LoadChildren(XmlNode node)
        {
            _listChanging = true;
            try
            {
                var config = GetAncestor<Scene>(true).Config;

                Children.Clear();
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    var obj = SceneComponent.Create(config, subnode);
                    Children.Add(obj);
                    obj.Load(subnode);
                }
            }
            finally
            {
                _listChanging = false;
            }
        }
        
        internal virtual void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var obj in Children) obj.GetLocaleStrings(strings);
        }
    }
}
