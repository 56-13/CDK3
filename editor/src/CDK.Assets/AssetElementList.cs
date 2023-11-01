using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace CDK.Assets
{
    public class BeforeListChangedEventArgs<T> : EventArgs
    {
        public ListChangedType ListChangedType { private set; get; }
        public int NewIndex { private set; get; }
        public int OldIndex { private set; get; }
        public T Object { set; get; }
        public bool Cancel { set; get; }

        public BeforeListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
        {
            ListChangedType = listChangedType;
            NewIndex = newIndex;
            OldIndex = oldIndex;
        }

        public BeforeListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex, T obj)
            : this(listChangedType, newIndex, oldIndex)

        {
            Object = obj;
        }
    }

    public class AssetElementList<T> : BindingList<T>
    {
        public AssetElement Parent { private set; get; }

        private class ClearCommand : IAssetCommand
        {
            private AssetElementList<T> list;
            private T[] items;

            public Asset Asset => list.Parent?.Owner;

            public ClearCommand(AssetElementList<T> list)
            {
                this.list = list;
                this.items = new T[list.Count];
                list.CopyTo(items, 0);
            }

            public void Undo()
            {
                foreach (T item in items) list.Add(item);
            }

            public void Redo()
            {
                list.Clear();
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class AddCommand : IAssetCommand
        {
            private AssetElementList<T> list;
            private int index;
            private T item;

            public Asset Asset => list.Parent?.Owner;

            public AddCommand(AssetElementList<T> list, int index)
            {
                this.list = list;
                this.index = index;
                item = list[index];
            }

            public void Undo()
            {
                list.RemoveAt(index);
            }

            public void Redo()
            {
                list.Insert(index, item);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class RemoveCommand : IAssetCommand
        {
            private AssetElementList<T> list;
            private int index;
            private T item;

            public Asset Asset => list.Parent?.Owner;

            public RemoveCommand(AssetElementList<T> list, int index)
            {
                this.list = list;
                this.index = index;
                this.item = list[index];
            }

            public void Undo()
            {
                list.Insert(index, item);
            }

            public void Redo()
            {
                list.RemoveAt(index);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class SetCommand : IAssetCommand
        {
            private AssetElementList<T> list;
            private int index;
            private T oldItem;
            private T newItem;

            public Asset Asset => list.Parent?.Owner;

            public SetCommand(AssetElementList<T> list, int index, T newItem)
            {
                this.list = list;
                this.index = index;
                this.oldItem = list[index];
                this.newItem = newItem;
            }

            public void Undo()
            {
                list[index] = oldItem;
            }

            public void Redo()
            {
                list[index] = newItem;
            }

            public bool Merge(IAssetCommand other) => false;
        }

        private class MoveCommand : IAssetCommand
        {
            private AssetElementList<T> list;
            private int newIndex;
            private int oldIndex;

            public Asset Asset => list.Parent?.Owner;

            public MoveCommand(AssetElementList<T> list, int newIndex, int oldIndex)
            {
                this.list = list;
                this.newIndex = newIndex;
                this.oldIndex = oldIndex;
            }

            public void Undo()
            {
                list.Move(newIndex, oldIndex);
            }

            public void Redo()
            {
                list.Move(oldIndex, newIndex);
            }

            public bool Merge(IAssetCommand other) => false;
        }

        public event EventHandler<BeforeListChangedEventArgs<T>> BeforeListChanged;

        private WeakReference<AssetElementList<T>> _bindingSource;
        private bool _bindingDeepCopy;
        private bool _bindingChanging;

        public AssetElementList(AssetElement parent)
        {
            Parent = parent;

            _reservedEvents = new List<ListChangedEventArgs>();
        }

        public AssetElementList(AssetElement parent, AssetElementList<T> other, bool binding, bool deepCopy)
        {
            Parent = parent;

            _reservedEvents = new List<ListChangedEventArgs>();

            Bind(other, binding, deepCopy);
        }

        internal void Bind(AssetElementList<T> other, bool binding, bool deepCopy)
        {
            UnBind();

            Clear();
            foreach (var item in other)
            {
                if (deepCopy)
                {
                    if (item is IBindingCloneable<T> bcitem) Add(bcitem.Clone(binding));
                    else if (item is ICloneable citem) Add((T)citem.Clone());
                    else Add(item);
                }
                else Add(item);
            }

            if (binding)
            {
                _bindingSource = new WeakReference<AssetElementList<T>>(other);
                _bindingDeepCopy = deepCopy;
                other.AddWeakListChanged(Binding_ListChanged);
            }
        }

        internal void UnBind()
        {
            if (_bindingSource != null && _bindingSource.TryGetTarget(out var prev))
            {
                prev.RemoveWeakListChanged(Binding_ListChanged);
            }
            _bindingSource = null;
        }

        private T GetBindingItem(in T item)
        {
            if (_bindingDeepCopy)
            {
                if (item is IBindingCloneable<T> bcitem) return bcitem.Clone(true);
                else if (item is ICloneable citem) return (T)citem.Clone();
            }
            return item;
        }

        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingSource != null && !_bindingChanging && AssetManager.Instance.CommandEnabled && _bindingSource.TryGetTarget(out var bindingSource)) 
            {
                var src = (AssetElementList<T>)sender;
                var dest = src == this ? bindingSource : this;

                _bindingChanging = true;

                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        {
                            var item = GetBindingItem(src[e.NewIndex]);
                            dest.Insert(e.NewIndex, item);
                        }
                        break;
                    case ListChangedType.ItemChanged:
                        if (e.PropertyDescriptor == null)
                        {
                            var item = GetBindingItem(src[e.NewIndex]);
                            dest[e.NewIndex] = item;
                        }
                        break;
                    case ListChangedType.ItemDeleted:
                        dest.RemoveAt(e.NewIndex);
                        break;
                    case ListChangedType.ItemMoved:
                        dest.Move(e.OldIndex, e.NewIndex);
                        break;
                    case ListChangedType.Reset:
                        dest.Clear();
                        foreach (var i in src)
                        {
                            var item = GetBindingItem(i);
                            dest.Add(item);
                        }
                        break;
                }

                _bindingChanging = false;
            }
        }

        internal new bool RaiseListChangedEvents
        {
            set => base.RaiseListChangedEvents = value;
            get => base.RaiseListChangedEvents;
        }

        protected override void ClearItems()
        {
            if (RaiseListChangedEvents)
            {
                if (BeforeListChanged != null)
                {
                    var e = new BeforeListChangedEventArgs<T>(ListChangedType.Reset, 0, 0);

                    BeforeListChanged(this, e);

                    if (e.Cancel) return;
                }
            }

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new ClearCommand(this));

                if (Parent != null) Parent.IsDirty = true;
            }

            base.ClearItems();

            Parent?.OnRefresh();
        }

        protected override void InsertItem(int index, T item)
        {
            if (RaiseListChangedEvents && BeforeListChanged != null)
            {
                var e = new BeforeListChangedEventArgs<T>(ListChangedType.ItemAdded, index, 0, item);

                BeforeListChanged(this, e);

                if (e.Cancel) return;
            }
            base.InsertItem(index, item);

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new AddCommand(this, index));

                if (Parent != null) Parent.IsDirty = true;
            }

            Parent?.OnRefresh();
        }

        protected override void RemoveItem(int index)
        {
            if (RaiseListChangedEvents)
            {
                if (BeforeListChanged != null)
                {
                    var e = new BeforeListChangedEventArgs<T>(ListChangedType.ItemDeleted, index, 0, this[index]);

                    BeforeListChanged(this, e);

                    if (e.Cancel) return;
                }
            }

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new RemoveCommand(this, index));

                if (Parent != null) Parent.IsDirty = true;
            }

            base.RemoveItem(index);

            Parent?.OnRefresh();
        }
        //============================================================================================================
        private bool setting;
        private bool listChangedProcessing;
        private List<ListChangedEventArgs> _reservedEvents;

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor == null && !setting) return;

            setting = false;
            if (listChangedProcessing)
            {
                _reservedEvents.Add(e);
            }
            else
            {
                listChangedProcessing = true;
                base.OnListChanged(e);
                Binding_ListChanged(this, e);
                foreach (var re in _reservedEvents)
                {
                    base.OnListChanged(re);
                    Binding_ListChanged(this, re);
                }

                _reservedEvents.Clear();
                listChangedProcessing = false;
            }
        }
        //이벤트가 중첩되서 호출되지 않게 처리 (model change 1->model list changed 1->model change 2->control list changed 2->control list changed 1) 와 같은 경우가 발생하지 않게 한다.
        //T의 상속된 항목의 property가 변경되었을 때 ItemChanged가 발생하지 않게 처리. ItemChanged는 실제 항목이 변경되었을 때 혹은 T의 property가 변경되었을 때만 처리되게 수정
        //============================================================================================================

        protected override void SetItem(int index, T item)
        {
            if (RaiseListChangedEvents)
            {
                if (BeforeListChanged != null)
                {
                    var e = new BeforeListChangedEventArgs<T>(ListChangedType.ItemChanged, index, 0, item);

                    BeforeListChanged(this, e);

                    if (e.Cancel) return;
                }
            }

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new SetCommand(this, index, item));

                if (Parent != null) Parent.IsDirty = true;
            }

            setting = true;
            //============================================================================================================
            //T의 상속된 항목의 property가 변경되었을 때 ItemChanged가 발생하지 않게 처리. ItemChanged는 실제 항목이 변경되었을 때 혹은 T의 property가 변경되었을 때만 처리되게 수정
            //============================================================================================================
            base.SetItem(index, item);

            Parent?.OnRefresh();
        }

        public void Move(int src, int dest)
        {
            if (RaiseListChangedEvents)
            {
                if (BeforeListChanged != null)
                {
                    var e = new BeforeListChangedEventArgs<T>(ListChangedType.ItemMoved, dest, src, this[src]);

                    BeforeListChanged(this, e);

                    if (e.Cancel) return;
                }
            }

            if (AssetManager.Instance.CommandEnabled)
            {
                AssetManager.Instance.Command(new MoveCommand(this, dest, src));

                if (Parent != null) Parent.IsDirty = true;
            }

            using (new AssetCommandHolder())
            {
                var raiseListChangedEvents = RaiseListChangedEvents;
                RaiseListChangedEvents = false;
                T item = this[src];
                RemoveAt(src);
                Insert(dest, item);
                RaiseListChangedEvents = raiseListChangedEvents;
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, dest, src));
            }

            Parent?.OnRefresh();
        }

        public void AddWeakListChanged(EventHandler<ListChangedEventArgs> handler)
        {
            WeakEventManager<AssetElementList<T>, ListChangedEventArgs>.AddHandler(this, "ListChanged", handler);
        }

        public void RemoveWeakListChanged(EventHandler<ListChangedEventArgs> handler)
        {
            WeakEventManager<AssetElementList<T>, ListChangedEventArgs>.RemoveHandler(this, "ListChanged", handler);
        }

        public void AddWeakBeforeListChanged(EventHandler<BeforeListChangedEventArgs<T>> handler)
        {
            WeakEventManager<AssetElementList<T>, BeforeListChangedEventArgs<T>>.AddHandler(this, "BeforeListChanged", handler);
        }

        public void RemoveWeakBeforeListChanged(EventHandler<BeforeListChangedEventArgs<T>> handler)
        {
            WeakEventManager<AssetElementList<T>, BeforeListChangedEventArgs<T>>.RemoveHandler(this, "BeforeListChanged", handler);
        }
    }
}
