namespace CDK.Assets
{
    partial class LocaleStringExportForm
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
            this.timestampPicker = new System.Windows.Forms.DateTimePicker();
            this.timestampCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.keyComboBox = new CDK.Assets.Components.ComboBox();
            this.localeCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.localeAllCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // timestampPicker
            // 
            this.timestampPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timestampPicker.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            this.timestampPicker.Enabled = false;
            this.timestampPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.timestampPicker.Location = new System.Drawing.Point(71, 12);
            this.timestampPicker.Name = "timestampPicker";
            this.timestampPicker.ShowUpDown = true;
            this.timestampPicker.Size = new System.Drawing.Size(151, 21);
            this.timestampPicker.TabIndex = 0;
            // 
            // timestampCheckBox
            // 
            this.timestampCheckBox.AutoSize = true;
            this.timestampCheckBox.Location = new System.Drawing.Point(12, 15);
            this.timestampCheckBox.Name = "timestampCheckBox";
            this.timestampCheckBox.Size = new System.Drawing.Size(53, 16);
            this.timestampCheckBox.TabIndex = 1;
            this.timestampCheckBox.Text = "From";
            this.timestampCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(147, 202);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(66, 202);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // keyComboBox
            // 
            this.keyComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.keyComboBox.FormattingEnabled = true;
            this.keyComboBox.Location = new System.Drawing.Point(71, 39);
            this.keyComboBox.Name = "keyComboBox";
            this.keyComboBox.Size = new System.Drawing.Size(151, 20);
            this.keyComboBox.TabIndex = 11;
            // 
            // localeCheckedListBox
            // 
            this.localeCheckedListBox.CheckOnClick = true;
            this.localeCheckedListBox.FormattingEnabled = true;
            this.localeCheckedListBox.IntegralHeight = false;
            this.localeCheckedListBox.Location = new System.Drawing.Point(12, 65);
            this.localeCheckedListBox.Name = "localeCheckedListBox";
            this.localeCheckedListBox.Size = new System.Drawing.Size(210, 131);
            this.localeCheckedListBox.TabIndex = 12;
            this.localeCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LocaleCheckedListBox_ItemCheck);
            // 
            // localeAllCheckBox
            // 
            this.localeAllCheckBox.AutoSize = true;
            this.localeAllCheckBox.Checked = true;
            this.localeAllCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localeAllCheckBox.Location = new System.Drawing.Point(12, 43);
            this.localeAllCheckBox.Name = "localeAllCheckBox";
            this.localeAllCheckBox.Size = new System.Drawing.Size(38, 16);
            this.localeAllCheckBox.TabIndex = 13;
            this.localeAllCheckBox.Text = "All";
            this.localeAllCheckBox.ThreeState = true;
            this.localeAllCheckBox.UseVisualStyleBackColor = true;
            this.localeAllCheckBox.CheckedChanged += new System.EventHandler(this.LocaleAllCheckBox_CheckedChanged);
            // 
            // LocaleStringExportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(234, 237);
            this.Controls.Add(this.localeAllCheckBox);
            this.Controls.Add(this.localeCheckedListBox);
            this.Controls.Add(this.keyComboBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.timestampCheckBox);
            this.Controls.Add(this.timestampPicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocaleStringExportForm";
            this.Text = "Locale Export";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker timestampPicker;
        private System.Windows.Forms.CheckBox timestampCheckBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private Components.ComboBox keyComboBox;
        private System.Windows.Forms.CheckedListBox localeCheckedListBox;
        private System.Windows.Forms.CheckBox localeAllCheckBox;
    }
}