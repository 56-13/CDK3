namespace CDK.Assets.Terrain
{
    partial class TerrainWallSourceControl
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
            this.selectionControl = new CDK.Assets.Meshing.MeshSelectionControl();
            this.label1 = new System.Windows.Forms.Label();
            this.boneListBox = new System.Windows.Forms.ListBox();
            this.boneContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.boneAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boneDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.bonePanel = new System.Windows.Forms.Panel();
            this.boneContextMenuStrip.SuspendLayout();
            this.panel.SuspendLayout();
            this.bonePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectionControl
            // 
            this.selectionControl.Location = new System.Drawing.Point(0, 0);
            this.selectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.selectionControl.Name = "selectionControl";
            this.selectionControl.Size = new System.Drawing.Size(320, 128);
            this.selectionControl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 57;
            this.label1.Text = "Bone";
            // 
            // boneListBox
            // 
            this.boneListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boneListBox.ContextMenuStrip = this.boneContextMenuStrip;
            this.boneListBox.FormattingEnabled = true;
            this.boneListBox.IntegralHeight = false;
            this.boneListBox.ItemHeight = 12;
            this.boneListBox.Location = new System.Drawing.Point(90, 0);
            this.boneListBox.Margin = new System.Windows.Forms.Padding(0);
            this.boneListBox.Name = "boneListBox";
            this.boneListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.boneListBox.Size = new System.Drawing.Size(230, 100);
            this.boneListBox.TabIndex = 58;
            // 
            // boneContextMenuStrip
            // 
            this.boneContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.boneAddToolStripMenuItem,
            this.boneRemoveToolStripMenuItem,
            this.boneUpToolStripMenuItem,
            this.boneDownToolStripMenuItem});
            this.boneContextMenuStrip.Name = "boneContextMenuStrip";
            this.boneContextMenuStrip.Size = new System.Drawing.Size(173, 92);
            this.boneContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.BoneContextMenuStrip_Opening);
            // 
            // boneAddToolStripMenuItem
            // 
            this.boneAddToolStripMenuItem.Name = "boneAddToolStripMenuItem";
            this.boneAddToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.boneAddToolStripMenuItem.Text = "Add";
            // 
            // boneRemoveToolStripMenuItem
            // 
            this.boneRemoveToolStripMenuItem.Name = "boneRemoveToolStripMenuItem";
            this.boneRemoveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.boneRemoveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.boneRemoveToolStripMenuItem.Text = "Remove";
            this.boneRemoveToolStripMenuItem.Click += new System.EventHandler(this.BoneRemoveToolStripMenuItem_Click);
            // 
            // boneUpToolStripMenuItem
            // 
            this.boneUpToolStripMenuItem.Name = "boneUpToolStripMenuItem";
            this.boneUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.boneUpToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.boneUpToolStripMenuItem.Text = "Up";
            this.boneUpToolStripMenuItem.Click += new System.EventHandler(this.BoneUpToolStripMenuItem_Click);
            // 
            // boneDownToolStripMenuItem
            // 
            this.boneDownToolStripMenuItem.Name = "boneDownToolStripMenuItem";
            this.boneDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.boneDownToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.boneDownToolStripMenuItem.Text = "Down";
            this.boneDownToolStripMenuItem.Click += new System.EventHandler(this.BoneDownToolStripMenuItem_Click);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.selectionControl);
            this.panel.Controls.Add(this.bonePanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 231);
            this.panel.TabIndex = 59;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // bonePanel
            // 
            this.bonePanel.Controls.Add(this.boneListBox);
            this.bonePanel.Controls.Add(this.label1);
            this.bonePanel.Location = new System.Drawing.Point(0, 131);
            this.bonePanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.bonePanel.Name = "bonePanel";
            this.bonePanel.Size = new System.Drawing.Size(320, 100);
            this.bonePanel.TabIndex = 2;
            // 
            // TerrainWallSourceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "TerrainWallSourceControl";
            this.Size = new System.Drawing.Size(320, 231);
            this.boneContextMenuStrip.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.bonePanel.ResumeLayout(false);
            this.bonePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Meshing.MeshSelectionControl selectionControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox boneListBox;
        private System.Windows.Forms.ContextMenuStrip boneContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem boneAddToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneRemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem boneDownToolStripMenuItem;
        private Components.StackPanel panel;
        private System.Windows.Forms.Panel bonePanel;
    }
}
