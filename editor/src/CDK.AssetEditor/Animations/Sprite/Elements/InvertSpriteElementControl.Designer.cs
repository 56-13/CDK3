namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class InvertSpriteElementControl
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.xCheckBox = new System.Windows.Forms.CheckBox();
            this.yCheckBox = new System.Windows.Forms.CheckBox();
            this.zCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // xCheckBox
            // 
            this.xCheckBox.AutoSize = true;
            this.xCheckBox.Location = new System.Drawing.Point(0, 0);
            this.xCheckBox.Name = "xCheckBox";
            this.xCheckBox.Size = new System.Drawing.Size(32, 16);
            this.xCheckBox.TabIndex = 0;
            this.xCheckBox.Text = "X";
            this.xCheckBox.UseVisualStyleBackColor = true;
            // 
            // yCheckBox
            // 
            this.yCheckBox.AutoSize = true;
            this.yCheckBox.Location = new System.Drawing.Point(38, 0);
            this.yCheckBox.Name = "yCheckBox";
            this.yCheckBox.Size = new System.Drawing.Size(32, 16);
            this.yCheckBox.TabIndex = 1;
            this.yCheckBox.Text = "Y";
            this.yCheckBox.UseVisualStyleBackColor = true;
            // 
            // zCheckBox
            // 
            this.zCheckBox.AutoSize = true;
            this.zCheckBox.Location = new System.Drawing.Point(76, 0);
            this.zCheckBox.Name = "zCheckBox";
            this.zCheckBox.Size = new System.Drawing.Size(32, 16);
            this.zCheckBox.TabIndex = 2;
            this.zCheckBox.Text = "Z";
            this.zCheckBox.UseVisualStyleBackColor = true;
            // 
            // InvertSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.zCheckBox);
            this.Controls.Add(this.yCheckBox);
            this.Controls.Add(this.xCheckBox);
            this.Name = "InvertSpriteElementControl";
            this.Size = new System.Drawing.Size(108, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox xCheckBox;
        private System.Windows.Forms.CheckBox yCheckBox;
        private System.Windows.Forms.CheckBox zCheckBox;
    }
}
