namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationLinkedControl
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
            this.loopCountUpDown = new CDK.Assets.Components.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.loopCountUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Loop Count";
            // 
            // loopCountUpDown
            // 
            this.loopCountUpDown.Location = new System.Drawing.Point(80, 0);
            this.loopCountUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.loopCountUpDown.Name = "loopCountUpDown";
            this.loopCountUpDown.Size = new System.Drawing.Size(60, 21);
            this.loopCountUpDown.TabIndex = 1;
            // 
            // AnimationDerivationLinkedControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.loopCountUpDown);
            this.Controls.Add(this.label1);
            this.Name = "AnimationDerivationLinkedControl";
            this.Size = new System.Drawing.Size(140, 21);
            ((System.ComponentModel.ISupportInitialize)(this.loopCountUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Assets.Components.NumericUpDown loopCountUpDown;

    }
}