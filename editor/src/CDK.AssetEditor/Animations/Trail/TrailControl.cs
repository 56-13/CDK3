using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Animations.Components;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Trail
{
    public partial class TrailControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private TrailObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TrailObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        sourcesControl.Sources = null;
                        sourcesControl.DataBindings.Clear();

                        distanceControl.DataBindings.Clear();
                        emissionCheckBox.DataBindings.Clear();
                        billboardCheckBox.DataBindings.Clear();
                        localSpaceCheckBox.DataBindings.Clear();

                        emissionPanel.DataBindings.Clear();
                        emissionLifeControl.DataBindings.Clear();
                        emissionSmoothnessControl.DataBindings.Clear();
                        repeatScalePanel.DataBindings.Clear();
                        repeatScaleControl.DataBindings.Clear();

                        colorControl.Color = null;
                        colorDurationControl.DataBindings.Clear();
                        colorLoopControl.DataBindings.Clear();

                        rotationControl.Value = null;
                        rotationDurationControl.DataBindings.Clear();
                        rotationLoopControl.DataBindings.Clear();

                        scaleControl.Value = null;
                        scaleDurationControl.DataBindings.Clear();
                        scaleLoopControl.DataBindings.Clear();
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        var trail = _Object.Origin;

                        sourcesControl.Sources = trail.Sources;
                        sourcesControl.DataBindings.Add("SelectedSource", _Object, "SelectedSource", true, DataSourceUpdateMode.OnPropertyChanged);

                        distanceControl.DataBindings.Add("Value", trail, "Distance", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionCheckBox.DataBindings.Add("Checked", trail, "Emission", false, DataSourceUpdateMode.OnPropertyChanged);
                        billboardCheckBox.DataBindings.Add("Checked", trail, "Billboard", false, DataSourceUpdateMode.OnPropertyChanged);
                        localSpaceCheckBox.DataBindings.Add("Checked", trail, "LocalSpace", false, DataSourceUpdateMode.OnPropertyChanged);

                        emissionPanel.DataBindings.Add("Visible", trail, "Emission", false, DataSourceUpdateMode.Never);
                        emissionLifeControl.DataBindings.Add("Value", trail, "EmissionLife", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionSmoothnessControl.DataBindings.Add("Value", trail, "EmissionSmoothness", true, DataSourceUpdateMode.OnPropertyChanged);
                        repeatScalePanel.DataBindings.Add("Visible", trail, "Repeat", false, DataSourceUpdateMode.Never);
                        repeatScaleControl.DataBindings.Add("Value", trail, "RepeatScale", false, DataSourceUpdateMode.OnPropertyChanged);

                        colorControl.Color = trail.Color;
                        colorDurationControl.DataBindings.Add("Value", trail, "ColorDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        colorLoopControl.DataBindings.Add("Loop", trail, "ColorLoop", false, DataSourceUpdateMode.OnPropertyChanged);

                        rotationControl.Value = trail.Rotation;
                        rotationDurationControl.DataBindings.Add("Value", trail, "RotationDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        rotationLoopControl.DataBindings.Add("Loop", trail, "RotationLoop", false, DataSourceUpdateMode.OnPropertyChanged);

                        scaleControl.Value = trail.Rotation;
                        scaleDurationControl.DataBindings.Add("Value", trail, "ScaleDuration", false, DataSourceUpdateMode.OnPropertyChanged);
                        scaleLoopControl.DataBindings.Add("Loop", trail, "ScaleLoop", false, DataSourceUpdateMode.OnPropertyChanged);
                    }

                    CollapseDefault();

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }
        public event EventHandler ObjectChanged;

        public TrailControl()
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
                        mainPanel.Controls.Remove(colorPanel);
                        mainPanel.Controls.Remove(rotationPanel);
                        mainPanel.Controls.Remove(scalePanel);

                        subPanel.Width = splitContainer.Panel2.Width;       //사이즈변경이 제대로 안됨

                        subPanel.Controls.Add(colorPanel);
                        subPanel.Controls.Add(rotationPanel);
                        subPanel.Controls.Add(scalePanel);
                    }
                    else
                    {
                        subPanel.Controls.Remove(colorPanel);
                        subPanel.Controls.Remove(rotationPanel);
                        subPanel.Controls.Remove(scalePanel);

                        colorPanel.Location = new System.Drawing.Point(0, instancePanel.Bottom);
                        rotationPanel.Location = new System.Drawing.Point(0, colorPanel.Bottom);
                        scalePanel.Location = new System.Drawing.Point(0, rotationPanel.Bottom);

                        mainPanel.Controls.Add(colorPanel);
                        mainPanel.Controls.Add(rotationPanel);
                        mainPanel.Controls.Add(scalePanel);
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

            objectPanel.Collapsed =
            sourcePanel.Collapsed = 
            instancePanel.Collapsed = 
            colorPanel.Collapsed = 
            rotationPanel.Collapsed = 
            scalePanel.Collapsed = true;

            mainPanel.ResumeLayout();
            subPanel.ResumeLayout();
        }

        public void CollapseDefault()
        {
            if (_Object != null)
            {
                mainPanel.SuspendLayout();
                subPanel.SuspendLayout();
                
                objectPanel.Collapsed =
                sourcePanel.Collapsed =
                instancePanel.Collapsed = false;

                var trail = _Object.Origin;

                colorPanel.Collapsed =
                    trail.Color.Type == AnimationColorType.None &&
                    trail.ColorDuration == 0;

                rotationPanel.Collapsed =
                    trail.Rotation.Type == AnimationFloatType.None &&
                    trail.RotationDuration == 0;

                scalePanel.Collapsed =
                    trail.Scale.Type == AnimationFloatType.None &&
                    trail.ScaleDuration == 0;

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }

    }
}
