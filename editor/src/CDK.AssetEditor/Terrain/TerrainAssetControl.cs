using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

namespace CDK.Assets.Terrain
{
    public partial class TerrainAssetControl : UserControl
    {
        private TerrainAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        widthUpDown.DataBindings.Clear();
                        heightUpDown.DataBindings.Clear();
                        altitudeUpDown.DataBindings.Clear();
                        vertexCellUpDown.DataBindings.Clear();
                        surfaceCellUpDown.DataBindings.Clear();
                        gridUpDown.DataBindings.Clear();

                        ambientOcclusionIntensityControl.DataBindings.Clear();
                    }

                    _Asset = value;

                    if (_Asset != null)
                    {
                        widthUpDown.DataBindings.Add("Value", Asset, "Width", false, DataSourceUpdateMode.Never);
                        heightUpDown.DataBindings.Add("Value", Asset, "Height", false, DataSourceUpdateMode.Never);
                        altitudeUpDown.DataBindings.Add("Value", Asset, "Altitude", false, DataSourceUpdateMode.OnPropertyChanged);
                        vertexCellUpDown.DataBindings.Add("Value", Asset, "VertexCell", false, DataSourceUpdateMode.Never);
                        surfaceCellUpDown.DataBindings.Add("Value", Asset, "SurfaceCell", false, DataSourceUpdateMode.Never);
                        gridUpDown.DataBindings.Add("Value", Asset, "Grid", false, DataSourceUpdateMode.OnPropertyChanged);

                        ambientOcclusionIntensityControl.DataBindings.Add("Value", Asset, "AmbientOcclusionIntensity", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        public TerrainAssetControl()
        {
            InitializeComponent();

            resizeAlignComboBox.DataSource = Enum.GetValues(typeof(Align));
            resizeAlignComboBox.SelectedItem = Align.CenterMiddle;
        }

        private void SizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            resizeAlignComboBox.Visible =
            resizeCancelButton.Visible =
            resizeApplyButton.Visible = Asset != null &&
                (Asset.Width != widthUpDown.Value ||
                Asset.Height != heightUpDown.Value ||
                Asset.VertexCell != vertexCellUpDown.Value ||
                Asset.SurfaceCell != surfaceCellUpDown.Value);
        }

        private void ResizeCancelButton_Click(object sender, EventArgs e)
        {
            if (Asset != null)
            {
                widthUpDown.Value = Asset.Width;
                heightUpDown.Value = Asset.Height;
                vertexCellUpDown.Value = Asset.VertexCell;
                surfaceCellUpDown.Value = Asset.SurfaceCell;
            }
        }

        private void ResizeApplyButton_Click(object sender, EventArgs e)
        {
            Asset?.Resize((int)widthUpDown.Value,
                (int)heightUpDown.Value,
                (int)vertexCellUpDown.Value,
                (int)surfaceCellUpDown.Value,
                (Align)resizeAlignComboBox.SelectedItem);

            resizeAlignComboBox.Visible =
            resizeCancelButton.Visible =
            resizeApplyButton.Visible = false;
        }
    }
}
