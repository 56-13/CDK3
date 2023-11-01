using System;
using System.Windows.Forms;

using CDK.Drawing;

namespace CDK.Assets.Animations.Particle.Shapes
{
    public partial class ParticleShapeBoxControl : UserControl
    {
        private ParticleShapeBox _Shape;
        public ParticleShapeBox Shape
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
                        rangeControl.DataBindings.Add("Vector3", _Shape, "Range", false, DataSourceUpdateMode.OnPropertyChanged);
                    }
                    ShapeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Shape;
        }
        public event EventHandler ShapeChanged;

        public ParticleShapeBoxControl()
        {
            InitializeComponent();
        }
    }
}
