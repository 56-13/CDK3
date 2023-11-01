namespace CDK.Assets.Animations.Sprite
{
    partial class SpriteControl
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
            this.listBox = new CDK.Assets.Components.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.layerUpDown = new CDK.Assets.Components.NumericUpDown();
            this.startTimeUpDown = new CDK.Assets.Components.NumericUpDown();
            this.endTimeUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.loopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.label5 = new System.Windows.Forms.Label();
            this.billboardCheckBox = new System.Windows.Forms.CheckBox();
            this.timelinePanel = new System.Windows.Forms.Panel();
            this.resetCheckBox = new System.Windows.Forms.CheckBox();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layerUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeUpDown)).BeginInit();
            this.timelinePanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.ContextMenuStrip = this.contextMenuStrip;
            this.listBox.FormattingEnabled = true;
            this.listBox.IntegralHeight = false;
            this.listBox.ItemHeight = 12;
            this.listBox.Location = new System.Drawing.Point(0, 54);
            this.listBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(350, 130);
            this.listBox.TabIndex = 0;
            this.listBox.Enter += new System.EventHandler(this.ListBox_Enter);
            this.listBox.Leave += new System.EventHandler(this.ListBox_Leave);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.upToolStripMenuItem,
            this.downToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(173, 158);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addToolStripMenuItem.Text = "Add";
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
            // elementPanel
            // 
            this.elementPanel.Location = new System.Drawing.Point(0, 428);
            this.elementPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.elementPanel.Name = "elementPanel";
            this.elementPanel.Size = new System.Drawing.Size(350, 0);
            this.elementPanel.TabIndex = 1;
            this.elementPanel.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Timeline Layer";
            // 
            // layerUpDown
            // 
            this.layerUpDown.Location = new System.Drawing.Point(113, 0);
            this.layerUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.layerUpDown.Name = "layerUpDown";
            this.layerUpDown.Size = new System.Drawing.Size(50, 21);
            this.layerUpDown.TabIndex = 3;
            // 
            // startTimeUpDown
            // 
            this.startTimeUpDown.DecimalPlaces = 3;
            this.startTimeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.startTimeUpDown.Location = new System.Drawing.Point(113, 27);
            this.startTimeUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.startTimeUpDown.Name = "startTimeUpDown";
            this.startTimeUpDown.Size = new System.Drawing.Size(50, 21);
            this.startTimeUpDown.TabIndex = 4;
            // 
            // endTimeUpDown
            // 
            this.endTimeUpDown.DecimalPlaces = 3;
            this.endTimeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.endTimeUpDown.Location = new System.Drawing.Point(189, 27);
            this.endTimeUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.endTimeUpDown.Name = "endTimeUpDown";
            this.endTimeUpDown.Size = new System.Drawing.Size(50, 21);
            this.endTimeUpDown.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "~";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "Timeline Duration";
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point(245, 26);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(23, 23);
            this.leftButton.TabIndex = 8;
            this.leftButton.Text = "<";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.LeftButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point(274, 26);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(23, 23);
            this.rightButton.TabIndex = 9;
            this.rightButton.Text = ">";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.RightButton_Click);
            // 
            // loopControl
            // 
            this.loopControl.FinishEnabled = true;
            this.loopControl.Location = new System.Drawing.Point(65, 0);
            this.loopControl.Name = "loopControl";
            this.loopControl.Size = new System.Drawing.Size(208, 21);
            this.loopControl.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "Loop";
            // 
            // billboardCheckBox
            // 
            this.billboardCheckBox.AutoSize = true;
            this.billboardCheckBox.Location = new System.Drawing.Point(279, 1);
            this.billboardCheckBox.Name = "billboardCheckBox";
            this.billboardCheckBox.Size = new System.Drawing.Size(73, 16);
            this.billboardCheckBox.TabIndex = 13;
            this.billboardCheckBox.Text = "Billboard";
            this.billboardCheckBox.UseVisualStyleBackColor = true;
            // 
            // timelinePanel
            // 
            this.timelinePanel.Controls.Add(this.resetCheckBox);
            this.timelinePanel.Controls.Add(this.layerUpDown);
            this.timelinePanel.Controls.Add(this.label1);
            this.timelinePanel.Controls.Add(this.startTimeUpDown);
            this.timelinePanel.Controls.Add(this.endTimeUpDown);
            this.timelinePanel.Controls.Add(this.label2);
            this.timelinePanel.Controls.Add(this.rightButton);
            this.timelinePanel.Controls.Add(this.listBox);
            this.timelinePanel.Controls.Add(this.label3);
            this.timelinePanel.Controls.Add(this.leftButton);
            this.timelinePanel.Location = new System.Drawing.Point(0, 238);
            this.timelinePanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timelinePanel.Name = "timelinePanel";
            this.timelinePanel.Size = new System.Drawing.Size(350, 184);
            this.timelinePanel.TabIndex = 14;
            this.timelinePanel.Visible = false;
            // 
            // resetCheckBox
            // 
            this.resetCheckBox.AutoSize = true;
            this.resetCheckBox.Location = new System.Drawing.Point(171, 3);
            this.resetCheckBox.Name = "resetCheckBox";
            this.resetCheckBox.Size = new System.Drawing.Size(56, 16);
            this.resetCheckBox.TabIndex = 10;
            this.resetCheckBox.Text = "Reset";
            this.resetCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.objectControl);
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.timelinePanel);
            this.panel.Controls.Add(this.elementPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(350, 428);
            this.panel.TabIndex = 15;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.billboardCheckBox);
            this.panel1.Controls.Add(this.loopControl);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(0, 211);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(350, 21);
            this.panel1.TabIndex = 0;
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(0, 0);
            this.objectControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(350, 205);
            this.objectControl.TabIndex = 11;
            // 
            // SpriteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "SpriteControl";
            this.Size = new System.Drawing.Size(350, 428);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layerUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startTimeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endTimeUpDown)).EndInit();
            this.timelinePanel.ResumeLayout(false);
            this.timelinePanel.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Assets.Components.ListBox listBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.Panel elementPanel;
        private System.Windows.Forms.Label label1;
        private Assets.Components.NumericUpDown layerUpDown;
        private Assets.Components.NumericUpDown startTimeUpDown;
        private Assets.Components.NumericUpDown endTimeUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private Components.AnimationLoopControl loopControl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox billboardCheckBox;
        private System.Windows.Forms.Panel timelinePanel;
        private Assets.Components.StackPanel panel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox resetCheckBox;
        private Scenes.SceneObjectControl objectControl;
    }
}
