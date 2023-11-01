using System;
using System.Windows.Forms;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public partial class ParticleShapeRectangleControl : UserControl
    {
        private ParticleShapeRectangle _Shape;
        public ParticleShapeRectangle Shape
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

        public ParticleShapeRectangleControl()
        {
            InitializeComponent();
        }
    }
}
