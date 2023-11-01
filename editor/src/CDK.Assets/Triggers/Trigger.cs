using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class Trigger : AssetElement
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public AssetElementList<TriggerUnitSet> Sets { private set; get; }
        public AssetElementList<TriggerMember> Members { private set; get; }

        private TriggerFormatAsset _Format;
        public TriggerFormatAsset Format
        {
            set
            {
                if (_Format != value)
                {
                    _Format?.RemoveWeakReset(Format_Reset);
                    _Format = value;
                    Format_Reset(value, EventArgs.Empty);
                    _Format?.AddWeakReset(Format_Reset);

                    OnPropertyChanged("Format");
                }
            }
            get => _Format;
        }

        private int _Dimension;
        public int Dimension
        {
            set => SetProperty(ref _Dimension, value);
            get => _Dimension;
        }

        public Trigger(AssetElement parent)
        {
            Parent = parent;

            _Dimension = 1;
            
            Sets = new AssetElementList<TriggerUnitSet>(this);
            Sets.ListChanged += Sets_ListChanged;

            Members = new AssetElementList<TriggerMember>(this);
            Members.BeforeListChanged += Members_BeforeListChanged;
            Members.ListChanged += Members_ListChanged;
        }

        public Trigger(AssetElement parent, Trigger trigger, bool content)
        {
            Parent = parent;

            _Format = trigger._Format;
            _Dimension = trigger._Dimension;

            Sets = new AssetElementList<TriggerUnitSet>(this);
            Sets.ListChanged += Sets_ListChanged;

            Members = new AssetElementList<TriggerMember>(this);
            Members.BeforeListChanged += Members_BeforeListChanged;
            Members.ListChanged += Members_ListChanged;

            if (content)
            {
                using (new AssetCommandHolder())
                {
                    foreach (var member in trigger.Members)
                    {
                        Members.Add(new TriggerMember(this, member.Name));
                    }
                    for (var i = 0; i < trigger.Sets.Count; i++)
                    {
                        Sets.Add(new TriggerUnitSet(this));
                    }
                    for (var i = 0; i < trigger.Sets.Count; i++)
                    {
                        Sets[i].CopyFrom(trigger.Sets[i]);
                    }
                }
            }
        }

        private void Members_BeforeListChanged(object sender, BeforeListChangedEventArgs<TriggerMember> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (Members.Any(m => m.Name == e.Object.Name) || Members[e.NewIndex].IsRetained())
                    {
                        e.Cancel = true;
                    }
                    break;
                case ListChangedType.ItemAdded:
                    if (Members.Count >= 256 || Members.Any(m => m.Name == e.Object.Name))
                    {
                        e.Cancel = true;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    if (Members[e.NewIndex].IsRetained())
                    {
                        e.Cancel = true;
                    }
                    break;
                case ListChangedType.Reset:
                    foreach (var member in Members)
                    {
                        if (member.IsRetained())
                        {
                            e.Cancel = true;
                            break;
                        }
                    }
                    break;
            }
        }

        private void Members_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null) break;
                    goto case ListChangedType.ItemAdded;
                case ListChangedType.ItemAdded:
                    if (Members[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.Reset:
                    foreach (var member in Members)
                    {
                        if (member.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
        }

        private void Sets_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (Sets[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    }
                    break;
                case ListChangedType.ItemAdded:
                    if (Sets[e.NewIndex].Parent != this) throw new InvalidOperationException();

                    for (int i = e.NewIndex; i < Sets.Count; i++)
                    {
                        Sets[i].OnPropertyChanged("IndexName");
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    for (int i = e.NewIndex; i < Sets.Count; i++)
                    {
                        Sets[i].OnPropertyChanged("IndexName");
                    }
                    break;
                case ListChangedType.ItemMoved:
                    Sets[e.OldIndex].OnPropertyChanged("IndexName");
                    Sets[e.NewIndex].OnPropertyChanged("IndexName");
                    break;
                case ListChangedType.Reset:
                    foreach (var set in Sets)
                    {
                        if (set.Parent != this) throw new InvalidOperationException();
                        set.OnPropertyChanged("IndexName");
                    }
                    break;
            }
        }

        private void Format_Reset(object sender, EventArgs e)
        {
            var owner = Owner;
            if (owner != null)
            {
                AssetManager.Instance.ClearCommands(owner);
            }
            using (new AssetCommandHolder())
            {
                foreach (var set in Sets)
                {
                    foreach (var unit in set.Units) unit.ResetFormat();
                }
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            if (_Format != null) retains.Add(_Format.Key);

            foreach (var set in Sets) set.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Format == element)
            {
                from = this;
                return true;
            }
            foreach (var set in Sets)
            {
                if (set.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        internal bool Compare(Trigger other)
        {
            return _Format == other._Format && _Dimension == other._Dimension;
        }

        internal void Build(BinaryWriter writer)
        {
            switch (_Dimension)
            {
                case 0:
                    if (Sets.Count == 0 || (Sets.Count == 1 && Sets[0].Units.Count == 0))
                    {
                        writer.Write((byte)0);
                        return;
                    }
                    if (Sets.Count != 1 || Sets[0].Units.Count != 1 || Sets[0].Units[0].Annotated)
                    {
                        throw new AssetException(Owner, "Dimension 속성이 0으로 설정되어 있을 경우 단일 트리거만 빌드됩니다.");
                    }
                    Sets[0].Units[0].Build(writer);
                    break;
                case 1:
                    if (Sets.Count == 0)
                    {
                        writer.Write((byte)0);
                        return;
                    }
                    if (Sets.Count != 1)
                    {
                        throw new AssetException(Owner, "Dimension 속성이 1로 설정되어 있을 경우 단일 트리거셋만 빌드됩니다.");
                    }
                    Sets[0].Build(writer);
                    break;
                case 2:
                    writer.WriteLength(Sets.Count);

                    foreach (var set in Sets)
                    {
                        set.Build(writer);
                    }
                    break;
            }
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("trigger");

            writer.WriteAttributeString("dimension", _Dimension.ToString());

            if (_Format != null)
            {
                if (_Format.Project != Owner.Project) throw new AssetException(Owner, "잘못된 포맷입니다.");

                writer.WriteAttributeString("format", _Format.Key);

                writer.WriteAttributes("members", Members.Select(m => m.Name));

                foreach (var set in Sets) set.Save(writer);
            }
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "trigger") throw new XmlException();

            Dimension = node.ReadAttributeInt("dimension");

            Members.Clear();
            Sets.Clear();

            if (node.HasAttribute("format"))
            {
                Format = (TriggerFormatAsset)node.ReadAttributeAsset("format");

                foreach (var member in node.ReadAttributeStrings("members"))
                {
                    Members.Add(new TriggerMember(this, member));
                }
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    Sets.Add(new TriggerUnitSet(this));
                }
                for (var i = 0; i < node.ChildNodes.Count; i++)
                {
                    Sets[i].Load(node.ChildNodes[i]);
                }
            }
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var set in Sets) set.GetLocaleStrings(strings);
        }
    }
}
