
namespace CDK.Assets.Terrain
{
    partial class TerrainAssetControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.gridUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.vertexCellUpDown = new CDK.Assets.Components.NumericUpDown();
            this.surfaceCellUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.ambientOcclusionIntensityControl = new CDK.Assets.Components.TrackControl();
            this.resizeApplyButton = new System.Windows.Forms.Button();
            this.resizeCancelButton = new System.Windows.Forms.Button();
            this.resizeAlignComboBox = new CDK.Assets.Components.ComboBox();
            this.altitudeUpDown = new CDK.Assets.Components.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexCellUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.surfaceCellUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.altitudeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width / Height";
            // 
            // widthUpDown
            // 
            this.widthUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.widthUpDown.Location = new System.Drawing.Point(194, 0);
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
            this.widthUpDown.ValueChanged += new System.EventHandler(this.SizeUpDown_ValueChanged);
            // 
            // heightUpDown
            // 
            this.heightUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.heightUpDown.Location = new System.Drawing.Point(260, 0);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Altitude / Grid";
            // 
            // gridUpDown
            // 
            this.gridUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gridUpDown.Location = new System.Drawing.Point(260, 82);
            this.gridUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridUpDown.Name = "gridUpDown";
            this.gridUpDown.Size = new System.Drawing.Size(60, 21);
            this.gridUpDown.TabIndex = 4;
            this.gridUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Vertex / Surface Div";
            // 
            // vertexCellUpDown
            // 
            this.vertexCellUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vertexCellUpDown.Location = new System.Drawing.Point(194, 27);
            this.vertexCellUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vertexCellUpDown.Name = "vertexCellUpDown";
            this.vertexCellUpDown.Size = new System.Drawing.Size(60, 21);
            this.vertexCellUpDown.TabIndex = 6;
            this.vertexCellUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.vertexCellUpDown.ValueChanged += new System.EventHandler(this.SizeUpDown_ValueChanged);
            // 
            // surfaceCellUpDown
            // 
            this.surfaceCellUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.surfaceCellUpDown.Location = new System.Drawing.Point(260, 27);
            this.surfaceCellUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.surfaceCellUpDown.Name = "surfaceCellUpDown";
            this.surfaceCellUpDown.Size = new System.Drawing.Size(60, 21);
            this.surfaceCellUpDown.TabIndex = 8;
            this.surfaceCellUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.surfaceCellUpDown.ValueChanged += new System.EventHandler(this.SizeUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ambient Occlusion";
            // 
            // ambientOcclusionIntensityControl
            // 
            this.ambientOcclusionIntensityControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ambientOcclusionIntensityControl.DecimalPlaces = 2;
            this.ambientOcclusionIntensityControl.Increment = 0.1F;
            this.ambientOcclusionIntensityControl.Location = new System.Drawing.Point(124, 109);
            this.ambientOcclusionIntensityControl.Maximum = 10F;
            this.ambientOcclusionIntensityControl.Minimum = 1F;
            this.ambientOcclusionIntensityControl.Name = "ambientOcclusionIntensityControl";
            this.ambientOcclusionIntensityControl.Size = new System.Drawing.Size(196, 21);
            this.ambientOcclusionIntensityControl.TabIndex = 13;
            this.ambientOcclusionIntensityControl.Value = 1F;
            // 
            // resizeApplyButton
            // 
            this.resizeApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeApplyButton.Location = new System.Drawing.Point(260, 54);
            this.resizeApplyButton.Name = "resizeApplyButton";
            this.resizeApplyButton.Size = new System.Drawing.Size(60, 22);
            this.resizeApplyButton.TabIndex = 14;
            this.resizeApplyButton.Text = "Apply";
            this.resizeApplyButton.UseVisualStyleBackColor = true;
            this.resizeApplyButton.Visible = false;
            this.resizeApplyButton.Click += new System.EventHandler(this.ResizeApplyButton_Click);
            // 
            // resizeCancelButton
            // 
            this.resizeCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeCancelButton.Location = new System.Drawing.Point(194, 54);
            this.resizeCancelButton.Name = "resizeCancelButton";
            this.resizeCancelButton.Size = new System.Drawing.Size(60, 22);
            this.resizeCancelButton.TabIndex = 15;
            this.resizeCancelButton.Text = "Cancel";
            this.resizeCancelButton.UseVisualStyleBackColor = true;
            this.resizeCancelButton.Visible = false;
            this.resizeCancelButton.Click += new System.EventHandler(this.ResizeCancelButton_Click);
            // 
            // resizeAlignComboBox
            // 
            this.resizeAlignComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resizeAlignComboBox.FormattingEnabled = true;
            this.resizeAlignComboBox.Location = new System.Drawing.Point(88, 55);
            this.resizeAlignComboBox.Name = "resizeAlignComboBox";
            this.resizeAlignComboBox.Size = new System.Drawing.Size(100, 20);
            this.resizeAlignComboBox.TabIndex = 16;
            this.resizeAlignComboBox.Visible = false;
            // 
            // altitudeUpDown
            // 
            this.altitudeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.altitudeUpDown.Location = new System.Drawing.Point(194, 82);
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
            this.altitudeUpDown.TabIndex = 17;
            this.altitudeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TerrainAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.altitudeUpDown);
            this.Controls.Add(this.resizeAlignComboBox);
            this.Controls.Add(this.resizeCancelButton);
            this.Controls.Add(this.resizeApplyButton);
            this.Controls.Add(this.ambientOcclusionIntensityControl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.surfaceCellUpDown);
            this.Controls.Add(this.vertexCellUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gridUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.heightUpDown);
            this.Controls.Add(this.widthUpDown);
            this.Controls.Add(this.label1);
            this.Name = "TerrainAssetControl";
            this.Size = new System.Drawing.Size(320, 130);
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexCellUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.surfaceCellUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.altitudeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Components.NumericUpDown widthUpDown;
        private Components.NumericUpDown heightUpDown;
        private System.Windows.Forms.Label label2;
        private Components.NumericUpDown gridUpDown;
        private System.Windows.Forms.Label label3;
        private Components.NumericUpDown vertexCellUpDown;
        private Components.NumericUpDown surfaceCellUpDown;
        private System.Windows.Forms.Label label5;
        private Components.TrackControl ambientOcclusionIntensityControl;
        private System.Windows.Forms.Button resizeApplyButton;
        private System.Windows.Forms.Button resizeCancelButton;
        private Components.ComboBox resizeAlignComboBox;
        private Components.NumericUpDown altitudeUpDown;
    }
}
