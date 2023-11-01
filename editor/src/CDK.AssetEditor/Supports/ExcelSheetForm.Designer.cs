namespace CDK.Assets.Support
{
    partial class ExcelSheetForm
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
            this.sheetComboBox = new CDK.Assets.Components.ComboBox();
            this.sheetTextBox = new System.Windows.Forms.TextBox();
            this.addSheetButton = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.removeSheetButton = new System.Windows.Forms.Button();
            this.renameSheetButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // sheetComboBox
            // 
            this.sheetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sheetComboBox.FormattingEnabled = true;
            this.sheetComboBox.Location = new System.Drawing.Point(12, 528);
            this.sheetComboBox.Name = "sheetComboBox";
            this.sheetComboBox.Size = new System.Drawing.Size(160, 20);
            this.sheetComboBox.TabIndex = 0;
            this.sheetComboBox.SelectedIndexChanged += new System.EventHandler(this.SheetComboBox_SelectedIndexChanged);
            // 
            // sheetTextBox
            // 
            this.sheetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sheetTextBox.Location = new System.Drawing.Point(178, 528);
            this.sheetTextBox.Name = "sheetTextBox";
            this.sheetTextBox.Size = new System.Drawing.Size(160, 21);
            this.sheetTextBox.TabIndex = 1;
            // 
            // addSheetButton
            // 
            this.addSheetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addSheetButton.Location = new System.Drawing.Point(425, 527);
            this.addSheetButton.Name = "addSheetButton";
            this.addSheetButton.Size = new System.Drawing.Size(75, 23);
            this.addSheetButton.TabIndex = 2;
            this.addSheetButton.Text = "Add";
            this.addSheetButton.UseVisualStyleBackColor = true;
            this.addSheetButton.Click += new System.EventHandler(this.AddSheetButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(960, 508);
            this.dataGridView.TabIndex = 3;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(816, 527);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(897, 526);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // removeSheetButton
            // 
            this.removeSheetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeSheetButton.Location = new System.Drawing.Point(506, 527);
            this.removeSheetButton.Name = "removeSheetButton";
            this.removeSheetButton.Size = new System.Drawing.Size(75, 23);
            this.removeSheetButton.TabIndex = 6;
            this.removeSheetButton.Text = "Remove";
            this.removeSheetButton.UseVisualStyleBackColor = true;
            this.removeSheetButton.Click += new System.EventHandler(this.RemoveSheetButton_Click);
            // 
            // renameSheetButton
            // 
            this.renameSheetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.renameSheetButton.Location = new System.Drawing.Point(344, 527);
            this.renameSheetButton.Name = "renameSheetButton";
            this.renameSheetButton.Size = new System.Drawing.Size(75, 23);
            this.renameSheetButton.TabIndex = 7;
            this.renameSheetButton.Text = "Rename";
            this.renameSheetButton.UseVisualStyleBackColor = true;
            this.renameSheetButton.Click += new System.EventHandler(this.RenameSheetButton_Click);
            // 
            // ExcelSheetForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.renameSheetButton);
            this.Controls.Add(this.removeSheetButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.addSheetButton);
            this.Controls.Add(this.sheetTextBox);
            this.Controls.Add(this.sheetComboBox);
            this.KeyPreview = true;
            this.Name = "ExcelSheetForm";
            this.Text = "Work Sheet";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.ComboBox sheetComboBox;
        private System.Windows.Forms.TextBox sheetTextBox;
        private System.Windows.Forms.Button addSheetButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button removeSheetButton;
        private System.Windows.Forms.Button renameSheetButton;
    }
}