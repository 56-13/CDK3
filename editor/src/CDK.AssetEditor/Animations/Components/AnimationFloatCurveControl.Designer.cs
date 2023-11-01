namespace CDK.Assets.Animations.Components
{
    partial class AnimationFloatCurveControl
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
            this.tUpDown = new CDK.Assets.Components.NumericUpDown();
            this.tLabel = new System.Windows.Forms.Label();
            this.vvLabel = new System.Windows.Forms.Label();
            this.vvUpDown = new CDK.Assets.Components.NumericUpDown();
            this.laLabel = new System.Windows.Forms.Label();
            this.laUpDown = new CDK.Assets.Components.NumericUpDown();
            this.vLabel = new System.Windows.Forms.Label();
            this.vUpDown = new CDK.Assets.Components.NumericUpDown();
            this.screenPanel = new System.Windows.Forms.Panel();
            this.screenControl = new CDK.Assets.Animations.Components.AnimationFloatCurveScreenControl();
            this.raUpDown = new CDK.Assets.Components.NumericUpDown();
            this.raLabel = new System.Windows.Forms.Label();
            this.windowButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vvUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.laUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vUpDown)).BeginInit();
            this.screenPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.raUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tUpDown
            // 
            this.tUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tUpDown.DecimalPlaces = 2;
            this.tUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.tUpDown.Location = new System.Drawing.Point(276, 0);
            this.tUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tUpDown.Name = "tUpDown";
            this.tUpDown.Size = new System.Drawing.Size(60, 21);
            this.tUpDown.TabIndex = 1;
            this.tUpDown.Visible = false;
            // 
            // tLabel
            // 
            this.tLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tLabel.AutoSize = true;
            this.tLabel.Location = new System.Drawing.Point(197, 2);
            this.tLabel.Name = "tLabel";
            this.tLabel.Size = new System.Drawing.Size(34, 12);
            this.tLabel.TabIndex = 2;
            this.tLabel.Text = "Time";
            this.tLabel.Visible = false;
            // 
            // vvLabel
            // 
            this.vvLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vvLabel.AutoSize = true;
            this.vvLabel.Location = new System.Drawing.Point(197, 110);
            this.vvLabel.Name = "vvLabel";
            this.vvLabel.Size = new System.Drawing.Size(52, 12);
            this.vvLabel.TabIndex = 8;
            this.vvLabel.Text = "Random";
            this.vvLabel.Visible = false;
            // 
            // vvUpDown
            // 
            this.vvUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vvUpDown.DecimalPlaces = 2;
            this.vvUpDown.Location = new System.Drawing.Point(276, 108);
            this.vvUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vvUpDown.Name = "vvUpDown";
            this.vvUpDown.Size = new System.Drawing.Size(60, 21);
            this.vvUpDown.TabIndex = 7;
            this.vvUpDown.Visible = false;
            // 
            // laLabel
            // 
            this.laLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.laLabel.AutoSize = true;
            this.laLabel.Location = new System.Drawing.Point(197, 56);
            this.laLabel.Name = "laLabel";
            this.laLabel.Size = new System.Drawing.Size(61, 12);
            this.laLabel.TabIndex = 6;
            this.laLabel.Text = "Left Angle";
            this.laLabel.Visible = false;
            // 
            // laUpDown
            // 
            this.laUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.laUpDown.DecimalPlaces = 2;
            this.laUpDown.Location = new System.Drawing.Point(276, 54);
            this.laUpDown.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.laUpDown.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.laUpDown.Name = "laUpDown";
            this.laUpDown.Size = new System.Drawing.Size(60, 21);
            this.laUpDown.TabIndex = 5;
            this.laUpDown.Visible = false;
            // 
            // vLabel
            // 
            this.vLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vLabel.AutoSize = true;
            this.vLabel.Location = new System.Drawing.Point(197, 29);
            this.vLabel.Name = "vLabel";
            this.vLabel.Size = new System.Drawing.Size(37, 12);
            this.vLabel.TabIndex = 4;
            this.vLabel.Text = "Value";
            this.vLabel.Visible = false;
            // 
            // vUpDown
            // 
            this.vUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vUpDown.DecimalPlaces = 2;
            this.vUpDown.Location = new System.Drawing.Point(276, 27);
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
            this.vUpDown.TabIndex = 3;
            this.vUpDown.Visible = false;
            // 
            // screenPanel
            // 
            this.screenPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.screenPanel.Controls.Add(this.screenControl);
            this.screenPanel.Location = new System.Drawing.Point(0, 0);
            this.screenPanel.Name = "screenPanel";
            this.screenPanel.Size = new System.Drawing.Size(192, 158);
            this.screenPanel.TabIndex = 4;
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenControl.ForeColor = System.Drawing.Color.Black;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(190, 156);
            this.screenControl.TabIndex = 1;
            this.screenControl.Text = "belzierScreenControl1";
            // 
            // raUpDown
            // 
            this.raUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.raUpDown.DecimalPlaces = 2;
            this.raUpDown.Location = new System.Drawing.Point(276, 81);
            this.raUpDown.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.raUpDown.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.raUpDown.Name = "raUpDown";
            this.raUpDown.Size = new System.Drawing.Size(60, 21);
            this.raUpDown.TabIndex = 9;
            this.raUpDown.Visible = false;
            // 
            // raLabel
            // 
            this.raLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.raLabel.AutoSize = true;
            this.raLabel.Location = new System.Drawing.Point(198, 83);
            this.raLabel.Name = "raLabel";
            this.raLabel.Size = new System.Drawing.Size(69, 12);
            this.raLabel.TabIndex = 10;
            this.raLabel.Text = "Right Angle";
            this.raLabel.Visible = false;
            // 
            // windowButton
            // 
            this.windowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowButton.Location = new System.Drawing.Point(197, 135);
            this.windowButton.Name = "windowButton";
            this.windowButton.Size = new System.Drawing.Size(139, 23);
            this.windowButton.TabIndex = 11;
            this.windowButton.Text = "Window";
            this.windowButton.UseVisualStyleBackColor = true;
            this.windowButton.Visible = false;
            this.windowButton.Click += new System.EventHandler(this.WindowButton_Click);
            // 
            // CurveControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.windowButton);
            this.Controls.Add(this.raLabel);
            this.Controls.Add(this.raUpDown);
            this.Controls.Add(this.vvLabel);
            this.Controls.Add(this.screenPanel);
            this.Controls.Add(this.laUpDown);
            this.Controls.Add(this.tLabel);
            this.Controls.Add(this.vvUpDown);
            this.Controls.Add(this.vLabel);
            this.Controls.Add(this.tUpDown);
            this.Controls.Add(this.vUpDown);
            this.Controls.Add(this.laLabel);
            this.Name = "CurveControl";
            this.Size = new System.Drawing.Size(336, 158);
            ((System.ComponentModel.ISupportInitialize)(this.tUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vvUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.laUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vUpDown)).EndInit();
            this.screenPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.raUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.NumericUpDown tUpDown;
        private System.Windows.Forms.Label tLabel;
        private System.Windows.Forms.Label vvLabel;
        private Assets.Components.NumericUpDown vvUpDown;
        private System.Windows.Forms.Label laLabel;
        private Assets.Components.NumericUpDown laUpDown;
        private System.Windows.Forms.Label vLabel;
        private Assets.Components.NumericUpDown vUpDown;
        private System.Windows.Forms.Panel screenPanel;
        private AnimationFloatCurveScreenControl screenControl;
        private Assets.Components.NumericUpDown raUpDown;
        private System.Windows.Forms.Label raLabel;
        private System.Windows.Forms.Button windowButton;
    }
}
