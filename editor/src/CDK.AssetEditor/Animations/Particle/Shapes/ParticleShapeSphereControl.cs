using System;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public partial class ParticleShapeSphereControl : UserControl
    {
        private ParticleShapeSphere _Shape;
        public ParticleShapeSphere Shape
        {
            set
            {
                if (_Shape != value)
                {
                    if (_Shape != null)
                    {
                        rangeControl.DataBindings.Clear();
                    }
                    _Shape = value;
                    if (_Shape != null)
                    {
                        rangeControl.DataBindings.Add("Value", _Shape, "Range", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ShapeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Shape;
        }
        public event EventHandler ShapeChanged;

        public ParticleShapeSphereControl()
        {
            InitializeComponent();
        }
    }
}
