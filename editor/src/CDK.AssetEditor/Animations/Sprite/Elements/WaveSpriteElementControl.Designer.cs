namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class WaveSpriteElementControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.positionControl = new CDK.Assets.Components.Vector3Control();
            this.label1 = new System.Windows.Forms.Label();
            this.xControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.yControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.zControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.radiusControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.thicknessControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.xControl);
            this.panel.Controls.Add(this.yControl);
            this.panel.Controls.Add(this.zControl);
            this.panel.Controls.Add(this.radiusControl);
            this.panel.Controls.Add(this.thicknessControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 154);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.positionControl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 21);
            this.panel1.TabIndex = 8;
            // 
            // positionControl
            // 
            this.positionControl.DecimalPlaces = 2;
            this.positionControl.Increment = 1F;
            this.positionControl.Location = new System.Drawing.Point(90, 0);
            this.positionControl.Maximum = 10000F;
            this.positionControl.Minimum = -10000F;
            this.positionControl.Name = "positionControl";
            this.positionControl.Size = new System.Drawing.Size(192, 21);
            this.positionControl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Position";
            // 
            // xControl
            // 
            this.xControl.Location = new System.Drawing.Point(0, 27);
            this.xControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.xControl.Size = new System.Drawing.Size(320, 20);
            this.xControl.TabIndex = 5;
            this.xControl.Title = "X";
            // 
            // yControl
            // 
            this.yControl.Location = new System.Drawing.Point(0, 53);
            this.yControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.yControl.Size = new System.Drawing.Size(320, 20);
            this.yControl.TabIndex = 7;
            this.yControl.Title = "Y";
            // 
            // zControl
            // 
            this.zControl.Location = new System.Drawing.Point(0, 79);
            this.zControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.zControl.Name = "zControl";
            this.zControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.zControl.Size = new System.Drawing.Size(320, 20);
            this.zControl.TabIndex = 6;
            this.zControl.Title = "Z";
            // 
            // radiusControl
            // 
            this.radiusControl.Location = new System.Drawing.Point(0, 105);
            this.radiusControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.radiusControl.Name = "radiusControl";
            this.radiusControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radiusControl.Size = new System.Drawing.Size(320, 20);
            this.radiusControl.TabIndex = 9;
            this.radiusControl.Title = "Radius";
            // 
            // thicknessControl
            // 
            this.thicknessControl.Location = new System.Drawing.Point(0, 131);
            this.thicknessControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.thicknessControl.Name = "thicknessControl";
            this.thicknessControl.Size = new System.Drawing.Size(320, 20);
            this.thicknessControl.TabIndex = 10;
            this.thicknessControl.Title = "Thickness";
            // 
            // WaveSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "WaveSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 151);
            this.panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Components.StackPanel panel;
        private Components.AnimationFloatControl xControl;
        private Components.AnimationFloatControl yControl;
        private Components.AnimationFloatControl zControl;
        private System.Windows.Forms.Panel panel1;
        private Assets.Components.Vector3Control positionControl;
        private System.Windows.Forms.Label label1;
        private Components.AnimationFloatControl radiusControl;
        private Components.AnimationFloatControl thicknessControl;
    }
}
