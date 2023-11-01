namespace CDK.Assets.Containers
{
    partial class ContainerAssetControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.viewComboBox = new CDK.Assets.Components.ComboBox();
            this.importButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.childComboBox = new CDK.Assets.Components.ComboBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.rightJumpButton = new System.Windows.Forms.Button();
            this.leftJumpButton = new System.Windows.Forms.Button();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localeExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localeImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.localeImportFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.localeExportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.listView = new CDK.Assets.Components.DoubleBufferedListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewComboBox
            // 
            this.viewComboBox.FormattingEnabled = true;
            this.viewComboBox.Location = new System.Drawing.Point(512, 3);
            this.viewComboBox.Name = "viewComboBox";
            this.viewComboBox.Size = new System.Drawing.Size(112, 20);
            this.viewComboBox.TabIndex = 1;
            this.viewComboBox.SelectedIndexChanged += new System.EventHandler(this.ViewComboBox_SelectedIndexChanged);
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(156, 0);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(64, 25);
            this.importButton.TabIndex = 2;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(226, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(64, 25);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point(296, 0);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(48, 25);
            this.leftButton.TabIndex = 5;
            this.leftButton.Text = "<<";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.LeftButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point(350, 0);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(48, 25);
            this.rightButton.TabIndex = 6;
            this.rightButton.Text = ">>";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.RightButton_Click);
            // 
            // childComboBox
            // 
            this.childComboBox.FormattingEnabled = true;
            this.childComboBox.Location = new System.Drawing.Point(0, 3);
            this.childComboBox.Name = "childComboBox";
            this.childComboBox.Size = new System.Drawing.Size(150, 20);
            this.childComboBox.TabIndex = 7;
            this.childComboBox.SelectedIndexChanged += new System.EventHandler(this.ChildComboBox_SelectedIndexChanged);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // rightJumpButton
            // 
            this.rightJumpButton.Location = new System.Drawing.Point(458, 0);
            this.rightJumpButton.Name = "rightJumpButton";
            this.rightJumpButton.Size = new System.Drawing.Size(48, 25);
            this.rightJumpButton.TabIndex = 9;
            this.rightJumpButton.Text = ">>>";
            this.rightJumpButton.UseVisualStyleBackColor = true;
            this.rightJumpButton.Click += new System.EventHandler(this.RightJumpButton_Click);
            // 
            // leftJumpButton
            // 
            this.leftJumpButton.Location = new System.Drawing.Point(404, 0);
            this.leftJumpButton.Name = "leftJumpButton";
            this.leftJumpButton.Size = new System.Drawing.Size(48, 25);
            this.leftJumpButton.TabIndex = 8;
            this.leftJumpButton.Text = "<<<";
            this.leftJumpButton.UseVisualStyleBackColor = true;
            this.leftJumpButton.Click += new System.EventHandler(this.LeftJumpButton_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.tagToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.localeToolStripMenuItem,
            this.buildToolStripMenuItem,
            this.uploadToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(145, 224);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RenameToolStripMenuItem_Click);
            // 
            // tagToolStripMenuItem
            // 
            this.tagToolStripMenuItem.Name = "tagToolStripMenuItem";
            this.tagToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.tagToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.tagToolStripMenuItem.Text = "Tag";
            this.tagToolStripMenuItem.Click += new System.EventHandler(this.TagToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItem_Click);
            // 
            // localeToolStripMenuItem
            // 
            this.localeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localeExportToolStripMenuItem,
            this.localeImportToolStripMenuItem});
            this.localeToolStripMenuItem.Name = "localeToolStripMenuItem";
            this.localeToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.localeToolStripMenuItem.Text = "Locale";
            // 
            // localeExportToolStripMenuItem
            // 
            this.localeExportToolStripMenuItem.Name = "localeExportToolStripMenuItem";
            this.localeExportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.localeExportToolStripMenuItem.Text = "Export";
            this.localeExportToolStripMenuItem.Click += new System.EventHandler(this.LocaleExportToolStripMenuItem_Click);
            // 
            // localeImportToolStripMenuItem
            // 
            this.localeImportToolStripMenuItem.Name = "localeImportToolStripMenuItem";
            this.localeImportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.localeImportToolStripMenuItem.Text = "Import";
            this.localeImportToolStripMenuItem.Click += new System.EventHandler(this.LocaleImportToolStripMenuItem_Click);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.buildToolStripMenuItem.Text = "Build";
            this.buildToolStripMenuItem.Click += new System.EventHandler(this.BuildToolStripMenuItem_Click);
            // 
            // uploadToolStripMenuItem
            // 
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            this.uploadToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.uploadToolStripMenuItem.Text = "Upload";
            this.uploadToolStripMenuItem.Click += new System.EventHandler(this.UploadToolStripMenuItem_Click);
            // 
            // localeImportFileDialog
            // 
            this.localeImportFileDialog.RestoreDirectory = true;
            // 
            // localeExportFileDialog
            // 
            this.localeExportFileDialog.RestoreDirectory = true;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.typeColumnHeader,
            this.descriptionColumnHeader});
            this.listView.ContextMenuStrip = this.contextMenuStrip;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 29);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(1140, 121);
            this.listView.TabIndex = 10;
            this.listView.TileSize = new System.Drawing.Size(100, 114);
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.List;
            this.listView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ListView_AfterLabelEdit);
            this.listView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListView_DrawItem);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.Enter += new System.EventHandler(this.ListView_Enter);
            this.listView.Leave += new System.EventHandler(this.ListView_Leave);
            this.listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseDoubleClick);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 320;
            // 
            // typeColumnHeader
            // 
            this.typeColumnHeader.Text = "Type";
            this.typeColumnHeader.Width = 80;
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Description";
            this.descriptionColumnHeader.Width = 480;
            // 
            // ContainerAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.rightJumpButton);
            this.Controls.Add(this.leftJumpButton);
            this.Controls.Add(this.childComboBox);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.viewComboBox);
            this.Name = "ContainerAssetControl";
            this.Size = new System.Drawing.Size(1140, 150);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.ComboBox viewComboBox;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private Components.ComboBox childComboBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button rightJumpButton;
        private System.Windows.Forms.Button leftJumpButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem localeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localeExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localeImportToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog localeImportFileDialog;
        private System.Windows.Forms.SaveFileDialog localeExportFileDialog;
        private Components.DoubleBufferedListView listView;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader typeColumnHeader;
        private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
    }
}
