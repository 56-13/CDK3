
namespace CDK.Assets.Terrain
{
    partial class TerrainSurfaceMaterialControl
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
            this.scaleControl = new CDK.Assets.Components.TrackControl();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rotationControl = new CDK.Assets.Components.TrackControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.triPlanerCheckBox = new System.Windows.Forms.CheckBox();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.shapePanel = new CDK.Assets.Components.StackPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel.SuspendLayout();
            this.shapePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // scaleControl
            // 
            this.scaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleControl.DecimalPlaces = 2;
            this.scaleControl.Increment = 0.1F;
            this.scaleControl.Location = new System.Drawing.Point(90, 0);
            this.scaleControl.Maximum = 10F;
            this.scaleControl.Minimum = 0.1F;
            this.scaleControl.Name = "scaleControl";
            this.scaleControl.Size = new System.Drawing.Size(224, 21);
            this.scaleControl.TabIndex = 55;
            this.scaleControl.Value = 1F;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 4);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 12);
            this.label8.TabIndex = 56;
            this.label8.Text = "Scale";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 12);
            this.label9.TabIndex = 57;
            this.label9.Text = "Rotation";
            // 
            // rotationControl
            // 
            this.rotationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationControl.Increment = 1F;
            this.rotationControl.Location = new System.Drawing.Point(90, 27);
            this.rotationControl.Maximum = 360F;
            this.rotationControl.Minimum = 0F;
            this.rotationControl.Name = "rotationControl";
            this.rotationControl.Size = new System.Drawing.Size(224, 21);
            this.rotationControl.TabIndex = 58;
            this.rotationControl.Value = 0F;
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 101);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 59;
            // 
            // triPlanerCheckBox
            // 
            this.triPlanerCheckBox.AutoSize = true;
            this.triPlanerCheckBox.Location = new System.Drawing.Point(0, 58);
            this.triPlanerCheckBox.Name = "triPlanerCheckBox";
            this.triPlanerCheckBox.Size = new System.Drawing.Size(79, 16);
            this.triPlanerCheckBox.TabIndex = 60;
            this.triPlanerCheckBox.Text = "Tri Planer";
            this.triPlanerCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.shapePanel);
            this.panel.Controls.Add(this.materialControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 883);
            this.panel.TabIndex = 61;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // shapePanel
            // 
            this.shapePanel.AutoSize = true;
            this.shapePanel.Collapsible = true;
            this.shapePanel.Controls.Add(this.panel1);
            this.shapePanel.Location = new System.Drawing.Point(0, 0);
            this.shapePanel.Margin = new System.Windows.Forms.Padding(0);
            this.shapePanel.Name = "shapePanel";
            this.shapePanel.Size = new System.Drawing.Size(320, 101);
            this.shapePanel.TabIndex = 0;
            this.shapePanel.Title = "Shape";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.scaleControl);
            this.panel1.Controls.Add(this.triPlanerCheckBox);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.rotationControl);
            this.panel1.Location = new System.Drawing.Point(3, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 74);
            this.panel1.TabIndex = 0;
            // 
            // TerrainSurfaceMaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "TerrainSurfaceMaterialControl";
            this.Size = new System.Drawing.Size(320, 883);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.shapePanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Components.TrackControl scaleControl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private Components.TrackControl rotationControl;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.CheckBox triPlanerCheckBox;
        private Components.StackPanel panel;
        private Components.StackPanel shapePanel;
        private System.Windows.Forms.Panel panel1;
    }
}
