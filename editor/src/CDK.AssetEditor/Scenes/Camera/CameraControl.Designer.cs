namespace CDK.Assets.Scenes
{
    partial class CameraControl
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
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.targetCheckBox = new System.Windows.Forms.CheckBox();
            this.targetControl = new CDK.Assets.Scenes.GizmoControl();
            this.frustumCheckBox = new System.Windows.Forms.CheckBox();
            this.frustumPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.farControl = new CDK.Assets.Components.TrackControl();
            this.nearControl = new CDK.Assets.Components.TrackControl();
            this.fovControl = new CDK.Assets.Components.TrackControl();
            this.blurCheckBox = new System.Windows.Forms.CheckBox();
            this.blurPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.blurIntensityControl = new CDK.Assets.Components.TrackControl();
            this.label2 = new System.Windows.Forms.Label();
            this.blurRangeControl = new CDK.Assets.Components.TrackControl();
            this.blurDistanceControl = new CDK.Assets.Components.TrackControl();
            this.focusCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.targetControlCheckBox = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.frustumPanel.SuspendLayout();
            this.blurPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.objectControl);
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.targetControl);
            this.panel.Controls.Add(this.frustumCheckBox);
            this.panel.Controls.Add(this.frustumPanel);
            this.panel.Controls.Add(this.blurCheckBox);
            this.panel.Controls.Add(this.blurPanel);
            this.panel.Controls.Add(this.focusCheckBox);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 644);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(0, 3);
            this.objectControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(320, 205);
            this.objectControl.TabIndex = 1;
            // 
            // targetCheckBox
            // 
            this.targetCheckBox.AutoSize = true;
            this.targetCheckBox.Location = new System.Drawing.Point(0, 0);
            this.targetCheckBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.targetCheckBox.Name = "targetCheckBox";
            this.targetCheckBox.Size = new System.Drawing.Size(60, 16);
            this.targetCheckBox.TabIndex = 2;
            this.targetCheckBox.Text = "Target";
            this.targetCheckBox.UseVisualStyleBackColor = true;
            // 
            // targetControl
            // 
            this.targetControl.Location = new System.Drawing.Point(0, 236);
            this.targetControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.targetControl.Name = "targetControl";
            this.targetControl.Size = new System.Drawing.Size(320, 180);
            this.targetControl.TabIndex = 3;
            // 
            // frustumCheckBox
            // 
            this.frustumCheckBox.AutoSize = true;
            this.frustumCheckBox.Location = new System.Drawing.Point(0, 422);
            this.frustumCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.frustumCheckBox.Name = "frustumCheckBox";
            this.frustumCheckBox.Size = new System.Drawing.Size(320, 16);
            this.frustumCheckBox.TabIndex = 4;
            this.frustumCheckBox.Text = "Frustum";
            this.frustumCheckBox.UseVisualStyleBackColor = true;
            // 
            // frustumPanel
            // 
            this.frustumPanel.Controls.Add(this.label5);
            this.frustumPanel.Controls.Add(this.label4);
            this.frustumPanel.Controls.Add(this.label1);
            this.frustumPanel.Controls.Add(this.farControl);
            this.frustumPanel.Controls.Add(this.nearControl);
            this.frustumPanel.Controls.Add(this.fovControl);
            this.frustumPanel.Location = new System.Drawing.Point(0, 444);
            this.frustumPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.frustumPanel.Name = "frustumPanel";
            this.frustumPanel.Size = new System.Drawing.Size(320, 75);
            this.frustumPanel.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "Far";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Near";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Fov";
            // 
            // farControl
            // 
            this.farControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.farControl.Increment = 1F;
            this.farControl.Location = new System.Drawing.Point(91, 54);
            this.farControl.Maximum = 100000F;
            this.farControl.Minimum = 100F;
            this.farControl.Name = "farControl";
            this.farControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.farControl.Size = new System.Drawing.Size(229, 21);
            this.farControl.TabIndex = 7;
            this.farControl.Value = 10000F;
            // 
            // nearControl
            // 
            this.nearControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nearControl.Increment = 1F;
            this.nearControl.Location = new System.Drawing.Point(91, 27);
            this.nearControl.Maximum = 99F;
            this.nearControl.Minimum = 1F;
            this.nearControl.Name = "nearControl";
            this.nearControl.Size = new System.Drawing.Size(229, 21);
            this.nearControl.TabIndex = 6;
            this.nearControl.Value = 10F;
            // 
            // fovControl
            // 
            this.fovControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fovControl.Increment = 1F;
            this.fovControl.Location = new System.Drawing.Point(91, 0);
            this.fovControl.Maximum = 180F;
            this.fovControl.Minimum = 0F;
            this.fovControl.Name = "fovControl";
            this.fovControl.Size = new System.Drawing.Size(229, 21);
            this.fovControl.TabIndex = 5;
            this.fovControl.Value = 60F;
            // 
            // blurCheckBox
            // 
            this.blurCheckBox.AutoSize = true;
            this.blurCheckBox.Location = new System.Drawing.Point(0, 525);
            this.blurCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.blurCheckBox.Name = "blurCheckBox";
            this.blurCheckBox.Size = new System.Drawing.Size(320, 16);
            this.blurCheckBox.TabIndex = 6;
            this.blurCheckBox.Text = "Blur";
            this.blurCheckBox.UseVisualStyleBackColor = true;
            // 
            // blurPanel
            // 
            this.blurPanel.Controls.Add(this.label6);
            this.blurPanel.Controls.Add(this.label3);
            this.blurPanel.Controls.Add(this.blurIntensityControl);
            this.blurPanel.Controls.Add(this.label2);
            this.blurPanel.Controls.Add(this.blurRangeControl);
            this.blurPanel.Controls.Add(this.blurDistanceControl);
            this.blurPanel.Location = new System.Drawing.Point(0, 547);
            this.blurPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.blurPanel.Name = "blurPanel";
            this.blurPanel.Size = new System.Drawing.Size(320, 75);
            this.blurPanel.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 12);
            this.label6.TabIndex = 23;
            this.label6.Text = "Distance";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "Intensity";
            // 
            // blurIntensityControl
            // 
            this.blurIntensityControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blurIntensityControl.DecimalPlaces = 1;
            this.blurIntensityControl.Increment = 0.1F;
            this.blurIntensityControl.Location = new System.Drawing.Point(91, 54);
            this.blurIntensityControl.Maximum = 8F;
            this.blurIntensityControl.Minimum = 1F;
            this.blurIntensityControl.Name = "blurIntensityControl";
            this.blurIntensityControl.Size = new System.Drawing.Size(229, 21);
            this.blurIntensityControl.TabIndex = 21;
            this.blurIntensityControl.Value = 4F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "Range";
            // 
            // blurRangeControl
            // 
            this.blurRangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blurRangeControl.Increment = 1F;
            this.blurRangeControl.Location = new System.Drawing.Point(91, 27);
            this.blurRangeControl.Maximum = 10000F;
            this.blurRangeControl.Minimum = 1F;
            this.blurRangeControl.Name = "blurRangeControl";
            this.blurRangeControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.blurRangeControl.Size = new System.Drawing.Size(229, 21);
            this.blurRangeControl.TabIndex = 19;
            this.blurRangeControl.Value = 1000F;
            // 
            // blurDistanceControl
            // 
            this.blurDistanceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blurDistanceControl.Increment = 1F;
            this.blurDistanceControl.Location = new System.Drawing.Point(91, 0);
            this.blurDistanceControl.Maximum = 10000F;
            this.blurDistanceControl.Minimum = 0F;
            this.blurDistanceControl.Name = "blurDistanceControl";
            this.blurDistanceControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.blurDistanceControl.Size = new System.Drawing.Size(229, 21);
            this.blurDistanceControl.TabIndex = 17;
            this.blurDistanceControl.Value = 0F;
            // 
            // focusCheckBox
            // 
            this.focusCheckBox.AutoSize = true;
            this.focusCheckBox.Location = new System.Drawing.Point(0, 628);
            this.focusCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.focusCheckBox.Name = "focusCheckBox";
            this.focusCheckBox.Size = new System.Drawing.Size(320, 16);
            this.focusCheckBox.TabIndex = 7;
            this.focusCheckBox.Text = "Focus";
            this.focusCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.targetControlCheckBox);
            this.panel1.Controls.Add(this.targetCheckBox);
            this.panel1.Location = new System.Drawing.Point(3, 214);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 16);
            this.panel1.TabIndex = 8;
            // 
            // targetControlCheckBox
            // 
            this.targetControlCheckBox.AutoSize = true;
            this.targetControlCheckBox.Location = new System.Drawing.Point(60, 0);
            this.targetControlCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.targetControlCheckBox.Name = "targetControlCheckBox";
            this.targetControlCheckBox.Size = new System.Drawing.Size(84, 16);
            this.targetControlCheckBox.TabIndex = 3;
            this.targetControlCheckBox.Text = "Controlling";
            this.targetControlCheckBox.UseVisualStyleBackColor = true;
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "CameraControl";
            this.Size = new System.Drawing.Size(320, 644);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.frustumPanel.ResumeLayout(false);
            this.frustumPanel.PerformLayout();
            this.blurPanel.ResumeLayout(false);
            this.blurPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.StackPanel panel;
        private SceneObjectControl objectControl;
        private System.Windows.Forms.CheckBox targetCheckBox;
        private GizmoControl targetControl;
        private System.Windows.Forms.CheckBox frustumCheckBox;
        private System.Windows.Forms.Panel frustumPanel;
        private Components.TrackControl fovControl;
        private System.Windows.Forms.CheckBox blurCheckBox;
        private System.Windows.Forms.Panel blurPanel;
        private System.Windows.Forms.Label label3;
        private Components.TrackControl blurIntensityControl;
        private System.Windows.Forms.Label label2;
        private Components.TrackControl blurRangeControl;
        private Components.TrackControl blurDistanceControl;
        private System.Windows.Forms.CheckBox focusCheckBox;
        private Components.TrackControl farControl;
        private Components.TrackControl nearControl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox targetControlCheckBox;
    }
}
