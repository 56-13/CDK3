using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Sources
{
    public partial class AnimationSourceImageControl : UserControl, ICollapsibleControl
    {
        private AnimationSourceImage _Source;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationSourceImage Source
        {
            set
            {
                if (_Source != value)
                {
                    if (_Source != null)
                    {
                        rootImageControl.DataBindings.Clear();
                        subImageControl.DataBindings.Clear();
                        durationControl.DataBindings.Clear();
                        loopControl.DataBindings.Clear();
                        materialControl.Material = null;
                    }
                    
                    _Source = value;

                    if (_Source != null)
                    {
                        //rootImageControl.DataBindings.Add("RootAsset", _Source, "Project", true, DataSourceUpdateMode.Never);
                        rootImageControl.DataBindings.Add("SelectedAsset", _Source, "RootImage", true, DataSourceUpdateMode.OnPropertyChanged);
                        subImageControl.DataBindings.Add("RootImage", _Source, "RootImage", true, DataSourceUpdateMode.Never);
                        subImageControl.DataBindings.Add("SelectedImages", _Source, "SubImages", true, DataSourceUpdateMode.OnPropertyChanged);
                        durationControl.DataBindings.Add("Value", _Source, "Duration", false, DataSourceUpdateMode.OnPropertyChanged);
                        loopControl.DataBindings.Add("Loop", _Source, "Loop", false, DataSourceUpdateMode.OnPropertyChanged);
                        materialControl.Material = _Source.Material;
                    }

                    SourceChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Source;
        }

        public event EventHandler SourceChanged;

        [DefaultValue(true)]
        public bool UsingDuration
        {
            set => durationPanel.Visible = value;
            get => durationPanel.Visible;
        }

        public AnimationSourceImageControl()
        {
            InitializeComponent();
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }

        public void CollapseAll() => materialControl.CollapseAll();
        public void CollapseDefault() => materialControl.CollapseDefault();
    }
}
