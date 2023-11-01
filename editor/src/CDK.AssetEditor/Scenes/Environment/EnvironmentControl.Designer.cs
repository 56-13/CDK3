
namespace CDK.Assets.Scenes
{
    partial class EnvironmentControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.shadowPanel = new System.Windows.Forms.Panel();
            this.allowShadowPixel32CheckBox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.allowShadowCheckBox = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.prefConfigRemoveButton = new System.Windows.Forms.Button();
            this.prefConfigAddButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fogPanel = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.fogCheckBox = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.prefConfigRenameButton = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lightConfigRenameButton = new System.Windows.Forms.Button();
            this.lightConfigRemoveButton = new System.Windows.Forms.Button();
            this.lightConfigAddButton = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.lightConfigComboBox = new CDK.Assets.Components.ComboBox();
            this.fogColorControl = new CDK.Assets.Components.ColorControl();
            this.fogFarControl = new CDK.Assets.Components.TrackControl();
            this.fogNearControl = new CDK.Assets.Components.TrackControl();
            this.ambientLightControl = new CDK.Assets.Components.ColorControl();
            this.skyboxControl = new CDK.Assets.AssetSelectControl();
            this.prefConfigComboBox = new CDK.Assets.Components.ComboBox();
            this.maxShadowResolutionComboBox = new CDK.Assets.Components.ComboBox();
            this.lightModeComboBox = new CDK.Assets.Components.ComboBox();
            this.bloomSizeControl = new CDK.Assets.Components.TrackControl();
            this.bloomThresholdControl = new CDK.Assets.Components.TrackControl();
            this.multisampleComboBox = new CDK.Assets.Components.ComboBox();
            this.gammaControl = new CDK.Assets.Components.TrackControl();
            this.exposureControl = new CDK.Assets.Components.TrackControl();
            this.bloomIntensityControl = new CDK.Assets.Components.TrackControl();
            this.cameraFarControl = new CDK.Assets.Components.TrackControl();
            this.cameraNearControl = new CDK.Assets.Components.TrackControl();
            this.cameraFovControl = new CDK.Assets.Components.TrackControl();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.shadowPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.fogPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cameraFarControl);
            this.groupBox1.Controls.Add(this.cameraNearControl);
            this.groupBox1.Controls.Add(this.cameraFovControl);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Camera";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Far";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Near";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fov";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Multisample";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.bloomSizeControl);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.bloomThresholdControl);
            this.groupBox3.Controls.Add(this.multisampleComboBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.gammaControl);
            this.groupBox3.Controls.Add(this.exposureControl);
            this.groupBox3.Controls.Add(this.bloomIntensityControl);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(0, 266);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 181);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Post Process";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "Bloom Threshold";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "Gamma";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Exposure";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Bloom Intensity";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.shadowPanel);
            this.groupBox5.Controls.Add(this.allowShadowCheckBox);
            this.groupBox5.Controls.Add(this.lightModeComboBox);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Location = new System.Drawing.Point(0, 134);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(320, 126);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Light Quality";
            // 
            // shadowPanel
            // 
            this.shadowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shadowPanel.Controls.Add(this.allowShadowPixel32CheckBox);
            this.shadowPanel.Controls.Add(this.maxShadowResolutionComboBox);
            this.shadowPanel.Controls.Add(this.label6);
            this.shadowPanel.Location = new System.Drawing.Point(3, 78);
            this.shadowPanel.Name = "shadowPanel";
            this.shadowPanel.Size = new System.Drawing.Size(314, 42);
            this.shadowPanel.TabIndex = 31;
            // 
            // allowShadowPixel32CheckBox
            // 
            this.allowShadowPixel32CheckBox.AutoSize = true;
            this.allowShadowPixel32CheckBox.Location = new System.Drawing.Point(3, 0);
            this.allowShadowPixel32CheckBox.Name = "allowShadowPixel32CheckBox";
            this.allowShadowPixel32CheckBox.Size = new System.Drawing.Size(178, 16);
            this.allowShadowPixel32CheckBox.TabIndex = 28;
            this.allowShadowPixel32CheckBox.Text = "Allow Shadow Precision 32";
            this.allowShadowPixel32CheckBox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "Max Shadow Resolution";
            // 
            // allowShadowCheckBox
            // 
            this.allowShadowCheckBox.AutoSize = true;
            this.allowShadowCheckBox.Location = new System.Drawing.Point(6, 51);
            this.allowShadowCheckBox.Name = "allowShadowCheckBox";
            this.allowShadowCheckBox.Size = new System.Drawing.Size(105, 16);
            this.allowShadowCheckBox.TabIndex = 27;
            this.allowShadowCheckBox.Text = "Allow Shadow";
            this.allowShadowCheckBox.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 12);
            this.label13.TabIndex = 25;
            this.label13.Text = "Light Mode";
            // 
            // prefConfigRemoveButton
            // 
            this.prefConfigRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prefConfigRemoveButton.Location = new System.Drawing.Point(298, 0);
            this.prefConfigRemoveButton.Name = "prefConfigRemoveButton";
            this.prefConfigRemoveButton.Size = new System.Drawing.Size(22, 22);
            this.prefConfigRemoveButton.TabIndex = 18;
            this.prefConfigRemoveButton.Text = "-";
            this.prefConfigRemoveButton.UseVisualStyleBackColor = true;
            this.prefConfigRemoveButton.Click += new System.EventHandler(this.PrefConfigRemoveButton_Click);
            // 
            // prefConfigAddButton
            // 
            this.prefConfigAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prefConfigAddButton.Location = new System.Drawing.Point(270, 0);
            this.prefConfigAddButton.Name = "prefConfigAddButton";
            this.prefConfigAddButton.Size = new System.Drawing.Size(22, 22);
            this.prefConfigAddButton.TabIndex = 17;
            this.prefConfigAddButton.Text = "+";
            this.prefConfigAddButton.UseVisualStyleBackColor = true;
            this.prefConfigAddButton.Click += new System.EventHandler(this.PrefConfigAddButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.fogPanel);
            this.groupBox2.Controls.Add(this.fogCheckBox);
            this.groupBox2.Controls.Add(this.ambientLightControl);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.skyboxControl);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Location = new System.Drawing.Point(0, 480);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(320, 182);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Light";
            // 
            // fogPanel
            // 
            this.fogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fogPanel.Controls.Add(this.fogColorControl);
            this.fogPanel.Controls.Add(this.label15);
            this.fogPanel.Controls.Add(this.fogFarControl);
            this.fogPanel.Controls.Add(this.label14);
            this.fogPanel.Controls.Add(this.label12);
            this.fogPanel.Controls.Add(this.fogNearControl);
            this.fogPanel.Location = new System.Drawing.Point(3, 72);
            this.fogPanel.Name = "fogPanel";
            this.fogPanel.Size = new System.Drawing.Size(314, 75);
            this.fogPanel.TabIndex = 22;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 12);
            this.label15.TabIndex = 21;
            this.label15.Text = "Fog Color";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(57, 12);
            this.label14.TabIndex = 0;
            this.label14.Text = "Fog Near";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 57);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 12);
            this.label12.TabIndex = 1;
            this.label12.Text = "Fog Far";
            // 
            // fogCheckBox
            // 
            this.fogCheckBox.AutoSize = true;
            this.fogCheckBox.Location = new System.Drawing.Point(6, 48);
            this.fogCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.fogCheckBox.Name = "fogCheckBox";
            this.fogCheckBox.Size = new System.Drawing.Size(45, 16);
            this.fogCheckBox.TabIndex = 20;
            this.fogCheckBox.Text = "Fog";
            this.fogCheckBox.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 12);
            this.label10.TabIndex = 15;
            this.label10.Text = "Ambient Light";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 156);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 18;
            this.label11.Text = "Skybox";
            // 
            // prefConfigRenameButton
            // 
            this.prefConfigRenameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.prefConfigRenameButton.Location = new System.Drawing.Point(236, 0);
            this.prefConfigRenameButton.Name = "prefConfigRenameButton";
            this.prefConfigRenameButton.Size = new System.Drawing.Size(28, 22);
            this.prefConfigRenameButton.TabIndex = 31;
            this.prefConfigRenameButton.Text = "Aa";
            this.prefConfigRenameButton.UseVisualStyleBackColor = true;
            this.prefConfigRenameButton.Click += new System.EventHandler(this.PrefConfigRenameButton_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(66, 12);
            this.label16.TabIndex = 6;
            this.label16.Text = "Preference";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 458);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 12);
            this.label17.TabIndex = 32;
            this.label17.Text = "Light";
            // 
            // lightConfigRenameButton
            // 
            this.lightConfigRenameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lightConfigRenameButton.Location = new System.Drawing.Point(236, 453);
            this.lightConfigRenameButton.Name = "lightConfigRenameButton";
            this.lightConfigRenameButton.Size = new System.Drawing.Size(28, 22);
            this.lightConfigRenameButton.TabIndex = 36;
            this.lightConfigRenameButton.Text = "Aa";
            this.lightConfigRenameButton.UseVisualStyleBackColor = true;
            this.lightConfigRenameButton.Click += new System.EventHandler(this.LightConfigRenameButton_Click);
            // 
            // lightConfigRemoveButton
            // 
            this.lightConfigRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lightConfigRemoveButton.Location = new System.Drawing.Point(298, 453);
            this.lightConfigRemoveButton.Name = "lightConfigRemoveButton";
            this.lightConfigRemoveButton.Size = new System.Drawing.Size(22, 22);
            this.lightConfigRemoveButton.TabIndex = 35;
            this.lightConfigRemoveButton.Text = "-";
            this.lightConfigRemoveButton.UseVisualStyleBackColor = true;
            this.lightConfigRemoveButton.Click += new System.EventHandler(this.LightConfigRemoveButton_Click);
            // 
            // lightConfigAddButton
            // 
            this.lightConfigAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lightConfigAddButton.Location = new System.Drawing.Point(270, 453);
            this.lightConfigAddButton.Name = "lightConfigAddButton";
            this.lightConfigAddButton.Size = new System.Drawing.Size(22, 22);
            this.lightConfigAddButton.TabIndex = 34;
            this.lightConfigAddButton.Text = "+";
            this.lightConfigAddButton.UseVisualStyleBackColor = true;
            this.lightConfigAddButton.Click += new System.EventHandler(this.LightConfigAddButton_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 50);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 12);
            this.label18.TabIndex = 14;
            this.label18.Text = "Bloom Size";
            // 
            // lightConfigComboBox
            // 
            this.lightConfigComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lightConfigComboBox.FormattingEnabled = true;
            this.lightConfigComboBox.ItemHeight = 12;
            this.lightConfigComboBox.Location = new System.Drawing.Point(115, 454);
            this.lightConfigComboBox.Name = "lightConfigComboBox";
            this.lightConfigComboBox.Size = new System.Drawing.Size(115, 20);
            this.lightConfigComboBox.TabIndex = 33;
            // 
            // fogColorControl
            // 
            this.fogColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fogColorControl.BackColor = System.Drawing.Color.White;
            this.fogColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fogColorControl.Location = new System.Drawing.Point(111, 0);
            this.fogColorControl.Name = "fogColorControl";
            this.fogColorControl.Size = new System.Drawing.Size(199, 21);
            this.fogColorControl.TabIndex = 23;
            this.fogColorControl.ValueGDI = System.Drawing.Color.Empty;
            // 
            // fogFarControl
            // 
            this.fogFarControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fogFarControl.Increment = 1F;
            this.fogFarControl.Location = new System.Drawing.Point(112, 54);
            this.fogFarControl.Maximum = 10000F;
            this.fogFarControl.Minimum = 0F;
            this.fogFarControl.Name = "fogFarControl";
            this.fogFarControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.fogFarControl.Size = new System.Drawing.Size(199, 21);
            this.fogFarControl.TabIndex = 3;
            this.fogFarControl.Value = 0F;
            // 
            // fogNearControl
            // 
            this.fogNearControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fogNearControl.Increment = 1F;
            this.fogNearControl.Location = new System.Drawing.Point(112, 27);
            this.fogNearControl.Maximum = 10000F;
            this.fogNearControl.Minimum = 0F;
            this.fogNearControl.Name = "fogNearControl";
            this.fogNearControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.fogNearControl.Size = new System.Drawing.Size(199, 21);
            this.fogNearControl.TabIndex = 2;
            this.fogNearControl.Value = 0F;
            // 
            // ambientLightControl
            // 
            this.ambientLightControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ambientLightControl.BackColor = System.Drawing.Color.White;
            this.ambientLightControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ambientLightControl.Location = new System.Drawing.Point(115, 20);
            this.ambientLightControl.Name = "ambientLightControl";
            this.ambientLightControl.Size = new System.Drawing.Size(199, 21);
            this.ambientLightControl.TabIndex = 14;
            this.ambientLightControl.ValueGDI = System.Drawing.Color.Empty;
            // 
            // skyboxControl
            // 
            this.skyboxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skyboxControl.Location = new System.Drawing.Point(114, 153);
            this.skyboxControl.Name = "skyboxControl";
            this.skyboxControl.Size = new System.Drawing.Size(199, 22);
            this.skyboxControl.TabIndex = 19;
            this.skyboxControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Skybox};
            // 
            // prefConfigComboBox
            // 
            this.prefConfigComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prefConfigComboBox.FormattingEnabled = true;
            this.prefConfigComboBox.ItemHeight = 12;
            this.prefConfigComboBox.Location = new System.Drawing.Point(115, 1);
            this.prefConfigComboBox.Name = "prefConfigComboBox";
            this.prefConfigComboBox.Size = new System.Drawing.Size(115, 20);
            this.prefConfigComboBox.TabIndex = 13;
            // 
            // maxShadowResolutionComboBox
            // 
            this.maxShadowResolutionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxShadowResolutionComboBox.FormattingEnabled = true;
            this.maxShadowResolutionComboBox.Location = new System.Drawing.Point(233, 22);
            this.maxShadowResolutionComboBox.Name = "maxShadowResolutionComboBox";
            this.maxShadowResolutionComboBox.Size = new System.Drawing.Size(78, 20);
            this.maxShadowResolutionComboBox.TabIndex = 30;
            // 
            // lightModeComboBox
            // 
            this.lightModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lightModeComboBox.FormattingEnabled = true;
            this.lightModeComboBox.Location = new System.Drawing.Point(236, 20);
            this.lightModeComboBox.Name = "lightModeComboBox";
            this.lightModeComboBox.Size = new System.Drawing.Size(78, 20);
            this.lightModeComboBox.TabIndex = 26;
            // 
            // bloomSizeControl
            // 
            this.bloomSizeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bloomSizeControl.Increment = 1F;
            this.bloomSizeControl.Location = new System.Drawing.Point(114, 46);
            this.bloomSizeControl.Maximum = 5F;
            this.bloomSizeControl.Minimum = 1F;
            this.bloomSizeControl.Name = "bloomSizeControl";
            this.bloomSizeControl.Size = new System.Drawing.Size(200, 21);
            this.bloomSizeControl.TabIndex = 15;
            this.bloomSizeControl.Value = 3F;
            // 
            // bloomThresholdControl
            // 
            this.bloomThresholdControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bloomThresholdControl.DecimalPlaces = 2;
            this.bloomThresholdControl.Increment = 0.01F;
            this.bloomThresholdControl.Location = new System.Drawing.Point(114, 100);
            this.bloomThresholdControl.Maximum = 2F;
            this.bloomThresholdControl.Minimum = 0F;
            this.bloomThresholdControl.Name = "bloomThresholdControl";
            this.bloomThresholdControl.Size = new System.Drawing.Size(200, 21);
            this.bloomThresholdControl.TabIndex = 12;
            this.bloomThresholdControl.Value = 1F;
            // 
            // multisampleComboBox
            // 
            this.multisampleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.multisampleComboBox.FormattingEnabled = true;
            this.multisampleComboBox.Location = new System.Drawing.Point(236, 20);
            this.multisampleComboBox.Name = "multisampleComboBox";
            this.multisampleComboBox.Size = new System.Drawing.Size(78, 20);
            this.multisampleComboBox.TabIndex = 8;
            // 
            // gammaControl
            // 
            this.gammaControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gammaControl.DecimalPlaces = 2;
            this.gammaControl.Increment = 0.01F;
            this.gammaControl.Location = new System.Drawing.Point(114, 154);
            this.gammaControl.Maximum = 3F;
            this.gammaControl.Minimum = 1F;
            this.gammaControl.Name = "gammaControl";
            this.gammaControl.Size = new System.Drawing.Size(200, 21);
            this.gammaControl.TabIndex = 11;
            this.gammaControl.Value = 1F;
            // 
            // exposureControl
            // 
            this.exposureControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exposureControl.DecimalPlaces = 2;
            this.exposureControl.Increment = 0.01F;
            this.exposureControl.Location = new System.Drawing.Point(114, 127);
            this.exposureControl.Maximum = 10F;
            this.exposureControl.Minimum = 0.5F;
            this.exposureControl.Name = "exposureControl";
            this.exposureControl.Size = new System.Drawing.Size(200, 21);
            this.exposureControl.TabIndex = 10;
            this.exposureControl.Value = 1F;
            // 
            // bloomIntensityControl
            // 
            this.bloomIntensityControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bloomIntensityControl.DecimalPlaces = 2;
            this.bloomIntensityControl.Increment = 0.01F;
            this.bloomIntensityControl.Location = new System.Drawing.Point(114, 73);
            this.bloomIntensityControl.Maximum = 10F;
            this.bloomIntensityControl.Minimum = 0F;
            this.bloomIntensityControl.Name = "bloomIntensityControl";
            this.bloomIntensityControl.Size = new System.Drawing.Size(200, 21);
            this.bloomIntensityControl.TabIndex = 6;
            this.bloomIntensityControl.Value = 1F;
            // 
            // cameraFarControl
            // 
            this.cameraFarControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraFarControl.Increment = 1F;
            this.cameraFarControl.Location = new System.Drawing.Point(115, 74);
            this.cameraFarControl.Maximum = 100000F;
            this.cameraFarControl.Minimum = 100F;
            this.cameraFarControl.Name = "cameraFarControl";
            this.cameraFarControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.cameraFarControl.Size = new System.Drawing.Size(199, 21);
            this.cameraFarControl.TabIndex = 5;
            this.cameraFarControl.Value = 10000F;
            // 
            // cameraNearControl
            // 
            this.cameraNearControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraNearControl.Increment = 1F;
            this.cameraNearControl.Location = new System.Drawing.Point(115, 47);
            this.cameraNearControl.Maximum = 99F;
            this.cameraNearControl.Minimum = 1F;
            this.cameraNearControl.Name = "cameraNearControl";
            this.cameraNearControl.Size = new System.Drawing.Size(199, 21);
            this.cameraNearControl.TabIndex = 4;
            this.cameraNearControl.Value = 10F;
            // 
            // cameraFovControl
            // 
            this.cameraFovControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cameraFovControl.Increment = 1F;
            this.cameraFovControl.Location = new System.Drawing.Point(115, 20);
            this.cameraFovControl.Maximum = 180F;
            this.cameraFovControl.Minimum = 0F;
            this.cameraFovControl.Name = "cameraFovControl";
            this.cameraFovControl.Size = new System.Drawing.Size(199, 21);
            this.cameraFovControl.TabIndex = 3;
            this.cameraFovControl.Value = 60F;
            // 
            // EnvironmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label17);
            this.Controls.Add(this.lightConfigRenameButton);
            this.Controls.Add(this.lightConfigRemoveButton);
            this.Controls.Add(this.lightConfigAddButton);
            this.Controls.Add(this.lightConfigComboBox);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.prefConfigRenameButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.prefConfigRemoveButton);
            this.Controls.Add(this.prefConfigAddButton);
            this.Controls.Add(this.prefConfigComboBox);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "EnvironmentControl";
            this.Size = new System.Drawing.Size(320, 662);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.shadowPanel.ResumeLayout(false);
            this.shadowPanel.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.fogPanel.ResumeLayout(false);
            this.fogPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Components.TrackControl cameraFarControl;
        private Components.TrackControl cameraNearControl;
        private Components.TrackControl cameraFovControl;
        private Components.ComboBox multisampleComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private Components.TrackControl gammaControl;
        private Components.TrackControl exposureControl;
        private Components.TrackControl bloomIntensityControl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label13;
        private Components.ComboBox lightModeComboBox;
        private Components.ComboBox prefConfigComboBox;
        private System.Windows.Forms.Button prefConfigRemoveButton;
        private System.Windows.Forms.Button prefConfigAddButton;
        private Components.ComboBox maxShadowResolutionComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox allowShadowPixel32CheckBox;
        private System.Windows.Forms.CheckBox allowShadowCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private Components.ColorControl ambientLightControl;
        private System.Windows.Forms.Label label10;
        private AssetSelectControl skyboxControl;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel fogPanel;
        private Components.ColorControl fogColorControl;
        private System.Windows.Forms.Label label15;
        private Components.TrackControl fogFarControl;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private Components.TrackControl fogNearControl;
        private System.Windows.Forms.CheckBox fogCheckBox;
        private System.Windows.Forms.Panel shadowPanel;
        private System.Windows.Forms.Button prefConfigRenameButton;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button lightConfigRenameButton;
        private System.Windows.Forms.Button lightConfigRemoveButton;
        private System.Windows.Forms.Button lightConfigAddButton;
        private Components.ComboBox lightConfigComboBox;
        private System.Windows.Forms.Label label9;
        private Components.TrackControl bloomThresholdControl;
        private Components.TrackControl bloomSizeControl;
        private System.Windows.Forms.Label label18;
    }
}
