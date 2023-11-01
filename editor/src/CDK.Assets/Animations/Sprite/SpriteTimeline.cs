using System;
using System.ComponentModel;
using System.Xml;
using System.IO;

namespace CDK.Assets.Animations.Sprite
{
    public class SpriteTimeline : AssetElement
    {
        public Sprite Parent { internal set; get; }
        public override AssetElement GetParent() => Parent;

        private float _StartTime;
        public float StartTime
        {
            set
            {
                if (!_moving)
                {
                    if (value + MinDuration > _EndTime) value = _EndTime - MinDuration;
                    if (value < 0) value = 0;
                    if (Parent != null && Parent.IsCollided(this, _Layer, value, _EndTime, out _)) return;
                }
                if (SetProperty(ref _StartTime, value)) OnPropertyChanged("Duration");
            }
            get => _StartTime;
        }

        private float _EndTime;
        public float EndTime
        {
            set
            {
                if (!_moving)
                {
                    if (value < _StartTime + MinDuration) value = _StartTime + MinDuration;
                    if (Parent != null && Parent.IsCollided(this, _Layer, _StartTime, value, out _)) return;
                }
                if (SetProperty(ref _EndTime, value)) OnPropertyChanged("Duration");
            }
            get => _EndTime;
        }

        public float Duration => _EndTime - _StartTime;

        private int _Layer;
        public int Layer
        {
            set
            {
                if (!_moving && Parent != null && Parent.IsCollided(this, _Layer, _StartTime, _EndTime, out _)) return;

                SetProperty(ref _Layer, value);
            }
            get => _Layer;
        }

        private bool _Reset;
        public bool Reset
        {
            set => SetProperty(ref _Reset, value);
            get => _Reset;
        }

        public const float MinDuration = 0.01f;
        public const int MaxDuration = 60;

        public AssetElementList<SpriteElement> Elements { private set; get; }

        private bool _moving;

        public SpriteTimeline(int layer, float startTime, float endTime)
        {
            _StartTime = startTime;
            _EndTime = endTime;
            _Layer = layer;

            Elements = new AssetElementList<SpriteElement>(this);
            Elements.BeforeListChanged += Elements_BeforeListChanged;
        }

        public SpriteTimeline(int layer, float time, SpriteTimeline other) : this(layer, time, time + other._EndTime - other._StartTime)
        {
            _Reset = other._Reset;

            foreach (var element in other.Elements) Elements.Add(element.Clone());
        }

        public SpriteTimeline(SpriteTimeline other) : this(other._Layer, other._StartTime, other._EndTime)
        {
            _Reset = other._Reset;

            foreach (var element in other.Elements) Elements.Add(element.Clone());
        }

        internal SpriteTimeline(XmlNode node)
        {
            _StartTime = node.ReadAttributeFloat("startTime");
            _EndTime = node.ReadAttributeFloat("endTime");
            _Layer = node.ReadAttributeInt("layer");
            _Reset = node.ReadAttributeBool("reset");

            Elements = new AssetElementList<SpriteElement>(this);
            Elements.BeforeListChanged += Elements_BeforeListChanged;

            using (new AssetCommandHolder())
            {
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    var name = subnode.LocalName;
                    name = name.Substring(0, 1).ToUpper() + name.Substring(1);
                    var type = (SpriteElementType)Enum.Parse(typeof(SpriteElementType), name);
                    var element = SpriteElement.Create(type);
                    element.Load(subnode);
                    Elements.Add(element);
                }
            }
        }

        private void Elements_BeforeListChanged(object sender, BeforeListChangedEventArgs<SpriteElement> e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.Object.Parent == this) e.Cancel = true;
                    else
                    {
                        Elements[e.NewIndex].Parent = null;
                        if (e.Object.Parent != null) e.Object.Parent.Elements.Remove(e.Object);
                        e.Object.Parent = this;
                    }
                    break;
                case ListChangedType.ItemAdded:
                    if (e.Object.Parent == this) e.Cancel = true;
                    else
                    {
                        if (e.Object.Parent != null) e.Object.Parent.Elements.Remove(e.Object);
                        e.Object.Parent = this;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Elements[e.NewIndex].Parent = null;
                    break;
                case ListChangedType.Reset:
                    foreach (var element in Elements) element.Parent = null;
                    break;
            }
        }

        public void Move(int layer, float timeDiff)
        {
            float startTime, endTime;
            if (timeDiff < 0)
            {
                startTime = _StartTime + timeDiff;
                if (startTime < 0)
                {
                    timeDiff = -_StartTime;
                    startTime = 0;
                }
                endTime = _EndTime + timeDiff;
            }
            else if (timeDiff > 0)
            {
                endTime = _EndTime + timeDiff;
                if (endTime > MaxDuration)
                {
                    timeDiff = MaxDuration - _EndTime;
                    endTime = MaxDuration;
                }
                startTime = _StartTime + timeDiff;
            }
            else
            {
                startTime = _StartTime;
                endTime = _EndTime;
            }
            if (Parent.IsCollided(this, layer, startTime, endTime, out var collision))
            {
                if (layer != _Layer) return;

                if (timeDiff < 0) {
                    startTime = collision.EndTime;
                    endTime = startTime + (_EndTime - _StartTime);
                }
                else if (timeDiff > 0)
                {
                    endTime = collision.StartTime;
                    startTime = endTime - (_EndTime - _StartTime);
                }
                if (startTime < 0 || endTime > MaxDuration || Parent.IsCollided(this, layer, startTime, endTime, out _)) return;
            }
            _moving = true;
            Layer = layer;
            StartTime = startTime;
            EndTime = endTime;
            _moving = false;
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("timeline");
            writer.WriteAttribute("startTime", _StartTime);
            writer.WriteAttribute("endTime", _EndTime);
            writer.WriteAttribute("layer", _Layer);
            writer.WriteAttribute("reset", _Reset);
            foreach (var element in Elements) element.Save(writer);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.Write(_StartTime);
            writer.Write(_EndTime);
            writer.Write((byte)_Layer);
            writer.Write(_Reset);
            writer.WriteLength(Elements.Count);
            foreach (var element in Elements) element.Build(writer);
        }
    }
}
