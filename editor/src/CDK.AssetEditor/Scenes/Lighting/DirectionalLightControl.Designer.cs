
namespace CDK.Assets.Scenes
{
    partial class DirectionalLightControl
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
            this.castShadow2DCheckBox = new System.Windows.Forms.CheckBox();
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
            this.panel.Size = new System.Drawing.Size(320, 401);
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
            this.panel1.Controls.Add(this.castShadow2DCheckBox);
            this.panel1.Controls.Add(this.castShadowCheckBox);
            this.panel1.Location = new System.Drawing.Point(0, 268);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 22);
            this.panel1.TabIndex = 3;
            // 
            // castShadow2DCheckBox
            // 
            this.castShadow2DCheckBox.AutoSize = true;
            this.castShadow2DCheckBox.Location = new System.Drawing.Point(109, 3);
            this.castShadow2DCheckBox.Name = "castShadow2DCheckBox";
            this.castShadow2DCheckBox.Size = new System.Drawing.Size(118, 16);
            this.castShadow2DCheckBox.TabIndex = 2;
            this.castShadow2DCheckBox.Text = "Cast Shadow 2D";
            this.castShadow2DCheckBox.UseVisualStyleBackColor = true;
            // 
            // castShadowCheckBox
            // 
            this.castShadowCheckBox.AutoSize = true;
            this.castShadowCheckBox.Location = new System.Drawing.Point(3, 3);
            this.castShadowCheckBox.Name = "castShadowCheckBox";
            this.castShadowCheckBox.Size = new System.Drawing.Size(100, 16);
            this.castShadowCheckBox.TabIndex = 1;
            this.castShadowCheckBox.Text = "Cast Shadow";
            this.castShadowCheckBox.UseVisualStyleBackColor = true;
            // 
            // shadowControl
            // 
            this.shadowControl.Location = new System.Drawing.Point(3, 296);
            this.shadowControl.Name = "shadowControl";
            this.shadowControl.Size = new System.Drawing.Size(314, 102);
            this.shadowControl.TabIndex = 4;
            // 
            // DirectionalLightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "DirectionalLightControl";
            this.Size = new System.Drawing.Size(320, 401);
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
        private System.Windows.Forms.CheckBox castShadow2DCheckBox;
        private System.Windows.Forms.CheckBox castShadowCheckBox;
        private ShadowControl shadowControl;
    }
}
