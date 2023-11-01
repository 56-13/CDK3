using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public enum SceneMode
    {
        Preview,
        Edit
    }

    public class Scene : SceneContainer
    {
        public Asset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public override bool IsDirty 
        {
            set
            {
                if (!Seperated) base.IsDirty = value;
            }
            get => !Seperated && base.IsDirty; 
        }
        public bool Seperated { internal set; get; }

        private SceneConfig _Config;
        public SceneConfig Config
        {
            internal set
            {
                if (SetProperty(ref _Config, value, false)) ResetConfig();
            }
            get => _Config ?? Parent.Project.SceneConfig ?? AssetManager.Instance.Config.Scene;
        }

        public Environment Environment { private set; get; }
        public World World { private set; get; }

        private SceneComponent _SelectedComponent;
        public SceneComponent SelectedComponent
        {
            set
            {
                if (value != null && value.Scene != this) throw new InvalidOperationException();

                var prev = _SelectedComponent;
                if (SetProperty(ref _SelectedComponent, value, false))
                {
                    prev?.Select(false);
                    _SelectedComponent?.Select(true);

                    if (_SelectedComponent is ISceneAnimation animation) SelectedAnimation = animation;
                }
            }
            get => _SelectedComponent;
        }

        private bool _SelectedObjectOnly;
        public bool SelectedObjectOnly
        {
            set => SetProperty(ref _SelectedObjectOnly, value, false);
            get => _SelectedObjectOnly;
        }

        private ISceneAnimation _SelectedAnimation;
        public ISceneAnimation SelectedAnimation
        {
            set => SetProperty(ref _SelectedAnimation, value, false);
            get => _SelectedAnimation;
        }

        private Color4 _BackColor;
        public Color4 BackColor
        {
            set => SetProperty(ref _BackColor, value, false);
            get => _BackColor;
        }

        private SceneObject _CameraObject;
        public SceneObject CameraObject
        {
            set => SetProperty(ref _CameraObject, value, false);
            get => _CameraObject;
        }

        public ref Camera Camera => ref _camera;

        private bool _CameraGizmo;
        public bool CameraGizmo
        {
            set => SetProperty(ref _CameraGizmo, value, false);
            get => _CameraGizmo;
        }

        private SceneMode _Mode;
        public SceneMode Mode
        {
            set => SetProperty(ref _Mode, value, false);
            get => _Mode;
        }

        private float _Speed;
        public float Speed
        {
            set => SetProperty(ref _Speed, value, false);
            get => _Speed;
        }

        private bool _SoundEnabled;
        public bool SoundEnabled
        {
            set => SetProperty(ref _SoundEnabled, value, false);
            get => _SoundEnabled;
        }

        private class Sound
        {
            public string path;
		    public float volume;
            public AudioControl control;
            public int loop;
            public int priority;
            public float duplication;
            public int handle;
        };

        private Dictionary<string, SceneObject> _objects;
        private Camera _camera;
        private QuadTree _quadTree;
        private SceneObject _dragObject;
        private List<Sound> _sounds;
        private bool _mousePicked;
        private int _mouseX;
        private int _mouseY;

        public const float MouseWheelCameraDelta = 0.1f;

        public Scene(Asset parent)
        {
            Parent = parent;
            Parent.AddWeakPropertyChanged(Parent_PropertyChanged);

            var groundConfig = Config.Grounds[0];

            _objects = new Dictionary<string, SceneObject>();
            _quadTree = new QuadTree(groundConfig.Space, groundConfig.Grid);
            _sounds = new List<Sound>();
            _BackColor = Color4.Black;
            _CameraGizmo = true;
            _Mode = SceneMode.Edit;
            _Speed = 1f;
            _SoundEnabled = true;
            
            Children.ListChanged += Objects_ListChanged;

            ResetCamera();
        }

        public Scene(Asset parent, Scene other, bool binding) : base(other, binding, true)
        {
            Parent = parent;
            Parent.AddWeakPropertyChanged(Parent_PropertyChanged);

            _objects = new Dictionary<string, SceneObject>();
            _quadTree = new QuadTree(other._quadTree.Space, other._quadTree.Grid);
            _sounds = new List<Sound>();
            _BackColor = other._BackColor;
            _CameraGizmo = other._CameraGizmo;
            _Mode = other._Mode;
            _Speed = other._Speed;
            _SoundEnabled = other._SoundEnabled;

            if (other._Config != null) _Config = binding ? other._Config : new SceneConfig(this, other._Config);

            Children.ListChanged += Objects_ListChanged;

            foreach (var obj in Children)
            {
                if (obj is Environment environment) Environment = environment;
                else if (obj is World world) World = world;
            }
            ResetCamera();
        }

        private void ResetConfig()
        {
            var config = Config;

            using (new AssetCommandHolder())
            {
                if (Environment != null)
                {
                    Environment.PreferenceConfig = config.Preferences.FirstOrDefault(e => e.Name == Environment.PreferenceConfig.Name) ?? config.Preferences[0];
                    Environment.EnvironmentConfig = config.Environments.FirstOrDefault(e => e.Name == Environment.EnvironmentConfig.Name) ?? config.Environments[0];
                }
                if (World is Ground ground)
                {
                    ground.Config = config.Grounds.FirstOrDefault(e => e.Name == ground.Config.Name) ?? config.Grounds[0];
                }
            }
        }
                
        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_Config == null && e.PropertyName == "Project")
            {
                OnPropertyChanged("Config");

                ResetConfig();
            }
        }

        private void Objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null) return;
                    break;
                case ListChangedType.ItemMoved:
                    return;
            }
            foreach (var obj in Children)
            {
                if (obj is Environment environment) Environment = environment;
                else if (obj is World world) World = world;
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            _Config?.AddRetains(retains);

            base.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (_Config != null && _Config.IsRetaining(element, out from)) return true;

            return base.IsRetaining(element, out from);
        }

        public override SceneComponentType[] SubTypes
        {
            get
            {
                return new SceneComponentType[]
                {
                    SceneComponentType.Environment,
                    SceneComponentType.Ground,
                    SceneComponentType.Terrain,
                    SceneComponentType.Object,
                    SceneComponentType.Box,
                    SceneComponentType.Sphere,
                    SceneComponentType.Capsule,
                    SceneComponentType.Mesh,
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
        public override bool AddSubEnabled(SceneComponent obj)
        {
            switch (obj.Type)
            {
                case SceneComponentType.Environment:
                    if (Environment == null) return true;
                    break;
                case SceneComponentType.Ground:
                case SceneComponentType.Terrain:
                    if (World == null) return true;
                    break;
                case SceneComponentType.Object:
                case SceneComponentType.Box:
                case SceneComponentType.Sphere:
                case SceneComponentType.Capsule:
                case SceneComponentType.Mesh:
                case SceneComponentType.Image:
                case SceneComponentType.Particle:
                case SceneComponentType.Trail:
                case SceneComponentType.Sprite:
                case SceneComponentType.DirectionalLight:
                case SceneComponentType.PointLight:
                case SceneComponentType.SpotLight:
                case SceneComponentType.Animation:
                case SceneComponentType.AnimationReference:
                case SceneComponentType.Camera:
                case SceneComponentType.Spawn:
                case SceneComponentType.Skybox:
                    return true;
            }
            return false;
        }

        public SceneObject GetObject(string key)
        {
            return _objects.TryGetValue(key, out var obj) ? obj : null;
        }

        internal void LinkObject(SceneObject obj)
        {
            _objects.Add(obj.Key, obj);
        }

        internal void UnlinkObject(SceneObject obj)
        {
            _objects.Remove(obj.Key);
            if (_SelectedComponent == obj) SelectedComponent = null;
            if (_CameraObject == obj) CameraObject = null;
        }

        internal void Locate(SceneObject obj) => _quadTree.Locate(obj);
        internal void Unlocate(SceneObject obj) => _quadTree.Unlocate(obj);
        internal void Relocate(SceneObject obj, in ABoundingBox naabb) => _quadTree.Relocate(obj, naabb);
        public IEnumerable<SceneObject> Select(in Ray ray) => _quadTree.Select(ray);
        public IEnumerable<SceneObject> Select(in ABoundingBox box) => _quadTree.Select(box);
        public IEnumerable<SceneObject> Select(in BoundingSphere sphere) => _quadTree.Select(sphere);
        public IEnumerable<SceneObject> Select(in BoundingFrustum frustum) => _quadTree.Select(frustum);

        public bool Intersects(in Ray ray, SceneObject origin, bool pick, CollisionFlags flags, out SceneObject selection, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            selection = null;
            distance = float.MaxValue;

            var result = false;

            if (World != null && World.Intersects(ray, flags, out distance, out hit)) result = true;

            foreach (var obj in _quadTree.Select(ray))
            {
                if (obj != origin)
                {
                    if (pick && obj.AllowPick)
                    {
                        if (obj.GetAABB(out var aabb) && aabb.Intersects(ray, CollisionFlags.None, out var d, out _) && d < distance)
                        {
                            selection = obj;
                            distance = d;
                            hit = Hit.Zero;
                            result = true;
                        }
                    }
                    else
                    {
                        var collider = obj.GetCollider();
                        if (collider != null && collider.Intersects(ray, flags, ref distance, ref hit))
                        {
                            selection = obj.AllowPick ? obj : null;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        /*
        public CollisionResult Intersects(SceneObject origin, CollisionFlags flags, out SceneObject selection, out float distance, out Hit hit)
        {
            hit = Hit.Zero;

            selection = null;
            distance = float.MaxValue;

            var result = false;

            if (World != null && World.Intersects(ray, flags, out distance, out hit)) result = true;

            foreach (var obj in _quadTree.Select(ray))
            {
                if (obj != origin)
                {
                    if (pick && !obj.Referencing)
                    {
                        if (obj.GetAABB(out var aabb) && aabb.Intersects(ray, CollisionFlags.None, out var d, out _) && d < distance)
                        {
                            selection = obj;
                            distance = d;
                            hit = Hit.Zero;
                            result = true;
                        }
                    }
                    else
                    {
                        var collider = obj.GetCollider();
                        if (collider != null && collider.Intersects(ray, flags, ref distance, ref hit))
                        {
                            selection = obj.Referencing ? null : obj;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
        */
        public float GetZ(SceneObject origin, in Vector3 pos)       //TODO:물리엔진 적용시 다른 방식으로 처리
        {
            var z = World?.GetZ(pos) ?? 0f;
            foreach (var obj in _quadTree.Select(pos))
            {
                if (obj != origin) obj.GetCollider()?.GetZ(pos, ref z);
            }
            return z;
        }

        public void ResetCamera()
        {
            if (_CameraObject == null)
            {
                var env = Environment?.EnvironmentConfig ?? Config.Environments[0];

                Vector3 target;
                if (World != null)
                {
                    target = World.Space.Center;
                    target.Z = 0;
                }
                else target = Vector3.Zero;

                var rot = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -env.CameraAngle * MathUtil.ToRadians);
                var pos = new Vector3(0, 0, env.CameraDistance);
                pos = Vector3.Transform(pos, rot);
                pos += target;

                var up = -Vector3.UnitY;
                Vector3.Transform(up, rot);

                _camera.SetView(pos, target, up);
            }
        }

        public void FitCamera()
        {
            if (_CameraObject == null && _camera.Fov != 0 && _SelectedComponent is SceneObject obj && obj.GetAABB(out var aabb))
            {
                _camera.Move(aabb.Center - _camera.Target);

                var corners = aabb.GetCorners();
                var dir = 0;
                var scale = 10;
                var f = _camera.Forward;
                var d = Vector3.Distance(_camera.Position, _camera.Target);

                for (; ; )
                {
                    var outside = false;
                    foreach (var p in corners)
                    {
                        var vp = Vector4.Transform(p, _camera.ViewProjection);
                        if (Math.Abs(vp.X) >= vp.W || Math.Abs(vp.Y) >= vp.W || Math.Abs(vp.Z) >= vp.W)
                        {
                            outside = true;
                            break;
                        }
                    }
                    if (dir == 0) dir = outside ? 1 : -1;
                    else if ((dir < 0) == outside) break;
                    scale += dir;
                    if (scale <= 0 || scale > 100) break;
                    _camera.Position = _camera.Target - f * (d * scale * 0.1f);
                }
            }
        }

        public void Rewind()
        {
            World?.Rewind();
            foreach (var i in _objects) i.Value.Rewind();
        }

        private void UpdateEnv(int width, int height, ref LightSpace lightSpace)
        {
            var pref = Environment?.PreferenceConfig ?? Config.Preferences[0];
            var env = Environment?.EnvironmentConfig ?? Config.Environments[0];

            _camera.SetProjection(env.CameraFov * MathUtil.ToRadians,
                width,
                height,
                env.CameraNear,
                env.CameraFar);

            _CameraObject?.CameraCapture(ref _camera);

            var space = World?.Space ?? Config.Grounds[0].Space;
            var grid = World?.Grid ?? Config.Grounds[0].Grid;

            if (env.UsingLight && pref.LightMode == LightMode.None)
            {
                lightSpace?.Dispose();
                lightSpace = null;
            }
            else
            {
                if (lightSpace == null) lightSpace = new LightSpace();
                lightSpace.Space = space;
            }
            _quadTree.Resize(space, grid);
        }

        private void LockStage(List<Task> tasks)
        {
            do
            {
                for (var i = tasks.Count - 1; i >= 0; i--)
                {
                    var task = tasks[i];
                    if (task.Wait(1)) tasks.RemoveAt(i);
                    else Asset.LoadReserved();
                }
            }
            while (tasks.Count > 0);
        }

        public void Update(int width, int height, ref LightSpace lightSpace, float delta, bool loop)
        {
            UpdateEnv(width, height, ref lightSpace);

            World?.Update(delta);

            var inflags = UpdateFlags.None;
            if (_camera.ViewUpdated) inflags |= UpdateFlags.View;

            var subobjs = new List<SceneObject>[UpdatePass.Max];
            var taskCount = 0;
            foreach (var i in _objects)
            {
                if (i.Value.Located)
                {
                    var pass = i.Value.GetUpdatePass();
                    if (subobjs[pass] == null) subobjs[pass] = new List<SceneObject>(_objects.Count);
                    subobjs[pass].Add(i.Value);
                    if (taskCount < subobjs[pass].Count) taskCount = subobjs[pass].Count;
                }
            }

            lightSpace?.BeginUpdate();

            var tasks = new List<Task>(taskCount);

            var lightSpace_ = lightSpace;

            SceneObject cameraRoot = null;
            if (_CameraObject != null)
            {
                cameraRoot = _CameraObject;
                while (cameraRoot != null && !cameraRoot.Located) cameraRoot = cameraRoot.GetAncestor<SceneObject>(false);
            }

            for (var i = 0; i <= UpdatePass.Max; i++)
            {
                if (subobjs[i] != null)
                {
                    foreach (var obj in subobjs[i])
                    {
                        var objloop = loop || obj != _SelectedAnimation;

                        if (obj == cameraRoot)
                        {
                            var outflags = inflags;
                            if (obj.Update(lightSpace, delta, true, ref outflags) != UpdateState.Stopped)
                            {
                                _CameraObject?.CameraCapture(ref _camera);
                                if (_camera.ViewUpdated) inflags |= UpdateFlags.View;
                            }
                            else if (objloop) obj.Rewind();
                        }
                        else
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                var outflags = inflags;
                                if (obj.Update(lightSpace_, delta, true, ref outflags) == UpdateState.Stopped && objloop) obj.Rewind();
                            }));
                        }
                    }
                    LockStage(tasks);
                }
            }

            lightSpace?.EndUpdate();

            UpdateSounds(delta);
        }

        private void LockStage(List<Task<ShowFlags>> tasks, ref ShowFlags showFlags)
        {
            do
            {
                for (var i = tasks.Count - 1; i >= 0; i--)
                {
                    var task = tasks[i];
                    if (task.Wait(1))
                    {
                        showFlags |= task.Result;
                        tasks.RemoveAt(i);
                    }
                    else Asset.LoadReserved();
                }
            }
            while (tasks.Count > 0);
        }

        private void PreProcess(Graphics graphics, LightSpace lightSpace)
        {
            var pref = Environment?.PreferenceConfig ?? Config.Preferences[0];
            var env = Environment?.EnvironmentConfig ?? Config.Environments[0];

            graphics.Push();

            if (env.UsingPostProcess)
            {
                var targetDesc = new RenderTargetDescription
                {
                    Width = graphics.Target.Width,
                    Height = graphics.Target.Height,
                    Attachments = new RenderTargetAttachmentDescription[env.BloomIntensity > 0 ? 3 : 2]
                };
                
                var att = 0;

                targetDesc.Attachments[att++] = new RenderTargetAttachmentDescription
                {
                    Attachment = FramebufferAttachment.ColorAttachment0,
                    Format = RawFormat.Rgb16f,
                    Samples = pref.Samples,
                    Texture = true
                };

                if (env.BloomIntensity > 0)
                {
                    var bloomDesc = new RenderTargetAttachmentDescription
                    {
                        Attachment = FramebufferAttachment.ColorAttachment1,
                        Format = RawFormat.Rgb16f,
                        Samples = pref.Samples
                    };
                    if (pref.Samples <= 1)
                    {   //멀티샘플의 경우 블룸맵을 다시 캡쳐하므로 랜더버퍼사용, 멀티샘플이 아니면 블룸맵을 바로 사용가능
                        bloomDesc.Texture = true;
                        bloomDesc.TextureMinFilter = TextureMinFilter.Linear;
                        bloomDesc.TextureMagFilter = TextureMagFilter.Linear;
                    }

                    targetDesc.Attachments[att++] = bloomDesc;
                }

                targetDesc.Attachments[att++] = new RenderTargetAttachmentDescription
                {
                    Attachment = FramebufferAttachment.DepthStencilAttachment,
                    Format = RawFormat.Depth24Stencil8,
                    Samples = pref.Samples
                };

                var target = RenderTargets.NewTemporary(targetDesc);

                target.ClearColor(0, env.UsingFog ? env.FogColor : _BackColor);
                target.ClearDepthStencil();
                if (env.BloomIntensity > 0)
                {
                    target.ClearColor(1, Color4.Black);
                    graphics.BloomThreshold = env.BloomThreshold;
                }
                graphics.Target = target;
            }
            else
            {
                graphics.Clear(env.UsingFog ? env.FogColor : _BackColor);
            }

            if (env.UsingFog)
            {
                graphics.FogColor = env.FogColor;
                graphics.FogNear = env.FogNear;
                graphics.FogFar = env.FogFar;
            }
            else graphics.ClearFog();

            graphics.Camera = _camera;          //TODO:멀티카메라를 사용할 경우 추후 대응

            graphics.World = Matrix4x4.Identity;

            if (lightSpace != null)
            {
                lightSpace.Mode = pref.LightMode;
                lightSpace.AllowShadow = pref.AllowShadow && env.UsingShadow;
                lightSpace.AllowShadowPixel32 = pref.AllowShadowPixel32 && env.UsingShadowPixel32;
                lightSpace.MaxShadowResolution = Math.Min(pref.MaxShadowResolution, env.MaxShadowResolution);

                lightSpace.AmbientLight = env.AmbientLight;
                lightSpace.EnvMap = env.SkyboxMap;
                lightSpace.EnvColor = env.SkyboxColor;
            }
        }

        public void Draw(Graphics graphics, LightSpace lightSpace)
        {
            PreProcess(graphics, lightSpace);

            graphics.DepthMode = DepthMode.ReadWrite;

            if (lightSpace != null) 
            {
                IEnumerable<SceneComponent> objs;

                if (!_SelectedObjectOnly || _SelectedComponent == null) 
                {
                    var objs_ = new List<SceneComponent>(_objects.Count);
                    foreach(var i in _objects)
                    {
                        if (i.Value.Located) objs_.Add(i.Value);
                    }
                    objs = objs_;
                }
                else objs = new SceneComponent[] { _SelectedComponent };

                while (lightSpace.BeginDraw(graphics, out var layer))
                {
                    World?.Draw(graphics, layer);
                    foreach (var obj in objs) obj.Draw(graphics, layer);
                    _dragObject?.Draw(graphics, layer);
                    lightSpace.EndDraw(graphics);
                }
            }

            var showFlags = ShowFlags.None;

            var vsobjs = new LinkedList<(SceneComponent obj, float distance)>();

            if (!_SelectedObjectOnly || _SelectedComponent == null)
            {
                var objs = _quadTree.Select(graphics.Camera.BoundingFrustum());
                var tasks = new List<Task<ShowFlags>>(objs.Count);
                foreach (var obj in objs) tasks.Add(Task.Run(() => obj.Show()));
                LockStage(tasks, ref showFlags);

                foreach (var obj in objs)
                {
                    if (obj.GetAABB(out var aabb))
                    {
                        var d = Vector3.Distance(graphics.Camera.Position, aabb.Center) - aabb.Radius;

                        var node = vsobjs.First;
                        while (node != null && d < node.Value.distance) node = node.Next;
                        if (node != null) vsobjs.AddBefore(node, (obj, d));
                        else vsobjs.AddLast((obj, d));
                    }
                }
            }
            else
            {
                showFlags |= _SelectedComponent.Show();
                vsobjs.AddLast((_SelectedComponent, 0));
            }

            if (_dragObject != null)
            {
                showFlags |= _dragObject.Show();
                vsobjs.AddLast((_dragObject, 0));
            }

            World?.Draw(graphics, InstanceLayer.Base);
            for (var node = vsobjs.First; node != null; node = node.Next) node.Value.obj.Draw(graphics, InstanceLayer.Base);

            graphics.DepthMode = DepthMode.Read;

            graphics.DrawSkybox();

            for (var layer = InstanceLayer.BlendBottom; layer <= InstanceLayer.BlendTop; layer++)
            {
                World?.Draw(graphics, layer);
                for (var node = vsobjs.Last; node != null; node = node.Previous) node.Value.obj.Draw(graphics, layer);
            }

            if ((showFlags & ShowFlags.Distortion) != 0)
            {
                var distortionRenderer = Renderers.Distortion;
                distortionRenderer.Begin(graphics);
                graphics.Renderer = distortionRenderer;
                World?.Draw(graphics, InstanceLayer.Distortion);
                for (var node = vsobjs.Last; node != null; node = node.Previous) node.Value.obj.Draw(graphics, InstanceLayer.Distortion);
                distortionRenderer.End(graphics);
            }

            CameraObject?.CameraFilter(graphics);

            if (_Mode == SceneMode.Edit)
            {
                var flag = _SelectedComponent != null;

                for (var node = vsobjs.Last; node != null; node = node.Previous)
                {
                    var obj = node.Value.obj;

                    if (obj == _SelectedComponent)
                    {
                        graphics.Material.BlendMode = BlendMode.None;
                        graphics.Color = Color4.White;
                        flag = false;
                    }
                    else if (obj.CursorAlwaysVisible)
                    {
                        graphics.Material.BlendMode = BlendMode.Alpha;
                        graphics.Color = Color4.FaintWhite;
                    }
                    else continue;

                    obj.Draw(graphics, InstanceLayer.Cursor);
                }
                graphics.Material.BlendMode = BlendMode.None;
                graphics.Color = Color4.White;

                if (flag) _SelectedComponent.Draw(graphics, InstanceLayer.Cursor);
            }

            if (_CameraGizmo) DrawCameraGizmo(graphics);

            PostProcess(graphics);
        }

        private void DrawCameraGizmo(Graphics graphics)
        {
            graphics.ClearFog();
            graphics.Clear(_BackColor, ClearBufferMask.DepthBufferBit);
            graphics.DepthMode = DepthMode.ReadWrite;
            graphics.Material.ReceiveShadow = false;
            graphics.Material.Metallic = 0;
            graphics.Material.Roughness = 1;

            graphics.Camera.Fov = 0;

            var p = graphics.Camera.Target;
            p += graphics.Camera.Right * (graphics.Camera.Width * 0.5f - 50);
            p += graphics.Camera.Up * (graphics.Camera.Height * 0.5f - 50);

            graphics.World = Matrix4x4.CreateTranslation(p);
            graphics.DrawVertices(VertexArrays.GetCube(0, Vector3.Zero, 10, Rectangle.ZeroToOne, out var aabb), PrimitiveMode.Triangles, aabb);

            var cone = VertexArrays.GetCylinder(0, new Vector3(0, 0, 10), 10, 0, 20, Rectangle.ZeroToOne, out aabb);

            graphics.Color = Color4.Blue;
            graphics.DrawVertices(cone, PrimitiveMode.Triangles, aabb);

            graphics.PushTransform();
            graphics.RotateY(MathUtil.PiOverTwo);
            graphics.Color = Color4.Red;
            graphics.DrawVertices(cone, PrimitiveMode.Triangles, aabb);

            graphics.ResetTransform();

            graphics.RotateX(-MathUtil.PiOverTwo);
            graphics.Color = Color4.Green;
            graphics.DrawVertices(cone, PrimitiveMode.Triangles, aabb);
            graphics.PopTransform();
        }

        private void PostProcess(Graphics graphics)
        {
            var pref = Environment?.PreferenceConfig ?? Config.Preferences[0];
            var env = Environment?.EnvironmentConfig ?? Config.Environments[0];

            if (env.UsingPostProcess)
            {
                var src = graphics.Target;
                graphics.Pop();
                var dest = graphics.Target;

                var command = graphics.Command((GraphicsApi api) => {
                    Shaders.PostProcess.Draw(api, src, dest, env.BloomPass, env.BloomIntensity, env.Exposure, env.Gamma);
                });

                command.AddFence(src, BatchFlag.Read | BatchFlag.Retrieve);
                command.AddFence(dest, BatchFlag.ReadWrite | BatchFlag.Retrieve);
            }
            else graphics.Pop();
        }

        private void StopSoundHandle(int handle)
        {
            lock (_sounds)
            {
                for (var i = 0; i < _sounds.Count; i++)
                {
                    if (_sounds[i].handle == handle)
                    {
                        _sounds.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public int PlaySound(string path, float volume, AudioControl control, int loop, int priority, float duplication, Vector3? pos) 
        {
	        if (!_SoundEnabled) return 0;

            var env = Environment?.EnvironmentConfig ?? Config.Environments[0];

            if (pos != null) 
            {
		        float d = Vector3.Distance(Camera.Position, pos.Value);
		        if (d >= env.SoundMaxDistance) return 0;
		        float f = env.SoundRefDistance / (env.SoundRefDistance + env.SoundRollOffFactor * (Math.Max(d, env.SoundRefDistance) - env.SoundRefDistance));
                volume *= f;
	        }

            lock (_sounds)
            {
                int playing = 0;

                foreach (var sound in _sounds) {
                    if (sound.control == control && sound.duplication != 0)
                    {
                        if (sound.path == path)
                        {
                            if (sound.volume < volume)
                            {
                                sound.volume = volume;
                                AudioPlayer.Instance.SetVolume(sound.handle, volume);
                            }
                            if (sound.loop < loop || (sound.loop != 0 && loop == 0))
                            {
                                sound.loop = loop;
                                AudioPlayer.Instance.SetLoop(sound.handle, loop);
                            }
                            if (sound.priority < priority)
                            {
                                sound.priority = priority;
                            }
                            return 0;
                        }
                        playing++;
                    }
                }
                
                var capacity = env.SoundCapacity(control);

                if (capacity != 0 && playing >= capacity)
                {
                    var index = -1;
                    var minpv = priority + volume;
                    for (int i = 0; i < _sounds.Count; i++)
                    {
                        var sound = _sounds[i];
                        if (sound.control == control)
                        {
                            var pv = sound.priority + sound.volume;
                            if (sound.control == control && pv <= minpv)
                            {
                                index = i;
                                minpv = pv;
                            }
                        }
                    }
                    if (index != -1) AudioPlayer.Instance.Stop(_sounds[index].handle);
                    else return 0;
                }
                {
                    var sound = new Sound
                    {
                        path = path,
                        volume = volume,
                        control = control,
                        loop = loop,
                        priority = loop,
                        duplication = Math.Max(duplication, env.SoundDuplication),
                        handle = AudioPlayer.Instance.Play(path, volume, control, loop, StopSoundHandle)
                    };
                    _sounds.Add(sound);

                    return sound.handle;
                }
            }
        }

        private void UpdateSounds(float delta)
        {
            lock (_sounds) 
            {
                foreach (var sound in _sounds) 
                {
                    sound.duplication = Math.Max(sound.duplication - delta, 0);
                }
            }
        }

        public bool KeyDown(KeyEventArgs e)
        {
            return _SelectedComponent != null && _SelectedComponent.KeyDown(e);
        }

        public bool KeyUp(KeyEventArgs e)
        {
            return _SelectedComponent != null && _SelectedComponent.KeyUp(e);
        }

        public bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            _mousePicked = false;

            if (_SelectedComponent != null && _SelectedComponent.MouseDown(e, controlKey, shiftKey)) return true;

            if (e.Button == MouseButtons.Left && controlKey && !shiftKey)
            {
                var ray = _camera.PickRay(new Vector2(e.X, e.Y));

                if (Intersects(ray, null, true, CollisionFlags.None, out var obj, out _, out _) && obj != null && obj != _SelectedComponent)
                {
                    SelectedComponent = obj;
                    _mousePicked = true;
                    return true;
                }
            }

            return false;
        }

        public bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            var result = !_mousePicked && _SelectedComponent != null && _SelectedComponent.MouseUp(e, controlKey, shiftKey);

            _mousePicked = false;

            return result;
        }

        public bool MouseMove(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            var result = !_mousePicked && _SelectedComponent != null && _SelectedComponent.MouseMove(e, _mouseX, _mouseY, controlKey, shiftKey);

            _mouseX = e.X;
            _mouseY = e.Y;

            return result;
        }

        public bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            return _SelectedComponent != null && _SelectedComponent.MouseWheel(e, controlKey, shiftKey);
        }

        private bool Drag(string data, in Ray ray, bool controlKey, bool shiftKey)
        {
            if (_SelectedComponent != null)
            {
                var selectedObject = _SelectedComponent is SceneObject sobj ? sobj : null;

                if (Intersects(ray, selectedObject, controlKey, CollisionFlags.None, out var obj, out var distance, out _))
                {
                    var pos = ray.Position + ray.Direction * distance;

                    if (_dragObject == null)
                    {
                        var origin = GetObject(data);

                        if (origin != null)
                        {
                            if (origin.AllowDrag)
                            {
                                var newobj = (SceneObject)origin.Clone(false);

                                if (_SelectedComponent.AddSubEnabled(newobj) || (_SelectedComponent.Parent is SceneComponent parent && parent.AddSubEnabled(newobj)))
                                {
                                    newobj.Located = true;
                                    newobj.UsingTransform = true;
                                    newobj.Transform.Target = null;
                                    newobj.Transform.Binding = null;
                                    newobj.Transform.Position = pos;
                                    newobj.Transform.FromGround = false;

                                    _dragObject = newobj;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            var asset = AssetManager.Instance.GetAsset(data);

                            if (asset != null && asset.Spawnable)
                            {
                                var newobj = (SceneObject)asset.NewSceneComponent();

                                if (_SelectedComponent.AddSubEnabled(newobj) || (_SelectedComponent.Parent is SceneComponent parent && parent.AddSubEnabled(newobj)))
                                {
                                    newobj.Located = true;
                                    newobj.UsingTransform = true;
                                    newobj.Transform.Position = pos;
                                    newobj.Transform.FromGround = false;

                                    _dragObject = newobj;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (controlKey && obj != null)
                        {
                            _dragObject.Transform.Target = obj;
                            _dragObject.Transform.Position = Vector3.Zero;
                        }
                        else
                        {
                            _dragObject.Transform.Target = null;
                            _dragObject.Transform.Position = pos;
                        }
                        return true;
                    }
                }
            }
            if (_dragObject != null)
            {
                _dragObject = null;
                OnRefresh();
            }
            return false;
        }

        public bool DragOver(string data, in Ray ray, bool controlKey, bool shiftKey)
        {
            return Drag(data, ray, controlKey, shiftKey);
        }

        public void DragLeave()
        {
            if (_dragObject != null)
            {
                _dragObject = null;
                OnRefresh();
            }
        }

        public void DragDrop(string data, in Ray ray, bool controlKey, bool shiftKey)
        {
            if (Drag(data, ray, controlKey, shiftKey))
            {
                if (_SelectedComponent.Attach(_dragObject, true) || (_SelectedComponent.Parent is SceneComponent parent && parent.Attach(_dragObject, true)))
                {
                    if (_dragObject.Transform.Target == null)
                    {
                        var p = _dragObject.Transform.Position;
                        p.Z = 0;
                        _dragObject.Transform.Position = p;
                        _dragObject.Transform.FromGround = true;
                    }
                    _dragObject = null;
                    OnRefresh();
                }
            }
        }

        internal void Save(XmlWriter writer) 
        {
            writer.WriteStartElement("scene");
            SaveChildren(writer);
            writer.WriteEndElement();
        }

        internal void Load(XmlNode node)
        {
            if (node.LocalName != "scene") throw new XmlException();

            LoadChildren(node);
        }

        internal void Build(BinaryWriter writer)
        {
            var param = new SceneBuildParam(0);

            var config = Environment?.EnvironmentConfig ?? Config.Environments[0];
            config.Build(writer, param);

            if (World != null) World.Build(writer);
            else writer.Write((byte)World.BuildType.None);

            var children = Children.Where(c => c is SceneObject);
            writer.WriteLength(children.Count());
            foreach (SceneObject child in children) child.Build(writer, param);
        }
    }
}
