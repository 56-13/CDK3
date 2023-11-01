using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Assets.Components;

namespace CDK.Assets.Texturing
{
    public partial class MaterialAssetControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private MaterialAsset _Asset;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MaterialAsset Asset
        {
            set
            {
                if (_Asset != value)
                {
                    if (_Asset != null)
                    {
                        colorCheckBox.DataBindings.Clear();
                        opacityCheckBox.DataBindings.Clear();
                        normalCheckBox.DataBindings.Clear();
                        displacementCheckBox.DataBindings.Clear();
                        metallicCheckBox.DataBindings.Clear();
                        roughnessCheckBox.DataBindings.Clear();
                        ambientOcclusionCheckBox.DataBindings.Clear();
                        emissiveCheckBox.DataBindings.Clear();
                        colorEncodingComboBox.DataBindings.Clear();
                        normalEncodingComboBox.DataBindings.Clear();
                        materialEncodingComboBox.DataBindings.Clear();
                        emissiveEncodingComboBox.DataBindings.Clear();
                        colorReferenceControl.DataBindings.Clear();
                        normalReferenceControl.DataBindings.Clear();
                        materialReferenceControl.DataBindings.Clear();
                        emissiveReferenceControl.DataBindings.Clear();
                    }
                    normalImportForm.NormalPath = null;
                    normalImportForm.DisplacementPath = null;
                    materialImportForm.MetallicPath = null;
                    materialImportForm.RoughnessPath = null;
                    materialImportForm.OcclusionPath = null;

                    _Asset = value;

                    if (_Asset != null)
                    {
                        colorCheckBox.DataBindings.Add("Checked", _Asset, "HasColor", false, DataSourceUpdateMode.Never);
                        opacityCheckBox.DataBindings.Add("Checked", _Asset, "HasOpacity", false, DataSourceUpdateMode.Never);
                        normalCheckBox.DataBindings.Add("Checked", _Asset, "HasNormal", false, DataSourceUpdateMode.Never);
                        displacementCheckBox.DataBindings.Add("Checked", _Asset, "HasDisplacement", false, DataSourceUpdateMode.Never);
                        metallicCheckBox.DataBindings.Add("Checked", _Asset, "HasMetallic", false, DataSourceUpdateMode.Never);
                        roughnessCheckBox.DataBindings.Add("Checked", _Asset, "HasRoughness", false, DataSourceUpdateMode.Never);
                        ambientOcclusionCheckBox.DataBindings.Add("Checked", _Asset, "HasAmbientOcclusion", false, DataSourceUpdateMode.Never);
                        emissiveCheckBox.DataBindings.Add("Checked", _Asset, "HasEmissive", false, DataSourceUpdateMode.Never);

                        colorEncodingComboBox.DataBindings.Add("SelectedItem", _Asset, "ColorEncoding", false, DataSourceUpdateMode.OnPropertyChanged);
                        normalEncodingComboBox.DataBindings.Add("SelectedItem", _Asset, "NormalEncoding", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialEncodingComboBox.DataBindings.Add("SelectedItem", _Asset, "MaterialEncoding", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissiveEncodingComboBox.DataBindings.Add("SelectedItem", _Asset, "EmissiveEncoding", false, DataSourceUpdateMode.OnPropertyChanged);

                        colorReferenceControl.DataBindings.Add("SelectedAsset", _Asset, "ColorReference", true, DataSourceUpdateMode.OnPropertyChanged);
                        normalReferenceControl.DataBindings.Add("SelectedAsset", _Asset, "NormalReference", true, DataSourceUpdateMode.OnPropertyChanged);
                        materialReferenceControl.DataBindings.Add("SelectedAsset", _Asset, "MaterialReference", true, DataSourceUpdateMode.OnPropertyChanged);
                        emissiveReferenceControl.DataBindings.Add("SelectedAsset", _Asset, "EmissiveReference", true, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    descriptionControl.Description = _Asset?.TextureDescription;

                    materialControl.Material = _Asset?.Material;

                    AssetChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Asset;
        }

        public event EventHandler AssetChanged;

        private MaterialAssetColorImportForm colorImportForm;
        private MaterialAssetNormalImportForm normalImportForm;
        private MaterialAssetMaterialImportForm materialImportForm;

        public MaterialAssetControl()
        {
            InitializeComponent();

            var colorEncodings = Enum.GetValues(typeof(TextureAssetColorEncoding));
            var compEncodings = Enum.GetValues(typeof(TextureAssetComponentEncoding));

            colorEncodingComboBox.DataSource = colorEncodings;
            normalEncodingComboBox.DataSource = compEncodings;
            materialEncodingComboBox.DataSource = compEncodings;
            emissiveEncodingComboBox.DataSource = colorEncodings;

            colorImportForm = new MaterialAssetColorImportForm();
            normalImportForm = new MaterialAssetNormalImportForm();
            materialImportForm = new MaterialAssetMaterialImportForm();

            openFileDialog.Filter = FileFilters.Image;

            Disposed += TextureAssetControl_Disposed;
        }

        private void TextureAssetControl_Disposed(object sender, EventArgs e)
        {
            colorImportForm.Dispose();
            normalImportForm.Dispose();
            materialImportForm.Dispose();
        }

        private void ColorImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (colorImportForm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.SetColorMap(colorImportForm.ColorPath, colorImportForm.OpacityPath);
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                    }
                }
            }
        }

        private void ColorClearButton_Click(object sender, EventArgs e)
        {
            _Asset?.SetColorMap(null, null);
        }

        private void NormalImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (normalImportForm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.SetNormalMap(normalImportForm.NormalPath, normalImportForm.DisplacementPath);
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                    }
                }
            }
        }

        private void NormalClearButton_Click(object sender, EventArgs e)
        {
            _Asset?.SetNormalMap(null, null);
        }

        private void MaterialImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (materialImportForm.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.SetMaterialMap(materialImportForm.MetallicPath, materialImportForm.RoughnessPath, materialImportForm.OcclusionPath);
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                    }
                }
            }
        }

        private void MaterialClearButton_Click(object sender, EventArgs e)
        {
            _Asset?.SetMaterialMap(null, null, null);
        }

        private void EmissiveImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _Asset.SetEmissiveMap(openFileDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show(this, "파일을 불러올 수 없습니다.");
                    }
                }
            }
        }

        private void EmissiveClearButton_Click(object sender, EventArgs e)
        {
            _Asset?.SetEmissiveMap(null);
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                openFileDialog.Filter = $"{FileFilters.Archive}|{FileFilters.Image}";

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

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            var action = (Action)(() => { Height = Splitted ? Math.Max(mainPanel.Height, subPanel.Height) : mainPanel.Height; });
            //서브컨트롤 크기변화로 인해 높이가 변화된 후 스크롤바가 생겼다면 서브컨트롤이 스크롤바의 너비에 맞게 조정되지 않음
            //스크롤바에 추가되는 컨트롤에만 이 코드 추가

            if (IsHandleCreated) BeginInvoke(action);
            else action.Invoke();
        }

        public bool Splitted
        {
            set
            {
                if (value != Splitted)
                {
                    mainPanel.SuspendLayout();
                    subPanel.SuspendLayout();

                    splitContainer.Panel2Collapsed = !value;

                    if (value)
                    {
                        mainPanel.Controls.Remove(materialControl);

                        subPanel.Width = splitContainer.Panel2.Width;       //사이즈변경이 제대로 안됨

                        subPanel.Controls.Add(materialControl);
                    }
                    else
                    {
                        subPanel.Controls.Remove(materialControl);

                        materialControl.Location = new System.Drawing.Point(0, descriptionPanel.Bottom);

                        mainPanel.Controls.Add(materialControl);
                    }

                    mainPanel.ResumeLayout();
                    subPanel.ResumeLayout();
                }
            }
            get => !splitContainer.Panel2Collapsed;
        }

        public void CollapseAll()
        {
            mainPanel.SuspendLayout();
            subPanel.SuspendLayout();

            sourcePanel.Collapsed =
            descriptionPanel.Collapsed = true;
            materialControl.CollapseAll();

            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Asset != null)
            {
                mainPanel.SuspendLayout();
                subPanel.SuspendLayout();

                sourcePanel.Collapsed =
                descriptionPanel.Collapsed = false;
                materialControl.CollapseDefault();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
