using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class AssetSelectViewForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => treeView.RootAsset = value;
            get => treeView.RootAsset;
        }

        [DefaultValue(-1)]
        public int Depth
        {
            set => treeView.Depth = value;
            get => treeView.Depth;
        }

        public AssetType[] Types
        {
            set => treeView.Types = value;
            get => treeView.Types;
        }

        [DefaultValue(false)]
        public bool Multiselect
        {
            set => treeView.Multiselect = value;
            get => treeView.Multiselect;
        }

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

        public AssetSelectViewForm()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedAssetChanged(object sender, EventArgs e)
        {
            var scene = treeView.SelectedAsset?.NewScene();

            if (scene != null) scene.Mode = Scenes.SceneMode.Preview;

            screenControl.Scene = scene;

            SelectedAssetChanged?.Invoke(this, e);
        }

        private void TreeView_SelectedAssetsChanged(object sender, EventArgs e)
        {
            SelectedAssetsChanged?.Invoke(this, e);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }
    }
}
