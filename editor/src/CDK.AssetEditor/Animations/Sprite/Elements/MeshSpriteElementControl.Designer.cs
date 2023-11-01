namespace CDK.Assets.Animations.Sprite.Elements
{
    partial class MeshSpriteElementControl
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
            this.fitDurationButton = new System.Windows.Forms.Button();
            this.loopPanel = new System.Windows.Forms.Panel();
            this.loopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.selectionControl = new CDK.Assets.Meshing.MeshSelectionControl();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.loopPanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fitDurationButton
            // 
            this.fitDurationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fitDurationButton.Location = new System.Drawing.Point(268, 0);
            this.fitDurationButton.Name = "fitDurationButton";
            this.fitDurationButton.Size = new System.Drawing.Size(52, 22);
            this.fitDurationButton.TabIndex = 3;
            this.fitDurationButton.Text = "→";
            this.fitDurationButton.UseVisualStyleBackColor = true;
            this.fitDurationButton.Click += new System.EventHandler(this.FitDurationButton_Click);
            // 
            // loopPanel
            // 
            this.loopPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loopPanel.Controls.Add(this.label1);
            this.loopPanel.Controls.Add(this.fitDurationButton);
            this.loopPanel.Controls.Add(this.loopControl);
            this.loopPanel.Location = new System.Drawing.Point(0, 134);
            this.loopPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.loopPanel.Name = "loopPanel";
            this.loopPanel.Size = new System.Drawing.Size(320, 22);
            this.loopPanel.TabIndex = 4;
            // 
            // loopControl
            // 
            this.loopControl.Location = new System.Drawing.Point(90, 1);
            this.loopControl.Name = "loopControl";
            this.loopControl.Size = new System.Drawing.Size(144, 21);
            this.loopControl.TabIndex = 1;
            // 
            // selectionControl
            // 
            this.selectionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionControl.Location = new System.Drawing.Point(0, 0);
            this.selectionControl.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.selectionControl.Name = "selectionControl";
            this.selectionControl.Size = new System.Drawing.Size(320, 128);
            this.selectionControl.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.selectionControl);
            this.panel.Controls.Add(this.loopPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 156);
            this.panel.TabIndex = 5;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Loop";
            // 
            // MeshSpriteElementControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "MeshSpriteElementControl";
            this.Size = new System.Drawing.Size(320, 156);
            this.loopPanel.ResumeLayout(false);
            this.loopPanel.PerformLayout();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Assets.Meshing.MeshSelectionControl selectionControl;
        private Components.AnimationLoopControl loopControl;
        private System.Windows.Forms.Button fitDurationButton;
        private System.Windows.Forms.Panel loopPanel;
        private Assets.Components.StackPanel panel;
        private System.Windows.Forms.Label label1;
    }
}
