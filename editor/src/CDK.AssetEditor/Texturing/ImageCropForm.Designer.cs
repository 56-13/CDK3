namespace CDK.Assets.Texturing
{
    partial class ImageCropForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.screenControl = new CDK.Assets.Texturing.ImageCropScreenControl();
            this.doneButton = new System.Windows.Forms.Button();
            this.cropButton = new System.Windows.Forms.Button();
            this.pivotLeftButton = new System.Windows.Forms.Button();
            this.pivotRightButton = new System.Windows.Forms.Button();
            this.pivotUpButton = new System.Windows.Forms.Button();
            this.pivotDownButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.screenControl);
            this.splitContainer1.Size = new System.Drawing.Size(984, 677);
            this.splitContainer1.SplitterDistance = 292;
            this.splitContainer1.TabIndex = 0;
            // 
            // listView
            // 
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(292, 677);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListView_ItemChecked);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.SizeChanged += new System.EventHandler(this.ListView_SizeChanged);
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "";
            this.columnHeader.Width = 120;
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(688, 677);
            this.screenControl.TabIndex = 1;
            this.screenControl.Text = "rootImageAssetReplaceScreenControl1";
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.doneButton.Location = new System.Drawing.Point(921, 695);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 4;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // cropButton
            // 
            this.cropButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cropButton.Location = new System.Drawing.Point(840, 695);
            this.cropButton.Name = "cropButton";
            this.cropButton.Size = new System.Drawing.Size(75, 23);
            this.cropButton.TabIndex = 3;
            this.cropButton.Text = "Crop";
            this.cropButton.UseVisualStyleBackColor = true;
            this.cropButton.Click += new System.EventHandler(this.CropButton_Click);
            // 
            // pivotLeftButton
            // 
            this.pivotLeftButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pivotLeftButton.Location = new System.Drawing.Point(724, 695);
            this.pivotLeftButton.Name = "pivotLeftButton";
            this.pivotLeftButton.Size = new System.Drawing.Size(23, 23);
            this.pivotLeftButton.TabIndex = 5;
            this.pivotLeftButton.Text = "←";
            this.pivotLeftButton.UseVisualStyleBackColor = true;
            this.pivotLeftButton.Click += new System.EventHandler(this.PivotLeftButton_Click);
            // 
            // pivotRightButton
            // 
            this.pivotRightButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pivotRightButton.Location = new System.Drawing.Point(753, 695);
            this.pivotRightButton.Name = "pivotRightButton";
            this.pivotRightButton.Size = new System.Drawing.Size(23, 23);
            this.pivotRightButton.TabIndex = 6;
            this.pivotRightButton.Text = "→";
            this.pivotRightButton.UseVisualStyleBackColor = true;
            this.pivotRightButton.Click += new System.EventHandler(this.PivotRightButton_Click);
            // 
            // pivotUpButton
            // 
            this.pivotUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pivotUpButton.Location = new System.Drawing.Point(782, 695);
            this.pivotUpButton.Name = "pivotUpButton";
            this.pivotUpButton.Size = new System.Drawing.Size(23, 23);
            this.pivotUpButton.TabIndex = 7;
            this.pivotUpButton.Text = "↑";
            this.pivotUpButton.UseVisualStyleBackColor = true;
            this.pivotUpButton.Click += new System.EventHandler(this.PivotUpButton_Click);
            // 
            // pivotDownButton
            // 
            this.pivotDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pivotDownButton.Location = new System.Drawing.Point(811, 695);
            this.pivotDownButton.Name = "pivotDownButton";
            this.pivotDownButton.Size = new System.Drawing.Size(23, 23);
            this.pivotDownButton.TabIndex = 8;
            this.pivotDownButton.Text = "↓";
            this.pivotDownButton.UseVisualStyleBackColor = true;
            this.pivotDownButton.Click += new System.EventHandler(this.PivotDownButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(686, 700);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Pivot";
            // 
            // selectAllCheckBox
            // 
            this.selectAllCheckBox.AutoCheck = false;
            this.selectAllCheckBox.AutoSize = true;
            this.selectAllCheckBox.Location = new System.Drawing.Point(12, 700);
            this.selectAllCheckBox.Name = "selectAllCheckBox";
            this.selectAllCheckBox.Size = new System.Drawing.Size(78, 16);
            this.selectAllCheckBox.TabIndex = 10;
            this.selectAllCheckBox.Text = "Check All";
            this.selectAllCheckBox.ThreeState = true;
            this.selectAllCheckBox.UseVisualStyleBackColor = true;
            this.selectAllCheckBox.Click += new System.EventHandler(this.SelectAllCheckBox_Click);
            // 
            // ImageCropForm
            // 
            this.AcceptButton = this.cropButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.doneButton;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.selectAllCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pivotDownButton);
            this.Controls.Add(this.pivotUpButton);
            this.Controls.Add(this.pivotRightButton);
            this.Controls.Add(this.pivotLeftButton);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.cropButton);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Name = "ImageCropForm";
            this.Text = "Crop";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ImageCropScreenControl screenControl;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Button cropButton;
        private System.Windows.Forms.Button pivotLeftButton;
        private System.Windows.Forms.Button pivotRightButton;
        private System.Windows.Forms.Button pivotUpButton;
        private System.Windows.Forms.Button pivotDownButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.CheckBox selectAllCheckBox;
    }
}