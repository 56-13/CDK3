using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class AssetSelectViewControl : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => treeView.RootAsset = value;
            get => treeView.RootAsset;
        }
        public event EventHandler RootAssetChanged;

        [DefaultValue(-1)]
        public int Depth
        {
            set => treeView.Depth = value;
            get => treeView.Depth;
        }
        public event EventHandler DepthChanged;

        [DefaultValue(null)]
        public AssetType[] Types
        {
            set => treeView.Types = value;
            get => treeView.Types;
        }
        public event EventHandler TypesChanged;

        [DefaultValue(false)]
        public bool Multiselect
        {
            set => treeView.Multiselect = value;
            get => treeView.Multiselect;
        }
        public event EventHandler MultiselectChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset SelectedAsset
        {
            set => treeView.SelectedAsset = value;
            get => treeView.SelectedAsset;
        }
        public event EventHandler SelectedAssetChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset[] SelectedAssets
        {
            set => treeView.SelectedAssets = value;
            get => treeView.SelectedAssets;
        }
        public event EventHandler SelectedAssetsChanged;

        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            set => splitContainer.Orientation = value;
            get => splitContainer.Orientation;
        }

        public int SplitterDistance
        {
            set => splitContainer.SplitterDistance = value;
            get => splitContainer.SplitterDistance;
        }

        [DefaultValue(false)]
        public bool AllowDrag
        {
            set => treeView.AllowDrag = value;
            get => treeView.AllowDrag;
        }

        [DefaultValue(false)]
        public bool FitScreen
        {
            set => screenControl.FitScreen = value;
            get => screenControl.FitScreen;
        }

        public AssetSelectViewControl()
        {
            InitializeComponent();

            treeView.RootAssetChanged += TreeView_RootAssetChanged;
            treeView.DepthChanged += TreeView_DepthChanged;
            treeView.TypesChanged += TreeView_TypesChanged;
            treeView.MultiselectChanged += TreeView_MultiselectChanged;
            treeView.SelectedAssetChanged += TreeView_SelectedAssetChanged;
            treeView.SelectedAssetsChanged += TreeView_SelectedAssetsChanged;
        }

        private void TreeView_RootAssetChanged(object sender, EventArgs e)
        {
            RootAssetChanged?.Invoke(this, e);
        }

        private void TreeView_DepthChanged(object sender, EventArgs e)
        {
            DepthChanged?.Invoke(this, e);
        }

        private void TreeView_TypesChanged(object sender, EventArgs e)
        {
            TypesChanged?.Invoke(this, e);
        }

        private void TreeView_MultiselectChanged(object sender, EventArgs e)
        {
            MultiselectChanged?.Invoke(this, e);
        }

        private void TreeView_SelectedAssetChanged(object sender, EventArgs e)
        {
            var scene = treeView.SelectedAsset?.NewScene();

            if (scene != null)
            {
                scene.CameraGizmo = !FitScreen;
                scene.Mode = Scenes.SceneMode.Preview;
            }
            screenControl.Scene = scene;

            SelectedAssetChanged?.Invoke(this, e);
        }

        private void TreeView_SelectedAssetsChanged(object sender, EventArgs e)
        {
            SelectedAssetsChanged?.Invoke(this, e);
        }
    }
}
