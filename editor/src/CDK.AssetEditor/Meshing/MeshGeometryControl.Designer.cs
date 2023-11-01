
namespace CDK.Assets.Meshing
{
    partial class MeshGeometryControl
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
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colliderListBox = new CDK.Assets.Components.ListBox();
            this.colliderControl = new CDK.Assets.Meshing.MeshColliderControl();
            this.boneCountUpDown = new System.Windows.Forms.NumericUpDown();
            this.vertexCountUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.infoPanel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.materialConfigsPanel = new CDK.Assets.Components.StackPanel();
            this.collidersPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boneCountUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexCountUpDown)).BeginInit();
            this.panel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.collidersPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(207, 92);
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
            // colliderListBox
            // 
            this.colliderListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colliderListBox.ContextMenuStrip = this.contextMenuStrip;
            this.colliderListBox.DisplayMember = "Name";
            this.colliderListBox.FormattingEnabled = true;
            this.colliderListBox.IntegralHeight = false;
            this.colliderListBox.ItemHeight = 12;
            this.colliderListBox.Location = new System.Drawing.Point(0, 0);
            this.colliderListBox.Name = "colliderListBox";
            this.colliderListBox.Size = new System.Drawing.Size(279, 100);
            this.colliderListBox.TabIndex = 10;
            // 
            // colliderControl
            // 
            this.colliderControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colliderControl.Location = new System.Drawing.Point(2, 106);
            this.colliderControl.Name = "colliderControl";
            this.colliderControl.Size = new System.Drawing.Size(277, 208);
            this.colliderControl.TabIndex = 9;
            // 
            // boneCountUpDown
            // 
            this.boneCountUpDown.Location = new System.Drawing.Point(72, 27);
            this.boneCountUpDown.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.boneCountUpDown.Name = "boneCountUpDown";
            this.boneCountUpDown.ReadOnly = true;
            this.boneCountUpDown.Size = new System.Drawing.Size(60, 21);
            this.boneCountUpDown.TabIndex = 8;
            // 
            // vertexCountUpDown
            // 
            this.vertexCountUpDown.Location = new System.Drawing.Point(72, 0);
            this.vertexCountUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.vertexCountUpDown.Name = "vertexCountUpDown";
            this.vertexCountUpDown.ReadOnly = true;
            this.vertexCountUpDown.Size = new System.Drawing.Size(60, 21);
            this.vertexCountUpDown.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Bone";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Vertex";
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.infoPanel);
            this.panel.Controls.Add(this.materialConfigsPanel);
            this.panel.Controls.Add(this.collidersPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(285, 437);
            this.panel.TabIndex = 13;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // infoPanel
            // 
            this.infoPanel.AutoSize = true;
            this.infoPanel.Collapsible = true;
            this.infoPanel.Controls.Add(this.panel1);
            this.infoPanel.Location = new System.Drawing.Point(0, 0);
            this.infoPanel.Margin = new System.Windows.Forms.Padding(0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(285, 75);
            this.infoPanel.TabIndex = 0;
            this.infoPanel.Title = "Information";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.boneCountUpDown);
            this.panel1.Controls.Add(this.vertexCountUpDown);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(3, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(279, 48);
            this.panel1.TabIndex = 0;
            // 
            // materialConfigsPanel
            // 
            this.materialConfigsPanel.AutoSize = true;
            this.materialConfigsPanel.Collapsible = true;
            this.materialConfigsPanel.Location = new System.Drawing.Point(0, 75);
            this.materialConfigsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.materialConfigsPanel.Name = "materialConfigsPanel";
            this.materialConfigsPanel.Size = new System.Drawing.Size(285, 21);
            this.materialConfigsPanel.TabIndex = 1;
            this.materialConfigsPanel.Title = "Material Configs";
            // 
            // collidersPanel
            // 
            this.collidersPanel.AutoSize = true;
            this.collidersPanel.Collapsible = true;
            this.collidersPanel.Controls.Add(this.panel2);
            this.collidersPanel.Location = new System.Drawing.Point(0, 96);
            this.collidersPanel.Margin = new System.Windows.Forms.Padding(0);
            this.collidersPanel.Name = "collidersPanel";
            this.collidersPanel.Size = new System.Drawing.Size(285, 341);
            this.collidersPanel.TabIndex = 2;
            this.collidersPanel.Title = "Colliders";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.colliderControl);
            this.panel2.Controls.Add(this.colliderListBox);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(279, 314);
            this.panel2.TabIndex = 0;
            // 
            // MeshGeometryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "MeshGeometryControl";
            this.Size = new System.Drawing.Size(285, 437);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.boneCountUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexCountUpDown)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.collidersPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MeshColliderControl colliderControl;
        private Components.ListBox colliderListBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown boneCountUpDown;
        private System.Windows.Forms.NumericUpDown vertexCountUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Components.StackPanel panel;
        private Components.StackPanel infoPanel;
        private System.Windows.Forms.Panel panel1;
        private Components.StackPanel materialConfigsPanel;
        private Components.StackPanel collidersPanel;
        private System.Windows.Forms.Panel panel2;
    }
}
