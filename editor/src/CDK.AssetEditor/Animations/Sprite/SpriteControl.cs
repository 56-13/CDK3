using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

using CDK.Assets.Scenes;
using CDK.Assets.Components;

using CDK.Assets.Animations.Sprite.Elements;

namespace CDK.Assets.Animations.Sprite
{
    public partial class SpriteControl : UserControl, ICollapsibleControl, ISplittableControl, ISceneBottomControlProvider
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
                        loopControl.DataBindings.Clear();
                        billboardCheckBox.DataBindings.Clear();
                        listBox.DataBindings.Clear();

                        _Sprite.PropertyChanged -= Sprite_PropertyChanged;
                    }

                    _Sprite = value;

                    if (_Sprite != null)
                    {
                        loopControl.DataBindings.Add("Loop", _Sprite.Origin, "Loop", false, DataSourceUpdateMode.OnPropertyChanged);
                        billboardCheckBox.DataBindings.Add("Checked", _Sprite.Origin, "Billboard", false, DataSourceUpdateMode.OnPropertyChanged);
                        listBox.DataBindings.Add("SelectedItem", _Sprite, "SelectedElement", true, DataSourceUpdateMode.OnPropertyChanged);

                        _Sprite.PropertyChanged += Sprite_PropertyChanged;
                    }

                    ResetSelectedTimeline();

                    ResetSelectedElement();

                    SpriteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Sprite;
        }

        public event EventHandler SpriteChanged;

        private List<Control> _elementControls;

        public SpriteControl()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(SpriteElementType)))
            {
                var addSubToolStripMenuItem = new ToolStripMenuItem
                {
                    Tag = type,
                    Text = type.ToString()
                };
                addSubToolStripMenuItem.Click += AddSubToolStripMenuItem_Click;
                addToolStripMenuItem.DropDownItems.Add(addSubToolStripMenuItem);
            }

            _elementControls = new List<Control>();

            Disposed += SpriteControl_Disposed;
        }

        private void SpriteControl_Disposed(object sender, EventArgs e)
        {
            foreach (var control in _elementControls) control.Dispose();

            if (_Sprite != null)
            {
                _Sprite.PropertyChanged -= Sprite_PropertyChanged;
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }

        public Control GetBottomControl() => new SpriteTimelineControl() { Sprite = _Sprite };

        private void Sprite_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedTimeline":
                    ResetSelectedTimeline();
                    break;
                case "SelectedElement":
                    ResetSelectedElement();
                    break;
            }
        }

        private void ResetSelectedTimeline()
        {
            var timeline = _Sprite?.SelectedTimeline;

            layerUpDown.DataBindings.Clear();
            startTimeUpDown.DataBindings.Clear();
            endTimeUpDown.DataBindings.Clear();
            resetCheckBox.DataBindings.Clear();

            if (timeline != null)
            {
                timelinePanel.Visible = true;

                layerUpDown.DataBindings.Add("Value", timeline, "Layer", false, DataSourceUpdateMode.OnPropertyChanged);
                startTimeUpDown.DataBindings.Add("Value", timeline, "StartTime", false, DataSourceUpdateMode.OnPropertyChanged);
                endTimeUpDown.DataBindings.Add("Value", timeline, "EndTime", false, DataSourceUpdateMode.OnPropertyChanged);
                resetCheckBox.DataBindings.Add("Checked", timeline, "Reset", false, DataSourceUpdateMode.OnPropertyChanged);
            }
            else
            {
                timelinePanel.Visible = false;
            }

            listBox.DataSource = timeline?.Elements;
            listBox.DisplayMember = "Type";
            listBox.SelectedItem = null;
        }

        private void ResetSelectedElement()
        {
            var element = (SpriteElement)listBox.SelectedItem;

            Control newControl = null;

            if (element != null)
            {
                switch (element.Type)
                {
                    case SpriteElementType.Image:
                        newControl = GetElementControl<ImageSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Mesh:
                        newControl = GetElementControl<MeshSpriteElementControl>(element);
                        break;
                    case SpriteElementType.String:
                        //TODO
                        break;
                    case SpriteElementType.Line:
                        newControl = GetElementControl<LineSpriteElementControl>(element);
                        break;
                    case SpriteElementType.GradientLine:
                        newControl = GetElementControl<GradientLineSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Rect:
                        newControl = GetElementControl<RectSpriteElementControl>(element);
                        break;
                    case SpriteElementType.GradientRect:
                        newControl = GetElementControl<GradientRectSpriteElementControl>(element);
                        break;
                    case SpriteElementType.RoundRect:
                        //TODO
                        break;
                    case SpriteElementType.GradientRoundRect:
                        //TODO
                        break;
                    case SpriteElementType.Arc:
                        newControl = GetElementControl<ArcSpriteElementControl>(element);
                        break;
                    case SpriteElementType.GradientArc:
                        newControl = GetElementControl<GradientArcSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Box:
                        newControl = GetElementControl<BoxSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Sphere:
                        newControl = GetElementControl<SphereSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Capsule:
                        //TODO
                        break;
                    case SpriteElementType.Cylinder:
                        //TODO
                        break;
                    case SpriteElementType.Extern:
                        //TODO
                        break;
                    case SpriteElementType.Translate:
                        newControl = GetElementControl<TranslateSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Rotate:
                        newControl = GetElementControl<RotateSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Scale:
                        newControl = GetElementControl<ScaleSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Invert:
                        newControl = GetElementControl<InvertSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Color:
                        newControl = GetElementControl<ColorSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Brightness:
                        newControl = GetElementControl<BrightnessSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Contrast:
                        newControl = GetElementControl<ContrastSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Saturation:
                        newControl = GetElementControl<SaturationSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Blur:
                        newControl = GetElementControl<BlurSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Lens:
                        newControl = GetElementControl<LensSpriteElementControl>(element);
                        break;
                    case SpriteElementType.Wave:
                        newControl = GetElementControl<WaveSpriteElementControl>(element);
                        break;
                }
            }

            if (elementPanel.Controls.Count == 0 || newControl != elementPanel.Controls[0])
            {
                elementPanel.SuspendLayout();
                elementPanel.Controls.Clear();

                if (newControl != null)
                {
                    if (newControl is ISplittableControl splittableControl) splittableControl.Splitted = _Splitted;

                    elementPanel.Visible = true;
                    elementPanel.Height = newControl.Height;
                    newControl.Width = elementPanel.Width;
                    newControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    elementPanel.Controls.Add(newControl);
                }
                else elementPanel.Visible = false;

                elementPanel.ResumeLayout();
            }
        }

        private C GetElementControl<C>(SpriteElement e) where C : Control, new()
        {
            C control = null;

            foreach (var otherControl in _elementControls)
            {
                if (otherControl.Parent == null && otherControl is C c)
                {
                    control = c;
                    break;
                }
            }
            if (control == null)
            {
                control = new C();
                control.SizeChanged += ElementControl_SizeChanged;
                control.ParentChanged += ElementControl_ParentChanged;
                _elementControls.Add(control);
            }
            control.GetType().GetProperty("Element").SetValue(control, e);
            return control;
        }

        private void ElementControl_SizeChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;

            if (control.Parent != null) elementPanel.Height = control.Height;
        }

        private void ElementControl_ParentChanged(object sender, EventArgs e)
        {
            if (((Control)sender).Parent == null) sender.GetType().GetProperty("Element").SetValue(sender, null);
        }

        private void ListBox_Enter(object sender, EventArgs e)
        {
            listBox.SelectionMode = SelectionMode.MultiExtended;
        }

        private void ListBox_Leave(object sender, EventArgs e)
        {
            listBox.SelectionMode = SelectionMode.One;
        }

        private void AddSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null)
            {
                var type = (SpriteElementType)((ToolStripMenuItem)sender).Tag;

                var element = SpriteElement.Create(type);

                timeline.Elements.Insert(listBox.SelectedIndex + 1, element);
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null && listBox.SelectedIndices.Count != 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                for (var i = indices.Length - 1; i >= 0; i--)
                {
                    timeline.Elements.RemoveAt(indices[i]);
                }
            }
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null && listBox.SelectedIndices.Count != 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices[0] > 0)
                {
                    foreach (var i in indices)
                    {
                        timeline.Elements.Move(i, i - 1);
                    }
                }
            }
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null && listBox.SelectedIndices.Count != 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                if (indices[indices.Length - 1] < timeline.Elements.Count - 1)
                {
                    for (var i = indices.Length - 1; i >= 0; i--)
                    {
                        timeline.Elements.Move(indices[i], indices[i] + 1);
                    }
                }
            }
        }

        private void Clip(bool cut)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null && listBox.SelectedIndices.Count != 0)
            {
                var indices = new int[listBox.SelectedIndices.Count];
                listBox.SelectedIndices.CopyTo(indices, 0);
                Array.Sort(indices);

                var elements = indices.Select(i => timeline.Elements[i]);
                AssetManager.Instance.Clip(elements, cut);
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clip(false);
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clip(true);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null && AssetManager.Instance.ClipObject is SpriteElement[] elements)
            {
                if (AssetManager.Instance.ClipCut)
                {
                    foreach (var element in elements)
                    {
                        if (element.Parent == timeline) return;
                    }
                    AssetManager.Instance.ClearClip();
                }
                else
                {
                    elements = elements.Select(element => element.Clone()).ToArray();
                }
                var index = listBox.SelectedIndex + 1;
                foreach (var element in elements)
                {
                    timeline.Elements.Insert(index++, element);
                }
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline == null) e.Cancel = true;
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null) timeline.Move(timeline.Layer, -0.001f);
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            var timeline = _Sprite?.SelectedTimeline;

            if (timeline != null) timeline.Move(timeline.Layer, 0.001f);
        }

        private bool _Splitted;
        public bool Splitted
        {
            set
            {
                if (_Splitted != value)
                {
                    _Splitted = value;

                    if (elementPanel.Controls.Count != 0)
                    {
                        var elementControl = elementPanel.Controls[0];

                        if (elementControl is ISplittableControl splittableControl) splittableControl.Splitted = value;
                    }
                }
            }
            get => _Splitted;
        }

        public void CollapseAll()
        {
            if (elementPanel.Controls.Count != 0)
            {
                var elementControl = elementPanel.Controls[0];

                if (elementControl is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseAll();
            }
        }

        public void CollapseDefault()
        {
            if (elementPanel.Controls.Count != 0)
            {
                var elementControl = elementPanel.Controls[0];

                if (elementControl is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseDefault();
            }
        }
    }
}
