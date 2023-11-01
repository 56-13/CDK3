namespace CDK.Assets.Animations.Components
{
    partial class AnimationColorChannelControl
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
            this.redControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.greenControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.blueControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.alphaControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.fixedChannelCheckBox = new System.Windows.Forms.CheckBox();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.screenControl = new CDK.Assets.Animations.Components.AnimationColorChannelScreenControl();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // redControl
            // 
            this.redControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.redControl.Increment = 0.01F;
            this.redControl.Location = new System.Drawing.Point(3, 3);
            this.redControl.MaxColor = System.Drawing.Color.Red;
            this.redControl.Name = "redControl";
            this.redControl.Size = new System.Drawing.Size(314, 20);
            this.redControl.TabIndex = 120;
            this.redControl.Title = "Red";
            // 
            // greenControl
            // 
            this.greenControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.greenControl.Increment = 0.01F;
            this.greenControl.Location = new System.Drawing.Point(3, 29);
            this.greenControl.MaxColor = System.Drawing.Color.Lime;
            this.greenControl.Name = "greenControl";
            this.greenControl.Size = new System.Drawing.Size(314, 20);
            this.greenControl.TabIndex = 121;
            this.greenControl.Title = "Green";
            // 
            // blueControl
            // 
            this.blueControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blueControl.Increment = 0.01F;
            this.blueControl.Location = new System.Drawing.Point(3, 55);
            this.blueControl.MaxColor = System.Drawing.Color.Blue;
            this.blueControl.Name = "blueControl";
            this.blueControl.Size = new System.Drawing.Size(314, 20);
            this.blueControl.TabIndex = 122;
            this.blueControl.Title = "Blue";
            // 
            // alphaControl
            // 
            this.alphaControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaControl.Increment = 0.01F;
            this.alphaControl.Location = new System.Drawing.Point(3, 81);
            this.alphaControl.MinColor = System.Drawing.Color.LightGray;
            this.alphaControl.Name = "alphaControl";
            this.alphaControl.Size = new System.Drawing.Size(314, 20);
            this.alphaControl.TabIndex = 123;
            this.alphaControl.Title = "Alpha";
            // 
            // fixedChannelCheckBox
            // 
            this.fixedChannelCheckBox.AutoSize = true;
            this.fixedChannelCheckBox.Location = new System.Drawing.Point(6, 107);
            this.fixedChannelCheckBox.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.fixedChannelCheckBox.Name = "fixedChannelCheckBox";
            this.fixedChannelCheckBox.Size = new System.Drawing.Size(308, 16);
            this.fixedChannelCheckBox.TabIndex = 124;
            this.fixedChannelCheckBox.Text = "Fixed Channel";
            this.fixedChannelCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.redControl);
            this.panel.Controls.Add(this.greenControl);
            this.panel.Controls.Add(this.blueControl);
            this.panel.Controls.Add(this.alphaControl);
            this.panel.Controls.Add(this.fixedChannelCheckBox);
            this.panel.Controls.Add(this.screenControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 202);
            this.panel.TabIndex = 125;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // screenControl
            // 
            this.screenControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenControl.Location = new System.Drawing.Point(3, 129);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(314, 70);
            this.screenControl.TabIndex = 125;
            this.screenControl.Text = "screenControl";
            // 
            // AnimationColorChannelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "AnimationColorChannelControl";
            this.Size = new System.Drawing.Size(320, 202);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AnimationFloatControl redControl;
        private AnimationFloatControl greenControl;
        private AnimationFloatControl blueControl;
        private AnimationFloatControl alphaControl;
        private System.Windows.Forms.CheckBox fixedChannelCheckBox;
        private Assets.Components.StackPanel panel;
        private AnimationColorChannelScreenControl screenControl;
    }
}
