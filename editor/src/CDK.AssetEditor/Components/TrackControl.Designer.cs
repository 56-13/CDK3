namespace CDK.Assets.Components
{
    partial class TrackControl
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.valueUpDown = new CDK.Assets.Components.NumericUpDown();
            this.valueTrackBar = new CDK.Assets.Components.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.valueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // valueUpDown
            // 
            this.valueUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.valueUpDown.Location = new System.Drawing.Point(180, 0);
            this.valueUpDown.Name = "valueUpDown";
            this.valueUpDown.Size = new System.Drawing.Size(60, 21);
            this.valueUpDown.TabIndex = 0;
            this.valueUpDown.ValueChanged += new System.EventHandler(this.ValueUpDown_ValueChanged);
            // 
            // valueTrackBar
            // 
            this.valueTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTrackBar.Location = new System.Drawing.Point(0, 0);
            this.valueTrackBar.Maximum = 100;
            this.valueTrackBar.Minimum = 0;
            this.valueTrackBar.Name = "valueTrackBar";
            this.valueTrackBar.Size = new System.Drawing.Size(174, 21);
            this.valueTrackBar.TabIndex = 7;
            this.valueTrackBar.Value = 0;
            // 
            // ScalarTrackControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.valueTrackBar);
            this.Controls.Add(this.valueUpDown);
            this.Name = "ScalarTrackControl";
            this.Size = new System.Drawing.Size(240, 21);
            ((System.ComponentModel.ISupportInitialize)(this.valueUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Components.NumericUpDown valueUpDown;
        private TrackBar valueTrackBar;
    }
}
