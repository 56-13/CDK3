using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class LineSpriteElementControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private SpriteElementLine _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementLine Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        position1Control.DataBindings.Clear();
                        x1Control.Value = null;
                        y1Control.Value = null;
                        z1Control.Value = null;
                        position2Control.DataBindings.Clear();
                        x2Control.Value = null;
                        y2Control.Value = null;
                        z2Control.Value = null;
                        materialControl.Material = null;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        position1Control.DataBindings.Add("Value", _Element, "Position1", false, DataSourceUpdateMode.OnPropertyChanged);
                        x1Control.Value = _Element.X0;
                        y1Control.Value = _Element.Y0;
                        z1Control.Value = _Element.Z0;
                        position1Control.DataBindings.Add("Value", _Element, "Position2", false, DataSourceUpdateMode.OnPropertyChanged);
                        x2Control.Value = _Element.X1;
                        y2Control.Value = _Element.Y1;
                        z2Control.Value = _Element.Z1;
                        materialControl.Material = _Element.Material;

                        CollapseDefault();
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public LineSpriteElementControl()
        {
            InitializeComponent();
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

                        materialControl.Location = new System.Drawing.Point(0, positionPanel.Bottom);

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

            positionPanel.Collapsed = true;
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

                positionPanel.Collapsed = false;
                materialControl.CollapseDefault();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
