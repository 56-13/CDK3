namespace CDK.Assets.Texturing
{
    partial class MaterialControl
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
            this.panel = new CDK.Assets.Components.StackPanel();
            this.originPanel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.localCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.originControl = new CDK.Assets.AssetSelectControl();
            this.shaderPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.shaderComboBox = new CDK.Assets.Components.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.distortionPanel = new System.Windows.Forms.Panel();
            this.distortionScaleControl = new CDK.Assets.Components.TrackControl();
            this.label11 = new System.Windows.Forms.Label();
            this.blendLayerPanel = new System.Windows.Forms.Panel();
            this.blendLayerComboBox = new CDK.Assets.Components.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.blendModePanel = new System.Windows.Forms.Panel();
            this.blendModeComboBox = new CDK.Assets.Components.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.shapePanel = new CDK.Assets.Components.StackPanel();
            this.cullModePanel = new System.Windows.Forms.Panel();
            this.cullModeComboBox = new CDK.Assets.Components.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.clipPanel = new System.Windows.Forms.Panel();
            this.depthBiasControl = new CDK.Assets.Components.TrackControl();
            this.depthTestCheckBox = new System.Windows.Forms.CheckBox();
            this.alphaTestBiasControl = new CDK.Assets.Components.TrackControl();
            this.alphaTestCheckBox = new System.Windows.Forms.CheckBox();
            this.displacementPanel = new System.Windows.Forms.Panel();
            this.displacementScaleControl = new CDK.Assets.Components.TrackControl();
            this.label4 = new System.Windows.Forms.Label();
            this.colorPanel = new CDK.Assets.Components.StackPanel();
            this.bloomCheckBox = new System.Windows.Forms.CheckBox();
            this.colorControl = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.colorAnimationPanel = new System.Windows.Forms.Panel();
            this.colorLoopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.colorDurationControl = new CDK.Assets.Components.TrackControl();
            this.lightPanel = new CDK.Assets.Components.StackPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.receiveShadow2DcheckBox = new System.Windows.Forms.CheckBox();
            this.receiveShadowCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rimControl = new CDK.Assets.Components.TrackControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.metallicControl = new CDK.Assets.Components.TrackControl();
            this.roughnessControl = new CDK.Assets.Components.TrackControl();
            this.ambientOcclusionControl = new CDK.Assets.Components.TrackControl();
            this.label10 = new System.Windows.Forms.Label();
            this.reflectionCheckBox = new System.Windows.Forms.CheckBox();
            this.emissionPanel = new CDK.Assets.Components.StackPanel();
            this.emissionControl = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.emissionAnimationPanel = new System.Windows.Forms.Panel();
            this.emissionLoopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.emissionDurationControl = new CDK.Assets.Components.TrackControl();
            this.uvAnimationPanel = new CDK.Assets.Components.StackPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.uvScrollAngleControl = new CDK.Assets.Components.TrackControl();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.uvScrollControl = new CDK.Assets.Components.TrackControl();
            this.panel.SuspendLayout();
            this.originPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.shaderPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.distortionPanel.SuspendLayout();
            this.blendLayerPanel.SuspendLayout();
            this.blendModePanel.SuspendLayout();
            this.shapePanel.SuspendLayout();
            this.cullModePanel.SuspendLayout();
            this.clipPanel.SuspendLayout();
            this.displacementPanel.SuspendLayout();
            this.colorPanel.SuspendLayout();
            this.colorAnimationPanel.SuspendLayout();
            this.lightPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.emissionPanel.SuspendLayout();
            this.emissionAnimationPanel.SuspendLayout();
            this.uvAnimationPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.originPanel);
            this.panel.Controls.Add(this.shaderPanel);
            this.panel.Controls.Add(this.shapePanel);
            this.panel.Controls.Add(this.colorPanel);
            this.panel.Controls.Add(this.lightPanel);
            this.panel.Controls.Add(this.emissionPanel);
            this.panel.Controls.Add(this.uvAnimationPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 782);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // originPanel
            // 
            this.originPanel.AutoSize = true;
            this.originPanel.Collapsible = true;
            this.originPanel.Controls.Add(this.panel1);
            this.originPanel.Location = new System.Drawing.Point(0, 0);
            this.originPanel.Margin = new System.Windows.Forms.Padding(0);
            this.originPanel.Name = "originPanel";
            this.originPanel.Size = new System.Drawing.Size(320, 78);
            this.originPanel.TabIndex = 61;
            this.originPanel.Title = "Material Asset";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.localCheckBox);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.loadButton);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.originControl);
            this.panel1.Location = new System.Drawing.Point(3, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 51);
            this.panel1.TabIndex = 0;
            // 
            // localCheckBox
            // 
            this.localCheckBox.AutoSize = true;
            this.localCheckBox.Checked = true;
            this.localCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localCheckBox.Location = new System.Drawing.Point(44, 2);
            this.localCheckBox.Name = "localCheckBox";
            this.localCheckBox.Size = new System.Drawing.Size(55, 16);
            this.localCheckBox.TabIndex = 52;
            this.localCheckBox.Text = "Local";
            this.localCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(291, 28);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(23, 23);
            this.saveButton.TabIndex = 51;
            this.saveButton.Text = "↑";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Location = new System.Drawing.Point(262, 28);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(23, 23);
            this.loadButton.TabIndex = 50;
            this.loadButton.Text = "↓";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 12);
            this.label5.TabIndex = 49;
            this.label5.Text = "Origin";
            // 
            // originControl
            // 
            this.originControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.originControl.Location = new System.Drawing.Point(105, 0);
            this.originControl.Name = "originControl";
            this.originControl.Size = new System.Drawing.Size(209, 22);
            this.originControl.TabIndex = 48;
            this.originControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            this.originControl.Importing += new System.EventHandler<CDK.Assets.AssetSelectImportEventArgs>(this.TextureControl_Importing);
            // 
            // shaderPanel
            // 
            this.shaderPanel.AutoSize = true;
            this.shaderPanel.Collapsible = true;
            this.shaderPanel.Controls.Add(this.panel2);
            this.shaderPanel.Controls.Add(this.distortionPanel);
            this.shaderPanel.Controls.Add(this.blendLayerPanel);
            this.shaderPanel.Controls.Add(this.blendModePanel);
            this.shaderPanel.Location = new System.Drawing.Point(0, 78);
            this.shaderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.shaderPanel.Name = "shaderPanel";
            this.shaderPanel.Size = new System.Drawing.Size(320, 125);
            this.shaderPanel.TabIndex = 76;
            this.shaderPanel.Title = "Material Shader";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.shaderComboBox);
            this.panel2.Controls.Add(this.label20);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 20);
            this.panel2.TabIndex = 67;
            // 
            // shaderComboBox
            // 
            this.shaderComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shaderComboBox.FormattingEnabled = true;
            this.shaderComboBox.Location = new System.Drawing.Point(90, 0);
            this.shaderComboBox.Name = "shaderComboBox";
            this.shaderComboBox.Size = new System.Drawing.Size(224, 20);
            this.shaderComboBox.TabIndex = 40;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(0, 3);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(45, 12);
            this.label20.TabIndex = 39;
            this.label20.Text = "Shader";
            // 
            // distortionPanel
            // 
            this.distortionPanel.Controls.Add(this.distortionScaleControl);
            this.distortionPanel.Controls.Add(this.label11);
            this.distortionPanel.Location = new System.Drawing.Point(3, 50);
            this.distortionPanel.Name = "distortionPanel";
            this.distortionPanel.Size = new System.Drawing.Size(314, 20);
            this.distortionPanel.TabIndex = 72;
            // 
            // distortionScaleControl
            // 
            this.distortionScaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.distortionScaleControl.Increment = 1F;
            this.distortionScaleControl.Location = new System.Drawing.Point(90, 0);
            this.distortionScaleControl.Maximum = 100F;
            this.distortionScaleControl.Minimum = 0F;
            this.distortionScaleControl.Name = "distortionScaleControl";
            this.distortionScaleControl.Size = new System.Drawing.Size(224, 21);
            this.distortionScaleControl.TabIndex = 30;
            this.distortionScaleControl.Value = 0F;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 4);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 12);
            this.label11.TabIndex = 29;
            this.label11.Text = "Distortion";
            // 
            // blendLayerPanel
            // 
            this.blendLayerPanel.Controls.Add(this.blendLayerComboBox);
            this.blendLayerPanel.Controls.Add(this.label12);
            this.blendLayerPanel.Location = new System.Drawing.Point(3, 76);
            this.blendLayerPanel.Name = "blendLayerPanel";
            this.blendLayerPanel.Size = new System.Drawing.Size(314, 20);
            this.blendLayerPanel.TabIndex = 68;
            // 
            // blendLayerComboBox
            // 
            this.blendLayerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blendLayerComboBox.FormattingEnabled = true;
            this.blendLayerComboBox.Location = new System.Drawing.Point(90, 0);
            this.blendLayerComboBox.Name = "blendLayerComboBox";
            this.blendLayerComboBox.Size = new System.Drawing.Size(224, 20);
            this.blendLayerComboBox.TabIndex = 60;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 3);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 12);
            this.label12.TabIndex = 59;
            this.label12.Text = "Blend Layer";
            // 
            // blendModePanel
            // 
            this.blendModePanel.Controls.Add(this.blendModeComboBox);
            this.blendModePanel.Controls.Add(this.label8);
            this.blendModePanel.Location = new System.Drawing.Point(3, 102);
            this.blendModePanel.Name = "blendModePanel";
            this.blendModePanel.Size = new System.Drawing.Size(314, 20);
            this.blendModePanel.TabIndex = 69;
            // 
            // blendModeComboBox
            // 
            this.blendModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blendModeComboBox.FormattingEnabled = true;
            this.blendModeComboBox.Location = new System.Drawing.Point(90, 0);
            this.blendModeComboBox.Name = "blendModeComboBox";
            this.blendModeComboBox.Size = new System.Drawing.Size(224, 20);
            this.blendModeComboBox.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 12);
            this.label8.TabIndex = 38;
            this.label8.Text = "Blend Mode";
            // 
            // shapePanel
            // 
            this.shapePanel.AutoSize = true;
            this.shapePanel.Collapsible = true;
            this.shapePanel.Controls.Add(this.cullModePanel);
            this.shapePanel.Controls.Add(this.clipPanel);
            this.shapePanel.Controls.Add(this.displacementPanel);
            this.shapePanel.Location = new System.Drawing.Point(0, 203);
            this.shapePanel.Margin = new System.Windows.Forms.Padding(0);
            this.shapePanel.Name = "shapePanel";
            this.shapePanel.Size = new System.Drawing.Size(320, 126);
            this.shapePanel.TabIndex = 77;
            this.shapePanel.Title = "Material Shape";
            // 
            // cullModePanel
            // 
            this.cullModePanel.Controls.Add(this.cullModeComboBox);
            this.cullModePanel.Controls.Add(this.label9);
            this.cullModePanel.Location = new System.Drawing.Point(3, 24);
            this.cullModePanel.Name = "cullModePanel";
            this.cullModePanel.Size = new System.Drawing.Size(314, 20);
            this.cullModePanel.TabIndex = 70;
            // 
            // cullModeComboBox
            // 
            this.cullModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cullModeComboBox.FormattingEnabled = true;
            this.cullModeComboBox.Location = new System.Drawing.Point(90, 0);
            this.cullModeComboBox.Name = "cullModeComboBox";
            this.cullModeComboBox.Size = new System.Drawing.Size(224, 20);
            this.cullModeComboBox.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 12);
            this.label9.TabIndex = 40;
            this.label9.Text = "Cull Mode";
            // 
            // clipPanel
            // 
            this.clipPanel.Controls.Add(this.depthBiasControl);
            this.clipPanel.Controls.Add(this.depthTestCheckBox);
            this.clipPanel.Controls.Add(this.alphaTestBiasControl);
            this.clipPanel.Controls.Add(this.alphaTestCheckBox);
            this.clipPanel.Location = new System.Drawing.Point(3, 50);
            this.clipPanel.Name = "clipPanel";
            this.clipPanel.Size = new System.Drawing.Size(314, 47);
            this.clipPanel.TabIndex = 60;
            // 
            // depthBiasControl
            // 
            this.depthBiasControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.depthBiasControl.Increment = 1F;
            this.depthBiasControl.Location = new System.Drawing.Point(90, 0);
            this.depthBiasControl.Maximum = 100F;
            this.depthBiasControl.Minimum = 0F;
            this.depthBiasControl.Name = "depthBiasControl";
            this.depthBiasControl.Size = new System.Drawing.Size(224, 21);
            this.depthBiasControl.TabIndex = 58;
            this.depthBiasControl.Value = 0F;
            // 
            // depthTestCheckBox
            // 
            this.depthTestCheckBox.AutoSize = true;
            this.depthTestCheckBox.Location = new System.Drawing.Point(0, 3);
            this.depthTestCheckBox.Name = "depthTestCheckBox";
            this.depthTestCheckBox.Size = new System.Drawing.Size(85, 16);
            this.depthTestCheckBox.TabIndex = 57;
            this.depthTestCheckBox.Text = "Depth Test";
            this.depthTestCheckBox.UseVisualStyleBackColor = true;
            // 
            // alphaTestBiasControl
            // 
            this.alphaTestBiasControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaTestBiasControl.DecimalPlaces = 3;
            this.alphaTestBiasControl.Increment = 0.01F;
            this.alphaTestBiasControl.Location = new System.Drawing.Point(90, 26);
            this.alphaTestBiasControl.Maximum = 0.5F;
            this.alphaTestBiasControl.Minimum = 0F;
            this.alphaTestBiasControl.Name = "alphaTestBiasControl";
            this.alphaTestBiasControl.Size = new System.Drawing.Size(224, 21);
            this.alphaTestBiasControl.TabIndex = 56;
            this.alphaTestBiasControl.Value = 0F;
            // 
            // alphaTestCheckBox
            // 
            this.alphaTestCheckBox.AutoSize = true;
            this.alphaTestCheckBox.Location = new System.Drawing.Point(0, 29);
            this.alphaTestCheckBox.Name = "alphaTestCheckBox";
            this.alphaTestCheckBox.Size = new System.Drawing.Size(85, 16);
            this.alphaTestCheckBox.TabIndex = 55;
            this.alphaTestCheckBox.Text = "Alpha Test";
            this.alphaTestCheckBox.UseVisualStyleBackColor = true;
            // 
            // displacementPanel
            // 
            this.displacementPanel.Controls.Add(this.displacementScaleControl);
            this.displacementPanel.Controls.Add(this.label4);
            this.displacementPanel.Location = new System.Drawing.Point(3, 103);
            this.displacementPanel.Name = "displacementPanel";
            this.displacementPanel.Size = new System.Drawing.Size(314, 20);
            this.displacementPanel.TabIndex = 71;
            // 
            // displacementScaleControl
            // 
            this.displacementScaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displacementScaleControl.DecimalPlaces = 1;
            this.displacementScaleControl.Increment = 0.1F;
            this.displacementScaleControl.Location = new System.Drawing.Point(90, 0);
            this.displacementScaleControl.Maximum = 20F;
            this.displacementScaleControl.Minimum = 0F;
            this.displacementScaleControl.Name = "displacementScaleControl";
            this.displacementScaleControl.Size = new System.Drawing.Size(224, 21);
            this.displacementScaleControl.TabIndex = 30;
            this.displacementScaleControl.Value = 0F;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 12);
            this.label4.TabIndex = 29;
            this.label4.Text = "Displacement";
            // 
            // colorPanel
            // 
            this.colorPanel.AutoSize = true;
            this.colorPanel.Collapsible = true;
            this.colorPanel.Controls.Add(this.bloomCheckBox);
            this.colorPanel.Controls.Add(this.colorControl);
            this.colorPanel.Controls.Add(this.colorAnimationPanel);
            this.colorPanel.Location = new System.Drawing.Point(0, 329);
            this.colorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(320, 123);
            this.colorPanel.TabIndex = 72;
            this.colorPanel.Title = "Material Color";
            // 
            // bloomCheckBox
            // 
            this.bloomCheckBox.AutoSize = true;
            this.bloomCheckBox.Location = new System.Drawing.Point(3, 24);
            this.bloomCheckBox.Name = "bloomCheckBox";
            this.bloomCheckBox.Size = new System.Drawing.Size(314, 16);
            this.bloomCheckBox.TabIndex = 46;
            this.bloomCheckBox.Text = "Bloom";
            this.bloomCheckBox.UseVisualStyleBackColor = true;
            // 
            // colorControl
            // 
            this.colorControl.Location = new System.Drawing.Point(3, 46);
            this.colorControl.Name = "colorControl";
            this.colorControl.Size = new System.Drawing.Size(314, 20);
            this.colorControl.TabIndex = 61;
            this.colorControl.Title = "Color";
            // 
            // colorAnimationPanel
            // 
            this.colorAnimationPanel.Controls.Add(this.colorLoopControl);
            this.colorAnimationPanel.Controls.Add(this.label7);
            this.colorAnimationPanel.Controls.Add(this.label6);
            this.colorAnimationPanel.Controls.Add(this.colorDurationControl);
            this.colorAnimationPanel.Location = new System.Drawing.Point(3, 72);
            this.colorAnimationPanel.Name = "colorAnimationPanel";
            this.colorAnimationPanel.Size = new System.Drawing.Size(314, 48);
            this.colorAnimationPanel.TabIndex = 65;
            // 
            // colorLoopControl
            // 
            this.colorLoopControl.Location = new System.Drawing.Point(87, 27);
            this.colorLoopControl.Name = "colorLoopControl";
            this.colorLoopControl.Size = new System.Drawing.Size(208, 21);
            this.colorLoopControl.TabIndex = 24;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 23;
            this.label7.Text = "Loop";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "Duration";
            // 
            // colorDurationControl
            // 
            this.colorDurationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorDurationControl.DecimalPlaces = 2;
            this.colorDurationControl.Increment = 0.01F;
            this.colorDurationControl.Location = new System.Drawing.Point(87, 0);
            this.colorDurationControl.Maximum = 10F;
            this.colorDurationControl.Minimum = 0F;
            this.colorDurationControl.Name = "colorDurationControl";
            this.colorDurationControl.Size = new System.Drawing.Size(227, 21);
            this.colorDurationControl.TabIndex = 0;
            this.colorDurationControl.Value = 0F;
            // 
            // lightPanel
            // 
            this.lightPanel.AutoSize = true;
            this.lightPanel.Collapsible = true;
            this.lightPanel.Controls.Add(this.panel3);
            this.lightPanel.Location = new System.Drawing.Point(0, 452);
            this.lightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.lightPanel.Name = "lightPanel";
            this.lightPanel.Size = new System.Drawing.Size(320, 154);
            this.lightPanel.TabIndex = 73;
            this.lightPanel.Title = "Material Light";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.receiveShadow2DcheckBox);
            this.panel3.Controls.Add(this.receiveShadowCheckBox);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.rimControl);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.metallicControl);
            this.panel3.Controls.Add(this.roughnessControl);
            this.panel3.Controls.Add(this.ambientOcclusionControl);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.reflectionCheckBox);
            this.panel3.Location = new System.Drawing.Point(3, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 127);
            this.panel3.TabIndex = 62;
            // 
            // receiveShadow2DcheckBox
            // 
            this.receiveShadow2DcheckBox.AutoSize = true;
            this.receiveShadow2DcheckBox.Location = new System.Drawing.Point(161, 3);
            this.receiveShadow2DcheckBox.Name = "receiveShadow2DcheckBox";
            this.receiveShadow2DcheckBox.Size = new System.Drawing.Size(88, 16);
            this.receiveShadow2DcheckBox.TabIndex = 51;
            this.receiveShadow2DcheckBox.Text = "Shadow 2D";
            this.receiveShadow2DcheckBox.UseVisualStyleBackColor = true;
            // 
            // receiveShadowCheckBox
            // 
            this.receiveShadowCheckBox.AutoSize = true;
            this.receiveShadowCheckBox.Location = new System.Drawing.Point(85, 3);
            this.receiveShadowCheckBox.Name = "receiveShadowCheckBox";
            this.receiveShadowCheckBox.Size = new System.Drawing.Size(70, 16);
            this.receiveShadowCheckBox.TabIndex = 50;
            this.receiveShadowCheckBox.Text = "Shadow";
            this.receiveShadowCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 21;
            this.label1.Text = "Metallic";
            // 
            // rimControl
            // 
            this.rimControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rimControl.DecimalPlaces = 3;
            this.rimControl.Increment = 0.01F;
            this.rimControl.Location = new System.Drawing.Point(90, 106);
            this.rimControl.Maximum = 1F;
            this.rimControl.Minimum = 0F;
            this.rimControl.Name = "rimControl";
            this.rimControl.Size = new System.Drawing.Size(224, 21);
            this.rimControl.TabIndex = 49;
            this.rimControl.Value = 1F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 23;
            this.label2.Text = "Roughness";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 12);
            this.label3.TabIndex = 48;
            this.label3.Text = "Rim Light";
            // 
            // metallicControl
            // 
            this.metallicControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metallicControl.DecimalPlaces = 3;
            this.metallicControl.Increment = 0.01F;
            this.metallicControl.Location = new System.Drawing.Point(90, 25);
            this.metallicControl.Maximum = 1F;
            this.metallicControl.Minimum = 0F;
            this.metallicControl.Name = "metallicControl";
            this.metallicControl.Size = new System.Drawing.Size(224, 21);
            this.metallicControl.TabIndex = 26;
            this.metallicControl.Value = 0.5F;
            // 
            // roughnessControl
            // 
            this.roughnessControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roughnessControl.DecimalPlaces = 3;
            this.roughnessControl.Increment = 0.01F;
            this.roughnessControl.Location = new System.Drawing.Point(90, 52);
            this.roughnessControl.Maximum = 1F;
            this.roughnessControl.Minimum = 0.001F;
            this.roughnessControl.Name = "roughnessControl";
            this.roughnessControl.Size = new System.Drawing.Size(224, 21);
            this.roughnessControl.TabIndex = 27;
            this.roughnessControl.Value = 0.5F;
            // 
            // ambientOcclusionControl
            // 
            this.ambientOcclusionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ambientOcclusionControl.DecimalPlaces = 3;
            this.ambientOcclusionControl.Increment = 0.01F;
            this.ambientOcclusionControl.Location = new System.Drawing.Point(90, 79);
            this.ambientOcclusionControl.Maximum = 2F;
            this.ambientOcclusionControl.Minimum = 0F;
            this.ambientOcclusionControl.Name = "ambientOcclusionControl";
            this.ambientOcclusionControl.Size = new System.Drawing.Size(224, 21);
            this.ambientOcclusionControl.TabIndex = 45;
            this.ambientOcclusionControl.Value = 1F;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(22, 12);
            this.label10.TabIndex = 44;
            this.label10.Text = "AO";
            // 
            // reflectionCheckBox
            // 
            this.reflectionCheckBox.AutoSize = true;
            this.reflectionCheckBox.Location = new System.Drawing.Point(0, 3);
            this.reflectionCheckBox.Name = "reflectionCheckBox";
            this.reflectionCheckBox.Size = new System.Drawing.Size(79, 16);
            this.reflectionCheckBox.TabIndex = 37;
            this.reflectionCheckBox.Text = "Reflection";
            this.reflectionCheckBox.UseVisualStyleBackColor = true;
            // 
            // emissionPanel
            // 
            this.emissionPanel.AutoSize = true;
            this.emissionPanel.Collapsible = true;
            this.emissionPanel.Controls.Add(this.emissionControl);
            this.emissionPanel.Controls.Add(this.emissionAnimationPanel);
            this.emissionPanel.Location = new System.Drawing.Point(0, 606);
            this.emissionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.emissionPanel.Name = "emissionPanel";
            this.emissionPanel.Size = new System.Drawing.Size(320, 101);
            this.emissionPanel.TabIndex = 74;
            this.emissionPanel.Title = "Material Emission";
            // 
            // emissionControl
            // 
            this.emissionControl.Location = new System.Drawing.Point(3, 24);
            this.emissionControl.Name = "emissionControl";
            this.emissionControl.Size = new System.Drawing.Size(314, 20);
            this.emissionControl.TabIndex = 63;
            this.emissionControl.Title = "Emission";
            // 
            // emissionAnimationPanel
            // 
            this.emissionAnimationPanel.Controls.Add(this.emissionLoopControl);
            this.emissionAnimationPanel.Controls.Add(this.label16);
            this.emissionAnimationPanel.Controls.Add(this.label17);
            this.emissionAnimationPanel.Controls.Add(this.emissionDurationControl);
            this.emissionAnimationPanel.Location = new System.Drawing.Point(3, 50);
            this.emissionAnimationPanel.Name = "emissionAnimationPanel";
            this.emissionAnimationPanel.Size = new System.Drawing.Size(314, 48);
            this.emissionAnimationPanel.TabIndex = 66;
            // 
            // emissionLoopControl
            // 
            this.emissionLoopControl.Location = new System.Drawing.Point(93, 27);
            this.emissionLoopControl.Name = "emissionLoopControl";
            this.emissionLoopControl.Size = new System.Drawing.Size(208, 21);
            this.emissionLoopControl.TabIndex = 24;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(0, 31);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 12);
            this.label16.TabIndex = 23;
            this.label16.Text = "Loop";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(0, 4);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(51, 12);
            this.label17.TabIndex = 22;
            this.label17.Text = "Duration";
            // 
            // emissionDurationControl
            // 
            this.emissionDurationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionDurationControl.DecimalPlaces = 2;
            this.emissionDurationControl.Increment = 0.01F;
            this.emissionDurationControl.Location = new System.Drawing.Point(90, 0);
            this.emissionDurationControl.Maximum = 10F;
            this.emissionDurationControl.Minimum = 0F;
            this.emissionDurationControl.Name = "emissionDurationControl";
            this.emissionDurationControl.Size = new System.Drawing.Size(224, 21);
            this.emissionDurationControl.TabIndex = 0;
            this.emissionDurationControl.Value = 0F;
            // 
            // uvAnimationPanel
            // 
            this.uvAnimationPanel.AutoSize = true;
            this.uvAnimationPanel.Collapsible = true;
            this.uvAnimationPanel.Controls.Add(this.panel4);
            this.uvAnimationPanel.Location = new System.Drawing.Point(0, 707);
            this.uvAnimationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.uvAnimationPanel.Name = "uvAnimationPanel";
            this.uvAnimationPanel.Size = new System.Drawing.Size(320, 75);
            this.uvAnimationPanel.TabIndex = 75;
            this.uvAnimationPanel.Title = "Material UV Animation";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.uvScrollAngleControl);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.label13);
            this.panel4.Controls.Add(this.uvScrollControl);
            this.panel4.Location = new System.Drawing.Point(3, 24);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(314, 48);
            this.panel4.TabIndex = 64;
            // 
            // uvScrollAngleControl
            // 
            this.uvScrollAngleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uvScrollAngleControl.Increment = 1F;
            this.uvScrollAngleControl.Location = new System.Drawing.Point(90, 27);
            this.uvScrollAngleControl.Maximum = 180F;
            this.uvScrollAngleControl.Minimum = -180F;
            this.uvScrollAngleControl.Name = "uvScrollAngleControl";
            this.uvScrollAngleControl.Size = new System.Drawing.Size(224, 21);
            this.uvScrollAngleControl.TabIndex = 66;
            this.uvScrollAngleControl.Value = 0F;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(0, 31);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 12);
            this.label14.TabIndex = 65;
            this.label14.Text = "Angle";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(0, 4);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 64;
            this.label13.Text = "Speed";
            // 
            // uvScrollControl
            // 
            this.uvScrollControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uvScrollControl.DecimalPlaces = 3;
            this.uvScrollControl.Increment = 0.001F;
            this.uvScrollControl.Location = new System.Drawing.Point(90, 0);
            this.uvScrollControl.Maximum = 1F;
            this.uvScrollControl.Minimum = 0F;
            this.uvScrollControl.Name = "uvScrollControl";
            this.uvScrollControl.Size = new System.Drawing.Size(224, 21);
            this.uvScrollControl.TabIndex = 63;
            this.uvScrollControl.Value = 0F;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(320, 782);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.originPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.shaderPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.distortionPanel.ResumeLayout(false);
            this.distortionPanel.PerformLayout();
            this.blendLayerPanel.ResumeLayout(false);
            this.blendLayerPanel.PerformLayout();
            this.blendModePanel.ResumeLayout(false);
            this.blendModePanel.PerformLayout();
            this.shapePanel.ResumeLayout(false);
            this.cullModePanel.ResumeLayout(false);
            this.cullModePanel.PerformLayout();
            this.clipPanel.ResumeLayout(false);
            this.clipPanel.PerformLayout();
            this.displacementPanel.ResumeLayout(false);
            this.displacementPanel.PerformLayout();
            this.colorPanel.ResumeLayout(false);
            this.colorPanel.PerformLayout();
            this.colorAnimationPanel.ResumeLayout(false);
            this.colorAnimationPanel.PerformLayout();
            this.lightPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.emissionPanel.ResumeLayout(false);
            this.emissionAnimationPanel.ResumeLayout(false);
            this.emissionAnimationPanel.PerformLayout();
            this.uvAnimationPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.StackPanel panel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label5;
        private AssetSelectControl originControl;
        private System.Windows.Forms.Panel clipPanel;
        private Components.ComboBox blendLayerComboBox;
        private System.Windows.Forms.Label label12;
        private Components.ComboBox blendModeComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private Components.TrackControl alphaTestBiasControl;
        private Components.ComboBox cullModeComboBox;
        private System.Windows.Forms.CheckBox alphaTestCheckBox;
        private Animations.Components.AnimationColorControl colorControl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private Components.TrackControl rimControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Components.TrackControl metallicControl;
        private Components.TrackControl roughnessControl;
        private System.Windows.Forms.CheckBox bloomCheckBox;
        private System.Windows.Forms.Label label4;
        private Components.TrackControl ambientOcclusionControl;
        private Components.TrackControl displacementScaleControl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox reflectionCheckBox;
        private Animations.Components.AnimationColorControl emissionControl;
        private System.Windows.Forms.Panel panel4;
        private Components.TrackControl uvScrollAngleControl;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private Components.TrackControl uvScrollControl;
        private System.Windows.Forms.Panel colorAnimationPanel;
        private Animations.Components.AnimationLoopControl colorLoopControl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Components.TrackControl colorDurationControl;
        private System.Windows.Forms.Panel emissionAnimationPanel;
        private Animations.Components.AnimationLoopControl emissionLoopControl;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private Components.TrackControl emissionDurationControl;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Panel panel2;
        private Components.ComboBox shaderComboBox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Panel blendLayerPanel;
        private System.Windows.Forms.Panel blendModePanel;
        private System.Windows.Forms.Panel cullModePanel;
        private System.Windows.Forms.Panel displacementPanel;
        private Components.StackPanel colorPanel;
        private Components.StackPanel lightPanel;
        private Components.StackPanel emissionPanel;
        private Components.StackPanel uvAnimationPanel;
        private Components.TrackControl depthBiasControl;
        private System.Windows.Forms.CheckBox depthTestCheckBox;
        private System.Windows.Forms.CheckBox receiveShadow2DcheckBox;
        private System.Windows.Forms.CheckBox receiveShadowCheckBox;
        private Components.StackPanel originPanel;
        private Components.StackPanel shaderPanel;
        private Components.StackPanel shapePanel;
        private System.Windows.Forms.Panel distortionPanel;
        private Components.TrackControl distortionScaleControl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox localCheckBox;
    }
}
