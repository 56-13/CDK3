namespace CDK.Assets.Animations.Components
{
    partial class AnimationLoopControl
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
            this.countUpDown = new CDK.Assets.Components.NumericUpDown();
            this.finishCheckBox = new System.Windows.Forms.CheckBox();
            this.roundTripCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.countUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // countUpDown
            // 
            this.countUpDown.Location = new System.Drawing.Point(0, 0);
            this.countUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.countUpDown.Name = "countUpDown";
            this.countUpDown.Size = new System.Drawing.Size(50, 21);
            this.countUpDown.TabIndex = 0;
            this.countUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // finishCheckBox
            // 
            this.finishCheckBox.AutoSize = true;
            this.finishCheckBox.Location = new System.Drawing.Point(150, 1);
            this.finishCheckBox.Name = "finishCheckBox";
            this.finishCheckBox.Size = new System.Drawing.Size(58, 16);
            this.finishCheckBox.TabIndex = 1;
            this.finishCheckBox.Text = "Finish";
            this.finishCheckBox.UseVisualStyleBackColor = true;
            this.finishCheckBox.Visible = false;
            this.finishCheckBox.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // roundTripCheckBox
            // 
            this.roundTripCheckBox.AutoSize = true;
            this.roundTripCheckBox.Location = new System.Drawing.Point(56, 1);
            this.roundTripCheckBox.Name = "roundTripCheckBox";
            this.roundTripCheckBox.Size = new System.Drawing.Size(88, 16);
            this.roundTripCheckBox.TabIndex = 2;
            this.roundTripCheckBox.Text = "Round-Trip";
            this.roundTripCheckBox.UseVisualStyleBackColor = true;
            this.roundTripCheckBox.CheckedChanged += new System.EventHandler(this.ValueChanged);
            // 
            // MovementLoopControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.roundTripCheckBox);
            this.Controls.Add(this.finishCheckBox);
            this.Controls.Add(this.countUpDown);
            this.Name = "MovementLoopControl";
            this.Size = new System.Drawing.Size(208, 21);
            ((System.ComponentModel.ISupportInitialize)(this.countUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.NumericUpDown countUpDown;
        private System.Windows.Forms.CheckBox finishCheckBox;
        private System.Windows.Forms.CheckBox roundTripCheckBox;
    }
}
