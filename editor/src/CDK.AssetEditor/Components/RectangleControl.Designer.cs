namespace CDK.Assets.Components
{
    partial class RectangleControl
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
            this.widthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.heightUpDown = new CDK.Assets.Components.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
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
            // widthUpDown
            // 
            this.widthUpDown.Location = new System.Drawing.Point(132, 0);
            this.widthUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.Size = new System.Drawing.Size(60, 21);
            this.widthUpDown.TabIndex = 24;
            this.widthUpDown.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // heightUpDown
            // 
            this.heightUpDown.Location = new System.Drawing.Point(198, 0);
            this.heightUpDown.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.Size = new System.Drawing.Size(60, 21);
            this.heightUpDown.TabIndex = 25;
            this.heightUpDown.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // RectangleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.widthUpDown);
            this.Controls.Add(this.heightUpDown);
            this.Controls.Add(this.xUpDown);
            this.Controls.Add(this.yUpDown);
            this.Name = "RectangleControl";
            this.Size = new System.Drawing.Size(258, 21);
            ((System.ComponentModel.ISupportInitialize)(this.xUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Components.NumericUpDown xUpDown;
        private Components.NumericUpDown yUpDown;
        private Components.NumericUpDown widthUpDown;
        private Components.NumericUpDown heightUpDown;
    }
}
