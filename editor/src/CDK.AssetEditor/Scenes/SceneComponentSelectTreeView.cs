using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Scenes
{
    public partial class SceneComponentSelectTreeView : UserControl
    {
        private Scene _Scene;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene Scene
        {
            set
            {
                if (_Scene != value)
                {
                    _Scene = value;

                    Reset();

                    SceneChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Scene;
        }

        public event EventHandler SceneChanged;

        private SceneComponentType[] _Types;
        public SceneComponentType[] Types
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

        private SceneComponent _SelectedComponent;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneComponent SelectedComponent
        {
            set
            {
                if (_SelectedComponent != value)
                {
                    var node = value != null ? FindNode(value, true) : null;

                    if (node != null)
                    {
                        _SelectedComponent = TypeCheck(value) ? value : null;

                        treeView.SelectedNode = node;

                        node.EnsureVisible();
                    }
                    else
                    {
                        _SelectedComponent = null;

                        treeView.SelectedNode = null;
                    }
                    SelectedComponentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SelectedComponent;
        }

        private List<SceneComponent> _SelectedComponents;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SceneComponent[] SelectedComponents
        {
            set
            {
                if (value == null)
                {
                    if (_SelectedComponents.Count == 0) return;
                }
                else
                {
                    if (_SelectedComponents.Count == value.Length)
                    {
                        foreach (var obj in value)
                        {
                            if (!_SelectedComponents.Contains(obj)) return;
                        }
                    }
                }
                foreach (var comp in _SelectedComponents)
                {
                    var node = FindNode(comp, true);

                    if (node != null) node.Checked = false;
                }
                _SelectedComponents.Clear();
                if (value != null)
                {
                    foreach (var comp in value)
                    {
                        if (TypeCheck(comp))
                        {
                            var node = FindNode(comp, true);

                            if (node != null)
                            {
                                _SelectedComponents.Add(comp);

                                node.Checked = true;
                            }
                        }
                    }
                }
                SelectedComponentsChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                var comps = _SelectedComponents.ToArray();
                SortComponents(comps);
                return comps;
            }
        }

        public event EventHandler SelectedComponentChanged;
        public event EventHandler SelectedComponentsChanged;
        public event TreeViewEventHandler Collapse;
        public event TreeViewEventHandler Expand;
        public event EventHandler Confirm;

        private Dictionary<SceneComponent, TreeNode> treeNodes;

        public SceneComponentSelectTreeView()
        {
            InitializeComponent();

            treeNodes = new Dictionary<SceneComponent, TreeNode>();

            _SelectedComponents = new List<SceneComponent>();

            treeView.ImageList = AssetControl.Instance?.SmallImageList;
        }

        private void Reset()
        {
            while (treeView.Nodes.Count != 0) RemoveNode(treeView.Nodes[0]);

            if (_Scene != null)
            {
                TreeNode node;

                foreach(var child in _Scene.Children)
                {
                    node = CreateNode(child);
                    if (node != null) treeView.Nodes.Add(node);
                }

                if (_SelectedComponent != null)
                {
                    if (!TypeCheck(_SelectedComponent) || (node = FindNode(_SelectedComponent, true)) == null)
                    {
                        _SelectedComponent = null;

                        SelectedComponentChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else treeView.SelectedNode = node;
                }
                if (_SelectedComponents.Count != 0)
                {
                    bool flag = false;

                    int i = 0;

                    while (i < _SelectedComponents.Count)
                    {
                        if (!TypeCheck(_SelectedComponents[i]) || (node = FindNode(_SelectedComponents[i], true)) == null)
                        {
                            flag = true;
                            _SelectedComponents.RemoveAt(i);
                        }
                        else
                        {
                            node.Checked = true;
                            i++;
                        }
                    }
                    if (flag) SelectedComponentsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool TypeCheck(SceneComponent comp)
        {
            return (_Types == null || Array.IndexOf(_Types, comp.Type) >= 0);
        }

        private bool TypeCheckAll(SceneComponent comp)
        {
            if (TypeCheck(comp)) return true;
            foreach (var child in comp.Children)
            {
                if (TypeCheckAll(child)) return true;
            }
            return false;
        }

        private TreeNode CreateNode(SceneComponent comp)
        {
            if (!TypeCheckAll(comp)) return null;

            var node = new TreeNode
            {
                Text = comp.Name,
                Tag = comp
            };
            node.SelectedImageKey = node.ImageKey = comp.Type.ToString();

            if (Multiselect && _SelectedComponents.Contains(comp)) node.Checked = true;

            treeNodes.Add(comp, node);

            comp.Children.ListChanged += Objects_ListChanged;

            if (comp.Children.Count != 0) node.Nodes.Add(new TreeNode());

            return node;
        }

        private void RemoveNode(TreeNode node)
        {
            while (node.Nodes.Count != 0) RemoveNode(node.Nodes[0]);
            
            var comp = (SceneComponent)node.Tag;

            if (comp != null)
            {
                comp.Children.ListChanged -= Objects_ListChanged;
                
                treeNodes.Remove(comp);
            }
            node.Remove();
        }

        public TreeNode FindNode(SceneComponent comp, bool expand)
        {
            if (treeNodes.TryGetValue(comp, out var node)) return node;

            if (expand && comp.Parent is SceneComponent parent)
            {
                node = FindNode(parent, true);

                if (node != null)
                {
                    node.Expand();

                    ExpandTreeNode(node);

                    if (treeNodes.TryGetValue(comp, out node)) return node;
                }
            }
            return null;
        }

        private void Objects_ListChanged(object sender, ListChangedEventArgs e)
        {
            var comp = (SceneComponent)((AssetElementList<SceneComponent>)sender).Parent;

            if (treeNodes.ContainsKey(comp))
            {
                var node = treeNodes[comp];

                if (node.IsExpanded)
                {
                    if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null)
                    {
                        if (e.PropertyDescriptor.Name.Equals("Name"))
                        {
                            foreach (TreeNode subnode in node.Nodes)
                            {
                                if (subnode.Tag == comp.Children[e.NewIndex])
                                {
                                    subnode.Text = comp.Children[e.NewIndex].Name;
                                    break;
                                }
                            }
                        }
                        return;
                    }
                    int i = 0;
                    foreach (var child in comp.Children)
                    {
                        if (TypeCheckAll(child))
                        {
                            bool flag = true;
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
                                var subnode = CreateNode(child);

                                if (subnode != null)
                                {
                                    node.Nodes.Insert(i, subnode);
                                }
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
                        foreach (var child in comp.Children)
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
                        var comp = (SceneComponent)e.Node?.Tag;

                        if (comp != null && !TypeCheck(comp)) comp = null;

                        if (_SelectedComponent != comp)
                        {
                            _SelectedComponent = comp;

                            SelectedComponentChanged?.Invoke(this, EventArgs.Empty);
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
                    CheckObjects((SceneComponent)e.Node.Tag, e.Node.Checked);

                    SelectedComponentsChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        private void CheckObjects(SceneComponent comp, bool check)
        {
            if (TypeCheck(comp))
            {
                if (check)
                {
                    if (!_SelectedComponents.Contains(comp)) _SelectedComponents.Add(comp);
                }
                else _SelectedComponents.Remove(comp);
            }
            
            if (treeNodes.TryGetValue(comp, out var node)) node.Checked = check;

            foreach (var child in comp.Children) CheckObjects(child, check);
        }

        private void TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!Multiselect && _SelectedComponent != null && Confirm != null)
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

            var comp = (SceneComponent)node.Tag;

            while (node.Nodes.Count != 0) RemoveNode(node.Nodes[0]);

            foreach (var child in comp.Children)
            {
                var subnode = CreateNode(child);

                if (subnode != null) node.Nodes.Add(subnode);
            }
            Expand?.Invoke(this, new TreeViewEventArgs(node));
        }

        private static void GetObjectIndices(SceneComponent a, List<int> indices)
        {
            if (a.Parent is SceneComponent parent)
            {
                GetObjectIndices(parent, indices);
                indices.Add(parent.Children.IndexOf(a));
            }
        }

        private static int CompareObjectIndices(List<int> indices0, List<int> indices1)
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

        private static void SortComponents(SceneComponent[] objs)
        {
            if (objs.Length > 1)
            {
                var indices = new List<int>[objs.Length];
                for (var i = 0; i < objs.Length; i++)
                {
                    indices[i] = new List<int>();
                    GetObjectIndices(objs[i], indices[i]);
                }
                bool flag;
                do
                {
                    flag = false;
                    for (var i = 0; i < objs.Length - 1; i++)
                    {
                        if (CompareObjectIndices(indices[i], indices[i + 1]) == -1)
                        {
                            {
                                var temp = indices[i];
                                indices[i] = indices[i + 1];
                                indices[i + 1] = temp;
                            }
                            {
                                var temp = objs[i];
                                objs[i] = objs[i + 1];
                                objs[i + 1] = temp;
                            }
                            flag = true;
                        }
                    }
                } while (flag);
            }
        }
    }
}
