using System;
using System.ComponentModel;
using System.Windows.Forms;

using CDK.Drawing;

namespace CDK.Assets.Animations.Sprite.Elements
{
    public partial class BlurSpriteElementControl : UserControl
    {
        private SpriteElementBlur _Element;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpriteElementBlur Element
        {
            set
            {
                if (_Element != value)
                {
                    if (_Element != null)
                    {
                        blendLayerComboBox.DataBindings.Clear();
                        modeComboBox.DataBindings.Clear();
                        frameCheckBox.DataBindings.Clear();
                        frameControl.DataBindings.Clear();
                        intensityControl.Value = null;
                        depthDistanceControl.Value = null;
                        depthRangeControl.Value = null;
                        directionXControl.Value = null;
                        directionYControl.Value = null;
                        centerXControl.Value = null;
                        centerYControl.Value = null;
                        centerRangeControl.Value = null;

                        _Element.PropertyChanged -= Element_PropertyChanged;
                    }

                    _Element = value;

                    if (_Element != null)
                    {
                        blendLayerComboBox.DataBindings.Add("SelectedItem", _Element, "BlendLayer", false, DataSourceUpdateMode.OnPropertyChanged);
                        modeComboBox.DataBindings.Add("SelectedItem", _Element, "Mode", false, DataSourceUpdateMode.OnPropertyChanged);
                        frameCheckBox.DataBindings.Add("Checked", _Element, "UsingFrame", false, DataSourceUpdateMode.OnPropertyChanged);
                        frameControl.DataBindings.Add("Value", _Element, "Frame", false, DataSourceUpdateMode.OnPropertyChanged);
                        intensityControl.Value = _Element.Intensity;
                        depthDistanceControl.Value = _Element.DepthDistance;
                        depthRangeControl.Value = _Element.DepthRange;
                        directionXControl.Value = _Element.DirectionX;
                        directionYControl.Value = _Element.DirectionY;
                        centerXControl.Value = _Element.CenterX;
                        centerYControl.Value = _Element.CenterY;
                        centerRangeControl.Value = _Element.CenterRange;

                        ResetControlVisible();

                        _Element.PropertyChanged += Element_PropertyChanged;
                    }

                    ElementChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Element;
        }

        public event EventHandler ElementChanged;

        public BlurSpriteElementControl()
        {
            InitializeComponent();

            blendLayerComboBox.DataSource = Enum.GetValues(typeof(InstanceBlendLayer));
            modeComboBox.DataSource = Enum.GetValues(typeof(BlurSpriteMode));

            Disposed += BlurSpriteElementControl_Disposed;
        }

        private void BlurSpriteElementControl_Disposed(object sender, EventArgs e)
        {
            if (_Element != null)
            {
                _Element.PropertyChanged -= Element_PropertyChanged;
            }
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            Height = panel.Height;
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Mode":
                    ResetControlVisible();
                    break;
                case "UsingFrame":
                    frameControl.Visible = _Element.UsingFrame;
                    break;
            }
        }

        private void ResetControlVisible()
        {
            panel.SuspendLayout();

            frameControl.Visible = _Element.UsingFrame;
            intensityControl.Visible = _Element.Mode == BlurSpriteMode.Normal || _Element.Mode == BlurSpriteMode.Depth;
            depthRangeControl.Visible = depthDistanceControl.Visible = _Element.Mode == BlurSpriteMode.Depth;
            directionXControl.Visible = directionYControl.Visible = _Element.Mode == BlurSpriteMode.Direction;
            centerXControl.Visible = centerYControl.Visible = centerRangeControl.Visible = _Element.Mode == BlurSpriteMode.Center;

            panel.ResumeLayout();
        }
    }
}
