namespace CDK.Assets.Animations.Derivations
{
    partial class AnimationDerivationEmissionControl
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
            this.emissionDelayUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.emissionCountUpDown = new CDK.Assets.Components.NumericUpDown();
            this.prewarmCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.emissionDelayUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emissionCountUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // emissionDelayUpDown
            // 
            this.emissionDelayUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionDelayUpDown.DecimalPlaces = 2;
            this.emissionDelayUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.emissionDelayUpDown.Location = new System.Drawing.Point(108, 3);
            this.emissionDelayUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.emissionDelayUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.emissionDelayUpDown.Name = "emissionDelayUpDown";
            this.emissionDelayUpDown.Size = new System.Drawing.Size(65, 21);
            this.emissionDelayUpDown.TabIndex = 25;
            this.emissionDelayUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(0, 5);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(94, 12);
            this.label18.TabIndex = 24;
            this.label18.Text = "Emission Delay";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "Emission Count";
            // 
            // emissionCountUpDown
            // 
            this.emissionCountUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionCountUpDown.Location = new System.Drawing.Point(108, 30);
            this.emissionCountUpDown.Name = "emissionCountUpDown";
            this.emissionCountUpDown.Size = new System.Drawing.Size(65, 21);
            this.emissionCountUpDown.TabIndex = 27;
            // 
            // prewarmCheckBox
            // 
            this.prewarmCheckBox.AutoSize = true;
            this.prewarmCheckBox.Location = new System.Drawing.Point(2, 57);
            this.prewarmCheckBox.Name = "prewarmCheckBox";
            this.prewarmCheckBox.Size = new System.Drawing.Size(75, 16);
            this.prewarmCheckBox.TabIndex = 28;
            this.prewarmCheckBox.Text = "Prewarm";
            this.prewarmCheckBox.UseVisualStyleBackColor = true;
            // 
            // AnimationDerivationEmissionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.prewarmCheckBox);
            this.Controls.Add(this.emissionCountUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.emissionDelayUpDown);
            this.Controls.Add(this.label18);
            this.Name = "AnimationDerivationEmissionControl";
            this.Size = new System.Drawing.Size(173, 76);
            ((System.ComponentModel.ISupportInitialize)(this.emissionDelayUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emissionCountUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.NumericUpDown emissionDelayUpDown;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label1;
        private Assets.Components.NumericUpDown emissionCountUpDown;
        private System.Windows.Forms.CheckBox prewarmCheckBox;
    }
}
