
namespace CDK.Assets.Animations.Sources
{
    partial class AnimationSourceImageControl
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.rootImageControl = new CDK.Assets.AssetSelectControl();
            this.label1 = new System.Windows.Forms.Label();
            this.subImageControl = new CDK.Assets.Texturing.SubImageSelectControl();
            this.durationPanel = new System.Windows.Forms.Panel();
            this.loopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.durationControl = new CDK.Assets.Components.TrackControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.panel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.durationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.panel2);
            this.panel.Controls.Add(this.subImageControl);
            this.panel.Controls.Add(this.durationPanel);
            this.panel.Controls.Add(this.materialControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 1067);
            this.panel.TabIndex = 59;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rootImageControl);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 22);
            this.panel2.TabIndex = 4;
            // 
            // rootImageControl
            // 
            this.rootImageControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rootImageControl.Location = new System.Drawing.Point(90, 0);
            this.rootImageControl.Name = "rootImageControl";
            this.rootImageControl.Size = new System.Drawing.Size(230, 22);
            this.rootImageControl.TabIndex = 50;
            this.rootImageControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.RootImage};
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image";
            // 
            // subImageControl
            // 
            this.subImageControl.Location = new System.Drawing.Point(0, 28);
            this.subImageControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.subImageControl.Name = "subImageControl";
            this.subImageControl.Size = new System.Drawing.Size(320, 200);
            this.subImageControl.TabIndex = 2;
            // 
            // durationPanel
            // 
            this.durationPanel.Controls.Add(this.loopControl);
            this.durationPanel.Controls.Add(this.label7);
            this.durationPanel.Controls.Add(this.label6);
            this.durationPanel.Controls.Add(this.durationControl);
            this.durationPanel.Location = new System.Drawing.Point(0, 234);
            this.durationPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.durationPanel.Name = "durationPanel";
            this.durationPanel.Size = new System.Drawing.Size(320, 48);
            this.durationPanel.TabIndex = 68;
            // 
            // loopControl
            // 
            this.loopControl.Location = new System.Drawing.Point(87, 27);
            this.loopControl.Name = "loopControl";
            this.loopControl.Size = new System.Drawing.Size(161, 21);
            this.loopControl.TabIndex = 24;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 23;
            this.label7.Text = "Loop";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 12);
            this.label6.TabIndex = 22;
            this.label6.Text = "Duration";
            // 
            // durationControl
            // 
            this.durationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.durationControl.DecimalPlaces = 2;
            this.durationControl.Increment = 0.01F;
            this.durationControl.Location = new System.Drawing.Point(87, 0);
            this.durationControl.Maximum = 10F;
            this.durationControl.Minimum = 0F;
            this.durationControl.Name = "durationControl";
            this.durationControl.Size = new System.Drawing.Size(233, 21);
            this.durationControl.TabIndex = 0;
            this.durationControl.Value = 0F;
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 285);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 5;
            // 
            // AnimationSourceImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "AnimationSourceImageControl";
            this.Size = new System.Drawing.Size(320, 1067);
            this.panel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.durationPanel.ResumeLayout(false);
            this.durationPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private AssetSelectControl rootImageControl;
        private Assets.Components.StackPanel panel;
        private Texturing.SubImageSelectControl subImageControl;
        private System.Windows.Forms.Panel panel2;
        private Assets.Texturing.MaterialControl materialControl;
        private System.Windows.Forms.Panel durationPanel;
        private Components.AnimationLoopControl loopControl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Assets.Components.TrackControl durationControl;
    }
}
