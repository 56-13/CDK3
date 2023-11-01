using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Terrain;

namespace CDK.Assets.Spawn
{
    public class SpawnObject : SceneObject
    {
        public override string Name
        {
            set => base.Name = value; 
            get => _Name ?? _Asset?.TagName ?? Type.ToString();
        }

        private SpawnAsset _Asset;
        [Binding]
        public SpawnAsset Asset
        {
            set
            {
                var prev = _Asset;
                if (SetProperty(ref _Asset, value))
                {
                    if (prev != null)
                    {
                        prev.RemoveWeakPropertyChanged(Asset_PropertyChanged);
                        prev.RemoveWeakRefresh(Asset_Refresh);
                    }
                    if (_Asset != null)
                    {
                        Asset.AddWeakPropertyChanged(Asset_PropertyChanged);
                        Asset.AddWeakRefresh(Asset_Refresh);
                    }

                    Attribute = _Asset != null ? new SpawnAttribute(this, _Asset) : null;

                    ResetViewObject();
                }
            }
            get => _Asset;
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "TagName":
                    OnPropertyChanged("Name");
                    break;
                case "View":
                    ResetViewObject();
                    break;
            }
        }
        private void Asset_Refresh(object sender, EventArgs e) => OnRefresh();

        public SceneObject ViewObject { private set; get; }
        public SpawnAttribute Attribute { private set; get; }

        private bool _collision;

        public SpawnObject(SpawnAsset asset = null)
        {
            if (asset != null) using (new AssetCommandHolder()) Asset = asset;
        }

        public SpawnObject(SpawnObject other, bool binding) : base(other, binding, true)
        {
            if (other._Asset != null) 
            { 
                AssetManager.Instance.InvokeRedirection(() =>
                {
                    using (new AssetCommandHolder()) Asset = other._Asset;
                });
            }
        }

        private void ResetViewObject()
        {
            var selected = Scene?.SelectedComponent == this;

            if (selected) ViewObject?.Select(false);

            if (GetBindingSource(out SpawnObject bindingSource)) ViewObject = (SceneObject)bindingSource.ViewObject?.Clone(true);
            else ViewObject = (SceneObject)Asset.View?.NewSceneComponent();

            if (ViewObject != null)
            {
                ViewObject.Parent = this;

                if (selected) ViewObject.Select(true);
            }
            OnPropertyChanged("ViewObject");

            AddUpdateFlags(UpdateFlags.Transform | UpdateFlags.AABB);
        }

        public bool CheckCollision()
        {
            var scene = Scene;

            if (scene?.World == null) return false;

            if (!GetTransform(out var t0)) return false;

            var p0 = t0.Translation.ToVector2();

            if (scene.World is TerrainComponent terrain && terrain.Asset != null)
            {
                var gminx = Math.Max((int)((p0.X - Asset.Collider) / terrain.Grid), 0);
                var gmaxx = Math.Min((int)((p0.X + Asset.Collider) / terrain.Grid), terrain.Asset.Width - 1);
                var gminy = Math.Max((int)((p0.Y - Asset.Collider) / terrain.Grid), 0);
                var gmaxy = Math.Min((int)((p0.Y + Asset.Collider) / terrain.Grid), terrain.Asset.Height - 1);

                for (var gx = gminx; gx <= gmaxx; gx++)
                {
                    for (var gy = gminy; gy <= gmaxy; gy++)
                    {
                        foreach (var tile in Asset.CollisionTiles)
                        {
                            if (tile == terrain.Asset.GetTile(gx, gy)) return true;
                        }
                    }
                }
            }

            var distance = Asset.Collider + Asset.CollisionTargets.Max((SpawnCollisionTarget target) => target.Distance);
            var box = new ABoundingBox(new Vector3(p0.X - distance, p0.Y - distance, scene.World.Space.Minimum.Z), new Vector3(p0.X + distance, p0.Y + distance, scene.World.Space.Maximum.Z));

            foreach(var obj in scene.Select(box))
            {
                if (obj != this && obj is SpawnObject spobj && spobj.GetTransform(out var t1))
                {
                    var p1 = t1.Translation.ToVector2();

                    if (spobj.CheckCollision(this, p0, p1, scene.World.Grid) || CheckCollision(spobj, p1, p0, scene.World.Grid)) return true;
                }
            }

            return false;
        }

        private bool CheckCollision(SpawnObject from, in Vector2 p0, in Vector2 p1, int grid)
        {
            var target = Asset.CollisionTargets.FirstOrDefault(e => e.Source == from.Asset.CollisionSource);
            if (target != null)
            {
                var d = (target.Distance + Asset.Collider + from.Asset.Collider) * grid;

                if (target.Square)
                {
                    if (Math.Abs(p0.X - p1.X) <= d && Math.Abs(p0.Y - p1.Y) <= d) return true;
                }
                else if (Vector2.DistanceSquared(p0, p1) <= d * d) return true;
            }
            return false;
        }

        public override SceneComponentType Type => SceneComponentType.Spawn;
        public override SceneComponent Clone(bool binding) => new SpawnObject(this, binding);
        protected override void OnLink() => ViewObject?.Link(Scene);
        protected override void OnUnlink() => ViewObject?.Unlink();
        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (name != null)
            {
                if (ViewObject != null) return ViewObject.GetTransform(progress, name, out result);
                else
                {
                    result = Matrix4x4.Identity;
                    return false;
                }
            }
            return base.GetTransform(progress, name, out result);
        }
        public override void GetTransformNames(ICollection<string> names) => ViewObject?.GetTransformNames(names);
        internal override bool AddAABB(ref ABoundingBox result) => ViewObject?.AddAABB(ref result) ?? false;
        internal override void Rewind() => ViewObject?.Rewind();
        protected override void OnUpdatePass(ref UpdatePass pass) => ViewObject?.GetUpdatePass(ref pass);
        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            var result = ViewObject?.Update(lightSpace, delta, alive, ref flags) ?? UpdateState.None;
            _collision = CheckCollision();
            return result;
        }
        internal override ShowFlags Show() => ViewObject?.Show() ?? ShowFlags.None;
        internal override bool CursorAlwaysVisible => true;
        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (layer == InstanceLayer.Cursor)
            {
                if (ViewObject != null && (ViewObject.CursorAlwaysVisible || Scene.SelectedComponent == this)) ViewObject?.Draw(graphics, layer);

                if (Scene.World != null)
                {
                    var r = Asset.Collider * Scene.World.Grid;

                    if (r > 0 && GetTransform(out var t))
                    {
                        var p = t.Translation;

                        graphics.Push();
                        graphics.Material.Shader = MaterialShader.NoLight;
                        graphics.Material.BlendMode = BlendMode.Alpha;
                        graphics.Material.DepthTest = false;
                        graphics.Material.Color = new Color4(_collision ? Color3.Red : Color3.Yellow, 0.5f);
                        graphics.DrawCircle(new ZRectangle(p.X - r, p.Y - r, p.Z, r, r), false);
                        graphics.Pop();
                    }
                }
            }
            else ViewObject?.Draw(graphics, layer);
        }

        internal override void Select(bool focus)
        {
            base.Select(focus);
            ViewObject?.Select(focus);
        }

        internal override bool KeyDown(KeyEventArgs e) => (ViewObject != null && ViewObject.KeyDown(e)) || base.KeyDown(e);
        internal override bool KeyUp(KeyEventArgs e) => (ViewObject != null && ViewObject.KeyUp(e)) || base.KeyUp(e);
        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey) => (ViewObject != null && ViewObject.MouseDown(e, controlKey, shiftKey)) || base.MouseDown(e, controlKey, shiftKey);
        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey) => (ViewObject != null && ViewObject.MouseUp(e, controlKey, shiftKey)) || base.MouseUp(e, controlKey, shiftKey);
        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey) => (ViewObject != null && ViewObject.MouseMove(e, prevX, prevY, controlKey, shiftKey)) || base.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        internal override bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey) => (ViewObject != null && ViewObject.MouseWheel(e, controlKey, shiftKey)) || base.MouseWheel(e, controlKey, shiftKey);
        protected override void SaveContent(XmlWriter writer)
        {
            base.SaveContent(writer);
            writer.WriteAttribute("asset", _Asset);
        }

        protected override void SaveChildren(XmlWriter writer)
        {
            writer.WriteStartElement("view");
            ViewObject?.Save(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("attributes");
            Attribute.Save(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("children");
            base.SaveChildren(writer);
            writer.WriteEndElement();
        }

        protected override void LoadContent(XmlNode node)
        {
            base.LoadContent(node);
            Asset = (SpawnAsset)node.ReadAttributeAsset("asset");
        }

        protected override void LoadChildren(XmlNode node)
        {
            ViewObject?.Load(node.ChildNodes[0].ChildNodes[0]);
            Attribute.Load(node.ChildNodes[1].ChildNodes[0]);
            base.LoadChildren(node.ChildNodes[2]);
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            if (CheckCollision()) throw new AssetException(Owner, $"{GetLocation()}가 다른 오브젝트와 충돌합니다.");

            BuildReference(writer, _Asset);
            //writer.Write(ViewObject != null);
            //if (ViewObject != null) ViewObject.Build(writer, param);           //TODO:ASSET REF은 포함하지 않아야 한다.
            Attribute.Build(writer);
        }
    }
}
