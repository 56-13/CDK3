namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class GradientLineSpriteElementControl
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
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.positionPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.position1Control = new CDK.Assets.Components.Vector3Control();
            this.label3 = new System.Windows.Forms.Label();
            this.x1Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.y1Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.z1Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.position2Control = new CDK.Assets.Components.Vector3Control();
            this.label1 = new System.Windows.Forms.Label();
            this.x2Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.y2Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.z2Control = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.gradientPanel = new CDK.Assets.Components.StackPanel();
            this.color1Control = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.color2Control = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            this.mainPanel.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gradientPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.positionPanel);
            this.mainPanel.Controls.Add(this.gradientPanel);
            this.mainPanel.Controls.Add(this.materialControl);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 1086);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // positionPanel
            // 
            this.positionPanel.AutoSize = true;
            this.positionPanel.Collapsible = true;
            this.positionPanel.Controls.Add(this.panel2);
            this.positionPanel.Controls.Add(this.x1Control);
            this.positionPanel.Controls.Add(this.y1Control);
            this.positionPanel.Controls.Add(this.z1Control);
            this.positionPanel.Controls.Add(this.panel1);
            this.positionPanel.Controls.Add(this.x2Control);
            this.positionPanel.Controls.Add(this.y2Control);
            this.positionPanel.Controls.Add(this.z2Control);
            this.positionPanel.Location = new System.Drawing.Point(0, 0);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(320, 231);
            this.positionPanel.TabIndex = 15;
            this.positionPanel.Title = "Position";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.position1Control);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 21);
            this.panel2.TabIndex = 14;
            // 
            // position1Control
            // 
            this.position1Control.DecimalPlaces = 2;
            this.position1Control.Increment = 1F;
            this.position1Control.Location = new System.Drawing.Point(90, 0);
            this.position1Control.Maximum = 10000F;
            this.position1Control.Minimum = -10000F;
            this.position1Control.Name = "position1Control";
            this.position1Control.Size = new System.Drawing.Size(192, 21);
            this.position1Control.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Position1";
            // 
            // x1Control
            // 
            this.x1Control.Location = new System.Drawing.Point(3, 51);
            this.x1Control.Name = "x1Control";
            this.x1Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.x1Control.Size = new System.Drawing.Size(314, 20);
            this.x1Control.TabIndex = 5;
            this.x1Control.Title = "X1";
            // 
            // y1Control
            // 
            this.y1Control.Location = new System.Drawing.Point(3, 77);
            this.y1Control.Name = "y1Control";
            this.y1Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.y1Control.Size = new System.Drawing.Size(314, 20);
            this.y1Control.TabIndex = 7;
            this.y1Control.Title = "Y1";
            // 
            // z1Control
            // 
            this.z1Control.Location = new System.Drawing.Point(3, 103);
            this.z1Control.Name = "z1Control";
            this.z1Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.z1Control.Size = new System.Drawing.Size(314, 20);
            this.z1Control.TabIndex = 6;
            this.z1Control.Title = "Z1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.position2Control);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 129);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 21);
            this.panel1.TabIndex = 15;
            // 
            // position2Control
            // 
            this.position2Control.DecimalPlaces = 2;
            this.position2Control.Increment = 1F;
            this.position2Control.Location = new System.Drawing.Point(90, 0);
            this.position2Control.Maximum = 10000F;
            this.position2Control.Minimum = -10000F;
            this.position2Control.Name = "position2Control";
            this.position2Control.Size = new System.Drawing.Size(192, 21);
            this.position2Control.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Position2";
            // 
            // x2Control
            // 
            this.x2Control.Location = new System.Drawing.Point(3, 156);
            this.x2Control.Name = "x2Control";
            this.x2Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.x2Control.Size = new System.Drawing.Size(314, 20);
            this.x2Control.TabIndex = 9;
            this.x2Control.Title = "X2";
            // 
            // y2Control
            // 
            this.y2Control.Location = new System.Drawing.Point(3, 182);
            this.y2Control.Name = "y2Control";
            this.y2Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.y2Control.Size = new System.Drawing.Size(314, 20);
            this.y2Control.TabIndex = 10;
            this.y2Control.Title = "Y2";
            // 
            // z2Control
            // 
            this.z2Control.Location = new System.Drawing.Point(3, 208);
            this.z2Control.Name = "z2Control";
            this.z2Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.z2Control.Size = new System.Drawing.Size(314, 20);
            this.z2Control.TabIndex = 8;
            this.z2Control.Title = "Z2";
            // 
            // gradientPanel
            // 
            this.gradientPanel.AutoSize = true;
            this.gradientPanel.Collapsible = true;
            this.gradientPanel.Controls.Add(this.color1Control);
            this.gradientPanel.Controls.Add(this.color2Control);
            this.gradientPanel.Location = new System.Drawing.Point(0, 231);
            this.gradientPanel.Margin = new System.Windows.Forms.Padding(0);
            this.gradientPanel.Name = "gradientPanel";
            this.gradientPanel.Size = new System.Drawing.Size(320, 73);
            this.gradientPanel.TabIndex = 19;
            this.gradientPanel.Title = "Gradient";
            // 
            // color1Control
            // 
            this.color1Control.Location = new System.Drawing.Point(3, 24);
            this.color1Control.Name = "color1Control";
            this.color1Control.Size = new System.Drawing.Size(314, 20);
            this.color1Control.TabIndex = 0;
            this.color1Control.Title = "Color1";
            // 
            // color2Control
            // 
            this.color2Control.Location = new System.Drawing.Point(3, 50);
            this.color2Control.Name = "color2Control";
            this.color2Control.Size = new System.Drawing.Size(314, 20);
            this.color2Control.TabIndex = 1;
            this.color2Control.Title = "Color2";
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 304);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mainPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.subPanel);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(320, 1086);
            this.splitContainer.SplitterDistance = 158;
            this.splitContainer.TabIndex = 1;
            // 
            // subPanel
            // 
            this.subPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subPanel.AutoSize = true;
            this.subPanel.Location = new System.Drawing.Point(0, 0);
            this.subPanel.Name = "subPanel";
            this.subPanel.Size = new System.Drawing.Size(158, 0);
            this.subPanel.TabIndex = 0;
            this.subPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // GradientLineSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "GradientLineSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 1086);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.positionPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gradientPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Assets.Components.StackPanel mainPanel;
        private Components.AnimationFloatControl x1Control;
        private Components.AnimationFloatControl y1Control;
        private Components.AnimationFloatControl z1Control;
        private Components.AnimationFloatControl x2Control;
        private Components.AnimationFloatControl z2Control;
        private Components.AnimationFloatControl y2Control;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Assets.Components.StackPanel subPanel;
        private System.Windows.Forms.Panel panel2;
        private Assets.Components.Vector3Control position1Control;
        private System.Windows.Forms.Label label3;
        private Assets.Components.StackPanel positionPanel;
        private System.Windows.Forms.Panel panel1;
        private Assets.Components.Vector3Control position2Control;
        private System.Windows.Forms.Label label1;
        private Assets.Components.StackPanel gradientPanel;
        private Components.AnimationColorControl color1Control;
        private Components.AnimationColorControl color2Control;
    }
}
