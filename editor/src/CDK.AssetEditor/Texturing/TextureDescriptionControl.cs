using System;
using System.Windows.Forms;
using System.ComponentModel;

using CDK.Drawing;

namespace CDK.Assets.Texturing
{
    partial class TextureDescriptionControl : UserControl
    {
        private TextureSlotDescription _Description;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextureSlotDescription Description
        {
            set
            {
                if (_Description != value)
                {
                    if (_Description != null)
                    {
                        wrapSComboBox.DataBindings.Clear();
                        wrapTComboBox.DataBindings.Clear();
                        wrapRComboBox.DataBindings.Clear();
                        minFilterComboBox.DataBindings.Clear();
                        magFilterComboBox.DataBindings.Clear();
                        borderColorControl.DataBindings.Clear();
                        mipmapCountComboBox.DataBindings.Clear();
                    }

                    _Description = value;

                    if (_Description != null)
                    {
                        wrapSComboBox.DataBindings.Add("SelectedItem", _Description, "WrapS", false, DataSourceUpdateMode.OnPropertyChanged);
                        wrapTComboBox.DataBindings.Add("SelectedItem", _Description, "WrapT", false, DataSourceUpdateMode.OnPropertyChanged);
                        wrapRComboBox.DataBindings.Add("SelectedItem", _Description, "WrapR", false, DataSourceUpdateMode.OnPropertyChanged);
                        minFilterComboBox.DataBindings.Add("SelectedItem", _Description, "MinFilter", false, DataSourceUpdateMode.OnPropertyChanged);
                        magFilterComboBox.DataBindings.Add("SelectedItem", _Description, "MagFilter", false, DataSourceUpdateMode.OnPropertyChanged);
                        borderColorControl.DataBindings.Add("Value4", _Description, "BorderColor", false, DataSourceUpdateMode.OnPropertyChanged);
                        mipmapCountComboBox.DataBindings.Add("SelectedItem", _Description, "MipmapCount", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    DescriptionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Description;
        }

        public event EventHandler DescriptionChanged;

        public TextureDescriptionControl()
        {
            InitializeComponent();

            wrapSComboBox.DataSource = Enum.GetValues(typeof(TextureWrapMode));
            wrapTComboBox.DataSource = Enum.GetValues(typeof(TextureWrapMode));
            wrapRComboBox.DataSource = Enum.GetValues(typeof(TextureWrapMode));

            minFilterComboBox.Items.Add(TextureMinFilter.Nearest);
            minFilterComboBox.Items.Add(TextureMinFilter.Linear);

            magFilterComboBox.Items.Add(TextureMagFilter.Nearest);
            magFilterComboBox.Items.Add(TextureMagFilter.Linear);

            for (var mipmapCount = 1; mipmapCount <= TextureSlotDescription.MaxMipmapCount; mipmapCount++)
            {
                mipmapCountComboBox.Items.Add(mipmapCount);
            }
        }

        private void MipmapCountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mipmapCountComboBox.SelectedIndex != 0)
            {
                minFilterComboBox.Items.Add(TextureMinFilter.NearestMipmapNearest);
                minFilterComboBox.Items.Add(TextureMinFilter.LinearMipmapNearest);
                minFilterComboBox.Items.Add(TextureMinFilter.NearestMipmapLinear);
                minFilterComboBox.Items.Add(TextureMinFilter.LinearMipmapLinear);
            }
            else
            {
                minFilterComboBox.Items.Remove(TextureMinFilter.NearestMipmapNearest);
                minFilterComboBox.Items.Remove(TextureMinFilter.LinearMipmapNearest);
                minFilterComboBox.Items.Remove(TextureMinFilter.NearestMipmapLinear);
                minFilterComboBox.Items.Remove(TextureMinFilter.LinearMipmapLinear);
            }
        }
    }
}
