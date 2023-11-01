namespace CDK.Assets.Animations
{
    partial class AnimationSubstanceViewControl
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
            this.prevButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.typeComboBox = new CDK.Assets.Components.ComboBox();
            this.panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(0, 0);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(22, 22);
            this.prevButton.TabIndex = 1;
            this.prevButton.Text = "←";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(298, 0);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(22, 22);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "*";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // typeComboBox
            // 
            this.typeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(28, 1);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(264, 20);
            this.typeComboBox.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Location = new System.Drawing.Point(0, 27);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 0);
            this.panel.TabIndex = 3;
            // 
            // AnimationSubstanceViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.typeComboBox);
            this.Name = "AnimationSubstanceViewControl";
            this.Size = new System.Drawing.Size(320, 27);
            this.ResumeLayout(false);

        }

        #endregion

        private Assets.Components.ComboBox typeComboBox;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Panel panel;
    }
}
