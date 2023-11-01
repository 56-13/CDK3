using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using CDK.Assets.Components;
using CDK.Assets.Support;

namespace CDK.Assets.Scenes
{
    public partial class SceneControl : UserControl
    {
        private Scene _Scene;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            set
            {
                if (_Scene != value)
                {
                    if (_Scene != null)
                    {
                        speedComboBox.DataBindings.Clear();
                        modeComboBox.DataBindings.Clear();
                        selectedObjectOnlyCheckBox.DataBindings.Clear();

                        _Scene.PropertyChanged -= Scene_PropertyChanged;
                        _Scene.Children.ListChanged -= Children_ListChanged;
                    }

                    _Scene = value;

                    screenControl.Scene = _Scene;

                    if (_Scene != null)
                    {
                        speedComboBox.DataBindings.Add("SelectedItem", _Scene, "Speed", false, DataSourceUpdateMode.OnPropertyChanged);
                        modeComboBox.DataBindings.Add("SelectedItem", _Scene, "Mode", false, DataSourceUpdateMode.OnPropertyChanged);
                        selectedObjectOnlyCheckBox.DataBindings.Add("Checked", _Scene, "SelectedObjectOnly", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Scene.PropertyChanged += Scene_PropertyChanged;
                        _Scene.Children.ListChanged += Children_ListChanged;
                    }

                    ResetTree();

                    ResetSelectedAnimation();

                    ResetLoopButtonImage();
                    ResetPlayButtonImage();

                    SceneChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Scene;
        }

        public event EventHandler SceneChanged;

        private Dictionary<SceneComponent, TreeNode> _treeNodes;
        private Timer _progressTimer;
        private Timer _durationTimer;
        private bool _progressUpdating;
        private bool _splitted;

        public SceneControl()
        {
            InitializeComponent();

            modeComboBox.DataSource = Enum.GetValues(typeof(SceneMode));

            _treeNodes = new Dictionary<SceneComponent, TreeNode>();

            treeView.ImageList = AssetControl.Instance?.SmallImageList;

            speedComboBox.Items.Add(0.25f);
            speedComboBox.Items.Add(0.5f);
            speedComboBox.Items.Add(1.0f);
            speedComboBox.Items.Add(1.5f);
            speedComboBox.Items.Add(2.0f);

            _progressTimer = new Timer
            {
                Interval = 50
            };
            _progressTimer.Tick += ProgressTimer_Tick;

            _durationTimer = new Timer
            {
                Interval = 1000
            };
            _durationTimer.Tick += DurationTimer_Tick;

            Disposed += SceneControl_Disposed;
        }

        private void SceneControl_Disposed(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                _Scene.PropertyChanged -= Scene_PropertyChanged;
                _Scene.Children.ListChanged -= Children_ListChanged;

                ClearTreeNodes(treeView.Nodes);
            }
            _progressTimer.Dispose();
            _durationTimer.Dispose();
        }

        private void Scene_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedObject":
                    ResetSelectedObject();
                    break;
                case "SelectedAnimation":
                    ResetSelectedAnimation();
                    break;
            }
        }

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            var children = (AssetElementList<SceneComponent>)sender;

            TreeNodeCollection nodes;
            if (children.Parent == _Scene) nodes = treeView.Nodes;
            else  if (_treeNodes.TryGetValue((SceneComponent)children.Parent, out var node)) nodes = node.Nodes;
            else return;

            treeView.BeginUpdate();

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    nodes.Insert(e.NewIndex, CreateTreeNode(children[e.NewIndex]));
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null)
                    {
                        if (e.PropertyDescriptor.Name == "Name")
                        {
                            nodes[e.NewIndex].Text = children[e.NewIndex].Name;
                        }
                    }
                    else
                    {
                        RemoveTreeNode(nodes[e.NewIndex]);
                        nodes.Insert(e.NewIndex, CreateTreeNode(children[e.NewIndex]));
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveTreeNode(nodes[e.NewIndex]);
                    break;
                case ListChangedType.ItemMoved:
                    {
                        var selectedNode = treeView.SelectedNode;
                        var subnode = nodes[e.OldIndex];
                        nodes.RemoveAt(e.OldIndex);
                        nodes.Insert(e.NewIndex, subnode);
                        treeView.SelectedNode = selectedNode;
                    }
                    break;
                case ListChangedType.Reset:
                    ClearTreeNodes(nodes);
                    foreach (var child in children)
                    {
                        nodes.Add(CreateTreeNode(child));
                    }
                    break;
            }
            treeView.EndUpdate();
        }

        private TreeNode CreateTreeNode(SceneComponent comp)
        {
            var node = new TreeNode
            {
                Tag = comp,
                Text = comp.Name
            };
            node.ImageKey = node.SelectedImageKey = comp.Type.ToString();
            comp.Children.ListChanged += Children_ListChanged;

            _treeNodes.Add(comp, node);

            foreach (var subobj in comp.Children)
            {
                node.Nodes.Add(CreateTreeNode(subobj));
            }
            return node;
        }

        private void RemoveTreeNode(TreeNode node)
        {
            var comp = (SceneComponent)node.Tag;
            comp.Children.ListChanged -= Children_ListChanged;
            ClearTreeNodes(node.Nodes);
            node.Remove();

            _treeNodes.Remove(comp);
        }

        private void ClearTreeNodes(TreeNodeCollection nodes)
        {
            while (nodes.Count != 0) RemoveTreeNode(nodes[0]);
        }

        private void ResetTree()
        {
            treeView.BeginUpdate();

            ClearTreeNodes(treeView.Nodes);

            if (_Scene != null)
            {
                foreach (var child in _Scene.Children)
                {
                    treeView.Nodes.Add(CreateTreeNode(child));
                }
            }

            ResetSelectedObject();

            treeView.EndUpdate();
        }

        private void ResetSelectedObject()
        {
            var comp = _Scene?.SelectedComponent;

            if (comp != null)
            {
                componentPanel.SuspendLayout();
                componentPanel.Controls.Clear();

                var control = AssetControl.Instance.GetControl(comp);

                if (control != null)
                {
                    control.Width = componentPanel.ClientSize.Width;
                    control.Location = Point.Empty;

                    componentPanel.Controls.Add(control);

                    ValidateBottomControl(control);
                }

                componentPanel.ResumeLayout();

                treeView.SelectedNode = _treeNodes[comp];
            }
            else
            {
                componentPanel.Controls.Clear();

                treeView.SelectedNode = null;
            }

            ResetSplitted();
        }

        private void ResetLoopButtonImage()
        {
            loopButton.BackgroundImage = screenControl.Loop ? Properties.Resources.loopOnButton : Properties.Resources.loopOffButton;
        }

        private void ResetPlayButtonImage()
        {
            playButton.BackgroundImage = screenControl.Updating ? Properties.Resources.pauseButton : Properties.Resources.playButton;
        }

        private void LoopButton_Click(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                screenControl.Loop = !screenControl.Loop;
                ResetLoopButtonImage();
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                screenControl.Updating = !screenControl.Updating;
                ResetPlayButtonImage();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_Scene != null)
            {
                _Scene.Rewind();

                screenControl.Updating = false;
                ResetPlayButtonImage();
            }
        }

        private static Control GetBottomControl(Control control)
        {
            if (control is ISceneBottomControlProvider provider) return provider.GetBottomControl();

            foreach (Control child in control.Controls)
            {
                var result = GetBottomControl(child);

                if (result != null) return result;
            }
            return null;
        }

        private void SetBottomControl(Control bottomControl)
        {
            if (screenSplitContainer.Panel2.Controls.Count != 0) screenSplitContainer.Panel2.Controls[0].Dispose();

            if (bottomControl != null)
            {
                bottomControl.Dock = DockStyle.Fill;
                screenSplitContainer.Panel2.Controls.Add(bottomControl);
                screenSplitContainer.Panel2Collapsed = false;
            }
            else screenSplitContainer.Panel2Collapsed = true;
        }

        public static void ValidateBottomControl(Control control)
        {
            var current = control.Parent;

            while (current != null)
            {
                if (current is SceneControl sceneControl)
                {
                    var bottomControl = GetBottomControl(control);

                    sceneControl.SetBottomControl(bottomControl);

                    break;
                }
                current = current.Parent;
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_Scene != null)
            {
                switch (e.Action)
                {
                    case TreeViewAction.ByMouse:
                    case TreeViewAction.ByKeyboard:
                        _Scene.SelectedComponent = (SceneObject)e.Node?.Tag;
                        break;
                }
            }
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView.SelectedNode = treeView.GetNodeAt(e.Location);
            }
        }

        private void TreeViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (_Scene == null)
            {
                e.Cancel = true;
                return;
            }

            showTreeView1ToolStripMenuItem.Checked = !subSplitContainer.Panel1Collapsed;

            var container = (SceneContainer)treeView.SelectedNode?.Tag;

            if (container == null) container = _Scene;

            addToolStripMenuItem.DropDownItems.Clear();

            foreach (var subType in container.SubTypes)
            {
                var addSubToolStripMenuItem = new ToolStripMenuItem
                {
                    Tag = subType,
                    Text = subType.ToString()
                };
                addSubToolStripMenuItem.Click += AddSubToolStripMenuItem_Click;
                addToolStripMenuItem.DropDownItems.Add(addSubToolStripMenuItem);
            }

            addToolStripMenuItem.Visible = addToolStripMenuItem.DropDownItems.Count != 0;

            importToolStripMenuItem.Visible = container.ImportFilter != null;

            removeToolStripMenuItem.Visible =
                upToolStripMenuItem.Visible =
                downToolStripMenuItem.Visible =
                cutToolStripMenuItem.Visible = container.GetParent() is SceneContainer;

            pasteToolStripMenuItem.Visible = AssetManager.Instance.ClipObject is SceneObject clip &&
                (container.AddSubEnabled(clip) ||
                (container.GetParent() is SceneContainer parent && parent.AddSubEnabled(clip)));
        }

        private void AddSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var container = (SceneContainer)treeView.SelectedNode?.Tag;

            if (container == null) container = _Scene;

            var type = (SceneComponentType)((ToolStripMenuItem)sender).Tag;

            container.AddSub(type);
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var container = (SceneContainer)treeView.SelectedNode?.Tag;

            if (container == null) container = _Scene;

            openFileDialog.Filter = container.ImportFilter;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    try
                    {
                        container.Import(filename);
                    }
                    catch (Exception uex)
                    {
                        ErrorHandler.Record(uex);

                        MessageBox.Show(filename + "파일을 로드할 수 없습니다.");
                    }
                }
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null || comp.Fixed || !(comp.Parent is SceneContainer parent)) return;

            if (comp.IsRetained(out var from, out var to))
            {
                AssetControl.ShowRetained(this, from, to);
                return;
            }

            parent.Children.Remove(comp);
        }

        private void UpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null || !(comp.Parent is SceneContainer parent)) return;

            var index = parent.Children.IndexOf(comp);

            if (index > 0) parent.Children.Move(index, index - 1);
        }

        private void DownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null || !(comp.Parent is SceneContainer parent)) return;

            var index = parent.Children.IndexOf(comp);

            if (index < parent.Children.Count) parent.Children.Move(index, index + 1);
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null) return;

            AssetManager.Instance.Clip(comp, false);
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null) return;

            AssetManager.Instance.Clip(comp, true);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Scene == null) return;

            var comp = (SceneContainer)treeView.SelectedNode?.Tag;

            if (comp == null) comp = _Scene;

            comp.Paste();
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp == null) return;

            treeView.LabelEdit = true;

            upToolStripMenuItem.Enabled = false;
            downToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            removeToolStripMenuItem.Enabled = false;

            treeView.SelectedNode.BeginEdit();
        }

        private void ShowTreeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subSplitContainer.Panel1Collapsed = !subSplitContainer.Panel1Collapsed;
        }

        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var comp = (SceneComponent)e.Node.Tag;

            var text = e.Label;

            if (text == string.Empty) text = null;

            comp.Name = text;

            e.CancelEdit = true;

            e.Node.Text = comp.Name;

            treeView.LabelEdit = false;

            upToolStripMenuItem.Enabled = true;
            downToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            removeToolStripMenuItem.Enabled = true;
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var comp = (SceneComponent)treeView.SelectedNode?.Tag;

            if (comp is SceneObject obj && obj.AllowDrag) DoDragDrop(obj.Key, DragDropEffects.Copy | DragDropEffects.Link);
        }

        private void ObjectContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            showTreeView2ToolStripMenuItem.Checked = !subSplitContainer.Panel1Collapsed;
            splitToolStripMenuItem.Checked = _splitted;
        }

        private void CollapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (componentPanel.Controls.Count != 0)
            {
                var control = componentPanel.Controls[0];

                if (control is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseAll();
            }
        }

        private void CollapseDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (componentPanel.Controls.Count != 0)
            {
                var control = componentPanel.Controls[0];

                if (control is ICollapsibleControl collapsibleControl) collapsibleControl.CollapseDefault();
            }
        }

        private void SetSplitted(bool flag)
        {
            if (_splitted != flag)
            {
                _splitted = flag;

                var w = splitContainer.Width - splitContainer.SplitterDistance;
                if (flag) w *= 2;
                else w /= 2;
                splitContainer.SplitterDistance = splitContainer.Width - w;
            }
        }

        private void ResetSplitted()
        {
            var flag = false;

            if (componentPanel.Controls.Count != 0)
            {
                var control = componentPanel.Controls[0];

                if (control is ISplittableControl spliitableControl) flag = spliitableControl.Splitted;
            }
            SetSplitted(flag);
        }

        private void SplitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (componentPanel.Controls.Count != 0)
            {
                var control = componentPanel.Controls[0];

                if (control is ISplittableControl spliitableControl)
                {
                    spliitableControl.Splitted = !spliitableControl.Splitted;

                    SetSplitted(spliitableControl.Splitted);
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            _durationTimer.Enabled = _progressTimer.Enabled = _Scene?.SelectedAnimation != null && Visible;
        }

        private void ResetSelectedAnimation()
        {
            var animation = _Scene?.SelectedAnimation;

            animationPanel.Visible = animation != null;

            ResetAnimationProgress();

            _durationTimer.Enabled = _progressTimer.Enabled = animation != null && Visible;
        }

        private void ResetAnimationProgress()
        {
            var animation = _Scene?.SelectedAnimation;

            //그래픽스가 아직 없을 수 있다.
            if (animation != null && screenControl.Graphics != null)
            {
                var min = animation.GetDuration(DurationParam.Min);
                var max = animation.GetDuration(DurationParam.Max);

                minDurationUpDown.Value = (decimal)min;
                maxDurationUpDown.Value = (decimal)max;

                if (!_progressUpdating)
                {
                    var progress = animation.Progress;
                    progressBar.Maximum = (int)(Math.Max(progress, max) * 100);
                    progressBar.Value = (int)(progress * 100);
                    progressUpDown.Maximum = (decimal)Math.Max(progress, max);
                    progressUpDown.Value = (decimal)progress;
                }
            }
            else
            {
                minDurationUpDown.Value = 0;
                maxDurationUpDown.Value = 0;
                progressBar.Maximum = 0;
                progressUpDown.Maximum = 0;
            }
        }

        private void DurationTimer_Tick(object sender, EventArgs e)
        {
            var animation = _Scene?.SelectedAnimation;
            
            //그래픽스가 아직 없을 수 있다.
            if (animation != null && screenControl.Graphics != null)
            {
                var min = animation.GetDuration(DurationParam.Min);
                var max = animation.GetDuration(DurationParam.Max);

                minDurationUpDown.Value = (decimal)min;
                maxDurationUpDown.Value = (decimal)max;

                if (!_progressUpdating)
                {
                    var progress = animation.Progress;
                    progressBar.Maximum = (int)(Math.Max(progress, max) * 100);
                    progressUpDown.Maximum = (decimal)Math.Max(progress, max);
                }
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            var animation = _Scene?.SelectedAnimation;

            //그래픽스가 아직 없을 수 있다.
            if (animation != null && screenControl.Graphics != null && !_progressUpdating)
            {
                var progress = animation.Progress;
                var barProgress = (int)(progress * 100);
                if (barProgress > progressBar.Maximum) progressBar.Maximum = barProgress;
                progressBar.Value = barProgress;
                var updownProgress = (decimal)progress;
                if (updownProgress > progressUpDown.Maximum) progressUpDown.Maximum = updownProgress;
                progressUpDown.Value = updownProgress;
            }
        }

        private bool UpdateProgress(float oldProgress, float newProgress)
        {
            if (newProgress == oldProgress) return false;
            
            _progressUpdating = true;

            if (screenControl.Updating)
            {
                screenControl.Updating = false;
                ResetPlayButtonImage();
            }

            float delta;
            if (newProgress < oldProgress)
            {
                _Scene.Rewind();
                delta = newProgress;
            }
            else delta = newProgress - oldProgress;

            screenControl.Update(delta);

            screenControl.Refresh();

            return true;
        }

        private void ProgressBar_ValueChanged(object sender, EventArgs e)
        {
            var animation = _Scene?.SelectedAnimation;

            if (animation != null && progressBar.Focused && UpdateProgress(animation.Progress, progressBar.Value * 0.01f))
            {
                var updownProgress = (decimal)(progressBar.Value * 0.01f);
                if (updownProgress > progressUpDown.Maximum) progressUpDown.Maximum = updownProgress;
                progressUpDown.Value = updownProgress;
            }
        }

        private void ProgressUpDown_ValueChanged(object sender, EventArgs e)
        {
            var animation = _Scene?.SelectedAnimation;

            if (animation != null && progressUpDown.Focused && UpdateProgress(animation.Progress, (float)progressUpDown.Value))
            {
                var barProgress = (int)(progressUpDown.Value * 100);
                if (barProgress > progressBar.Maximum) progressBar.Maximum = barProgress;
                progressBar.Value = barProgress;
            }
        }

        private void ProgressControl_Leave(object sender, EventArgs e)
        {
            _progressUpdating = false;
        }
    }
}

