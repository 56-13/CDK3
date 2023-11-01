
namespace CDK.Assets.Scenes
{
    partial class SpotLightControl
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
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.colorControl = new CDK.Assets.Components.ColorFControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.attenuationComboBox = new CDK.Assets.Components.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dispersionControl = new CDK.Assets.Components.TrackControl();
            this.label2 = new System.Windows.Forms.Label();
            this.angleControl = new CDK.Assets.Components.TrackControl();
            this.label1 = new System.Windows.Forms.Label();
            this.castShadowCheckBox = new System.Windows.Forms.CheckBox();
            this.shadowControl = new CDK.Assets.Scenes.ShadowControl();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.objectControl);
            this.panel.Controls.Add(this.colorControl);
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.shadowControl);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 479);
            this.panel.TabIndex = 0;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(3, 3);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(314, 205);
            this.objectControl.TabIndex = 1;
            // 
            // colorControl
            // 
            this.colorControl.Location = new System.Drawing.Point(3, 214);
            this.colorControl.Name = "colorControl";
            this.colorControl.Size = new System.Drawing.Size(314, 48);
            this.colorControl.TabIndex = 2;
            this.colorControl.ValueGDI = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.attenuationComboBox);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.dispersionControl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.angleControl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.castShadowCheckBox);
            this.panel1.Location = new System.Drawing.Point(0, 268);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 100);
            this.panel1.TabIndex = 3;
            // 
            // attenuationComboBox
            // 
            this.attenuationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.attenuationComboBox.FormattingEnabled = true;
            this.attenuationComboBox.ItemHeight = 12;
            this.attenuationComboBox.Location = new System.Drawing.Point(94, 55);
            this.attenuationComboBox.Name = "attenuationComboBox";
            this.attenuationComboBox.Size = new System.Drawing.Size(111, 20);
            this.attenuationComboBox.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 12);
            this.label9.TabIndex = 40;
            this.label9.Text = "Attenuation";
            // 
            // dispersionControl
            // 
            this.dispersionControl.DecimalPlaces = 2;
            this.dispersionControl.Increment = 1F;
            this.dispersionControl.Location = new System.Drawing.Point(91, 27);
            this.dispersionControl.Maximum = 15F;
            this.dispersionControl.Minimum = 0F;
            this.dispersionControl.Name = "dispersionControl";
            this.dispersionControl.Size = new System.Drawing.Size(226, 21);
            this.dispersionControl.TabIndex = 5;
            this.dispersionControl.Value = 0F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Dispersion";
            // 
            // angleControl
            // 
            this.angleControl.DecimalPlaces = 2;
            this.angleControl.Increment = 1F;
            this.angleControl.Location = new System.Drawing.Point(91, 0);
            this.angleControl.Maximum = 30F;
            this.angleControl.Minimum = 0F;
            this.angleControl.Name = "angleControl";
            this.angleControl.Size = new System.Drawing.Size(226, 21);
            this.angleControl.TabIndex = 3;
            this.angleControl.Value = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Angle";
            // 
            // castShadowCheckBox
            // 
            this.castShadowCheckBox.AutoSize = true;
            this.castShadowCheckBox.Location = new System.Drawing.Point(3, 83);
            this.castShadowCheckBox.Name = "castShadowCheckBox";
            this.castShadowCheckBox.Size = new System.Drawing.Size(100, 16);
            this.castShadowCheckBox.TabIndex = 1;
            this.castShadowCheckBox.Text = "Cast Shadow";
            this.castShadowCheckBox.UseVisualStyleBackColor = true;
            // 
            // shadowControl
            // 
            this.shadowControl.Location = new System.Drawing.Point(3, 374);
            this.shadowControl.Name = "shadowControl";
            this.shadowControl.Size = new System.Drawing.Size(314, 102);
            this.shadowControl.TabIndex = 4;
            // 
            // SpotLightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "SpotLightControl";
            this.Size = new System.Drawing.Size(320, 479);
            this.panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.StackPanel panel;
        private SceneObjectControl objectControl;
        private Components.ColorFControl colorControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox castShadowCheckBox;
        private ShadowControl shadowControl;
        private Components.TrackControl angleControl;
        private System.Windows.Forms.Label label1;
        private Components.TrackControl dispersionControl;
        private System.Windows.Forms.Label label2;
        private Components.ComboBox attenuationComboBox;
        private System.Windows.Forms.Label label9;
    }
}
