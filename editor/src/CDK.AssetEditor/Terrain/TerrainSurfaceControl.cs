using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Texturing;

namespace CDK.Assets.Terrain
{
    public partial class TerrainSurfaceControl : UserControl
    {
        private TerrainSurfaceComponent _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TerrainSurfaceComponent Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        sizeControl.DataBindings.Clear();
                        degreeControl.DataBindings.Clear();
                        attenuationControl.DataBindings.Clear();
                        shadowControl.DataBindings.Clear();
                        shadowAttenuationControl.DataBindings.Clear();
                        modeComboBox.DataBindings.Clear();
                        selectControl.Asset = null;
                        selectControl.DataBindings.Clear();
                        originControl.Asset = null;
                        originControl.DataBindings.Clear();
                        materialControl.DataBindings.Clear();
                    }

                    _Object = value;
                    
                    if (_Object != null)
                    {
                        sizeControl.DataBindings.Add("Value", _Object, "Size", false, DataSourceUpdateMode.OnPropertyChanged);
                        degreeControl.DataBindings.Add("Value", _Object, "Degree", false, DataSourceUpdateMode.OnPropertyChanged);
                        attenuationControl.DataBindings.Add("Value", _Object, "Attenuation", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowControl.DataBindings.Add("Value", _Object, "Shadow", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowAttenuationControl.DataBindings.Add("Value", _Object, "ShadowAttenuation", false, DataSourceUpdateMode.OnPropertyChanged);
                        modeComboBox.DataBindings.Add("SelectedItem", _Object, "Mode", false, DataSourceUpdateMode.OnPropertyChanged);
                        selectControl.Asset = _Object.Asset;
                        selectControl.DataBindings.Add("SelectedSurface", _Object, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);
                        originControl.Asset = _Object.Asset;
                        originControl.DataBindings.Add("SelectedSurface", _Object, "Origin", true, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.DataBindings.Add("Surface", _Object, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);
                        var materialControlVisibleBinding = new Binding("Visible", _Object, "Selection", true, DataSourceUpdateMode.Never);
                        materialControlVisibleBinding.Format += MaterialControlVisibleBinding_Format;
                        materialControl.DataBindings.Add(materialControlVisibleBinding);
                    }
                    else
                    {
                        materialControl.Visible = false;
                    }
                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        private void MaterialControlVisibleBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = ((TerrainSurface)e.Value != null);
        }

        public event EventHandler ObjectChanged;

        private AssetSelectViewForm importForm;

        public TerrainSurfaceControl()
        {
            InitializeComponent();

            modeComboBox.DataSource = Enum.GetValues(typeof(TerrainModifyMode));

            importForm = new AssetSelectViewForm
            {
                Multiselect = true,
                Types = new AssetType[]
            {
                AssetType.Material
            }
            };
            Disposed += TerrainSurfaceControl_Disposed;
        }

        private void TerrainSurfaceControl_Disposed(object sender, EventArgs e)
        {
            importForm.Dispose();
        }

        private void MaterialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !materialCheckBox.Checked;
        }

        private void OriginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            surfaceSplitContainer.Panel1Collapsed = !originCheckBox.Checked;

            originControl.SelectedSurface = null;
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Surfaces.IndexOf(selection) + 1;

                    _Object.Asset.Surfaces.Insert(i, new TerrainSurface(_Object.Asset));
                }
                else
                {
                    _Object.Asset.Surfaces.Add(new TerrainSurface(_Object.Asset));
                }
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                if (importForm.ShowDialog(this) == DialogResult.OK)
                {
                    var selection = _Object.Selection;

                    var i = selection != null ? _Object.Asset.Surfaces.IndexOf(selection) + 1 : _Object.Asset.Surfaces.Count;

                    foreach (MaterialAsset origin in importForm.SelectedAssets)
                    {
                        var surface = new TerrainSurface(_Object.Asset);
                        surface.Material.Origin = origin;
                        _Object.Asset.Surfaces.Insert(i, surface);
                        i++;
                    }
                }
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    _Object.Asset.Surfaces.Remove(selection);
                }
            }
        }

        private void MoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Surfaces.IndexOf(selection);

                    if (i > 0)
                    {
                        _Object.Asset.Surfaces.Move(i, i - 1);
                    }
                }
            }
        }

        private void MoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Object != null)
            {
                var selection = _Object.Selection;

                if (selection != null)
                {
                    var i = _Object.Asset.Surfaces.IndexOf(selection);

                    if (i < _Object.Asset.Surfaces.Count - 1)
                    {
                        _Object.Asset.Surfaces.Move(i, i + 1);
                    }
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (_Object != null && _Object.Selection != null)
            {
                _Object.Asset.ClearSurface(_Object.Selection);
            }
        }

        private void FillButton_Click(object sender, EventArgs e)
        {
            if (_Object != null && _Object.Selection != null)
            {
                _Object.Asset.FillSurface(_Object.Selection);
            }
        }
    }
}
