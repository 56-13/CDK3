namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class ImageSpriteElementControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.imagePanel = new CDK.Assets.Components.StackPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rootImageControl = new CDK.Assets.AssetSelectControl();
            this.label7 = new System.Windows.Forms.Label();
            this.subImageControl = new CDK.Assets.Texturing.SubImageSelectControl();
            this.positionPanel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.alignComboBox = new CDK.Assets.Components.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.positionControl = new CDK.Assets.Components.Vector3Control();
            this.label1 = new System.Windows.Forms.Label();
            this.xControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.yControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.zControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.shadowPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.shadowTypeComboBox = new CDK.Assets.Components.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.shadowDistancePanel = new System.Windows.Forms.Panel();
            this.shadowDistanceUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.shadowRotatePanel = new System.Windows.Forms.Panel();
            this.shadowRotateFlatnessUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.shadowRotateOffsetControl = new CDK.Assets.Components.Vector2Control();
            this.label8 = new System.Windows.Forms.Label();
            this.shadowFlatPanel = new System.Windows.Forms.Panel();
            this.shadowFlatOffsetControl = new CDK.Assets.Components.Vector2Control();
            this.shadowFlatYFlipCheckBox = new System.Windows.Forms.CheckBox();
            this.shadowFlatXFlipCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.imagePanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.shadowPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.shadowDistancePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowDistanceUpDown)).BeginInit();
            this.shadowRotatePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowRotateFlatnessUpDown)).BeginInit();
            this.shadowFlatPanel.SuspendLayout();
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
            this.splitContainer.Panel1.Controls.Add(this.mainPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.subPanel);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(320, 1372);
            this.splitContainer.SplitterDistance = 155;
            this.splitContainer.TabIndex = 2;
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.imagePanel);
            this.mainPanel.Controls.Add(this.positionPanel);
            this.mainPanel.Controls.Add(this.shadowPanel);
            this.mainPanel.Controls.Add(this.materialControl);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 1372);
            this.mainPanel.TabIndex = 1;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // imagePanel
            // 
            this.imagePanel.AutoSize = true;
            this.imagePanel.Collapsible = true;
            this.imagePanel.Controls.Add(this.panel3);
            this.imagePanel.Controls.Add(this.subImageControl);
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Margin = new System.Windows.Forms.Padding(0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(320, 255);
            this.imagePanel.TabIndex = 9;
            this.imagePanel.Title = "Image";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rootImageControl);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(3, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 22);
            this.panel3.TabIndex = 6;
            // 
            // rootImageControl
            // 
            this.rootImageControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rootImageControl.Location = new System.Drawing.Point(90, 0);
            this.rootImageControl.Name = "rootImageControl";
            this.rootImageControl.Size = new System.Drawing.Size(224, 22);
            this.rootImageControl.TabIndex = 50;
            this.rootImageControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.RootImage};
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Image";
            // 
            // subImageControl
            // 
            this.subImageControl.Location = new System.Drawing.Point(3, 52);
            this.subImageControl.Name = "subImageControl";
            this.subImageControl.Size = new System.Drawing.Size(314, 200);
            this.subImageControl.TabIndex = 5;
            // 
            // positionPanel
            // 
            this.positionPanel.AutoSize = true;
            this.positionPanel.Collapsible = true;
            this.positionPanel.Controls.Add(this.panel1);
            this.positionPanel.Controls.Add(this.xControl);
            this.positionPanel.Controls.Add(this.yControl);
            this.positionPanel.Controls.Add(this.zControl);
            this.positionPanel.Location = new System.Drawing.Point(0, 255);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(320, 152);
            this.positionPanel.TabIndex = 10;
            this.positionPanel.Title = "Position";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.alignComboBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.positionControl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 47);
            this.panel1.TabIndex = 1;
            // 
            // alignComboBox
            // 
            this.alignComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alignComboBox.FormattingEnabled = true;
            this.alignComboBox.Location = new System.Drawing.Point(90, 27);
            this.alignComboBox.Name = "alignComboBox";
            this.alignComboBox.Size = new System.Drawing.Size(224, 20);
            this.alignComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Align";
            // 
            // positionControl
            // 
            this.positionControl.DecimalPlaces = 2;
            this.positionControl.Increment = 1F;
            this.positionControl.Location = new System.Drawing.Point(90, 0);
            this.positionControl.Maximum = 10000F;
            this.positionControl.Minimum = -10000F;
            this.positionControl.Name = "positionControl";
            this.positionControl.Size = new System.Drawing.Size(192, 21);
            this.positionControl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Position";
            // 
            // xControl
            // 
            this.xControl.Location = new System.Drawing.Point(3, 77);
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.xControl.Size = new System.Drawing.Size(314, 20);
            this.xControl.TabIndex = 2;
            this.xControl.Title = "X";
            // 
            // yControl
            // 
            this.yControl.Location = new System.Drawing.Point(3, 103);
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.yControl.Size = new System.Drawing.Size(314, 20);
            this.yControl.TabIndex = 4;
            this.yControl.Title = "Y";
            // 
            // zControl
            // 
            this.zControl.Location = new System.Drawing.Point(3, 129);
            this.zControl.Name = "zControl";
            this.zControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.zControl.Size = new System.Drawing.Size(314, 20);
            this.zControl.TabIndex = 3;
            this.zControl.Title = "Z";
            // 
            // shadowPanel
            // 
            this.shadowPanel.AutoSize = true;
            this.shadowPanel.Collapsible = true;
            this.shadowPanel.Controls.Add(this.panel2);
            this.shadowPanel.Controls.Add(this.shadowDistancePanel);
            this.shadowPanel.Controls.Add(this.shadowRotatePanel);
            this.shadowPanel.Controls.Add(this.shadowFlatPanel);
            this.shadowPanel.Location = new System.Drawing.Point(0, 407);
            this.shadowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.shadowPanel.Name = "shadowPanel";
            this.shadowPanel.Size = new System.Drawing.Size(320, 177);
            this.shadowPanel.TabIndex = 11;
            this.shadowPanel.Title = "Shadow";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.shadowTypeComboBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 20);
            this.panel2.TabIndex = 5;
            // 
            // shadowTypeComboBox
            // 
            this.shadowTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shadowTypeComboBox.FormattingEnabled = true;
            this.shadowTypeComboBox.Location = new System.Drawing.Point(90, 0);
            this.shadowTypeComboBox.Name = "shadowTypeComboBox";
            this.shadowTypeComboBox.Size = new System.Drawing.Size(224, 20);
            this.shadowTypeComboBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Shadow";
            // 
            // shadowDistancePanel
            // 
            this.shadowDistancePanel.Controls.Add(this.shadowDistanceUpDown);
            this.shadowDistancePanel.Controls.Add(this.label4);
            this.shadowDistancePanel.Location = new System.Drawing.Point(3, 50);
            this.shadowDistancePanel.Name = "shadowDistancePanel";
            this.shadowDistancePanel.Size = new System.Drawing.Size(314, 21);
            this.shadowDistancePanel.TabIndex = 6;
            // 
            // shadowDistanceUpDown
            // 
            this.shadowDistanceUpDown.DecimalPlaces = 2;
            this.shadowDistanceUpDown.Location = new System.Drawing.Point(148, 0);
            this.shadowDistanceUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.shadowDistanceUpDown.Name = "shadowDistanceUpDown";
            this.shadowDistanceUpDown.Size = new System.Drawing.Size(60, 21);
            this.shadowDistanceUpDown.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Shadow Distance";
            // 
            // shadowRotatePanel
            // 
            this.shadowRotatePanel.Controls.Add(this.shadowRotateFlatnessUpDown);
            this.shadowRotatePanel.Controls.Add(this.label6);
            this.shadowRotatePanel.Controls.Add(this.shadowRotateOffsetControl);
            this.shadowRotatePanel.Controls.Add(this.label8);
            this.shadowRotatePanel.Location = new System.Drawing.Point(3, 77);
            this.shadowRotatePanel.Name = "shadowRotatePanel";
            this.shadowRotatePanel.Size = new System.Drawing.Size(314, 48);
            this.shadowRotatePanel.TabIndex = 8;
            // 
            // shadowRotateFlatnessUpDown
            // 
            this.shadowRotateFlatnessUpDown.DecimalPlaces = 2;
            this.shadowRotateFlatnessUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.shadowRotateFlatnessUpDown.Location = new System.Drawing.Point(148, 27);
            this.shadowRotateFlatnessUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.shadowRotateFlatnessUpDown.Name = "shadowRotateFlatnessUpDown";
            this.shadowRotateFlatnessUpDown.Size = new System.Drawing.Size(60, 21);
            this.shadowRotateFlatnessUpDown.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "Shadow Rotate Flatness";
            // 
            // shadowRotateOffsetControl
            // 
            this.shadowRotateOffsetControl.DecimalPlaces = 2;
            this.shadowRotateOffsetControl.Increment = 1F;
            this.shadowRotateOffsetControl.Location = new System.Drawing.Point(148, 0);
            this.shadowRotateOffsetControl.Maximum = 100F;
            this.shadowRotateOffsetControl.Minimum = -100F;
            this.shadowRotateOffsetControl.Name = "shadowRotateOffsetControl";
            this.shadowRotateOffsetControl.PointGDI = new System.Drawing.Point(0, 0);
            this.shadowRotateOffsetControl.Size = new System.Drawing.Size(126, 21);
            this.shadowRotateOffsetControl.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 2);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "Shadow Rotate Offset";
            // 
            // shadowFlatPanel
            // 
            this.shadowFlatPanel.Controls.Add(this.shadowFlatOffsetControl);
            this.shadowFlatPanel.Controls.Add(this.shadowFlatYFlipCheckBox);
            this.shadowFlatPanel.Controls.Add(this.shadowFlatXFlipCheckBox);
            this.shadowFlatPanel.Controls.Add(this.label5);
            this.shadowFlatPanel.Location = new System.Drawing.Point(3, 131);
            this.shadowFlatPanel.Name = "shadowFlatPanel";
            this.shadowFlatPanel.Size = new System.Drawing.Size(314, 43);
            this.shadowFlatPanel.TabIndex = 7;
            // 
            // shadowFlatOffsetControl
            // 
            this.shadowFlatOffsetControl.DecimalPlaces = 2;
            this.shadowFlatOffsetControl.Increment = 1F;
            this.shadowFlatOffsetControl.Location = new System.Drawing.Point(148, 0);
            this.shadowFlatOffsetControl.Maximum = 100F;
            this.shadowFlatOffsetControl.Minimum = -100F;
            this.shadowFlatOffsetControl.Name = "shadowFlatOffsetControl";
            this.shadowFlatOffsetControl.PointGDI = new System.Drawing.Point(0, 0);
            this.shadowFlatOffsetControl.Size = new System.Drawing.Size(126, 21);
            this.shadowFlatOffsetControl.TabIndex = 9;
            // 
            // shadowFlatYFlipCheckBox
            // 
            this.shadowFlatYFlipCheckBox.AutoSize = true;
            this.shadowFlatYFlipCheckBox.Location = new System.Drawing.Point(148, 27);
            this.shadowFlatYFlipCheckBox.Name = "shadowFlatYFlipCheckBox";
            this.shadowFlatYFlipCheckBox.Size = new System.Drawing.Size(130, 16);
            this.shadowFlatYFlipCheckBox.TabIndex = 7;
            this.shadowFlatYFlipCheckBox.Text = "Shadow Flat Y Flip";
            this.shadowFlatYFlipCheckBox.UseVisualStyleBackColor = true;
            // 
            // shadowFlatXFlipCheckBox
            // 
            this.shadowFlatXFlipCheckBox.AutoSize = true;
            this.shadowFlatXFlipCheckBox.Location = new System.Drawing.Point(0, 27);
            this.shadowFlatXFlipCheckBox.Name = "shadowFlatXFlipCheckBox";
            this.shadowFlatXFlipCheckBox.Size = new System.Drawing.Size(130, 16);
            this.shadowFlatXFlipCheckBox.TabIndex = 6;
            this.shadowFlatXFlipCheckBox.Text = "Shadow Flat X Flip";
            this.shadowFlatXFlipCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Shadow Flat L/R";
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(3, 587);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(314, 782);
            this.materialControl.TabIndex = 0;
            // 
            // subPanel
            // 
            this.subPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subPanel.AutoSize = true;
            this.subPanel.Location = new System.Drawing.Point(0, 0);
            this.subPanel.Margin = new System.Windows.Forms.Padding(0);
            this.subPanel.Name = "subPanel";
            this.subPanel.Size = new System.Drawing.Size(155, 0);
            this.subPanel.TabIndex = 0;
            this.subPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // ImageSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "ImageSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 1372);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.imagePanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.positionPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.shadowPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.shadowDistancePanel.ResumeLayout(false);
            this.shadowDistancePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowDistanceUpDown)).EndInit();
            this.shadowRotatePanel.ResumeLayout(false);
            this.shadowRotatePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowRotateFlatnessUpDown)).EndInit();
            this.shadowFlatPanel.ResumeLayout(false);
            this.shadowFlatPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Assets.Components.StackPanel mainPanel;
        private System.Windows.Forms.Panel panel1;
        private Components.AnimationFloatControl xControl;
        private Components.AnimationFloatControl yControl;
        private Components.AnimationFloatControl zControl;
        private System.Windows.Forms.Label label2;
        private Assets.Components.ComboBox alignComboBox;
        private Assets.Components.Vector3Control positionControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private Assets.Components.ComboBox shadowTypeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel shadowDistancePanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel shadowFlatPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox shadowFlatXFlipCheckBox;
        private System.Windows.Forms.CheckBox shadowFlatYFlipCheckBox;
        private System.Windows.Forms.Panel shadowRotatePanel;
        private System.Windows.Forms.Label label8;
        private Assets.Components.Vector2Control shadowRotateOffsetControl;
        private Assets.Components.Vector2Control shadowFlatOffsetControl;
        private System.Windows.Forms.Label label6;
        private Assets.Components.NumericUpDown shadowRotateFlatnessUpDown;
        private Assets.Components.NumericUpDown shadowDistanceUpDown;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Assets.Components.StackPanel subPanel;
        private Assets.Components.StackPanel imagePanel;
        private Assets.Components.StackPanel positionPanel;
        private Assets.Components.StackPanel shadowPanel;
        private System.Windows.Forms.Panel panel3;
        private AssetSelectControl rootImageControl;
        private System.Windows.Forms.Label label7;
        private Texturing.SubImageSelectControl subImageControl;
        private Assets.Texturing.MaterialControl materialControl;
    }
}
