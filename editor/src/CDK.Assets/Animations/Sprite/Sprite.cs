using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;

namespace CDK.Assets.Animations.Sprite
{
    public class Sprite : AssetElement, IAnimationSubstance
    {
        public AssetElement Parent { private set; get; }
        public override AssetElement GetParent() => Parent;

        public AssetElementList<SpriteTimeline> Timelines { private set; get; }

        private AnimationLoop _Loop;
        public AnimationLoop Loop
        {
            set => SetProperty(ref _Loop, value);
            get => _Loop;
        }

        private bool _Billboard;
        public bool Billboard
        {
            set => SetProperty(ref _Billboard, value);
            get => _Billboard;
        }

        public Sprite(AssetElement parent)
        {
            Parent = parent;

            Timelines = new AssetElementList<SpriteTimeline>(this);
            Timelines.BeforeListChanged += Timelines_BeforeListChanged;
        }

        public Sprite(AssetElement parent, Sprite other)
        {
            Parent = parent;

            Timelines = new AssetElementList<SpriteTimeline>(this);
            Timelines.BeforeListChanged += Timelines_BeforeListChanged;

            using (new AssetCommandHolder())
            {
                foreach (var timeline in other.Timelines) Timelines.Add(new SpriteTimeline(timeline));
            }

            _Loop = other._Loop;
            _Billboard = other._Billboard;
        }

        private void Timelines_BeforeListChanged(object sender, BeforeListChangedEventArgs<SpriteTimeline> e)
        {
            switch(e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.Object.Parent == this || IsCollided(Timelines[e.NewIndex], e.Object, out _)) e.Cancel = true;
                    else
                    {
                        Timelines[e.NewIndex].Parent = null;
                        if (e.Object.Parent != null) e.Object.Parent.Timelines.Remove(e.Object);
                        e.Object.Parent = this;
                    }
                    break;
                case ListChangedType.ItemAdded:
                    if (e.Object.Parent == this || IsCollided(e.Object, out _)) e.Cancel = true;
                    else
                    {
                        if (e.Object.Parent != null) e.Object.Parent.Timelines.Remove(e.Object);
                        e.Object.Parent = this;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Timelines[e.NewIndex].Parent = null;
                    break;
                case ListChangedType.Reset:
                    foreach (var timeline in Timelines) timeline.Parent = null;
                    break;
            }
        }

        public float SingleDuration => Timelines.Max(timeline => (float?)timeline.EndTime) ?? 0;
        public float TotalDuration => _Loop.Count != 0 ? SingleDuration * _Loop.Count : 0;
        public bool IsCollided(SpriteTimeline origin, int layer, float startTime, float endTime, out SpriteTimeline collision)
        {
            collision = Timelines.FirstOrDefault(t => t != origin && t.Layer == layer && Math.Max(t.StartTime, startTime) < Math.Min(t.EndTime, endTime));
            return collision != null;
        }

        public bool IsCollided(SpriteTimeline origin, SpriteTimeline newTimeline, out SpriteTimeline collision)
        {
            return IsCollided(origin, newTimeline.Layer, newTimeline.StartTime, newTimeline.EndTime, out collision);
        }

        public bool IsCollided(SpriteTimeline newTimeline, out SpriteTimeline collision)
        {
            return IsCollided(null, newTimeline, out collision);
        }

        internal void GetTransformNames(ICollection<string> names)
        {
            foreach (var timeline in Timelines)
            {
                foreach (var e in timeline.Elements) e.GetTransformNames(names);
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var timeline in Timelines)
            {
                foreach (var e in timeline.Elements) e.AddRetains(retains);
            }
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var timeline in Timelines)
            {
                foreach (var e in timeline.Elements)
                {
                    if (e.IsRetaining(element, out from)) return true;
                }
            }
            from = null;
            return false;
        }

        internal void Load(XmlNode node)
        {
            Billboard = node.ReadAttributeBool("billboard");
            Loop = new AnimationLoop(node, "loop");

            Timelines.Clear();
            foreach (XmlNode subnode in node.ChildNodes) Timelines.Add(new SpriteTimeline(subnode));
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("sprite");
            writer.WriteAttribute("billboard", _Billboard);
            _Loop.Save(writer, "loop");
            foreach (var timeline in Timelines) timeline.Save(writer);
            writer.WriteEndElement();
        }

        internal void Build(BinaryWriter writer)
        {
            writer.WriteLength(Timelines.Count);
            foreach (var timeline in Timelines) timeline.Build(writer);
            writer.Write(_Billboard);
            _Loop.Build(writer);
        }

        internal void GetLocaleStrings(ICollection<LocaleString> strings)
        {
            foreach (var timeline in Timelines)
            {
                foreach (var e in timeline.Elements) e.GetLocaleStrings(strings);
            }
        }

        SceneComponentType IAnimationSubstance.Type => SceneComponentType.Sprite;
        SceneObject IAnimationSubstance.CreateObject(AnimationObjectFragment parent) => new SpriteObject(this) { Parent = parent };
        IAnimationSubstance IAnimationSubstance.Clone(AnimationFragment parent) => new Sprite(parent, this);
        void IAnimationSubstance.GetTransformNames(ICollection<string> names) => GetTransformNames(names);
        void IAnimationSubstance.AddRetains(ICollection<string> retains) => AddRetains(retains);
        bool IAnimationSubstance.IsRetaining(AssetElement element, out AssetElement from) => IsRetaining(element, out from);
        void IAnimationSubstance.Save(XmlWriter writer) => Save(writer);
        void IAnimationSubstance.Load(XmlNode node) => Load(node);
        void IAnimationSubstance.Build(BinaryWriter writer) => Build(writer);
        void IAnimationSubstance.GetLocaleStrings(ICollection<LocaleString> strings) => GetLocaleStrings(strings);
    }
}
