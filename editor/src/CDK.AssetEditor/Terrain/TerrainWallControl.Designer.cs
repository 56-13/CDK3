
namespace CDK.Assets.Terrain
{
    partial class TerrainWallControl
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
            this.sourceCheckBox = new System.Windows.Forms.CheckBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.sourceListBox = new CDK.Assets.Components.ListBox();
            this.sourceMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceControl = new CDK.Assets.Terrain.TerrainWallSourceControl();
            this.instanceTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.instancePointPanel = new System.Windows.Forms.Panel();
            this.instancePointZUpDown = new CDK.Assets.Components.NumericUpDown();
            this.instancePointXUpDown = new CDK.Assets.Components.NumericUpDown();
            this.instancePointYUpDown = new CDK.Assets.Components.NumericUpDown();
            this.instancePanel = new System.Windows.Forms.Panel();
            this.instanceFlipButton = new System.Windows.Forms.Button();
            this.instanceDownButton = new System.Windows.Forms.Button();
            this.instanceUpButton = new System.Windows.Forms.Button();
            this.instanceLoopUpDown = new CDK.Assets.Components.NumericUpDown();
            this.instanceDeleteButton = new System.Windows.Forms.Button();
            this.instancePointDeleteButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.sourceMenuStrip.SuspendLayout();
            this.instancePointPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointZUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointXUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointYUpDown)).BeginInit();
            this.instancePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceLoopUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // sourceCheckBox
            // 
            this.sourceCheckBox.AutoSize = true;
            this.sourceCheckBox.Checked = true;
            this.sourceCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sourceCheckBox.Location = new System.Drawing.Point(3, 85);
            this.sourceCheckBox.Name = "sourceCheckBox";
            this.sourceCheckBox.Size = new System.Drawing.Size(64, 16);
            this.sourceCheckBox.TabIndex = 57;
            this.sourceCheckBox.Text = "Source";
            this.sourceCheckBox.UseVisualStyleBackColor = true;
            this.sourceCheckBox.CheckedChanged += new System.EventHandler(this.SourceCheckBox_CheckedChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 107);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.sourceListBox);
            this.splitContainer.Panel1MinSize = 90;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.Controls.Add(this.sourceControl);
            this.splitContainer.Size = new System.Drawing.Size(320, 593);
            this.splitContainer.SplitterDistance = 90;
            this.splitContainer.TabIndex = 67;
            // 
            // sourceListBox
            // 
            this.sourceListBox.ContextMenuStrip = this.sourceMenuStrip;
            this.sourceListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceListBox.FormattingEnabled = true;
            this.sourceListBox.ItemHeight = 12;
            this.sourceListBox.Location = new System.Drawing.Point(0, 0);
            this.sourceListBox.Name = "sourceListBox";
            this.sourceListBox.Size = new System.Drawing.Size(320, 90);
            this.sourceListBox.TabIndex = 0;
            // 
            // sourceMenuStrip
            // 
            this.sourceMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.sourceMenuStrip.Name = "surfaceMenuStrip";
            this.sourceMenuStrip.Size = new System.Drawing.Size(207, 114);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.MoveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.MoveDownToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameToolStripMenuItem_Click);
            // 
            // sourceControl
            // 
            this.sourceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceControl.Location = new System.Drawing.Point(0, 0);
            this.sourceControl.Name = "sourceControl";
            this.sourceControl.Size = new System.Drawing.Size(320, 231);
            this.sourceControl.TabIndex = 0;
            // 
            // instanceTextBox
            // 
            this.instanceTextBox.Location = new System.Drawing.Point(62, 0);
            this.instanceTextBox.Name = "instanceTextBox";
            this.instanceTextBox.ReadOnly = true;
            this.instanceTextBox.Size = new System.Drawing.Size(255, 21);
            this.instanceTextBox.TabIndex = 68;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 69;
            this.label1.Text = "Instance";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 70;
            this.label2.Text = "Loop";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 12);
            this.label3.TabIndex = 75;
            this.label3.Text = "Point";
            // 
            // instancePointPanel
            // 
            this.instancePointPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instancePointPanel.Controls.Add(this.instancePointDeleteButton);
            this.instancePointPanel.Controls.Add(this.instancePointZUpDown);
            this.instancePointPanel.Controls.Add(this.label3);
            this.instancePointPanel.Controls.Add(this.instancePointXUpDown);
            this.instancePointPanel.Controls.Add(this.instancePointYUpDown);
            this.instancePointPanel.Location = new System.Drawing.Point(0, 58);
            this.instancePointPanel.Name = "instancePointPanel";
            this.instancePointPanel.Size = new System.Drawing.Size(320, 21);
            this.instancePointPanel.TabIndex = 76;
            // 
            // instancePointZUpDown
            // 
            this.instancePointZUpDown.DecimalPlaces = 2;
            this.instancePointZUpDown.Location = new System.Drawing.Point(194, 0);
            this.instancePointZUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.instancePointZUpDown.Name = "instancePointZUpDown";
            this.instancePointZUpDown.Size = new System.Drawing.Size(60, 21);
            this.instancePointZUpDown.TabIndex = 74;
            // 
            // instancePointXUpDown
            // 
            this.instancePointXUpDown.Location = new System.Drawing.Point(62, 0);
            this.instancePointXUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.instancePointXUpDown.Name = "instancePointXUpDown";
            this.instancePointXUpDown.ReadOnly = true;
            this.instancePointXUpDown.Size = new System.Drawing.Size(60, 21);
            this.instancePointXUpDown.TabIndex = 72;
            // 
            // instancePointYUpDown
            // 
            this.instancePointYUpDown.Location = new System.Drawing.Point(128, 0);
            this.instancePointYUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.instancePointYUpDown.Name = "instancePointYUpDown";
            this.instancePointYUpDown.ReadOnly = true;
            this.instancePointYUpDown.Size = new System.Drawing.Size(60, 21);
            this.instancePointYUpDown.TabIndex = 73;
            // 
            // instancePanel
            // 
            this.instancePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instancePanel.Controls.Add(this.instanceDeleteButton);
            this.instancePanel.Controls.Add(this.instanceFlipButton);
            this.instancePanel.Controls.Add(this.instanceDownButton);
            this.instancePanel.Controls.Add(this.instanceUpButton);
            this.instancePanel.Controls.Add(this.instanceTextBox);
            this.instancePanel.Controls.Add(this.label1);
            this.instancePanel.Controls.Add(this.label2);
            this.instancePanel.Controls.Add(this.instanceLoopUpDown);
            this.instancePanel.Location = new System.Drawing.Point(0, 3);
            this.instancePanel.Name = "instancePanel";
            this.instancePanel.Size = new System.Drawing.Size(320, 49);
            this.instancePanel.TabIndex = 77;
            // 
            // instanceFlipButton
            // 
            this.instanceFlipButton.Location = new System.Drawing.Point(216, 28);
            this.instanceFlipButton.Name = "instanceFlipButton";
            this.instanceFlipButton.Size = new System.Drawing.Size(38, 21);
            this.instanceFlipButton.TabIndex = 74;
            this.instanceFlipButton.Text = "↔";
            this.instanceFlipButton.UseVisualStyleBackColor = true;
            this.instanceFlipButton.Click += new System.EventHandler(this.InstanceFlipButton_Click);
            // 
            // instanceDownButton
            // 
            this.instanceDownButton.Location = new System.Drawing.Point(172, 28);
            this.instanceDownButton.Name = "instanceDownButton";
            this.instanceDownButton.Size = new System.Drawing.Size(38, 21);
            this.instanceDownButton.TabIndex = 73;
            this.instanceDownButton.Text = "↓";
            this.instanceDownButton.UseVisualStyleBackColor = true;
            this.instanceDownButton.Click += new System.EventHandler(this.InstanceDownButton_Click);
            // 
            // instanceUpButton
            // 
            this.instanceUpButton.Location = new System.Drawing.Point(128, 28);
            this.instanceUpButton.Name = "instanceUpButton";
            this.instanceUpButton.Size = new System.Drawing.Size(38, 21);
            this.instanceUpButton.TabIndex = 72;
            this.instanceUpButton.Text = "↑";
            this.instanceUpButton.UseVisualStyleBackColor = true;
            this.instanceUpButton.Click += new System.EventHandler(this.InstanceUpButton_Click);
            // 
            // instanceLoopUpDown
            // 
            this.instanceLoopUpDown.Location = new System.Drawing.Point(62, 28);
            this.instanceLoopUpDown.Name = "instanceLoopUpDown";
            this.instanceLoopUpDown.Size = new System.Drawing.Size(60, 21);
            this.instanceLoopUpDown.TabIndex = 71;
            // 
            // instanceDeleteButton
            // 
            this.instanceDeleteButton.Location = new System.Drawing.Point(260, 28);
            this.instanceDeleteButton.Name = "instanceDeleteButton";
            this.instanceDeleteButton.Size = new System.Drawing.Size(38, 21);
            this.instanceDeleteButton.TabIndex = 75;
            this.instanceDeleteButton.Text = "-";
            this.instanceDeleteButton.UseVisualStyleBackColor = true;
            this.instanceDeleteButton.Click += new System.EventHandler(this.InstanceDeleteButton_Click);
            // 
            // instancePointDeleteButton
            // 
            this.instancePointDeleteButton.Location = new System.Drawing.Point(260, 0);
            this.instancePointDeleteButton.Name = "instancePointDeleteButton";
            this.instancePointDeleteButton.Size = new System.Drawing.Size(38, 21);
            this.instancePointDeleteButton.TabIndex = 76;
            this.instancePointDeleteButton.Text = "-";
            this.instancePointDeleteButton.UseVisualStyleBackColor = true;
            this.instancePointDeleteButton.Click += new System.EventHandler(this.InstancePointDeleteButton_Click);
            // 
            // TerrainWallControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.instancePanel);
            this.Controls.Add(this.instancePointPanel);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.sourceCheckBox);
            this.Name = "TerrainWallControl";
            this.Size = new System.Drawing.Size(320, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.sourceMenuStrip.ResumeLayout(false);
            this.instancePointPanel.ResumeLayout(false);
            this.instancePointPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointZUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointXUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.instancePointYUpDown)).EndInit();
            this.instancePanel.ResumeLayout(false);
            this.instancePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instanceLoopUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox sourceCheckBox;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ContextMenuStrip sourceMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.TextBox instanceTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Components.NumericUpDown instanceLoopUpDown;
        private Components.NumericUpDown instancePointXUpDown;
        private Components.NumericUpDown instancePointYUpDown;
        private Components.NumericUpDown instancePointZUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel instancePointPanel;
        private System.Windows.Forms.Panel instancePanel;
        private Components.ListBox sourceListBox;
        private TerrainWallSourceControl sourceControl;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Button instanceDownButton;
        private System.Windows.Forms.Button instanceUpButton;
        private System.Windows.Forms.Button instanceFlipButton;
        private System.Windows.Forms.Button instanceDeleteButton;
        private System.Windows.Forms.Button instancePointDeleteButton;
    }
}
