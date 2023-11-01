
namespace CDK.Assets.Meshing
{
    partial class MeshSelectionControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.animationClearButton = new System.Windows.Forms.Button();
            this.animationComboBox = new CDK.Assets.Components.ComboBox();
            this.geometryComboBox = new CDK.Assets.Components.ComboBox();
            this.assetControl = new CDK.Assets.AssetSelectControl();
            this.frameDivisionLabel = new System.Windows.Forms.Label();
            this.animationAssetControl = new CDK.Assets.AssetSelectControl();
            this.label3 = new System.Windows.Forms.Label();
            this.frameDivisionComboBox = new CDK.Assets.Components.ComboBox();
            this.animationPanel = new System.Windows.Forms.Panel();
            this.geometryPanel = new System.Windows.Forms.Panel();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.attrPanel = new System.Windows.Forms.Panel();
            this.loopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.label7 = new System.Windows.Forms.Label();
            this.collisionCheckBox = new System.Windows.Forms.CheckBox();
            this.animationPanel.SuspendLayout();
            this.geometryPanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.attrPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Geometry";
            // 
            // animationClearButton
            // 
            this.animationClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.animationClearButton.Location = new System.Drawing.Point(297, 27);
            this.animationClearButton.Name = "animationClearButton";
            this.animationClearButton.Size = new System.Drawing.Size(23, 23);
            this.animationClearButton.TabIndex = 17;
            this.animationClearButton.Text = "-";
            this.animationClearButton.UseVisualStyleBackColor = true;
            this.animationClearButton.Click += new System.EventHandler(this.AnimationClearButton_Click);
            // 
            // animationComboBox
            // 
            this.animationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationComboBox.FormattingEnabled = true;
            this.animationComboBox.Location = new System.Drawing.Point(90, 28);
            this.animationComboBox.Name = "animationComboBox";
            this.animationComboBox.Size = new System.Drawing.Size(200, 20);
            this.animationComboBox.TabIndex = 15;
            // 
            // geometryComboBox
            // 
            this.geometryComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.geometryComboBox.FormattingEnabled = true;
            this.geometryComboBox.Location = new System.Drawing.Point(90, 27);
            this.geometryComboBox.Name = "geometryComboBox";
            this.geometryComboBox.Size = new System.Drawing.Size(230, 20);
            this.geometryComboBox.TabIndex = 13;
            // 
            // assetControl
            // 
            this.assetControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assetControl.Enabled = false;
            this.assetControl.Location = new System.Drawing.Point(90, 0);
            this.assetControl.Name = "assetControl";
            this.assetControl.Size = new System.Drawing.Size(230, 22);
            this.assetControl.TabIndex = 19;
            this.assetControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Mesh};
            // 
            // frameDivisionLabel
            // 
            this.frameDivisionLabel.AutoSize = true;
            this.frameDivisionLabel.Location = new System.Drawing.Point(146, 29);
            this.frameDivisionLabel.Name = "frameDivisionLabel";
            this.frameDivisionLabel.Size = new System.Drawing.Size(89, 12);
            this.frameDivisionLabel.TabIndex = 20;
            this.frameDivisionLabel.Text = "Frame Division";
            // 
            // animationAssetControl
            // 
            this.animationAssetControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationAssetControl.Enabled = false;
            this.animationAssetControl.Location = new System.Drawing.Point(90, 0);
            this.animationAssetControl.Name = "animationAssetControl";
            this.animationAssetControl.Size = new System.Drawing.Size(230, 22);
            this.animationAssetControl.TabIndex = 22;
            this.animationAssetControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Mesh};
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "Animation";
            // 
            // frameDivisionComboBox
            // 
            this.frameDivisionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.frameDivisionComboBox.FormattingEnabled = true;
            this.frameDivisionComboBox.Location = new System.Drawing.Point(90, 27);
            this.frameDivisionComboBox.Name = "frameDivisionComboBox";
            this.frameDivisionComboBox.Size = new System.Drawing.Size(50, 20);
            this.frameDivisionComboBox.TabIndex = 24;
            // 
            // animationPanel
            // 
            this.animationPanel.Controls.Add(this.animationComboBox);
            this.animationPanel.Controls.Add(this.label3);
            this.animationPanel.Controls.Add(this.animationClearButton);
            this.animationPanel.Controls.Add(this.animationAssetControl);
            this.animationPanel.Location = new System.Drawing.Point(0, 53);
            this.animationPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.animationPanel.Name = "animationPanel";
            this.animationPanel.Size = new System.Drawing.Size(320, 49);
            this.animationPanel.TabIndex = 25;
            // 
            // geometryPanel
            // 
            this.geometryPanel.Controls.Add(this.assetControl);
            this.geometryPanel.Controls.Add(this.label2);
            this.geometryPanel.Controls.Add(this.geometryComboBox);
            this.geometryPanel.Location = new System.Drawing.Point(0, 0);
            this.geometryPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.geometryPanel.Name = "geometryPanel";
            this.geometryPanel.Size = new System.Drawing.Size(320, 47);
            this.geometryPanel.TabIndex = 26;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.geometryPanel);
            this.panel.Controls.Add(this.animationPanel);
            this.panel.Controls.Add(this.attrPanel);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 155);
            this.panel.TabIndex = 27;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // attrPanel
            // 
            this.attrPanel.Controls.Add(this.loopControl);
            this.attrPanel.Controls.Add(this.label7);
            this.attrPanel.Controls.Add(this.frameDivisionComboBox);
            this.attrPanel.Controls.Add(this.collisionCheckBox);
            this.attrPanel.Controls.Add(this.frameDivisionLabel);
            this.attrPanel.Location = new System.Drawing.Point(0, 108);
            this.attrPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.attrPanel.Name = "attrPanel";
            this.attrPanel.Size = new System.Drawing.Size(320, 47);
            this.attrPanel.TabIndex = 27;
            // 
            // loopControl
            // 
            this.loopControl.FinishEnabled = true;
            this.loopControl.Location = new System.Drawing.Point(90, 0);
            this.loopControl.Name = "loopControl";
            this.loopControl.Size = new System.Drawing.Size(208, 21);
            this.loopControl.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 25;
            this.label7.Text = "Loop";
            // 
            // collisionCheckBox
            // 
            this.collisionCheckBox.AutoSize = true;
            this.collisionCheckBox.Location = new System.Drawing.Point(2, 28);
            this.collisionCheckBox.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.collisionCheckBox.Name = "collisionCheckBox";
            this.collisionCheckBox.Size = new System.Drawing.Size(73, 16);
            this.collisionCheckBox.TabIndex = 28;
            this.collisionCheckBox.Text = "Collision";
            this.collisionCheckBox.UseVisualStyleBackColor = true;
            // 
            // MeshSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "MeshSelectionControl";
            this.Size = new System.Drawing.Size(320, 155);
            this.animationPanel.ResumeLayout(false);
            this.animationPanel.PerformLayout();
            this.geometryPanel.ResumeLayout(false);
            this.geometryPanel.PerformLayout();
            this.panel.ResumeLayout(false);
            this.attrPanel.ResumeLayout(false);
            this.attrPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private Components.ComboBox geometryComboBox;
        private Components.ComboBox animationComboBox;
        private System.Windows.Forms.Button animationClearButton;
        private AssetSelectControl assetControl;
        private System.Windows.Forms.Label frameDivisionLabel;
        private AssetSelectControl animationAssetControl;
        private System.Windows.Forms.Label label3;
        private Components.ComboBox frameDivisionComboBox;
        private System.Windows.Forms.Panel animationPanel;
        private System.Windows.Forms.Panel geometryPanel;
        private Components.StackPanel panel;
        private System.Windows.Forms.Panel attrPanel;
        private Animations.Components.AnimationLoopControl loopControl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox collisionCheckBox;
    }
}
