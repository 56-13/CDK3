using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using CDK.Assets.Texturing;

namespace CDK.Assets.Meshing
{
    public partial class MeshSelectionMaterialControl : UserControl
    {
        private Material _Material;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Material Material
        {
            set
            {
                if (_Material != value)
                {
                    if (_Material != null)
                    {
                        nameLabel.DataBindings.Clear();
                        assetControl.DataBindings.Clear();
                        editButton.DataBindings.Clear();
                    }

                    _Material = value;

                    if (_Material != null)
                    {
                        nameLabel.DataBindings.Add("Text", _Material, "Name", false, DataSourceUpdateMode.Never);
                        //assetControl.DataBindings.Add("RootAsset", _Material, "Project", true, DataSourceUpdateMode.Never);
                        assetControl.DataBindings.Add("SelectedAsset", _Material, "Origin", true, DataSourceUpdateMode.OnPropertyChanged);

                        var editButtonFontColorBinding = new Binding("Text", _Material, "Local", false, DataSourceUpdateMode.Never);
                        editButtonFontColorBinding.Format += (s, e) =>
                        {
                            e.Value = (bool)e.Value ? "●" : "○";
                        };
                        editButton.DataBindings.Add(editButtonFontColorBinding);
                    }

                    MaterialChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Material;
        }

        public event EventHandler MaterialChanged;

        public event EventHandler MaterialEdit;


        public MeshSelectionMaterialControl()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            MaterialEdit?.Invoke(this, EventArgs.Empty);
        }
    }
}
