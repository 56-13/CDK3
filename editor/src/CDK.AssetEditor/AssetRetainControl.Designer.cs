namespace CDK.Assets
{
    partial class AssetRetainControl
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
            this.fromRadioButton = new System.Windows.Forms.RadioButton();
            this.toRadioButton = new System.Windows.Forms.RadioButton();
            this.refreshButton = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // fromRadioButton
            // 
            this.fromRadioButton.AutoSize = true;
            this.fromRadioButton.Checked = true;
            this.fromRadioButton.Location = new System.Drawing.Point(3, 3);
            this.fromRadioButton.Name = "fromRadioButton";
            this.fromRadioButton.Size = new System.Drawing.Size(52, 16);
            this.fromRadioButton.TabIndex = 0;
            this.fromRadioButton.Text = "From";
            this.fromRadioButton.UseVisualStyleBackColor = true;
            this.fromRadioButton.CheckedChanged += new System.EventHandler(this.FromRadioButton_CheckedChanged);
            // 
            // toRadioButton
            // 
            this.toRadioButton.AutoSize = true;
            this.toRadioButton.Location = new System.Drawing.Point(61, 3);
            this.toRadioButton.Name = "toRadioButton";
            this.toRadioButton.Size = new System.Drawing.Size(38, 16);
            this.toRadioButton.TabIndex = 1;
            this.toRadioButton.Text = "To";
            this.toRadioButton.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(105, 0);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.IntegralHeight = false;
            this.listBox.ItemHeight = 12;
            this.listBox.Location = new System.Drawing.Point(0, 29);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(300, 271);
            this.listBox.TabIndex = 3;
            this.listBox.DoubleClick += new System.EventHandler(this.ListBox_DoubleClick);
            // 
            // AssetRetainControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.toRadioButton);
            this.Controls.Add(this.fromRadioButton);
            this.Name = "AssetRetainControl";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton fromRadioButton;
        private System.Windows.Forms.RadioButton toRadioButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ListBox listBox;
    }
}
