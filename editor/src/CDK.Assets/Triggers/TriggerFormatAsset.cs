using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.IO;

using CDK.Assets.Updaters;

namespace CDK.Assets.Triggers
{
    public class TriggerFormatAsset : Asset
    {
        private Dictionary<string, TriggerElementItem[]> _items;
        private Dictionary<string, TriggerFormat> _wraps;
        private List<TriggerFormat> _formats;

        private static readonly TriggerFormat ErrorFormat = new TriggerFormat("알 수 없는 포맷", -1, "알 수 없는 포맷", new List<TriggerElementFormat>());

        private IntegerElementType _CodeBoundary;
        public IntegerElementType CodeBoundary
        {
            get
            {
                Load();
                return _CodeBoundary;
            }
        }

        private IntegerElementType _SetBoundary;
        public IntegerElementType SetBoundary
        {
            get
            {
                Load();
                return _SetBoundary;
            }
        }

        private IntegerElementType _ContainerBoundary;
        public IntegerElementType ContainerBoundary
        {
            get
            {
                Load();
                return _ContainerBoundary;
            }
        }

        public class TriggerFormatCollection : IEnumerable<TriggerFormat>, IListSource
        {
            private TriggerFormatAsset _asset;
            public TriggerFormat this[int idx] => _asset._formats[idx];
            public TriggerFormat this[string name] => _asset._formats.FirstOrDefault(f => f.Name == name) ?? ErrorFormat;
            public int Count => _asset._formats.Count;

            internal TriggerFormatCollection(TriggerFormatAsset asset)
            {
                _asset = asset;
            }

            bool IListSource.ContainsListCollection => false;

            IList IListSource.GetList() => _asset._formats.AsReadOnly();
            IEnumerator<TriggerFormat> IEnumerable<TriggerFormat>.GetEnumerator() => _asset._formats.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _asset._formats.GetEnumerator();
        }

        private TriggerFormatCollection _Formats;
        public TriggerFormatCollection Formats
        {
            get
            {
                Load();
                return _Formats;
            }
        }

        public event EventHandler Reset;
        
        public void AddWeakReset(EventHandler<EventArgs> handler)
        {
            WeakEventManager<TriggerFormatAsset, EventArgs>.AddHandler(this, "Reset", handler);
        }

        public void RemoveWeakReset(EventHandler<EventArgs> handler)
        {
            WeakEventManager<TriggerFormatAsset, EventArgs>.RemoveHandler(this, "Reset", handler);
        }

        public TriggerFormatAsset()
        {
            _items = new Dictionary<string, TriggerElementItem[]>();
            _wraps = new Dictionary<string, TriggerFormat>();
            _formats = new List<TriggerFormat>();

            _Formats = new TriggerFormatCollection(this);
        }

        public TriggerFormatAsset(TriggerFormatAsset other, bool content) : base(other, content)
        {
            if (content)
            {
                other.Load();

                _items = other._items;
                _wraps = other._wraps;
                _formats = other._formats;
            }
            else
            {
                _items = new Dictionary<string, TriggerElementItem[]>();
                _wraps = new Dictionary<string, TriggerFormat>();
                _formats = new List<TriggerFormat>();
            }

            _Formats = new TriggerFormatCollection(this);
        }

        internal TriggerElementItem[] GetItems(string key)
        {
            return _items.TryGetValue(key, out var item) ? item : null;
        }

        internal TriggerFormat GetWrap(string key)
        {
            return _wraps.TryGetValue(key, out var wrap) ? wrap : null;
        }

        public override AssetType Type => AssetType.TriggerFormat;

        private void Load(XmlNode node, Dictionary<string, TriggerElementItem[]> items, Dictionary<string, TriggerFormat> wraps, List<TriggerFormat> formats)
        {
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.NodeType == XmlNodeType.EntityReference)
                {
                    Load(subnode, items, wraps, formats);
                }
                else if (subnode.NodeType == XmlNodeType.Element)
                {
                    if (subnode.LocalName == "items")
                    {
                        var key = subnode.Attributes["name"].Value;
                        var subitems = new List<TriggerElementItem>();

                        foreach (XmlNode itemNode in subnode.ChildNodes)
                        {
                            if (itemNode.LocalName == "item")
                            {
                                var item = new TriggerElementItem(itemNode, subitems.Count);

                                subitems.Add(item);
                            }
                        }
                        items.Add(key, subitems.ToArray());
                    }
                    else if (subnode.LocalName == "wrap")
                    {
                        var key = subnode.Attributes["name"].Value;

                        foreach (XmlNode formatNode in subnode.ChildNodes)
                        {
                            if (formatNode.LocalName == "triggerFormat")
                            {
                                var format = new TriggerFormat(formatNode, 0, false);

                                wraps.Add(key, format);
                            }
                        }
                    }
                    else if (subnode.LocalName == "triggerFormat")
                    {
                        var format = new TriggerFormat(subnode, formats.Count, true);

                        formats.Add(format);
                    }
                }
            }
        }

        private void Load(XmlNode node)
        {
            if (node.LocalName != "triggerFormatAsset") throw new XmlException();

            var codeBoundary = node.ReadAttributeEnum<IntegerElementType>("codeBoundary");
            var setBoundary = node.ReadAttributeEnum<IntegerElementType>("setBoundary");
            var containerBoundary = node.ReadAttributeEnum<IntegerElementType>("containerBoundary");

            var items = new Dictionary<string, TriggerElementItem[]>();
            var wraps = new Dictionary<string, TriggerFormat>();
            var formats = new List<TriggerFormat>();

            Load(node, items, wraps, formats);

            _CodeBoundary = codeBoundary;
            _SetBoundary = setBoundary;
            _ContainerBoundary = containerBoundary;

            _items = items;
            _wraps = wraps;
            _formats = formats;

            IsDirty = true;

            Reset?.Invoke(this, EventArgs.Empty);
        }

        public void Load(string xml)
        {
            var doc = new XmlDocument();

            doc.LoadXml(xml);

            Load(doc.ChildNodes[1]);
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("triggerFormatAsset");
            writer.WriteAttribute("version", Updater.AssetVersion); 
            writer.WriteAttribute("codeBoundary", CodeBoundary);
            writer.WriteAttribute("setBoundary", SetBoundary);
            writer.WriteAttribute("containerBoundary", ContainerBoundary);

            foreach (var key in _items.Keys)
            {
                writer.WriteStartElement("items");
                writer.WriteAttribute("name", key);
                foreach (var item in _items[key])
                {
                    item.Write(writer);
                }
                writer.WriteEndElement();
            }
            foreach (var key in _wraps.Keys)
            {
                writer.WriteStartElement("wrap");
                writer.WriteAttribute("name", key);
                _wraps[key].Save(writer);
                writer.WriteEndElement();
            }
            foreach (var format in _formats)
            {
                format.Save(writer);
            }

            writer.WriteEndElement();
        }

        public override Asset Clone(bool content) => new TriggerFormatAsset(this, content);

        public override void Import(string path)
        {
            var doc = new XmlDocument();

            doc.Load(path);

            foreach (XmlNode node in doc.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element && node.LocalName == "triggerFormatAsset")
                {
                    Load(node);

                    return;
                }
            }

            throw new XmlException();
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
                Save(writer);
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

                Load(doc.ChildNodes[1]);
            }
        }

        protected override void GetDataPaths(List<string> paths)
        {
            paths.Add(ContentXmlPath);
        }
    }
}
