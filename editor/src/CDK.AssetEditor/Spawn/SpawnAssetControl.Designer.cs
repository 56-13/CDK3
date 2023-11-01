namespace CDK.Assets.Spawn
{
    partial class SpawnAssetControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.colliderReferenceControl = new CDK.Assets.Attributes.AttributeElementSelectControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.collisionTilesDataGridView = new System.Windows.Forms.DataGridView();
            this.collisionTargetsDataGridView = new System.Windows.Forms.DataGridView();
            this.collisionTargetSourceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.collisionTargetDistanceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.collisionTargetSquareColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.viewTreeView = new CDK.Assets.AssetSelectTreeView();
            this.viewScreenControl = new CDK.Assets.Scenes.SceneScreenControl();
            this.attributeControl = new CDK.Assets.Attributes.AttributeControl();
            this.label6 = new System.Windows.Forms.Label();
            this.collisionSourceTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.locationTypeComboBox = new CDK.Assets.Components.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.colliderUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.subControl = new CDK.Assets.Containers.ContainerAssetControl();
            this.collisionTileColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.collisionTileElementColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collisionTilesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.collisionTargetsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colliderUpDown)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(980, 640);
            this.tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.colliderReferenceControl);
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.collisionSourceTextBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.locationTypeComboBox);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.colliderUpDown);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(972, 614);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Preference";
            // 
            // colliderReferenceControl
            // 
            this.colliderReferenceControl.Location = new System.Drawing.Point(386, 6);
            this.colliderReferenceControl.Name = "colliderReferenceControl";
            this.colliderReferenceControl.Size = new System.Drawing.Size(258, 21);
            this.colliderReferenceControl.TabIndex = 27;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.collisionTilesDataGridView);
            this.splitContainer1.Panel1.Controls.Add(this.collisionTargetsDataGridView);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Size = new System.Drawing.Size(958, 575);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(312, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Collision Tiles";
            // 
            // collisionTilesDataGridView
            // 
            this.collisionTilesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.collisionTilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.collisionTilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.collisionTileColumn,
            this.collisionTileElementColumn});
            this.collisionTilesDataGridView.Location = new System.Drawing.Point(314, 20);
            this.collisionTilesDataGridView.Name = "collisionTilesDataGridView";
            this.collisionTilesDataGridView.RowTemplate.Height = 23;
            this.collisionTilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.collisionTilesDataGridView.Size = new System.Drawing.Size(323, 127);
            this.collisionTilesDataGridView.TabIndex = 8;
            this.collisionTilesDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CollisionTilesDataGridView_CellBeginEdit);
            this.collisionTilesDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CollisionTilesDataGridView_CellEndEdit);
            // 
            // collisionTargetsDataGridView
            // 
            this.collisionTargetsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.collisionTargetsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.collisionTargetsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.collisionTargetSourceColumn,
            this.collisionTargetDistanceColumn,
            this.collisionTargetSquareColumn});
            this.collisionTargetsDataGridView.Location = new System.Drawing.Point(0, 20);
            this.collisionTargetsDataGridView.Name = "collisionTargetsDataGridView";
            this.collisionTargetsDataGridView.RowTemplate.Height = 23;
            this.collisionTargetsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.collisionTargetsDataGridView.Size = new System.Drawing.Size(308, 127);
            this.collisionTargetsDataGridView.TabIndex = 6;
            this.collisionTargetsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.CollisionTargetsDataGridView_CellBeginEdit);
            this.collisionTargetsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CollisionTargetsDataGridView_CellEndEdit);
            // 
            // collisionTargetSourceColumn
            // 
            this.collisionTargetSourceColumn.DataPropertyName = "Source";
            this.collisionTargetSourceColumn.HeaderText = "Source";
            this.collisionTargetSourceColumn.Name = "collisionTargetSourceColumn";
            this.collisionTargetSourceColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.collisionTargetSourceColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.collisionTargetSourceColumn.Width = 140;
            // 
            // collisionTargetDistanceColumn
            // 
            this.collisionTargetDistanceColumn.DataPropertyName = "Distance";
            this.collisionTargetDistanceColumn.HeaderText = "Distance";
            this.collisionTargetDistanceColumn.Name = "collisionTargetDistanceColumn";
            this.collisionTargetDistanceColumn.Width = 75;
            // 
            // collisionTargetSquareColumn
            // 
            this.collisionTargetSquareColumn.DataPropertyName = "Square";
            this.collisionTargetSquareColumn.HeaderText = "Square";
            this.collisionTargetSquareColumn.Name = "collisionTargetSquareColumn";
            this.collisionTargetSquareColumn.Width = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Collision Targets";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.attributeControl);
            this.splitContainer2.Size = new System.Drawing.Size(958, 421);
            this.splitContainer2.SplitterDistance = 250;
            this.splitContainer2.TabIndex = 10;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.viewTreeView);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.viewScreenControl);
            this.splitContainer3.Size = new System.Drawing.Size(958, 250);
            this.splitContainer3.SplitterDistance = 309;
            this.splitContainer3.TabIndex = 0;
            // 
            // viewTreeView
            // 
            this.viewTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewTreeView.Location = new System.Drawing.Point(0, 0);
            this.viewTreeView.Name = "viewTreeView";
            this.viewTreeView.Size = new System.Drawing.Size(309, 250);
            this.viewTreeView.TabIndex = 0;
            this.viewTreeView.Types = new CDK.Assets.AssetType[] {
        CDK.Assets.AssetType.Material,
        CDK.Assets.AssetType.Mesh,
        CDK.Assets.AssetType.Animation};
            // 
            // viewScreenControl
            // 
            this.viewScreenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewScreenControl.Location = new System.Drawing.Point(0, 0);
            this.viewScreenControl.Name = "viewScreenControl";
            this.viewScreenControl.Size = new System.Drawing.Size(645, 250);
            this.viewScreenControl.TabIndex = 0;
            this.viewScreenControl.Text = "sceneScreenControl1";
            // 
            // attributeControl
            // 
            this.attributeControl.BackColor = System.Drawing.Color.Transparent;
            this.attributeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributeControl.Location = new System.Drawing.Point(0, 0);
            this.attributeControl.Name = "attributeControl";
            this.attributeControl.Size = new System.Drawing.Size(958, 167);
            this.attributeControl.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(275, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "Attributes";
            // 
            // collisionSourceTextBox
            // 
            this.collisionSourceTextBox.Location = new System.Drawing.Point(110, 6);
            this.collisionSourceTextBox.Name = "collisionSourceTextBox";
            this.collisionSourceTextBox.Size = new System.Drawing.Size(140, 21);
            this.collisionSourceTextBox.TabIndex = 25;
            this.collisionSourceTextBox.Validated += new System.EventHandler(this.CollisionSourceTextBox_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "Collision Source";
            // 
            // locationTypeComboBox
            // 
            this.locationTypeComboBox.Font = new System.Drawing.Font("굴림", 10F);
            this.locationTypeComboBox.FormattingEnabled = true;
            this.locationTypeComboBox.Location = new System.Drawing.Point(709, 6);
            this.locationTypeComboBox.Name = "locationTypeComboBox";
            this.locationTypeComboBox.Size = new System.Drawing.Size(90, 21);
            this.locationTypeComboBox.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(650, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 19;
            this.label5.Text = "Location";
            // 
            // colliderUpDown
            // 
            this.colliderUpDown.DecimalPlaces = 2;
            this.colliderUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.colliderUpDown.Location = new System.Drawing.Point(310, 6);
            this.colliderUpDown.Name = "colliderUpDown";
            this.colliderUpDown.Size = new System.Drawing.Size(70, 21);
            this.colliderUpDown.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "Collider";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.subControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(972, 614);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sub";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // subControl
            // 
            this.subControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subControl.Location = new System.Drawing.Point(3, 3);
            this.subControl.Name = "subControl";
            this.subControl.Size = new System.Drawing.Size(966, 608);
            this.subControl.TabIndex = 0;
            // 
            // collisionTileColumn
            // 
            this.collisionTileColumn.DataPropertyName = "Name";
            this.collisionTileColumn.HeaderText = "Tile";
            this.collisionTileColumn.Name = "collisionTileColumn";
            this.collisionTileColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.collisionTileColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.collisionTileColumn.Width = 140;
            // 
            // collisionTileElementColumn
            // 
            this.collisionTileElementColumn.DataPropertyName = "Element";
            this.collisionTileElementColumn.HeaderText = "Element";
            this.collisionTileElementColumn.Name = "collisionTileElementColumn";
            this.collisionTileElementColumn.Width = 140;
            // 
            // SpawnAssetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "SpawnAssetControl";
            this.Size = new System.Drawing.Size(980, 640);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collisionTilesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.collisionTargetsDataGridView)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.colliderUpDown)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox collisionSourceTextBox;
        private System.Windows.Forms.Label label1;
        private Components.ComboBox locationTypeComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown colliderUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView collisionTargetsDataGridView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView collisionTilesDataGridView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Attributes.AttributeControl attributeControl;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private AssetSelectTreeView viewTreeView;
        private Scenes.SceneScreenControl viewScreenControl;
        private Attributes.AttributeElementSelectControl colliderReferenceControl;
        private Containers.ContainerAssetControl subControl;
        private System.Windows.Forms.DataGridViewTextBoxColumn collisionTargetSourceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn collisionTargetDistanceColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn collisionTargetSquareColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn collisionTileColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn collisionTileElementColumn;
    }
}
