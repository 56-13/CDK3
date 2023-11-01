using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Animations;

namespace CDK.Assets.Scenes
{
    [Flags]
    public enum ShowFlags
    {
        None = 0,
        Distortion = 1
    }

    [Flags]
    public enum UpdateFlags
    {
        None = 0,
        Transform = 1,
        AABB = 2,
        View = 4
    }

    public enum UpdateState
    {
        None,
        Stopped,
        Alive,
        Finishing
    }

    public enum DurationParam
    {
        Min,
        Max,
        Avg
    }

    public class SceneObject : SceneComponent
    {
        public string Key { protected set; get; }
        
        private bool _Located;
        [Binding]
        public bool Located
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled && Parent is AnimationObjectFragment) value = false;

                if (SetProperty(ref _Located, value))
                {
                    if (Scene != null)
                    {
                        if (_Located) Locate();
                        else Unlocate();
                    }
                }
            }
            get => _Located;
        }

        private bool _UsingTransform;
        [Binding]
        public bool UsingTransform
        {
            set
            {
                if (AssetManager.Instance.CommandEnabled && Parent is AnimationObjectFragment) value = false;

                if (SetProperty(ref _UsingTransform, value)) _updateFlags |= UpdateFlags.Transform;
            }
            get => _UsingTransform;
        }

        public Gizmo Transform { private set; get; }

        private UpdateFlags _updateFlags;
        private ABoundingBox _aabb;
        private bool _aabbFlag;

        public SceneObject()
        {
            Key = AssetManager.NewKey();

            Transform = new Gizmo(this);

            PropertyChanged += SceneObject_PropertyChanged;
        }

        public SceneObject(SceneObject other, bool binding, bool children) : base(other, binding, children)
        {
            AssetManager.Instance.AddRedirection(other, this);

            Key = other.Key;
            _Located = other._Located;
            _UsingTransform = other._UsingTransform;
            Transform = new Gizmo(this, other.Transform, binding);
            _updateFlags = other._updateFlags;
            _aabb = other._aabb;
            _aabbFlag = other._aabbFlag;
            TransformUpdated = other.TransformUpdated;

            PropertyChanged += SceneObject_PropertyChanged;
        }

        private void SceneObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent")
            {
                Located = _Located;
                UsingTransform = _UsingTransform;
                //reset located, usingTransform
            }
        }

        private void Locate()
        {
            _Located = false;       //prevent relocate
            Update(0);
            if ((_updateFlags & UpdateFlags.AABB) != 0)
            {
                var aabb = ABoundingBox.None;
                if ((_aabbFlag = AddAABB(ref aabb))) _aabb = aabb;
                _updateFlags &= ~UpdateFlags.AABB;
            }
            _Located = true;
            Scene.Locate(this);
            OnLocate();
        }

        private void Unlocate()
        {
            Scene.Unlocate(this);
            OnUnlocate();
        }

        protected override void OnLink()
        {
            if (Scene.GetObject(Key) != null) Key = AssetManager.NewKey();
            
            Scene.LinkObject(this);

            if (_Located)
            {
                Locate();

                AssetManager.Instance.Invoke(() => OnPropertyChanged("Located"));
            }
        }

        protected override void OnUnlink()
        {
            if (_Located)
            {
                Unlocate();
                
                AssetManager.Instance.Invoke(() => OnPropertyChanged("Located"));
            }
            Scene.UnlinkObject(this);
        }

        protected virtual void OnLocate() { }
        protected virtual void OnUnlocate() { }
        protected virtual UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags) => UpdateState.None;
        protected virtual void OnUpdatePass(ref UpdatePass pass) { }
        protected virtual void OnDraw(Graphics graphics, InstanceLayer layer) { }

        public override SceneComponentType Type => SceneComponentType.Object;
        public override SceneComponent Clone(bool binding) => new SceneObject(this, binding, true);
        public virtual bool AllowDrag => false;
        public override SceneComponentType[] SubTypes
        {
            get
            {
                return new SceneComponentType[] {
                    SceneComponentType.Object,
                    SceneComponentType.Box,
                    SceneComponentType.Sphere,
                    SceneComponentType.Capsule,
                    SceneComponentType.Mesh,
                    SceneComponentType.Image,
                    SceneComponentType.Particle,
                    SceneComponentType.Trail,
                    SceneComponentType.Sprite,
                    SceneComponentType.DirectionalLight,
                    SceneComponentType.PointLight,
                    SceneComponentType.SpotLight,
                    SceneComponentType.Animation,
                    SceneComponentType.AnimationReference,
                    SceneComponentType.Camera,
                    SceneComponentType.Spawn
                };
            }
        }

        public virtual bool GetTransform(float progress, string name, out Matrix4x4 result) 
        {
            if (name == null)
            {
                if (Parent is AnimationObjectFragment animation) return animation.GetTransform(progress + animation.Progress - Progress, null, out result);
                if (_UsingTransform) return Transform.GetTransform(progress, out result);
                if (Parent is SceneObject parent) return parent.GetTransform(progress + parent.Progress - Progress, null, out result);
            }
            result = Matrix4x4.Identity;
            return false;
        }

        public bool GetTransform(string name, out Matrix4x4 result) => GetTransform(Progress, name, out result);
        public bool GetTransform(out Matrix4x4 result) => GetTransform(Progress, null, out result);
        public virtual void GetTransformNames(ICollection<string> names) { }
        public bool TransformUpdated { private set; get; }
        public bool FromGround
        {
            get
            {
                if (Parent is AnimationObjectFragment animation) return animation.Root.FromGround;
                if (_UsingTransform && Transform.FromGround) return true;
                return Parent is SceneObject parent && parent.FromGround;
            }
        }

        public bool GetAABB(out ABoundingBox result)
        {
            result = _aabb;
            return _aabbFlag;
        }
        internal virtual bool AddAABB(ref ABoundingBox result) => false;

        public Collider GetCollider()
        {
            Collider result = null;
            AddCollider(ref result);
            return result;
        }
        internal virtual void AddCollider(ref Collider result) { }
        internal void AddUpdateFlags(UpdateFlags flags) => _updateFlags |= flags;
        public virtual float Progress => 0f;
        public virtual float GetDuration(DurationParam param, float duration = 0) => 0f;
        internal virtual bool CameraCapture(ref Camera camera)
        {
            foreach(var child in Children)
            {
                if (child is SceneObject obj && obj.CameraCapture(ref camera)) return true;
            }
            return false;
        }
        internal virtual bool CameraFilter(Graphics graphics)
        {
            foreach (var child in Children)
            {
                if (child is SceneObject obj && obj.CameraFilter(graphics)) return true;
            }
            return false;
        }
        internal virtual bool AfterCameraUpdate() => Parent is SceneObject parent && parent.AfterCameraUpdate();
        internal bool GetUpdatePass(ref UpdatePass pass)
        {
            if (!pass.Remaining) return false;
            OnUpdatePass(ref pass);
            if (!pass.Remaining) return false;
            if (Parent is SceneObject parent && !pass.AddPrecedence(parent)) return false;
            var camera = Scene.CameraObject;
            if (camera != null && AfterCameraUpdate() && !pass.AddPrecedence(camera)) return false;
            if (_UsingTransform && !Transform.GetUpdatePass(ref pass)) return false;
            return true;
        }
        internal int GetUpdatePass()
        {
            var pass = new UpdatePass();
            GetUpdatePass(ref pass);
            return pass;
        }

        internal virtual void Rewind() { }

        internal UpdateState Update(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            if (_UsingTransform)
            {
                if (Transform.Update()) flags |= UpdateFlags.Transform;
            }
            else if (Parent is SceneObject parent)
            {
                if (parent.TransformUpdated) flags |= UpdateFlags.Transform;
            }

            flags |= _updateFlags;
            _updateFlags = 0;

            var result = OnUpdate(lightSpace, delta, alive, ref flags);

            if ((flags & UpdateFlags.AABB) != 0)
            {
                if (_Located && Scene != null)
                {
                    var aabb = ABoundingBox.None;
                    var aabbFlag = AddAABB(ref aabb);
                    if (aabb != _aabb || aabbFlag != _aabbFlag)
                    {
                        if (aabbFlag) Scene.Relocate(this, aabb);
                        else Scene.Unlocate(this);
                        _aabb = aabb;
                        _aabbFlag = aabbFlag;
                    }
                }
                else _updateFlags = UpdateFlags.AABB;
            }

            TransformUpdated = (flags & UpdateFlags.Transform) != 0;

            return result;
        }

        internal bool Update(float delta)
        {
            var outflags = UpdateFlags.None;
            return Update(null, delta, true, ref outflags) != UpdateState.Stopped;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer) 
        {
            if (_UsingTransform)
            {
                switch (layer)
                {
                    case InstanceLayer.Base:
                        if (Transform.GetTransform(Progress, out var transform))
                        {
                            var cursor = VertexArrays.GetSphere(0, Vector3.Zero, 8, Rectangle.ZeroToOne, out var aabb);

                            graphics.Push();
                            graphics.Translate(transform.Translation);
                            graphics.DrawVertices(cursor, PrimitiveMode.Triangles, aabb);
                            graphics.Pop();
                        }
                        break;
                    case InstanceLayer.Cursor:
                        Transform.Draw(graphics);
                        break;
                }
            }
        }

        public virtual string GetStatus() => null;

        internal override void Select(bool focus) 
        {
            if (_UsingTransform) Transform.Select(focus);
        }
        
        internal virtual void Offset(in Vector3 offset, bool retrieving)
        {
            if (retrieving && _UsingTransform && Transform.TargetKey == null)
            {
                Transform.Position += offset;
                retrieving = false;
            }
            foreach (var child in Children) OffsetChildren(child, offset, retrieving);
        }

        private void OffsetChildren(SceneComponent comp, in Vector3 offset, bool retrieving)
        {
            if (comp is SceneObject obj) obj.Offset(offset, retrieving);
            foreach (var child in Children) OffsetChildren(child, offset, retrieving);
        }

        internal override bool KeyDown(KeyEventArgs e) => (_UsingTransform && Transform.KeyDown(e)) || base.KeyDown(e);
        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey) => (_UsingTransform && Transform.MouseDown(e, controlKey, shiftKey)) || base.MouseDown(e, controlKey, shiftKey);
        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey) => (_UsingTransform && Transform.MouseUp(e, controlKey, shiftKey)) || base.MouseUp(e, controlKey, shiftKey);
        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey) => (_UsingTransform && Transform.MouseMove(e, prevX, prevY, controlKey, shiftKey)) || base.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var obj in Children) obj.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_UsingTransform && Transform.IsRetaining(element, out from)) return true;

            foreach (var obj in Children)
            {
                if (obj.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        protected override void SaveContent(XmlWriter writer) 
        {
            writer.WriteAttribute("key", Key);
            writer.WriteAttribute("located", _Located);
            if (_UsingTransform) Transform.Save(writer, "transform");
        }

        protected override void LoadContent(XmlNode node)
        {
            Key = node.ReadAttributeString("key", Key);
            Located = node.ReadAttributeBool("located");
            UsingTransform = node.HasAttribute("transform");
            if (_UsingTransform) Transform.Load(node, "transform");
        }

        internal void Build(BinaryWriter writer, SceneBuildParam param)
        {
            writer.Write((byte)Type);
            writer.Write(param.KeyToId(Key));
            writer.Write(_Located);
            writer.Write(_UsingTransform);
            if (_UsingTransform) Transform.Build(writer, param);
            BuildChildren(writer, param);
            BuildContent(writer, param);
        }
        protected virtual void BuildContent(BinaryWriter writer, SceneBuildParam param) { }
        protected virtual void BuildChildren(BinaryWriter writer, SceneBuildParam param)
        {
            writer.WriteLength(Children.Count());
            foreach (SceneObject child in Children) child.Build(writer, param);
        }
    }
}
