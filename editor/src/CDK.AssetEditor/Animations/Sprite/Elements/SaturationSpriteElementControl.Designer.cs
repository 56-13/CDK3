namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class SaturationSpriteElementControl
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
            this.saturationControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.SuspendLayout();
            // 
            // saturationControl
            // 
            this.saturationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saturationControl.Location = new System.Drawing.Point(0, 0);
            this.saturationControl.Margin = new System.Windows.Forms.Padding(0);
            this.saturationControl.Name = "saturationControl";
            this.saturationControl.Size = new System.Drawing.Size(320, 20);
            this.saturationControl.TabIndex = 6;
            this.saturationControl.Title = "Value";
            this.saturationControl.SizeChanged += new System.EventHandler(this.SaturationControl_SizeChanged);
            // 
            // SaturationSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.saturationControl);
            this.Name = "SaturationSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.AnimationFloatControl saturationControl;
    }
}
