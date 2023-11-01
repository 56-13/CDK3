namespace CDK.Assets.Animations.Particle.Shapes
{
    partial class ParticleShapeConeControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.topRangeControl = new CDK.Assets.Components.TrackControl();
            this.bottomRangeControl = new CDK.Assets.Components.TrackControl();
            this.heightControl = new CDK.Assets.Components.TrackControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Top";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Bottom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Height";
            // 
            // topRangeControl
            // 
            this.topRangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topRangeControl.DecimalPlaces = 2;
            this.topRangeControl.Increment = 1F;
            this.topRangeControl.Location = new System.Drawing.Point(90, 0);
            this.topRangeControl.Maximum = 5000F;
            this.topRangeControl.Minimum = 0F;
            this.topRangeControl.Name = "topRangeControl";
            this.topRangeControl.Size = new System.Drawing.Size(210, 21);
            this.topRangeControl.TabIndex = 5;
            this.topRangeControl.Value = 0F;
            // 
            // bottomRangeControl
            // 
            this.bottomRangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomRangeControl.DecimalPlaces = 2;
            this.bottomRangeControl.Increment = 1F;
            this.bottomRangeControl.Location = new System.Drawing.Point(90, 27);
            this.bottomRangeControl.Maximum = 5000F;
            this.bottomRangeControl.Minimum = 0F;
            this.bottomRangeControl.Name = "bottomRangeControl";
            this.bottomRangeControl.Size = new System.Drawing.Size(210, 21);
            this.bottomRangeControl.TabIndex = 6;
            this.bottomRangeControl.Value = 0F;
            // 
            // heightControl
            // 
            this.heightControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.heightControl.DecimalPlaces = 2;
            this.heightControl.Increment = 1F;
            this.heightControl.Location = new System.Drawing.Point(90, 54);
            this.heightControl.Maximum = 5000F;
            this.heightControl.Minimum = 0F;
            this.heightControl.Name = "heightControl";
            this.heightControl.Size = new System.Drawing.Size(210, 21);
            this.heightControl.TabIndex = 7;
            this.heightControl.Value = 0F;
            // 
            // ParticleShapeConeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heightControl);
            this.Controls.Add(this.bottomRangeControl);
            this.Controls.Add(this.topRangeControl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ParticleShapeConeControl";
            this.Size = new System.Drawing.Size(300, 75);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Assets.Components.TrackControl topRangeControl;
        private Assets.Components.TrackControl bottomRangeControl;
        private Assets.Components.TrackControl heightControl;
    }
}
