using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets
{
    public partial class AssetRetainControl : UserControl
    {
        private Asset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Asset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    _Asset = value;

                    ResetListBox();

                    Invalidate();

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        public AssetRetainControl()
        {
            InitializeComponent();
        }

        private void ResetListBox()
        {
            listBox.BeginUpdate();

            listBox.Items.Clear();

            if (_Asset != null)
            {
                var assets = fromRadioButton.Checked ? _Asset.GetRetainOrigins() : _Asset.GetRetainTargets();

                foreach (Asset asset in assets)
                {
                    listBox.Items.Add(asset);
                }
            }

            listBox.EndUpdate();
        }

        private void FromRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                ResetListBox();
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                ResetListBox();
            }
        }

        private void ListBox_DoubleClick(object sender, EventArgs e)
        {
            var asset = (Asset)listBox.SelectedItem; 

            if (asset != null)
            {
                AssetManager.Instance.Open(asset);
            }
        }
    }
}
