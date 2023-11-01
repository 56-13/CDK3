
namespace CDK.Assets.Texturing
{
    partial class MaterialAssetColorImportForm
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
            this.colorPathTextBox = new System.Windows.Forms.TextBox();
            this.colorImportButton = new System.Windows.Forms.Button();
            this.colorClearButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.opacityClearButton = new System.Windows.Forms.Button();
            this.opacityImportButton = new System.Windows.Forms.Button();
            this.opacityPathTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // colorPathTextBox
            // 
            this.colorPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPathTextBox.Location = new System.Drawing.Point(100, 13);
            this.colorPathTextBox.Name = "colorPathTextBox";
            this.colorPathTextBox.ReadOnly = true;
            this.colorPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.colorPathTextBox.TabIndex = 0;
            // 
            // colorImportButton
            // 
            this.colorImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorImportButton.Location = new System.Drawing.Point(402, 12);
            this.colorImportButton.Name = "colorImportButton";
            this.colorImportButton.Size = new System.Drawing.Size(23, 23);
            this.colorImportButton.TabIndex = 8;
            this.colorImportButton.Text = "+";
            this.colorImportButton.UseVisualStyleBackColor = true;
            this.colorImportButton.Click += new System.EventHandler(this.ColorImportButton_Click);
            // 
            // colorClearButton
            // 
            this.colorClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.colorClearButton.Location = new System.Drawing.Point(429, 12);
            this.colorClearButton.Name = "colorClearButton";
            this.colorClearButton.Size = new System.Drawing.Size(23, 23);
            this.colorClearButton.TabIndex = 12;
            this.colorClearButton.Text = "-";
            this.colorClearButton.UseVisualStyleBackColor = true;
            this.colorClearButton.Click += new System.EventHandler(this.ColorClearButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "Opacity";
            // 
            // opacityClearButton
            // 
            this.opacityClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.opacityClearButton.Location = new System.Drawing.Point(429, 39);
            this.opacityClearButton.Name = "opacityClearButton";
            this.opacityClearButton.Size = new System.Drawing.Size(23, 23);
            this.opacityClearButton.TabIndex = 17;
            this.opacityClearButton.Text = "-";
            this.opacityClearButton.UseVisualStyleBackColor = true;
            this.opacityClearButton.Click += new System.EventHandler(this.OpacityClearButton_Click);
            // 
            // opacityImportButton
            // 
            this.opacityImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.opacityImportButton.Location = new System.Drawing.Point(402, 39);
            this.opacityImportButton.Name = "opacityImportButton";
            this.opacityImportButton.Size = new System.Drawing.Size(23, 23);
            this.opacityImportButton.TabIndex = 16;
            this.opacityImportButton.Text = "+";
            this.opacityImportButton.UseVisualStyleBackColor = true;
            this.opacityImportButton.Click += new System.EventHandler(this.OpacityImportButton_Click);
            // 
            // opacityPathTextBox
            // 
            this.opacityPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.opacityPathTextBox.Location = new System.Drawing.Point(100, 40);
            this.opacityPathTextBox.Name = "opacityPathTextBox";
            this.opacityPathTextBox.ReadOnly = true;
            this.opacityPathTextBox.Size = new System.Drawing.Size(296, 21);
            this.opacityPathTextBox.TabIndex = 15;
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
            // TextureAssetColorImportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(464, 103);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.opacityClearButton);
            this.Controls.Add(this.opacityImportButton);
            this.Controls.Add(this.opacityPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.colorClearButton);
            this.Controls.Add(this.colorImportButton);
            this.Controls.Add(this.colorPathTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureAssetColorImportForm";
            this.Text = "Import Color";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox colorPathTextBox;
        private System.Windows.Forms.Button colorImportButton;
        private System.Windows.Forms.Button colorClearButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button opacityClearButton;
        private System.Windows.Forms.Button opacityImportButton;
        private System.Windows.Forms.TextBox opacityPathTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}