using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Meshing
{
    public class MeshObject : SceneObject, ISceneAnimation
    {
        public MeshSelection Selection { private set; get; }
        public MeshAsset Asset { private set; get; }
        public bool AssetEdit { private set; get; }
        public override string Name
        {
            set => base.Name = value;
            get => _Name ?? Asset?.TagName ?? Type.ToString();
        }

        private Instance _instance;
        private float _progress;
        private int _random;

        public MeshObject(MeshAsset asset = null, bool assetEdit = false)
        {
            AssetEdit = assetEdit && asset != null;

            Selection = assetEdit ? asset.Selection : new MeshSelection(this, asset);

            Init();
        }

        public MeshObject(MeshSelection selection)
        {
            Selection = selection;

            Init();
        }

        public MeshObject(MeshObject other, bool binding) : base(other, binding, !other.AssetEdit)
        {
            Selection = binding ? other.Selection : new MeshSelection(this, other.Selection);

            Init();
        }

        private void Init()
        {
            Selection.AddWeakPropertyChanged(Selection_PropertyChanged);

            AssetManager.Instance.InvokeRedirection(() =>
            {
                Asset = AssetManager.Instance.GetRedirection(Selection.Asset);
                Asset?.AddWeakPropertyChanged(Asset_PropertyChanged);

                if (Selection.UpdateInstance(ref _instance)) AddUpdateFlags(UpdateFlags.Transform | UpdateFlags.AABB);

                if (AssetEdit)
                {
                    Children.Add(new MeshGeometriesComponent(Asset));
                    Children.Add(new MeshAnimationsComponent(Asset));
                }
            });

            _random = RandomUtil.Next();
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TagName") OnPropertyChanged("Name");
        }

        private void Selection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Asset")
            {
                Asset?.RemoveWeakPropertyChanged(Asset_PropertyChanged);
                Asset = Selection.Asset;
                Asset?.AddWeakPropertyChanged(Asset_PropertyChanged);

                OnPropertyChanged("Name");
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            Selection.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Selection.IsRetaining(element, out from)) return true;

            return Transform.IsRetaining(element, out from);
        }

        public override SceneComponentType Type => SceneComponentType.Mesh;
        public override SceneComponent Clone(bool binding) => new MeshObject(this, binding);
        public override SceneComponentType[] SubTypes => AssetEdit ? new SceneComponentType[0] : base.SubTypes;
        public override bool AddSubEnabled(SceneComponent obj)
        {
            if (AssetEdit)
            {
                switch (obj.Type)
                {
                    case SceneComponentType.MeshGeometries:
                        return ((MeshGeometriesComponent)obj).Asset == Asset;
                    case SceneComponentType.MeshAnimations:
                        return ((MeshAnimationsComponent)obj).Asset == Asset;
                }
                return false;
            }
            return base.AddSubEnabled(obj);
        }
        public override void AddSub(SceneComponentType type) 
        { 
            if (!AssetEdit) base.AddSub(type);
        }

        public override string ImportFilter => AssetEdit ? FileFilters.Mesh : null;
        public override void Import(string path)
        {
            if (AssetEdit) Asset.Import(path, true, true);
        }

        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (!base.GetTransform(progress, null, out result)) return false;

            if (name != null)
            {
                if (_instance != null && _instance.GetNodeTransform(name, out var nodeTransform)) result = nodeTransform * result;
                else return false;
            }
            return true;
        }
        public override void GetTransformNames(ICollection<string> names) => Selection.Geometry?.Origin.GetBoneNames(names);
        public override float Progress => _instance?.Progress ?? 0f;
        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (_instance != null && GetTransform(out var transform) && _instance.GetAABB(out var aabb))
            {
                aabb.Transform(transform);
                result.Append(aabb);
                return true;
            }
            return false;
        }
        internal override void AddCollider(ref Collider result)
        {
            if (_instance != null && Selection.Geometry != null && Selection.Collision && GetTransform(out var transform)) Selection.Geometry.AddCollider(_instance, transform, ref result);
        }

        internal override void Rewind()
        {
            _progress = 0;
            _random = RandomUtil.Next();
            _instance?.Rewind();
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            _progress += delta;

            if (Selection.UpdateInstance(ref _instance) || (flags & UpdateFlags.Transform) != 0) flags |= UpdateFlags.Transform | UpdateFlags.AABB;

            if (_instance == null || _instance.Animation == null) return UpdateState.None;
            
            var prev = _instance.Progress;
            var next = _instance.Progress = Selection.Loop.GetProgress(_progress / _instance.Animation.Duration) * _instance.Animation.Duration;
            if (prev != next) flags |= UpdateFlags.Transform | UpdateFlags.AABB;
            if (Selection.Loop.Count == 0) return UpdateState.None;
            else if (_progress < _instance.Animation.Duration * Selection.Loop.Count) return UpdateState.Alive;
            else return Selection.Loop.Finish ? UpdateState.Stopped : UpdateState.None;
        }

        internal override ShowFlags Show() => Selection.ShowFlags;

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == InstanceLayer.Cursor && Transform.Draw(graphics)) return;

            if (_instance != null && GetTransform(out var transform))
            {
                var world = graphics.World;
                graphics.World = transform * world;
                _instance.Draw(graphics, layer, _progress, _random, null);
                graphics.World = world;
            }
        }

        internal void DrawCollider(Graphics graphics, MeshCollider selection)
        {
            if (Selection.Geometry != null && _instance != null && GetTransform(out var transform))
            {
                graphics.Push();
                graphics.Material.Shader = MaterialShader.NoLight;
                graphics.Transform(transform);

                foreach (var collider in Selection.Geometry.Colliders)
                {
                    graphics.Color = selection == collider ? Color4.White : Color4.TranslucentWhite;

                    collider.Draw(graphics, _instance);
                }
                if (selection != null && selection.NodeName != null)
                {
                    var node = Selection.Geometry.Origin.FindNode(selection.NodeName);

                    if (node != null)
                    {
                        graphics.DepthMode = DepthMode.None;
                        graphics.Color = Color4.Green;
                        Vector3? prev = null;
                        DrawBoneCurrent(graphics, node, ref prev);
                        graphics.Color = Color4.DarkGreen;
                        foreach (var subnode in node.Children) DrawBone(graphics, subnode, prev);
                    }
                }
                graphics.Pop();
            }
        }

        private void DrawBoneCurrent(Graphics graphics, Node node, ref Vector3? prev)
        {
            if (Selection.Geometry.Origin.HasBone(node.Name))
            {
                var transform = _instance.GetNodeTransform(node);

                var position = transform.Translation;

                graphics.Material.Shader = MaterialShader.Light;

                var world = graphics.World;
                graphics.World = transform * world;
                graphics.DrawBox(-Vector3.One, Vector3.One);
                graphics.World = world;

                if (prev != null)
                {
                    graphics.Material.Shader = MaterialShader.NoLight;
                    graphics.DrawLine(prev.Value, position);
                }

                prev = transform.Translation;
            }
        }

        private void DrawBone(Graphics graphics, Node node, Vector3? prev)
        {
            DrawBoneCurrent(graphics, node, ref prev);

            foreach (var subnode in node.Children) DrawBone(graphics, subnode, prev);
        }

        protected override void SaveChildren(XmlWriter writer)
        {
            Selection.Save(writer);
            if (!AssetEdit)
            {
                writer.WriteStartElement("children");
                base.SaveChildren(writer);
                writer.WriteEndElement();
            }
        }

        protected override void LoadChildren(XmlNode node)
        {
            Selection.Load(node.ChildNodes[0]);
            if (!AssetEdit) base.LoadChildren(node.ChildNodes[1]);
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Selection.Build(writer);
        }

        protected override void BuildChildren(BinaryWriter writer, SceneBuildParam param)
        {
            if (!AssetEdit) base.BuildChildren(writer, param);
            else writer.WriteLength(0);
        }

        float ISceneAnimation.Progress => _instance?.Progress ?? 0f;
        float ISceneAnimation.GetDuration(DurationParam param) => Selection.Animation?.Duration ?? 0f;
    }
}
