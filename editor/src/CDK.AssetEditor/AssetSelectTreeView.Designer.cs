﻿namespace CDK.Assets
{
    partial class AssetSelectTreeView
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.ItemHeight = 18;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(150, 150);
            this.treeView.TabIndex = 0;
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterCheck);
            this.treeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterCollapse);
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView_BeforeExpand);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TreeView_ItemDrag);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseDoubleClick);
            // 
            // AssetSelectTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Name = "AssetSelectTreeView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
    }
}
