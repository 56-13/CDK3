using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace CDK.Assets.Triggers
{
    public class TriggerUnitSet : AssetElement
    {
        public Trigger Parent { private set; get; }
        public override AssetElement GetParent() => Parent.Sets.Contains(this) ? Parent : null;

        public AssetElementList<TriggerUnit> Units { private set; get; }

        private string _Name;
        public string Name
        {
            set
            {
                if (string.IsNullOrEmpty(value)) value = "New " + (Index + 1);

                if (SetProperty(ref _Name, value)) OnPropertyChanged("IndexName");
            }
            get => _Name;
        }

        public int Index => Parent.Sets.IndexOf(this);

        public string IndexName => Index + ". " + _Name;

        public TriggerUnitSet(Trigger parent)
        {
            Parent = parent;

            _Name = "New " + (parent.Sets.Count + 1);

            Units = new AssetElementList<TriggerUnit>(this);
            Units.BeforeListChanged += Units_BeforeListChanged;
            Units.ListChanged += Units_ListChanged;
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var unit in Units) unit.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (TriggerUnit unit in Units)
            {
                if (unit.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        private void Units_BeforeListChanged(object sender, BeforeListChangedEventArgs<TriggerUnit> e)
        {
            switch(e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    Units[e.NewIndex].Linked = false;
                    break;
                case ListChangedType.Reset:
                    foreach (var unit in Units) unit.Linked = false;
                    break;
            }
        }

        private void Units_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        if (Units[e.NewIndex].Parent != this) throw new InvalidOperationException();
                        Units[e.NewIndex].Linked = true;
                    }
                    break;
                case ListChangedType.ItemAdded:
                    if (Units[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    Units[e.NewIndex].Linked = true;
                    break;
                case ListChangedType.Reset:
                    foreach (TriggerUnit unit in Units)
                    {
                        if (unit.Parent != this) throw new InvalidOperationException();
                        unit.Linked = true;
                    }
                    break;
            }
        }

        public void ClipUnits(int[] indices, bool cut)
        {
            var units = new TriggerUnit[indices.Length];
            var i = 0;
            foreach (var index in indices) units[i++] = Units[index];
            AssetManager.Instance.Clip(units, cut);
        }

        public void PasteUnits(int index)
        {
            if (AssetManager.Instance.ClipObject is TriggerUnit[] clipUnits)
            {
                if (clipUnits[0].Parent.Parent.Format == Parent.Format)
                {
                    var cut = AssetManager.Instance.ClipCut;

                    var newUnits = new TriggerUnit[clipUnits.Length];

                    for (var i = 0; i < newUnits.Length; i++)
                    {
                        newUnits[i] = new TriggerUnit(this, clipUnits[i].Format);
                    }
                    for (var i = 0; i < newUnits.Length; i++)
                    {
                        newUnits[i].CopyFrom(clipUnits[i], clipUnits, newUnits, !cut);
                    }
                    for (var i = 0; i < newUnits.Length; i++)
                    {
                        Units.Insert(index + i, newUnits[i]);
                    } 
                    if (cut)
                    {
                        foreach (var clipUnit in clipUnits)
                        {
                            clipUnit.Parent.Units.Remove(clipUnit);
                        }
                        AssetManager.Instance.ClearClip();
                    }
                }
            }
        }

        public void CopyFrom(TriggerUnitSet src)
        {
            Name = src.Name;

            Units.Clear();

            foreach (var su in src.Units)
            {
                var du = new TriggerUnit(this, su.Format);

                Units.Add(du);
            }
            for (var i = 0; i < Units.Count; i++)
            {
                Units[i].CopyFrom(src.Units[i], true);
            }
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("triggerUnitSet");
            writer.WriteAttributeString("name", _Name);
            foreach (var unit in Units) unit.Save(writer);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "triggerUnitSet") throw new XmlException();

            Name = node.Attributes["name"].Value;

            Units.Clear();

            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName == "triggerUnit")
                {
                    var unit = new TriggerUnit(this, Parent.Format.Formats[subnode.Attributes["name"].Value]);

                    Units.Add(unit);
                }
            }
            var i = 0;
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.LocalName.Equals("triggerUnit"))
                {
                    Units[i++].Load(subnode);
                }
            }
        }

        internal void Build(BinaryWriter writer)
        {
            var count = 0;
            foreach (var unit in Units)
            {
                if (!unit.Annotated) count++;
            }
            writer.WriteLength(count);
            foreach (var unit in Units)
            {
                if (!unit.Annotated) unit.Build(writer);
            }
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var unit in Units) unit.GetLocaleStrings(strings);
        }

        public override string ToString() => _Name;
    }
}
