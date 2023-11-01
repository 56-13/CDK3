namespace CDK.Assets.Meshing
{
    partial class MaterialConfigControl
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.assetControl = new CDK.Assets.AssetSelectControl();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(0, 4);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(50, 12);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Material";
            // 
            // assetControl
            // 
            this.assetControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetControl.Location = new System.Drawing.Point(90, 0);
            this.assetControl.Name = "assetControl";
            this.assetControl.Size = new System.Drawing.Size(230, 22);
            this.assetControl.TabIndex = 1;
            this.assetControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material};
            // 
            // MaterialConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.assetControl);
            this.Controls.Add(this.nameLabel);
            this.Name = "MaterialConfigControl";
            this.Size = new System.Drawing.Size(320, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private AssetSelectControl assetControl;
    }
}
