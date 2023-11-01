
namespace CDK.Assets.Scenes
{
    partial class SceneControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.animationPanel = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.TrackBar();
            this.maxDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.progressUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.minDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.speedComboBox = new CDK.Assets.Components.ComboBox();
            this.modeComboBox = new CDK.Assets.Components.ComboBox();
            this.screenSplitContainer = new System.Windows.Forms.SplitContainer();
            this.screenControl = new CDK.Assets.Scenes.SceneScreenControl();
            this.selectedObjectOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.loopButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.subSplitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showTreeView1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.componentPanel = new CDK.Assets.Components.ScrollPanel();
            this.objectContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showTreeView2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.animationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.screenSplitContainer)).BeginInit();
            this.screenSplitContainer.Panel1.SuspendLayout();
            this.screenSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subSplitContainer)).BeginInit();
            this.subSplitContainer.Panel1.SuspendLayout();
            this.subSplitContainer.Panel2.SuspendLayout();
            this.subSplitContainer.SuspendLayout();
            this.treeViewContextMenuStrip.SuspendLayout();
            this.objectContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.screenSplitContainer);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.subSplitContainer);
            this.splitContainer.Size = new System.Drawing.Size(1392, 947);
            this.splitContainer.SplitterDistance = 1000;
            this.splitContainer.TabIndex = 0;
            // 
            // animationPanel
            // 
            this.animationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationPanel.Controls.Add(this.progressBar);
            this.animationPanel.Controls.Add(this.maxDurationUpDown);
            this.animationPanel.Controls.Add(this.progressUpDown);
            this.animationPanel.Controls.Add(this.label1);
            this.animationPanel.Controls.Add(this.minDurationUpDown);
            this.animationPanel.Location = new System.Drawing.Point(83, 923);
            this.animationPanel.Name = "animationPanel";
            this.animationPanel.Size = new System.Drawing.Size(645, 21);
            this.animationPanel.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.AutoSize = false;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Maximum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(427, 21);
            this.progressBar.TabIndex = 53;
            this.progressBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.progressBar.ValueChanged += new System.EventHandler(this.ProgressBar_ValueChanged);
            this.progressBar.Leave += new System.EventHandler(this.ProgressControl_Leave);
            // 
            // maxDurationUpDown
            // 
            this.maxDurationUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxDurationUpDown.DecimalPlaces = 2;
            this.maxDurationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.maxDurationUpDown.Location = new System.Drawing.Point(585, 0);
            this.maxDurationUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.maxDurationUpDown.Name = "maxDurationUpDown";
            this.maxDurationUpDown.ReadOnly = true;
            this.maxDurationUpDown.Size = new System.Drawing.Size(60, 21);
            this.maxDurationUpDown.TabIndex = 52;
            // 
            // progressUpDown
            // 
            this.progressUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressUpDown.DecimalPlaces = 2;
            this.progressUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.progressUpDown.Location = new System.Drawing.Point(433, 0);
            this.progressUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.progressUpDown.Name = "progressUpDown";
            this.progressUpDown.Size = new System.Drawing.Size(60, 21);
            this.progressUpDown.TabIndex = 54;
            this.progressUpDown.ValueChanged += new System.EventHandler(this.ProgressUpDown_ValueChanged);
            this.progressUpDown.Leave += new System.EventHandler(this.ProgressControl_Leave);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(565, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 51;
            this.label1.Text = "~";
            // 
            // minDurationUpDown
            // 
            this.minDurationUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minDurationUpDown.DecimalPlaces = 2;
            this.minDurationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.minDurationUpDown.Location = new System.Drawing.Point(499, 0);
            this.minDurationUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.minDurationUpDown.Name = "minDurationUpDown";
            this.minDurationUpDown.ReadOnly = true;
            this.minDurationUpDown.Size = new System.Drawing.Size(60, 21);
            this.minDurationUpDown.TabIndex = 50;
            // 
            // speedComboBox
            // 
            this.speedComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.speedComboBox.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.speedComboBox.FormattingEnabled = true;
            this.speedComboBox.ItemHeight = 13;
            this.speedComboBox.Location = new System.Drawing.Point(734, 923);
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.Size = new System.Drawing.Size(60, 21);
            this.speedComboBox.TabIndex = 55;
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(912, 923);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(88, 21);
            this.modeComboBox.TabIndex = 48;
            // 
            // screenSplitContainer
            // 
            this.screenSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.screenSplitContainer.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.screenSplitContainer.Name = "screenSplitContainer";
            this.screenSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // screenSplitContainer.Panel1
            // 
            this.screenSplitContainer.Panel1.Controls.Add(this.animationPanel);
            this.screenSplitContainer.Panel1.Controls.Add(this.screenControl);
            this.screenSplitContainer.Panel1.Controls.Add(this.speedComboBox);
            this.screenSplitContainer.Panel1.Controls.Add(this.modeComboBox);
            this.screenSplitContainer.Panel1.Controls.Add(this.playButton);
            this.screenSplitContainer.Panel1.Controls.Add(this.stopButton);
            this.screenSplitContainer.Panel1.Controls.Add(this.selectedObjectOnlyCheckBox);
            this.screenSplitContainer.Panel1.Controls.Add(this.loopButton);
            this.screenSplitContainer.Panel2Collapsed = true;
            this.screenSplitContainer.Size = new System.Drawing.Size(1000, 947);
            this.screenSplitContainer.SplitterDistance = 709;
            this.screenSplitContainer.TabIndex = 47;
            // 
            // screenControl
            // 
            this.screenControl.AllowDrop = true;
            this.screenControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(1000, 917);
            this.screenControl.TabIndex = 0;
            // 
            // selectedObjectOnlyCheckBox
            // 
            this.selectedObjectOnlyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedObjectOnlyCheckBox.AutoSize = true;
            this.selectedObjectOnlyCheckBox.Location = new System.Drawing.Point(800, 926);
            this.selectedObjectOnlyCheckBox.Name = "selectedObjectOnlyCheckBox";
            this.selectedObjectOnlyCheckBox.Size = new System.Drawing.Size(106, 16);
            this.selectedObjectOnlyCheckBox.TabIndex = 6;
            this.selectedObjectOnlyCheckBox.Text = "Selection Only";
            this.selectedObjectOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // loopButton
            // 
            this.loopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loopButton.BackgroundImage = global::CDK.Assets.Properties.Resources.loopOnButton;
            this.loopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.loopButton.FlatAppearance.BorderSize = 0;
            this.loopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loopButton.Location = new System.Drawing.Point(2, 923);
            this.loopButton.Name = "loopButton";
            this.loopButton.Size = new System.Drawing.Size(21, 21);
            this.loopButton.TabIndex = 5;
            this.loopButton.UseVisualStyleBackColor = true;
            this.loopButton.Click += new System.EventHandler(this.LoopButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopButton.BackgroundImage = global::CDK.Assets.Properties.Resources.stopButton;
            this.stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.stopButton.FlatAppearance.BorderSize = 0;
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopButton.Location = new System.Drawing.Point(56, 923);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(21, 21);
            this.stopButton.TabIndex = 3;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.playButton.BackgroundImage = global::CDK.Assets.Properties.Resources.playButton;
            this.playButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Location = new System.Drawing.Point(29, 923);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(21, 21);
            this.playButton.TabIndex = 1;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // subSplitContainer
            // 
            this.subSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.subSplitContainer.Name = "subSplitContainer";
            this.subSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // subSplitContainer.Panel1
            // 
            this.subSplitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // subSplitContainer.Panel2
            // 
            this.subSplitContainer.Panel2.Controls.Add(this.componentPanel);
            this.subSplitContainer.Size = new System.Drawing.Size(388, 947);
            this.subSplitContainer.SplitterDistance = 250;
            this.subSplitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.ContextMenuStrip = this.treeViewContextMenuStrip;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ItemHeight = 18;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(388, 250);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeView_AfterLabelEdit);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TreeView_ItemDrag);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeView_MouseDown);
            // 
            // treeViewContextMenuStrip
            // 
            this.treeViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTreeView1ToolStripMenuItem,
            this.addToolStripMenuItem,
            this.importToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.upToolStripMenuItem,
            this.downToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.treeViewContextMenuStrip.Name = "contextMenuStrip";
            this.treeViewContextMenuStrip.Size = new System.Drawing.Size(173, 224);
            this.treeViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.TreeViewContextMenuStrip_Opening);
            // 
            // showTreeView1ToolStripMenuItem
            // 
            this.showTreeView1ToolStripMenuItem.Name = "showTreeView1ToolStripMenuItem";
            this.showTreeView1ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.showTreeView1ToolStripMenuItem.Text = "Show Tree";
            this.showTreeView1ToolStripMenuItem.Click += new System.EventHandler(this.ShowTreeViewToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // upToolStripMenuItem
            // 
            this.upToolStripMenuItem.Name = "upToolStripMenuItem";
            this.upToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.upToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.upToolStripMenuItem.Text = "Up";
            this.upToolStripMenuItem.Click += new System.EventHandler(this.UpToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.downToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.downToolStripMenuItem.Text = "Down";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.DownToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameToolStripMenuItem_Click);
            // 
            // objectPanel
            // 
            this.componentPanel.ContextMenuStrip = this.objectContextMenuStrip;
            this.componentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentPanel.Location = new System.Drawing.Point(0, 0);
            this.componentPanel.Name = "objectPanel";
            this.componentPanel.Size = new System.Drawing.Size(388, 693);
            this.componentPanel.TabIndex = 0;
            // 
            // objectContextMenuStrip
            // 
            this.objectContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTreeView2ToolStripMenuItem,
            this.splitToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.collapseDefaultToolStripMenuItem});
            this.objectContextMenuStrip.Name = "objectContextMenuStrip";
            this.objectContextMenuStrip.Size = new System.Drawing.Size(163, 92);
            this.objectContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ObjectContextMenuStrip_Opening);
            // 
            // showTreeView2ToolStripMenuItem
            // 
            this.showTreeView2ToolStripMenuItem.Name = "showTreeView2ToolStripMenuItem";
            this.showTreeView2ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.showTreeView2ToolStripMenuItem.Text = "Show Tree";
            this.showTreeView2ToolStripMenuItem.Click += new System.EventHandler(this.ShowTreeViewToolStripMenuItem_Click);
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            this.splitToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.splitToolStripMenuItem.Text = "Split";
            this.splitToolStripMenuItem.Click += new System.EventHandler(this.SplitToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.CollapseAllToolStripMenuItem_Click);
            // 
            // collapseDefaultToolStripMenuItem
            // 
            this.collapseDefaultToolStripMenuItem.Name = "collapseDefaultToolStripMenuItem";
            this.collapseDefaultToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.collapseDefaultToolStripMenuItem.Text = "Collapse Default";
            this.collapseDefaultToolStripMenuItem.Click += new System.EventHandler(this.CollapseDefaultToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // SceneControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "SceneControl";
            this.Size = new System.Drawing.Size(1392, 947);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.animationPanel.ResumeLayout(false);
            this.animationPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxDurationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minDurationUpDown)).EndInit();
            this.screenSplitContainer.Panel1.ResumeLayout(false);
            this.screenSplitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.screenSplitContainer)).EndInit();
            this.screenSplitContainer.ResumeLayout(false);
            this.subSplitContainer.Panel1.ResumeLayout(false);
            this.subSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.subSplitContainer)).EndInit();
            this.subSplitContainer.ResumeLayout(false);
            this.treeViewContextMenuStrip.ResumeLayout(false);
            this.objectContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private SceneScreenControl screenControl;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button loopButton;
        private System.Windows.Forms.SplitContainer subSplitContainer;
        private System.Windows.Forms.TreeView treeView;
        private Components.ScrollPanel componentPanel;
        private System.Windows.Forms.ContextMenuStrip treeViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTreeView1ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip objectContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showTreeView2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitToolStripMenuItem;
        private System.Windows.Forms.CheckBox selectedObjectOnlyCheckBox;
        private System.Windows.Forms.SplitContainer screenSplitContainer;
        private Components.ComboBox modeComboBox;
        private System.Windows.Forms.NumericUpDown maxDurationUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown minDurationUpDown;
        private System.Windows.Forms.TrackBar progressBar;
        private System.Windows.Forms.NumericUpDown progressUpDown;
        private Components.ComboBox speedComboBox;
        private System.Windows.Forms.Panel animationPanel;
    }
}
