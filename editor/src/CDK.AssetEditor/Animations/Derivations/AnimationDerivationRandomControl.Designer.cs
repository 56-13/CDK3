namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationRandomControl
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
            this.loopCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // loopCheckBox
            // 
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Location = new System.Drawing.Point(3, 3);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(52, 16);
            this.loopCheckBox.TabIndex = 0;
            this.loopCheckBox.Text = "Loop";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            // 
            // AnimationDerivationRandomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loopCheckBox);
            this.Name = "AnimationDerivationRandomControl";
            this.Size = new System.Drawing.Size(58, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox loopCheckBox;
    }
}
