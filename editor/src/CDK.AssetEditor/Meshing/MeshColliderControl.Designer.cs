
namespace CDK.Assets.Meshing
{
    partial class MeshColliderControl
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
            this.shapeComboBox = new CDK.Assets.Components.ComboBox();
            this.radius2Control = new CDK.Assets.Components.TrackControl();
            this.label1 = new System.Windows.Forms.Label();
            this.radius0Control = new CDK.Assets.Components.TrackControl();
            this.radius1Control = new CDK.Assets.Components.TrackControl();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nodeNameComboBox = new CDK.Assets.Components.ComboBox();
            this.relativeCheckBox = new System.Windows.Forms.CheckBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.positionControl = new CDK.Assets.Components.Vector3Control();
            this.rotationControl = new CDK.Assets.Components.Vector3Control();
            this.inclusiveCheckBox = new System.Windows.Forms.CheckBox();
            this.nodeNameClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // shapeComboBox
            // 
            this.shapeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeComboBox.FormattingEnabled = true;
            this.shapeComboBox.Location = new System.Drawing.Point(72, 26);
            this.shapeComboBox.Name = "shapeComboBox";
            this.shapeComboBox.Size = new System.Drawing.Size(213, 20);
            this.shapeComboBox.TabIndex = 9;
            this.shapeComboBox.SelectedIndexChanged += new System.EventHandler(this.ShapeComboBox_SelectedIndexChanged);
            // 
            // radius2Control
            // 
            this.radius2Control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radius2Control.Increment = 1F;
            this.radius2Control.Location = new System.Drawing.Point(72, 187);
            this.radius2Control.Maximum = 1000F;
            this.radius2Control.Minimum = -1000F;
            this.radius2Control.Name = "radius2Control";
            this.radius2Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radius2Control.Size = new System.Drawing.Size(213, 21);
            this.radius2Control.TabIndex = 13;
            this.radius2Control.Value = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Shape";
            // 
            // radius0Control
            // 
            this.radius0Control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radius0Control.Increment = 1F;
            this.radius0Control.Location = new System.Drawing.Point(72, 133);
            this.radius0Control.Maximum = 1000F;
            this.radius0Control.Minimum = -1000F;
            this.radius0Control.Name = "radius0Control";
            this.radius0Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radius0Control.Size = new System.Drawing.Size(213, 21);
            this.radius0Control.TabIndex = 11;
            this.radius0Control.Value = 0F;
            // 
            // radius1Control
            // 
            this.radius1Control.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radius1Control.Increment = 1F;
            this.radius1Control.Location = new System.Drawing.Point(72, 160);
            this.radius1Control.Maximum = 1000F;
            this.radius1Control.Minimum = -1000F;
            this.radius1Control.Name = "radius1Control";
            this.radius1Control.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radius1Control.Size = new System.Drawing.Size(213, 21);
            this.radius1Control.TabIndex = 12;
            this.radius1Control.Value = 0F;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Radius";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Position";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "Rotation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Bone";
            // 
            // nodeNameComboBox
            // 
            this.nodeNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeNameComboBox.FormattingEnabled = true;
            this.nodeNameComboBox.Location = new System.Drawing.Point(72, 0);
            this.nodeNameComboBox.Name = "nodeNameComboBox";
            this.nodeNameComboBox.Size = new System.Drawing.Size(186, 20);
            this.nodeNameComboBox.TabIndex = 17;
            // 
            // relativeCheckBox
            // 
            this.relativeCheckBox.AutoSize = true;
            this.relativeCheckBox.Location = new System.Drawing.Point(72, 56);
            this.relativeCheckBox.Name = "relativeCheckBox";
            this.relativeCheckBox.Size = new System.Drawing.Size(68, 16);
            this.relativeCheckBox.TabIndex = 18;
            this.relativeCheckBox.Text = "Relative";
            this.relativeCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(220, 53);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(65, 20);
            this.resetButton.TabIndex = 19;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // positionControl
            // 
            this.positionControl.DecimalPlaces = 2;
            this.positionControl.Increment = 1F;
            this.positionControl.Location = new System.Drawing.Point(72, 79);
            this.positionControl.Maximum = 1000F;
            this.positionControl.Minimum = -1000F;
            this.positionControl.Name = "positionControl";
            this.positionControl.Size = new System.Drawing.Size(192, 21);
            this.positionControl.TabIndex = 20;
            // 
            // rotationControl
            // 
            this.rotationControl.DecimalPlaces = 2;
            this.rotationControl.Increment = 1F;
            this.rotationControl.Location = new System.Drawing.Point(72, 106);
            this.rotationControl.Maximum = 360F;
            this.rotationControl.Minimum = -360F;
            this.rotationControl.Name = "rotationControl";
            this.rotationControl.Size = new System.Drawing.Size(192, 21);
            this.rotationControl.TabIndex = 21;
            // 
            // inclusiveCheckBox
            // 
            this.inclusiveCheckBox.AutoSize = true;
            this.inclusiveCheckBox.Location = new System.Drawing.Point(146, 56);
            this.inclusiveCheckBox.Name = "inclusiveCheckBox";
            this.inclusiveCheckBox.Size = new System.Drawing.Size(74, 16);
            this.inclusiveCheckBox.TabIndex = 22;
            this.inclusiveCheckBox.Text = "Inclusive";
            this.inclusiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // nodeNameClearButton
            // 
            this.nodeNameClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeNameClearButton.Location = new System.Drawing.Point(264, -1);
            this.nodeNameClearButton.Name = "nodeNameClearButton";
            this.nodeNameClearButton.Size = new System.Drawing.Size(22, 22);
            this.nodeNameClearButton.TabIndex = 23;
            this.nodeNameClearButton.Text = "-";
            this.nodeNameClearButton.UseVisualStyleBackColor = true;
            this.nodeNameClearButton.Click += new System.EventHandler(this.NodeNameClearButton_Click);
            // 
            // MeshColliderAttributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nodeNameClearButton);
            this.Controls.Add(this.inclusiveCheckBox);
            this.Controls.Add(this.rotationControl);
            this.Controls.Add(this.positionControl);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.relativeCheckBox);
            this.Controls.Add(this.nodeNameComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.shapeComboBox);
            this.Controls.Add(this.radius2Control);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radius0Control);
            this.Controls.Add(this.radius1Control);
            this.Controls.Add(this.label3);
            this.Name = "MeshColliderAttributeControl";
            this.Size = new System.Drawing.Size(285, 208);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.ComboBox shapeComboBox;
        private Components.TrackControl radius2Control;
        private System.Windows.Forms.Label label1;
        private Components.TrackControl radius0Control;
        private Components.TrackControl radius1Control;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Components.ComboBox nodeNameComboBox;
        private System.Windows.Forms.CheckBox relativeCheckBox;
        private System.Windows.Forms.Button resetButton;
        private Components.Vector3Control positionControl;
        private Components.Vector3Control rotationControl;
        private System.Windows.Forms.CheckBox inclusiveCheckBox;
        private System.Windows.Forms.Button nodeNameClearButton;
    }
}
