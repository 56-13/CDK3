namespace CDK.Assets.Attributes
{
    partial class AttributeAssetControl
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
            this.attributeControl = new CDK.Assets.Attributes.AttributeControl();
            this.buildCheckBox = new System.Windows.Forms.CheckBox();
            this.importbutton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // attributeControl
            // 
            this.attributeControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.attributeControl.BackColor = System.Drawing.Color.Transparent;
            this.attributeControl.Location = new System.Drawing.Point(0, 30);
            this.attributeControl.Name = "attributeControl";
            this.attributeControl.Size = new System.Drawing.Size(820, 420);
            this.attributeControl.TabIndex = 0;
            // 
            // buildCheckBox
            // 
            this.buildCheckBox.AutoSize = true;
            this.buildCheckBox.Location = new System.Drawing.Point(172, 5);
            this.buildCheckBox.Name = "buildCheckBox";
            this.buildCheckBox.Size = new System.Drawing.Size(52, 16);
            this.buildCheckBox.TabIndex = 7;
            this.buildCheckBox.Text = "Build";
            this.buildCheckBox.UseVisualStyleBackColor = true;
            // 
            // importbutton
            // 
            this.importbutton.Location = new System.Drawing.Point(86, 0);
            this.importbutton.Name = "importbutton";
            this.importbutton.Size = new System.Drawing.Size(80, 24);
            this.importbutton.TabIndex = 6;
            this.importbutton.Text = "Import";
            this.importbutton.UseVisualStyleBackColor = true;
            this.importbutton.Click += new System.EventHandler(this.Importbutton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(0, 0);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(80, 24);
            this.exportButton.TabIndex = 5;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.RestoreDirectory = true;
            // 
            // AttributeAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buildCheckBox);
            this.Controls.Add(this.importbutton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.attributeControl);
            this.Name = "AttributeAssetControl";
            this.Size = new System.Drawing.Size(820, 450);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AttributeControl attributeControl;
        private System.Windows.Forms.CheckBox buildCheckBox;
        private System.Windows.Forms.Button importbutton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private System.Windows.Forms.OpenFileDialog importFileDialog;
    }
}
