namespace CDK.Assets.Animations.Components
{
    partial class AnimationFloatConstantControl
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
            this.vUpDown = new CDK.Assets.Components.NumericUpDown();
            this.vvUpDown = new CDK.Assets.Components.NumericUpDown();
            this.trackBar = new CDK.Assets.Components.RangeTrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.vUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vvUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // vUpDown
            // 
            this.vUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vUpDown.DecimalPlaces = 2;
            this.vUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.vUpDown.Location = new System.Drawing.Point(74, 0);
            this.vUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.vUpDown.Name = "vUpDown";
            this.vUpDown.Size = new System.Drawing.Size(60, 21);
            this.vUpDown.TabIndex = 0;
            // 
            // vvUpDown
            // 
            this.vvUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vvUpDown.DecimalPlaces = 2;
            this.vvUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.vvUpDown.Location = new System.Drawing.Point(140, 0);
            this.vvUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vvUpDown.Name = "vvUpDown";
            this.vvUpDown.Size = new System.Drawing.Size(60, 21);
            this.vvUpDown.TabIndex = 1;
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.BackColor = System.Drawing.Color.White;
            this.trackBar.ForeColor = System.Drawing.Color.Black;
            this.trackBar.Increment = 1F;
            this.trackBar.Location = new System.Drawing.Point(0, 27);
            this.trackBar.Maximum = 1F;
            this.trackBar.Minimum = -1F;
            this.trackBar.Name = "trackBar";
            this.trackBar.Range = 0F;
            this.trackBar.Size = new System.Drawing.Size(200, 12);
            this.trackBar.TabIndex = 2;
            this.trackBar.Text = "rangeTrackBar1";
            this.trackBar.Value = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 3;
            // 
            // ConstantControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.vvUpDown);
            this.Controls.Add(this.vUpDown);
            this.Name = "ConstantControl";
            this.Size = new System.Drawing.Size(200, 39);
            ((System.ComponentModel.ISupportInitialize)(this.vUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vvUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.NumericUpDown vUpDown;
        private Assets.Components.NumericUpDown vvUpDown;
        private Assets.Components.RangeTrackBar trackBar;
        private System.Windows.Forms.Label label1;
    }
}
