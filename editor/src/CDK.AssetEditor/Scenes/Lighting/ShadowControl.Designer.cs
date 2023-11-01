
namespace CDK.Assets.Scenes
{
    partial class ShadowControl
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
            this.label12 = new System.Windows.Forms.Label();
            this.pixel32RadioButton = new System.Windows.Forms.RadioButton();
            this.pixel16RadioButton = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.resolutionComboBox = new CDK.Assets.Components.ComboBox();
            this.biasControl = new CDK.Assets.Components.TrackControl();
            this.bleedingControl = new CDK.Assets.Components.TrackControl();
            this.blurControl = new CDK.Assets.Components.TrackControl();
            this.SuspendLayout();
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 4);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 12);
            this.label12.TabIndex = 33;
            this.label12.Text = "Precision";
            // 
            // pixel32RadioButton
            // 
            this.pixel32RadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pixel32RadioButton.AutoSize = true;
            this.pixel32RadioButton.Location = new System.Drawing.Point(270, 2);
            this.pixel32RadioButton.Name = "pixel32RadioButton";
            this.pixel32RadioButton.Size = new System.Drawing.Size(50, 16);
            this.pixel32RadioButton.TabIndex = 32;
            this.pixel32RadioButton.Text = "FP32";
            this.pixel32RadioButton.UseVisualStyleBackColor = true;
            // 
            // pixel16RadioButton
            // 
            this.pixel16RadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pixel16RadioButton.AutoSize = true;
            this.pixel16RadioButton.Checked = true;
            this.pixel16RadioButton.Location = new System.Drawing.Point(214, 2);
            this.pixel16RadioButton.Name = "pixel16RadioButton";
            this.pixel16RadioButton.Size = new System.Drawing.Size(50, 16);
            this.pixel16RadioButton.TabIndex = 31;
            this.pixel16RadioButton.TabStop = true;
            this.pixel16RadioButton.Text = "FP16";
            this.pixel16RadioButton.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 12);
            this.label9.TabIndex = 30;
            this.label9.Text = "Bias";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 85);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 12);
            this.label10.TabIndex = 28;
            this.label10.Text = "Bleeding";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 12);
            this.label11.TabIndex = 26;
            this.label11.Text = "Blur";
            // 
            // resolutionComboBox
            // 
            this.resolutionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resolutionComboBox.FormattingEnabled = true;
            this.resolutionComboBox.Location = new System.Drawing.Point(91, 0);
            this.resolutionComboBox.Name = "resolutionComboBox";
            this.resolutionComboBox.Size = new System.Drawing.Size(117, 20);
            this.resolutionComboBox.TabIndex = 34;
            // 
            // biasControl
            // 
            this.biasControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.biasControl.DecimalPlaces = 4;
            this.biasControl.Increment = 0.0001F;
            this.biasControl.Location = new System.Drawing.Point(91, 54);
            this.biasControl.Maximum = 0.01F;
            this.biasControl.Minimum = 0.0001F;
            this.biasControl.Name = "biasControl";
            this.biasControl.Size = new System.Drawing.Size(229, 21);
            this.biasControl.TabIndex = 29;
            this.biasControl.Value = 0.001F;
            // 
            // bleedingControl
            // 
            this.bleedingControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bleedingControl.DecimalPlaces = 2;
            this.bleedingControl.Increment = 0.01F;
            this.bleedingControl.Location = new System.Drawing.Point(91, 81);
            this.bleedingControl.Maximum = 0.99F;
            this.bleedingControl.Minimum = 0F;
            this.bleedingControl.Name = "bleedingControl";
            this.bleedingControl.Size = new System.Drawing.Size(229, 21);
            this.bleedingControl.TabIndex = 27;
            this.bleedingControl.Value = 0.9F;
            // 
            // blurControl
            // 
            this.blurControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blurControl.DecimalPlaces = 1;
            this.blurControl.Increment = 0.1F;
            this.blurControl.Location = new System.Drawing.Point(91, 27);
            this.blurControl.Maximum = 8F;
            this.blurControl.Minimum = 1F;
            this.blurControl.Name = "blurControl";
            this.blurControl.Size = new System.Drawing.Size(229, 21);
            this.blurControl.TabIndex = 25;
            this.blurControl.Value = 4F;
            // 
            // ShadowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resolutionComboBox);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.pixel32RadioButton);
            this.Controls.Add(this.pixel16RadioButton);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.biasControl);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.bleedingControl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.blurControl);
            this.Name = "ShadowControl";
            this.Size = new System.Drawing.Size(320, 102);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton pixel32RadioButton;
        private System.Windows.Forms.RadioButton pixel16RadioButton;
        private System.Windows.Forms.Label label9;
        private Components.TrackControl biasControl;
        private System.Windows.Forms.Label label10;
        private Components.TrackControl bleedingControl;
        private System.Windows.Forms.Label label11;
        private Components.TrackControl blurControl;
        private Components.ComboBox resolutionComboBox;
    }
}
