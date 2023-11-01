using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    public partial class SkyboxAssetControl : UserControl
    {
        private SkyboxAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SkyboxAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        encodingComboBox.DataBindings.Clear();
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        encodingComboBox.DataBindings.Add("SelectedItem", _Asset, "Encoding", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        public SkyboxAssetControl()
        {
            InitializeComponent();

            var encodings = Enum.GetValues(typeof(SkyboxAssetEncoding));

            encodingComboBox.DataSource = encodings;
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                openFileDialog.Filter = FileFilters.Image;

                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.Import(openFileDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                    }
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null) _Asset.Content.Bitmap = null;
        }
    }
}
