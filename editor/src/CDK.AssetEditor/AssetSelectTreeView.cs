using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace CDK.Assets
{
    public partial class AssetSelectTreeView : UserControl
    {
        private Asset _RootAsset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set
            {
                if (_RootAsset != value)
                {
                    if (_RootAsset != null)
                    {
                        _RootAsset.PropertyChanged -= RootAsset_PropertyChanged;
                    }

                    _RootAsset = value;

                    if (_RootAsset != null)
                    {
                        _RootAsset.PropertyChanged += RootAsset_PropertyChanged;
                    }
                    Reset();

                    RootAssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _RootAsset;
        }

        public event EventHandler RootAssetChanged;

        private int _Depth;

        [DefaultValue(-1)]
        public int Depth
        {
            set
            {
                if (_Depth != value)
                {
                    _Depth = value;

                    Reset();

                    DepthChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Depth;
        }

        public event EventHandler DepthChanged;

        private AssetType[] _Types;

        [DefaultValue(null)]
        public AssetType[] Types
        {
            set
            {
                if (_Types != null ? (value == null || !_Types.SequenceEqual(value)) : value != null) 
                {
                    _Types = value;

                    Reset();

                    TypesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Types;
        }

        public event EventHandler TypesChanged;

        [DefaultValue(false)]
        public bool Multiselect
        {
            set
            {
                if (treeView.CheckBoxes != value)
                {
                    treeView.CheckBoxes = value;

                    MultiselectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => treeView.CheckBoxes;
        }

        public event EventHandler MultiselectChanged;

        private Asset _SelectedAsset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset SelectedAsset
        {
            set
            {
                if (_SelectedAsset != value)
                {
                    var node = value != null ? FindNode(value, true) : null;

                    if (node != null)
                    {
                        _SelectedAsset = TypeCheck(value) ? value : null;

                        treeView.SelectedNode = node;

                        node.EnsureVisible();
                    }
                    else
                    {
                        _SelectedAsset = null;

                        treeView.SelectedNode = null;
                    }
                    SelectedAssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SelectedAsset;
        }

        private List<Asset> _SelectedAssets;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset[] SelectedAssets
        {
            set
            {
                if (value == null)
                {
                    if (_SelectedAssets.Count == 0) return;
                }
                else
                {
                    if (_SelectedAssets.Count == value.Length)
                    {
                        foreach (var asset in value)
                        {
                            if (!_SelectedAssets.Contains(asset)) return;
                        }
                    }
                }
                foreach (var asset in _SelectedAssets)
                {
                    var node = FindNode(asset, true);

                    if (node != null) node.Checked = false;
                }
                _SelectedAssets.Clear();
                if (value != null)
                {
                    foreach (var asset in value)
                    {
                        if (TypeCheck(asset))
                        {
                            var node = FindNode(asset, true);

                            if (node != null)
                            {
                                _SelectedAssets.Add(asset);

                                node.Checked = true;
                            }
                        }
                    }
                }
                SelectedAssetsChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                var assets = _SelectedAssets.ToArray();
                SortAssets(assets);
                return assets;
            }
        }

        [DefaultValue(false)]
        public bool AllowDrag { set; get; }

        public event EventHandler SelectedAssetChanged;
        public event EventHandler SelectedAssetsChanged;
        public event TreeViewEventHandler Collapse;
        public event TreeViewEventHandler Expand;
        public event EventHandler Confirm;

        private Dictionary<Asset, TreeNode> treeNodes;

        public AssetSelectTreeView()
        {
            InitializeComponent();

            treeNodes = new Dictionary<Asset, TreeNode>();

            _SelectedAssets = new List<Asset>();

            _Depth = -1;

            treeView.ImageList = AssetControl.Instance?.SmallImageList;

            Disposed += AssetSelectTreeView_Disposed;
        }

        private void AssetSelectTreeView_Disposed(object sender, EventArgs e)
        {
            if (_RootAsset != null)
            {
                _RootAsset.PropertyChanged -= RootAsset_PropertyChanged;
            }
        }

        private void RootAsset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("TagName") && treeView.Nodes.Count != 0)
            {
                treeView.Nodes[0].Text = _RootAsset.TagName;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (treeView.Nodes.Count == 0) Reset();
        }

        private void Reset()
        {
            while (treeView.Nodes.Count != 0) RemoveNode(treeView.Nodes[0]);

            TreeNode node;

            if (_RootAsset != null)
            {
                node = CreateNode(_RootAsset, 0);

                if (node != null) treeView.Nodes.Add(node);
            }
            else if (AssetManager.IsCreated && _Depth < 0)
            {
                foreach (var project in AssetManager.Instance.Projects)
                {
                    node = CreateNode(project, 0);

                    if (node != null) treeView.Nodes.Add(node);
                }
            }
            else return;

            if (_SelectedAsset != null)
            {
                if (!TypeCheck(_SelectedAsset) || (node = FindNode(_SelectedAsset, true)) == null)
                {
                    _SelectedAsset = null;

                    SelectedAssetChanged?.Invoke(this, EventArgs.Empty);
                }
                else treeView.SelectedNode = node;
            }
            if (_SelectedAssets.Count != 0)
            {
                var flag = false;

                var i = 0;

                while (i < _SelectedAssets.Count)
                {
                    if (!TypeCheck(_SelectedAssets[i]) || (node = FindNode(_SelectedAssets[i], true)) == null)
                    {
                        flag = true;
                        _SelectedAssets.RemoveAt(i);
                    }
                    else
                    {
                        node.Checked = true;
                        i++;
                    }
                }
                if (flag) SelectedAssetsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool TypeCheck(Asset asset)
        {
            return (_Types == null || Array.IndexOf(_Types, asset.Type) >= 0) && (_Depth < 0 || GetDepth(asset) == _Depth);
        }

        private bool TypeCheckAll(Asset asset)
        {
            if (TypeCheck(asset)) return true;

            foreach (var child in asset.Children)
            {
                if (TypeCheckAll(child)) return true;
            }
            return false;
        }

        private TreeNode CreateNode(Asset asset, int depth)
        {
            if (!TypeCheckAll(asset)) return null;

            var node = new TreeNode
            {
                Text = asset.TagName,
                Tag = asset
            };
            node.SelectedImageKey = node.ImageKey = asset.Type.ToString();
            if (!asset.Linked) node.ForeColor = Color.Red;

            if (Multiselect && _SelectedAssets.Contains(asset)) node.Checked = true;

            treeNodes.Add(asset, node);

            if (_Depth < 0 || depth < _Depth)
            {
                asset.Children.ListChanged += Children_ListChanged;

                if (asset.IsListed) depth++;

                if (asset.Children.Count != 0) node.Nodes.Add(new TreeNode());
            }
            return node;
        }

        private void RemoveNode(TreeNode node)
        {
            while (node.Nodes.Count != 0) RemoveNode(node.Nodes[0]);

            var asset = (Asset)node.Tag;

            if (asset != null)
            {
                asset.Children.ListChanged -= Children_ListChanged;
                
                treeNodes.Remove(asset);
            }
            node.Remove();
        }

        private int GetDepth(Asset asset)
        {
            if (asset == _RootAsset) return 0;

            var parent = asset.Parent;

            if (parent == null) return 0;

            var depth = GetDepth(parent);

            if (parent.IsListed) depth++;

            return depth;
        }

        public TreeNode FindNode(Asset asset, bool expand)
        {
            if (treeNodes.TryGetValue(asset, out var node)) return node;

            if (expand && asset.Parent != null)
            {
                node = FindNode(asset.Parent, true);

                if (node != null)
                {
                    node.Expand();

                    ExpandTreeNode(node);

                    if (treeNodes.TryGetValue(asset, out node)) return node;
                }
            }
            return null;
        }

        public List<TreeNode> AllNodes => new List<TreeNode>(treeNodes.Values);

        public List<Asset> AllAssets => new List<Asset>(treeNodes.Keys);

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            var asset = ((AssetElementList<Asset>)sender).Parent.Owner;

            if (treeNodes.ContainsKey(asset))
            {
                var node = treeNodes[asset];

                if (node.IsExpanded)
                {
                    if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null)
                    {
                        if (e.PropertyDescriptor.Name.Equals("TagName"))
                        {
                            foreach (TreeNode subnode in node.Nodes)
                            {
                                if (subnode.Tag == asset.Children[e.NewIndex])
                                {
                                    subnode.Text = asset.Children[e.NewIndex].TagName;
                                    break;
                                }
                            }
                        }
                        return;
                    }
                    var depth = GetDepth(asset);

                    var i = 0;
                    foreach (var child in asset.Children)
                    {
                        if (TypeCheckAll(child))
                        {
                            var flag = true;
                            for (int j = 0; j < node.Nodes.Count; j++)
                            {
                                var subnode = node.Nodes[j];

                                if (subnode.Tag == child)
                                {
                                    flag = false;
                                    if (i != j)
                                    {
                                        subnode.Remove();
                                        node.Nodes.Insert(i, subnode);
                                    }
                                    break;
                                }
                            }
                            if (flag)
                            {
                                var subnode = CreateNode(child, depth);

                                if (subnode != null) node.Nodes.Insert(i, subnode);
                            }
                            i++;
                        }
                    }
                    while (i < node.Nodes.Count) RemoveNode(node.Nodes[node.Nodes.Count - 1]);
                }
                else
                {
                    if (e.PropertyDescriptor == null)
                    {
                        var flag = false;
                        foreach (var child in asset.Children)
                        {
                            if (TypeCheckAll(child))
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag)
                        {
                            if (node.Nodes.Count == 0) node.Nodes.Add(new TreeNode());
                        }
                        else node.Nodes.Clear();
                    }
                }
            }
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Action)
            {
                case TreeViewAction.ByMouse:
                case TreeViewAction.ByKeyboard:
                    {
                        var asset = (Asset)e.Node?.Tag;

                        if (asset != null && !TypeCheck(asset)) asset = null;

                        if (_SelectedAsset != asset)
                        {
                            _SelectedAsset = asset;

                            SelectedAssetChanged?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    break;
            }
        }

        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            switch (e.Action)
            {
                case TreeViewAction.ByMouse:
                case TreeViewAction.ByKeyboard:
                    CheckAssets((Asset)e.Node.Tag, e.Node.Checked);

                    SelectedAssetsChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        private void CheckAssets(Asset asset, bool check)
        {
            if (TypeCheck(asset))
            {
                if (check)
                {
                    if (!_SelectedAssets.Contains(asset)) _SelectedAssets.Add(asset);
                }
                else _SelectedAssets.Remove(asset);
            }
            
            if (treeNodes.TryGetValue(asset, out var node)) node.Checked = check;

            foreach (var child in asset.Children)
            {
                CheckAssets(child, check);
            }
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!Multiselect && _SelectedAsset != null && Confirm != null)
            {
                Confirm(this, EventArgs.Empty);
            }
        }

        private void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count != 0)
            {
                while (e.Node.Nodes.Count != 0) RemoveNode(e.Node.Nodes[0]);

                e.Node.Nodes.Add(new TreeNode());

                Collapse?.Invoke(this, e);
            }
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ExpandTreeNode(e.Node);
        }

        private void ExpandTreeNode(TreeNode node)
        {
            if (node.Nodes.Count != 0 && node.Nodes[0].Tag != null) return;

            var asset = (Asset)node.Tag;

            var depth = GetDepth(asset);

            while (node.Nodes.Count != 0) RemoveNode(node.Nodes[0]);

            foreach (var child in asset.Children)
            {
                var subnode = CreateNode(child, depth);

                if (subnode != null) node.Nodes.Add(subnode);
            }
            Expand?.Invoke(this, new TreeViewEventArgs(node));
        }

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (AllowDrag)
            {
                var asset = (Asset)((TreeNode)e.Item).Tag;

                if (TypeCheck(asset)) treeView.DoDragDrop(asset.Key, DragDropEffects.Copy);
            }
        }

        private static void GetAssetIndices(Asset a, List<int> indices)
        {
            if (a.Parent != null)
            {
                GetAssetIndices(a.Parent, indices);
                indices.Add(a.Index);
            }
        }

        private static int CompareAssetIndices(List<int> indices0, List<int> indices1)
        {
            var count = Math.Min(indices0.Count, indices1.Count);

            for (var i = 0; i < count; i++)
            {
                if (indices0[i] < indices1[i]) return 1;
                if (indices0[i] > indices1[i]) return -1;
            }
            if (indices0.Count < indices1.Count) return 1;
            if (indices0.Count > indices1.Count) return -1;
            return 0;
        }

        private static void SortAssets(Asset[] assets)
        {
            if (assets.Length > 1)
            {
                var indices = new List<int>[assets.Length];
                for (var i = 0; i < assets.Length; i++)
                {
                    indices[i] = new List<int>();
                    GetAssetIndices(assets[i], indices[i]);
                }
                bool flag;
                do
                {
                    flag = false;
                    for (var i = 0; i < assets.Length - 1; i++)
                    {
                        if (CompareAssetIndices(indices[i], indices[i + 1]) == -1)
                        {
                            {
                                var temp = indices[i];
                                indices[i] = indices[i + 1];
                                indices[i + 1] = temp;
                            }
                            {
                                var temp = assets[i];
                                assets[i] = assets[i + 1];
                                assets[i + 1] = temp;
                            }
                            flag = true;
                        }
                    }
                } while (flag);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                Reset();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
