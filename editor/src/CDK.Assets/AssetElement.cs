using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;

using CDK.Assets.Containers;

namespace CDK.Assets
{
    public abstract class AssetElement : NotifyPropertyChanged
    {
        private WeakReference<AssetElement> _bindingSource;
        private bool _bindingChanging;

        protected AssetElement() { }

        protected AssetElement(AssetElement other, bool binding)
        {
            if (binding)
            {
                _bindingSource = new WeakReference<AssetElement>(other);
                other.AddWeakPropertyChanged(BindingSource_PropertyChanged);
                PropertyChanged += AssetElement_PropertyChanged;
            }
        }

        internal bool GetBindingSource<T>(out T bindingSource) where T : AssetElement
        {
            if (_bindingSource == null || !_bindingSource.TryGetTarget(out var bindingSource_))
            {
                bindingSource = null;
                return false;
            }
            bindingSource = (T)bindingSource_;
            return true;
        }

        protected bool BeginBinding<T>(out T bindingSource) where T : AssetElement
        {
            if (_bindingSource == null || _bindingChanging || !AssetManager.Instance.CommandEnabled || !_bindingSource.TryGetTarget(out var bindingSource_))
            {
                bindingSource = null;
                return false;
            }
            bindingSource = (T)bindingSource_;
            _bindingChanging = true;
            return true;
        }

        protected void EndBinding()
        {
            Debug.Assert(_bindingChanging);
            _bindingChanging = false;
        }

        private void AssetElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BeginBinding(out AssetElement bindingSource))
            {
                try
                {
                    var prop = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).
                        FirstOrDefault(p => p.Name == e.PropertyName && p.GetCustomAttributes(typeof(BindingAttribute), false).Count() == 1);
                    if (prop != null) prop.SetValue(bindingSource, prop.GetValue(this));
                }
                finally
                {
                    EndBinding();
                }
            }
        }

        private void BindingSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BeginBinding(out AssetElement bindingSource))
            {
                try
                {
                    var prop = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).
                        FirstOrDefault(p => p.Name == e.PropertyName && p.GetCustomAttributes(typeof(BindingAttribute), false).Count() == 1);
                    if (prop != null) prop.SetValue(this, prop.GetValue(bindingSource));
                }
                finally
                {
                    EndBinding();
                }
            }
        }

        public abstract AssetElement GetParent();
        public T GetAncestor<T>(bool self) where T : AssetElement
        {
            var obj = self ? this : GetParent();
            while (obj != null)
            {
                if (obj is T t) return t;
                obj = obj.GetParent();
            }
            return null;
        }

        public bool Contains(AssetElement element)
        {
            var current = element;
            do
            {
                if (current == this) return true;
                current = current.GetParent();
            } while (current != null);
            return false;
        }

        public Asset Owner => GetAncestor<Asset>(true);
        public ProjectAsset Project => GetAncestor<ProjectAsset>(true);

        public virtual bool IsDirty
        {
            set
            {
                var parent = GetParent();
                if (parent != null) parent.IsDirty = value;
            }
            get => GetParent()?.IsDirty ?? false;
        }

        public virtual bool IsListed => false;
        public virtual string GetLocation() => GetParent()?.GetLocation() ?? string.Empty;
        public virtual IEnumerable<AssetElement> GetChildren() => new AssetElement[0];
        public virtual IEnumerable<AssetElement> GetSiblings() => new AssetElement[0];

        public event EventHandler Refresh;

        private bool _refresh;
        internal void OnRefresh()
        {
            if (!_refresh)      //circular call 방지
            {
                _refresh = true;
                Refresh?.Invoke(this, EventArgs.Empty);
                if (!(this is Asset)) GetParent()?.OnRefresh();     //not good but..
                _refresh = false;
            }
        }

        protected bool SetProperty<T>(ref T src, T dest, bool command = true, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(src, dest))
            {
                if (command)
                {
                    if (AssetManager.Instance.CommandEnabled)
                    {
                        AssetManager.Instance.Command(new AssetPropertyCommand(this, src, dest, propertyName));

                        IsDirty = true;
                    }
                }
                src = dest;
                OnPropertyChanged(propertyName);
                OnRefresh();
                return true;
            }
            return false;
        }

        protected bool SetSharedProperty<T>(ref T src, T dest, bool command = true, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref src, dest, command, propertyName))
            {
                if (AssetManager.Instance.RetrieveEnabled)
                {
                    using (new AssetRetrieveHolder())
                    {
                        var prop = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        foreach (var other in GetSiblings()) prop.SetValue(other, dest);
                    }
                }
                return true;
            }
            return false;
        }

        internal virtual void AddRetains(ICollection<string> retains) { }
        internal virtual bool IsRetaining(AssetElement element, out AssetElement from)
        {
            from = null;
            return false;
        }

        public bool IsRetained() => IsRetained(true, true, out _, out _);
        public bool IsRetained(bool retrieving, bool children) => IsRetained(retrieving, children, out _, out _);
        public bool IsRetained(out AssetElement from, out AssetElement to) => IsRetained(true, true, out from, out to);
        public virtual bool IsRetained(bool retrieving, bool children, out AssetElement from, out AssetElement to)
        {
            var owner = Owner;

            if (owner != null)
            {
                if (owner.IsRetaining(this, out from))
                {
                    to = this;
                    return true;
                }
                foreach (var a in owner.GetRetainOrigins())
                {
                    if (a.IsRetaining(this, out from))
                    {
                        to = this;
                        return true;
                    }
                }
            }
            else
            {
                var root = GetParent();
                if (root != null)
                {
                    while (root.GetParent() != null) root = root.GetParent();
                    if (root.IsRetaining(this, out from))
                    {
                        to = this;
                        return true;
                    }
                }
            }
            if (retrieving)
            {
                var parent = GetParent();

                if (parent != null && !parent.IsListed)
                {
                    using (new AssetRetrieveHolder())
                    {
                        foreach (var a in GetSiblings())
                        {
                            if (a.IsRetained(retrieving, children, out from, out to)) return true;
                        }
                    }
                }
            }
            if (children)
            {
                foreach (var a in GetChildren())
                {
                    if (a.IsRetained(false, children, out from, out to)) return true;
                }
            }
            from = null;
            to = null;
            return false;
        }

        protected bool IsValidReference(Asset root, Asset target, int depth, bool allowEmpty)
        {
            if (target == null) return allowEmpty;

            var current = target;

            var i = 0;
            while (i < depth)
            {
                current = current.Parent;

                if (current == null) return false;
                if (current.IsListed) i++;
            }
            return current == root;
        }

        protected void BuildReference(BinaryWriter writer, Asset root, Asset target, int depth, bool allowEmpty)
        {
            if (target != null)
            {
                if (target.Project != Project || target.IsUnused) throw new AssetException(Owner, $"{GetLocation()}에서 잘못된 리소스가 참조되었습니다.");

                var current = target;

                var i = 0;

                while (i < depth)
                {
                    current = current.Parent;

                    if (current == null) throw new AssetException(Owner, $"{GetLocation()}에서 잘못된 리소스가 참조되었습니다.");

                    if (current.IsListed)
                    {
                        writer.Write((short)current.Index);

                        i++;
                    }
                }
                if (current != root) throw new AssetException(Owner, $"{GetLocation()}에서 잘못된 리소스가 참조되었습니다.");
            }
            else if (allowEmpty)
            {
                for (var i = 0; i < depth; i++) writer.Write((short)-1);
            }
            else throw new AssetException(Owner, $"{GetLocation()}의 값이 입력되지 않았습니다.");
        }

        protected void BuildReference(BinaryWriter writer, Asset target)
        {
            if (target == null) throw new AssetException(Owner, $"{GetLocation()}의 값이 입력되지 않았습니다.");
            else if (target.Project != Project || target.IsUnused) throw new AssetException(Owner, $"{GetLocation()}에서 잘못된 리소스가 참조되었습니다.");

            var indices = target.Indices;
            writer.WriteLength(indices.Count());
            foreach (var index in indices) writer.Write((short)index);
        }

        public void AddWeakRefresh(EventHandler<EventArgs> handler)
        {
            WeakEventManager<AssetElement, EventArgs>.AddHandler(this, "Refresh", handler);
        }

        public void RemoveWeakRefresh(EventHandler<EventArgs> handler)
        {
            WeakEventManager<AssetElement, EventArgs>.RemoveHandler(this, "Refresh", handler);
        }
    }
}

