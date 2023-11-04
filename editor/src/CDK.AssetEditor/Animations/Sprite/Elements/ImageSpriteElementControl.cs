using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Drawing;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class ImageSpriteElementControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private SpriteElementImage _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementImage Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        rootImageControl.DataBindings.Clear();
                        subImageControl.DataBindings.Clear();
                        positionControl.DataBindings.Clear();
                        alignComboBox.DataBindings.Clear();
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        shadowTypeComboBox.DataBindings.Clear();
                        shadowDistanceUpDown.DataBindings.Clear();
                        shadowFlatOffsetControl.DataBindings.Clear();
                        shadowFlatXFlipCheckBox.DataBindings.Clear();
                        shadowFlatYFlipCheckBox.DataBindings.Clear();
                        shadowRotateOffsetControl.DataBindings.Clear();
                        shadowRotateFlatnessUpDown.DataBindings.Clear();
                        materialControl.Material = null;

                        _Element.PropertyChanged -= Element_PropertyChanged;
                        _Element.Image.Material.PropertyChanged -= Material_PropertyChanged;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        //rootImageControl.DataBindings.Add("RootAsset", _Element.Image, "Project", true, DataSourceUpdateMode.Never);
                        rootImageControl.DataBindings.Add("SelectedAsset", _Element.Image, "RootImage", true, DataSourceUpdateMode.OnPropertyChanged);
                        subImageControl.DataBindings.Add("RootImage", _Element.Image, "RootImage", true, DataSourceUpdateMode.Never);
                        subImageControl.DataBindings.Add("SelectedImages", _Element.Image, "SubImages", true, DataSourceUpdateMode.OnPropertyChanged);
                        positionControl.DataBindings.Add("Value", _Element, "Position", false, DataSourceUpdateMode.OnPropertyChanged);
                        alignComboBox.DataBindings.Add("SelectedItem", _Element, "Align", false, DataSourceUpdateMode.OnPropertyChanged);
                        xControl.Value = _Element.X;
                        yControl.Value = _Element.Y;
                        zControl.Value = _Element.Z;
                        shadowTypeComboBox.DataBindings.Add("SelectedItem", _Element, "ShadowType", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowDistanceUpDown.DataBindings.Add("Value", _Element, "ShadowDistance", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowFlatOffsetControl.DataBindings.Add("Value", _Element, "ShadowFlatOffset", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowFlatXFlipCheckBox.DataBindings.Add("Checked", _Element, "ShadowFlatXFlip", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowFlatYFlipCheckBox.DataBindings.Add("Checked", _Element, "ShadowFlatYFlip", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowRotateOffsetControl.DataBindings.Add("Value", _Element, "ShadowRotateOffset", false, DataSourceUpdateMode.OnPropertyChanged);
                        shadowRotateFlatnessUpDown.DataBindings.Add("Value", _Element, "ShadowRotateFlatness", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Element.Image.Material;

                        _Element.PropertyChanged += Element_PropertyChanged;
                        _Element.Image.Material.PropertyChanged += Material_PropertyChanged;

                        ResetShadowView();

                        CollapseDefault();
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public ImageSpriteElementControl()
        {
            InitializeComponent();

            alignComboBox.DataSource = Enum.GetValues(typeof(Align));
            shadowTypeComboBox.DataSource = Enum.GetValues(typeof(ImageSpriteElementShadowType));

            Disposed += ImageSpriteElementControl_Disposed;
        }

        private void ImageSpriteElementControl_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.PropertyChanged -= Element_PropertyChanged;
                _Element.Image.Material.PropertyChanged -= Material_PropertyChanged;
            }
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShadowType")
            {
                ResetShadowView();
            }
        }

        private void Material_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shader") ResetShadowView();
        }

        private void ResetShadowView()
        {
            mainPanel.SuspendLayout();
            subPanel.SuspendLayout();

            shadowPanel.Visible = _Element.Image.Material.Shader == Drawing.MaterialShader.NoLight;

            switch (_Element.ShadowType)
            {
                case ImageSpriteElementShadowType.None:
                    shadowDistancePanel.Visible = false;
                    shadowFlatPanel.Visible = false;
                    shadowRotatePanel.Visible = false;
                    break;
                case ImageSpriteElementShadowType.Flat:
                    shadowDistancePanel.Visible = true;
                    shadowFlatPanel.Visible = true;
                    shadowRotatePanel.Visible = false;
                    break;
                case ImageSpriteElementShadowType.Rotate:
                    shadowDistancePanel.Visible = true;
                    shadowFlatPanel.Visible = false;
                    shadowRotatePanel.Visible = true;
                    break;
            }

            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = Splitted ? Math.Max(mainPanel.Height, subPanel.Height) : mainPanel.Height;
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

                        materialControl.Location = new System.Drawing.Point(0, shadowPanel.Bottom);

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

            imagePanel.Collapsed =
            positionPanel.Collapsed =
            shadowPanel.Collapsed = true;
            materialControl.CollapseAll();

            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Element != null)
            {
                mainPanel.SuspendLayout();
                subPanel.SuspendLayout();

                imagePanel.Collapsed =
                positionPanel.Collapsed = false;
                shadowPanel.Collapsed = _Element.ShadowType == ImageSpriteElementShadowType.None;
                materialControl.CollapseAll();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
