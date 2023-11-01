namespace CDK.Assets.Animations.Particle.Shapes
{
    partial class ParticleShapeHemisphereControl
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
            this.rangeControl = new CDK.Assets.Components.TrackControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Range";
            // 
            // rangeControl
            // 
            this.rangeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangeControl.DecimalPlaces = 2;
            this.rangeControl.Increment = 1F;
            this.rangeControl.Location = new System.Drawing.Point(90, 0);
            this.rangeControl.Maximum = 5000F;
            this.rangeControl.Minimum = 0F;
            this.rangeControl.Name = "rangeControl";
            this.rangeControl.Size = new System.Drawing.Size(210, 21);
            this.rangeControl.TabIndex = 1;
            this.rangeControl.Value = 0F;
            // 
            // ParticleShapeHemisphereControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rangeControl);
            this.Controls.Add(this.label1);
            this.Name = "ParticleShapeHemisphereControl";
            this.Size = new System.Drawing.Size(300, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Assets.Components.TrackControl rangeControl;
    }
}
