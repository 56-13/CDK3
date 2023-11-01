namespace CDK.Assets.Scenes
{
    partial class BoxControl
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
            this.objectPanel = new CDK.Assets.Components.StackPanel();
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.extentControl = new CDK.Assets.Components.Vector3TrackControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            this.mainPanel.SuspendLayout();
            this.objectPanel.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.mainPanel.Controls.Add(this.objectPanel);
            this.mainPanel.Controls.Add(this.materialControl);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 1095);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectPanel
            // 
            this.objectPanel.AutoSize = true;
            this.objectPanel.Collapsible = true;
            this.objectPanel.Controls.Add(this.objectControl);
            this.objectPanel.Controls.Add(this.panel1);
            this.objectPanel.Location = new System.Drawing.Point(0, 0);
            this.objectPanel.Margin = new System.Windows.Forms.Padding(0);
            this.objectPanel.Name = "objectPanel";
            this.objectPanel.Size = new System.Drawing.Size(320, 313);
            this.objectPanel.TabIndex = 15;
            this.objectPanel.Title = "Object";
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(3, 24);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(314, 205);
            this.objectControl.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.extentControl);
            this.panel1.Location = new System.Drawing.Point(3, 235);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 75);
            this.panel1.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Extent";
            // 
            // extentControl
            // 
            this.extentControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.extentControl.DecimalPlaces = 2;
            this.extentControl.Increment = 1F;
            this.extentControl.Location = new System.Drawing.Point(90, 0);
            this.extentControl.Maximum = 1000F;
            this.extentControl.Minimum = 1F;
            this.extentControl.Name = "extentControl";
            this.extentControl.Size = new System.Drawing.Size(224, 75);
            this.extentControl.TabIndex = 0;
            // 
            // materialControl
            // 
            this.materialControl.Location = new System.Drawing.Point(0, 313);
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
            this.splitContainer.Size = new System.Drawing.Size(320, 1095);
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
            // BoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "BoxControl";
            this.Size = new System.Drawing.Size(320, 1095);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.objectPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.StackPanel mainPanel;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Components.StackPanel subPanel;
        private Components.StackPanel objectPanel;
        private SceneObjectControl objectControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private Components.Vector3TrackControl extentControl;
    }
}
