namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class BlurSpriteElementControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlurSpriteElementControl));
            this.panel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.modeComboBox = new CDK.Assets.Components.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.intensityControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.directionXControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.directionYControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.centerRangeControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.blendLayerComboBox = new CDK.Assets.Components.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.frameCheckBox = new System.Windows.Forms.CheckBox();
            this.frameControl = new CDK.Assets.Components.RectangleControl();
            this.depthDistanceCheckBox = new System.Windows.Forms.CheckBox();
            this.depthDistanceControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.depthRangeControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.centerXControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.centerYControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.panel2);
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.panel3);
            this.panel.Controls.Add(this.intensityControl);
            this.panel.Controls.Add(this.depthDistanceCheckBox);
            this.panel.Controls.Add(this.depthDistanceControl);
            this.panel.Controls.Add(this.depthRangeControl);
            this.panel.Controls.Add(this.directionXControl);
            this.panel.Controls.Add(this.directionYControl);
            this.panel.Controls.Add(this.centerXControl);
            this.panel.Controls.Add(this.centerYControl);
            this.panel.Controls.Add(this.centerRangeControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(360, 303);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.modeComboBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 26);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(360, 20);
            this.panel1.TabIndex = 8;
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(90, 0);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(270, 20);
            this.modeComboBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mode";
            // 
            // intensityControl
            // 
            this.intensityControl.Location = new System.Drawing.Point(0, 79);
            this.intensityControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.intensityControl.Name = "intensityControl";
            this.intensityControl.Size = new System.Drawing.Size(360, 20);
            this.intensityControl.TabIndex = 5;
            this.intensityControl.Title = "Intensity";
            // 
            // directionXControl
            // 
            this.directionXControl.Location = new System.Drawing.Point(0, 179);
            this.directionXControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.directionXControl.Name = "directionXControl";
            this.directionXControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.directionXControl.Size = new System.Drawing.Size(360, 20);
            this.directionXControl.TabIndex = 7;
            this.directionXControl.Title = "Direction X";
            // 
            // directionYControl
            // 
            this.directionYControl.Location = new System.Drawing.Point(0, 205);
            this.directionYControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.directionYControl.Name = "directionYControl";
            this.directionYControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.directionYControl.Size = new System.Drawing.Size(360, 20);
            this.directionYControl.TabIndex = 6;
            this.directionYControl.Title = "Direction Y";
            // 
            // centerRangeControl
            // 
            this.centerRangeControl.Location = new System.Drawing.Point(0, 283);
            this.centerRangeControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.centerRangeControl.Name = "centerRangeControl";
            this.centerRangeControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.centerRangeControl.Size = new System.Drawing.Size(360, 20);
            this.centerRangeControl.TabIndex = 9;
            this.centerRangeControl.Title = "Center Range";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.blendLayerComboBox);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(360, 20);
            this.panel2.TabIndex = 11;
            // 
            // blendLayerComboBox
            // 
            this.blendLayerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blendLayerComboBox.FormattingEnabled = true;
            this.blendLayerComboBox.Location = new System.Drawing.Point(90, 0);
            this.blendLayerComboBox.Name = "blendLayerComboBox";
            this.blendLayerComboBox.Size = new System.Drawing.Size(270, 20);
            this.blendLayerComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Blend Layer";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.frameControl);
            this.panel3.Controls.Add(this.frameCheckBox);
            this.panel3.Location = new System.Drawing.Point(0, 52);
            this.panel3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(360, 21);
            this.panel3.TabIndex = 9;
            // 
            // frameCheckBox
            // 
            this.frameCheckBox.AutoSize = true;
            this.frameCheckBox.Location = new System.Drawing.Point(0, 2);
            this.frameCheckBox.Name = "frameCheckBox";
            this.frameCheckBox.Size = new System.Drawing.Size(60, 16);
            this.frameCheckBox.TabIndex = 2;
            this.frameCheckBox.Text = "Frame";
            this.frameCheckBox.UseVisualStyleBackColor = true;
            // 
            // frameControl
            // 
            this.frameControl.Increment = 1F;
            this.frameControl.Location = new System.Drawing.Point(90, 0);
            this.frameControl.Maximum = 1000F;
            this.frameControl.Minimum = -1000F;
            this.frameControl.Name = "frameControl";
            this.frameControl.Size = new System.Drawing.Size(258, 21);
            this.frameControl.TabIndex = 3;
            // 
            // depthDistanceCheckBox
            // 
            this.depthDistanceCheckBox.AutoSize = true;
            this.depthDistanceCheckBox.Location = new System.Drawing.Point(0, 105);
            this.depthDistanceCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.depthDistanceCheckBox.Name = "depthDistanceCheckBox";
            this.depthDistanceCheckBox.Size = new System.Drawing.Size(360, 16);
            this.depthDistanceCheckBox.TabIndex = 2;
            this.depthDistanceCheckBox.Text = "Depth DIstance";
            this.depthDistanceCheckBox.UseVisualStyleBackColor = true;
            // 
            // depthDistanceControl
            // 
            this.depthDistanceControl.Location = new System.Drawing.Point(0, 127);
            this.depthDistanceControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.depthDistanceControl.Name = "depthDistanceControl";
            this.depthDistanceControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.depthDistanceControl.Size = new System.Drawing.Size(360, 20);
            this.depthDistanceControl.TabIndex = 12;
            this.depthDistanceControl.Title = "Depth Distance";
            // 
            // depthRangeControl
            // 
            this.depthRangeControl.Location = new System.Drawing.Point(0, 153);
            this.depthRangeControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.depthRangeControl.Name = "depthRangeControl";
            this.depthRangeControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.depthRangeControl.Size = new System.Drawing.Size(360, 20);
            this.depthRangeControl.TabIndex = 13;
            this.depthRangeControl.Title = "Depth Range";
            // 
            // centerXControl
            // 
            this.centerXControl.Location = new System.Drawing.Point(0, 231);
            this.centerXControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.centerXControl.Name = "centerXControl";
            this.centerXControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.centerXControl.Size = new System.Drawing.Size(360, 20);
            this.centerXControl.TabIndex = 15;
            this.centerXControl.Title = "Center X";
            // 
            // centerYControl
            // 
            this.centerYControl.Location = new System.Drawing.Point(0, 257);
            this.centerYControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.centerYControl.Name = "centerYControl";
            this.centerYControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.centerYControl.Size = new System.Drawing.Size(360, 20);
            this.centerYControl.TabIndex = 14;
            this.centerYControl.Title = "Center Y";
            // 
            // BlurSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "BlurSpriteElementControl";
            this.Size = new System.Drawing.Size(360, 303);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.StackPanel panel;
        private Components.AnimationFloatControl intensityControl;
        private Components.AnimationFloatControl directionXControl;
        private Components.AnimationFloatControl directionYControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private Components.AnimationFloatControl centerRangeControl;
        private Assets.Components.ComboBox modeComboBox;
        private System.Windows.Forms.Panel panel2;
        private Assets.Components.ComboBox blendLayerComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox frameCheckBox;
        private Assets.Components.RectangleControl frameControl;
        private System.Windows.Forms.CheckBox depthDistanceCheckBox;
        private Components.AnimationFloatControl depthDistanceControl;
        private Components.AnimationFloatControl depthRangeControl;
        private Components.AnimationFloatControl centerXControl;
        private Components.AnimationFloatControl centerYControl;
    }
}
