using System;
using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Sources;
using CDK.Assets.Animations.Components;

using MeshInstance = CDK.Drawing.Meshing.Instance;

namespace CDK.Assets.Animations.Particle
{
    public class ParticleObject : SceneObject
    {
        public Particle Origin { private set; get; }

        private AnimationSource _SelectedSource;
        public AnimationSource SelectedSource
        {
            set => SetProperty(ref _SelectedSource, value, false);
            get => _SelectedSource;
        }

        private const int InstanceRandomCount = 18;
        private class Instance
        {
            public Matrix4x4? transform;
            public Matrix4x4 world;
            public AnimationSource source;
            public Vector3 firstDir;
            public Vector3 lastDir;
            public Vector3 pos;
            public Vector3 billboardDir;
            public Vector3 billboard;
            public Color4 color;
            public float[] randoms;
            public float life;
            public float progress;
            public float delta;
        }

        private LinkedList<Instance> _instances;

        public int InstanceCount => _instances.Count;

        private float _progress;
        private float _counter;
        private float _remaining;
        private bool _emitting;
        private bool _visible;
        private int _random;
        private List<List<Instance>> _imageFragments;
        private List<(AnimationSource source, MeshInstance origin, List<VertexArrayInstance> instances)> _meshFragments;

        private const int MaxInstances = 1000;

        public ParticleObject()
        {
            Origin = new Particle(this);

            Init();
        }

        public ParticleObject(Particle origin)
        {
            Origin = origin;
            Origin.AddWeakRefresh(Origin_Refresh);

            Init();
        }

        public ParticleObject(ParticleObject other, bool binding) : base(other, binding, true)
        {
            if (binding)
            {
                Origin = other.Origin;
                Origin.AddWeakRefresh(Origin_Refresh);
            }
            else Origin = new Particle(this, other.Origin);

            Init();
        }

        private void Origin_Refresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        private void Init()
        {
            _instances = new LinkedList<Instance>();

            if (Origin.Prewarm && Origin.EmissionRate > 0)
            {
                _counter = Origin.EmissionMax / Origin.EmissionRate;
            }
            _emitting = true;

            _imageFragments = new List<List<Instance>>();
            _meshFragments = new List<(AnimationSource, MeshInstance, List<VertexArrayInstance>)>();
            _random = RandomUtil.Next();
        }

        private void AddInstance(Matrix4x4? transform, float delta)
        {
            if (Origin.Sources.Count == 0 || _instances.Count >= MaxInstances) return;

            var life = Origin.Life.Value + Origin.Life.ValueVar * RandomUtil.NextFloat(-1, 1);

            if (life <= delta) return;

            var p = new Instance
            {
                transform = transform,
                source = Origin.Sources[RandomUtil.Next(0, Origin.Sources.Count)],
                life = life,
                progress = delta,
                delta = delta,
                randoms = new float[InstanceRandomCount]
            };
            for (var i = 0; i < InstanceRandomCount; i++) p.randoms[i] = RandomUtil.NextFloat();

            Origin.Shape.Issue(out p.pos, out p.firstDir, Origin.ShapeShell);

            _instances.AddLast(p);
        }

        private void UpdateInstance(Instance p, in Matrix4x4 worldPrev, in Matrix4x4 cameraView)
        {
            var progress = p.progress / p.life;

            var randomSeq = 0;

            p.color = Origin.Color.GetColor(progress, new Color4(
                p.randoms[randomSeq++],
                p.randoms[randomSeq++],
                p.randoms[randomSeq++],
                p.randoms[randomSeq++]));

            p.lastDir = new Vector3(
                Origin.X.GetValue(progress, p.randoms[randomSeq++]),
                Origin.Y.GetValue(progress, p.randoms[randomSeq++]),
                Origin.Z.GetValue(progress, p.randoms[randomSeq++]));

            p.lastDir += Origin.Radial.GetValue(progress, p.randoms[randomSeq++]) * p.firstDir;
            p.pos += p.lastDir * p.delta;

            p.billboardDir = new Vector3(
                Origin.BillboardX.GetValue(progress, p.randoms[randomSeq++]),
                Origin.BillboardY.GetValue(progress, p.randoms[randomSeq++]),
                Origin.BillboardZ.GetValue(progress, p.randoms[randomSeq++]));

            p.billboard += p.billboardDir * p.delta;

            var rotation = new Vector3(
                Origin.RotationX.GetValue(progress, p.randoms[randomSeq++]) * MathUtil.ToRadians,
                Origin.RotationY.GetValue(progress, p.randoms[randomSeq++]) * MathUtil.ToRadians,
                Origin.RotationZ.GetValue(progress, p.randoms[randomSeq++]) * MathUtil.ToRadians);

            Vector3 scale;
            if (Origin.ScaleEach)
            {
                scale = new Vector3(
                    Origin.ScaleX.GetValue(progress, p.randoms[randomSeq++]),
                    Origin.ScaleY.GetValue(progress, p.randoms[randomSeq++]),
                    Origin.ScaleZ.GetValue(progress, p.randoms[randomSeq++]));
            }
            else scale = new Vector3(Origin.ScaleX.GetValue(progress, p.randoms[randomSeq++]));

            p.world = p.transform ?? worldPrev;

            var world = Matrix4x4.Identity;

            //TODO:CHECK
            var cameraX = new Vector3(cameraView.M11, cameraView.M21, cameraView.M31);
            var cameraY = new Vector3(-cameraView.M12, -cameraView.M22, -cameraView.M32);
            var cameraZ = new Vector3(cameraView.M13, cameraView.M23, cameraView.M33);

            switch (Origin.View)
            {
                case ParticleView.Billboard:
                    cameraView.Billboard(p.world, out world);
                    break;
                case ParticleView.HorizontalBillboard:
                    cameraView.HorizontalBillboard(p.world, out world);
                    break;
                case ParticleView.VerticalBillboard:
                    cameraView.VerticalBillboard(p.world, out world);
                    break;
                case ParticleView.StretchBillboard:
                    {
                        Vector3 dir = p.lastDir;
                        dir += cameraX * p.billboardDir.X;
                        dir += cameraY * p.billboardDir.Y;
                        dir += cameraZ * p.billboardDir.Z;
                        cameraView.StretchBillboard(p.world, dir, Origin.StretchRate, out world);
                    }
                    break;
            }
            if (rotation != Vector3.Zero)
            {
                world = Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * world;
            }
            if (scale.X != 1)
            {
                world.M11 *= scale.X;
                world.M12 *= scale.X;
                world.M13 *= scale.X;
            }
            if (scale.Y != 1)
            {
                world.M21 *= scale.Y;
                world.M22 *= scale.Y;
                world.M23 *= scale.Y;
            }
            if (scale.Z != 1)
            {
                world.M31 *= scale.Z;
                world.M32 *= scale.Z;
                world.M33 *= scale.Z;
            }
            world.M41 = p.pos.X;
            world.M42 = p.pos.Y;
            world.M43 = p.pos.Z;
            world *= worldPrev;

            if (p.billboard.X != 0)
            {
                var offset = cameraX * p.billboard.X * p.world.M11;
                world.M41 += offset.X;
                world.M42 += offset.Y;
                world.M43 += offset.Z;
            }
            if (p.billboard.Y != 0)
            {
                var offset = cameraY * p.billboard.Y * p.world.M22;
                world.M41 += offset.X;
                world.M42 += offset.Y;
                world.M43 += offset.Z;
            }
            if (p.billboard.Z != 0)
            {
                var offset = cameraZ * p.billboard.Z * p.world.M33;
                world.M41 += offset.X;
                world.M42 += offset.Y;
                world.M43 += offset.Z;
            }
            p.world = world;
            p.delta = 0;
        }
        public override SceneComponentType Type => SceneComponentType.Particle;
        public override SceneComponent Clone(bool binding) => new ParticleObject(this, binding);
        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (GetTransform(out var transform))
            {
                Origin.Shape.AddAABB(transform, ref result);
                return true;
            }
            return false;
        }

        public override float GetDuration(DurationParam param, float duration = 0)
        {
            if (duration != 0)
            {
                switch (param)
                {
                    case DurationParam.Avg:
                        duration += (Origin.Life.Value + Origin.Life.ValueVar) * 0.5f;
                        break;
                    case DurationParam.Max:
                        duration += Origin.Life.Value + Origin.Life.ValueVar;
                        break;
                }
            }
            return duration;
        }

        public override float Progress => _progress;
        internal override bool AfterCameraUpdate()
        {
            return Origin.View != ParticleView.None ||
                Origin.BillboardX.Type != AnimationFloatType.None ||
                Origin.BillboardY.Type != AnimationFloatType.None ||
                Origin.BillboardZ.Type != AnimationFloatType.None;
        }

        internal override void Rewind()
        {
            _instances.Clear();
            _progress = 0;
            _counter = Origin.Prewarm && Origin.EmissionRate > 0 ? Origin.EmissionMax / Origin.EmissionRate : 0;
            _remaining = 0;
            _emitting = true;
            _random = RandomUtil.Next();
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            var inflags = flags;

            if ((inflags & UpdateFlags.Transform) != 0) flags |= UpdateFlags.AABB;

            _progress += delta;
            _remaining += delta;
            _emitting &= alive;

            if (_emitting) return UpdateState.Alive;

            if (!alive && Origin.Clear) _instances.Clear();
            else
            {
                var node = _instances.First;
                while (node != null)
                {
                    var p = node.Value;
                    var next = node.Next;
                    if (p.progress + _remaining >= p.life) _instances.Remove(node);
                    node = next;
                }
                if (_instances.Count != 0) return alive ? UpdateState.Alive : UpdateState.Finishing;
            }
            return Origin.Finish ? UpdateState.Stopped : UpdateState.None;
        }

        internal override ShowFlags Show()
        {
            _imageFragments.Clear();
            foreach (var frag in _meshFragments) frag.instances.Clear();

            var worldPrev = Matrix4x4.Identity;

            if (Origin.LocalSpace && !GetTransform(out worldPrev))
            {
                _visible = false;
                _meshFragments.Clear();
                return ShowFlags.None;
            }

            if (_remaining != 0)
            {
                var node = _instances.First;
                while (node != null)
                {
                    var p = node.Value;
                    p.delta = _remaining;
                    p.progress += _remaining;
                    var next = node.Next;
                    if (p.progress >= p.life) _instances.Remove(node);
                    node = next;
                }

                if (_emitting && Origin.EmissionRate > 0)
                {
                    var rate = 1 / Origin.EmissionRate;

                    var lifeMax = Origin.Life.Value + Origin.Life.ValueVar;

                    _counter += _remaining;
                    if (_counter > lifeMax)
                    {
                        _counter %= rate;
                        _counter += (float)Math.Ceiling(lifeMax / rate) * rate;
                    }

                    while (_counter >= rate)
                    {
                        _counter -= rate;

                        if (_counter < lifeMax && _instances.Count < Origin.EmissionMax)
                        {
                            if (GetTransform(_progress - _counter, null, out var transform))
                            {
                                AddInstance(Origin.LocalSpace ? null : new Matrix4x4?(transform), _counter);
                            }
                        }
                    }
                }

                var cameraView = Scene.Camera.View;

                var tasks = new Task[_instances.Count];
                var i = 0;
                foreach (var pp in _instances)
                {
                    var p = pp;
                    tasks[i++] = Task.Run(() => UpdateInstance(p, worldPrev, cameraView));
                }
                Task.WaitAll(tasks);

                _remaining = 0;
            }

            var showFlags = ShowFlags.None;

            foreach (var i in _instances)
            {
                switch (i.source.Type)
                {
                    case AnimationSourceType.Image:
                        {
                            var source = (AnimationSourceImage)i.source;

                            if (source.RootImage != null && source.SubImages.Length != 0)
                            {
                                var isNew = true;
                                for (var j = 0; j < _imageFragments.Count; j++)
                                {
                                    var otherSource = (AnimationSourceImage)_imageFragments[j][0].source;

                                    if (source == otherSource)
                                    {
                                        _imageFragments[j].Add(i);
                                        isNew = false;
                                        break;
                                    }
                                }
                                if (isNew)
                                {
                                    var iis = new List<Instance> { i };
                                    _imageFragments.Add(iis);
                                }
                            }
                        }
                        break;
                    case AnimationSourceType.Mesh:
                        {
                            var selection = ((AnimationSourceMesh)i.source).Selection;

                            if (selection.Geometry != null)
                            {
                                showFlags |= selection.ShowFlags;

                                var instanceProgress = selection.GetInstanceProgress(i.progress);           //TODO:DIFF WITH CLIENT

                                var isNew = true;

                                foreach (var frag in _meshFragments)
                                {
                                    if (frag.source == i.source && (frag.instances.Count == 0 || frag.origin.Progress == instanceProgress))
                                    {
                                        frag.origin.Progress = i.progress;
                                        frag.instances.Add(new VertexArrayInstance(i.world, i.color));
                                        isNew = false;
                                    }
                                }

                                if (isNew)
                                {
                                    var origin = new MeshInstance(selection.Geometry.Origin)
                                    {
                                        Skin = selection,
                                        FrameDivision = selection.FrameDivision,
                                        Animation = selection.Animation?.Origin,
                                        Progress = i.progress
                                    };
                                    (AnimationSource, MeshInstance, List<VertexArrayInstance>) e = (i.source, origin, new List<VertexArrayInstance> { new VertexArrayInstance(i.world, i.color) });
                                    _meshFragments.Add(e);
                                }
                            }
                        }
                        break;
                }
            }

            var cameraPos = Scene.Camera.Position;
            foreach (var iis in _imageFragments)
            {
                iis.Sort((i0, i1) =>
                {
                    var d0 = Vector3.DistanceSquared(cameraPos, i0.world.Translation);
                    var d1 = Vector3.DistanceSquared(cameraPos, i1.world.Translation);
                    return d1.CompareTo(d0);
                });
            }
            _visible = true;

            return showFlags;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (!_visible) return;

            if (_imageFragments.Count != 0)
            {
                foreach (var iis in _imageFragments)
                {
                    var source = (AnimationSourceImage)iis[0].source;

                    var texture = source.RootImage.Content.Texture;

                    if ((layer != InstanceLayer.Cursor || source == _SelectedSource) && texture != null && source.Material.Apply(graphics, _progress, _random, layer, false)) 
                    {
                        var command = new StreamRenderCommand(graphics, layer != InstanceLayer.Cursor ? PrimitiveMode.Triangles : PrimitiveMode.Lines);

                        if (layer != InstanceLayer.Cursor) command.State.Material.ColorMap = texture;

                        var vi = 0;

                        foreach (var p in iis)
                        {
                            command.World = p.world;
                            command.State.Material.Color = p.color;

                            var image = source.GetImage(p.progress / p.life);

                            var frame = image.Frame;
                            var rx = frame.Width * image.ContentScale * 0.5f;
                            var lx = -rx;
                            var by = frame.Height * image.ContentScale * 0.5f;
                            var ty = -by;
                            var lu = (float)frame.Left / texture.Description.Width;
                            var ru = (float)frame.Right / texture.Description.Width;
                            var tv = (float)frame.Top / texture.Description.Height;
                            var bv = (float)frame.Bottom / texture.Description.Height;

                            if (layer != InstanceLayer.Cursor)
                            {
                                command.AddIndex(vi + 0);
                                command.AddIndex(vi + 1);
                                command.AddIndex(vi + 2);
                                command.AddIndex(vi + 1);
                                command.AddIndex(vi + 3);
                                command.AddIndex(vi + 2);
                            }
                            else
                            {
                                command.AddIndex(vi + 0);
                                command.AddIndex(vi + 1);
                                command.AddIndex(vi + 1);
                                command.AddIndex(vi + 3);
                                command.AddIndex(vi + 3);
                                command.AddIndex(vi + 2);
                                command.AddIndex(vi + 2);
                                command.AddIndex(vi + 0);
                            }
                            vi += 4;

                            command.AddVertex(new FVertex(new Vector3(lx, ty, 0), new Vector2(lu, tv)));
                            command.AddVertex(new FVertex(new Vector3(rx, ty, 0), new Vector2(ru, tv)));
                            command.AddVertex(new FVertex(new Vector3(lx, by, 0), new Vector2(lu, bv)));
                            command.AddVertex(new FVertex(new Vector3(rx, by, 0), new Vector2(ru, bv)));
                        }

                        graphics.Command(command);
                    }
                }
            }
            foreach (var (source, origin, instances) in _meshFragments)
            {
                if (layer != InstanceLayer.Cursor || source == _SelectedSource)
                {
                    origin.Draw(graphics, layer, _progress, _random, instances);
                }
            }
            if (layer == InstanceLayer.Cursor && GetTransform(out var transform))
            {
                graphics.Push();
                graphics.Material.Shader = MaterialShader.NoLight;
                graphics.Transform(transform);
                Origin.Shape.Draw(graphics);
                graphics.Pop();
            }
        }

        public override string GetStatus() => $"particle count:{InstanceCount}";

        protected override void SaveContent(XmlWriter writer)
        {
            base.SaveContent(writer);
            Origin.Save(writer);
        }

        protected override void LoadContent(XmlNode node)
        {
            base.LoadContent(node);
            Origin.Load(node);
        }

        protected override void BuildContent(BinaryWriter writer, SceneBuildParam param)
        {
            Origin.Build(writer);
        }
    }
}
