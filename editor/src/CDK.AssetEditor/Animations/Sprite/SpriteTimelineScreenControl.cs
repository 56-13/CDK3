using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using MathUtil = CDK.Drawing.MathUtil;

namespace CDK.Assets.Animations.Sprite
{
    public partial class SpriteTimelineScreenControl : Control
    {
        private SpriteObject _Sprite;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteObject Sprite
        {
            set
            {
                if (_Sprite != value)
                {
                    if (_Sprite != null)
                    {
                        _Sprite.PropertyChanged -= Sprite_PropertyChanged;
                        _Sprite.Origin.Timelines.ListChanged -= Sprite_Timelines_ListChanged;
                    }

                    _Sprite = value;

                    if (_Sprite != null)
                    {
                        _durationScale = Math.Max((int)_Sprite.Origin.SingleDuration, 1);
                        if (Parent != null) FitDurationScale();

                        _Sprite.PropertyChanged += Sprite_PropertyChanged;
                        _Sprite.Origin.Timelines.ListChanged += Sprite_Timelines_ListChanged;
                    }

                    Invalidate();

                    SpriteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Sprite;
        }

        public event EventHandler SpriteChanged;

        private int _contextMenuCount;
        private int _durationScale;
        private int _mouseX;
        private int _mouseY;

        private enum TimelineCursor
        {
            Body,
            Start,
            End
        }
        private TimelineCursor _cursor;
        private bool _candidateSelected;

        private class Candidate
        {
            public SpriteTimeline timeline;
            public TimelineCursor cursor;

            public Candidate(SpriteTimeline timeline, TimelineCursor cursor)
            {
                this.timeline = timeline;
                this.cursor = cursor;
            }
        }

        private Control _parent;


        private const int LayerCount = 10;
        private const int DefaultDurationScale = 10;
        private const int MinGridWidth = 20;
        private const int PointRadius = 5;
        private const int PointRadius2 = PointRadius * 2;
        private const int PaddingX = 20;
        private const int PaddingX2 = PaddingX + PaddingX;
        private const int PaddingY = 20;

        public SpriteTimelineScreenControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            InitializeComponent();

            _contextMenuCount = contextMenuStrip.Items.Count;

            _durationScale = DefaultDurationScale;

            Disposed += SpriteTimelineControl_Disposed;
        }

        private void SpriteTimelineControl_Disposed(object sender, EventArgs e)
        {
            if (_Sprite != null)
            {
                _Sprite.PropertyChanged -= Sprite_PropertyChanged;
                _Sprite.Origin.Timelines.ListChanged -= Sprite_Timelines_ListChanged;
            }
            if (_parent != null) _parent.SizeChanged -= Parent_SizeChanged;
        }

        private void Sprite_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "SelectedTimeline":
                    Invalidate();
                    break;
            }
        }

        private void Sprite_Timelines_ListChanged(object sender, ListChangedEventArgs e)
        {
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (_parent != null) _parent.SizeChanged -= Parent_SizeChanged;
            _parent = Parent;
            if (_parent != null)
            {
                FitDurationScale();

                _parent.SizeChanged += Parent_SizeChanged;
            }

            base.OnParentChanged(e);
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            FitDurationScale();
        }

        private void FitDurationScale()
        {
            if (AssetManager.IsCreated)
            {
                Width = PaddingX2 + Math.Max(Parent.Width - PaddingX2, 0) * SpriteTimeline.MaxDuration / _durationScale;
            }
        }

        private int TimeToX(float time)
        {
            return (int)(PaddingX + (Width - PaddingX2) * time / SpriteTimeline.MaxDuration);
        }

        private float XToTime(int x)
        {
            return MathUtil.Clamp((float)(x - PaddingX) * SpriteTimeline.MaxDuration / (Width - PaddingX2), 0, SpriteTimeline.MaxDuration);
        }

        private float DistanceToTime(int d)
        {
            return (float)d * SpriteTimeline.MaxDuration / (Width - PaddingX2);
        }

        private int LayerToY(int layer)
        {
            return (int)(PaddingY + (layer + 0.5f) * (Height - PaddingY) / LayerCount);
        }

        private int YToLayer(int y)
        {
            return MathUtil.Clamp((y - PaddingY) * LayerCount / (Height - PaddingY), 0, LayerCount - 1);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Width < PaddingX2 || Height < PaddingY + LayerCount * PointRadius2) return;

            var gxc = SpriteTimeline.MaxDuration * 10;
            if ((Width - PaddingX2) / gxc < MinGridWidth) gxc = SpriteTimeline.MaxDuration;

            for (var i = 0; i <= gxc; i++)
            {
                var gx = PaddingX + i * (Width - PaddingX2) / gxc;
                pe.Graphics.DrawLine(Pens.LightGray, gx, PaddingY, gx, Height);
            }

            for (var i = 0; i < LayerCount; i++)
            {
                var gy = LayerToY(i);

                pe.Graphics.DrawLine(Pens.LightGray, PaddingX, gy, Width - PaddingX, gy);
            }

            for (var i = 1; i <= SpriteTimeline.MaxDuration; i++)
            {
                var x = TimeToX(i);
                var str = i.ToString();
                var sw = pe.Graphics.MeasureString(str, Font).Width;
                pe.Graphics.DrawString(str, Font, Brushes.LightGray, x - sw / 2, 5);
            }

            if (_Sprite != null)
            {
                foreach (var timeline in _Sprite.Origin.Timelines)
                {
                    var sx = TimeToX(timeline.StartTime) + PointRadius;
                    var ex = TimeToX(timeline.EndTime) - PointRadius;

                    if (sx < ex)
                    {
                        var y = LayerToY(timeline.Layer);

                        if (_Sprite.SelectedTimeline == timeline)
                        {
                            if (_cursor != TimelineCursor.Body)
                            {
                                var sc = _cursor == TimelineCursor.Start ? Color.LightGreen : Color.DarkGreen;
                                var ec = _cursor == TimelineCursor.End ? Color.LightGreen : Color.DarkGreen;

                                using (var brush = new LinearGradientBrush(new Point(sx, y), new Point(ex, y), sc, ec))
                                {
                                    pe.Graphics.FillRectangle(brush, sx, y - 1, ex - sx, 3);
                                }
                            }
                            else pe.Graphics.FillRectangle(Brushes.LightGreen, sx, y - 1, ex - sx, 3);
                        }
                        else pe.Graphics.FillRectangle(Brushes.DarkGreen, sx, y - 1, ex - sx, 3);
                    }
                }
                foreach (var timeline in _Sprite.Origin.Timelines)
                {
                    var sx = TimeToX(timeline.StartTime);
                    var y = LayerToY(timeline.Layer);

                    var brush = _Sprite.SelectedTimeline == timeline && _cursor != TimelineCursor.End ? Brushes.LightGreen : Brushes.DarkGreen;

                    pe.Graphics.FillEllipse(brush, sx - PointRadius, y - PointRadius, PointRadius2, PointRadius2);
                }
                foreach (var timeline in _Sprite.Origin.Timelines)
                {
                    var ex = TimeToX(timeline.EndTime);
                    var y = LayerToY(timeline.Layer);

                    var pen = _Sprite.SelectedTimeline == timeline && _cursor != TimelineCursor.Start ? Pens.LightGreen : Pens.DarkGreen;

                    pe.Graphics.DrawEllipse(pen, ex - PointRadius, y - PointRadius, PointRadius2, PointRadius2);
                }
            }
            base.OnPaint(pe);
        }

        private void ClearCandidates()
        {
            while (contextMenuStrip.Items.Count > _contextMenuCount) contextMenuStrip.Items.RemoveAt(0);
        }

        private void AddTimeline(int x, int y)
        {
            var layer = YToLayer(y);
            var startTime = XToTime(x);
            var endTime = startTime + SpriteTimeline.MinDuration;
            if (endTime > SpriteTimeline.MaxDuration)
            {
                endTime = SpriteTimeline.MaxDuration;
                startTime = endTime - SpriteTimeline.MinDuration;
            }
            if (!_Sprite.Origin.IsCollided(null, layer, startTime, endTime, out _))
            {
                var timeline = new SpriteTimeline(layer, startTime, endTime);
                _Sprite.Origin.Timelines.Add(timeline);
                _Sprite.SelectedTimeline = timeline;
                _cursor = TimelineCursor.Body;
            }
        }

        private void MoveTimeline(SpriteTimeline timeline, int layer, int xdiff)
        {
            var timeDiff = DistanceToTime(xdiff);

            switch (_cursor)
            {
                case TimelineCursor.Body:
                    timeline.Move(layer, timeDiff);
                    break;
                case TimelineCursor.Start:
                    timeline.StartTime += timeDiff;
                    break;
                case TimelineCursor.End:
                    timeline.EndTime += timeDiff;
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            if (_Sprite != null && e.Button == MouseButtons.Left)
            {
                ClearCandidates();

                var layer = YToLayer(e.Y);
                var time = XToTime(e.X);

                var candidates = new List<Candidate>();

                foreach (var timeline in _Sprite.Origin.Timelines.Where(t => t.Layer == layer && e.X >= TimeToX(t.StartTime) - PointRadius && e.X <= TimeToX(t.EndTime) + PointRadius))
                {
                    if (Math.Abs(e.X - TimeToX(timeline.StartTime)) < PointRadius) candidates.Add(new Candidate(timeline, TimelineCursor.Start));
                    if (Math.Abs(e.X - TimeToX(timeline.EndTime)) < PointRadius) candidates.Add(new Candidate(timeline, TimelineCursor.End));
                    candidates.Add(new Candidate(timeline, TimelineCursor.Body));
                }

                if (candidates.Count > 0)
                {
                    if (candidates.Count > 1)
                    {
                        var i = 0;
                        foreach (var candiate in candidates)
                        {
                            var selectToolStripMenuItem = new ToolStripMenuItem
                            {
                                Tag = candiate,
                                Text = $"{candiate.timeline.StartTime:F3} ~ {candiate.timeline.EndTime:F3} ({candiate.cursor})"
                            };
                            selectToolStripMenuItem.Click += SelectToolStripMenuItem_Click;
                            contextMenuStrip.Items.Insert(i++, selectToolStripMenuItem);
                        }
                    }
                    if (!_candidateSelected)
                    {
                        _Sprite.SelectedTimeline = candidates[0].timeline;
                        _cursor = candidates[0].cursor;
                        Invalidate();
                    }
                }
                _candidateSelected = false;
            }

            _mouseX = e.X;
            _mouseY = e.Y;

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_Sprite != null && e.Button == MouseButtons.Left && _Sprite.SelectedTimeline != null)
            {
                MoveTimeline(_Sprite.SelectedTimeline, YToLayer(e.Y), e.X - _mouseX);

                Refresh();
            }

            _mouseX = e.X;
            _mouseY = e.Y;

            base.OnMouseMove(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_Sprite != null && e.Button == MouseButtons.Left) AddTimeline(e.X, e.Y);

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;

            _durationScale = MathUtil.Clamp(_durationScale - Math.Sign(e.Delta), 1, SpriteTimeline.MaxDuration);
            FitDurationScale();

            Invalidate();

            base.OnMouseWheel(e);
        }

        private void SelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sprite != null)
            {
                var selection = (Candidate)((ToolStripMenuItem)sender).Tag;

                _Sprite.SelectedTimeline = selection.timeline;
                _cursor = selection.cursor;
                _candidateSelected = true;

                Invalidate();

                ClearCandidates();
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTimeline(_mouseX, _mouseY);
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null)
            {
                _Sprite.Origin.Timelines.Remove(timeline);

                _Sprite.SelectedTimeline = null;
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null) AssetManager.Instance.Clip(timeline, false);
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null) AssetManager.Instance.Clip(timeline, true);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Sprite != null && AssetManager.Instance.ClipObject is SpriteTimeline timeline)
            {
                var layer = YToLayer(_mouseY);
                var time = XToTime(_mouseX);

                if (AssetManager.Instance.ClipCut)
                {
                    if (timeline.Parent == _Sprite.Origin) timeline.Move(layer, time - timeline.StartTime);
                    else _Sprite.Origin.Timelines.Add(timeline);

                    AssetManager.Instance.ClearClip();
                }
                else
                {
                    timeline = new SpriteTimeline(layer, time, timeline);
                    _Sprite.Origin.Timelines.Add(timeline);
                }

                if (timeline.Parent == _Sprite.Origin)      //if added
                {
                    _Sprite.SelectedTimeline = timeline;
                    _cursor = TimelineCursor.Body;
                }

                ClearCandidates();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null) 
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        MoveTimeline(timeline, timeline.Layer, -1);
                        break;
                    case Keys.Right:
                        MoveTimeline(timeline, timeline.Layer, 1);
                        break;
                    case Keys.Up:
                        if (timeline.Layer > 0) timeline.Move(timeline.Layer - 1, 0);
                        break;
                    case Keys.Down:
                        if (timeline.Layer < LayerCount - 1) timeline.Move(timeline.Layer + 1, 0);
                        break;
                    case Keys.Escape:
                        _Sprite.SelectedTimeline = null;
                        break;
                }
            }

            base.OnKeyDown(e);
        }
    }
}
