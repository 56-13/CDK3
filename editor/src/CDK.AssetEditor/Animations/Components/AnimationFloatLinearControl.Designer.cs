namespace CDK.Assets.Animations.Components
{
    partial class AnimationFloatLinearControl
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
            this.sTrackBar = new CDK.Assets.Components.RangeTrackBar();
            this.svvUpDown = new CDK.Assets.Components.NumericUpDown();
            this.svUpDown = new CDK.Assets.Components.NumericUpDown();
            this.eTrackBar = new CDK.Assets.Components.RangeTrackBar();
            this.evvUpDown = new CDK.Assets.Components.NumericUpDown();
            this.evUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.smoothCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.svvUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.evvUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.evUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // sTrackBar
            // 
            this.sTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sTrackBar.BackColor = System.Drawing.Color.White;
            this.sTrackBar.ForeColor = System.Drawing.Color.Black;
            this.sTrackBar.Increment = 1F;
            this.sTrackBar.Location = new System.Drawing.Point(0, 27);
            this.sTrackBar.Maximum = 1F;
            this.sTrackBar.Minimum = -1F;
            this.sTrackBar.Name = "sTrackBar";
            this.sTrackBar.Range = 0F;
            this.sTrackBar.Size = new System.Drawing.Size(200, 12);
            this.sTrackBar.TabIndex = 5;
            this.sTrackBar.Text = "rangeTrackBar1";
            this.sTrackBar.Value = 0F;
            // 
            // svvUpDown
            // 
            this.svvUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.svvUpDown.DecimalPlaces = 2;
            this.svvUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.svvUpDown.Location = new System.Drawing.Point(140, 0);
            this.svvUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.svvUpDown.Name = "svvUpDown";
            this.svvUpDown.Size = new System.Drawing.Size(60, 21);
            this.svvUpDown.TabIndex = 4;
            // 
            // svUpDown
            // 
            this.svUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.svUpDown.DecimalPlaces = 2;
            this.svUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.svUpDown.Location = new System.Drawing.Point(74, 0);
            this.svUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.svUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.svUpDown.Name = "svUpDown";
            this.svUpDown.Size = new System.Drawing.Size(60, 21);
            this.svUpDown.TabIndex = 3;
            // 
            // eTrackBar
            // 
            this.eTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eTrackBar.BackColor = System.Drawing.Color.White;
            this.eTrackBar.ForeColor = System.Drawing.Color.Black;
            this.eTrackBar.Increment = 1F;
            this.eTrackBar.Location = new System.Drawing.Point(0, 72);
            this.eTrackBar.Maximum = 1F;
            this.eTrackBar.Minimum = -1F;
            this.eTrackBar.Name = "eTrackBar";
            this.eTrackBar.Range = 0F;
            this.eTrackBar.Size = new System.Drawing.Size(200, 12);
            this.eTrackBar.TabIndex = 8;
            this.eTrackBar.Text = "rangeTrackBar1";
            this.eTrackBar.Value = 0F;
            // 
            // evvUpDown
            // 
            this.evvUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.evvUpDown.DecimalPlaces = 2;
            this.evvUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.evvUpDown.Location = new System.Drawing.Point(140, 45);
            this.evvUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.evvUpDown.Name = "evvUpDown";
            this.evvUpDown.Size = new System.Drawing.Size(60, 21);
            this.evvUpDown.TabIndex = 7;
            // 
            // evUpDown
            // 
            this.evUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.evUpDown.DecimalPlaces = 2;
            this.evUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.evUpDown.Location = new System.Drawing.Point(74, 45);
            this.evUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.evUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.evUpDown.Name = "evUpDown";
            this.evUpDown.Size = new System.Drawing.Size(60, 21);
            this.evUpDown.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "End";
            // 
            // smoothCheckBox
            // 
            this.smoothCheckBox.AutoSize = true;
            this.smoothCheckBox.Location = new System.Drawing.Point(0, 90);
            this.smoothCheckBox.Name = "smoothCheckBox";
            this.smoothCheckBox.Size = new System.Drawing.Size(67, 16);
            this.smoothCheckBox.TabIndex = 11;
            this.smoothCheckBox.Text = "Smooth";
            this.smoothCheckBox.UseVisualStyleBackColor = true;
            // 
            // LinearControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.smoothCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eTrackBar);
            this.Controls.Add(this.evvUpDown);
            this.Controls.Add(this.evUpDown);
            this.Controls.Add(this.sTrackBar);
            this.Controls.Add(this.svvUpDown);
            this.Controls.Add(this.svUpDown);
            this.Name = "LinearControl";
            this.Size = new System.Drawing.Size(200, 106);
            ((System.ComponentModel.ISupportInitialize)(this.svvUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.evvUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.evUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.RangeTrackBar sTrackBar;
        private Assets.Components.NumericUpDown svvUpDown;
        private Assets.Components.NumericUpDown svUpDown;
        private Assets.Components.RangeTrackBar eTrackBar;
        private Assets.Components.NumericUpDown evvUpDown;
        private Assets.Components.NumericUpDown evUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox smoothCheckBox;
    }
}
