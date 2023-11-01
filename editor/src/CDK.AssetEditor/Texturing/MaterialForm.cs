using System.ComponentModel;

namespace CDK.Assets.Texturing
{
    public partial class MaterialForm : AssetForm
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Material Material
        {
            set => materialControl.Material = value;
            get => materialControl.Material;
        }

        public MaterialForm()
        {
            InitializeComponent();
        }
    }
}
