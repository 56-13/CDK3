using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    public enum ImageSelectFormType
    {
        Both,
        RootImageOnly,
        SubImageOnly
    };

    partial class ImageSelectForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => treeView.RootAsset = value;
            get => treeView.RootAsset;
        }

        private ImageSelectFormType _Type;
        public ImageSelectFormType Type
        {
            set
            {
                if (_Type != value)
                {
                    _Type = value;

                    switch (_Type)
                    {
                        case ImageSelectFormType.Both:
                            treeView.Types = new AssetType[] { AssetType.RootImage, AssetType.SubImage };
                            break;
                        case ImageSelectFormType.RootImageOnly:
                            treeView.Types = new AssetType[] { AssetType.RootImage };
                            break;
                        case ImageSelectFormType.SubImageOnly:
                            treeView.Types = new AssetType[] { AssetType.SubImage };
                            break;
                    }
                }
            }
            get => _Type;
        }

        public int Depth
        {
            set => treeView.Depth = value;
            get => treeView.Depth;
        }
        public bool Multiselect
        {
            set => treeView.Multiselect = value;
            get => treeView.Multiselect;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAsset[] SelectedImages
        {
            set
            {
                Asset[] assets = null;
                if (value != null)
                {
                    assets = new Asset[value.Length];
                    Array.Copy(value, assets, value.Length);
                }
                treeView.SelectedAssets = assets;
            }
            get
            {
                var value = treeView.SelectedAssets;
                var images = new ImageAsset[value.Length];
                Array.Copy(value, images, value.Length);
                return images;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageAsset SelectedImage
        {
            set => treeView.SelectedAsset = value;
            get => (ImageAsset)treeView.SelectedAsset;
        }

        public ImageSelectForm()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedAssetChanged(object sender, EventArgs e)
        {
            screenControl.Image = (ImageAsset)treeView.SelectedAsset;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (treeView.Multiselect ? treeView.SelectedAssets.Length != 0 : treeView.SelectedAsset != null)
            {
                DialogResult = DialogResult.OK;

                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void TreeView_Confirm(object sender, EventArgs e)
        {
            OkButton_Click(this, EventArgs.Empty);
        }
    }
}