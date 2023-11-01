namespace CDK.Assets.Texturing
{
    partial class TextureDescriptionControl
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
            this.magFilterComboBox = new CDK.Assets.Components.ComboBox();
            this.minFilterComboBox = new CDK.Assets.Components.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.wrapTComboBox = new CDK.Assets.Components.ComboBox();
            this.wrapSComboBox = new CDK.Assets.Components.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.borderColorControl = new CDK.Assets.Components.ColorControl();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.wrapRComboBox = new CDK.Assets.Components.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.mipmapCountComboBox = new CDK.Assets.Components.ComboBox();
            this.SuspendLayout();
            // 
            // magFilterComboBox
            // 
            this.magFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.magFilterComboBox.FormattingEnabled = true;
            this.magFilterComboBox.Location = new System.Drawing.Point(90, 104);
            this.magFilterComboBox.Name = "magFilterComboBox";
            this.magFilterComboBox.Size = new System.Drawing.Size(210, 20);
            this.magFilterComboBox.TabIndex = 18;
            // 
            // minFilterComboBox
            // 
            this.minFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.minFilterComboBox.FormattingEnabled = true;
            this.minFilterComboBox.Location = new System.Drawing.Point(90, 78);
            this.minFilterComboBox.Name = "minFilterComboBox";
            this.minFilterComboBox.Size = new System.Drawing.Size(210, 20);
            this.minFilterComboBox.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "Min Filter";
            // 
            // wrapTComboBox
            // 
            this.wrapTComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wrapTComboBox.FormattingEnabled = true;
            this.wrapTComboBox.Location = new System.Drawing.Point(90, 26);
            this.wrapTComboBox.Name = "wrapTComboBox";
            this.wrapTComboBox.Size = new System.Drawing.Size(210, 20);
            this.wrapTComboBox.TabIndex = 15;
            // 
            // wrapSComboBox
            // 
            this.wrapSComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wrapSComboBox.FormattingEnabled = true;
            this.wrapSComboBox.Location = new System.Drawing.Point(90, 0);
            this.wrapSComboBox.Name = "wrapSComboBox";
            this.wrapSComboBox.Size = new System.Drawing.Size(210, 20);
            this.wrapSComboBox.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "WrapS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 133);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 12);
            this.label3.TabIndex = 27;
            this.label3.Text = "Border Color";
            // 
            // borderColorControl
            // 
            this.borderColorControl.AlphaChannel = true;
            this.borderColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.borderColorControl.BackColor = System.Drawing.Color.White;
            this.borderColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.borderColorControl.Location = new System.Drawing.Point(90, 130);
            this.borderColorControl.Name = "borderColorControl";
            this.borderColorControl.Size = new System.Drawing.Size(97, 21);
            this.borderColorControl.TabIndex = 29;
            this.borderColorControl.ValueGDI = System.Drawing.Color.Empty;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 12);
            this.label4.TabIndex = 30;
            this.label4.Text = "Mag Filter";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 31;
            this.label5.Text = "WrapT";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 32;
            this.label6.Text = "WrapR";
            // 
            // wrapRComboBox
            // 
            this.wrapRComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wrapRComboBox.FormattingEnabled = true;
            this.wrapRComboBox.Location = new System.Drawing.Point(90, 52);
            this.wrapRComboBox.Name = "wrapRComboBox";
            this.wrapRComboBox.Size = new System.Drawing.Size(210, 20);
            this.wrapRComboBox.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(193, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 12);
            this.label7.TabIndex = 34;
            this.label7.Text = "Mipmap";
            // 
            // mipmapCountComboBox
            // 
            this.mipmapCountComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mipmapCountComboBox.FormattingEnabled = true;
            this.mipmapCountComboBox.Location = new System.Drawing.Point(250, 130);
            this.mipmapCountComboBox.Name = "mipmapCountComboBox";
            this.mipmapCountComboBox.Size = new System.Drawing.Size(50, 20);
            this.mipmapCountComboBox.TabIndex = 35;
            this.mipmapCountComboBox.SelectedIndexChanged += new System.EventHandler(this.MipmapCountComboBox_SelectedIndexChanged);
            // 
            // TextureDescriptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mipmapCountComboBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.wrapRComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.borderColorControl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.magFilterComboBox);
            this.Controls.Add(this.minFilterComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wrapTComboBox);
            this.Controls.Add(this.wrapSComboBox);
            this.Controls.Add(this.label1);
            this.Name = "TextureDescriptionControl";
            this.Size = new System.Drawing.Size(300, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.ComboBox magFilterComboBox;
        private Components.ComboBox minFilterComboBox;
        private System.Windows.Forms.Label label2;
        private Components.ComboBox wrapTComboBox;
        private Components.ComboBox wrapSComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private Components.ColorControl borderColorControl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Components.ComboBox wrapRComboBox;
        private System.Windows.Forms.Label label7;
        private Components.ComboBox mipmapCountComboBox;
    }
}
