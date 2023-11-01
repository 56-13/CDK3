namespace CDK.Assets.Texturing
{
    partial class ImageResizeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.widthUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.heightUpDown = new CDK.Assets.Components.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.rateUpDown = new CDK.Assets.Components.NumericUpDown();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.stretchCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rateUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // widthUpDown
            // 
            this.widthUpDown.Location = new System.Drawing.Point(120, 34);
            this.widthUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.widthUpDown.Name = "widthUpDown";
            this.widthUpDown.ReadOnly = true;
            this.widthUpDown.Size = new System.Drawing.Size(60, 21);
            this.widthUpDown.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Height";
            // 
            // heightUpDown
            // 
            this.heightUpDown.Location = new System.Drawing.Point(120, 61);
            this.heightUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.heightUpDown.Name = "heightUpDown";
            this.heightUpDown.ReadOnly = true;
            this.heightUpDown.Size = new System.Drawing.Size(60, 21);
            this.heightUpDown.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Rate";
            // 
            // rateUpDown
            // 
            this.rateUpDown.Location = new System.Drawing.Point(120, 88);
            this.rateUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.rateUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rateUpDown.Name = "rateUpDown";
            this.rateUpDown.Size = new System.Drawing.Size(60, 21);
            this.rateUpDown.TabIndex = 5;
            this.rateUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.rateUpDown.ValueChanged += new System.EventHandler(this.RateUpDown_ValueChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(12, 12);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 16);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.Text = "Size";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.RadioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(66, 12);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(48, 16);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Rate";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(105, 115);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(24, 115);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // stretchCheckBox
            // 
            this.stretchCheckBox.AutoSize = true;
            this.stretchCheckBox.Location = new System.Drawing.Point(120, 13);
            this.stretchCheckBox.Name = "stretchCheckBox";
            this.stretchCheckBox.Size = new System.Drawing.Size(63, 16);
            this.stretchCheckBox.TabIndex = 10;
            this.stretchCheckBox.Text = "Stretch";
            this.stretchCheckBox.UseVisualStyleBackColor = true;
            // 
            // ImageResizeForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(192, 149);
            this.Controls.Add(this.stretchCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.rateUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.heightUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.widthUpDown);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageResizeForm";
            this.Text = "Resize";
            ((System.ComponentModel.ISupportInitialize)(this.widthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rateUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.NumericUpDown widthUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Components.NumericUpDown heightUpDown;
        private System.Windows.Forms.Label label3;
        private Components.NumericUpDown rateUpDown;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox stretchCheckBox;
    }
}