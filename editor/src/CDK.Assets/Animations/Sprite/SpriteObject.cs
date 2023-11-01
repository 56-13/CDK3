using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations.Sprite
{
    public class SpriteObject : SceneObject
    {
        public Sprite Origin { private set; get; }

        private SpriteTimeline _SelectedTimeline;
        public SpriteTimeline SelectedTimeline
        {
            set
            {
                if (SetProperty(ref _SelectedTimeline, value, false)) SelectedElement = null;
            }
            get => _SelectedTimeline;
        }

        private SpriteElement _SelectedElement;
        public SpriteElement SelectedElement
        {
            set => SetProperty(ref _SelectedElement, value, false);
            get => _SelectedElement;
        }

        private List<SpriteTimeline>[] _timelines;
        private Matrix4x4 _transform;
        private float _progress;
        private float _clippedProgress;
        private int _randomSeed;
        private bool _visible;
        private bool _cursor;
        private bool _mousePicked;

        internal Dictionary<SpriteElement, Instance> MeshInstances { private set; get; }

        public SpriteObject()
        {
            Origin = new Sprite(this);

            Init();
        }

        public SpriteObject(Sprite origin)
        {
            Origin = origin;
            Origin.AddWeakRefresh(Origin_Refresh);

            Init();
        }

        public SpriteObject(SpriteObject other, bool binding) : base(other, binding, true)
        {
            if (binding)
            {
                Origin = other.Origin;
                Origin.AddWeakRefresh(Origin_Refresh);
            }
            else Origin = new Sprite(this, other.Origin);

            Init();
        }

        private void Origin_Refresh(object sender, EventArgs e)
        {
            IsDirty = true;
            OnRefresh();
        }

        private void Init()
        {
            _timelines = new List<SpriteTimeline>[2];
            _timelines[0] = new List<SpriteTimeline>();
            _timelines[1] = new List<SpriteTimeline>();
            _randomSeed = RandomUtil.Next();

            MeshInstances = new Dictionary<SpriteElement, Instance>();

            ResetTimelines();
        }

        public override SceneComponentType Type => SceneComponentType.Sprite;
        public override SceneComponent Clone(bool binding) => new SpriteObject(this, binding);
        internal override bool AddAABB(ref ABoundingBox result)
        {
            if (GetTransform(_progress, null, out var transform)) return false;

            var param = new SpriteElement.TransformParam
            {
                Parent = this,
                Transform = transform
            };
            var flag = false;
            foreach (var timeline in _timelines[_cursor ? 1 : 0])
            {
                if (timeline.Reset) param.Transform = transform;
                param.Duration = timeline.Duration;
                param.Progress = (_clippedProgress - timeline.StartTime) / param.Duration;

                var random = new Random(_randomSeed);
                foreach (var element in timeline.Elements)
                {
                    param.Random = random.Next();
                    flag |= element.AddAABB(ref param, ref result);
                }
            }
            return flag;
        }

        internal override void AddCollider(ref Collider result)
        {
            if (GetTransform(_progress, null, out var transform)) return;

            var param = new SpriteElement.TransformParam
            {
                Parent = this,
                Transform = transform
            };
            foreach (var timeline in _timelines[_cursor ? 1 : 0])
            {
                if (timeline.Reset) param.Transform = transform;
                param.Duration = timeline.Duration;
                param.Progress = (_clippedProgress - timeline.StartTime) / param.Duration;

                var random = new Random(_randomSeed);
                foreach (var element in timeline.Elements)
                {
                    param.Random = random.Next();
                    element.AddCollider(ref param, ref result);
                }
            }
        }

        public override bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (!base.GetTransform(progress, null, out result)) return false;
            if (Origin.Billboard) Scene.Camera.View.Billboard(result, out result);
            if (name != null) return true;

            List<SpriteTimeline> timelines;
            float clippedProgress;

            if (MathUtil.NearEqual(_progress, progress))
            {
                timelines = _timelines[_cursor ? 1 : 0];
                clippedProgress = _clippedProgress;
            }
            else
            {
                timelines = new List<SpriteTimeline>();
                clippedProgress = ClipProgress(progress);
                GetTimelines(clippedProgress, timelines);
            }

            var transform = result;
            var param = new SpriteElement.TransformParam
            {
                Parent = this,
                Transform = transform
            };
            foreach (var timeline in timelines)
            {
                if (timeline.Reset) param.Transform = transform;
                param.Duration = timeline.Duration;
                param.Progress = (clippedProgress - timeline.StartTime) / param.Duration;

                var random = new Random(_randomSeed);
                foreach (var element in timeline.Elements)
                {
                    param.Random = random.Next();
                    if (element.GetTransform(ref param, name, ref result)) return true;
                }
            }
            return false;
        }
        public override float GetDuration(DurationParam param, float duration = 0) => Origin.TotalDuration;
        public override float Progress => _progress;
        internal override bool AfterCameraUpdate() => Origin.Billboard || base.AfterCameraUpdate();

        private bool ClipProgress()
        {
            var clippedProgress = ClipProgress(_progress);
            if (_clippedProgress != clippedProgress)
            {
                _clippedProgress = clippedProgress;
                return true;
            }
            return false;
        }

        private float ClipProgress(float progress)
        {
            var result = 0f;
            var duration = Origin.SingleDuration;
            if (duration != 0) result = Origin.Loop.GetProgress(progress / duration) * duration;
            return result;
        }

        private void ResetTimelines()
        {
            _cursor = !_cursor;
            GetTimelines(_clippedProgress, _timelines[_cursor ? 1 : 0]);
        }

        private void GetTimelines(float progress, List<SpriteTimeline> result) 
        {
            result.Clear();

            if (Origin.Timelines.Count != 0)
            {
                foreach (var timeline in Origin.Timelines)
                {
                    if (timeline.StartTime <= progress && progress <= timeline.EndTime)
                    {
                        int i = 0;
                        while (i < result.Count)
                        {
                            if (timeline.Layer >= result[i].Layer) i++;
                            else break;
                        }
                        result.Insert(i, timeline);
                    }
                }
            }
        }

        internal override void Rewind()
        {
            _progress = 0;
            _clippedProgress = 0;
            _randomSeed = RandomUtil.Next();
            ResetTimelines();
        }

        protected override UpdateState OnUpdate(LightSpace lightSpace, float delta, bool alive, ref UpdateFlags flags)
        {
            var prevClippedProgress = _clippedProgress;
            _progress += delta;

            var flag = true;
            if (ClipProgress())
            {
                ResetTimelines();
                flag = _timelines[_cursor ? 1 : 0].SequenceEqual(_timelines[_cursor ? 0 : 1]);
            }
            if (flag)
            {
                var inflags = flags;
                if (Origin.Billboard && (inflags & UpdateFlags.View) != 0) inflags |= UpdateFlags.Transform;

                var param = new SpriteElement.TransformUpdatedParam
                {
                    Parent = this,
                    Inflags = inflags
                };
                foreach (var timeline in _timelines[_cursor ? 1 : 0]) {
                    if (timeline.Reset) param.Inflags = inflags;
                    param.Duration = timeline.Duration;
                    param.Progress0 = (prevClippedProgress - timeline.StartTime) / param.Duration;
                    param.Progress1 = (_clippedProgress - timeline.StartTime) / param.Duration;
                    foreach (var element in timeline.Elements) element.GetTransformUpdated(ref param, ref flags);
                }
            }
            else flags |= UpdateFlags.Transform | UpdateFlags.AABB;

            if (Origin.Loop.Count == 0) return UpdateState.None;
            else if (_progress < Origin.SingleDuration * Origin.Loop.Count) return alive ? UpdateState.Alive : UpdateState.Finishing;
            else return Origin.Loop.Finish ? UpdateState.Stopped : UpdateState.None;
        }

        internal override ShowFlags Show()
        {
            var showFlags = ShowFlags.None;
            var timelines = _timelines[_cursor ? 1 : 0];
            foreach (var timeline in timelines)
            {
                foreach (var element in timeline.Elements) showFlags |= element.ShowFlags;
            }
            _visible = timelines.Count != 0 && GetTransform(_progress, null, out _transform);
            return showFlags;
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer)
        {
            if (_visible)
            {
                var prev = graphics.World;
                graphics.World = _transform * prev;
                graphics.Push();

                var param = new SpriteElement.DrawParam
                {
                    Graphics = graphics,
                    Layer = layer,
                    Parent = this
                };
                foreach (var timeline in _timelines[_cursor ? 1 : 0])
                {
                    if (timeline.Reset) graphics.Reset();

                    if (layer != InstanceLayer.Cursor || _SelectedTimeline == null || _SelectedTimeline == timeline)
                    {
                        param.Duration = timeline.Duration;
                        param.Progress = (_clippedProgress - timeline.StartTime) / param.Duration;

                        var random = new Random(_randomSeed);
                        foreach (var element in timeline.Elements)
                        {
                            param.Random = random.Next();
                            param.Visible = layer != InstanceLayer.Cursor || _SelectedElement == null || _SelectedElement == element;
                            element.Draw(ref param);
                        }
                    }
                }
                graphics.Pop();
                graphics.World = prev;
            }
        }

        internal override void Select(bool focus)
        {
            _mousePicked = false;
        }

        internal override bool KeyDown(KeyEventArgs e) => (_SelectedElement != null && _SelectedElement.KeyDown(this, e)) || base.KeyDown(e);
        internal override bool KeyUp(KeyEventArgs e) => (_SelectedElement != null && _SelectedElement.KeyUp(this, e)) || base.KeyUp(e);
        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            _mousePicked = false;

            if (_SelectedElement != null && _SelectedElement.MouseDown(this, e, controlKey, shiftKey)) return true;

            if (e.Button == MouseButtons.Left && controlKey && !shiftKey && GetTransform(out var transform))
            {
                SpriteElement selection = null;

                var ray = Scene.Camera.PickRay(new Vector2(e.X, e.Y));

                var distance = float.MaxValue;

                var param = new SpriteElement.TransformParam
                {
                    Parent = this,
                    Transform = transform
                };
                foreach (var timeline in _timelines[_cursor ? 1 : 0])
                {
                    if (timeline.Reset) param.Transform = transform;
                    param.Duration = timeline.Duration;
                    param.Progress = (_clippedProgress - timeline.StartTime) / param.Duration;

                    var random = new Random(_randomSeed);
                    foreach (var element in timeline.Elements)
                    {
                        param.Random = random.Next();
                        if (element.Pick(ref param, ray, ref distance)) selection = element;
                    }
                }
                if (selection != null) 
                { 
                    SelectedElement = selection;
                    _mousePicked = true;
                    return true;
                }
            }
            return false;
        }

        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            var result = _mousePicked && _SelectedElement != null && _SelectedElement.MouseUp(this, e, controlKey, shiftKey);

            _mousePicked = false;

            return result || base.MouseUp(e, controlKey, shiftKey);
        }

        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey)
        {
            return (_mousePicked && _SelectedElement != null && _SelectedElement.MouseMove(this, e, prevX, prevY, controlKey, shiftKey)) || base.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        }

        internal override bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey)
        {
            return (_SelectedElement != null && _SelectedElement.MouseWheel(this, e, controlKey, shiftKey)) || base.MouseWheel(e, controlKey, shiftKey);
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
