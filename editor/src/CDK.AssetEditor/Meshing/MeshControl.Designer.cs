
namespace CDK.Assets.Meshing
{
    partial class MeshControl
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
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flipUVCheckBox = new System.Windows.Forms.CheckBox();
            this.axisComboBox = new CDK.Assets.Components.ComboBox();
            this.scaleControl = new CDK.Assets.Components.TrackControl();
            this.selectionControl = new CDK.Assets.Meshing.MeshSelectionControl();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Scale";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Axis";
            // 
            // flipUVCheckBox
            // 
            this.flipUVCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flipUVCheckBox.AutoSize = true;
            this.flipUVCheckBox.Location = new System.Drawing.Point(256, 30);
            this.flipUVCheckBox.Name = "flipUVCheckBox";
            this.flipUVCheckBox.Size = new System.Drawing.Size(64, 16);
            this.flipUVCheckBox.TabIndex = 18;
            this.flipUVCheckBox.Text = "Flip UV";
            this.flipUVCheckBox.UseVisualStyleBackColor = true;
            // 
            // axisComboBox
            // 
            this.axisComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axisComboBox.FormattingEnabled = true;
            this.axisComboBox.Location = new System.Drawing.Point(90, 27);
            this.axisComboBox.Name = "axisComboBox";
            this.axisComboBox.Size = new System.Drawing.Size(160, 20);
            this.axisComboBox.TabIndex = 9;
            // 
            // scaleControl
            // 
            this.scaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleControl.DecimalPlaces = 2;
            this.scaleControl.Increment = 0.01F;
            this.scaleControl.Location = new System.Drawing.Point(90, 0);
            this.scaleControl.Maximum = 100F;
            this.scaleControl.Minimum = 0.01F;
            this.scaleControl.Name = "scaleControl";
            this.scaleControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.scaleControl.Size = new System.Drawing.Size(230, 21);
            this.scaleControl.TabIndex = 7;
            this.scaleControl.Value = 1F;
            // 
            // selectionControl
            // 
            this.selectionControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionControl.Location = new System.Drawing.Point(0, 55);
            this.selectionControl.Name = "selectionControl";
            this.selectionControl.Size = new System.Drawing.Size(320, 128);
            this.selectionControl.TabIndex = 19;
            this.selectionControl.SizeChanged += new System.EventHandler(this.SelectionControl_SizeChanged);
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.selectionControl);
            this.Controls.Add(this.flipUVCheckBox);
            this.Controls.Add(this.axisComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scaleControl);
            this.Controls.Add(this.label3);
            this.Name = "MeshControl";
            this.Size = new System.Drawing.Size(320, 183);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.TrackControl scaleControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private Components.ComboBox axisComboBox;
        private System.Windows.Forms.CheckBox flipUVCheckBox;
        private MeshSelectionControl selectionControl;
    }
}
