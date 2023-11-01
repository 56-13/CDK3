using System;
using System.Windows.Forms;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public partial class ParticleShapeCircleControl : UserControl
    {
        private ParticleShapeCircle _Shape;
        public ParticleShapeCircle Shape
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
                        rangeControl.DataBindings.Add("Vector2", _Shape, "Range", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ShapeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Shape;
        }
        public event EventHandler ShapeChanged;

        public ParticleShapeCircleControl()
        {
            InitializeComponent();
        }
    }
}
