namespace CDK.Assets.Attributes
{
    partial class AttributeControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.itemButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.localeColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upToolStripMenuItem,
            this.downToolStripMenuItem,
            this.replaceValueToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(173, 70);
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
            // replaceValueToolStripMenuItem
            // 
            this.replaceValueToolStripMenuItem.Name = "replaceValueToolStripMenuItem";
            this.replaceValueToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.replaceValueToolStripMenuItem.Text = "Replace Value";
            this.replaceValueToolStripMenuItem.Click += new System.EventHandler(this.ReplaceValueToolStripMenuItem_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.valueColumn,
            this.typeColumn,
            this.itemButtonColumn,
            this.descriptionColumn,
            this.listColumn,
            this.localeColumn});
            this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(820, 423);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellClick);
            this.dataGridView.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridView_UserDeletedRow);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DataGridView_UserDeletingRow);
            // 
            // nameColumn
            // 
            this.nameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.nameColumn.DataPropertyName = "Name";
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.MinimumWidth = 120;
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.nameColumn.Width = 120;
            // 
            // valueColumn
            // 
            this.valueColumn.DataPropertyName = "Value";
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.valueColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.valueColumn.HeaderText = "Value";
            this.valueColumn.Name = "valueColumn";
            this.valueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.valueColumn.Width = 120;
            // 
            // typeColumn
            // 
            this.typeColumn.DataPropertyName = "Type";
            this.typeColumn.HeaderText = "Type";
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.Width = 80;
            // 
            // itemButtonColumn
            // 
            this.itemButtonColumn.HeaderText = "Items";
            this.itemButtonColumn.Name = "itemButtonColumn";
            this.itemButtonColumn.Width = 42;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.DataPropertyName = "Description";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.descriptionColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.descriptionColumn.HeaderText = "Description";
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.descriptionColumn.Width = 240;
            // 
            // listColumn
            // 
            this.listColumn.DataPropertyName = "ListType";
            this.listColumn.HeaderText = "List";
            this.listColumn.Name = "listColumn";
            this.listColumn.Width = 80;
            // 
            // localeColumn
            // 
            this.localeColumn.DataPropertyName = "Locale";
            this.localeColumn.HeaderText = "Locale";
            this.localeColumn.Name = "localeColumn";
            this.localeColumn.Width = 50;
            // 
            // exportFileDialog
            // 
            this.exportFileDialog.Filter = "Excel|*.xlsx|UTF-8 CSV|*.csv";
            this.exportFileDialog.RestoreDirectory = true;
            // 
            // importFileDialog
            // 
            this.importFileDialog.Filter = "Excel|*.xlsx|UTF-8 CSV|*.csv";
            // 
            // AttributeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.dataGridView);
            this.Name = "AttributeControl";
            this.Size = new System.Drawing.Size(820, 423);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private System.Windows.Forms.ToolStripMenuItem replaceValueToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog importFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn typeColumn;
        private System.Windows.Forms.DataGridViewButtonColumn itemButtonColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn listColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn localeColumn;
    }
}
