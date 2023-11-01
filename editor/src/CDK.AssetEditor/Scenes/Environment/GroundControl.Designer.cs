
namespace CDK.Assets.Scenes
{
    partial class GroundControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.widthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.heightUpDown = new CDK.Assets.Components.NumericUpDown();
            this.altitudeUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gridUpDown = new CDK.Assets.Components.NumericUpDown();
            this.gridColorControl = new CDK.Assets.Components.ColorControl();
            this.materialControl = new CDK.Assets.Texturing.MaterialControl();
            this.gridVisibleCheckBox = new System.Windows.Forms.CheckBox();
            this.groundPanel = new System.Windows.Forms.Panel();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.terrainPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.terrainControl = new CDK.Assets.AssetSelectControl();
            this.configPanel = new System.Windows.Forms.Panel();
            this.configRenameButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.configRemoveButton = new System.Windows.Forms.Button();
            this.configAddButton = new System.Windows.Forms.Button();
            this.configComboBox = new CDK.Assets.Components.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.altitudeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUpDown)).BeginInit();
            this.groundPanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.terrainPanel.SuspendLayout();
            this.configPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width / Height";
            // 
            // widthUpDown
            // 
            this.widthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.widthUpDown.Location = new System.Drawing.Point(195, 0);
            this.widthUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.widthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.Size = new System.Drawing.Size(60, 21);
            this.widthUpDown.TabIndex = 1;
            this.widthUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // heightUpDown
            // 
            this.heightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.heightUpDown.Location = new System.Drawing.Point(261, 0);
            this.heightUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.heightUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.Size = new System.Drawing.Size(60, 21);
            this.heightUpDown.TabIndex = 2;
            this.heightUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // altitudeUpDown
            // 
            this.altitudeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.altitudeUpDown.Location = new System.Drawing.Point(195, 27);
            this.altitudeUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.altitudeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.altitudeUpDown.Name = "altitudeUpDown";
            this.altitudeUpDown.Size = new System.Drawing.Size(60, 21);
            this.altitudeUpDown.TabIndex = 3;
            this.altitudeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Altitude / Grid";
            // 
            // gridUpDown
            // 
            this.gridUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gridUpDown.Location = new System.Drawing.Point(261, 27);
            this.gridUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridUpDown.Name = "gridUpDown";
            this.gridUpDown.Size = new System.Drawing.Size(60, 21);
            this.gridUpDown.TabIndex = 5;
            this.gridUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gridColorControl
            // 
            this.gridColorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridColorControl.BackColor = System.Drawing.Color.White;
            this.gridColorControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridColorControl.Location = new System.Drawing.Point(91, 54);
            this.gridColorControl.Name = "gridColorControl";
            this.gridColorControl.Size = new System.Drawing.Size(230, 21);
            this.gridColorControl.TabIndex = 44;
            this.gridColorControl.ValueGDI = System.Drawing.Color.Empty;
            // 
            // materialControl
            // 
            this.materialControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialControl.Location = new System.Drawing.Point(0, 136);
            this.materialControl.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(320, 782);
            this.materialControl.TabIndex = 45;
            // 
            // gridVisibleCheckBox
            // 
            this.gridVisibleCheckBox.AutoSize = true;
            this.gridVisibleCheckBox.Location = new System.Drawing.Point(1, 56);
            this.gridVisibleCheckBox.Name = "gridVisibleCheckBox";
            this.gridVisibleCheckBox.Size = new System.Drawing.Size(81, 16);
            this.gridVisibleCheckBox.TabIndex = 46;
            this.gridVisibleCheckBox.Text = "Grid Color";
            this.gridVisibleCheckBox.UseVisualStyleBackColor = true;
            // 
            // groundPanel
            // 
            this.groundPanel.Controls.Add(this.widthUpDown);
            this.groundPanel.Controls.Add(this.gridVisibleCheckBox);
            this.groundPanel.Controls.Add(this.label1);
            this.groundPanel.Controls.Add(this.heightUpDown);
            this.groundPanel.Controls.Add(this.gridColorControl);
            this.groundPanel.Controls.Add(this.altitudeUpDown);
            this.groundPanel.Controls.Add(this.gridUpDown);
            this.groundPanel.Controls.Add(this.label2);
            this.groundPanel.Location = new System.Drawing.Point(0, 55);
            this.groundPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groundPanel.Name = "groundPanel";
            this.groundPanel.Size = new System.Drawing.Size(320, 75);
            this.groundPanel.TabIndex = 47;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.terrainPanel);
            this.panel.Controls.Add(this.configPanel);
            this.panel.Controls.Add(this.groundPanel);
            this.panel.Controls.Add(this.materialControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 918);
            this.panel.TabIndex = 48;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // terrainPanel
            // 
            this.terrainPanel.Controls.Add(this.label3);
            this.terrainPanel.Controls.Add(this.terrainControl);
            this.terrainPanel.Location = new System.Drawing.Point(0, 0);
            this.terrainPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.terrainPanel.Name = "terrainPanel";
            this.terrainPanel.Size = new System.Drawing.Size(320, 22);
            this.terrainPanel.TabIndex = 48;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Terrain";
            // 
            // terrainControl
            // 
            this.terrainControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.terrainControl.Location = new System.Drawing.Point(91, 0);
            this.terrainControl.Name = "terrainControl";
            this.terrainControl.Size = new System.Drawing.Size(229, 22);
            this.terrainControl.TabIndex = 0;
            this.terrainControl.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Terrain};
            // 
            // configPanel
            // 
            this.configPanel.Controls.Add(this.configRenameButton);
            this.configPanel.Controls.Add(this.label4);
            this.configPanel.Controls.Add(this.configRemoveButton);
            this.configPanel.Controls.Add(this.configAddButton);
            this.configPanel.Controls.Add(this.configComboBox);
            this.configPanel.Location = new System.Drawing.Point(0, 28);
            this.configPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(320, 21);
            this.configPanel.TabIndex = 49;
            // 
            // configRenameButton
            // 
            this.configRenameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.configRenameButton.Location = new System.Drawing.Point(234, -1);
            this.configRenameButton.Name = "configRenameButton";
            this.configRenameButton.Size = new System.Drawing.Size(28, 22);
            this.configRenameButton.TabIndex = 32;
            this.configRenameButton.Text = "Aa";
            this.configRenameButton.UseVisualStyleBackColor = true;
            this.configRenameButton.Click += new System.EventHandler(this.ConfigRenameButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Ground";
            // 
            // configRemoveButton
            // 
            this.configRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.configRemoveButton.Location = new System.Drawing.Point(297, -1);
            this.configRemoveButton.Name = "configRemoveButton";
            this.configRemoveButton.Size = new System.Drawing.Size(23, 22);
            this.configRemoveButton.TabIndex = 23;
            this.configRemoveButton.Text = "-";
            this.configRemoveButton.UseVisualStyleBackColor = true;
            this.configRemoveButton.Click += new System.EventHandler(this.ConfigRemoveButton_Click);
            // 
            // configAddButton
            // 
            this.configAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.configAddButton.Location = new System.Drawing.Point(268, -1);
            this.configAddButton.Name = "configAddButton";
            this.configAddButton.Size = new System.Drawing.Size(23, 22);
            this.configAddButton.TabIndex = 22;
            this.configAddButton.Text = "+";
            this.configAddButton.UseVisualStyleBackColor = true;
            this.configAddButton.Click += new System.EventHandler(this.ConfigAddButton_Click);
            // 
            // configComboBox
            // 
            this.configComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configComboBox.FormattingEnabled = true;
            this.configComboBox.ItemHeight = 12;
            this.configComboBox.Location = new System.Drawing.Point(91, 0);
            this.configComboBox.Name = "configComboBox";
            this.configComboBox.Size = new System.Drawing.Size(137, 20);
            this.configComboBox.TabIndex = 21;
            // 
            // GroundControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "GroundControl";
            this.Size = new System.Drawing.Size(320, 918);
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.altitudeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUpDown)).EndInit();
            this.groundPanel.ResumeLayout(false);
            this.groundPanel.PerformLayout();
            this.panel.ResumeLayout(false);
            this.terrainPanel.ResumeLayout(false);
            this.terrainPanel.PerformLayout();
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Components.NumericUpDown widthUpDown;
        private Components.NumericUpDown heightUpDown;
        private Components.NumericUpDown altitudeUpDown;
        private System.Windows.Forms.Label label2;
        private Components.NumericUpDown gridUpDown;
        private Components.ColorControl gridColorControl;
        private Texturing.MaterialControl materialControl;
        private System.Windows.Forms.CheckBox gridVisibleCheckBox;
        private System.Windows.Forms.Panel groundPanel;
        private Components.StackPanel panel;
        private System.Windows.Forms.Panel terrainPanel;
        private System.Windows.Forms.Label label3;
        private AssetSelectControl terrainControl;
        private System.Windows.Forms.Panel configPanel;
        private System.Windows.Forms.Button configRemoveButton;
        private System.Windows.Forms.Button configAddButton;
        private Components.ComboBox configComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button configRenameButton;
    }
}
