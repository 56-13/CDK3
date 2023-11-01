namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class ScaleSpriteElementControl
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
            this.panel = new CDK.Assets.Components.StackPanel();
            this.eachCheckBox = new System.Windows.Forms.CheckBox();
            this.xControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.yControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.zControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.eachCheckBox);
            this.panel.Controls.Add(this.xControl);
            this.panel.Controls.Add(this.yControl);
            this.panel.Controls.Add(this.zControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 94);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // eachCheckBox
            // 
            this.eachCheckBox.AutoSize = true;
            this.eachCheckBox.Location = new System.Drawing.Point(0, 0);
            this.eachCheckBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.eachCheckBox.Name = "eachCheckBox";
            this.eachCheckBox.Size = new System.Drawing.Size(320, 16);
            this.eachCheckBox.TabIndex = 8;
            this.eachCheckBox.Text = "Each";
            this.eachCheckBox.UseVisualStyleBackColor = true;
            // 
            // xControl
            // 
            this.xControl.Location = new System.Drawing.Point(0, 22);
            this.xControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.xControl.Size = new System.Drawing.Size(320, 20);
            this.xControl.TabIndex = 5;
            this.xControl.Title = "X";
            // 
            // yControl
            // 
            this.yControl.Location = new System.Drawing.Point(0, 48);
            this.yControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.yControl.Size = new System.Drawing.Size(320, 20);
            this.yControl.TabIndex = 7;
            this.yControl.Title = "Y";
            // 
            // zControl
            // 
            this.zControl.Location = new System.Drawing.Point(0, 74);
            this.zControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.zControl.Name = "zControl";
            this.zControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.zControl.Size = new System.Drawing.Size(320, 20);
            this.zControl.TabIndex = 6;
            this.zControl.Title = "Z";
            // 
            // ScaleSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "ScaleSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 94);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.StackPanel panel;
        private Components.AnimationFloatControl xControl;
        private Components.AnimationFloatControl yControl;
        private Components.AnimationFloatControl zControl;
        private System.Windows.Forms.CheckBox eachCheckBox;
    }
}
