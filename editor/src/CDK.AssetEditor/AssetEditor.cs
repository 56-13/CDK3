using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using CDK.Assets.Containers;
using CDK.Assets.Support;

namespace CDK.Assets
{
    public partial class AssetEditor : Form
    {
        private bool labelEditTag;

        private List<AssetForm> assetForms;

        private Dictionary<Asset, TreeNode> treeNodes;

        private ProgressThread progressThread;

        private LocaleStringExportForm localeExportForm;

        #if DEBUG
        private static readonly string Title = "AssetEditor DBG." + AssetManager.EditorVersion;
        #else
        private static readonly string Title = "AssetEditor REL." + AssetManager.EditorVersion;
        #endif

        private Asset[] SelectedAssets
        {
            get
            {
                Asset[] assets = new Asset[treeView.SelectedNodes.Count];
                for (int i = 0; i < assets.Length; i++) assets[i] = (Asset)treeView.SelectedNodes[i].Tag;
                return assets;
            }
        }

        public AssetEditor()
        {
            InitializeComponent();

            Text = Title;

            openFileDialog.Filter = FileFilters.Xml;
            localeImportFileDialog.Filter = localeExportFileDialog.Filter = FileFilters.ExcelOrCsv;

            treeView.ImageList = AssetControl.Instance?.SmallImageList;

            treeNodes = new Dictionary<Asset, TreeNode>();

            assetForms = new List<AssetForm>();

            levelDownPackageToolStripMenuItem.Tag = AssetType.Package;
            levelDownListToolStripMenuItem.Tag = AssetType.List;
            levelDownBlockToolStripMenuItem.Tag = AssetType.Block;
            levelDownBlockListToolStripMenuItem.Tag = AssetType.BlockList;
            levelDownFileToolStripMenuItem.Tag = AssetType.File;
            levelDownFolderToolStripMenuItem.Tag = AssetType.Folder;

            localeExportForm = new LocaleStringExportForm();

            if (AssetManager.IsCreated)
            {
                AssetManager.Instance.Projects.ListChanged += Projects_ListChanged;
                AssetManager.Instance.Invoking += AssetEditor_Invoking;
                AssetManager.Instance.Opening += AssetEditor_Opening;
                AssetManager.Instance.Messaging += AssetEditor_Messaging;
                AssetManager.Instance.StartProgressing += AssetEditor_StartProgressing;
                AssetManager.Instance.Progressing += AssetEditor_Progressing;
                AssetManager.Instance.EndProgressing += AssetEditor_EndProgressing;
                AssetManager.Instance.PropertyChanged += AssetEditor_PropertyChanged;
                AssetManager.Instance.BeginLoading += AssetEditor_BeginLoading;
                AssetManager.Instance.EndLoading += AssetEditor_EndLoading;

                foreach (var project in AssetManager.Instance.Projects)
                {
                    treeView.Nodes.Add(CreateTreeNode(project));
                }
                saveAllToolStripMenuItem.Enabled = AssetManager.Instance.Projects.Count != 0;
                saveToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = AssetManager.Instance.UndoCommandEnabled;
                redoToolStripMenuItem.Enabled = AssetManager.Instance.RedoCommandEnabled;

                if (AssetManager.Instance.Config.ProjectPath != null)
                {
                    var path = AssetManager.Instance.Config.ProjectPath;
                    openFileDialog.FileName = path;
                    openFileDialog.InitialDirectory = path.Substring(0, path.LastIndexOf('\\'));
                }
                /*
                if (AssetManager.Instance.Config.BuildPath != null)
                {
                    folderBrowserDialog.SelectedPath = AssetManager.Instance.Config.BuildPath;
                }
                */
                if (AssetManager.Instance.IsDeveloper)
                {
                    developerToolStripMenuItem.Checked = true;
                }
            }
            //assetFixToolStripMenuItem.Visible = AssetFix.Enabled;

            FormClosing += AssetEditor_FormClosing;

            Disposed += AssetEditor_Disposed;
        }

        private void AssetEditor_Disposed(object sender, EventArgs e)
        {
            if (AssetManager.IsCreated)
            {
                AssetManager.Instance.Projects.ListChanged -= Projects_ListChanged;
                AssetManager.Instance.Invoking -= AssetEditor_Invoking;
                AssetManager.Instance.Opening -= AssetEditor_Opening;
                AssetManager.Instance.Messaging -= AssetEditor_Messaging;
                AssetManager.Instance.StartProgressing -= AssetEditor_StartProgressing;
                AssetManager.Instance.Progressing -= AssetEditor_Progressing;
                AssetManager.Instance.EndProgressing -= AssetEditor_EndProgressing;
                AssetManager.Instance.BeginLoading -= AssetEditor_BeginLoading;
                AssetManager.Instance.EndLoading -= AssetEditor_EndLoading;
                AssetManager.Instance.PropertyChanged -= AssetEditor_PropertyChanged;
            }
            foreach (var assetForm in assetForms)
            {
                assetForm.Dispose();
            }
            foreach (TreeNode node in treeView.Nodes)
            {
                DisposeTreeNode(node);
            }

            localeExportForm.Dispose();

            progressThread?.End();
        }

        private void AssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            var flag = false;
            foreach (var project in AssetManager.Instance.Projects)
            {
                project.Save(false);

                if (CheckUnsaved(project))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                switch (MessageBox.Show(this, "저장되지 않은 파일이 있습니다. 저장하고 종료하시겠습니까?", "저장", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        try
                        {
                            foreach (ProjectAsset project in AssetManager.Instance.Projects)
                            {
                                project.SaveAll(false);
                            }
                        }
                        catch (AssetException ex)
                        {
                            if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                            {
                                AssetManager.Instance.Open(ex.Asset);

                                e.Cancel = true;
                            }
                        }
                        catch (Exception uex)
                        {
                            ErrorHandler.Record(uex);

                            if (MessageBox.Show(this, "파일을 저장할 수 없습니다. 무시하고 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                e.Cancel = true;
                            }
                        }
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
        private void LocaleSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.Locale = (string)((ToolStripMenuItem)sender).Tag;
        }

        private void AssetEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName){
                case "UndoCommandEnabled":
                    undoToolStripMenuItem.Enabled = AssetManager.Instance.UndoCommandEnabled;
                    break;
                case "RedoCommandEnabled":
                    redoToolStripMenuItem.Enabled = AssetManager.Instance.RedoCommandEnabled;
                    break;
                case "Locale":
                    {
                        var locale = AssetManager.Instance.Locale;
                        foreach (ToolStripMenuItem localeSubToolStripMenuItem in localeToolStripMenuItem.DropDownItems)
                        {
                            localeSubToolStripMenuItem.Checked = string.Equals((string)localeSubToolStripMenuItem.Tag, locale);
                        }
                    }
                    break;
            }
        }

        private void Projects_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    treeView.Nodes.Insert(e.NewIndex, CreateTreeNode(AssetManager.Instance.Projects[e.NewIndex]));

                    saveAllToolStripMenuItem.Enabled = true;
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null)
                    {
                        switch (e.PropertyDescriptor.Name)
                        {
                            case "Title":
                                treeView.Nodes[e.NewIndex].Text = AssetManager.Instance.Projects[e.NewIndex].Title;
                                break;
                            case "IsDirty":
                                saveToolStripMenuItem.Enabled = IsSaveEnabled;
                                break;
                        }
                    }
                    else
                    {
                        RemoveTreeNode(treeView.Nodes[e.NewIndex]);
                        treeView.Nodes.Insert(e.NewIndex, CreateTreeNode(AssetManager.Instance.Projects[e.NewIndex]));
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    RemoveTreeNode(treeView.Nodes[e.NewIndex]);

                    if (AssetManager.Instance.Projects.Count == 0)
                    {
                        saveAllToolStripMenuItem.Enabled = false;
                    }
                    break;
                case ListChangedType.ItemMoved:
                    {
                        treeView.BeginUpdate();
                        var subnode = treeView.Nodes[e.OldIndex];
                        treeView.Nodes.RemoveAt(e.OldIndex);
                        treeView.Nodes.Insert(e.NewIndex, subnode);
                        treeView.EndUpdate();
                    }
                    break;
                case ListChangedType.Reset:
                    while (treeView.Nodes.Count != 0)
                    {
                        RemoveTreeNode(treeView.Nodes[0]);
                    }

                    saveAllToolStripMenuItem.Enabled = false;

                    foreach (var project in AssetManager.Instance.Projects)
                    {
                        treeView.Nodes.Add(CreateTreeNode(project));

                        saveAllToolStripMenuItem.Enabled = true;
                    }
                    break;

            }
        }

        private void AssetEditor_Opening(object sender, AssetOpenEventArgs e)
        {
            foreach (var assetForm in assetForms)
            {
                if (assetForm.Tag == e.Asset)
                {
                    return;
                }
            }
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Tag == e.Asset)
                {
                    tabControl.SelectedTab = page;
                    return;
                }
            }
            {
                tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;

                var page = new TabPage();
                page.DataBindings.Add("Text", e.Asset, "Title", false, DataSourceUpdateMode.Never);
                page.Tag = e.Asset;
                tabControl.TabPages.Add(page);

                tabControl.SelectedTab = page;
                TabControl_SelectedIndexChanged(tabControl, EventArgs.Empty);

                tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            }
        }

        private TreeNode FindTreeNode(Asset asset)
        {
            TreeNode node = null;

            if (asset != null && !treeNodes.TryGetValue(asset, out node))
            {
                node = FindTreeNode(asset.Parent);

                if (node != null)
                {
                    node.Expand();

                    treeNodes.TryGetValue(asset, out node);
                }
            }
            return node;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var page = tabControl.SelectedTab;

            if (page != null)
            {
                var asset = (Asset)page.Tag;

                var node = FindTreeNode(asset);

                if (node != null)
                {
                    treeView.SelectedNode = node;

                    node.EnsureVisible();
                }
                if (page.Controls.Count == 0)
                {
                    foreach (TabPage otherPage in tabControl.TabPages)
                    {
                        if (otherPage != page) otherPage.Controls.Clear();
                    }

                    var control = AssetControl.Instance.GetControl(asset, page);
                    if (control != null)
                    {
                        control.Dock = DockStyle.Fill;
                        page.Controls.Add(control);
                    }
                }
            }
        }

        private void AssetEditor_Invoking(object sender, AssetInvokeEventArgs e)
        {
            if (!IsHandleCreated) CreateHandle();

            BeginInvoke(e.Action);
        }

        private void AssetEditor_Messaging(object sender, AssetMessageEventArgs e)
        {
            if (MessageBox.Show(this, e.Message) == DialogResult.OK && e.Asset != null)
            {
                AssetManager.Instance.Open(e.Asset);
            }
        }

        private void AssetEditor_StartProgressing(object sender, AssetBeginProgressEventArgs e)
        {
            if (progressThread != null)
            {
                progressThread.End();
                progressThread = null;
            }
            progressThread = new ProgressThread(e.Maximum);
            progressThread.Start();
        }

        private void AssetEditor_Progressing(object sender, AssetProgressEventArgs e)
        {
            if (progressThread != null)
            {
                progressThread.Progress(e.Progress, e.Message);
            }
        }

        private void AssetEditor_EndProgressing(object sender, EventArgs e)
        {
            if (progressThread != null)
            {
                progressThread.End();
                progressThread = null;
            }
        }

        private void AssetEditor_BeginLoading(object sender, AssetBeginLoadingEventArgs e)
        {
            Text = Title + " Loading - " + e.Asset.Location;
        }

        private void AssetEditor_EndLoading(object sender, EventArgs e)
        {
            Text = Title;
        }

        private HashSet<Asset> _titleChanged = new HashSet<Asset>();         //속도가 매우 느림, 한번에 하는 것이 빠름

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            var asset = ((AssetElementList<Asset>)sender).Parent.Owner;

            if (treeNodes.TryGetValue(asset, out var node)) 
            {
                if (node.IsExpanded)
                {
                    switch (e.ListChangedType)
                    {
                        case ListChangedType.ItemAdded:
                            node.Nodes.Insert(e.NewIndex, CreateTreeNode(asset.Children[e.NewIndex]));
                            break;
                        case ListChangedType.ItemChanged:
                            if (e.PropertyDescriptor != null)
                            {
                                switch (e.PropertyDescriptor.Name)
                                {
                                    case "Title":
                                        if (_titleChanged.Count == 0)
                                        {
                                            var h = Handle;

                                            BeginInvoke((Action)(() =>
                                            {
                                                treeView.BeginUpdate();
                                                foreach (var a in _titleChanged)
                                                {
                                                    if (treeNodes.TryGetValue(a, out var n))
                                                    {
                                                        n.Text = a.Title;
                                                    }
                                                }
                                                _titleChanged.Clear();
                                                treeView.EndUpdate();
                                            }));
                                        }
                                        _titleChanged.Add(asset.Children[e.NewIndex]);
                                        break;
                                    case "IsDirty":
                                        saveToolStripMenuItem.Enabled = IsSaveEnabled;
                                        break;
                                }
                            }
                            else
                            {
                                RemoveTreeNode(node.Nodes[e.NewIndex]);
                                node.Nodes.Insert(e.NewIndex, CreateTreeNode(asset.Children[e.NewIndex]));
                            }
                            break;
                        case ListChangedType.ItemDeleted:
                            RemoveTreeNode(node.Nodes[e.NewIndex]);
                            break;
                        case ListChangedType.ItemMoved:
                            {
                                TreeNode subnode = node.Nodes[e.OldIndex];
                                node.Nodes.RemoveAt(e.OldIndex);
                                node.Nodes.Insert(e.NewIndex, subnode);
                            }
                            break;
                        case ListChangedType.Reset:
                            while (node.Nodes.Count != 0)
                            {
                                RemoveTreeNode(node.Nodes[0]);
                            }
                            foreach (var child in asset.Children)
                            {
                                node.Nodes.Add(CreateTreeNode(child));
                            }
                            break;

                    }
                }
                else
                {
                    if (asset.Children.Count == 0)
                    {
                        if (node.Nodes.Count != 0)
                        {
                            node.Nodes.Clear();
                        }
                    }
                    else
                    {
                        if (node.Nodes.Count == 0)
                        {
                            node.Nodes.Add(new TreeNode());
                        }
                    }
                }
            }
        }

        private TreeNode CreateTreeNode(Asset asset)
        {
            if (treeNodes.TryGetValue(asset, out var node))
            {
                return node;
            }

            node = new TreeNode
            {
                Text = asset.Title,
                Tag = asset
            };
            node.SelectedImageKey = node.ImageKey = asset.Type.ToString();
            node.ForeColor = NodeColor(asset);

            treeNodes.Add(asset, node);

            asset.Children.ListChanged += Children_ListChanged;

            if (asset.Children.Count != 0)
            {
                TreeNode dummyNode = new TreeNode();
                node.Nodes.Add(dummyNode);
            }

            return node;
        }

        private void DisposeTreeNode(TreeNode node)
        {
            foreach (TreeNode subnode in node.Nodes)
            {
                DisposeTreeNode(subnode);
            }

            var asset = (Asset)node.Tag;

            if (asset != null)
            {
                asset.Children.ListChanged -= Children_ListChanged;

                treeNodes.Remove(asset);
            }
        }

        private void RemoveTreeNode(TreeNode node)
        {
            while (node.Nodes.Count != 0)
            {
                RemoveTreeNode(node.Nodes[0]);
            }

            var asset = (Asset)node.Tag;

            if (asset != null)
            {
                asset.Children.ListChanged -= Children_ListChanged;

                treeNodes.Remove(asset);

                for (var i = 0; i < tabControl.TabCount; i++)
                {
                    TabPage page = tabControl.TabPages[i];

                    if (page.Tag == asset)
                    {
                        page.Controls.Clear();
                        AssetControl.Instance.RemoveControlData(page);
                        page.Dispose();
                        break;
                    }
                }
            }
            node.Remove();
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new NewProjectForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    var path = form.ProjectPath + "\\" + form.ProjectName;
                    foreach (var project in AssetManager.Instance.Projects)
                    {
                        if (project.ContentPath == path)
                        {
                            return;
                        }
                    }
                    AssetManager.Instance.Projects.Add(new ProjectAsset(form.ProjectName, form.ProjectPath));

                    AssetManager.Instance.Config.ProjectPath = form.ProjectPath + "\\" + form.ProjectName + ".xml";
                }
            }
        }

        private void AssetMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                e.Cancel = true;
            }
        }

        private void ResetAssetMenuStrip()
        {
            while (addToolStripMenuItem.DropDownItems.Count != 0)
            {
                addToolStripMenuItem.DropDownItems[0].Dispose();
            }
            var asset = (Asset)treeView.SelectedNode.Tag;

            var isDeveloper = AssetManager.Instance.IsDeveloper;

            if (asset.IsListed || isDeveloper)
            {
                foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
                {
                    if (asset.AddChildEnabled(type))
                    {
                        var addAssetToolStripMenuItem = new ToolStripMenuItem
                        {
                            Text = type.ToString(),
                            Tag = type
                        };
                        addAssetToolStripMenuItem.Click += AddAssetToolStripMenuItem_Click;
                        addToolStripMenuItem.DropDownItems.Add(addAssetToolStripMenuItem);
                    }
                }
            }
            addToolStripMenuItem.Enabled = addToolStripMenuItem.DropDownItems.Count != 0;

            cutToolStripMenuItem.Enabled = removeToolStripMenuItem.Enabled =
                isDeveloper || asset.Parent == null || asset.Parent.Type == AssetType.Project || asset.Parent.IsListed;
            renameToolStripMenuItem.Enabled = levelDownToolStripMenuItem.Enabled = isDeveloper;
            saveToolStripMenuItem.Enabled = IsSaveEnabled;
        }

        private bool treeViewBlockExpandCollapse;

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var asset = (Asset)e.Node.Tag;

            AssetManager.Instance.Open(asset);
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (treeView.SelectedNode != null)
                {
                    var asset = (Asset)treeView.SelectedNode.Tag;

                    AssetManager.Instance.Open(asset);
                }
            }
        }

        private void TreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (treeViewBlockExpandCollapse)
            {
                e.Cancel = true;

                treeViewBlockExpandCollapse = false;
            }
        }
       
        private void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode subnode in e.Node.Nodes)
            {
                DisposeTreeNode(subnode);
            }
            treeView.BeginUpdate();
            e.Node.Nodes.Clear();
            e.Node.Nodes.Add(new TreeNode());
            treeView.EndUpdate();
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (treeViewBlockExpandCollapse)
            {
                e.Cancel = true;

                treeViewBlockExpandCollapse = false;
            }
            else
            {
                var asset = (Asset)e.Node.Tag;
                if (asset.Children.Count != 0)
                {
                    treeView.BeginUpdate();
                    e.Node.Nodes.Clear();
                    foreach (var child in ((Asset)e.Node.Tag).Children)
                    {
                        e.Node.Nodes.Add(CreateTreeNode(child));
                    }
                    treeView.EndUpdate();
                }
            }
        }

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            treeViewBlockExpandCollapse = e.Clicks > 1;
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var asset = (Asset)treeView.SelectedNode?.Tag;

            if (asset != null) treeView.DoDragDrop(asset.Key, DragDropEffects.Copy | DragDropEffects.Link);
        }

        private void TabCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = tabControl.SelectedTab;

            if (page != null)
            {
                page.Controls.Clear();
                AssetControl.Instance.RemoveControlData(page);
                page.Dispose();
            }
        }

        private void TabCloseOthersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = tabControl.SelectedTab;

            if (page != null)
            {
                tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;
                tabControl.TabPages.Remove(page);
                while (tabControl.TabPages.Count != 0)
                {
                    tabControl.TabPages[0].Controls.Clear();
                    tabControl.TabPages[0].Dispose();
                }
                tabControl.TabPages.Add(page);
                TabControl_SelectedIndexChanged(tabControl, EventArgs.Empty);
                tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            }
        }

        private void TabCloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;
            while (tabControl.TabPages.Count != 0)
            {
                tabControl.TabPages[0].Controls.Clear();
                tabControl.TabPages[0].Dispose();
            }
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void TabWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage page = tabControl.SelectedTab;

            if (page != null && page.Controls.Count != 0)
            {
                var control = page.Controls[0];

                var asset = (Asset)page.Tag;

                page.Controls.Clear();
                
                var form = new AssetForm
                {
                    Tag = asset
                };
                form.DataBindings.Add("Text", asset, "Title", false, DataSourceUpdateMode.Never);
                form.FormClosing += AssetForm_FormClosing;
                control.Dock = DockStyle.Fill;
                form.Controls.Add(control);

                AssetControl.Instance.RemoveControlData(page);
                page.Dispose();

                form.Show(this);

                assetForms.Add(form);
            }
        }

        private void AssetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var form = (AssetForm)sender;
            form.Controls.Clear();
            assetForms.Remove(form);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = openFileDialog.FileName;

                foreach (var project in AssetManager.Instance.Projects)
                {
                    if ((project.ContentPath + ".xml").Equals(path))
                    {
                        MessageBox.Show(this, "이미 열려 있는 프로젝트는 열 수 없습니다.");
                        return;
                    }
                    else if (project.ContentPath[0] != path[0])
                    {
                        MessageBox.Show(this, "기존에 열려 있는 프로젝트와 드라이브 볼륨이 다른 프로젝트는 열 수 없습니다.");
                        return;
                    }
                }
                
                if (!ProjectAsset.GitOccupy(path))
                {
                    MessageBox.Show(this, "프로젝트의 변형을 막기 위해 Git가 사용 중인 경우 프로젝트를 로드할 수 없습니다. 먼저 Git 프로세스가 종료된 후 다시 시도해 주세요.");
                    return;
                }

                try
                {
                    var project = new ProjectAsset(path);

                    AssetManager.Instance.Projects.Add(project);

                    AssetManager.Instance.Config.ProjectPath = path;
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "프로젝트 파일을 읽을 수 없습니다.");
                }
            }
        }

        private Asset WorkingAsset
        {
            get
            {
                if (treeView.Focused)
                {
                    var node = treeView.SelectedNode;

                    if (node != null)
                    {
                        return (Asset)node.Tag;
                    }
                }
                else
                {
                    var page = tabControl.SelectedTab;

                    if (page != null)
                    {
                        return (Asset)page.Tag;
                    }
                }
                return null;
            }
        }

        private bool IsSaveEnabled
        {
            get
            {
                var asset = WorkingAsset;

                return asset != null && asset.IsDirty;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var asset = WorkingAsset;

            if (asset != null && asset.IsDirty)
            {
                try
                {
                    asset.Save(false);
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                }
            }
        }
        
        private void SaveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var project in AssetManager.Instance.Projects)
                {
                    project.SaveAll(false);
                }
            }
            catch (AssetException ex)
            {
                if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                {
                    AssetManager.Instance.Open(ex.Asset);
                }
            }
            catch (Exception uex)
            {
                ErrorHandler.Record(uex);

                MessageBox.Show(this, "파일을 저장할 수 없습니다.");
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.UndoCommand();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.RedoCommand();
        }

        private void BuildAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var path = folderBrowserDialog.SelectedPath + '\\';

                    var assets = new Asset[AssetManager.Instance.Projects.Count];
                    for (var i = 0; i < assets.Length; i++) assets[i] = AssetManager.Instance.Projects[i];
                    Asset.Build(assets, path);

                    //AssetManager.Instance.Config.BuildPath = folderBrowserDialog.SelectedPath;

                    MessageBox.Show(this, "빌드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "빌드를 진행할 수 없습니다.");
                }
            }
        }

        private void UploadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var path = folderBrowserDialog.SelectedPath + '\\';

                var assets = new Asset[AssetManager.Instance.Projects.Count];
                for (var i = 0; i < assets.Length; i++) assets[i] = AssetManager.Instance.Projects[i];
                Asset.Upload(assets);

                MessageBox.Show(this, "업로드가 완료되었습니다.");
            }
            catch (AssetException ex)
            {
                if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                {
                    AssetManager.Instance.Open(ex.Asset);
                }
            }
            catch (Exception uex)
            {
                ErrorHandler.Record(uex);

                MessageBox.Show(this, "업로드를 진행할 수 없습니다.");
            }
        }

        private void LocaleExport(Asset[] assets, string path, string keyLocale, string[] targetLocales, DateTime? timestamp)
        {
            try
            {
                var cells = Asset.LocaleExport(assets, keyLocale, targetLocales, timestamp);

                if (path.EndsWith(".xlsx"))
                {
                    using (var excel = new Excel(path))
                    {
                        using (var form = new ExcelSheetForm())
                        {
                            form.Excel = excel;

                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                for (var r = 0; r < cells.Length; r++)
                                {
                                    for (var c = 0; c < cells[r].Length; c++)
                                    {
                                        excel.SetCell(r, c, cells[r][c]);
                                    }
                                }
                                excel.Save();
                            }
                        }
                    }
                }
                else
                {
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        using (var writer = new StreamWriter(fs, Encoding.UTF8))
                        {
                            foreach (var row in cells)
                            {
                                writer.WriteLine(CSV.Make(row));
                            }
                        }
                    }
                }
                MessageBox.Show(this, "추출이 완료되었습니다.");
            }
            catch (AssetException ex)
            {
                if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                {
                    AssetManager.Instance.Open(ex.Asset);
                }
            }
            catch (Exception uex)
            {
                ErrorHandler.Record(uex);

                MessageBox.Show(this, "추출을 진행할 수 없습니다.");
            }
        }

        private void LocaleExportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (localeExportForm.ShowDialog(this) == DialogResult.OK && localeExportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeExportFileDialog.FileName;

                var assets = new Asset[AssetManager.Instance.Projects.Count];
                for (var i = 0; i < AssetManager.Instance.Projects.Count; i++)
                {
                    assets[i] = AssetManager.Instance.Projects[i];
                }

                var keyLocale = localeExportForm.KeyLocale;
                var targetLocales = localeExportForm.TargetLocales;
                var timestamp = localeExportForm.Timestamp;

                LocaleExport(assets, path, keyLocale, targetLocales, timestamp);
            }
        }

        private void LocaleCleanAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "불필요한 데이터를 정리하시겠습니까?", "정리", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    var assets = new Asset[AssetManager.Instance.Projects.Count];
                    for (var i = 0; i < AssetManager.Instance.Projects.Count; i++)
                    {
                        assets[i] = AssetManager.Instance.Projects[i];
                    }
                    Asset.LocaleClean(assets);

                    MessageBox.Show(this, "정리가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "정리를 진행할 수 없습니다.");
                }
            }
        }

        private void LocaleImport(Asset[] assets, string path)
        {
            var collisions = new List<LocaleString.ImportCollision>();

            try
            {
                string[][] cells;

                if (path.EndsWith(".xlsx"))
                {
                    using (var excel = new Excel(path))
                    {
                        using (var form = new ExcelSheetForm())
                        {
                            form.Excel = excel;
                            form.ReadOnly = true;

                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                cells = excel.GetCells();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var csv = File.ReadAllText(path, FileEncoding.GetFileEncoding(path));

                    cells = CSV.ParseAll(csv);
                }

                Asset.LocaleImport(assets, cells, collisions);

                if (collisions.Count != 0)
                {
                    if (MessageBox.Show(this, "일부 텍스트가 내용이 다릅니다.") == DialogResult.OK)
                    {
                        using (LocaleStringImportCollisionForm form = new LocaleStringImportCollisionForm(collisions))
                        {
                            bool ignore = form.ShowDialog(this) == DialogResult.Ignore;

                            if (ignore)
                            {
                                Asset.LocaleImport(assets, cells, null);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "변환이 완료되었습니다.");
                }
            }
            catch (AssetException ex)
            {
                if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                {
                    AssetManager.Instance.Open(ex.Asset);
                }
            }
            catch (Exception uex)
            {
                ErrorHandler.Record(uex);

                MessageBox.Show(this, "변환을 진행할 수 없습니다.");
            }
        }

        private void LocaleImportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (localeImportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeImportFileDialog.FileName;

                var assets = new Asset[AssetManager.Instance.Projects.Count];
                for (var i = 0; i < AssetManager.Instance.Projects.Count; i++)
                {
                    assets[i] = AssetManager.Instance.Projects[i];
                }
                LocaleImport(assets, path);
            }
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                var asset = (Asset)treeView.SelectedNode.Tag;

                AssetManager.Instance.Open(asset);
            }
        }

        private void AddAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                var asset = (Asset)treeView.SelectedNode.Tag;

                var item = (ToolStripMenuItem)sender;

                var type = (AssetType)item.Tag;

                asset.Children.Add(asset.NewChild(type));
            }
        }

        private bool CheckUnsaved(Asset asset)
        {
            if (asset.IsDirty) return true;
            foreach (var child in asset.Children)
            {
                if (CheckUnsaved(child)) return true;
            }
            return false;
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNodes.Count != 0 && MessageBox.Show(this, "리소스를 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var assets = new List<Asset>(treeView.SelectedNodes.Count);

                foreach (var node in treeView.SelectedNodes)
                {
                    assets.Add((Asset)node.Tag);
                }

                var i = 0;
                while (i < assets.Count)
                {
                    var src = assets[i];

                    bool valid = true;

                    for (int j = 0; j < assets.Count; j++)
                    {
                        if (i != j)
                        {
                            var dest = assets[j];

                            var current = src;
                            do
                            {
                                if (dest == current)
                                {
                                    valid = false;
                                    goto next;
                                }
                                current = current.Parent;
                            } while (current != null);
                        }
                    }
                next:
                    if (valid)
                    {
                        i++;
                    }
                    else
                    {
                        assets.RemoveAt(i);
                    }
                }

                foreach (var asset in assets)
                {
                    if (asset.IsRetained(out var from, out var to))
                    {
                        AssetControl.ShowRetained(this, from, to);
                        return;
                    }
                }

                i = 0;
                while (i < assets.Count)
                {
                    var asset = assets[i];

                    if (asset.Type == AssetType.Project)
                    {
                        asset.Save(false);

                        if (CheckUnsaved(asset))
                        {
                            switch (MessageBox.Show(this, "저장되지 않은 파일이 있습니다. 프로젝트를 닫기 전에 저장하시겠습니까?", "저장", MessageBoxButtons.YesNoCancel))
                            {
                                case DialogResult.Yes:
                                    try
                                    {
                                        asset.SaveAll(false);

                                        AssetManager.Instance.Projects.Remove((ProjectAsset)asset);
                                    }
                                    catch (AssetException ex)
                                    {
                                        if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                                        {
                                            AssetManager.Instance.Open(ex.Asset);
                                        }
                                    }
                                    catch (Exception uex)
                                    {
                                        ErrorHandler.Record(uex);

                                        MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                                    }
                                    break;
                                case DialogResult.No:
                                    AssetManager.Instance.Projects.Remove((ProjectAsset)asset);
                                    break;
                            }
                        }
                        else
                        {
                            AssetManager.Instance.Projects.Remove((ProjectAsset)asset);
                        }
                        assets.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                foreach (var asset in assets)
                {
                    if (asset.Parent != null)
                    {
                        asset.Parent.Children.Remove(asset);
                    }
                }
            }
        }

        private void ClipAssets(bool cut)
        {
            if (treeView.SelectedNodes.Count != 0)
            {
                AssetManager.Instance.Clip(SelectedAssets, cut);
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipAssets(false);
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipAssets(true);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                var asset = (Asset)treeView.SelectedNode.Tag;

                asset.PasteChildren();
            }
        }


        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.Text = ((Asset)treeView.SelectedNode.Tag).Name;

                labelEditTag = false;

                treeView.LabelEdit = true;

                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;

                treeView.SelectedNode.BeginEdit();
            }
        }

        private void TagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.Text = ((Asset)treeView.SelectedNode.Tag).Tag;

                labelEditTag = true;

                treeView.LabelEdit = true;

                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;

                treeView.SelectedNode.BeginEdit();
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var dirpath = folderBrowserDialog.SelectedPath;

                    foreach (var node in treeView.SelectedNodes)
                    {
                        var asset = (Asset)node.Tag;

                        asset.Export(dirpath);
                    }

                    MessageBox.Show(this, "내보내기가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "내보내기를 진행할 수 없습니다.");
                }
            }
        }

        private void BuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var path = folderBrowserDialog.SelectedPath + '\\';

                    Asset.Build(SelectedAssets, path);
                    
                    //AssetManager.Instance.BuildPath = folderBrowserDialog.SelectedPath;

                    MessageBox.Show(this, "빌드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "빌드를 진행할 수 없습니다.");
                }
            }
        }

        private void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    Asset.Upload(SelectedAssets);

                    MessageBox.Show(this, "업로드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "업로드를 진행할 수 없습니다.");
                }
            }
        }

        private void LocaleExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && localeExportForm.ShowDialog(this) == DialogResult.OK && localeExportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var path = localeExportFileDialog.FileName;

                var keyLocale = localeExportForm.KeyLocale;
                var targetLocales = localeExportForm.TargetLocales;
                var timestamp = localeExportForm.Timestamp;

                LocaleExport(SelectedAssets, path, keyLocale, targetLocales, timestamp);
            }
        }

        private void LocaleImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && localeImportFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string path = localeImportFileDialog.FileName;
                
                LocaleImport(SelectedAssets, path);
            }
        }

        private void LocaleCleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null && MessageBox.Show(this, "불필요한 데이터를 정리하시겠습니까?", "정리", MessageBoxButtons.YesNo) == DialogResult.Yes)            
            {
                try
                {
                    Asset.LocaleClean(SelectedAssets);

                    MessageBox.Show(this, "정리가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "정리를 진행할 수 없습니다.");
                }
            }
        }

        
        private void ResaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    foreach (var node in treeView.SelectedNodes)
                    {
                        var asset = (Asset)node.Tag;

                        asset.Save(true);
                    }
                    MessageBox.Show(this, "재저장이 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                }
            }
        }
        private void ResaveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    Asset.SaveAll(SelectedAssets, true);
                    
                    MessageBox.Show(this, "하위 전체 재저장이 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                }
            }
        }


        private void ReloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    foreach (var node in treeView.SelectedNodes)
                    {
                        var asset = (Asset)node.Tag;

                        asset.Load(true);
                    }
                    MessageBox.Show(this, "재로드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 로드할 수 없습니다.");
                }
            }
        }

        private void ReloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    foreach (var node in treeView.SelectedNodes)
                    {
                        var asset = (Asset)node.Tag;

                        asset.LoadAll(true);
                    }
                    MessageBox.Show(this, "하위 전체 재로드가 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 로드할 수 없습니다.");
                }
            }
        }

        private void CopyKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                Clipboard.SetText(((Asset)treeView.SelectedNode.Tag).Key);
            }
        }

        private void ReissueKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    foreach (var node in treeView.SelectedNodes)
                    {
                        var asset = (Asset)node.Tag;

                        asset.ReissueKey();
                    }
                    MessageBox.Show(this, "키재발급이 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                }
            }
        }

        private void ReissueKeyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                try
                {
                    Asset.ReissueKeyAll(SelectedAssets);
                    
                    MessageBox.Show(this, "하위 전체 키재발급이 완료되었습니다.");
                }
                catch (AssetException ex)
                {
                    if (MessageBox.Show(this, ex.Message) == DialogResult.OK && ex.Asset != null)
                    {
                        AssetManager.Instance.Open(ex.Asset);
                    }
                }
                catch (Exception uex)
                {
                    ErrorHandler.Record(uex);

                    MessageBox.Show(this, "파일을 저장할 수 없습니다.");
                }
            }
        }

        private void CleanFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if (MessageBox.Show(this, "해당 에셋의 경로에 포함되지 않는 하위 파일 및 폴더가 모두 삭제됩니다. 계속하시겠습니까?", "삭제", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    try
                    {
                        foreach (var node in treeView.SelectedNodes)
                        {
                            var asset = (Asset)node.Tag;

                            asset.CleanFiles();
                        }
                        MessageBox.Show(this, "파일 삭제가 완료되었습니다.");
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일 삭제를 완료할 수 없습니다.");
                    }
                }
            }
        }

        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var asset = (Asset)e.Node.Tag;

            if (e.Label != null)
            {
                if (labelEditTag)
                {
                    asset.Tag = e.Label;
                }
                else
                {
                    asset.Name = e.Label;
                }
            }
            e.CancelEdit = true;

            e.Node.Text = asset.Title;

            treeView.LabelEdit = false;

            copyToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            cutToolStripMenuItem.Enabled = removeToolStripMenuItem.Enabled = 
                AssetManager.Instance.IsDeveloper || asset.Parent == null || asset.Parent.Type == AssetType.Project || asset.Parent.IsListed;
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null) ResetAssetMenuStrip();

            retainControl.Asset = retainsToolStripMenuItem.Checked && e.Node != null ? (Asset)e.Node.Tag : null;
        }

        private void TabMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var p = tabControl.PointToClient(Cursor.Position);
            for (int i = 0; i < tabControl.TabCount; i++)
            {
                var r = tabControl.GetTabRect(i);
                if (r.Contains(p))
                {
                    tabControl.SelectedIndex = i;
                    return;
                }
            }
            e.Cancel = true;
        }

        private void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hoverIndex = GetHoverTabIndex();
                if (hoverIndex >= 0)
                {
                    tabControl.Tag = tabControl.TabPages[hoverIndex];
                }
            }
        }

        private void TabControl_MouseUp(object sender, MouseEventArgs e)
        {
            tabControl.Tag = null;
        }
        private void TabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && tabControl.Tag != null)
            {
                var clickedTab = (TabPage)tabControl.Tag;
                tabControl.DoDragDrop(clickedTab, DragDropEffects.All);
            }
        }
        private void TabControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TabPage)) != null)
            {
                var dragTab = (TabPage)e.Data.GetData(typeof(TabPage));
                var dragTabIndex = tabControl.TabPages.IndexOf(dragTab);

                var hoverTabIndex = GetHoverTabIndex();
                if (hoverTabIndex < 0)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
                var hoverTab = tabControl.TabPages[hoverTabIndex];
                e.Effect = DragDropEffects.Move;

                if (dragTab == hoverTab) return;

                var dragTabRect = tabControl.GetTabRect(dragTabIndex);
                var hoverTabRect = tabControl.GetTabRect(hoverTabIndex);

                if (dragTabRect.Width < hoverTabRect.Width)
                {
                    var tcLocation = tabControl.PointToScreen(tabControl.Location);

                    if (dragTabIndex < hoverTabIndex)
                    {
                        if ((e.X - tcLocation.X) > ((hoverTabRect.X + hoverTabRect.Width) - dragTabRect.Width))
                            SwapTabPages(dragTab, hoverTab);
                    }
                    else if (dragTabIndex > hoverTabIndex)
                    {
                        if ((e.X - tcLocation.X) < (hoverTabRect.X + dragTabRect.Width))
                            SwapTabPages(dragTab, hoverTab);
                    }
                }
                else SwapTabPages(dragTab, hoverTab);

                tabControl.SelectedIndex = tabControl.TabPages.IndexOf(dragTab);
            }
        }

        private int GetHoverTabIndex()
        {
            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                if (tabControl.GetTabRect(i).Contains(tabControl.PointToClient(Cursor.Position)))
                    return i;
            }
            return -1;
        }

        private void SwapTabPages(TabPage src, TabPage dst)
        {
            var srcIndex = tabControl.TabPages.IndexOf(src);
            var destIndex = tabControl.TabPages.IndexOf(dst);
            tabControl.TabPages[destIndex] = src;
            tabControl.TabPages[srcIndex] = dst;
            tabControl.Refresh();
        }

        private void BrowserVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            browserVisibleToolStripMenuItem.Checked = !browserVisibleToolStripMenuItem.Checked;
        }

        private void BrowserVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer.Panel1Collapsed = !browserVisibleToolStripMenuItem.Checked;
        }

        private Color NodeColor(Asset asset)
        {
            if (!asset.Linked) return Color.Red;
            if (retainsToolStripMenuItem.Checked && asset.IsRetained(false, false)) return asset.IsUnused ? Color.Orange : Color.Green;
            return Color.Empty;
        }

        private void RetainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var showRetain = !retainsToolStripMenuItem.Checked;

            retainsToolStripMenuItem.Checked = showRetain;

            foreach (var node in treeNodes.Values)
            {
                var asset = (Asset)node.Tag;

                node.ForeColor = NodeColor(asset);
            }

            navSplitContainer.Panel2Collapsed = !showRetain;

            retainControl.Asset = showRetain && treeView.SelectedNode != null ? (Asset)treeView.SelectedNode.Tag : null;
        }

        private void LevelDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNodes.Count != 0)
            {
                var type = (AssetType)((ToolStripMenuItem)sender).Tag;

                Asset.LevelDown(type, SelectedAssets);
            }
        }

        private void LevelUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                var asset = (Asset)treeView.SelectedNode.Tag;

                asset.LevelUp();
            }
        }

        private void DeveloperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetManager.Instance.IsDeveloper = developerToolStripMenuItem.Checked = !developerToolStripMenuItem.Checked;
        }

        private void TreeView_Enter(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = IsSaveEnabled;
        }

        private void TreeView_Leave(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = IsSaveEnabled;
        }

        private void RunApp(int profile)
        {
            var run = false;

            foreach (var project in AssetManager.Instance.Projects)
            {
                var execPath = project.ExecutablePath;

                if (execPath != null)
                {
                    run = true;

                    var execFileName = Path.GetFileName(execPath);
                    var execDirPath = Path.GetDirectoryName(execPath);

                    var args = new StringBuilder();

                    try
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo.UseShellExecute = true;
                            process.StartInfo.WorkingDirectory = execDirPath;
                            process.StartInfo.FileName = execFileName;
                            if (profile >= 0) process.StartInfo.Arguments = profile.ToString();
                            if (!process.Start())
                            {
                                MessageBox.Show(string.Format("{0} 을 실행할 수 없습니다.", project.Name));
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show(string.Format("{0} 을 실행할 수 없습니다.", project.Name));
                    }
                }
            }
            if (!run)
            {
                MessageBox.Show("실행 가능한 프로젝트가 없습니다.");
            }
        }

        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunApp(-1);
        }

        private void Run0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunApp(0);
        }

        private void Run1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunApp(1);
        }

        private void Run2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunApp(2);
        }

        private void Run3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunApp(3);
        }

        private void ToolToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var localeItems = new List<string>(localeToolStripMenuItem.DropDownItems.Count);
            foreach (ToolStripMenuItem item in localeToolStripMenuItem.DropDownItems)
            {
                localeItems.Add((string)item.Tag);
            }
            if (!localeItems.SequenceEqual(AssetManager.Instance.Config.Locales))
            {
                localeToolStripMenuItem.DropDownItems.Clear();

                foreach (var locale in AssetManager.Instance.Config.Locales)
                {
                    var localeSubToolStripMenuItem = new ToolStripMenuItem
                    {
                        Size = new Size(53, 20),
                        Text = locale,
                        Tag = locale
                    };
                    if (AssetManager.Instance.Locale == locale)
                    {
                        localeSubToolStripMenuItem.Checked = true;
                    }
                    localeSubToolStripMenuItem.Click += LocaleSubToolStripMenuItem_Click;
                    localeToolStripMenuItem.DropDownItems.Add(localeSubToolStripMenuItem);
                }
            }
        }

        private void OptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO:REMOVE
        }

    }
}

