namespace CDK.Assets.Animations.Components
{
    partial class AnimationColorLinearControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.smoothCheckBox = new System.Windows.Forms.CheckBox();
            this.startColorControl = new CDK.Assets.Components.ColorControl();
            this.endColorControl = new CDK.Assets.Components.ColorControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "End Color";
            // 
            // smoothCheckBox
            // 
            this.smoothCheckBox.AutoSize = true;
            this.smoothCheckBox.Location = new System.Drawing.Point(2, 59);
            this.smoothCheckBox.Name = "smoothCheckBox";
            this.smoothCheckBox.Size = new System.Drawing.Size(67, 16);
            this.smoothCheckBox.TabIndex = 4;
            this.smoothCheckBox.Text = "Smooth";
            this.smoothCheckBox.UseVisualStyleBackColor = true;
            // 
            // startColorControl
            // 
            this.startColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startColorControl.BackColor = System.Drawing.Color.White;
            this.startColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.startColorControl.Location = new System.Drawing.Point(109, 0);
            this.startColorControl.Name = "startColorControl";
            this.startColorControl.Size = new System.Drawing.Size(171, 21);
            this.startColorControl.TabIndex = 5;
            // 
            // endColorControl
            // 
            this.endColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.endColorControl.BackColor = System.Drawing.Color.White;
            this.endColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.endColorControl.Location = new System.Drawing.Point(109, 27);
            this.endColorControl.Name = "endColorControl";
            this.endColorControl.Size = new System.Drawing.Size(171, 21);
            this.endColorControl.TabIndex = 6;
            // 
            // AnimationColorLinearControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.endColorControl);
            this.Controls.Add(this.startColorControl);
            this.Controls.Add(this.smoothCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AnimationColorLinearControl";
            this.Size = new System.Drawing.Size(280, 75);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox smoothCheckBox;
        private Assets.Components.ColorControl startColorControl;
        private Assets.Components.ColorControl endColorControl;
    }
}
