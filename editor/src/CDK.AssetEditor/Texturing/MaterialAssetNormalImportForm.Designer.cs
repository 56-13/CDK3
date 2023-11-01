
namespace CDK.Assets.Texturing
{
    partial class MaterialAssetNormalImportForm
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
            this.normalPathTextBox = new System.Windows.Forms.TextBox();
            this.normalImportButton = new System.Windows.Forms.Button();
            this.normalClearButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.displacementClearButton = new System.Windows.Forms.Button();
            this.displacementImportButton = new System.Windows.Forms.Button();
            this.displacementPathTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // normalPathTextBox
            // 
            this.normalPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.normalPathTextBox.Location = new System.Drawing.Point(100, 13);
            this.normalPathTextBox.Name = "normalPathTextBox";
            this.normalPathTextBox.ReadOnly = true;
            this.normalPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.normalPathTextBox.TabIndex = 0;
            // 
            // normalImportButton
            // 
            this.normalImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.normalImportButton.Location = new System.Drawing.Point(402, 12);
            this.normalImportButton.Name = "normalImportButton";
            this.normalImportButton.Size = new System.Drawing.Size(23, 23);
            this.normalImportButton.TabIndex = 8;
            this.normalImportButton.Text = "+";
            this.normalImportButton.UseVisualStyleBackColor = true;
            this.normalImportButton.Click += new System.EventHandler(this.NormalImportButton_Click);
            // 
            // normalClearButton
            // 
            this.normalClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.normalClearButton.Location = new System.Drawing.Point(429, 12);
            this.normalClearButton.Name = "normalClearButton";
            this.normalClearButton.Size = new System.Drawing.Size(23, 23);
            this.normalClearButton.TabIndex = 12;
            this.normalClearButton.Text = "-";
            this.normalClearButton.UseVisualStyleBackColor = true;
            this.normalClearButton.Click += new System.EventHandler(this.NormalClearButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Normal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Displacement";
            // 
            // displacementClearButton
            // 
            this.displacementClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.displacementClearButton.Location = new System.Drawing.Point(429, 39);
            this.displacementClearButton.Name = "displacementClearButton";
            this.displacementClearButton.Size = new System.Drawing.Size(23, 23);
            this.displacementClearButton.TabIndex = 17;
            this.displacementClearButton.Text = "-";
            this.displacementClearButton.UseVisualStyleBackColor = true;
            this.displacementClearButton.Click += new System.EventHandler(this.DisplacementClearButton_Click);
            // 
            // displacementImportButton
            // 
            this.displacementImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.displacementImportButton.Location = new System.Drawing.Point(402, 39);
            this.displacementImportButton.Name = "displacementImportButton";
            this.displacementImportButton.Size = new System.Drawing.Size(23, 23);
            this.displacementImportButton.TabIndex = 16;
            this.displacementImportButton.Text = "+";
            this.displacementImportButton.UseVisualStyleBackColor = true;
            this.displacementImportButton.Click += new System.EventHandler(this.DisplacementImportButton_Click);
            // 
            // displacementPathTextBox
            // 
            this.displacementPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displacementPathTextBox.Location = new System.Drawing.Point(100, 40);
            this.displacementPathTextBox.Name = "displacementPathTextBox";
            this.displacementPathTextBox.ReadOnly = true;
            this.displacementPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.displacementPathTextBox.TabIndex = 15;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(377, 68);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 19;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(296, 68);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // TextureAssetNormalImportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(464, 103);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.displacementClearButton);
            this.Controls.Add(this.displacementImportButton);
            this.Controls.Add(this.displacementPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.normalClearButton);
            this.Controls.Add(this.normalImportButton);
            this.Controls.Add(this.normalPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureAssetNormalImportForm";
            this.Text = "Import Normal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox normalPathTextBox;
        private System.Windows.Forms.Button normalImportButton;
        private System.Windows.Forms.Button normalClearButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button displacementClearButton;
        private System.Windows.Forms.Button displacementImportButton;
        private System.Windows.Forms.TextBox displacementPathTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}