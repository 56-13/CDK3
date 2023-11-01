
namespace CDK.Assets.Texturing
{
    partial class MaterialAssetMaterialImportForm
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
            this.metallicPathTextBox = new System.Windows.Forms.TextBox();
            this.metallicImportButton = new System.Windows.Forms.Button();
            this.metallicClearButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.roughnessClearButton = new System.Windows.Forms.Button();
            this.roughnessImportButton = new System.Windows.Forms.Button();
            this.roughnessPathTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.occlusionClearButton = new System.Windows.Forms.Button();
            this.occlusionImportButton = new System.Windows.Forms.Button();
            this.occlusionPathTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // metallicPathTextBox
            // 
            this.metallicPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metallicPathTextBox.Location = new System.Drawing.Point(100, 12);
            this.metallicPathTextBox.Name = "metallicPathTextBox";
            this.metallicPathTextBox.ReadOnly = true;
            this.metallicPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.metallicPathTextBox.TabIndex = 0;
            // 
            // metallicImportButton
            // 
            this.metallicImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metallicImportButton.Location = new System.Drawing.Point(402, 12);
            this.metallicImportButton.Name = "metallicImportButton";
            this.metallicImportButton.Size = new System.Drawing.Size(23, 23);
            this.metallicImportButton.TabIndex = 8;
            this.metallicImportButton.Text = "+";
            this.metallicImportButton.UseVisualStyleBackColor = true;
            this.metallicImportButton.Click += new System.EventHandler(this.MetallicImportButton_Click);
            // 
            // metallicClearButton
            // 
            this.metallicClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.metallicClearButton.Location = new System.Drawing.Point(429, 12);
            this.metallicClearButton.Name = "metallicClearButton";
            this.metallicClearButton.Size = new System.Drawing.Size(23, 23);
            this.metallicClearButton.TabIndex = 12;
            this.metallicClearButton.Text = "-";
            this.metallicClearButton.UseVisualStyleBackColor = true;
            this.metallicClearButton.Click += new System.EventHandler(this.MetallicClearButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Metallic";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Roughness";
            // 
            // roughnessClearButton
            // 
            this.roughnessClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.roughnessClearButton.Location = new System.Drawing.Point(429, 39);
            this.roughnessClearButton.Name = "roughnessClearButton";
            this.roughnessClearButton.Size = new System.Drawing.Size(23, 23);
            this.roughnessClearButton.TabIndex = 17;
            this.roughnessClearButton.Text = "-";
            this.roughnessClearButton.UseVisualStyleBackColor = true;
            this.roughnessClearButton.Click += new System.EventHandler(this.RoughnessClearButton_Click);
            // 
            // roughnessImportButton
            // 
            this.roughnessImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.roughnessImportButton.Location = new System.Drawing.Point(402, 39);
            this.roughnessImportButton.Name = "roughnessImportButton";
            this.roughnessImportButton.Size = new System.Drawing.Size(23, 23);
            this.roughnessImportButton.TabIndex = 16;
            this.roughnessImportButton.Text = "+";
            this.roughnessImportButton.UseVisualStyleBackColor = true;
            this.roughnessImportButton.Click += new System.EventHandler(this.RoughnessImportButton_Click);
            // 
            // roughnessPathTextBox
            // 
            this.roughnessPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roughnessPathTextBox.Location = new System.Drawing.Point(100, 39);
            this.roughnessPathTextBox.Name = "roughnessPathTextBox";
            this.roughnessPathTextBox.ReadOnly = true;
            this.roughnessPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.roughnessPathTextBox.TabIndex = 15;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(377, 98);
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
            this.okButton.Location = new System.Drawing.Point(296, 98);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 18;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // occlusionClearButton
            // 
            this.occlusionClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.occlusionClearButton.Location = new System.Drawing.Point(429, 66);
            this.occlusionClearButton.Name = "occlusionClearButton";
            this.occlusionClearButton.Size = new System.Drawing.Size(23, 23);
            this.occlusionClearButton.TabIndex = 23;
            this.occlusionClearButton.Text = "-";
            this.occlusionClearButton.UseVisualStyleBackColor = true;
            this.occlusionClearButton.Click += new System.EventHandler(this.OcclusionClearButton_Click);
            // 
            // occlusionImportButton
            // 
            this.occlusionImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.occlusionImportButton.Location = new System.Drawing.Point(402, 66);
            this.occlusionImportButton.Name = "occlusionImportButton";
            this.occlusionImportButton.Size = new System.Drawing.Size(23, 23);
            this.occlusionImportButton.TabIndex = 22;
            this.occlusionImportButton.Text = "+";
            this.occlusionImportButton.UseVisualStyleBackColor = true;
            this.occlusionImportButton.Click += new System.EventHandler(this.OcclusionImportButton_Click);
            // 
            // occlusionPathTextBox
            // 
            this.occlusionPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.occlusionPathTextBox.Location = new System.Drawing.Point(100, 66);
            this.occlusionPathTextBox.Name = "occlusionPathTextBox";
            this.occlusionPathTextBox.ReadOnly = true;
            this.occlusionPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.occlusionPathTextBox.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "AO";
            // 
            // TextureAssetMaterialImportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(464, 133);
            this.Controls.Add(this.occlusionClearButton);
            this.Controls.Add(this.occlusionImportButton);
            this.Controls.Add(this.occlusionPathTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.roughnessClearButton);
            this.Controls.Add(this.roughnessImportButton);
            this.Controls.Add(this.roughnessPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.metallicClearButton);
            this.Controls.Add(this.metallicImportButton);
            this.Controls.Add(this.metallicPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureAssetMaterialImportForm";
            this.Text = "Import Normal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox metallicPathTextBox;
        private System.Windows.Forms.Button metallicImportButton;
        private System.Windows.Forms.Button metallicClearButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button roughnessClearButton;
        private System.Windows.Forms.Button roughnessImportButton;
        private System.Windows.Forms.TextBox roughnessPathTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button occlusionClearButton;
        private System.Windows.Forms.Button occlusionImportButton;
        private System.Windows.Forms.TextBox occlusionPathTextBox;
        private System.Windows.Forms.Label label3;
    }
}