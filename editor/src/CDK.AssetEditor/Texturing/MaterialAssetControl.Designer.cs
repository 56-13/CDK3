
namespace CDK.Assets.Texturing
{
    partial class MaterialAssetControl
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
            this.colorCheckBox = new System.Windows.Forms.CheckBox();
            this.normalCheckBox = new System.Windows.Forms.CheckBox();
            this.displacementCheckBox = new System.Windows.Forms.CheckBox();
            this.metallicCheckBox = new System.Windows.Forms.CheckBox();
            this.roughnessCheckBox = new System.Windows.Forms.CheckBox();
            this.ambientOcclusionCheckBox = new System.Windows.Forms.CheckBox();
            this.colorImportButton = new System.Windows.Forms.Button();
            this.colorClearButton = new System.Windows.Forms.Button();
            this.normalClearButton = new System.Windows.Forms.Button();
            this.normalImportButton = new System.Windows.Forms.Button();
            this.materialClearButton = new System.Windows.Forms.Button();
            this.materialImportButton = new System.Windows.Forms.Button();
            this.opacityCheckBox = new System.Windows.Forms.CheckBox();
            this.emissiveCheckBox = new System.Windows.Forms.CheckBox();
            this.emissiveClearButton = new System.Windows.Forms.Button();
            this.emissiveImportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.emissiveEncodingComboBox = new CDK.Assets.Components.ComboBox();
            this.descriptionControl = new CDK.Assets.Texturing.TextureDescriptionControl();
            this.materialEncodingComboBox = new CDK.Assets.Components.ComboBox();
            this.normalEncodingComboBox = new CDK.Assets.Components.ComboBox();
            this.colorEncodingComboBox = new CDK.Assets.Components.ComboBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.sourcePanel = new CDK.Assets.Components.StackPanel();
            this.panel = new System.Windows.Forms.Panel();
            this.emissiveReferenceControl = new CDK.Assets.AssetSelectControl();
            this.materialReferenceControl = new CDK.Assets.AssetSelectControl();
            this.normalReferenceControl = new CDK.Assets.AssetSelectControl();
            this.colorReferenceControl = new CDK.Assets.AssetSelectControl();
            this.descriptionPanel = new CDK.Assets.Components.StackPanel();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            this.mainPanel.SuspendLayout();
            this.sourcePanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.descriptionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorCheckBox
            // 
            this.colorCheckBox.AutoCheck = false;
            this.colorCheckBox.AutoSize = true;
            this.colorCheckBox.Location = new System.Drawing.Point(0, 31);
            this.colorCheckBox.Name = "colorCheckBox";
            this.colorCheckBox.Size = new System.Drawing.Size(62, 16);
            this.colorCheckBox.TabIndex = 0;
            this.colorCheckBox.Text = "Diffuse";
            this.colorCheckBox.UseVisualStyleBackColor = true;
            // 
            // normalCheckBox
            // 
            this.normalCheckBox.AutoCheck = false;
            this.normalCheckBox.AutoSize = true;
            this.normalCheckBox.Location = new System.Drawing.Point(0, 91);
            this.normalCheckBox.Name = "normalCheckBox";
            this.normalCheckBox.Size = new System.Drawing.Size(65, 16);
            this.normalCheckBox.TabIndex = 1;
            this.normalCheckBox.Text = "Normal";
            this.normalCheckBox.UseVisualStyleBackColor = true;
            // 
            // displacementCheckBox
            // 
            this.displacementCheckBox.AutoCheck = false;
            this.displacementCheckBox.AutoSize = true;
            this.displacementCheckBox.Location = new System.Drawing.Point(0, 113);
            this.displacementCheckBox.Name = "displacementCheckBox";
            this.displacementCheckBox.Size = new System.Drawing.Size(59, 16);
            this.displacementCheckBox.TabIndex = 2;
            this.displacementCheckBox.Text = "Height";
            this.displacementCheckBox.UseVisualStyleBackColor = true;
            // 
            // metallicCheckBox
            // 
            this.metallicCheckBox.AutoCheck = false;
            this.metallicCheckBox.AutoSize = true;
            this.metallicCheckBox.Location = new System.Drawing.Point(0, 153);
            this.metallicCheckBox.Name = "metallicCheckBox";
            this.metallicCheckBox.Size = new System.Drawing.Size(68, 16);
            this.metallicCheckBox.TabIndex = 3;
            this.metallicCheckBox.Text = "Metallic";
            this.metallicCheckBox.UseVisualStyleBackColor = true;
            // 
            // roughnessCheckBox
            // 
            this.roughnessCheckBox.AutoCheck = false;
            this.roughnessCheckBox.AutoSize = true;
            this.roughnessCheckBox.Location = new System.Drawing.Point(0, 175);
            this.roughnessCheckBox.Name = "roughnessCheckBox";
            this.roughnessCheckBox.Size = new System.Drawing.Size(88, 16);
            this.roughnessCheckBox.TabIndex = 4;
            this.roughnessCheckBox.Text = "Roughness";
            this.roughnessCheckBox.UseVisualStyleBackColor = true;
            // 
            // ambientOcclusionCheckBox
            // 
            this.ambientOcclusionCheckBox.AutoCheck = false;
            this.ambientOcclusionCheckBox.AutoSize = true;
            this.ambientOcclusionCheckBox.Location = new System.Drawing.Point(0, 197);
            this.ambientOcclusionCheckBox.Name = "ambientOcclusionCheckBox";
            this.ambientOcclusionCheckBox.Size = new System.Drawing.Size(41, 16);
            this.ambientOcclusionCheckBox.TabIndex = 5;
            this.ambientOcclusionCheckBox.Text = "AO";
            this.ambientOcclusionCheckBox.UseVisualStyleBackColor = true;
            // 
            // colorImportButton
            // 
            this.colorImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorImportButton.Location = new System.Drawing.Point(260, 28);
            this.colorImportButton.Name = "colorImportButton";
            this.colorImportButton.Size = new System.Drawing.Size(23, 22);
            this.colorImportButton.TabIndex = 7;
            this.colorImportButton.Text = "+";
            this.colorImportButton.UseVisualStyleBackColor = true;
            this.colorImportButton.Click += new System.EventHandler(this.ColorImportButton_Click);
            // 
            // colorClearButton
            // 
            this.colorClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorClearButton.Location = new System.Drawing.Point(289, 28);
            this.colorClearButton.Name = "colorClearButton";
            this.colorClearButton.Size = new System.Drawing.Size(23, 22);
            this.colorClearButton.TabIndex = 11;
            this.colorClearButton.Text = "-";
            this.colorClearButton.UseVisualStyleBackColor = true;
            this.colorClearButton.Click += new System.EventHandler(this.ColorClearButton_Click);
            // 
            // normalClearButton
            // 
            this.normalClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.normalClearButton.Location = new System.Drawing.Point(289, 88);
            this.normalClearButton.Name = "normalClearButton";
            this.normalClearButton.Size = new System.Drawing.Size(23, 22);
            this.normalClearButton.TabIndex = 14;
            this.normalClearButton.Text = "-";
            this.normalClearButton.UseVisualStyleBackColor = true;
            this.normalClearButton.Click += new System.EventHandler(this.NormalClearButton_Click);
            // 
            // normalImportButton
            // 
            this.normalImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.normalImportButton.Location = new System.Drawing.Point(260, 88);
            this.normalImportButton.Name = "normalImportButton";
            this.normalImportButton.Size = new System.Drawing.Size(23, 22);
            this.normalImportButton.TabIndex = 12;
            this.normalImportButton.Text = "+";
            this.normalImportButton.UseVisualStyleBackColor = true;
            this.normalImportButton.Click += new System.EventHandler(this.NormalImportButton_Click);
            // 
            // materialClearButton
            // 
            this.materialClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.materialClearButton.Location = new System.Drawing.Point(289, 149);
            this.materialClearButton.Name = "materialClearButton";
            this.materialClearButton.Size = new System.Drawing.Size(23, 22);
            this.materialClearButton.TabIndex = 17;
            this.materialClearButton.Text = "-";
            this.materialClearButton.UseVisualStyleBackColor = true;
            this.materialClearButton.Click += new System.EventHandler(this.MaterialClearButton_Click);
            // 
            // materialImportButton
            // 
            this.materialImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.materialImportButton.Location = new System.Drawing.Point(260, 149);
            this.materialImportButton.Name = "materialImportButton";
            this.materialImportButton.Size = new System.Drawing.Size(23, 22);
            this.materialImportButton.TabIndex = 15;
            this.materialImportButton.Text = "+";
            this.materialImportButton.UseVisualStyleBackColor = true;
            this.materialImportButton.Click += new System.EventHandler(this.MaterialImportButton_Click);
            // 
            // opacityCheckBox
            // 
            this.opacityCheckBox.AutoCheck = false;
            this.opacityCheckBox.AutoSize = true;
            this.opacityCheckBox.Location = new System.Drawing.Point(0, 53);
            this.opacityCheckBox.Name = "opacityCheckBox";
            this.opacityCheckBox.Size = new System.Drawing.Size(67, 16);
            this.opacityCheckBox.TabIndex = 19;
            this.opacityCheckBox.Text = "Opacity";
            this.opacityCheckBox.UseVisualStyleBackColor = true;
            // 
            // emissiveCheckBox
            // 
            this.emissiveCheckBox.AutoCheck = false;
            this.emissiveCheckBox.AutoSize = true;
            this.emissiveCheckBox.Location = new System.Drawing.Point(0, 224);
            this.emissiveCheckBox.Name = "emissiveCheckBox";
            this.emissiveCheckBox.Size = new System.Drawing.Size(76, 16);
            this.emissiveCheckBox.TabIndex = 32;
            this.emissiveCheckBox.Text = "Emissive";
            this.emissiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // emissiveClearButton
            // 
            this.emissiveClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emissiveClearButton.Location = new System.Drawing.Point(289, 221);
            this.emissiveClearButton.Name = "emissiveClearButton";
            this.emissiveClearButton.Size = new System.Drawing.Size(23, 22);
            this.emissiveClearButton.TabIndex = 35;
            this.emissiveClearButton.Text = "-";
            this.emissiveClearButton.UseVisualStyleBackColor = true;
            this.emissiveClearButton.Click += new System.EventHandler(this.EmissiveClearButton_Click);
            // 
            // emissiveImportButton
            // 
            this.emissiveImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emissiveImportButton.Location = new System.Drawing.Point(260, 221);
            this.emissiveImportButton.Name = "emissiveImportButton";
            this.emissiveImportButton.Size = new System.Drawing.Size(23, 22);
            this.emissiveImportButton.TabIndex = 34;
            this.emissiveImportButton.Text = "+";
            this.emissiveImportButton.UseVisualStyleBackColor = true;
            this.emissiveImportButton.Click += new System.EventHandler(this.EmissiveImportButton_Click);
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.Location = new System.Drawing.Point(0, 0);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(312, 22);
            this.importButton.TabIndex = 44;
            this.importButton.Text = "Import All";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // emissiveEncodingComboBox
            // 
            this.emissiveEncodingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissiveEncodingComboBox.FormattingEnabled = true;
            this.emissiveEncodingComboBox.Location = new System.Drawing.Point(90, 222);
            this.emissiveEncodingComboBox.Name = "emissiveEncodingComboBox";
            this.emissiveEncodingComboBox.Size = new System.Drawing.Size(164, 20);
            this.emissiveEncodingComboBox.TabIndex = 33;
            // 
            // descriptionControl
            // 
            this.descriptionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionControl.Location = new System.Drawing.Point(3, 24);
            this.descriptionControl.Name = "descriptionControl";
            this.descriptionControl.Size = new System.Drawing.Size(312, 150);
            this.descriptionControl.TabIndex = 18;
            // 
            // materialEncodingComboBox
            // 
            this.materialEncodingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialEncodingComboBox.FormattingEnabled = true;
            this.materialEncodingComboBox.Location = new System.Drawing.Point(90, 150);
            this.materialEncodingComboBox.Name = "materialEncodingComboBox";
            this.materialEncodingComboBox.Size = new System.Drawing.Size(164, 20);
            this.materialEncodingComboBox.TabIndex = 16;
            // 
            // normalEncodingComboBox
            // 
            this.normalEncodingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.normalEncodingComboBox.FormattingEnabled = true;
            this.normalEncodingComboBox.Location = new System.Drawing.Point(90, 89);
            this.normalEncodingComboBox.Name = "normalEncodingComboBox";
            this.normalEncodingComboBox.Size = new System.Drawing.Size(164, 20);
            this.normalEncodingComboBox.TabIndex = 13;
            // 
            // colorEncodingComboBox
            // 
            this.colorEncodingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorEncodingComboBox.FormattingEnabled = true;
            this.colorEncodingComboBox.Location = new System.Drawing.Point(90, 29);
            this.colorEncodingComboBox.Name = "colorEncodingComboBox";
            this.colorEncodingComboBox.Size = new System.Drawing.Size(164, 20);
            this.colorEncodingComboBox.TabIndex = 10;
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.sourcePanel);
            this.mainPanel.Controls.Add(this.descriptionPanel);
            this.mainPanel.Controls.Add(this.materialControl);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(318, 1257);
            this.mainPanel.TabIndex = 62;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // sourcePanel
            // 
            this.sourcePanel.AutoSize = true;
            this.sourcePanel.Collapsible = true;
            this.sourcePanel.Controls.Add(this.panel);
            this.sourcePanel.Location = new System.Drawing.Point(0, 0);
            this.sourcePanel.Margin = new System.Windows.Forms.Padding(0);
            this.sourcePanel.Name = "sourcePanel";
            this.sourcePanel.Size = new System.Drawing.Size(318, 298);
            this.sourcePanel.TabIndex = 64;
            this.sourcePanel.Title = "Source";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.emissiveReferenceControl);
            this.panel.Controls.Add(this.materialReferenceControl);
            this.panel.Controls.Add(this.normalReferenceControl);
            this.panel.Controls.Add(this.colorReferenceControl);
            this.panel.Controls.Add(this.importButton);
            this.panel.Controls.Add(this.colorCheckBox);
            this.panel.Controls.Add(this.normalCheckBox);
            this.panel.Controls.Add(this.displacementCheckBox);
            this.panel.Controls.Add(this.metallicCheckBox);
            this.panel.Controls.Add(this.roughnessCheckBox);
            this.panel.Controls.Add(this.ambientOcclusionCheckBox);
            this.panel.Controls.Add(this.colorImportButton);
            this.panel.Controls.Add(this.colorEncodingComboBox);
            this.panel.Controls.Add(this.normalImportButton);
            this.panel.Controls.Add(this.normalEncodingComboBox);
            this.panel.Controls.Add(this.materialImportButton);
            this.panel.Controls.Add(this.materialEncodingComboBox);
            this.panel.Controls.Add(this.opacityCheckBox);
            this.panel.Controls.Add(this.emissiveCheckBox);
            this.panel.Controls.Add(this.emissiveEncodingComboBox);
            this.panel.Controls.Add(this.emissiveImportButton);
            this.panel.Controls.Add(this.colorClearButton);
            this.panel.Controls.Add(this.normalClearButton);
            this.panel.Controls.Add(this.materialClearButton);
            this.panel.Controls.Add(this.emissiveClearButton);
            this.panel.Location = new System.Drawing.Point(3, 24);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(312, 271);
            this.panel.TabIndex = 0;
            // 
            // emissiveReferenceControl
            // 
            this.emissiveReferenceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissiveReferenceControl.Location = new System.Drawing.Point(90, 249);
            this.emissiveReferenceControl.Name = "emissiveReferenceControl";
            this.emissiveReferenceControl.Size = new System.Drawing.Size(222, 22);
            this.emissiveReferenceControl.TabIndex = 48;
            this.emissiveReferenceControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            // 
            // materialReferenceControl
            // 
            this.materialReferenceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialReferenceControl.Location = new System.Drawing.Point(90, 177);
            this.materialReferenceControl.Name = "materialReferenceControl";
            this.materialReferenceControl.Size = new System.Drawing.Size(222, 22);
            this.materialReferenceControl.TabIndex = 47;
            this.materialReferenceControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            // 
            // normalReferenceControl
            // 
            this.normalReferenceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.normalReferenceControl.Location = new System.Drawing.Point(90, 116);
            this.normalReferenceControl.Name = "normalReferenceControl";
            this.normalReferenceControl.Size = new System.Drawing.Size(222, 22);
            this.normalReferenceControl.TabIndex = 46;
            this.normalReferenceControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            // 
            // colorReferenceControl
            // 
            this.colorReferenceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorReferenceControl.Location = new System.Drawing.Point(90, 56);
            this.colorReferenceControl.Name = "colorReferenceControl";
            this.colorReferenceControl.Size = new System.Drawing.Size(222, 22);
            this.colorReferenceControl.TabIndex = 45;
            this.colorReferenceControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            // 
            // descriptionPanel
            // 
            this.descriptionPanel.AutoSize = true;
            this.descriptionPanel.Collapsible = true;
            this.descriptionPanel.Controls.Add(this.descriptionControl);
            this.descriptionPanel.Location = new System.Drawing.Point(0, 298);
            this.descriptionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.descriptionPanel.Name = "descriptionPanel";
            this.descriptionPanel.Size = new System.Drawing.Size(318, 177);
            this.descriptionPanel.TabIndex = 65;
            this.descriptionPanel.Title = "Texture";
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 475);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(318, 782);
            this.materialControl.TabIndex = 63;
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
            this.splitContainer.Size = new System.Drawing.Size(318, 1257);
            this.splitContainer.SplitterDistance = 157;
            this.splitContainer.TabIndex = 63;
            // 
            // subPanel
            // 
            this.subPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subPanel.AutoSize = true;
            this.subPanel.Location = new System.Drawing.Point(0, 0);
            this.subPanel.Margin = new System.Windows.Forms.Padding(0);
            this.subPanel.Name = "subPanel";
            this.subPanel.Size = new System.Drawing.Size(157, 0);
            this.subPanel.TabIndex = 0;
            this.subPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // MaterialAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "MaterialAssetControl";
            this.Size = new System.Drawing.Size(318, 1257);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.sourcePanel.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.descriptionPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox colorCheckBox;
        private System.Windows.Forms.CheckBox normalCheckBox;
        private System.Windows.Forms.CheckBox displacementCheckBox;
        private System.Windows.Forms.CheckBox metallicCheckBox;
        private System.Windows.Forms.CheckBox roughnessCheckBox;
        private System.Windows.Forms.CheckBox ambientOcclusionCheckBox;
        private System.Windows.Forms.Button colorImportButton;
        private Components.ComboBox colorEncodingComboBox;
        private System.Windows.Forms.Button colorClearButton;
        private System.Windows.Forms.Button normalClearButton;
        private Components.ComboBox normalEncodingComboBox;
        private System.Windows.Forms.Button normalImportButton;
        private System.Windows.Forms.Button materialClearButton;
        private Components.ComboBox materialEncodingComboBox;
        private System.Windows.Forms.Button materialImportButton;
        private TextureDescriptionControl descriptionControl;
        private System.Windows.Forms.CheckBox opacityCheckBox;
        private System.Windows.Forms.CheckBox emissiveCheckBox;
        private Components.ComboBox emissiveEncodingComboBox;
        private System.Windows.Forms.Button emissiveClearButton;
        private System.Windows.Forms.Button emissiveImportButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private Components.StackPanel mainPanel;
        private System.Windows.Forms.Panel panel;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Components.StackPanel subPanel;
        private Components.StackPanel sourcePanel;
        private Components.StackPanel descriptionPanel;
        private AssetSelectControl emissiveReferenceControl;
        private AssetSelectControl materialReferenceControl;
        private AssetSelectControl normalReferenceControl;
        private AssetSelectControl colorReferenceControl;
    }
}
