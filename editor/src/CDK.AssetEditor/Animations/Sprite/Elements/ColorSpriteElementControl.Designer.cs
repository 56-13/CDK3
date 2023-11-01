namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class ColorSpriteElementControl
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
            this.colorControl = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.SuspendLayout();
            // 
            // colorControl
            // 
            this.colorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorControl.Location = new System.Drawing.Point(0, 0);
            this.colorControl.Name = "colorControl";
            this.colorControl.Size = new System.Drawing.Size(320, 20);
            this.colorControl.TabIndex = 0;
            this.colorControl.Title = "Color";
            this.colorControl.SizeChanged += new System.EventHandler(this.ColorControl_SizeChanged);
            // 
            // ColorSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.colorControl);
            this.Name = "ColorSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.AnimationColorControl colorControl;
    }
}