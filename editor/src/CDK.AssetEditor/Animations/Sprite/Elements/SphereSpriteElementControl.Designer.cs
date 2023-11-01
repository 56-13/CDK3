namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class SphereSpriteElementControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.positionPanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.positionControl = new CDK.Assets.Components.Vector3Control();
            this.label3 = new System.Windows.Forms.Label();
            this.xControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.yControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.zControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.shapePanel = new CDK.Assets.Components.StackPanel();
            this.radiusControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.shapePanel.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer.Size = new System.Drawing.Size(320, 955);
            this.splitContainer.SplitterDistance = 158;
            this.splitContainer.TabIndex = 1;
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.positionPanel);
            this.mainPanel.Controls.Add(this.shapePanel);
            this.mainPanel.Controls.Add(this.materialControl);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 955);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // positionPanel
            // 
            this.positionPanel.AutoSize = true;
            this.positionPanel.Collapsible = true;
            this.positionPanel.Controls.Add(this.panel2);
            this.positionPanel.Controls.Add(this.xControl);
            this.positionPanel.Controls.Add(this.yControl);
            this.positionPanel.Controls.Add(this.zControl);
            this.positionPanel.Location = new System.Drawing.Point(0, 0);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(320, 126);
            this.positionPanel.TabIndex = 15;
            this.positionPanel.Title = "Position";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.positionControl);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 21);
            this.panel2.TabIndex = 14;
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Position";
            // 
            // xControl
            // 
            this.xControl.Location = new System.Drawing.Point(3, 51);
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.xControl.Size = new System.Drawing.Size(314, 20);
            this.xControl.TabIndex = 5;
            this.xControl.Title = "X";
            // 
            // yControl
            // 
            this.yControl.Location = new System.Drawing.Point(3, 77);
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.yControl.Size = new System.Drawing.Size(314, 20);
            this.yControl.TabIndex = 7;
            this.yControl.Title = "Y";
            // 
            // zControl
            // 
            this.zControl.Location = new System.Drawing.Point(3, 103);
            this.zControl.Name = "zControl";
            this.zControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.zControl.Size = new System.Drawing.Size(314, 20);
            this.zControl.TabIndex = 6;
            this.zControl.Title = "Z";
            // 
            // shapePanel
            // 
            this.shapePanel.AutoSize = true;
            this.shapePanel.Collapsible = true;
            this.shapePanel.Controls.Add(this.radiusControl);
            this.shapePanel.Location = new System.Drawing.Point(0, 126);
            this.shapePanel.Margin = new System.Windows.Forms.Padding(0);
            this.shapePanel.Name = "shapePanel";
            this.shapePanel.Size = new System.Drawing.Size(320, 47);
            this.shapePanel.TabIndex = 16;
            this.shapePanel.Title = "Shape";
            // 
            // radiusControl
            // 
            this.radiusControl.Location = new System.Drawing.Point(3, 24);
            this.radiusControl.Name = "radiusControl";
            this.radiusControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radiusControl.Size = new System.Drawing.Size(314, 20);
            this.radiusControl.TabIndex = 9;
            this.radiusControl.Title = "Radius";
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 173);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 0;
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
            // SphereSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "SphereSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 955);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.positionPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.shapePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Assets.Components.StackPanel mainPanel;
        private Components.AnimationFloatControl xControl;
        private Components.AnimationFloatControl yControl;
        private Components.AnimationFloatControl zControl;
        private Components.AnimationFloatControl radiusControl;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Assets.Components.StackPanel subPanel;
        private System.Windows.Forms.Panel panel2;
        private Assets.Components.Vector3Control positionControl;
        private System.Windows.Forms.Label label3;
        private Assets.Components.StackPanel positionPanel;
        private Assets.Components.StackPanel shapePanel;
    }
}
