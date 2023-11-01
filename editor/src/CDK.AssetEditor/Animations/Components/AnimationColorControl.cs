using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using CDK.Assets.Components;

namespace CDK.Assets.Animations.Components
{
    public partial class AnimationColorControl : UserControl
    {
        private AnimationColor _Color;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnimationColor Color
        {
            set
            {
                if (_Color != value)
                {
                    if (_Color != null)
                    {
                        typeComboBox.DataBindings.Clear();

                        _Color.PropertyChanged -= Color_PropertyChanged;
                    }

                    _Color = value;

                    if (_Color != null)
                    {
                        typeComboBox.DataBindings.Add("SelectedItem", _Color, "Type", false, DataSourceUpdateMode.OnPropertyChanged);

                        _Color.PropertyChanged += Color_PropertyChanged;
                    }

                    ResetSubControl();

                    ColorChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get => _Color;
        }

        public event EventHandler ColorChanged;

        [DefaultValue("")]
        public string Title
        {
            set => label1.Text = value;
            get => label1.Text;
        }

        private Control _control;

        public AnimationColorControl()
        {
            InitializeComponent();

            typeComboBox.DataSource = Enum.GetValues(typeof(AnimationColorType));

            Disposed += AnimationColorControl_Disposed;
        }

        private void AnimationColorControl_Disposed(object sender, EventArgs e)
        {
            if (_Color != null)
            {
                _Color.PropertyChanged -= Color_PropertyChanged;
            }
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Type")) ResetSubControl();
        }

        private void ResetSubControl()
        {
            Control newControl = null;

            if (_Color != null)
            {
                switch (_Color.Type)
                {
                    case AnimationColorType.Constant:
                        if (_Color.Impl.Normalized)
                        {
                            if (!(_control is ColorControl control)) control = new ColorControl();
                            control.AlphaChannel = _Color.Impl.AlphaChannel;
                            control.DataBindings.Clear();
                            control.DataBindings.Add("Value4", _Color.Impl, "Color", false, DataSourceUpdateMode.OnPropertyChanged);
                            newControl = control;
                        }
                        else
                        {
                            if (!(_control is ColorFControl control)) control = new ColorFControl();
                            control.AlphaChannel = _Color.Impl.AlphaChannel;
                            control.DataBindings.Clear();
                            control.DataBindings.Add("Value4", _Color, "Color", false, DataSourceUpdateMode.OnPropertyChanged);
                            newControl = control;
                        }
                        newControl.Location = new Point(90, 26);
                        break;
                    case AnimationColorType.Linear:
                        if (_Color.Impl.Normalized)
                        {
                            if (!(_control is AnimationColorLinearControl control)) control = new AnimationColorLinearControl();
                            control.Impl = (AnimationColorLinear)_Color.Impl;
                            newControl = control;
                        }
                        else
                        {
                            if (!(_control is AnimationColorFLinearControl control)) control = new AnimationColorFLinearControl();
                            control.Impl = (AnimationColorLinear)_Color.Impl;
                            newControl = control;
                        }
                        newControl.Location = new Point(29, 26);
                        break;
                    case AnimationColorType.Curve:
                        //TODO
                        break;
                    case AnimationColorType.Channel:
                        {
                            if (!(_control is AnimationColorChannelControl control))
                            {
                                control = new AnimationColorChannelControl();
                                control.SizeChanged += ChannelControl_SizeChanged;
                            }
                            control.Impl = (AnimationColorChannel)_Color.Impl;
                            newControl = control;
                        }
                        newControl.Location = new Point(29, 26);
                        break;
                }
            }
            if (_control != newControl)
            {
                SuspendLayout();

                _control?.Dispose();

                if (newControl != null)
                {
                    newControl.Width = Width - newControl.Location.X;
                    newControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Controls.Add(newControl);

                    Height = 26 + newControl.Height;
                }
                else Height = 20;

                _control = newControl;

                ResumeLayout();
            }
        }

        private void ChannelControl_SizeChanged(object sender, EventArgs e)
        {
            var control = (Control)sender;
            Height = 26 + control.Height;
        }
    }
}
