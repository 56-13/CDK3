namespace CDK.Assets.Texturing
{
    partial class SubImageSelectControl
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel = new System.Windows.Forms.Panel();
            this.screenControl = new CDK.Assets.Texturing.ImageSelectScreenControl();
            this.imageSelectScreenControl1 = new CDK.Assets.Texturing.ImageSelectScreenControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel);
            this.splitContainer.Panel2.Controls.Add(this.imageSelectScreenControl1);
            this.splitContainer.Size = new System.Drawing.Size(300, 150);
            this.splitContainer.SplitterDistance = 150;
            this.splitContainer.TabIndex = 0;
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
            this.listView.Size = new System.Drawing.Size(150, 150);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListView_ItemCheck);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.SizeChanged += new System.EventHandler(this.ListView_SizeChanged);
            // 
            // columnHeader
            // 
            this.columnHeader.Text = "";
            this.columnHeader.Width = 120;
            // 
            // panel
            // 
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel.Controls.Add(this.screenControl);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(146, 150);
            this.panel.TabIndex = 1;
            // 
            // screenControl
            // 
            this.screenControl.BackColor = System.Drawing.Color.White;
            this.screenControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenControl.Location = new System.Drawing.Point(0, 0);
            this.screenControl.Name = "screenControl";
            this.screenControl.Size = new System.Drawing.Size(144, 148);
            this.screenControl.TabIndex = 1;
            this.screenControl.Text = "imageSelectScreenControl2";
            // 
            // imageSelectScreenControl1
            // 
            this.imageSelectScreenControl1.Location = new System.Drawing.Point(62, 58);
            this.imageSelectScreenControl1.Name = "imageSelectScreenControl1";
            this.imageSelectScreenControl1.Size = new System.Drawing.Size(75, 23);
            this.imageSelectScreenControl1.TabIndex = 0;
            this.imageSelectScreenControl1.Text = "imageSelectScreenControl1";
            // 
            // SubImageSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "SubImageSelectControl";
            this.Size = new System.Drawing.Size(300, 150);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private ImageSelectScreenControl imageSelectScreenControl1;
        private ImageSelectScreenControl screenControl;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader;
    }
}
