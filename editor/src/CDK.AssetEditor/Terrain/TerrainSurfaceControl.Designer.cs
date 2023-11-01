
namespace CDK.Assets.Terrain
{
    partial class TerrainSurfaceControl
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
            this.components = new System.ComponentModel.Container();
            this.originCheckBox = new System.Windows.Forms.CheckBox();
            this.materialCheckBox = new System.Windows.Forms.CheckBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.modeComboBox = new CDK.Assets.Components.ComboBox();
            this.fillButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sizeControl = new CDK.Assets.Components.TrackControl();
            this.shadowAttenuationControl = new CDK.Assets.Components.TrackControl();
            this.degreeControl = new CDK.Assets.Components.TrackControl();
            this.attenuationControl = new CDK.Assets.Components.TrackControl();
            this.shadowControl = new CDK.Assets.Components.TrackControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.surfaceSplitContainer = new System.Windows.Forms.SplitContainer();
            this.originControl = new CDK.Assets.Terrain.TerrainSurfaceSelectControl();
            this.selectControl = new CDK.Assets.Terrain.TerrainSurfaceSelectControl();
            this.surfaceMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialControl = new CDK.Assets.Terrain.TerrainSurfaceMaterialControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.surfaceSplitContainer)).BeginInit();
            this.surfaceSplitContainer.Panel1.SuspendLayout();
            this.surfaceSplitContainer.Panel2.SuspendLayout();
            this.surfaceSplitContainer.SuspendLayout();
            this.surfaceMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // originCheckBox
            // 
            this.originCheckBox.AutoSize = true;
            this.originCheckBox.Location = new System.Drawing.Point(77, 139);
            this.originCheckBox.Name = "originCheckBox";
            this.originCheckBox.Size = new System.Drawing.Size(57, 16);
            this.originCheckBox.TabIndex = 61;
            this.originCheckBox.Text = "Origin";
            this.originCheckBox.UseVisualStyleBackColor = true;
            this.originCheckBox.CheckedChanged += new System.EventHandler(this.OriginCheckBox_CheckedChanged);
            // 
            // materialCheckBox
            // 
            this.materialCheckBox.AutoSize = true;
            this.materialCheckBox.Checked = true;
            this.materialCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.materialCheckBox.Location = new System.Drawing.Point(2, 139);
            this.materialCheckBox.Name = "materialCheckBox";
            this.materialCheckBox.Size = new System.Drawing.Size(69, 16);
            this.materialCheckBox.TabIndex = 57;
            this.materialCheckBox.Text = "Material";
            this.materialCheckBox.UseVisualStyleBackColor = true;
            this.materialCheckBox.CheckedChanged += new System.EventHandler(this.MaterialCheckBox_CheckedChanged);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.Location = new System.Drawing.Point(210, 135);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(52, 23);
            this.clearButton.TabIndex = 60;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(0, 112);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(117, 12);
            this.label15.TabIndex = 59;
            this.label15.Text = "Shadow Attenuation";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(0, 85);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(51, 12);
            this.label14.TabIndex = 58;
            this.label14.Text = "Shadow";
            // 
            // modeComboBox
            // 
            this.modeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(146, 137);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(58, 20);
            this.modeComboBox.TabIndex = 56;
            // 
            // fillButton
            // 
            this.fillButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fillButton.Location = new System.Drawing.Point(268, 135);
            this.fillButton.Name = "fillButton";
            this.fillButton.Size = new System.Drawing.Size(52, 23);
            this.fillButton.TabIndex = 55;
            this.fillButton.Text = "Fill";
            this.fillButton.UseVisualStyleBackColor = true;
            this.fillButton.Click += new System.EventHandler(this.FillButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 12);
            this.label5.TabIndex = 54;
            this.label5.Text = "Attenuation";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 12);
            this.label6.TabIndex = 53;
            this.label6.Text = "Intensity";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 12);
            this.label4.TabIndex = 52;
            this.label4.Text = "Size";
            // 
            // sizeControl
            // 
            this.sizeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sizeControl.DecimalPlaces = 2;
            this.sizeControl.Increment = 0.1F;
            this.sizeControl.Location = new System.Drawing.Point(129, 0);
            this.sizeControl.Maximum = 5F;
            this.sizeControl.Minimum = 0F;
            this.sizeControl.Name = "sizeControl";
            this.sizeControl.Size = new System.Drawing.Size(191, 21);
            this.sizeControl.TabIndex = 62;
            this.sizeControl.Value = 3F;
            // 
            // shadowAttenuationControl
            // 
            this.shadowAttenuationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shadowAttenuationControl.DecimalPlaces = 2;
            this.shadowAttenuationControl.Increment = 0.01F;
            this.shadowAttenuationControl.Location = new System.Drawing.Point(129, 108);
            this.shadowAttenuationControl.Maximum = 1F;
            this.shadowAttenuationControl.Minimum = 0F;
            this.shadowAttenuationControl.Name = "shadowAttenuationControl";
            this.shadowAttenuationControl.Size = new System.Drawing.Size(191, 21);
            this.shadowAttenuationControl.TabIndex = 63;
            this.shadowAttenuationControl.Value = 0F;
            // 
            // degreeControl
            // 
            this.degreeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.degreeControl.DecimalPlaces = 2;
            this.degreeControl.Increment = 0.01F;
            this.degreeControl.Location = new System.Drawing.Point(129, 27);
            this.degreeControl.Maximum = 1F;
            this.degreeControl.Minimum = 0.01F;
            this.degreeControl.Name = "degreeControl";
            this.degreeControl.Size = new System.Drawing.Size(191, 21);
            this.degreeControl.TabIndex = 64;
            this.degreeControl.Value = 1F;
            // 
            // attenuationControl
            // 
            this.attenuationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.attenuationControl.DecimalPlaces = 2;
            this.attenuationControl.Increment = 0.01F;
            this.attenuationControl.Location = new System.Drawing.Point(129, 54);
            this.attenuationControl.Maximum = 0.9F;
            this.attenuationControl.Minimum = 0F;
            this.attenuationControl.Name = "attenuationControl";
            this.attenuationControl.Size = new System.Drawing.Size(191, 21);
            this.attenuationControl.TabIndex = 65;
            this.attenuationControl.Value = 0.5F;
            // 
            // shadowControl
            // 
            this.shadowControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shadowControl.DecimalPlaces = 2;
            this.shadowControl.Increment = 0.01F;
            this.shadowControl.Location = new System.Drawing.Point(129, 81);
            this.shadowControl.Maximum = 1F;
            this.shadowControl.Minimum = 0F;
            this.shadowControl.Name = "shadowControl";
            this.shadowControl.Size = new System.Drawing.Size(191, 21);
            this.shadowControl.TabIndex = 66;
            this.shadowControl.Value = 0F;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 168);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.surfaceSplitContainer);
            this.splitContainer.Panel1MinSize = 90;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.Controls.Add(this.materialControl);
            this.splitContainer.Size = new System.Drawing.Size(320, 1332);
            this.splitContainer.SplitterDistance = 440;
            this.splitContainer.TabIndex = 67;
            // 
            // surfaceSplitContainer
            // 
            this.surfaceSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.surfaceSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.surfaceSplitContainer.Name = "surfaceSplitContainer";
            // 
            // surfaceSplitContainer.Panel1
            // 
            this.surfaceSplitContainer.Panel1.Controls.Add(this.originControl);
            this.surfaceSplitContainer.Panel1Collapsed = true;
            // 
            // surfaceSplitContainer.Panel2
            // 
            this.surfaceSplitContainer.Panel2.Controls.Add(this.selectControl);
            this.surfaceSplitContainer.Size = new System.Drawing.Size(320, 440);
            this.surfaceSplitContainer.SplitterDistance = 121;
            this.surfaceSplitContainer.TabIndex = 1;
            // 
            // originControl
            // 
            this.originControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.originControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originControl.Location = new System.Drawing.Point(0, 0);
            this.originControl.Name = "originControl";
            this.originControl.Size = new System.Drawing.Size(121, 100);
            this.originControl.TabIndex = 1;
            // 
            // selectControl
            // 
            this.selectControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.selectControl.ContextMenuStrip = this.surfaceMenuStrip;
            this.selectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectControl.Location = new System.Drawing.Point(0, 0);
            this.selectControl.Name = "selectControl";
            this.selectControl.Size = new System.Drawing.Size(320, 440);
            this.selectControl.TabIndex = 0;
            // 
            // surfaceMenuStrip
            // 
            this.surfaceMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.importToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.surfaceMenuStrip.Name = "surfaceMenuStrip";
            this.surfaceMenuStrip.Size = new System.Drawing.Size(207, 114);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.AddToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.MoveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.MoveDownToolStripMenuItem_Click);
            // 
            // materialControl
            // 
            this.materialControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialControl.Location = new System.Drawing.Point(0, 0);
            this.materialControl.Name = "materialControl";
            this.materialControl.Size = new System.Drawing.Size(317, 883);
            this.materialControl.TabIndex = 0;
            // 
            // TerrainSurfaceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.shadowControl);
            this.Controls.Add(this.attenuationControl);
            this.Controls.Add(this.degreeControl);
            this.Controls.Add(this.shadowAttenuationControl);
            this.Controls.Add(this.sizeControl);
            this.Controls.Add(this.originCheckBox);
            this.Controls.Add(this.materialCheckBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.fillButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Name = "TerrainSurfaceControl";
            this.Size = new System.Drawing.Size(320, 1500);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.surfaceSplitContainer.Panel1.ResumeLayout(false);
            this.surfaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.surfaceSplitContainer)).EndInit();
            this.surfaceSplitContainer.ResumeLayout(false);
            this.surfaceMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox originCheckBox;
        private System.Windows.Forms.CheckBox materialCheckBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private Components.ComboBox modeComboBox;
        private System.Windows.Forms.Button fillButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private Components.TrackControl sizeControl;
        private Components.TrackControl shadowAttenuationControl;
        private Components.TrackControl degreeControl;
        private Components.TrackControl attenuationControl;
        private Components.TrackControl shadowControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private TerrainSurfaceSelectControl selectControl;
        private TerrainSurfaceMaterialControl materialControl;
        private System.Windows.Forms.SplitContainer surfaceSplitContainer;
        private TerrainSurfaceSelectControl originControl;
        private System.Windows.Forms.ContextMenuStrip surfaceMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
    }
}
