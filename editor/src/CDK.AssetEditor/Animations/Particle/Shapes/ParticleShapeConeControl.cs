using System;
using System.Windows.Forms;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public partial class ParticleShapeConeControl : UserControl
    {
        private ParticleShapeCone _Shape;
        public ParticleShapeCone Shape
        {
            set
            {
                if (_Shape != value)
                {
                    if (_Shape != null)
                    {
                        topRangeControl.DataBindings.Clear();
                        bottomRangeControl.DataBindings.Clear();
                        heightControl.DataBindings.Clear();
                    }
                    _Shape = value;
                    if (_Shape != null)
                    {
                        topRangeControl.DataBindings.Add("Value", _Shape, "TopRange", false, DataSourceUpdateMode.OnPropertyChanged);
                        bottomRangeControl.DataBindings.Add("Value", _Shape, "BottomRange", false, DataSourceUpdateMode.OnPropertyChanged);
                        heightControl.DataBindings.Add("Value", _Shape, "Height", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ShapeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Shape;
        }
        public event EventHandler ShapeChanged;

        public ParticleShapeConeControl()
        {
            InitializeComponent();
        }
    }
}
