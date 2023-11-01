
namespace CDK.Assets.Texturing
{
    partial class ImageAssetControl
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pivotCheckBox = new System.Windows.Forms.CheckBox();
            this.screenPanel = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cropButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.hasSubImagesCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.opaqueButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.packButton = new System.Windows.Forms.Button();
            this.replaceButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.ratioComboBox = new CDK.Assets.Components.ComboBox();
            this.screenControl = new CDK.Assets.Texturing.ImageScreenControl();
            this.subControl = new CDK.Assets.Texturing.SubImageControl();
            this.colorComboBox = new CDK.Assets.Components.ComboBox();
            this.scaledHeightUpDown = new CDK.Assets.Components.NumericUpDown();
            this.scaledWidthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.contentScaleComboBox = new CDK.Assets.Components.ComboBox();
            this.encodingComboBox = new CDK.Assets.Components.ComboBox();
            this.textureDescriptionControl = new CDK.Assets.Texturing.TextureDescriptionControl();
            this.encodedHeightUpDown = new CDK.Assets.Components.NumericUpDown();
            this.encodedWidthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.heightUpDown = new CDK.Assets.Components.NumericUpDown();
            this.widthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.containerAssetControl = new CDK.Assets.Containers.ContainerAssetControl();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.screenPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaledHeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaledWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodedHeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodedWidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1000, 762);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.pivotCheckBox);
            this.tabPage1.Controls.Add(this.ratioComboBox);
            this.tabPage1.Controls.Add(this.screenPanel);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.colorComboBox);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(992, 736);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Root";
            // 
            // pivotCheckBox
            // 
            this.pivotCheckBox.AutoSize = true;
            this.pivotCheckBox.Location = new System.Drawing.Point(178, 8);
            this.pivotCheckBox.Name = "pivotCheckBox";
            this.pivotCheckBox.Size = new System.Drawing.Size(51, 16);
            this.pivotCheckBox.TabIndex = 4;
            this.pivotCheckBox.Text = "Pivot";
            this.pivotCheckBox.UseVisualStyleBackColor = true;
            // 
            // screenPanel
            // 
            this.screenPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenPanel.AutoScroll = true;
            this.screenPanel.Controls.Add(this.screenControl);
            this.screenPanel.Location = new System.Drawing.Point(6, 32);
            this.screenPanel.Name = "screenPanel";
            this.screenPanel.Size = new System.Drawing.Size(704, 698);
            this.screenPanel.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.subControl);
            this.groupBox2.Location = new System.Drawing.Point(716, 378);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 352);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sub";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.scaledHeightUpDown);
            this.groupBox1.Controls.Add(this.scaledWidthUpDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cropButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.contentScaleComboBox);
            this.groupBox1.Controls.Add(this.encodingComboBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.hasSubImagesCheckBox);
            this.groupBox1.Controls.Add(this.textureDescriptionControl);
            this.groupBox1.Controls.Add(this.encodedHeightUpDown);
            this.groupBox1.Controls.Add(this.encodedWidthUpDown);
            this.groupBox1.Controls.Add(this.heightUpDown);
            this.groupBox1.Controls.Add(this.widthUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.opaqueButton);
            this.groupBox1.Controls.Add(this.clearButton);
            this.groupBox1.Controls.Add(this.packButton);
            this.groupBox1.Controls.Add(this.replaceButton);
            this.groupBox1.Controls.Add(this.addButton);
            this.groupBox1.Controls.Add(this.importButton);
            this.groupBox1.Location = new System.Drawing.Point(716, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 366);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Root";
            // 
            // cropButton
            // 
            this.cropButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cropButton.Location = new System.Drawing.Point(6, 48);
            this.cropButton.Name = "cropButton";
            this.cropButton.Size = new System.Drawing.Size(60, 23);
            this.cropButton.TabIndex = 20;
            this.cropButton.Text = "Crop";
            this.cropButton.UseVisualStyleBackColor = true;
            this.cropButton.Click += new System.EventHandler(this.CropButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 344);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "Content Scale";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "Encoding";
            // 
            // hasSubImagesCheckBox
            // 
            this.hasSubImagesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hasSubImagesCheckBox.AutoSize = true;
            this.hasSubImagesCheckBox.Location = new System.Drawing.Point(156, 343);
            this.hasSubImagesCheckBox.Name = "hasSubImagesCheckBox";
            this.hasSubImagesCheckBox.Size = new System.Drawing.Size(92, 16);
            this.hasSubImagesCheckBox.TabIndex = 15;
            this.hasSubImagesCheckBox.Text = "Sub Images";
            this.hasSubImagesCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "Encoded Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Origin Size";
            // 
            // opaqueButton
            // 
            this.opaqueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.opaqueButton.Location = new System.Drawing.Point(204, 48);
            this.opaqueButton.Name = "opaqueButton";
            this.opaqueButton.Size = new System.Drawing.Size(60, 23);
            this.opaqueButton.TabIndex = 5;
            this.opaqueButton.Text = "Opaque";
            this.opaqueButton.UseVisualStyleBackColor = true;
            this.opaqueButton.Click += new System.EventHandler(this.OpaqueButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(138, 48);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(60, 23);
            this.clearButton.TabIndex = 4;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // packButton
            // 
            this.packButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.packButton.Location = new System.Drawing.Point(72, 48);
            this.packButton.Name = "packButton";
            this.packButton.Size = new System.Drawing.Size(60, 23);
            this.packButton.TabIndex = 3;
            this.packButton.Text = "Pack";
            this.packButton.UseVisualStyleBackColor = true;
            this.packButton.Click += new System.EventHandler(this.PackButton_Click);
            // 
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.Location = new System.Drawing.Point(182, 19);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(82, 23);
            this.replaceButton.TabIndex = 2;
            this.replaceButton.Text = "Replace";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(94, 19);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(82, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.Location = new System.Drawing.Point(6, 19);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(82, 23);
            this.importButton.TabIndex = 0;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.containerAssetControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(992, 736);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sub";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 12);
            this.label5.TabIndex = 21;
            this.label5.Text = "Content Scaled Size";
            // 
            // ratioComboBox
            // 
            this.ratioComboBox.FormattingEnabled = true;
            this.ratioComboBox.Location = new System.Drawing.Point(92, 6);
            this.ratioComboBox.Name = "ratioComboBox";
            this.ratioComboBox.Size = new System.Drawing.Size(80, 20);
            this.ratioComboBox.TabIndex = 3;
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(0, 0);
            this.screenControl.TabIndex = 0;
            this.screenControl.Text = "imageScreenControl1";
            this.screenControl.SubAssetChanged += new System.EventHandler(this.ScreenControl_SubAssetChanged);
            // 
            // subControl
            // 
            this.subControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subControl.Location = new System.Drawing.Point(6, 20);
            this.subControl.Name = "subControl";
            this.subControl.Size = new System.Drawing.Size(258, 326);
            this.subControl.TabIndex = 0;
            // 
            // colorComboBox
            // 
            this.colorComboBox.FormattingEnabled = true;
            this.colorComboBox.Location = new System.Drawing.Point(6, 6);
            this.colorComboBox.Name = "colorComboBox";
            this.colorComboBox.Size = new System.Drawing.Size(80, 20);
            this.colorComboBox.TabIndex = 0;
            // 
            // scaledHeightUpDown
            // 
            this.scaledHeightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scaledHeightUpDown.DecimalPlaces = 1;
            this.scaledHeightUpDown.Location = new System.Drawing.Point(204, 131);
            this.scaledHeightUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.scaledHeightUpDown.Name = "scaledHeightUpDown";
            this.scaledHeightUpDown.ReadOnly = true;
            this.scaledHeightUpDown.Size = new System.Drawing.Size(60, 21);
            this.scaledHeightUpDown.TabIndex = 23;
            // 
            // scaledWidthUpDown
            // 
            this.scaledWidthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scaledWidthUpDown.DecimalPlaces = 1;
            this.scaledWidthUpDown.Location = new System.Drawing.Point(138, 131);
            this.scaledWidthUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.scaledWidthUpDown.Name = "scaledWidthUpDown";
            this.scaledWidthUpDown.ReadOnly = true;
            this.scaledWidthUpDown.Size = new System.Drawing.Size(60, 21);
            this.scaledWidthUpDown.TabIndex = 22;
            // 
            // contentScaleComboBox
            // 
            this.contentScaleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentScaleComboBox.FormattingEnabled = true;
            this.contentScaleComboBox.Location = new System.Drawing.Point(96, 340);
            this.contentScaleComboBox.Name = "contentScaleComboBox";
            this.contentScaleComboBox.Size = new System.Drawing.Size(54, 20);
            this.contentScaleComboBox.TabIndex = 18;
            // 
            // encodingComboBox
            // 
            this.encodingComboBox.FormattingEnabled = true;
            this.encodingComboBox.Location = new System.Drawing.Point(96, 158);
            this.encodingComboBox.Name = "encodingComboBox";
            this.encodingComboBox.Size = new System.Drawing.Size(168, 20);
            this.encodingComboBox.TabIndex = 17;
            // 
            // textureDescriptionControl
            // 
            this.textureDescriptionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureDescriptionControl.Location = new System.Drawing.Point(6, 184);
            this.textureDescriptionControl.Name = "textureDescriptionControl";
            this.textureDescriptionControl.Size = new System.Drawing.Size(258, 150);
            this.textureDescriptionControl.TabIndex = 12;
            // 
            // encodedHeightUpDown
            // 
            this.encodedHeightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.encodedHeightUpDown.Location = new System.Drawing.Point(204, 104);
            this.encodedHeightUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.encodedHeightUpDown.Name = "encodedHeightUpDown";
            this.encodedHeightUpDown.ReadOnly = true;
            this.encodedHeightUpDown.Size = new System.Drawing.Size(60, 21);
            this.encodedHeightUpDown.TabIndex = 11;
            // 
            // encodedWidthUpDown
            // 
            this.encodedWidthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.encodedWidthUpDown.Location = new System.Drawing.Point(138, 104);
            this.encodedWidthUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.encodedWidthUpDown.Name = "encodedWidthUpDown";
            this.encodedWidthUpDown.ReadOnly = true;
            this.encodedWidthUpDown.Size = new System.Drawing.Size(60, 21);
            this.encodedWidthUpDown.TabIndex = 10;
            // 
            // heightUpDown
            // 
            this.heightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.heightUpDown.Location = new System.Drawing.Point(204, 77);
            this.heightUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.ReadOnly = true;
            this.heightUpDown.Size = new System.Drawing.Size(60, 21);
            this.heightUpDown.TabIndex = 9;
            // 
            // widthUpDown
            // 
            this.widthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.widthUpDown.Location = new System.Drawing.Point(138, 77);
            this.widthUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.ReadOnly = true;
            this.widthUpDown.Size = new System.Drawing.Size(60, 21);
            this.widthUpDown.TabIndex = 8;
            // 
            // containerAssetControl
            // 
            this.containerAssetControl.BackColor = System.Drawing.SystemColors.Control;
            this.containerAssetControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerAssetControl.Location = new System.Drawing.Point(3, 3);
            this.containerAssetControl.Name = "containerAssetControl";
            this.containerAssetControl.Size = new System.Drawing.Size(986, 730);
            this.containerAssetControl.TabIndex = 0;
            // 
            // ImageAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "ImageAssetControl";
            this.Size = new System.Drawing.Size(1000, 762);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.screenPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scaledHeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaledWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodedHeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodedWidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private Components.ComboBox colorComboBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button opaqueButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button packButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button importButton;
        private Components.NumericUpDown encodedHeightUpDown;
        private Components.NumericUpDown encodedWidthUpDown;
        private Components.NumericUpDown heightUpDown;
        private Components.NumericUpDown widthUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private TextureDescriptionControl textureDescriptionControl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel screenPanel;
        private System.Windows.Forms.CheckBox hasSubImagesCheckBox;
        private SubImageControl subControl;
        private ImageScreenControl screenControl;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private Components.ComboBox encodingComboBox;
        private System.Windows.Forms.Label label4;
        private Containers.ContainerAssetControl containerAssetControl;
        private System.Windows.Forms.Label label3;
        private Components.ComboBox contentScaleComboBox;
        private System.Windows.Forms.Button cropButton;
        private Components.ComboBox ratioComboBox;
        private System.Windows.Forms.CheckBox pivotCheckBox;
        private System.Windows.Forms.Label label5;
        private Components.NumericUpDown scaledHeightUpDown;
        private Components.NumericUpDown scaledWidthUpDown;
    }
}
