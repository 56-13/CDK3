namespace CDK.Assets.Texturing
{
    partial class ImageReplaceForm
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.srcListView = new System.Windows.Forms.ListView();
            this.srcColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.destListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenControl = new CDK.Assets.Texturing.ImageReplaceScreenControl();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.doneButton = new System.Windows.Forms.Button();
            this.replaceButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.screenControl);
            this.splitContainer1.Size = new System.Drawing.Size(984, 677);
            this.splitContainer1.SplitterDistance = 492;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.srcListView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.destListBox);
            this.splitContainer2.Size = new System.Drawing.Size(492, 677);
            this.splitContainer2.SplitterDistance = 247;
            this.splitContainer2.TabIndex = 0;
            // 
            // srcListView
            // 
            this.srcListView.CheckBoxes = true;
            this.srcListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.srcColumnHeader});
            this.srcListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.srcListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.srcListView.HideSelection = false;
            this.srcListView.Location = new System.Drawing.Point(0, 0);
            this.srcListView.Name = "srcListView";
            this.srcListView.Size = new System.Drawing.Size(247, 677);
            this.srcListView.TabIndex = 0;
            this.srcListView.UseCompatibleStateImageBehavior = false;
            this.srcListView.View = System.Windows.Forms.View.Details;
            this.srcListView.SelectedIndexChanged += new System.EventHandler(this.SrcListView_SelectedIndexChanged);
            this.srcListView.SizeChanged += new System.EventHandler(this.SrcListView_SizeChanged);
            // 
            // srcColumnHeader
            // 
            this.srcColumnHeader.Text = "";
            this.srcColumnHeader.Width = 120;
            // 
            // destListBox
            // 
            this.destListBox.ContextMenuStrip = this.contextMenuStrip;
            this.destListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.destListBox.FormattingEnabled = true;
            this.destListBox.IntegralHeight = false;
            this.destListBox.ItemHeight = 12;
            this.destListBox.Location = new System.Drawing.Point(0, 0);
            this.destListBox.Name = "destListBox";
            this.destListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.destListBox.Size = new System.Drawing.Size(241, 677);
            this.destListBox.TabIndex = 0;
            this.destListBox.SelectedIndexChanged += new System.EventHandler(this.DestListBox_SelectedIndexChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upToolStripMenuItem,
            this.downToolStripMenuItem,
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(173, 92);
            // 
            // upToolStripMenuItem
            // 
            this.upToolStripMenuItem.Name = "upToolStripMenuItem";
            this.upToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.upToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.upToolStripMenuItem.Text = "Up";
            this.upToolStripMenuItem.Click += new System.EventHandler(this.UpToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.downToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.downToolStripMenuItem.Text = "Down";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.DownToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(488, 677);
            this.screenControl.TabIndex = 1;
            this.screenControl.Text = "imageReplaceScreenControl1";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
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
            // replaceButton
            // 
            this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceButton.Location = new System.Drawing.Point(840, 695);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(75, 23);
            this.replaceButton.TabIndex = 3;
            this.replaceButton.Text = "Replace";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // ImageReplaceForm
            // 
            this.AcceptButton = this.replaceButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.doneButton;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Name = "ImageReplaceForm";
            this.Text = "Replace";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox destListBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private ImageReplaceScreenControl screenControl;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.ListView srcListView;
        private System.Windows.Forms.ColumnHeader srcColumnHeader;
    }
}