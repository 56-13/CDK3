using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;

using CDK.Assets.Scenes;
using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Sources;

using MeshInstance = CDK.Drawing.Meshing.Instance;

namespace CDK.Assets.Animations.Trail
{
    public class TrailObject : SceneObject
    {
        public Trail Origin { private set; get; }

        private AnimationSource _SelectedSource;
        public AnimationSource SelectedSource
        {
            set => SetProperty(ref _SelectedSource, value);
            get => _SelectedSource;
        }

        private class TrailPoint
        {
            public Vector3 point;
            public Matrix4x4 transform;
            public float progress;
            public int link;

            public TrailPoint(in Vector3 point, float progress, int link)
            {
                this.point = point;
                this.progress = progress;
                this.link = link;
            }
        }

        private List<TrailPoint> _points;
        private float _progress;
        private float _remaining;
        private int _counter;
        private int _link;
        private Color4 _colorRandom;
        private float _rotationRandom;
        private float _scaleRandom;
        private int _materialRandom;
        private bool _emitting;
        private List<MeshInstance> _meshInstances;

        private enum MouseTesting
        {
            None,
            Wait,
            AddNew,
            End
        }
        private MouseTesting _mouseTesting;

        private const int MaxPoints = 1000;

        private const float MouseTestMaxDistance = 500;

        public TrailObject()
        {
            Origin = new Trail(this);

            Init();
        }

        public TrailObject(Trail origin)
        {
            Origin = origin;

            Init();
        }

        public TrailObject(TrailObject other, bool binding) : base(other, binding, true)
        {
            if (binding)
            {
                Origin = other.Origin;
                Origin.AddWeakRefresh(Origin_Refresh);
            }
            else Origin = new Trail(this, other.Origin);

            Init();
        }

        private void Origin_Refresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        private void Init()
        {
            _points = new List<TrailPoint>();
            _meshInstances = new List<MeshInstance>();
            ResetRandoms();
        }

        private void ResetRandoms()
        {
            _link = Origin.Sources.Count <= 1 ? 0 : RandomUtil.Next(0, Origin.Sources.Count - 1);

            _colorRandom = new Color4(
                RandomUtil.NextFloat(0, 1),
                RandomUtil.NextFloat(0, 1),
                RandomUtil.NextFloat(0, 1),
                RandomUtil.NextFloat(0, 1));
            _rotationRandom = RandomUtil.NextFloat(0, 1);
            _scaleRandom = RandomUtil.NextFloat(0, 1);
            _materialRandom = RandomUtil.Next();
        }
        
        public override SceneComponentType Type => SceneComponentType.Trail;
        public override SceneComponent Clone(bool binding) => new TrailObject(this, binding);
        internal override bool AddAABB(ref ABoundingBox result)
        {
            var flag = false;
            if (_mouseTesting != MouseTesting.None)
            {
                foreach (var p in _points) result.Append(p.point);
                flag = true;
            }
            else
            {
                if (GetTransform(0, null, out var transform))
                {
                    result.Append(transform.Translation);
                    flag = true;
                }
                if (GetTransform(_progress, null, out transform))
                {
                    result.Append(transform.Translation);
                    flag = true;
                }
            }
            return flag;
        }

        public override float Progress => _progress;
        internal override bool AfterCameraUpdate() => true;
        internal override void Rewind()
        {
            _progress = 0;
            _remaining = 0;
            _counter = 0;
            _points.Clear();
            _emitting = true;

            if (_mouseTesting != MouseTesting.None) _mouseTesting = MouseTesting.End;

            ResetRandoms();
        }

        public override float GetDuration(DurationParam param, float duration = 0)
        {
            foreach (var source in Origin.Sources)
            {
                switch (source.Type)
                {
                    case AnimationSourceType.Image:
                        {
                            var imageSource = (AnimationSourceImage)source;
                            var d = imageSource.Duration * imageSource.Loop.Count;
                            if (duration < d) duration = d;
                        }
                        break;
                    case AnimationSourceType.Mesh:
                        {
                            var meshSource = (AnimationSourceMesh)source;
                            if (meshSource.Selection.Animation != null)
                            {
                                var d = meshSource.Selection.Animation.Duration * meshSource.Selection.Loop.Count;
                                if (duration < d) duration = d;
                            }
                        }
                        break;
                }
            }
            if (Origin.ColorDuration != 0)
            {
                var d = Origin.ColorDuration * Origin.ColorLoop.Count;

                if (duration < d) duration = d;
            }
            if (Origin.RotationDuration != 0)
            {
                var d = Origin.RotationDuration * Origin.RotationLoop.Count;

                if (duration < d) duration = d;
            }
            if (Origin.ScaleDuration != 0)
            {
                var d = Origin.ScaleDuration * Origin.ScaleLoop.Count;

                if (duration < d) duration = d;
            }
            if (duration != 0 && Origin.Emission && Origin.EmissionLife != 0)
            {
                switch (param)
                {
                    case DurationParam.Avg:
                        duration += Origin.EmissionLife * 0.5f;
                        break;
                    case DurationParam.Max:
                        duration += Origin.EmissionLife;
                        break;
                }
            }
            return duration;
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            _progress += delta;
            _remaining += delta;
            _counter++;

            _emitting &= alive;

            var remaining = false;

            if ((flags & UpdateFlags.Transform) != 0) flags |= UpdateFlags.AABB;

            if (!Origin.Emission) _points.Clear();
            else if (Origin.EmissionLife != 0)
            {
                while (_points.Count != 0 && _points[0].progress <= _progress - Origin.EmissionLife) _points.RemoveAt(0);

                if (_points.Count != 0) remaining = true;
            }

            switch (_mouseTesting)
            {
                case MouseTesting.Wait:
                    remaining = true;
                    break;
                case MouseTesting.AddNew:
                    flags |= UpdateFlags.AABB;
                    _mouseTesting = MouseTesting.Wait;
                    remaining = true;
                    break;
                case MouseTesting.End:
                    flags |= UpdateFlags.AABB;
                    _mouseTesting = MouseTesting.None;
                    break;
            }

            if (alive) return UpdateState.Alive;
            else if (remaining) return UpdateState.Finishing;
            return Origin.EmissionLife != 0 ? UpdateState.Stopped : UpdateState.None;
        }

        private enum BreakType
        {
            None,
            Rewind,
            Reverse
        }

        private void AddBreak(float progress, float delta, float duration, AnimationLoop loop, ref List<(float delta, BreakType type)> breaks)
        {
            if (duration != 0)
            {
                var step0 = (int)Math.Ceiling(progress / duration);
                var step1 = (int)Math.Ceiling((progress - delta) / duration);

                if (step0 != step1 && (loop.Count == 0 || step1 <= loop.Count))
                {
                    if (breaks == null) breaks = new List<(float delta, BreakType type)>(1);
                    (float delta, BreakType type) entry;
                    entry.delta = progress - (int)(progress / duration) * duration;
                    entry.type = loop.Count == 0 || step0 <= loop.Count ? (loop.RoundTrip ? BreakType.Reverse : BreakType.Rewind) : BreakType.None;
                    
                    var i = 0;
                    while (i < breaks.Count)
                    {
                        if (entry.delta > breaks[i].delta) break;
                        i++;
                    }
                    breaks.Insert(i, entry);
                }
            }
        }

        private void AddPoint(in Vector3 p, float progress)
        {
            if (_points.Count >= MaxPoints) return;

            var replace = false;
            if (_points.Count != 0)
            {
                var pp = _points.Last();
                if (pp.link == _link && Vector3.DistanceSquared(pp.point, p) < Origin.Distance * Origin.Distance) return;
                if (_points.Count > 1)
                {
                    var plp = _points[_points.Count - 2];
                    replace = (pp.link != _link && pp.link != plp.link) || VectorUtil.NearEqual(Vector3.Normalize(pp.point - plp.point), Vector3.Normalize(p - pp.point));
                }
                else if (_points.Count == 1)
                {
                    replace = pp.link != _link;
                }
            }
            var tp = new TrailPoint(p, progress, _link);
            if (replace) _points[_points.Count - 1] = tp;
            else _points.Add(tp);
        }

        private void AddPoint(in Vector3 p) => AddPoint(p, _progress);

        private void UpdatePointTransforms(int s, int e)
        {
            var pslen = e - s + 1;
            var ps = new Vector3[pslen, 2];

            var cameraForward = Scene.Camera.Forward;

            for (var i = s; i <= e; i++)
            {
                var si = Math.Max(i - 1, s);
                var ei = Math.Min(i + 1, e);
                var p0 = _points[si].point;
                var p2 = _points[ei].point;
                var pf = Vector3.Normalize(p2 - p0);

                Vector3 pr;
                if (Origin.Billboard) pr = -cameraForward;
                else if (MathUtil.NearOne(Math.Abs(Vector3.Dot(pf, Vector3.UnitZ)))) pr = Vector3.Normalize(Vector3.Cross(pf, Vector3.UnitX));
                else pr = Vector3.Normalize(Vector3.Cross(Vector3.UnitZ, pf));

                ps[i - s, 0] = pf;
                ps[i - s, 1] = pr;
            }

            if (!Origin.Billboard && pslen > 2)
            {
                for (var ss = 0; ss < Origin.EmissionSmoothness; ss++)
                {
                    for (var i = 1; i < pslen - 1; i++)
                    {
                        ps[i, 1] = Vector3.Normalize(ps[i, 1] + ps[i + 1, 1] + ps[i - 1, 1]);
                    }
                }
            }

            for (var i = s; i <= e; i++)
            {
                var pf = ps[i - s, 0];
                var pr = ps[i - s, 1];
                var pu = Vector3.Normalize(Vector3.Cross(pr, pf));

                var p = _points[i];

                var delta = _progress - p.progress;
                var scale = Origin.Scale.GetValue(Origin.ScaleDuration != 0 ? Origin.ScaleLoop.GetProgress(delta / Origin.ScaleDuration) : 1, _scaleRandom);
                var rotation = Origin.Rotation.GetValue(Origin.RotationDuration != 0 ? Origin.RotationLoop.GetProgress(delta / Origin.RotationDuration) : 1, _rotationRandom);

                if (rotation != 0)
                {
                    var rm = Quaternion.CreateFromAxisAngle(pf, rotation * MathUtil.ToRadians);
                    pr = Vector3.Transform(pr, rm);
                    pu = Vector3.Transform(pu, rm);
                }
                pr *= scale;
                pu *= scale;

                var m = new Matrix4x4
                {
                    M11 = pf.X,
                    M12 = pf.Y,
                    M13 = pf.Z,

                    M21 = pu.X,
                    M22 = pu.Y,
                    M23 = pu.Z,

                    M31 = pr.X,
                    M32 = pr.Y,
                    M33 = pr.Z,

                    M41 = p.point.X,
                    M42 = p.point.Y,
                    M43 = p.point.Z,

                    M44 = 1
                };
                _points[i].transform = m;
            }
        }

        internal override ShowFlags Show()
        {
            var showFlags = ShowFlags.None;

            if (Parent != null && _mouseTesting == MouseTesting.None)
            {
                Matrix4x4 transform;

                if (!Origin.Emission)
                {
                    if (GetTransform(0, null, out transform)) AddPoint(transform.Translation, 0);
                    if (GetTransform(_progress, null, out transform)) AddPoint(transform.Translation, _progress);
                }
                else
                {
                    if (Origin.LocalSpace)
                    {
                        foreach (var p in _points)
                        {
                            if (GetTransform(p.progress, null, out transform)) p.point = transform.Translation;
                        }
                    }
                    if (_emitting && _counter != 0)
                    {
                        List<(float delta, BreakType type)> breaks = null;

                        if (Parent is AnimationObjectFragment current)
                        {
                            for (; ; )
                            {
                                AddBreak(current.Progress, _remaining, current.GetPathDuration(), current.Origin.PathLoop, ref breaks);
                                AddBreak(current.Progress, _remaining, current.Origin.RotationDuration, current.Origin.RotationLoop, ref breaks);
                                AddBreak(current.Progress, _remaining, current.Origin.ScaleDuration, current.Origin.ScaleLoop, ref breaks);
                                if (current.Parent is AnimationObjectFragment parent) current = parent;
                                else break;
                            }
                        }

                        for (var i = _counter - 1; i >= 0; i--)
                        {
                            var delta = i * _remaining / _counter;

                            float progress;

                            if (breaks != null)
                            {
                                while (breaks.Count != 0 && delta < breaks[0].delta)
                                {
                                    var b = breaks[0];

                                    if (b.type == BreakType.Rewind)
                                    {
                                        progress = _progress - b.delta - 0.0001f;

                                        if (GetTransform(progress, null, out transform))
                                        {
                                            AddPoint(transform.Translation, progress);
                                            _link++;
                                        }
                                    }
                                    progress = _progress - b.delta;

                                    if (GetTransform(progress, null, out transform))
                                    {
                                        AddPoint(transform.Translation, progress);

                                        if (b.type == BreakType.Reverse)
                                        {
                                            _link++;
                                            AddPoint(transform.Translation, progress);
                                        }
                                    }

                                    breaks.RemoveAt(0);
                                }
                            }

                            progress = _progress - delta;

                            if (GetTransform(progress, null, out transform)) AddPoint(transform.Translation, progress);
                        }
                    }
                }
            }

            _remaining = 0;
            _counter = 0;

            var meshCount = 0;

            if (Origin.Sources.Count != 0)
            {
                var s = 0;

                while (s < _points.Count)
                {
                    var link = _points[s].link;

                    var e = s;

                    while (e < _points.Count - 1 && _points[e + 1].link == link) e++;

                    if (s < e)
                    {
                        UpdatePointTransforms(s, e);

                        var source = Origin.Sources[link % Origin.Sources.Count];

                        switch (source.Type)
                        {
                            case AnimationSourceType.Mesh:
                                showFlags |= ShowMesh((AnimationSourceMesh)source, s, e, ref meshCount);
                                break;
                        }
                    }

                    s = e + 1;
                }
            }

            while (meshCount > _meshInstances.Count) _meshInstances.RemoveAt(_meshInstances.Count - 1);

            return showFlags;
        }

        private ShowFlags ShowMesh(AnimationSourceMesh source, int s, int e, ref int meshCount)
        {
            if (source.Bones.Count < 2) return ShowFlags.None;

            var boneDistances = new float[source.Bones.Count];
            var boneTotalDistance = 0f;
            var pp = Vector3.Zero;

            for (var i = 0; i < source.Bones.Count; i++)
            {
                if (source.Selection.Geometry.Origin.GetNodeTransform(source.Bones[i], null, 0, out var boneTransform))
                {
                    if (i == 0)
                    {
                        pp = boneTransform.Translation;
                    }
                    else
                    {
                        var p = boneTransform.Translation;
                        var d = Vector3.Distance(p, pp);
                        boneTotalDistance += d;
                        boneDistances[i] = boneTotalDistance;
                        pp = p;
                    }
                }
                else return ShowFlags.None;
            }
            var pointDistances = new float[e - s + 1];
            var pointTotalDistance = 0f;
            pp = _points[s].point;
            for (var i = s + 1; i <= e; i++)
            {
                var d = Vector3.Distance(_points[i].point, pp);
                pointTotalDistance += d;
                pointDistances[i - s] = pointTotalDistance;
                pp = _points[i].point;
            }
            
            var seperation = Origin.RepeatScale > 0 ? Math.Max((int)(pointTotalDistance * Origin.RepeatScale / boneTotalDistance), 1) : 1;

            var pc = 0;

            var ap = source.Selection.Animation != null ? source.Selection.Loop.GetProgress(_progress / source.Selection.Animation.Duration) * source.Selection.Animation.Duration : 0;

            for (var sc = 0; sc < seperation; sc++)
            {
                MeshInstance instance;
                if (meshCount >= _meshInstances.Count)
                {
                    instance = new MeshInstance(source.Selection.Geometry.Origin);
                    _meshInstances.Add(instance);
                }
                else if (_meshInstances[meshCount].Geometry != source.Selection.Geometry.Origin) {
                    instance = new MeshInstance(source.Selection.Geometry.Origin);
                    _meshInstances[meshCount] = instance;
                }
                else instance = _meshInstances[meshCount];

                instance.Skin = source.Selection;
                instance.FrameDivision = source.Selection.FrameDivision;
                instance.Animation = source.Selection.Animation?.Origin;
                instance.Progress = ap;
                meshCount++;

                for (var i = 0; i < source.Bones.Count; i++)
                {
                    var pointDistance = pointTotalDistance * (boneDistances[i] + boneTotalDistance * sc) / (boneTotalDistance * seperation);

                    while (pc < pointDistances.Length - 2 && pointDistance > pointDistances[pc + 1]) pc++;

                    var tp = (pointDistance - pointDistances[pc]) / (pointDistances[pc + 1] - pointDistances[pc]);

                    instance.SetCustomTransform(source.Bones[i], Matrix4x4.Lerp(_points[pc + s].transform, _points[pc + s + 1].transform, tp));
                }
            }

            return source.Selection.ShowFlags;
        }

        private void DrawImage(Graphics graphics, AnimationSourceImage source, int s, int e)
        {
            var image = source.GetImage(_progress);
            var texture = image?.Content.Texture;
            if (texture == null) return;
            var frame = image.Frame;
            if (frame.Width < 2) return;

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Triangles);
            command.State.Material.ColorMap = texture;

            var vi = 0;
            for (int i = s; i < e; i++)
            {
                command.AddIndex(vi + 0);
                command.AddIndex(vi + 2);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 2);
                command.AddIndex(vi + 3);
                command.AddIndex(vi + 1);
                vi += 2;
            }

            var h = (float)frame.Height * image.ContentScale / 2;
            var d = (float)frame.Left / texture.Description.Width;
            var tv = (float)frame.Top / texture.Description.Height;
            var bv = (float)frame.Bottom / texture.Description.Height;

            for (var i = s; i <= e; i++)
            {
                var u = Origin.RepeatScale > 0 ? d : (frame.X + (float)frame.Width * (i - s) / (e - s)) / texture.Description.Width + d;

                var p = _points[i];

                var color = Origin.Color.GetColor(Origin.ColorDuration != 0 ? Origin.ColorLoop.GetProgress((_progress - p.progress) / Origin.ColorDuration) : 1, _colorRandom);

                var pu = p.transform.Up() * h;

                var pn = Vector3.Zero;
                var pt = Vector3.Zero;
                if (command.State.UsingVertexNormal)
                {
                    pn = Vector3.Normalize(p.transform.Right());
                    if (Vector3.Dot(pn, graphics.Camera.Forward) < 0) pn = -pn;
                    pt = Vector3.Normalize(p.transform.Up());
                }
                command.AddVertex(new FVertex(p.point + pu, color, new Vector2(u, tv), pn, pt));
                command.AddVertex(new FVertex(p.point - pu, color, new Vector2(u, bv), pn, pt));

                if (Origin.RepeatScale > 0 && i < e) d += Vector3.Distance(p.point, _points[i + 1].point) / (texture.Description.Width * image.ContentScale * Origin.RepeatScale);
            }
            graphics.Command(command);
        }

        private void DrawImageCursor(Graphics graphics, AnimationSourceImage source, int s, int e)
        {
            var image = source.GetImage(_progress);
            if (image == null) return;
            var frame = image.Frame;
            if (frame.Width < 2) return;

            var command = new StreamRenderCommand(graphics, PrimitiveMode.Triangles);

            command.State.PolygonMode = PolygonMode.Line;
            command.State.Material.Shader = MaterialShader.NoLight;

            var vi = 0;
            for (int i = s; i < e; i++)
            {
                command.AddIndex(vi + 0);
                command.AddIndex(vi + 2);
                command.AddIndex(vi + 1);
                command.AddIndex(vi + 2);
                command.AddIndex(vi + 3);
                command.AddIndex(vi + 1);
                vi += 2;
            }

            var h = frame.Height * image.ContentScale / 2;

            for (var i = s; i <= e; i++)
            {
                var p = _points[i];

                var pu = p.transform.Up() * h;

                command.AddVertex(new FVertex(p.point + pu));
                command.AddVertex(new FVertex(p.point - pu));
            }
            graphics.Command(command);
        }

        private void DrawImages(Graphics graphics, InstanceLayer layer)
        {
            var s = 0;

            while (s < _points.Count)
            {
                var link = _points[s].link;

                var e = s;

                while (e < _points.Count - 1 && _points[e + 1].link == link) e++;

                if (s < e)
                {
                    var source = Origin.Sources[link % Origin.Sources.Count];

                    switch (source.Type)
                    {
                        case AnimationSourceType.Image:
                            {
                                var imageSource = (AnimationSourceImage)source;
                                if (layer == InstanceLayer.Cursor)
                                {
                                    DrawImageCursor(graphics, imageSource, s, e);
                                }
                                else if (imageSource.Material.Apply(graphics, _progress, _materialRandom, layer, true))
                                {
                                    DrawImage(graphics, imageSource, s, e);
                                    graphics.Pop();
                                }
                            }
                            break;
                    }
                }
                s = e + 1;
            }
        }

        private void DrawMeshes(Graphics graphics, InstanceLayer layer)
        {
            graphics.PushColor();
            graphics.Color = Origin.Color.GetColor(Origin.ColorDuration != 0 ? Origin.ColorLoop.GetProgress(_progress / Origin.ColorDuration) : 1, _colorRandom);
            foreach (var mi in _meshInstances)
            {
                mi.Draw(graphics, layer, _progress, _materialRandom);
            }
            graphics.PopColor();
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (Origin.Sources.Count != 0 && layer != InstanceLayer.Shadow2D)
            {
                DrawImages(graphics, layer);
                DrawMeshes(graphics, layer);
            }
        }

        public override string GetStatus() => $"trail count:{_points.Count}";

        internal override void Select(bool focus)
        {
            if (_mouseTesting != MouseTesting.None)
            {
                _points.Clear();
                _mouseTesting = MouseTesting.End;
            }
        }

        private void AddPoint(MouseEventArgs e)
        {
            var scene = Parent.GetAncestor<Scene>(false);
            var ray = Scene.Camera.PickRay(new Vector2(e.X, e.Y));
            if (scene == null || !scene.Intersects(ray, null, false, CollisionFlags.None, out _, out float d, out _) || d > MouseTestMaxDistance) d = MouseTestMaxDistance;
            var pos = ray.Position + ray.Direction * d;
            pos.Z += 1;     //avoid depth culling
            AddPoint(pos);
        }

        internal override bool KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _mouseTesting != MouseTesting.None)
            {
                _points.Clear();
                _mouseTesting = MouseTesting.End;
                return true;
            }
            return base.KeyDown(e);
        }

        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey && !shiftKey)
            {
                _points.Clear();
                _link++;
                AddPoint(e);
                _mouseTesting = MouseTesting.AddNew;
                return true;
            }

            return base.MouseDown(e, controlKey, shiftKey);
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            if (e.Button == MouseButtons.Left && !controlKey && !shiftKey && Parent != null && _mouseTesting != MouseTesting.None)
            {
                if (!Origin.Emission)
                {
                    while (_points.Count > 1) _points.RemoveAt(_points.Count - 1);
                }
                AddPoint(e);
                _mouseTesting = MouseTesting.AddNew;
                return true;
            }
            return base.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        }

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
