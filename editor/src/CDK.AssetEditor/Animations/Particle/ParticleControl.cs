using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Assets.Animations.Components;
using CDK.Assets.Animations.Particle.Shapes;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Particle
{
    public partial class ParticleControl : UserControl, ICollapsibleControl, ISplittableControl
    {
        private ParticleObject _Object;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ParticleObject Object
        {
            set
            {
                if (_Object != value)
                {
                    if (_Object != null)
                    {
                        sourcesControl.Sources = null;
                        sourcesControl.DataBindings.Clear();

                        shapeTypeComboBox.DataBindings.Clear();
                        shapeShellCheckBox.DataBindings.Clear();

                        lifeControl.Constant = null;
                        emissionRateControl.DataBindings.Clear();
                        emissionMaxControl.DataBindings.Clear();
                        viewComboBox.DataBindings.Clear();
                        stretchRateLabel.DataBindings.Clear();
                        stretchRateUpDown.DataBindings.Clear();
                        prewarmCheckBox.DataBindings.Clear();
                        localSpaceCheckBox.DataBindings.Clear();
                        clearCheckBox.DataBindings.Clear();
                        finishCheckBox.DataBindings.Clear();

                        colorControl.Color = null;

                        radialControl.Value = null;
                        xControl.Value = null;
                        yControl.Value = null;
                        zControl.Value = null;
                        billboardXControl.Value = null;
                        billboardYControl.Value = null;
                        billboardZControl.Value = null;

                        rotationXControl.Value = null;
                        rotationYControl.Value = null;
                        rotationZControl.Value = null;

                        scaleXControl.Value = null;
                        scaleYControl.Value = null;
                        scaleZControl.Value = null;
                        scaleYControl.DataBindings.Clear();
                        scaleZControl.DataBindings.Clear();
                        scaleEachCheckBox.DataBindings.Clear();

                        _Object.PropertyChanged -= Particle_PropertyChanged;
                    }

                    _Object = value;

                    objectControl.Object = _Object;

                    if (_Object != null)
                    {
                        var particle = _Object.Origin;

                        sourcesControl.Sources = particle.Sources;
                        sourcesControl.DataBindings.Add("SelectedSource", _Object, "SelectedSource", true, DataSourceUpdateMode.OnPropertyChanged);

                        shapeTypeComboBox.DataBindings.Add("SelectedItem", particle, "ShapeType", false, DataSourceUpdateMode.OnPropertyChanged);
                        shapeShellCheckBox.DataBindings.Add("Checked", particle, "ShapeShell", false, DataSourceUpdateMode.OnPropertyChanged);

                        lifeControl.Constant = particle.Life;
                        emissionRateControl.DataBindings.Add("Value", particle, "EmissionRate", false, DataSourceUpdateMode.OnPropertyChanged);
                        emissionMaxControl.DataBindings.Add("Value", particle, "EmissionMax", false, DataSourceUpdateMode.OnPropertyChanged);
                        viewComboBox.DataBindings.Add("SelectedItem", particle, "View", false, DataSourceUpdateMode.OnPropertyChanged);
                        stretchRateUpDown.DataBindings.Add("Value", particle, "StretchRate", false, DataSourceUpdateMode.OnPropertyChanged);
                        var stretchRateLabelBinding = new Binding("Visible", particle, "View", true, DataSourceUpdateMode.Never);
                        stretchRateLabelBinding.Format += StretchRateVisibleBinding_Format;
                        stretchRateLabel.DataBindings.Add(stretchRateLabelBinding);
                        var stretchRateUpDownBinding = new Binding("Visible", particle, "View", true, DataSourceUpdateMode.Never);
                        stretchRateUpDownBinding.Format += StretchRateVisibleBinding_Format;
                        stretchRateUpDown.DataBindings.Add(stretchRateUpDownBinding);
                        prewarmCheckBox.DataBindings.Add("Checked", particle, "Prewarm", false, DataSourceUpdateMode.OnPropertyChanged);
                        localSpaceCheckBox.DataBindings.Add("Checked", particle, "LocalSpace", false, DataSourceUpdateMode.OnPropertyChanged);
                        clearCheckBox.DataBindings.Add("Checked", particle, "Clear", false, DataSourceUpdateMode.OnPropertyChanged);
                        finishCheckBox.DataBindings.Add("Checked", particle, "Finish", false, DataSourceUpdateMode.OnPropertyChanged);

                        colorControl.Color = particle.Color;

                        radialControl.Value = particle.Radial;
                        xControl.Value = particle.X;
                        yControl.Value = particle.Y;
                        zControl.Value = particle.Z;
                        billboardXControl.Value = particle.BillboardX;
                        billboardYControl.Value = particle.BillboardY;
                        billboardZControl.Value = particle.BillboardZ;

                        rotationXControl.Value = particle.RotationX;
                        rotationYControl.Value = particle.RotationY;
                        rotationZControl.Value = particle.RotationZ;

                        scaleXControl.Value = particle.ScaleX;
                        scaleYControl.Value = particle.ScaleY;
                        scaleZControl.Value = particle.ScaleZ;
                        scaleYControl.DataBindings.Add("Visible", particle, "ScaleEach", false, DataSourceUpdateMode.Never);
                        scaleZControl.DataBindings.Add("Visible", particle, "ScaleEach", false, DataSourceUpdateMode.Never);
                        scaleEachCheckBox.DataBindings.Add("Checked", particle, "ScaleEach", false, DataSourceUpdateMode.OnPropertyChanged);

                        ResetShape();

                        CollapseDefault();

                        _Object.PropertyChanged += Particle_PropertyChanged;
                    }

                    ObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Object;
        }

        public event EventHandler ObjectChanged;

        private void StretchRateVisibleBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = (ParticleView)e.Value == ParticleView.StretchBillboard;
        }

        private ParticleShapeSphereControl _shapeSphereControl;
        private ParticleShapeHemisphereControl _shapeHemisphereControl;
        private ParticleShapeConeControl _shapeConeControl;
        private ParticleShapeBoxControl _shapeBoxControl;
        private ParticleShapeCircleControl _shapeCircleControl;
        private ParticleShapeRectangleControl _shapeRectangleControl;

        public ParticleControl()
        {
            InitializeComponent();

            shapeTypeComboBox.DataSource = Enum.GetValues(typeof(ParticleShapeType));
            viewComboBox.DataSource = Enum.GetValues(typeof(ParticleView));

            Disposed += ParticleControl_Disposed;
        }

        private void ParticleControl_Disposed(object sender, EventArgs e)
        {
            _shapeSphereControl?.Dispose();
            _shapeHemisphereControl?.Dispose();
            _shapeConeControl?.Dispose();
            _shapeBoxControl?.Dispose();
            _shapeCircleControl?.Dispose();
            _shapeRectangleControl?.Dispose();

            if (_Object != null)
            {
                _Object.PropertyChanged -= Particle_PropertyChanged;
            }
        }

        private void Particle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShapeType")
            {
                ResetShape();
            }
        }

        private void ResetShape()
        {
            var particle = _Object.Origin;

            Control newControl = null;
            switch (particle.ShapeType)
            {
                case ParticleShapeType.Sphere:
                    if (_shapeSphereControl == null) _shapeSphereControl = new ParticleShapeSphereControl();
                    _shapeSphereControl.Shape = (ParticleShapeSphere)particle.Shape;
                    newControl = _shapeSphereControl;
                    break;
                case ParticleShapeType.Hemisphere:
                    if (_shapeHemisphereControl == null) _shapeHemisphereControl = new ParticleShapeHemisphereControl();
                    _shapeHemisphereControl.Shape = (ParticleShapeHemisphere)particle.Shape;
                    newControl = _shapeHemisphereControl;
                    break;
                case ParticleShapeType.Cone:
                    if (_shapeConeControl == null) _shapeConeControl = new ParticleShapeConeControl();
                    _shapeConeControl.Shape = (ParticleShapeCone)particle.Shape;
                    newControl = _shapeConeControl;
                    break;
                case ParticleShapeType.Box:
                    if (_shapeBoxControl == null) _shapeBoxControl = new ParticleShapeBoxControl();
                    _shapeBoxControl.Shape = (ParticleShapeBox)particle.Shape;
                    newControl = _shapeBoxControl;
                    break;
                case ParticleShapeType.Circle:
                    if (_shapeCircleControl == null) _shapeCircleControl = new ParticleShapeCircleControl();
                    _shapeCircleControl.Shape = (ParticleShapeCircle)particle.Shape;
                    newControl = _shapeCircleControl;
                    break;
                case ParticleShapeType.Rectangle:
                    if (_shapeRectangleControl == null) _shapeRectangleControl = new ParticleShapeRectangleControl();
                    _shapeRectangleControl.Shape = (ParticleShapeRectangle)particle.Shape;
                    newControl = _shapeRectangleControl;
                    break;
            }
            if (shapeControlPanel.Controls.Count == 0 || newControl != shapeControlPanel.Controls[0])
            {
                shapeControlPanel.SuspendLayout();
                shapeControlPanel.Controls.Clear();
                if (newControl != null)
                {
                    shapeControlPanel.Height = newControl.Height;
                    newControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    newControl.Width = shapeControlPanel.Width;
                    shapeControlPanel.Controls.Add(newControl);
                }
                shapeControlPanel.ResumeLayout();

                if (_shapeSphereControl != null && _shapeSphereControl != newControl) _shapeSphereControl.Shape = null;
                if (_shapeHemisphereControl != null && _shapeHemisphereControl != newControl) _shapeHemisphereControl.Shape = null;
                if (_shapeConeControl != null && _shapeConeControl != newControl) _shapeConeControl.Shape = null;
                if (_shapeBoxControl != null && _shapeBoxControl != newControl) _shapeBoxControl.Shape = null;
                if (_shapeCircleControl != null && _shapeCircleControl != newControl) _shapeCircleControl.Shape = null;
                if (_shapeRectangleControl != null && _shapeRectangleControl != newControl) _shapeRectangleControl.Shape = null;
            }
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
                        mainPanel.Controls.Remove(positionPanel);
                        mainPanel.Controls.Remove(rotationPanel);
                        mainPanel.Controls.Remove(scalePanel);

                        subPanel.Width = splitContainer.Panel2.Width;       //사이즈변경이 제대로 안됨

                        subPanel.Controls.Add(colorPanel);
                        subPanel.Controls.Add(positionPanel);
                        subPanel.Controls.Add(rotationPanel);
                        subPanel.Controls.Add(scalePanel);
                    }
                    else
                    {
                        subPanel.Controls.Remove(colorPanel);
                        subPanel.Controls.Remove(positionPanel);
                        subPanel.Controls.Remove(rotationPanel);
                        subPanel.Controls.Remove(scalePanel);

                        colorPanel.Location = new System.Drawing.Point(0, instancePanel.Bottom);
                        positionPanel.Location = new System.Drawing.Point(0, colorPanel.Bottom);
                        rotationPanel.Location = new System.Drawing.Point(0, positionPanel.Bottom);
                        scalePanel.Location = new System.Drawing.Point(0, rotationPanel.Bottom);

                        mainPanel.Controls.Add(colorPanel);
                        mainPanel.Controls.Add(positionPanel);
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
            shapePanel.Collapsed = 
            instancePanel.Collapsed = 
            colorPanel.Collapsed = 
            positionPanel.Collapsed =
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

                objectPanel.Collapsed = false;

                sourcePanel.Collapsed =
                shapePanel.Collapsed =
                instancePanel.Collapsed = false;

                var particle = _Object.Origin;

                colorPanel.Collapsed =
                    particle.Color.Type == AnimationColorType.None;

                positionPanel.Collapsed =
                    particle.Radial.Type == AnimationFloatType.None &&
                    particle.X.Type == AnimationFloatType.None &&
                    particle.Y.Type == AnimationFloatType.None &&
                    particle.Z.Type == AnimationFloatType.None &&
                    particle.BillboardX.Type == AnimationFloatType.None &&
                    particle.BillboardY.Type == AnimationFloatType.None &&
                    particle.BillboardZ.Type == AnimationFloatType.None;

                rotationPanel.Collapsed =
                    particle.RotationX.Type == AnimationFloatType.None &&
                    particle.RotationY.Type == AnimationFloatType.None &&
                    particle.RotationZ.Type == AnimationFloatType.None;

                scalePanel.Collapsed =
                    particle.ScaleX.Type == AnimationFloatType.None &&
                    (!particle.ScaleEach || particle.ScaleY.Type == AnimationFloatType.None) &&
                    (!particle.ScaleEach || particle.ScaleZ.Type == AnimationFloatType.None);

                mainPanel.ResumeLayout();
                subPanel.ResumeLayout();
            }
        }
    }
}
