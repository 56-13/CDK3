using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class AssetSelectControl : UserControl
    {
        private Asset _SelectedAsset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset SelectedAsset
        {
            set
            {
                if (_SelectedAsset != value)
                {
                    if (_SelectedAsset != null)
                    {
                        textBox.DataBindings.Clear();
                    }

                    _SelectedAsset = value;

                    if (_SelectedAsset != null)
                    {
                        textBox.DataBindings.Add("Text", _SelectedAsset, "TagName", false, DataSourceUpdateMode.Never);
                    }
                    else
                    {
                        textBox.Text = string.Empty;
                    }

                    SelectedAssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _SelectedAsset;
        }
        public event EventHandler SelectedAssetChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset RootAsset
        {
            set => _selectForm.RootAsset = value;
            get => _selectForm.RootAsset;
        }

        [DefaultValue(-1)]
        public int Depth
        {
            set => _selectForm.Depth = value;
            get => _selectForm.Depth;
        }

        [DefaultValue(null)]
        public AssetType[] Types
        {
            set => _selectForm.Types = value;
            get => _selectForm.Types;
        }

        public event EventHandler<AssetSelectImportEventArgs> Importing;

        private AssetSelectViewForm _selectForm;

        public AssetSelectControl()
        {
            InitializeComponent();

            _selectForm = new AssetSelectViewForm();

            Disposed += AssetSelectControl_Disposed;
        }

        private void AssetSelectControl_Disposed(object sender, EventArgs e)
        {
            _selectForm.Dispose();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            _selectForm.SelectedAsset = _SelectedAsset;

            if (_selectForm.ShowDialog(this) == DialogResult.OK)
            {
                var asset = _selectForm.SelectedAsset;

                if (asset != null)
                {
                    if (Importing != null)
                    {
                        var se = new AssetSelectImportEventArgs(asset);

                        Importing.Invoke(this, se);

                        if (!se.Cancel) SelectedAsset = asset;
                    }
                    else SelectedAsset = asset;
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            SelectedAsset = null;
        }

        private int GetDepth(Asset asset)
        {
            if (asset == RootAsset) return 0;

            var parent = asset.Parent;

            if (parent == null) return 0;

            var depth = GetDepth(parent);

            if (parent.IsListed) depth++;

            return depth;
        }

        private bool TypeCheck(Asset asset)
        {
            var types = Types;
            var depth = Depth;

            return (types == null || Array.IndexOf(types, asset.Type) >= 0) && (depth < 0 || GetDepth(asset) == depth);
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            var key = (string)e.Data.GetData(DataFormats.Text);

            if (key != null)
            {
                var asset = AssetManager.Instance.GetAsset(key);

                if (asset != null && TypeCheck(asset))
                {
                    e.Effect = DragDropEffects.Link;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void TextBox_DragDrop(object sender, DragEventArgs e)
        {
            var key = (string)e.Data.GetData(DataFormats.Text);

            if (key != null)
            {
                var asset = AssetManager.Instance.GetAsset(key);

                if (asset != null && TypeCheck(asset)) SelectedAsset = asset;
            }
        }
    }

    public class AssetSelectImportEventArgs : EventArgs
    {
        public Asset Asset { private set; get; }
        public bool Cancel { set; get; }

        public AssetSelectImportEventArgs(Asset asset)
        {
            Asset = asset;
        }
    }
}
