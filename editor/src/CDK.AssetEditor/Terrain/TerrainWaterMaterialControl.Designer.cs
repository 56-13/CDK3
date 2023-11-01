
namespace CDK.Assets.Terrain
{
    partial class TerrainWaterMaterialControl
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
            this.textureScaleControl = new CDK.Assets.Components.TrackControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.perturbIntensityControl = new CDK.Assets.Components.TrackControl();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.foamDepthControl = new CDK.Assets.Components.TrackControl();
            this.foamScaleControl = new CDK.Assets.Components.TrackControl();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.foamIntensityControl = new CDK.Assets.Components.TrackControl();
            this.foamTextureControl = new CDK.Assets.AssetSelectControl();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.angleControl = new CDK.Assets.Components.TrackControl();
            this.forwardSpeedControl = new CDK.Assets.Components.TrackControl();
            this.crossSpeedControl = new CDK.Assets.Components.TrackControl();
            this.waveDistanceControl = new CDK.Assets.Components.TrackControl();
            this.waveAltitudeControl = new CDK.Assets.Components.TrackControl();
            this.depthMaxControl = new CDK.Assets.Components.TrackControl();
            this.label13 = new System.Windows.Forms.Label();
            this.shallowColorControl = new CDK.Assets.Components.ColorControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.shapePanel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.foamPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.movementPanel = new CDK.Assets.Components.StackPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.depthPanel = new CDK.Assets.Components.StackPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel.SuspendLayout();
            this.shapePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.foamPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.movementPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.depthPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textureScaleControl
            // 
            this.textureScaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureScaleControl.DecimalPlaces = 2;
            this.textureScaleControl.Increment = 0.01F;
            this.textureScaleControl.Location = new System.Drawing.Point(90, 27);
            this.textureScaleControl.Maximum = 2F;
            this.textureScaleControl.Minimum = 0.01F;
            this.textureScaleControl.Name = "textureScaleControl";
            this.textureScaleControl.Size = new System.Drawing.Size(224, 21);
            this.textureScaleControl.TabIndex = 68;
            this.textureScaleControl.Value = 1F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 67;
            this.label2.Text = "Scale";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 12);
            this.label1.TabIndex = 66;
            this.label1.Text = "Perturbation";
            // 
            // perturbIntensityControl
            // 
            this.perturbIntensityControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.perturbIntensityControl.DecimalPlaces = 2;
            this.perturbIntensityControl.Increment = 0.01F;
            this.perturbIntensityControl.Location = new System.Drawing.Point(90, 0);
            this.perturbIntensityControl.Maximum = 0.1F;
            this.perturbIntensityControl.Minimum = 0F;
            this.perturbIntensityControl.Name = "perturbIntensityControl";
            this.perturbIntensityControl.Size = new System.Drawing.Size(224, 21);
            this.perturbIntensityControl.TabIndex = 65;
            this.perturbIntensityControl.Value = 0.05F;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 4);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 12);
            this.label12.TabIndex = 71;
            this.label12.Text = "Texture";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 70;
            this.label5.Text = "Depth";
            // 
            // foamDepthControl
            // 
            this.foamDepthControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foamDepthControl.Increment = 1F;
            this.foamDepthControl.Location = new System.Drawing.Point(90, 82);
            this.foamDepthControl.Maximum = 1000F;
            this.foamDepthControl.Minimum = 0F;
            this.foamDepthControl.Name = "foamDepthControl";
            this.foamDepthControl.Size = new System.Drawing.Size(224, 21);
            this.foamDepthControl.TabIndex = 69;
            this.foamDepthControl.Value = 0F;
            // 
            // foamScaleControl
            // 
            this.foamScaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foamScaleControl.DecimalPlaces = 2;
            this.foamScaleControl.Increment = 0.01F;
            this.foamScaleControl.Location = new System.Drawing.Point(90, 55);
            this.foamScaleControl.Maximum = 2F;
            this.foamScaleControl.Minimum = 0.01F;
            this.foamScaleControl.Name = "foamScaleControl";
            this.foamScaleControl.Size = new System.Drawing.Size(224, 21);
            this.foamScaleControl.TabIndex = 68;
            this.foamScaleControl.Value = 1F;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 67;
            this.label3.Text = "Scale";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 12);
            this.label4.TabIndex = 66;
            this.label4.Text = "Intensity";
            // 
            // foamIntensityControl
            // 
            this.foamIntensityControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foamIntensityControl.DecimalPlaces = 2;
            this.foamIntensityControl.Increment = 0.01F;
            this.foamIntensityControl.Location = new System.Drawing.Point(90, 28);
            this.foamIntensityControl.Maximum = 1F;
            this.foamIntensityControl.Minimum = 0.01F;
            this.foamIntensityControl.Name = "foamIntensityControl";
            this.foamIntensityControl.Size = new System.Drawing.Size(224, 21);
            this.foamIntensityControl.TabIndex = 65;
            this.foamIntensityControl.Value = 1F;
            // 
            // foamTextureControl
            // 
            this.foamTextureControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.foamTextureControl.Location = new System.Drawing.Point(90, 0);
            this.foamTextureControl.Name = "foamTextureControl";
            this.foamTextureControl.Size = new System.Drawing.Size(224, 22);
            this.foamTextureControl.TabIndex = 49;
            this.foamTextureControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.RootImage};
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 58);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 12);
            this.label10.TabIndex = 100;
            this.label10.Text = "Cross";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 12);
            this.label6.TabIndex = 99;
            this.label6.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 12);
            this.label7.TabIndex = 98;
            this.label7.Text = "Wave Height";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 12);
            this.label8.TabIndex = 97;
            this.label8.Text = "Wave Width";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 12);
            this.label9.TabIndex = 96;
            this.label9.Text = "Forward";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 4);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 12);
            this.label11.TabIndex = 95;
            this.label11.Text = "Angle";
            // 
            // angleControl
            // 
            this.angleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.angleControl.Increment = 1F;
            this.angleControl.Location = new System.Drawing.Point(90, 0);
            this.angleControl.Maximum = 360F;
            this.angleControl.Minimum = 0F;
            this.angleControl.Name = "angleControl";
            this.angleControl.Size = new System.Drawing.Size(224, 21);
            this.angleControl.TabIndex = 71;
            this.angleControl.Value = 0F;
            // 
            // forwardSpeedControl
            // 
            this.forwardSpeedControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.forwardSpeedControl.Increment = 1F;
            this.forwardSpeedControl.Location = new System.Drawing.Point(90, 27);
            this.forwardSpeedControl.Maximum = 100F;
            this.forwardSpeedControl.Minimum = 0F;
            this.forwardSpeedControl.Name = "forwardSpeedControl";
            this.forwardSpeedControl.Size = new System.Drawing.Size(224, 21);
            this.forwardSpeedControl.TabIndex = 101;
            this.forwardSpeedControl.Value = 0F;
            // 
            // crossSpeedControl
            // 
            this.crossSpeedControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crossSpeedControl.Increment = 1F;
            this.crossSpeedControl.Location = new System.Drawing.Point(90, 54);
            this.crossSpeedControl.Maximum = 100F;
            this.crossSpeedControl.Minimum = 0F;
            this.crossSpeedControl.Name = "crossSpeedControl";
            this.crossSpeedControl.Size = new System.Drawing.Size(224, 21);
            this.crossSpeedControl.TabIndex = 102;
            this.crossSpeedControl.Value = 0F;
            // 
            // waveDistanceControl
            // 
            this.waveDistanceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveDistanceControl.Increment = 1F;
            this.waveDistanceControl.Location = new System.Drawing.Point(90, 81);
            this.waveDistanceControl.Maximum = 90F;
            this.waveDistanceControl.Minimum = 0F;
            this.waveDistanceControl.Name = "waveDistanceControl";
            this.waveDistanceControl.Size = new System.Drawing.Size(224, 21);
            this.waveDistanceControl.TabIndex = 103;
            this.waveDistanceControl.Value = 0F;
            // 
            // waveAltitudeControl
            // 
            this.waveAltitudeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveAltitudeControl.Increment = 1F;
            this.waveAltitudeControl.Location = new System.Drawing.Point(90, 108);
            this.waveAltitudeControl.Maximum = 10F;
            this.waveAltitudeControl.Minimum = 0F;
            this.waveAltitudeControl.Name = "waveAltitudeControl";
            this.waveAltitudeControl.Size = new System.Drawing.Size(224, 21);
            this.waveAltitudeControl.TabIndex = 104;
            this.waveAltitudeControl.Value = 0F;
            // 
            // depthMaxControl
            // 
            this.depthMaxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.depthMaxControl.Increment = 1F;
            this.depthMaxControl.Location = new System.Drawing.Point(90, 0);
            this.depthMaxControl.Maximum = 1000F;
            this.depthMaxControl.Minimum = 0F;
            this.depthMaxControl.Name = "depthMaxControl";
            this.depthMaxControl.Size = new System.Drawing.Size(224, 21);
            this.depthMaxControl.TabIndex = 105;
            this.depthMaxControl.Value = 0F;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(0, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 12);
            this.label13.TabIndex = 109;
            this.label13.Text = "Shallow";
            // 
            // shallowColorControl
            // 
            this.shallowColorControl.AlphaChannel = true;
            this.shallowColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shallowColorControl.BackColor = System.Drawing.Color.White;
            this.shallowColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.shallowColorControl.Location = new System.Drawing.Point(90, 27);
            this.shallowColorControl.Name = "shallowColorControl";
            this.shallowColorControl.Size = new System.Drawing.Size(224, 21);
            this.shallowColorControl.TabIndex = 108;
            this.shallowColorControl.ValueGDI = System.Drawing.Color.Empty;
            // 
            // materialControl
            // 
            this.materialControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialControl.Location = new System.Drawing.Point(0, 75);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 69;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.shapePanel);
            this.panel.Controls.Add(this.materialControl);
            this.panel.Controls.Add(this.foamPanel);
            this.panel.Controls.Add(this.movementPanel);
            this.panel.Controls.Add(this.depthPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 1218);
            this.panel.TabIndex = 115;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // shapePanel
            // 
            this.shapePanel.AutoSize = true;
            this.shapePanel.Collapsible = true;
            this.shapePanel.Controls.Add(this.panel1);
            this.shapePanel.Location = new System.Drawing.Point(0, 0);
            this.shapePanel.Margin = new System.Windows.Forms.Padding(0);
            this.shapePanel.Name = "shapePanel";
            this.shapePanel.Size = new System.Drawing.Size(320, 75);
            this.shapePanel.TabIndex = 115;
            this.shapePanel.Title = "Shape";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.perturbIntensityControl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textureScaleControl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 48);
            this.panel1.TabIndex = 0;
            // 
            // foamPanel
            // 
            this.foamPanel.AutoSize = true;
            this.foamPanel.Collapsible = true;
            this.foamPanel.Controls.Add(this.panel2);
            this.foamPanel.Location = new System.Drawing.Point(0, 857);
            this.foamPanel.Margin = new System.Windows.Forms.Padding(0);
            this.foamPanel.Name = "foamPanel";
            this.foamPanel.Size = new System.Drawing.Size(320, 130);
            this.foamPanel.TabIndex = 116;
            this.foamPanel.Title = "Foam";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.foamDepthControl);
            this.panel2.Controls.Add(this.foamTextureControl);
            this.panel2.Controls.Add(this.foamScaleControl);
            this.panel2.Controls.Add(this.foamIntensityControl);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 103);
            this.panel2.TabIndex = 0;
            // 
            // movementPanel
            // 
            this.movementPanel.AutoSize = true;
            this.movementPanel.Collapsible = true;
            this.movementPanel.Controls.Add(this.panel3);
            this.movementPanel.Location = new System.Drawing.Point(0, 987);
            this.movementPanel.Margin = new System.Windows.Forms.Padding(0);
            this.movementPanel.Name = "movementPanel";
            this.movementPanel.Size = new System.Drawing.Size(320, 156);
            this.movementPanel.TabIndex = 117;
            this.movementPanel.Title = "Movement";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.angleControl);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.waveDistanceControl);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.crossSpeedControl);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.waveAltitudeControl);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.forwardSpeedControl);
            this.panel3.Location = new System.Drawing.Point(3, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 129);
            this.panel3.TabIndex = 0;
            // 
            // depthPanel
            // 
            this.depthPanel.AutoSize = true;
            this.depthPanel.Collapsible = true;
            this.depthPanel.Controls.Add(this.panel4);
            this.depthPanel.Location = new System.Drawing.Point(0, 1143);
            this.depthPanel.Margin = new System.Windows.Forms.Padding(0);
            this.depthPanel.Name = "depthPanel";
            this.depthPanel.Size = new System.Drawing.Size(320, 75);
            this.depthPanel.TabIndex = 118;
            this.depthPanel.Title = "Depth";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.depthMaxControl);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.shallowColorControl);
            this.panel4.Controls.Add(this.label13);
            this.panel4.Location = new System.Drawing.Point(3, 24);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(314, 48);
            this.panel4.TabIndex = 0;
            // 
            // TerrainWaterMaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "TerrainWaterMaterialControl";
            this.Size = new System.Drawing.Size(320, 1218);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.shapePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.foamPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.movementPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.depthPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Components.TrackControl textureScaleControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Components.TrackControl perturbIntensityControl;
        private Components.TrackControl foamScaleControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Components.TrackControl foamIntensityControl;
        private AssetSelectControl foamTextureControl;
        private System.Windows.Forms.Label label5;
        private Components.TrackControl foamDepthControl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private Components.TrackControl angleControl;
        private Components.TrackControl forwardSpeedControl;
        private Components.TrackControl crossSpeedControl;
        private Components.TrackControl waveDistanceControl;
        private Components.TrackControl waveAltitudeControl;
        private Components.TrackControl depthMaxControl;
        private System.Windows.Forms.Label label13;
        private Components.ColorControl shallowColorControl;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.Label label12;
        private Components.StackPanel panel;
        private Components.StackPanel shapePanel;
        private System.Windows.Forms.Panel panel1;
        private Components.StackPanel foamPanel;
        private System.Windows.Forms.Panel panel2;
        private Components.StackPanel movementPanel;
        private System.Windows.Forms.Panel panel3;
        private Components.StackPanel depthPanel;
        private System.Windows.Forms.Panel panel4;
    }
}
