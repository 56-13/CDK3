using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;

namespace CDK.Assets.Scenes
{
    public partial class SphereControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private SphereObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SphereObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        radiusControl.DataBindings.Clear();
                        materialControl.Material = null;
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        radiusControl.DataBindings.Add("Value", _Object, "Radius", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Object.Material;

                        CollapseDefault();
                    }

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        public SphereControl()
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

                        materialControl.Location = new System.Drawing.Point(0, objectPanel.Bottom);

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

            objectPanel.Collapsed = true;
            materialControl.CollapseAll();

            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Object != null)
            {
                mainPanel.SuspendLayout();
                subPanel.SuspendLayout();

                objectPanel.Collapsed = false;
                materialControl.CollapseDefault();

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
