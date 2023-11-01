
namespace CDK.Assets.Meshing
{
    partial class MeshAnimationControl
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
            this.durationTextBox = new System.Windows.Forms.TextBox();
            this.playCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Duration";
            // 
            // durationTextBox
            // 
            this.durationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.durationTextBox.Location = new System.Drawing.Point(57, 0);
            this.durationTextBox.Name = "durationTextBox";
            this.durationTextBox.ReadOnly = true;
            this.durationTextBox.Size = new System.Drawing.Size(208, 21);
            this.durationTextBox.TabIndex = 1;
            // 
            // playCheckBox
            // 
            this.playCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playCheckBox.AutoSize = true;
            this.playCheckBox.Location = new System.Drawing.Point(271, 3);
            this.playCheckBox.Name = "playCheckBox";
            this.playCheckBox.Size = new System.Drawing.Size(49, 16);
            this.playCheckBox.TabIndex = 2;
            this.playCheckBox.Text = "Play";
            this.playCheckBox.UseVisualStyleBackColor = true;
            this.playCheckBox.CheckedChanged += new System.EventHandler(this.PlayCheckBox_CheckedChanged);
            // 
            // MeshAnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playCheckBox);
            this.Controls.Add(this.durationTextBox);
            this.Controls.Add(this.label1);
            this.Name = "MeshAnimationControl";
            this.Size = new System.Drawing.Size(320, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox durationTextBox;
        private System.Windows.Forms.CheckBox playCheckBox;
    }
}
