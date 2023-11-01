
namespace CDK.Assets.Terrain
{
    partial class TerrainAltitudeControl
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
            this.slopeRadioButton = new System.Windows.Forms.RadioButton();
            this.modifyRadioButton = new System.Windows.Forms.RadioButton();
            this.panel = new System.Windows.Forms.Panel();
            this.attenuationControl = new CDK.Assets.Components.TrackControl();
            this.degreeControl = new CDK.Assets.Components.TrackControl();
            this.sizeControl = new CDK.Assets.Components.TrackControl();
            this.fillButton = new System.Windows.Forms.Button();
            this.allDegreeUpDown = new CDK.Assets.Components.NumericUpDown();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.modeComboBox = new CDK.Assets.Components.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allDegreeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // slopeRadioButton
            // 
            this.slopeRadioButton.AutoSize = true;
            this.slopeRadioButton.Location = new System.Drawing.Point(67, 0);
            this.slopeRadioButton.Name = "slopeRadioButton";
            this.slopeRadioButton.Size = new System.Drawing.Size(55, 16);
            this.slopeRadioButton.TabIndex = 54;
            this.slopeRadioButton.Text = "Slope";
            this.slopeRadioButton.UseVisualStyleBackColor = true;
            // 
            // modifyRadioButton
            // 
            this.modifyRadioButton.AutoSize = true;
            this.modifyRadioButton.Checked = true;
            this.modifyRadioButton.Location = new System.Drawing.Point(0, 0);
            this.modifyRadioButton.Name = "modifyRadioButton";
            this.modifyRadioButton.Size = new System.Drawing.Size(61, 16);
            this.modifyRadioButton.TabIndex = 53;
            this.modifyRadioButton.TabStop = true;
            this.modifyRadioButton.Text = "Modify";
            this.modifyRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.attenuationControl);
            this.panel.Controls.Add(this.degreeControl);
            this.panel.Controls.Add(this.sizeControl);
            this.panel.Controls.Add(this.fillButton);
            this.panel.Controls.Add(this.allDegreeUpDown);
            this.panel.Controls.Add(this.downButton);
            this.panel.Controls.Add(this.upButton);
            this.panel.Controls.Add(this.modeComboBox);
            this.panel.Controls.Add(this.label11);
            this.panel.Controls.Add(this.label10);
            this.panel.Controls.Add(this.label9);
            this.panel.Location = new System.Drawing.Point(0, 22);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 104);
            this.panel.TabIndex = 55;
            // 
            // attenuationControl
            // 
            this.attenuationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.attenuationControl.DecimalPlaces = 2;
            this.attenuationControl.Increment = 0.01F;
            this.attenuationControl.Location = new System.Drawing.Point(73, 54);
            this.attenuationControl.Maximum = 0.9F;
            this.attenuationControl.Minimum = 0F;
            this.attenuationControl.Name = "attenuationControl";
            this.attenuationControl.Size = new System.Drawing.Size(247, 21);
            this.attenuationControl.TabIndex = 60;
            this.attenuationControl.Value = 0.5F;
            // 
            // degreeControl
            // 
            this.degreeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.degreeControl.DecimalPlaces = 2;
            this.degreeControl.Increment = 0.01F;
            this.degreeControl.Location = new System.Drawing.Point(73, 27);
            this.degreeControl.Maximum = 1F;
            this.degreeControl.Minimum = 0.01F;
            this.degreeControl.Name = "degreeControl";
            this.degreeControl.Size = new System.Drawing.Size(247, 21);
            this.degreeControl.TabIndex = 59;
            this.degreeControl.Value = 0.2F;
            // 
            // sizeControl
            // 
            this.sizeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sizeControl.DecimalPlaces = 2;
            this.sizeControl.Increment = 0.1F;
            this.sizeControl.Location = new System.Drawing.Point(73, 0);
            this.sizeControl.Maximum = 8F;
            this.sizeControl.Minimum = 0.5F;
            this.sizeControl.Name = "sizeControl";
            this.sizeControl.Size = new System.Drawing.Size(247, 21);
            this.sizeControl.TabIndex = 58;
            this.sizeControl.Value = 4F;
            // 
            // fillButton
            // 
            this.fillButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fillButton.Location = new System.Drawing.Point(268, 81);
            this.fillButton.Name = "fillButton";
            this.fillButton.Size = new System.Drawing.Size(52, 23);
            this.fillButton.TabIndex = 53;
            this.fillButton.Text = "Fill";
            this.fillButton.UseVisualStyleBackColor = true;
            this.fillButton.Click += new System.EventHandler(this.FillButton_Click);
            // 
            // allDegreeUpDown
            // 
            this.allDegreeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.allDegreeUpDown.DecimalPlaces = 2;
            this.allDegreeUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.allDegreeUpDown.Location = new System.Drawing.Point(86, 82);
            this.allDegreeUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.allDegreeUpDown.Name = "allDegreeUpDown";
            this.allDegreeUpDown.Size = new System.Drawing.Size(60, 21);
            this.allDegreeUpDown.TabIndex = 57;
            // 
            // downButton
            // 
            this.downButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downButton.Location = new System.Drawing.Point(210, 82);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(52, 23);
            this.downButton.TabIndex = 56;
            this.downButton.Text = "Down";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // upButton
            // 
            this.upButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.upButton.Location = new System.Drawing.Point(152, 81);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(52, 23);
            this.upButton.TabIndex = 55;
            this.upButton.Text = "Up";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(0, 82);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(80, 20);
            this.modeComboBox.TabIndex = 54;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 4);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(30, 12);
            this.label11.TabIndex = 50;
            this.label11.Text = "Size";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 31);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 12);
            this.label10.TabIndex = 51;
            this.label10.Text = "Degree";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 12);
            this.label9.TabIndex = 52;
            this.label9.Text = "Attenuation";
            // 
            // TerrainAltitudeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.slopeRadioButton);
            this.Controls.Add(this.modifyRadioButton);
            this.Name = "TerrainAltitudeControl";
            this.Size = new System.Drawing.Size(320, 126);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allDegreeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton slopeRadioButton;
        private System.Windows.Forms.RadioButton modifyRadioButton;
        private System.Windows.Forms.Panel panel;
        private Components.TrackControl attenuationControl;
        private Components.TrackControl degreeControl;
        private Components.TrackControl sizeControl;
        private System.Windows.Forms.Button fillButton;
        private Components.NumericUpDown allDegreeUpDown;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private Components.ComboBox modeComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
    }
}
