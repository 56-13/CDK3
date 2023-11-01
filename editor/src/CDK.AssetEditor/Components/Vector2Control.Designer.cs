namespace CDK.Assets.Components
{
    partial class Vector2Control
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
            this.xUpDown = new CDK.Assets.Components.NumericUpDown();
            this.yUpDown = new CDK.Assets.Components.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // xUpDown
            // 
            this.xUpDown.Location = new System.Drawing.Point(0, 0);
            this.xUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.xUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.xUpDown.Name = "xUpDown";
            this.xUpDown.Size = new System.Drawing.Size(60, 21);
            this.xUpDown.TabIndex = 21;
            this.xUpDown.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // yUpDown
            // 
            this.yUpDown.Location = new System.Drawing.Point(66, 0);
            this.yUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.yUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.yUpDown.Name = "yUpDown";
            this.yUpDown.Size = new System.Drawing.Size(60, 21);
            this.yUpDown.TabIndex = 22;
            this.yUpDown.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // Vector2Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xUpDown);
            this.Controls.Add(this.yUpDown);
            this.Name = "Vector2Control";
            this.Size = new System.Drawing.Size(126, 21);
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Components.NumericUpDown xUpDown;
        private Components.NumericUpDown yUpDown;
    }
}
