namespace CDK.Assets.Texturing
{
    partial class SubImageControl
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubImageControl));
            this.importButton = new System.Windows.Forms.Button();
            this.resizeButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.opaqueButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.elementMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.elementAddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elementDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localeCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.screenControl = new CDK.Assets.Texturing.SubImageScreenControl();
            this.elementListBox = new CDK.Assets.Components.ListBox();
            this.frameControl = new CDK.Assets.Components.RectangleControl();
            this.borderXCheckBox = new System.Windows.Forms.CheckBox();
            this.borderYCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pivotControl = new CDK.Assets.Components.Vector2Control();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.scaledWidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.scaledHeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.elementMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaledWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaledHeightUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(187, 27);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(70, 21);
            this.importButton.TabIndex = 52;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // resizeButton
            // 
            this.resizeButton.Location = new System.Drawing.Point(188, 81);
            this.resizeButton.Name = "resizeButton";
            this.resizeButton.Size = new System.Drawing.Size(70, 21);
            this.resizeButton.TabIndex = 53;
            this.resizeButton.Text = "Resize";
            this.resizeButton.UseVisualStyleBackColor = true;
            this.resizeButton.Click += new System.EventHandler(this.ResizeButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(0, 0);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size(258, 21);
            this.nameTextBox.TabIndex = 54;
            // 
            // opaqueButton
            // 
            this.opaqueButton.Location = new System.Drawing.Point(188, 108);
            this.opaqueButton.Name = "opaqueButton";
            this.opaqueButton.Size = new System.Drawing.Size(70, 21);
            this.opaqueButton.TabIndex = 55;
            this.opaqueButton.Text = "Opaque";
            this.opaqueButton.UseVisualStyleBackColor = true;
            this.opaqueButton.Click += new System.EventHandler(this.OpaqueButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(187, 54);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(70, 21);
            this.exportButton.TabIndex = 57;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // elementMenuStrip
            // 
            this.elementMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.elementAddToolStripMenuItem,
            this.elementRemoveToolStripMenuItem,
            this.elementUpToolStripMenuItem,
            this.elementDownToolStripMenuItem});
            this.elementMenuStrip.Name = "elementMenuStrip";
            this.elementMenuStrip.Size = new System.Drawing.Size(173, 92);
            // 
            // elementAddToolStripMenuItem
            // 
            this.elementAddToolStripMenuItem.Name = "elementAddToolStripMenuItem";
            this.elementAddToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.elementAddToolStripMenuItem.Text = "Add";
            this.elementAddToolStripMenuItem.Click += new System.EventHandler(this.ElementAddToolStripMenuItem_Click);
            // 
            // elementRemoveToolStripMenuItem
            // 
            this.elementRemoveToolStripMenuItem.Name = "elementRemoveToolStripMenuItem";
            this.elementRemoveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.elementRemoveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.elementRemoveToolStripMenuItem.Text = "Remove";
            this.elementRemoveToolStripMenuItem.Click += new System.EventHandler(this.ElementRemoveToolStripMenuItem_Click);
            // 
            // elementUpToolStripMenuItem
            // 
            this.elementUpToolStripMenuItem.Name = "elementUpToolStripMenuItem";
            this.elementUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.elementUpToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.elementUpToolStripMenuItem.Text = "Up";
            this.elementUpToolStripMenuItem.Click += new System.EventHandler(this.ElementUpToolStripMenuItem_Click);
            // 
            // elementDownToolStripMenuItem
            // 
            this.elementDownToolStripMenuItem.Name = "elementDownToolStripMenuItem";
            this.elementDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.elementDownToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.elementDownToolStripMenuItem.Text = "Down";
            this.elementDownToolStripMenuItem.Click += new System.EventHandler(this.ElementDownToolStripMenuItem_Click);
            // 
            // localeCheckedListBox
            // 
            this.localeCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.localeCheckedListBox.CheckOnClick = true;
            this.localeCheckedListBox.Enabled = false;
            this.localeCheckedListBox.FormattingEnabled = true;
            this.localeCheckedListBox.IntegralHeight = false;
            this.localeCheckedListBox.Location = new System.Drawing.Point(132, 237);
            this.localeCheckedListBox.Name = "localeCheckedListBox";
            this.localeCheckedListBox.Size = new System.Drawing.Size(126, 59);
            this.localeCheckedListBox.TabIndex = 61;
            this.localeCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LocaleCheckedListBox_ItemCheck);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.screenControl);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(182, 102);
            this.panel1.TabIndex = 71;
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(180, 100);
            this.screenControl.TabIndex = 70;
            this.screenControl.Text = "screenControl";
            // 
            // elementListBox
            // 
            this.elementListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.elementListBox.ContextMenuStrip = this.elementMenuStrip;
            this.elementListBox.FormattingEnabled = true;
            this.elementListBox.IntegralHeight = false;
            this.elementListBox.ItemHeight = 12;
            this.elementListBox.Location = new System.Drawing.Point(0, 237);
            this.elementListBox.Name = "elementListBox";
            this.elementListBox.Size = new System.Drawing.Size(126, 59);
            this.elementListBox.TabIndex = 60;
            // 
            // frameControl
            // 
            this.frameControl.Increment = 1F;
            this.frameControl.Location = new System.Drawing.Point(0, 134);
            this.frameControl.Maximum = 65536F;
            this.frameControl.Minimum = 0F;
            this.frameControl.Name = "frameControl";
            this.frameControl.Size = new System.Drawing.Size(258, 21);
            this.frameControl.TabIndex = 58;
            // 
            // borderXCheckBox
            // 
            this.borderXCheckBox.AutoSize = true;
            this.borderXCheckBox.Location = new System.Drawing.Point(132, 215);
            this.borderXCheckBox.Name = "borderXCheckBox";
            this.borderXCheckBox.Size = new System.Drawing.Size(32, 16);
            this.borderXCheckBox.TabIndex = 72;
            this.borderXCheckBox.Text = "X";
            this.borderXCheckBox.UseVisualStyleBackColor = true;
            // 
            // borderYCheckBox
            // 
            this.borderYCheckBox.AutoSize = true;
            this.borderYCheckBox.Location = new System.Drawing.Point(170, 215);
            this.borderYCheckBox.Name = "borderYCheckBox";
            this.borderYCheckBox.Size = new System.Drawing.Size(32, 16);
            this.borderYCheckBox.TabIndex = 73;
            this.borderYCheckBox.Text = "Y";
            this.borderYCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 74;
            this.label1.Text = "Pivot";
            // 
            // pivotControl
            // 
            this.pivotControl.DecimalPlaces = 1;
            this.pivotControl.Increment = 1F;
            this.pivotControl.Location = new System.Drawing.Point(132, 188);
            this.pivotControl.Maximum = 1000F;
            this.pivotControl.Minimum = -1000F;
            this.pivotControl.Name = "pivotControl";
            this.pivotControl.PointFGDI = ((System.Drawing.PointF)(resources.GetObject("pivotControl.PointFGDI")));
            this.pivotControl.PointGDI = new System.Drawing.Point(0, 0);
            this.pivotControl.Size = new System.Drawing.Size(126, 21);
            this.pivotControl.TabIndex = 75;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 216);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 12);
            this.label2.TabIndex = 76;
            this.label2.Text = "Border";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 12);
            this.label3.TabIndex = 77;
            this.label3.Text = "Content Scaled Size";
            // 
            // scaledWidthUpDown
            // 
            this.scaledWidthUpDown.DecimalPlaces = 1;
            this.scaledWidthUpDown.Location = new System.Drawing.Point(132, 161);
            this.scaledWidthUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.scaledWidthUpDown.Name = "scaledWidthUpDown";
            this.scaledWidthUpDown.ReadOnly = true;
            this.scaledWidthUpDown.Size = new System.Drawing.Size(60, 21);
            this.scaledWidthUpDown.TabIndex = 78;
            // 
            // scaledHeightUpDown
            // 
            this.scaledHeightUpDown.DecimalPlaces = 1;
            this.scaledHeightUpDown.Location = new System.Drawing.Point(198, 161);
            this.scaledHeightUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.scaledHeightUpDown.Name = "scaledHeightUpDown";
            this.scaledHeightUpDown.ReadOnly = true;
            this.scaledHeightUpDown.Size = new System.Drawing.Size(60, 21);
            this.scaledHeightUpDown.TabIndex = 79;
            // 
            // SubImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scaledHeightUpDown);
            this.Controls.Add(this.scaledWidthUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pivotControl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.borderYCheckBox);
            this.Controls.Add(this.borderXCheckBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.localeCheckedListBox);
            this.Controls.Add(this.elementListBox);
            this.Controls.Add(this.frameControl);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.opaqueButton);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.resizeButton);
            this.Controls.Add(this.importButton);
            this.Name = "SubImageControl";
            this.Size = new System.Drawing.Size(258, 296);
            this.elementMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scaledWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaledHeightUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button resizeButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button opaqueButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private Components.RectangleControl frameControl;
        private Components.ListBox elementListBox;
        private System.Windows.Forms.ContextMenuStrip elementMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem elementAddToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementRemoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elementDownToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox localeCheckedListBox;
        private SubImageScreenControl screenControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox borderXCheckBox;
        private System.Windows.Forms.CheckBox borderYCheckBox;
        private System.Windows.Forms.Label label1;
        private Components.Vector2Control pivotControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown scaledWidthUpDown;
        private System.Windows.Forms.NumericUpDown scaledHeightUpDown;
    }
}
