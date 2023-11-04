using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Drawing;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class GradientRectSpriteElementControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private SpriteElementGradientRect _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementGradientRect Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        positionControl.DataBindings.Clear();
                        alignComboBox.DataBindings.Clear();
                        fillCheckBox.DataBindings.Clear();
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        widthControl.Value = null;
                        heightControl.Value = null;
                        color1Control.Color = null;
                        color2Control.Color = null;
                        horizontalRadioButton.DataBindings.Clear();
                        materialControl.Material = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        positionControl.DataBindings.Add("Value", _Element, "Position", false, DataSourceUpdateMode.OnPropertyChanged);
                        alignComboBox.DataBindings.Add("SelectedItem", _Element, "Align", false, DataSourceUpdateMode.OnPropertyChanged);
                        fillCheckBox.DataBindings.Add("Checked", _Element, "Fill", false, DataSourceUpdateMode.OnPropertyChanged);
                        xControl.Value = _Element.X;
                        yControl.Value = _Element.Y;
                        zControl.Value = _Element.Z;
                        widthControl.Value = _Element.Width;
                        heightControl.Value = _Element.Height;
                        color1Control.Color = _Element.Color0;
                        color2Control.Color = _Element.Color1;
                        horizontalRadioButton.DataBindings.Add("Checked", _Element, "Horizontal", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Element.Material;

                        CollapseDefault();
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public GradientRectSpriteElementControl()
        {
            InitializeComponent();

            alignComboBox.DataSource = Enum.GetValues(typeof(Align));
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

                        materialControl.Location = new System.Drawing.Point(0, shapePanel.Bottom);

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

            positionPanel.Collapsed =
            shapePanel.Collapsed = 
            gradientPanel.Collapsed = true;
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

                positionPanel.Collapsed =
                shapePanel.Collapsed =
                gradientPanel.Collapsed = false;
                materialControl.CollapseDefault();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
