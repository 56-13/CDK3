namespace CDK.Assets.Components
{
    partial class ColorFControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.amplificationControl = new CDK.Assets.Components.TrackControl();
            this.colorControl = new CDK.Assets.Components.ColorControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Amplification";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 26;
            this.label2.Text = "Color";
            // 
            // amplificationControl
            // 
            this.amplificationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.amplificationControl.DecimalPlaces = 2;
            this.amplificationControl.Increment = 0.01F;
            this.amplificationControl.Location = new System.Drawing.Point(90, 27);
            this.amplificationControl.Maximum = 100F;
            this.amplificationControl.Minimum = 1F;
            this.amplificationControl.Name = "amplificationControl";
            this.amplificationControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.amplificationControl.Size = new System.Drawing.Size(167, 21);
            this.amplificationControl.TabIndex = 27;
            this.amplificationControl.Value = 1F;
            this.amplificationControl.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // colorControl
            // 
            this.colorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorControl.BackColor = System.Drawing.Color.White;
            this.colorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorControl.Location = new System.Drawing.Point(90, 0);
            this.colorControl.Name = "colorControl";
            this.colorControl.Size = new System.Drawing.Size(167, 21);
            this.colorControl.TabIndex = 0;
            this.colorControl.ValueGDI = System.Drawing.Color.White;
            this.colorControl.Value4Changed += new System.EventHandler(this.Component_ValueChanged);
            // 
            // ColorFControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.amplificationControl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.colorControl);
            this.Name = "ColorFControl";
            this.Size = new System.Drawing.Size(257, 48);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ColorControl colorControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private TrackControl amplificationControl;
    }
}
